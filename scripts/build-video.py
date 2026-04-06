#!/usr/bin/env python3
"""
Zostaví video z ilustrácií a audio nahrávky rozprávky.

Kľúčová stratégia: Vypočíta presné časovanie obrázkov na základe
počtu slov v každom segmente textu, aby obrázky zodpovedali
hovorenému textu.

Použitie:
    python scripts/build-video.py --story-dir rozpravky/2026-04-05-tomasova-zlata-minca
    python scripts/build-video.py --story-dir rozpravky/2026-04-05-tomasova-zlata-minca --plan-only

Vyžaduje: ffmpeg, ffprobe v PATH
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


def calculate_timeline(text: str, total_duration: float) -> list:
    """Vypočíta časovú os pre obrázky na základe textu."""

    # Definícia segmentov: (názov, začiatok textu, koniec textu, obrázok)
    segments = [
        ("title",    "Tomášova zlatá minca.",    "Tomášova zlatá minca.", "cover-16x9.png"),
        ("scene-01", "Kde bolo, tam bolo",       "staré?",                "scene-01.png"),
        ("scene-02", "V ten piatkový deň",       "cez druhého.",          "scene-02.png"),
        ("scene-03", "Kubko sa otočil",           "nič nepovedal.",        "scene-03.png"),
        ("scene-04", "Na druhý deň šiel",        "vyrezávaní.",           "scene-04.png"),
        ("scene-05", "Na povale bolo prašno",     "nerozumel.",            "scene-05.png"),
        ("scene-06", "Keď ju zdvihol",            "čo chceš.",             "scene-06.png"),
        ("scene-07", "Tomáš nečakal ani deň",     "sedadlo.",              "scene-07.png"),
        ("scene-08", "Práve vtedy prišiel Kubko", "najlepší bicykel.",     "scene-08.png"),
        ("scene-09", "Večer doma sa Tomáš",       "zastavil.",             "scene-09.png"),
        ("scene-10", "Na mieste, kde vždy",       "to ticho bolo prázdne.", "scene-10.png"),
        ("scene-11", "O týždeň mal Tomáš",        "čo robiť.",            "scene-11.png"),
        ("scene-12", "A hádajte, kto vtedy",      "všetko jasne.",         "scene-12.png"),
        ("scene-13", "Tomáš bežal.",              "teplo pri srdci.",      "scene-13.png"),
        ("scene-14", "Zišiel dole do dielne",     "žijú šťastne dodnes.", "scene-14.png"),
        ("moral",    "Poučenie.",                 "na svete.",             "cover-16x9.png"),
    ]

    # Spočítaj slová v každom segmente
    word_counts = []
    for name, start_m, end_m, img in segments:
        si = text.find(start_m)
        ei = text.find(end_m, si) + len(end_m)
        seg_text = text[si:ei]
        wc = count_words(seg_text)
        word_counts.append(wc)

    total_words = sum(word_counts)

    # Rozpočet páuz (8 veľkých prechodov medzi scénami × 1.5s)
    pause_budget = 8 * 1.5
    spoken_budget = total_duration - pause_budget

    # Proporcionálne rozdelenie
    raw_durations = [(wc / total_words) * spoken_budget for wc in word_counts]

    # Titulná karta minimálne 5 sekúnd
    raw_durations[0] = max(raw_durations[0], 5.0)

    # Škáluj na presný celkový čas
    scale = (total_duration - pause_budget) / sum(raw_durations)
    durations = [d * scale for d in raw_durations]

    # Indexy kde sú pauzy (pred týmito segmentmi)
    # Zodpovedá "..." značkám v clean-text.txt
    pause_before = {1, 3, 4, 7, 9, 11, 13, 15}

    # Zostav časovú os
    timeline = []
    t = 0.0

    for i in range(len(segments)):
        name, _, _, img = segments[i]

        if i in pause_before:
            t += 1.5

        start = t
        end = t + durations[i]

        timeline.append({
            "name": name,
            "image": img,
            "start": round(start, 2),
            "end": round(end, 2),
            "duration": round(durations[i], 2),
            "words": word_counts[i],
        })
        t = end

    # Posledný segment presne na koniec audia
    timeline[-1]["end"] = total_duration
    timeline[-1]["duration"] = round(total_duration - timeline[-1]["start"], 2)

    return timeline


def build_ffmpeg_command(timeline: list, images_dir: Path, audio_path: Path,
                         output_path: Path, total_duration: float) -> list:
    """Zostaví FFmpeg príkaz pre slideshow video s crossfade prechodmi."""

    # Krok 1: Najprv preškáluj obrázky na 1920x1080
    # Krok 2: Zostav video z obrázkov s presným časovaním

    # Použi concat demuxer pre najpresnejšie časovanie
    # Najprv vytvor krátke video klipy pre každý obrázok, potom ich spoj

    inputs = []
    filter_parts = []
    concat_inputs = []

    for i, seg in enumerate(timeline):
        img_path = images_dir / seg["image"]

        # Display duration includes trailing pause (gap to next segment)
        if i < len(timeline) - 1:
            display_dur = round(timeline[i + 1]["start"] - seg["start"], 3)
        else:
            display_dur = round(total_duration - seg["start"], 3)

        inputs.extend(["-loop", "1", "-t", str(display_dur), "-i", str(img_path)])

        # Škáluj na 1920x1080, pridaj fade-in/fade-out pre crossfade efekt
        fade_dur = 0.8
        fade_out_start = max(0, display_dur - fade_dur)

        scale = f"[{i}:v]scale=1920:1080:force_original_aspect_ratio=decrease,pad=1920:1080:(ow-iw)/2:(oh-ih)/2:color=white,setsar=1"

        # Fade-in pre všetky okrem prvého, fade-out pre všetky okrem posledného
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
    text_path = story_dir / "audio" / "clean-text.txt"
    with open(text_path, "r", encoding="utf-8-sig") as f:
        text = f.read()

    # Vypočítaj časovú os
    timeline = calculate_timeline(text, total_duration)

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
        # Show last 30 lines of stderr
        stderr_lines = result.stderr.strip().split("\n")
        for line in stderr_lines[-30:]:
            print(f"   {line}")
        sys.exit(1)

    # Skontroluj výstup
    if output_path.exists():
        size_mb = output_path.stat().st_size / (1024 * 1024)
        # Get output duration
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
