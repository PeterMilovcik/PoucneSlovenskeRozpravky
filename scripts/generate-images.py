#!/usr/bin/env python3
"""
Generátor ilustrácií pre rozprávky — používa GPT Image (gpt-image-1) cez Responses API.

Kľúčová stratégia: Vygeneruje cover obrázok ako prvý, potom ho použije
ako referenčný obrázok pre všetky scény → konzistentný štýl a postavy.

Použitie:
    python scripts/generate-images.py --story-dir rozpravky/2026-04-05-tomasova-zlata-minca
    python scripts/generate-images.py --story-dir rozpravky/2026-04-05-tomasova-zlata-minca --scene 5
    python scripts/generate-images.py --story-dir rozpravky/2026-04-05-tomasova-zlata-minca --all

Vyžaduje:
    pip install openai
    export OPENAI_API_KEY=your-key
"""

import argparse
import base64
import json
import os
import re
import sys
import time
from pathlib import Path

try:
    from openai import OpenAI
    HAS_OPENAI = True
except ImportError:
    HAS_OPENAI = False


def parse_prompts_md(prompts_path: Path) -> dict:
    """Parsuje prompts.md a extrahuje prompty pre obálku a scény."""
    text = prompts_path.read_text(encoding="utf-8")

    result = {"cover": None, "scenes": {}}

    cover_match = re.search(
        r"## Obálka.*?\*\*Prompt\*\*:\s*\n(.*?)(?=\n\*\*Výsledok\*\*|\n---|\n## Scéna|\Z)",
        text, re.DOTALL
    )
    if cover_match:
        result["cover"] = cover_match.group(1).strip()

    scene_pattern = re.compile(
        r"## Scéna (\d+):.*?\*\*Prompt\*\*:\s*\n(.*?)(?=\n\*\*Výsledok\*\*|\n---|\n## Scéna|\Z)",
        re.DOTALL
    )
    for match in scene_pattern.finditer(text):
        scene_num = int(match.group(1))
        prompt = match.group(2).strip()
        result["scenes"][scene_num] = prompt

    return result


def encode_image_b64(path: Path) -> str:
    """Načíta obrázok a vráti base64 string."""
    return base64.b64encode(path.read_bytes()).decode("utf-8")


def generate_with_reference(client, prompt: str, reference_path: Path = None,
                            size: str = "1536x1024", quality: str = "high") -> bytes:
    """Vygeneruje obrázok cez GPT Image API s voliteľným referenčným obrázkom.

    Stratégia:
    - Bez referencie: Image API generations (pre cover)
    - S referenciou: Image API edits (pre scény — cover ako štýlová referencia)
    """

    if reference_path and reference_path.exists():
        # Použi edits endpoint s cover ako referenčným obrázkom
        style_prefix = (
            "Generate a NEW children's book illustration in EXACTLY the same soft watercolor style "
            "as the reference image. Match the art style, color palette, warm golden tones, "
            "gentle brushstrokes, and character proportions from the reference. "
            "The main boy character must look identical to the boy in the reference — "
            "same wavy brown hair, blue eyes, face shape, blue-and-white striped t-shirt. "
            "No text, no letters, no words anywhere in the image.\n\n"
        )

        print(f"  ⏳ Generujem cez GPT Image edits (s referenciou, quality={quality})...")
        with open(reference_path, "rb") as ref_file:
            response = client.images.edit(
                model="gpt-image-1",
                image=ref_file,
                prompt=style_prefix + prompt,
                n=1,
                size=size,
                quality=quality,
            )
    else:
        # Bez referencie — štandardné generovanie (pre cover)
        print(f"  ⏳ Generujem cez GPT Image generate (bez referencie, quality={quality})...")
        response = client.images.generate(
            model="gpt-image-1",
            prompt=prompt,
            n=1,
            size=size,
            quality=quality,
        )

    img = response.data[0]
    if hasattr(img, 'b64_json') and img.b64_json:
        return base64.b64decode(img.b64_json)
    elif hasattr(img, 'url') and img.url:
        # Fallback: stiahni z URL
        import ssl
        import urllib.request
        ctx = ssl.create_default_context()
        ctx.check_hostname = False
        ctx.verify_mode = ssl.CERT_NONE
        req = urllib.request.Request(img.url)
        with urllib.request.urlopen(req, timeout=60, context=ctx) as resp:
            return resp.read()
    else:
        raise RuntimeError("Žiadne dáta obrázka v odpovedi.")


def generate_dalle3(client, prompt: str, size: str = "1792x1024") -> bytes:
    """Fallback: Generuje cez DALL-E 3 Image API (bez referencie)."""
    import ssl
    import urllib.request

    print("  ⏳ Generujem cez DALL-E 3 (fallback)...")
    response = client.images.generate(
        model="dall-e-3",
        prompt=prompt,
        n=1,
        size=size,
        quality="hd",
        response_format="url"
    )
    image_url = response.data[0].url

    ctx = ssl.create_default_context()
    ctx.check_hostname = False
    ctx.verify_mode = ssl.CERT_NONE
    req = urllib.request.Request(image_url)
    with urllib.request.urlopen(req, timeout=60, context=ctx) as resp:
        return resp.read()


def main():
    parser = argparse.ArgumentParser(description="Generátor ilustrácií pre rozprávky")
    parser.add_argument("--story-dir", required=True, help="Cesta k adresáru rozprávky")
    parser.add_argument("--scene", type=int, action="append", help="Číslo scény (možno opakovať)")
    parser.add_argument("--cover-only", action="store_true", help="Generuj len obálku")
    parser.add_argument("--all", action="store_true", help="Generuj všetky scény aj obálku")
    parser.add_argument("--dry-run", action="store_true", help="Len zobraz prompty, negeneruj")
    parser.add_argument("--model", default="gpt-image", choices=["gpt-image", "dall-e-3"],
                        help="Model na generovanie (default: gpt-image)")
    parser.add_argument("--quality", default="high", choices=["low", "medium", "high"],
                        help="Kvalita obrázkov (default: high)")
    parser.add_argument("--no-reference", action="store_true",
                        help="Negeneruj s referenčným obrázkom")
    args = parser.parse_args()

    api_key = os.environ.get("OPENAI_API_KEY")
    if not api_key and not args.dry_run:
        print("❌ OPENAI_API_KEY nie je nastavený!")
        sys.exit(1)

    if not HAS_OPENAI and not args.dry_run:
        print("❌ openai knižnica nie je nainštalovaná! Spusti: pip install openai")
        sys.exit(1)

    client = None
    if not args.dry_run:
        import httpx
        client = OpenAI(api_key=api_key, http_client=httpx.Client(verify=False))

    story_dir = Path(args.story_dir)
    images_dir = story_dir / "images"
    prompts_path = images_dir / "prompts.md"

    if not prompts_path.exists():
        print(f"❌ Súbor {prompts_path} neexistuje!")
        sys.exit(1)

    print(f"📖 Načítavam prompty z {prompts_path}")
    prompts = parse_prompts_md(prompts_path)
    print(f"   Obálka: {'✅' if prompts['cover'] else '❌'}")
    print(f"   Scény: {sorted(prompts['scenes'].keys())}")
    print(f"   Model: {'GPT Image (gpt-image-1)' if args.model == 'gpt-image' else 'DALL-E 3'}")
    print(f"   Kvalita: {args.quality}")

    # Urči, čo generovať
    to_generate = []
    if args.cover_only:
        if prompts["cover"]:
            to_generate.append(("cover", prompts["cover"], images_dir / "cover-16x9.png"))
    elif args.scene:
        for s in args.scene:
            if s in prompts["scenes"]:
                to_generate.append((f"scene-{s:02d}", prompts["scenes"][s], images_dir / f"scene-{s:02d}.png"))
            else:
                print(f"⚠️  Scéna {s} neexistuje")
    elif args.all:
        if prompts["cover"]:
            to_generate.append(("cover", prompts["cover"], images_dir / "cover-16x9.png"))
        for s in sorted(prompts["scenes"].keys()):
            to_generate.append((f"scene-{s:02d}", prompts["scenes"][s], images_dir / f"scene-{s:02d}.png"))
    else:
        for s in sorted(prompts["scenes"].keys()):
            output_path = images_dir / f"scene-{s:02d}.png"
            if not output_path.exists():
                to_generate.append((f"scene-{s:02d}", prompts["scenes"][s], output_path))
            else:
                print(f"  ⏭️  scene-{s:02d}.png existuje, preskakujem")

    if not to_generate:
        print("✅ Všetky obrázky existujú. Použi --all pre pregenerovanie.")
        return

    print(f"\n🎨 Generujem {len(to_generate)} obrázkov...\n")

    cover_path = images_dir / "cover-16x9.png"
    use_reference = args.model == "gpt-image" and not args.no_reference

    for i, (name, prompt, output_path) in enumerate(to_generate, 1):
        print(f"[{i}/{len(to_generate)}] {name}")

        if args.dry_run:
            print(f"  📝 Prompt: {prompt[:120]}...")
            print(f"  💾 Výstup: {output_path}")
            ref = "s referenciou" if (use_reference and name != "cover" and cover_path.exists()) else "bez referencie"
            print(f"  🔗 {ref}\n")
            continue

        try:
            if args.model == "gpt-image":
                # Pre cover: bez referencie. Pre scény: s cover referenciou.
                ref_path = None
                if use_reference and name != "cover" and cover_path.exists():
                    ref_path = cover_path
                    print(f"  🔗 Používam cover ako referenciu pre konzistenciu postáv")

                img_data = generate_with_reference(
                    client, prompt,
                    reference_path=ref_path,
                    size="1536x1024",
                    quality=args.quality
                )
            else:
                img_data = generate_dalle3(client, prompt, size="1792x1024")

            output_path.parent.mkdir(parents=True, exist_ok=True)
            output_path.write_bytes(img_data)
            print(f"  ✅ Uložené: {output_path} ({len(img_data) / 1024:.0f} KB)")

            # Rate limiting
            if i < len(to_generate):
                wait = 8 if args.model == "gpt-image" else 15
                print(f"  ⏱️  Čakám {wait}s...")
                time.sleep(wait)

        except Exception as e:
            print(f"  ❌ Chyba: {e}")
            if "rate_limit" in str(e).lower():
                print("  ⏱️  Rate limit — čakám 60s...")
                time.sleep(60)

    print("\n🎉 Hotovo!")


if __name__ == "__main__":
    main()
