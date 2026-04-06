# 📚 Poučné Slovenské Rozprávky

Automatizovaný workspace pre tvorbu, spracovanie a publikáciu poučných slovenských rozprávok pre deti od 6 rokov.

## 🎯 O projekte

Tento projekt využíva **GitHub Copilot CLI** ako kreatívny engine na generovanie unikátnych, poučných rozprávok písaných po slovensky. Každá rozprávka prechádza kompletným pipeline:

```
Nápad → Outline → Text → QA Review → Audio → Ilustrácie → Video → Publikácia
```

### Kľúčové vlastnosti

- 🇸🇰 **Slovenčina** – všetky rozprávky sú písané v slovenskom jazyku
- 👶 **Pre deti 6+** – vhodný jazyk, pozitívne hodnoty, žiadne násilie
- 📖 **Poučné** – každá rozprávka obsahuje jasný morál/ponaučenie
- 🤖 **Automatizované** – GitHub Copilot CLI agenti + Python/C# pipeline
- 🔊 **Audio** – ElevenLabs TTS (hlas „George") pre slovenský hlas
- 🎨 **Ilustrácie** – GPT Image (gpt-image-1) pre detské ilustrácie vo vodovkovom štýle
- 🎬 **Video** – slideshow video s časovaním podľa audia pre YouTube
- 📊 **Tracking** – kompletný lifecycle management v `katalog.json`

## 📂 Štruktúra projektu

```
PoucneSlovenskeRozpravky/
├── .github/                    # Copilot instructions, skills
│   ├── copilot-instructions.md # Hlavné inštrukcie pre Copilot
│   ├── instructions/           # Inštrukcie pre jednotlivé fázy pipeline
│   └── skills/                 # Copilot skills (grammar, style, age-check...)
├── rozpravky/                  # Všetky rozprávky (každá vo vlastnom priečinku)
│   └── YYYY-MM-DD-slug/       # Adresár jednej rozprávky
│       ├── outline.md          # Osnova príbehu
│       ├── rozpravka.md        # Text rozprávky
│       ├── metadata.json       # Metadáta
│       ├── audio/              # Audio súbory (clean-text.txt, rozpravka.mp3)
│       ├── images/             # Ilustrácie (scene-01.png ... prompts.md)
│       ├── video/              # Video (rozpravka.mp4, assembly-plan.md)
│       └── publish/            # Publikačné metadáta
├── config/                     # Konfigurácia
│   ├── writing-style-prompt.md # Štýlový sprievodca (syntéza 10 slovenských tradícií)
│   ├── themes.json             # Dostupné témy
│   ├── characters.json         # Archetypy postáv
│   └── style-guide.json        # Technické štýlové pravidlá
├── sablony/                    # Šablóny promptov pre generovanie
├── scripts/                    # Automatizačné skripty
│   ├── generate-images.py      # GPT Image pipeline
│   ├── build-video.py          # Zostavenie videa z audia + obrázkov
│   ├── youtube/upload.py       # YouTube upload via OAuth
│   ├── generate-story.ps1      # Automatické generovanie rozprávky
│   └── setup-environment.ps1   # Nastavenie prostredia
├── docs/                       # GitHub Pages (blog)
├── src/                        # C# / .NET 10 zdrojový kód (QA pipeline)
├── tests/                      # Testy
├── katalog.json                # Master katalóg všetkých rozprávok
├── AGENTS.md                   # Definície Copilot sub-agentov
├── LICENSE                     # All Rights Reserved © 2026 Peter Miľovčík
└── README.md                   # Tento súbor
```

## 🔧 Prerekvizity

- [GitHub Copilot CLI](https://github.com/github/copilot-cli)
- [.NET 10 SDK](https://dotnet.microsoft.com/)
- [Python 3.10+](https://www.python.org/) (pre skripty: generate-images, build-video, youtube upload)
- [FFmpeg](https://ffmpeg.org/) (pre video zostavenie)
- **API kľúče** (nastavené ako environment variables):
  - `OPENAI_API_KEY` – OpenAI (GPT Image)
  - `ELEVENLABS_API_KEY` – ElevenLabs (TTS)
- **YouTube OAuth** – `scripts/youtube/client_secret.json` (nie je v git, viď `.gitignore`)

## 🚀 Začíname

### 1. Nastavenie prostredia

```powershell
.\scripts\setup-environment.ps1
```

### 2. Interaktívne generovanie (Copilot CLI)

```bash
copilot
# Potom zadajte: "Vygeneruj novú rozprávku na 10 minút"
```

Pipeline sa skladá z krokov riadených Copilot CLI agentmi:

1. **Architekt** vytvorí outline → `outline.md`
2. **Rozprávkár** napíše text → `rozpravka.md`
3. **Korektor + Štylistik + Recenzent** urobia QA review
4. **Zvukár** pripraví audio → `audio/rozpravka.mp3`
5. **Ilustrátor** vygeneruje obrázky → `images/scene-*.png`
6. **Strihač** zostaví video → `video/rozpravka.mp4`
7. **Vydavateľ** publikuje na GitHub Pages + YouTube

### 3. Python skripty

```bash
python scripts/generate-images.py rozpravky/<id>    # Generovanie ilustrácií
python scripts/build-video.py rozpravky/<id>        # Zostavenie videa
python scripts/youtube/upload.py <video> <meta>     # YouTube upload
```

### 4. C# Pipeline (QA)

```bash
cd src
dotnet run --project PoucneRozpravky.CLI -- review <id>
dotnet run --project PoucneRozpravky.CLI -- status
```

## 📋 Lifecycle rozprávky

```
outline_draft → outline_ready → text_draft → [QA Review] → text_ready
→ audio_ready → images_ready → video_ready → blog_published → youtube_published
→ fully_published
```

Stav každej rozprávky je sledovaný v `katalog.json`.

## 🤖 Copilot CLI Agenti

| Agent | Úloha | Kľúčový výstup |
|-------|-------|-----------------|
| 📐 Architekt | Generuje osnovu/outline rozprávky | `outline.md` |
| 📖 Rozprávkár | Píše text rozprávky | `rozpravka.md` |
| 🔍 Korektor | Hĺbková gramatická kontrola slovenčiny | Korektúrna správa |
| 🎨 Štylistik | Kontrola štylistiky a čitateľnosti | Štylistická správa |
| 📋 Recenzent | Obsahová recenzia (logika, vhodnosť, poučnosť) | Verdikt SCHVÁLENÉ/NESCHVÁLENÉ |
| 🖼️ Ilustrátor | GPT Image prompty + generovanie ilustrácií | `images/scene-*.png` |
| 🔊 Zvukár | Audio pipeline (ElevenLabs TTS) | `audio/rozpravka.mp3` |
| 🎬 Strihač | Video zostavenie (FFmpeg slideshow) | `video/rozpravka.mp4` |
| 📢 Vydavateľ | Publikácia na GitHub Pages + YouTube | Blog post + video |

## 📊 Publikačné platformy

| Platforma | Stav | URL |
|-----------|------|-----|
| **GitHub Pages** (blog) | ✅ Aktívne | [petermilovcik.github.io/PoucneSlovenskeRozpravky](https://petermilovcik.github.io/PoucneSlovenskeRozpravky/) |
| **YouTube** | ✅ Aktívne | [Poučné Slovenské Rozprávky](https://www.youtube.com/@PoucneSlovenskeRozpravky) |
| **Spotify** (podcast) | 🔜 Plánované | — |

## 📚 Publikované rozprávky

| # | Názov | Téma | Dĺžka | Dátum |
|---|-------|------|--------|-------|
| 1 | [Tomášova zlatá minca](https://petermilovcik.github.io/PoucneSlovenskeRozpravky/rozpravky/tomasova-zlata-minca/) | Vďačnosť, priateľstvo | 8 min | 2026-04-06 |

## 📝 Licencia

© 2026 Peter Miľovčík. Všetky práva vyhradené. Viď [LICENSE](LICENSE).
