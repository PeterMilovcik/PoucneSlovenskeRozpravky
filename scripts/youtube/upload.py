#!/usr/bin/env python3
"""
YouTube Video Upload Script for Poučné Slovenské Rozprávky.

Usage:
    python upload.py --metadata <youtube-metadata.json> --video <video-file.mp4>

Requires:
    pip install google-auth-oauthlib google-api-python-client
    
Setup:
    1. Place client_secret.json in this directory (from Google Cloud Console)
    2. First run will open browser for OAuth authorization
    3. Token is saved to token.json for subsequent runs
"""

import argparse
import json
import os
import sys
from pathlib import Path

# Google API imports
try:
    from google.oauth2.credentials import Credentials
    from google_auth_oauthlib.flow import InstalledAppFlow
    from google.auth.transport.requests import Request
    from googleapiclient.discovery import build
    from googleapiclient.http import MediaFileUpload
except ImportError:
    print("Chýbajú závislosti. Spusti: pip install google-auth-oauthlib google-api-python-client")
    sys.exit(1)

SCOPES = ["https://www.googleapis.com/auth/youtube.upload"]
SCRIPT_DIR = Path(__file__).parent
CLIENT_SECRET = SCRIPT_DIR / "client_secret.json"
TOKEN_FILE = SCRIPT_DIR / "token.json"


def get_authenticated_service():
    """Authenticate and return YouTube API service."""
    creds = None
    
    if TOKEN_FILE.exists():
        creds = Credentials.from_authorized_user_file(str(TOKEN_FILE), SCOPES)
    
    if not creds or not creds.valid:
        if creds and creds.expired and creds.refresh_token:
            creds.refresh(Request())
        else:
            if not CLIENT_SECRET.exists():
                print(f"CHYBA: Súbor {CLIENT_SECRET} neexistuje!")
                print("Stiahni ho z Google Cloud Console → APIs & Services → Credentials")
                sys.exit(1)
            flow = InstalledAppFlow.from_client_secrets_file(str(CLIENT_SECRET), SCOPES)
            creds = flow.run_local_server(port=0)
        
        TOKEN_FILE.write_text(creds.to_json())
        print(f"Token uložený do {TOKEN_FILE}")
    
    return build("youtube", "v3", credentials=creds)


def upload_video(youtube, video_path: str, metadata: dict):
    """Upload video to YouTube with metadata."""
    
    body = {
        "snippet": {
            "title": metadata.get("title", "Poučná Slovenská Rozprávka"),
            "description": metadata.get("description", ""),
            "tags": metadata.get("tags", []),
            "categoryId": "27" if metadata.get("category") == "Education" else "24",
            "defaultLanguage": metadata.get("language", "sk"),
            "defaultAudioLanguage": metadata.get("language", "sk"),
        },
        "status": {
            "privacyStatus": metadata.get("visibility", "private"),
            "madeForKids": metadata.get("made_for_kids", True),
            "selfDeclaredMadeForKids": metadata.get("made_for_kids", True),
        },
    }
    
    media = MediaFileUpload(
        video_path,
        mimetype="video/mp4",
        resumable=True,
        chunksize=10 * 1024 * 1024,  # 10 MB chunks
    )
    
    request = youtube.videos().insert(
        part="snippet,status",
        body=body,
        media_body=media,
    )
    
    print(f"Nahrávam: {video_path}")
    print(f"Názov: {body['snippet']['title']}")
    
    response = None
    retries = 0
    max_retries = 5
    while response is None:
        try:
            status, response = request.next_chunk()
            if status:
                progress = int(status.progress() * 100)
                print(f"  Nahrané: {progress}%")
            retries = 0  # Reset on success
        except Exception as e:
            retries += 1
            if retries > max_retries:
                raise
            wait = min(2 ** retries, 60)
            print(f"  ⚠️ Chyba: {e}")
            print(f"  Opakujem za {wait}s... (pokus {retries}/{max_retries})")
            import time
            time.sleep(wait)
    
    video_id = response["id"]
    video_url = f"https://www.youtube.com/watch?v={video_id}"
    print(f"\n✅ Video nahrané!")
    print(f"   URL: {video_url}")
    print(f"   ID: {video_id}")
    
    # Set thumbnail if available
    thumbnail = metadata.get("thumbnail")
    if thumbnail and Path(thumbnail).exists():
        try:
            youtube.thumbnails().set(
                videoId=video_id,
                media_body=MediaFileUpload(thumbnail, mimetype="image/png"),
            ).execute()
            print(f"   Thumbnail nastavený: {thumbnail}")
        except Exception as e:
            print(f"   ⚠️ Thumbnail sa nepodarilo nastaviť: {e}")
    
    return video_id, video_url


def main():
    parser = argparse.ArgumentParser(description="Upload rozprávky na YouTube")
    parser.add_argument("--metadata", required=True, help="Cesta k youtube-metadata.json")
    parser.add_argument("--video", required=True, help="Cesta k video súboru (.mp4)")
    args = parser.parse_args()
    
    # Load metadata
    with open(args.metadata, "r", encoding="utf-8") as f:
        metadata = json.load(f)
    
    # Check video exists
    if not Path(args.video).exists():
        print(f"CHYBA: Video súbor neexistuje: {args.video}")
        sys.exit(1)
    
    # Resolve thumbnail path relative to metadata file
    if "thumbnail" in metadata:
        meta_dir = Path(args.metadata).parent
        thumb_path = meta_dir / metadata["thumbnail"]
        if thumb_path.exists():
            metadata["thumbnail"] = str(thumb_path)
    
    # Upload
    youtube = get_authenticated_service()
    video_id, video_url = upload_video(youtube, args.video, metadata)
    
    # Save result
    result = {"video_id": video_id, "video_url": video_url}
    result_file = Path(args.metadata).parent / "youtube-result.json"
    with open(result_file, "w", encoding="utf-8") as f:
        json.dump(result, f, indent=2, ensure_ascii=False)
    print(f"\nVýsledok uložený do: {result_file}")


if __name__ == "__main__":
    main()
