#!/usr/bin/env python3
"""
Zostaví video z ilustrácií a audio nahrávky rozprávky.

Kľúčová stratégia: Vypočíta presné časovanie obrázkov na základe
počtu slov v každom segmente textu, aby obrázky zodpovedali
hovorenému textu.

Použitie:
    python scripts/build-video.py --story-dir rozpravky/2026-04-06-odvazny-matej-a-traja-lapajovia
    python scripts/build-video.py --story-dir rozpravky/2026-04-06-odvazny-matej-a-traja-lapajovia --plan-only

Vyžaduje: ffmpeg, ffprobe v PATH

Konfigurácia: Skript hľadá video/segments.json v adresári rozprávky.
Ak neexistuje, automaticky generuje segmenty z audio-text.txt a images/.
"""

import argparse
import json
import os
import re
import subprocess
import sys
from pathlib import Path


def get_audio_duration(audio_path: Path) -> float:
    """Zistí presnú dĺžku audio súboru."""
    result = subprocess.run(
        ["ffprobe", "-v", "quiet", "-show_entries", "format=duration",
         "-of", "csv=p=0", str(audio_path)],
        capture_output=True, text=True
    )
    return float(result.stdout.strip())


def count_words(text: str) -> int:
    """Spočíta slová v texte (bez interpunkcie '...')."""
    clean = re.sub(r'\.{3}', '', text)
    clean = re.sub(r'\s+', ' ', clean).strip()
    return len(clean.split()) if clean else 0


def load_segments_config(story_dir: Path) -> list:
    """Načíta konfiguráciu segmentov z video/segments.json."""
    config_path = story_dir / "video" / "segments.json"
    if not config_path.exists():
        return None
    with open(config_path, "r", encoding="utf-8") as f:
        data = json.load(f)
    return data["segments"]


def auto_generate_segments(text: str, images_dir: Path) -> list:
    """Automaticky vygeneruje segmenty z textu a dostupných obrázkov."""
    # Nájdi dostupné scény
    scene_files = sorted([f.name for f in images_dir.glob("scene-*.png")])
    has_cover = (images_dir / "cover-16x9.png").exists()

    # Rozdeľ text na časti podľa "..." páuz
    chunks = re.split(r'\.\.\.\s*', text)
    chunks = [c.strip() for c in chunks if c.strip()]

    if not chunks:
        print("❌ Žiadny text v audio-text.txt")
        sys.exit(1)

    # Rozdeľ chunks rovnomerne medzi obrázky
    n_images = len(scene_files)
    n_chunks = len(chunks)
    chunks_per_image = max(1, n_chunks // n_images)

    segments = []

    # Titulná karta (cover)
    if has_cover:
        segments.append({
            "name": "title",
            "image": "cover-16x9.png",
            "text_start": chunks[0][:30],
            "text_end": chunks[0][:30],
            "forced_duration": 4.0
        })

    # Rozdeľ chunks medzi scény
    chunk_idx = 0
    for i, scene_file in enumerate(scene_files):
        start_idx = chunk_idx
        if i < n_images - 1:
            end_idx = min(start_idx + chunks_per_image, n_chunks - 1)
        else:
            end_idx = n_chunks - 1

        seg_text_start = chunks[start_idx][:40] if start_idx < n_chunks else ""
        seg_text_end = chunks[end_idx][-40:] if end_idx < n_chunks else ""

        segments.append({
            "name": scene_file.replace(".png", ""),
            "image": scene_file,
            "text_start": seg_text_start,
            "text_end": seg_text_end
        })
        chunk_idx = end_idx + 1

    return segments


def calculate_timeline(text: str, segments: list, total_duration: float) -> list:
    """Vypočíta časovú os pre obrázky na základe textu a konfigurácie."""

    # Pre každý segment nájdi zodpovedajúci text a spočítaj slová
    timeline = []
    for seg in segments:
        start_marker = seg["text_start"]
        end_marker = seg["text_end"]

        si = text.find(start_marker)
        if si < 0:
            print(f"  ⚠️  Segment '{seg['name']}': start marker nenájdený: '{start_marker[:50]}'")
            si = 0

        ei = text.find(end_marker, si)
        if ei < 0:
            ei = si + len(start_marker)
        else:
            ei += len(end_marker)

        seg_text = text[si:ei]
        wc = count_words(seg_text)

        # Pre titulnú kartu a morál nastav minimum slov
        if seg.get("forced_duration"):
            wc = max(wc, 1)

        timeline.append({
            "name": seg["name"],
            "image": seg["image"],
            "words": wc,
            "forced_duration": seg.get("forced_duration"),
        })

    # Odpočítaj fixné trvania z celkového času
    forced_total = sum(t["forced_duration"] for t in timeline if t["forced_duration"])
    remaining_duration = total_duration - forced_total

    # Celkový počet slov v nefixných segmentoch
    variable_words = sum(t["words"] for t in timeline if not t["forced_duration"])
    if variable_words == 0:
        variable_words = 1

    # Vypočítaj trvania
    t = 0.0
    for entry in timeline:
        if entry["forced_duration"]:
            dur = entry["forced_duration"]
        else:
            dur = (entry["words"] / variable_words) * remaining_duration
            dur = max(dur, 3.0)  # Minimum 3 sekundy

        entry["start"] = round(t, 2)
        entry["duration"] = round(dur, 2)
        entry["end"] = round(t + dur, 2)
        t += dur

    # Škáluj aby presne sedel na celkový čas
    if timeline:
        scale = total_duration / t if t > 0 else 1.0
        t = 0.0
        for entry in timeline:
            if entry.get("forced_duration"):
                entry["start"] = round(t, 2)
                entry["end"] = round(t + entry["duration"], 2)
                t += entry["duration"]
            else:
                dur = entry["duration"] * scale
                entry["start"] = round(t, 2)
                entry["duration"] = round(dur, 2)
                entry["end"] = round(t + dur, 2)
                t += dur

        # Posledný segment presne na koniec
        timeline[-1]["end"] = total_duration
        timeline[-1]["duration"] = round(total_duration - timeline[-1]["start"], 2)

    return timeline


def build_ffmpeg_command(timeline: list, images_dir: Path, audio_path: Path,
                         output_path: Path, total_duration: float) -> list:
    """Zostaví FFmpeg príkaz pre slideshow video s crossfade prechodmi."""

    inputs = []
    filter_parts = []
    concat_inputs = []

    for i, seg in enumerate(timeline):
        img_path = images_dir / seg["image"]

        # Display duration = od začiatku tohto segmentu po začiatok ďalšieho
        if i < len(timeline) - 1:
            display_dur = round(timeline[i + 1]["start"] - seg["start"], 3)
        else:
            display_dur = round(total_duration - seg["start"], 3)

        display_dur = max(display_dur, 0.1)

        inputs.extend(["-loop", "1", "-t", str(display_dur), "-i", str(img_path)])

        # Škáluj na 1920x1080, pridaj fade-in/fade-out
        fade_dur = 0.8
        fade_out_start = max(0, display_dur - fade_dur)

        scale = f"[{i}:v]scale=1920:1080:force_original_aspect_ratio=decrease,pad=1920:1080:(ow-iw)/2:(oh-ih)/2:color=white,setsar=1"

        if i == 0:
            scale += f",fade=t=out:st={fade_out_start}:d={fade_dur}"
        elif i == len(timeline) - 1:
            scale += f",fade=t=in:st=0:d={fade_dur}"
        else:
            scale += f",fade=t=in:st=0:d={fade_dur},fade=t=out:st={fade_out_start}:d={fade_dur}"

        scale += f"[v{i}]"
        filter_parts.append(scale)
        concat_inputs.append(f"[v{i}]")

    # Concat filter
    n = len(timeline)
    concat = "".join(concat_inputs) + f"concat=n={n}:v=1:a=0[outv]"
    filter_parts.append(concat)

    # Audio input
    audio_idx = n
    inputs.extend(["-i", str(audio_path)])

    filter_complex = ";".join(filter_parts)

    cmd = ["ffmpeg", "-y"]
    cmd.extend(inputs)
    cmd.extend([
        "-filter_complex", filter_complex,
        "-map", "[outv]",
        "-map", f"{audio_idx}:a",
        "-c:v", "libx264",
        "-crf", "18",
        "-preset", "slow",
        "-pix_fmt", "yuv420p",
        "-c:a", "aac",
        "-b:a", "192k",
        "-movflags", "+faststart",
        str(output_path)
    ])

    return cmd


def main():
    parser = argparse.ArgumentParser(description="Zostaví video rozprávky")
    parser.add_argument("--story-dir", required=True, help="Cesta k adresáru rozprávky")
    parser.add_argument("--plan-only", action="store_true", help="Len vypíš plán, nevytvár video")
    parser.add_argument("--output", help="Výstupný súbor (default: video/rozpravka.mp4)")
    args = parser.parse_args()

    story_dir = Path(args.story_dir)
    images_dir = story_dir / "images"
    audio_path = story_dir / "audio" / "rozpravka.mp3"
    video_dir = story_dir / "video"
    video_dir.mkdir(parents=True, exist_ok=True)

    output_path = Path(args.output) if args.output else video_dir / "rozpravka.mp4"

    # Načítaj audio dĺžku
    print(f"🎵 Audio: {audio_path}")
    total_duration = get_audio_duration(audio_path)
    print(f"   Dĺžka: {total_duration:.2f}s ({int(total_duration//60)}:{total_duration%60:05.2f})")

    # Načítaj text pre výpočet časovania
    text_path = story_dir / "audio-text.txt"
    if not text_path.exists():
        text_path = story_dir / "audio" / "clean-text.txt"
    with open(text_path, "r", encoding="utf-8-sig") as f:
        text = f.read()

    # Načítaj konfiguráciu segmentov
    segments = load_segments_config(story_dir)
    if segments:
        print(f"📋 Načítaná konfigurácia: video/segments.json ({len(segments)} segmentov)")
    else:
        print(f"📋 Automatická segmentácia z obrázkov")
        segments = auto_generate_segments(text, images_dir)

    # Vypočítaj časovú os
    timeline = calculate_timeline(text, segments, total_duration)

    # Vypíš plán
    print(f"\n📋 Assembly Plan — {len(timeline)} segmentov\n")
    header = f"{'#':>2} {'Obrázok':<16} {'Začiatok':>8} {'Koniec':>8} {'Trvanie':>8} {'Slov':>5}"
    print(header)
    print("-" * len(header))

    for i, seg in enumerate(timeline):
        sm = f"{int(seg['start']//60)}:{seg['start']%60:05.2f}"
        em = f"{int(seg['end']//60)}:{seg['end']%60:05.2f}"
        print(f"{i+1:>2} {seg['image']:<16} {sm:>8} {em:>8} {seg['duration']:>7.1f}s {seg['words']:>5}")

    # Ulož plán ako JSON
    plan_path = video_dir / "assembly-plan.json"
    with open(plan_path, "w", encoding="utf-8") as f:
        json.dump({"total_duration": total_duration, "segments": timeline}, f, indent=2, ensure_ascii=False)
    print(f"\n💾 Plán uložený: {plan_path}")

    if args.plan_only:
        return

    # Skontroluj, že všetky obrázky existujú
    missing = []
    for seg in timeline:
        img_path = images_dir / seg["image"]
        if not img_path.exists():
            missing.append(str(img_path))

    if missing:
        print(f"\n❌ Chýbajúce obrázky:")
        for m in missing:
            print(f"   {m}")
        sys.exit(1)

    # Zostav FFmpeg príkaz
    print(f"\n🎬 Zostavujem video: {output_path}")
    cmd = build_ffmpeg_command(timeline, images_dir, audio_path, output_path, total_duration)

    # Spusti FFmpeg
    print(f"   FFmpeg príkaz: {len(cmd)} argumentov")
    print(f"   Toto môže trvať niekoľko minút...\n")

    result = subprocess.run(cmd, capture_output=True, text=True)

    if result.returncode != 0:
        print(f"❌ FFmpeg chyba (exit code {result.returncode}):")
        stderr_lines = result.stderr.strip().split("\n")
        for line in stderr_lines[-30:]:
            print(f"   {line}")
        sys.exit(1)

    # Skontroluj výstup
    if output_path.exists():
        size_mb = output_path.stat().st_size / (1024 * 1024)
        out_dur = get_audio_duration(output_path)
        print(f"\n✅ Video vytvorené!")
        print(f"   Súbor: {output_path}")
        print(f"   Veľkosť: {size_mb:.1f} MB")
        print(f"   Trvanie: {out_dur:.2f}s ({int(out_dur//60)}:{out_dur%60:05.2f})")
    else:
        print(f"\n❌ Výstupný súbor nebol vytvorený!")
        sys.exit(1)


if __name__ == "__main__":
    main()
