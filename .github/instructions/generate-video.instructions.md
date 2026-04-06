# 🎬 Inštrukcie: Generovanie videa (Strihač)

> Tieto inštrukcie sú určené pre agenta **Strihač**, ktorý vytvára video verziu rozprávky zo zvukového záznamu a ilustrácií.

## 1. Cieľ

Vytvoriť slideshow video, ktoré kombinuje audio nahrávku rozprávky s ilustráciami. Video musí byť pripravené na publikáciu na YouTube v kvalite 1080p. Obrázky musia **presne zodpovedať** hovoreným pasážam — synchronizácia je kľúčová.

## 2. Automatizovaný skript — `scripts/build-video.py`

> ⚠️ **VŽDY používaj automatizovaný skript.** Nezostavuj FFmpeg príkazy ručne — ručné príkazy sú nepresné a vedú k desynchronizácii audia a videa.

### 2.1 Použitie

```bash
# Krok 1: Náhľad časovania (nevytvára video, len assembly plan)
python scripts/build-video.py --story-dir rozpravky/[id-rozpravky] --plan-only

# Krok 2: Generovanie videa
python scripts/build-video.py --story-dir rozpravky/[id-rozpravky]
```

### 2.2 Požiadavky

Skript vyžaduje, aby v adresári rozprávky existovali:

| Súbor | Popis |
|-------|-------|
| `audio/rozpravka.mp3` | Finálna audio nahrávka |
| `audio/clean-text.txt` | Čistý text pre TTS (bez Markdown, bez YAML) |
| `images/cover-16x9.png` | Obálka rozprávky (používa sa ako titulná aj záverečná karta) |
| `images/scene-01.png` … `scene-NN.png` | Ilustrácie ku scénam |

### 2.3 Výstupy

| Súbor | Popis |
|-------|-------|
| `video/assembly-plan.json` | Presný plán zostrihania s časovaním |
| `video/rozpravka.mp4` | Finálne video pripravené na YouTube |

## 3. Princíp časovania — proporcionálne podľa počtu slov

Skript počíta **presné časovanie** pre každý obrázok na základe textu:

### 3.1 Ako to funguje

1. Načíta `audio/clean-text.txt` a rozdelí ho na segmenty
2. Každý segment je definovaný **začiatočnou a koncovou textovou značkou**
3. Spočíta slová v každom segmente
4. Pridelí čas **proporcionálne** podľa počtu slov
5. Zohľadní rozpočet páuz medzi sekciami (1,5s na veľký prechod)

### 3.2 Definícia segmentov v skripte

Segmenty sú definované v funkcii `calculate_timeline()` ako zoznam:

```python
segments = [
    ("title",    "Začiatok textu.",         "Koniec textu.",       "cover-16x9.png"),
    ("scene-01", "Kde bolo, tam bolo",      "koniec prvej scény.", "scene-01.png"),
    ("scene-02", "Začiatok druhej scény",   "koniec druhej scény.", "scene-02.png"),
    # ...
    ("moral",    "Poučenie.",               "koniec poučenia.",    "cover-16x9.png"),
]
```

**Pre každú novú rozprávku musíš upraviť tieto segmenty**, aby textové značky zodpovedali skutočnému textu v `audio/clean-text.txt`.

### 3.3 Rozpočet páuz

- Medzi hlavnými sekciami príbehu sú pauzy (~1,5s každá)
- Tieto zodpovedajú značkám `...` v `clean-text.txt`
- Celkový rozpočet páuz sa odpočíta od dĺžky audia pred proporcionálnym rozdelením
- Indexy segmentov s pauzou sú definované v `pause_before` v skripte

### 3.4 Titulný segment

- Používa `cover-16x9.png` — **nie** osobitnú titulnú kartu
- Minimálna dĺžka: **5 sekúnd**
- Ak má málo slov, škáluje sa nahor na minimum

### 3.5 Záverečný segment (Poučenie)

- Tiež používa `cover-16x9.png` — rovnaký obrázok ako na začiatku
- Trvanie sa vypočíta tak, aby presne pokrylo zvyšok audia
- Posledný segment sa vždy roztiahne na `total_duration - segment.start`

## 4. ⚠️ Kritická chyba — trvanie segmentu musí zahŕňať medzery

> Toto je najdôležitejšie poučenie z prvej produkcie. Bez tejto opravy video skončí **12+ sekúnd** pred audiom.

### 4.1 Problém

Medzi segmentmi sú pauzy (medzery). Ak sa do FFmpeg `-t` parametra zadá len čistý čas hovoreného textu **bez medzery za ním**, video bude kratšie než audio.

### 4.2 Správny výpočet

```python
# Pre všetky segmenty okrem posledného:
display_dur = next_segment.start - current_segment.start

# Pre posledný segment:
display_dur = total_duration - segment.start
```

Takto sa trailing pauza za každým segmentom **zahrnie** do doby zobrazenia obrázka.

### 4.3 Zakázané

- ❌ **NIKDY** nepoužívaj `-shortest` vlajku vo FFmpeg — môže orezať audio
- ❌ **NIKDY** nepočítaj trvanie len zo slov bez medzier

## 5. Verifikácia dĺžky — povinná

> Po každom zostavení videa **VŽDY** skontroluj dĺžku.

```bash
ffprobe -v quiet -show_entries format=duration -of csv=p=0 video/rozpravka.mp4
ffprobe -v quiet -show_entries format=duration -of csv=p=0 audio/rozpravka.mp3
```

| Kontrola | Kritérium |
|----------|-----------|
| Rozdiel dĺžok video vs. audio | **≤ 1 sekunda** |
| Ak je rozdiel > 1s | Chyba v časovaní — oprav a zostavaj znova |

Skript túto kontrolu vykonáva automaticky a vypíše trvanie na konci.

## 6. Mapovanie obrázkov na text

### 6.1 Princíp

Každá ilustrácia zodpovedá konkrétnemu úseku textu v `audio/clean-text.txt`:

- **Začiatočná značka** — prvé slová scény (napr. `"Kde bolo, tam bolo"`)
- **Koncová značka** — posledné slová scény (napr. `"za horami."`)
- Značky musia **presne zodpovedať** textu v `clean-text.txt`

### 6.2 Štruktúra segmentov

| Segment | Obrázok | Účel |
|---------|---------|------|
| `title` | `cover-16x9.png` | Titulná karta (~5 sekúnd) |
| `scene-01` … `scene-NN` | `scene-01.png` … `scene-NN.png` | Ilustrácie ku scénam |
| `moral` | `cover-16x9.png` | Poučenie na záver |

### 6.3 Koľko scén na rozprávku

| Dĺžka rozprávky | Scény | Celkový počet segmentov |
|------------------|-------|------------------------|
| 5 minút | 4–6 | 6–8 (title + scény + moral) |
| 10 minút | 10–14 | 12–16 |
| 15 minút | 14–18 | 16–20 |
| 20+ minút | 18–22 | 20–24 |

**Typický výstup**: 10-minútová rozprávka → 14 scén → 16 segmentov → ~80 MB video, ~515s.

## 7. Prechody (Transitions)

### Typ prechodu

Skript používa **fade-in a fade-out** na každom segmente:

- **Trvanie fade**: 0,8 sekundy
- Prvý segment: len fade-out
- Posledný segment: len fade-in
- Stredné segmenty: fade-in aj fade-out
- Výsledok: jemný crossfade efekt medzi obrázkami

### Prečo tento prístup

- Je to jemné a neinvazívne pre detského diváka
- Evokuje „otáčanie stránok" v knižke
- Implementácia cez concat filter je spoľahlivá a presná

## 8. FFmpeg technické nastavenia

Skript používa tieto nastavenia. **Nemeň ich**, pokiaľ nie je konkrétny dôvod:

### 8.1 Spracovanie obrázkov

Každý obrázok sa škáluje a podložia na presné rozlíšenie:

```
scale=1920:1080:force_original_aspect_ratio=decrease,pad=1920:1080:(ow-iw)/2:(oh-ih)/2:color=white,setsar=1
```

- Zachová pomer strán
- Biely padding na okrajoch
- SAR 1:1 pre korektné zobrazenie

### 8.2 Video nastavenia

| Parameter | Hodnota | Poznámka |
|-----------|---------|----------|
| **Rozlíšenie** | 1920×1080 (Full HD) | YouTube štandard |
| **FPS** | 30 | Plynulé prechody |
| **Video kodek** | H.264 (libx264) | Univerzálna kompatibilita |
| **Pixel formát** | yuv420p | YouTube kompatibilita |
| **CRF** | 18 | Vysoká kvalita |
| **Preset** | slow | Lepšia kompresia |
| **movflags** | +faststart | Streamovanie na YouTube |

### 8.3 Audio nastavenia

| Parameter | Hodnota | Poznámka |
|-----------|---------|----------|
| **Audio kodek** | AAC | YouTube štandard |
| **Bitrate** | 192 kbps | Dobrá kvalita pre hlas |
| **Sample rate** | 44100 Hz | Štandard |
| **Kanály** | Stereo | YouTube odporúčanie |

### 8.4 Dôležité FFmpeg vlajky

- `-movflags +faststart` — umožní streamovanie na YouTube
- `-pix_fmt yuv420p` — kompatibilita so všetkými prehrávačmi
- `-crf 18` — vysoká kvalita (nižšie číslo = vyššia kvalita)
- `-preset slow` — lepšia kompresia, dlhší čas kódovania
- ❌ **Nepoužívaj** `-shortest` — orezáva audio

## 9. Assembly plan — plán zostrihania

Skript automaticky generuje `video/assembly-plan.json`:

```json
{
  "total_duration": 515.23,
  "segments": [
    {
      "name": "title",
      "image": "cover-16x9.png",
      "start": 0.0,
      "end": 6.54,
      "duration": 6.54,
      "words": 4
    },
    {
      "name": "scene-01",
      "image": "scene-01.png",
      "start": 8.04,
      "end": 47.32,
      "duration": 39.28,
      "words": 87
    }
  ]
}
```

### Využitie assembly plan

- **YouTube časové značky** — použi `start` hodnoty pre kapitoly v popise videa
- **Kontrola synchronizácie** — ručne porovnaj, či scény zodpovedajú audio
- **Debugging** — ak video nesedí, skontroluj assembly plan najprv

## 10. Adresárová štruktúra

```
rozpravky/[id-rozpravky]/
├── audio/
│   ├── rozpravka.mp3          # Audio nahrávka
│   └── clean-text.txt         # Čistý text pre TTS (zdroj pre časovanie)
├── images/
│   ├── cover-16x9.png         # Obálka (titulná + záverečná karta)
│   ├── scene-01.png           # Ilustrácia scéna 1
│   ├── scene-02.png           # Ilustrácia scéna 2
│   └── ...
└── video/
    ├── rozpravka.mp4          # Finálne video
    └── assembly-plan.json     # Plán zostrihania (generovaný skriptom)
```

> **Poznámka**: Nepotrebujeme samostatné `title-card.mp4` ani `end-card.mp4`. Obálka `cover-16x9.png` slúži ako titulná aj záverečná karta.

## 11. Pracovný postup krok za krokom

### Krok 1: Skontroluj vstupné súbory

Pred spustením over, že existujú:

- [ ] `audio/rozpravka.mp3` — finálna audio nahrávka
- [ ] `audio/clean-text.txt` — čistý text bez Markdown/YAML
- [ ] `images/cover-16x9.png` — obálka rozprávky
- [ ] `images/scene-01.png` … `scene-NN.png` — všetky ilustrácie

### Krok 2: Uprav segmenty v skripte

Otvor `scripts/build-video.py` a uprav funkciu `calculate_timeline()`:

1. Aktualizuj zoznam `segments` — textové značky musia zodpovedať `clean-text.txt`
2. Aktualizuj `pause_before` — indexy segmentov, pred ktorými je pauza
3. Over, že počet segmentov zodpovedá počtu ilustrácií + title + moral

### Krok 3: Náhľad časovania

```bash
python scripts/build-video.py --story-dir rozpravky/[id-rozpravky] --plan-only
```

Skontroluj výstup:

- Každý segment má rozumné trvanie (min. ~15s, max. ~90s pre scény)
- Titulný segment je aspoň 5 sekúnd
- Celkový čas zodpovedá dĺžke audia
- Počet segmentov je správny

### Krok 4: Zostavenie videa

```bash
python scripts/build-video.py --story-dir rozpravky/[id-rozpravky]
```

### Krok 5: Verifikácia

1. Skontroluj výpis skriptu — dĺžka videa vs. audio (rozdiel ≤ 1s)
2. Prehraj video a over synchronizáciu obrázkov s hovorom
3. Over, že `video/assembly-plan.json` bol vygenerovaný

## 12. Riešenie problémov

### Video je kratšie než audio

- **Príčina**: Trvanie segmentov nezahŕňa medzery (pauzy)
- **Riešenie**: Skontroluj výpočet `display_dur` — musí používať `next_segment.start - current_segment.start`

### Obrázok nezodpovedá hovorenej pasáži

- **Príčina**: Textové značky v segmentoch nesedia s `clean-text.txt`
- **Riešenie**: Otvor `clean-text.txt` a skopíruj presné začiatočné/koncové frázy

### FFmpeg chyba „No such file"

- **Príčina**: Chýbajúci obrázok v `images/`
- **Riešenie**: Skript vypíše zoznam chýbajúcich súborov — doplň ich

### Video má čierne snímky

- **Príčina**: Obrázok má iný pomer strán a padding nefunguje
- **Riešenie**: Skript automaticky škáluje a podkladá — skontroluj, či obrázky nie sú poškodené

## 13. Kontrola kvality

Pred odovzdaním skontroluj:

- [ ] Video sa prehrá bez artefaktov od začiatku do konca
- [ ] **Dĺžka videa zodpovedá dĺžke audia (±1 sekunda)**
- [ ] Audio a obrázky sú synchronizované — obrázok zodpovedá scéne
- [ ] Prechody sú plynulé (fade 0,8s)
- [ ] Titulná karta (obálka) sa zobrazí na začiatku
- [ ] Obálka sa zobrazí znova pri poučení na konci
- [ ] Rozlíšenie je 1920×1080
- [ ] Žiadne čierne snímky ani vizuálne chyby
- [ ] Audio nie je orezané

## 14. Kontrolný zoznam

- [ ] Vstupné súbory existujú (`audio/rozpravka.mp3`, `audio/clean-text.txt`, obrázky)
- [ ] Segmenty v `build-video.py` sú aktualizované pre aktuálnu rozprávku
- [ ] Textové značky presne zodpovedajú `clean-text.txt`
- [ ] `--plan-only` bol spustený a časovanie je rozumné
- [ ] Video bolo zostavené skriptom `scripts/build-video.py`
- [ ] Dĺžka videa ≈ dĺžka audia (rozdiel ≤ 1 sekunda)
- [ ] `video/assembly-plan.json` bol vygenerovaný
- [ ] Video bolo ručne prehrané a synchronizácia overená
- [ ] Video je vo formáte H.264, 1080p, 30 fps
- [ ] Audio je AAC, 192 kbps
