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
- 🤖 **Automatizované** – GitHub Copilot CLI + C# pipeline
- 🔊 **Audio** – ElevenLabs TTS pre slovenský hlas
- 🎨 **Ilustrácie** – DALL-E 3 pre detské ilustrácie
- 🎬 **Video** – slideshow video pre YouTube
- 📊 **Tracking** – kompletný lifecycle management každej rozprávky

## 📂 Štruktúra projektu

```
PoucneSlovenskeRozpravky/
├── .github/                    # Copilot instructions, agents, skills
├── rozpravky/                  # Všetky rozprávky (každá vo vlastnom priečinku)
├── sablony/                    # Šablóny promptov
├── config/                     # Konfigurácia (témy, postavy, štýl)
├── src/                        # C# / .NET 10 zdrojový kód
├── tests/                      # Testy
├── scripts/                    # Automatizačné skripty
├── katalog.json                # Master katalóg všetkých rozprávok
├── AGENTS.md                   # Definície Copilot sub-agentov
└── README.md                   # Tento súbor
```

## 🔧 Prerekvizity

- [GitHub Copilot CLI](https://github.com/github/copilot-cli)
- [.NET 10 SDK](https://dotnet.microsoft.com/)
- [Docker](https://www.docker.com/) (pre LanguageTool)
- [FFmpeg](https://ffmpeg.org/) (pre video generovanie)
- API kľúče: ElevenLabs, OpenAI (DALL-E)

## 🚀 Začíname

### 1. Interaktívne generovanie (Copilot CLI)

```bash
copilot
# Potom zadajte: "Vygeneruj novú rozprávku na 15 minút"
```

### 2. C# Pipeline (post-processing)

```bash
cd src
dotnet run --project PoucneRozpravky.CLI -- pipeline <id>
```

### 3. Jednotlivé kroky

```bash
dotnet run --project PoucneRozpravky.CLI -- review <id>    # QA review
dotnet run --project PoucneRozpravky.CLI -- audio <id>     # Generovanie audio
dotnet run --project PoucneRozpravky.CLI -- images <id>    # Generovanie ilustrácií
dotnet run --project PoucneRozpravky.CLI -- video <id>     # Vytvorenie videa
dotnet run --project PoucneRozpravky.CLI -- publish <id>   # Publikácia
dotnet run --project PoucneRozpravky.CLI -- status         # Stav všetkých rozprávok
```

## 📋 Lifecycle rozprávky

```
outline_draft → outline_ready → text_draft → [QA Pipeline] → text_ready → audio_ready → images_ready → video_ready → fully_published
```

## 🤖 Copilot CLI Agenti

| Agent | Úloha |
|-------|-------|
| Architekt | Generuje osnovu/outline rozprávky |
| Rozprávkár | Píše text rozprávky |
| Korektor | Hĺbková gramatická kontrola slovenčiny |
| Štylistik | Kontrola štylistiky a čitateľnosti |
| Recenzent | Obsahová recenzia (logika, vhodnosť, poučnosť) |
| Ilustrátor | Pripravuje prompty pre ilustrácie |
| Zvukár | Riadi audio pipeline |
| Strihač | Riadi video pipeline |
| Vydavateľ | Publikuje na platformy |

## 📊 Publikačné platformy

- **Text**: Blog/web
- **Audio**: Spotify (podcast)
- **Video**: YouTube

## 📝 Licencia

Všetky rozprávky sú originálny obsah generovaný AI.
