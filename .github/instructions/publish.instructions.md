# 📢 Inštrukcie: Publikácia (Vydavateľ)

> Tieto inštrukcie sú určené pre agenta **Vydavateľ**, ktorý riadi publikáciu rozprávky na všetky platformy — blog (GitHub Pages) a YouTube.

## 1. Cieľ

Pripraviť a publikovať rozprávku na dostupné platformy s konzistentnými metadátami a formátovaním. Každá platforma má špecifické požiadavky, ale hlavný obsah musí byť zhodný.

## 2. Blog post (GitHub Pages / Jekyll)

### Platforma

Blog beží na **GitHub Pages s Jekyll** generátorom:

- **Repozitár**: `PeterMilovcik/PoucneSlovenskeRozpravky`
- **Blog URL**: `https://petermilovcik.github.io/PoucneSlovenskeRozpravky/`
- **Blog posty**: ukladajú sa do `docs/_rozpravky/` ako Markdown súbory s YAML front matter
- **Obrázky**: ukladajú sa do `docs/images/[slug]/`
- **Publikácia**: po `git commit` + `git push` sa GitHub Pages automaticky nasadí

### Optimalizácia obrázkov pre blog

Obrázky pre blog musia byť **optimalizované JPG** (nie PNG) kvôli rýchlemu načítaniu:

```bash
ffmpeg -i scene-XX.png -vf "scale=1200:-1" -q:v 4 -y docs/images/[slug]/scene-XX.jpg
```

- Vstup: PNG ilustrácie z `rozpravky/[id]/images/`
- Výstup: JPG do `docs/images/[slug]/`
- Šírka: 1200px, výška sa automaticky dopočíta
- Kvalita: `-q:v 4` (dobrý pomer kvalita/veľkosť)

### Formát blog postu

Súbor: `docs/_rozpravky/[slug].md`

```markdown
---
title: "[Názov rozprávky]"
slug: "[slug]"
date: "YYYY-MM-DD"
author: "Poučné Slovenské Rozprávky"
category: "rozprávky"
tags:
  - [tag1]
  - [tag2]
  - [tag3]
length_minutes: [číslo]
age_group: "6+"
moral: "[Morálne ponaučenie]"
cover_image: "/PoucneSlovenskeRozpravky/images/[slug]/cover.jpg"
video_url: "[URL na YouTube video]"
description: "[SEO opis, 150–160 znakov]"
---

# [Názov rozprávky]

![Obálka rozprávky](/PoucneSlovenskeRozpravky/images/[slug]/cover.jpg)

**Dĺžka**: [X] minút | **Vek**: 6+ | **Téma**: [téma]

> **Ponaučenie**: [Morálne ponaučenie jednou vetou]

---

[Celý text rozprávky s ilustráciami vloženými medzi odseky]

---

## 📺 Pozrite si

- [YouTube](link)

## 📚 Ďalšie rozprávky

[Odkazy na ďalšie rozprávky zo série]
```

### Metadáta blog postu

| Pole | Požiadavka |
|------|------------|
| **title** | Presný názov rozprávky |
| **slug** | URL-friendly identifikátor (malé písmená, pomlčky, bez diakritiky) |
| **date** | Dátum publikácie vo formáte YYYY-MM-DD |
| **description** | SEO opis, 150–160 znakov, obsahuje kľúčové slová |
| **tags** | 3–5 relevantných tagov v slovenčine |
| **cover_image** | Cesta k obálke v JPG formáte (s prefixom `/PoucneSlovenskeRozpravky/`) |

### Vkladanie ilustrácií do textu

Ilustrácie vlož medzi scény príbehu:

```markdown
[Text scény 1...]

![Popis scény](/PoucneSlovenskeRozpravky/images/[slug]/scene-01.jpg)

[Text scény 2...]
```

- Každý obrázok musí mať **alt text** popisujúci scénu
- Alt text musí byť v **slovenčine**
- Obrázky rozdeľujú text a zvyšujú čitateľnosť
- Obrázky musia byť vo formáte **JPG** (optimalizované pre web)

## 3. Spotify podcast epizóda

> **TODO: Konfigurácia pre Spotify podcast bude doplnená neskôr.**

## 4. YouTube video

### Kanál

YouTube publikácia sa vykonáva na **brand kanál** (nie osobný účet):

- **Kanál**: Poučné Slovenské Rozprávky
- **URL kanála**: `https://www.youtube.com/channel/UCwclmlniUJeq5on7s8tEKBQ`
- **YouTube Studio**: `https://studio.youtube.com/channel/UCwclmlniUJeq5on7s8tEKBQ`

> ⚠️ **DÔLEŽITÉ**: Vždy naviguj PRIAMO na URL YouTube Studia brand kanála. Nepoužívaj `studio.youtube.com` bez špecifikácie kanála — to by otvorilo osobný účet.

> ⚠️ **DÔLEŽITÉ**: YouTube NEUMOŽŇUJE nahradiť obsah videa. Ak je potrebná oprava, treba nahrať nové video a staré zmazať.

### Workflow nahrávania na YouTube (cez Playwright MCP)

Nahrávanie na YouTube sa vykonáva cez **Playwright MCP** (automatizovaný prehliadač), NIE cez YouTube API:

```
1. Naviguj na YouTube Studio brand kanála:
   https://studio.youtube.com/channel/UCwclmlniUJeq5on7s8tEKBQ
2. Klikni "Upload videos" → "Select files" → použi file_upload (video/rozpravka.mp4)
3. Počkaj na dokončenie nahrávania
4. Vyplň Details tab:
   - Title (názov videa)
   - Description (popis videa s časovými značkami)
   - Made for kids = Yes
   - Thumbnail (nahraj thumbnail obrázok)
5. Klikni "Show more" → vyplň:
   - Tags (tagy)
   - Language = Slovak
6. Next → Video elements (preskočiť) → Next → Initial check → Next
7. Visibility → Public → Publish
8. Zatvor dialóg publikácie
```

### Názov videa

```
[Názov rozprávky] | Rozprávka pre deti | Poučné Slovenské Rozprávky
```

- Maximálna dĺžka: **100 znakov** (ideálne do 70)
- Obsahuje názov rozprávky a kľúčové slová
- Emoji sú povolené na začiatku: „🧸 Odvážny zajačik | ..."

### Popis videa

```
🧸 [Názov rozprávky]

[2–3 vety opis príbehu — bez spoilerov]

⏱️ Dĺžka: [X] minút
👶 Vhodné pre deti od 6 rokov
📖 Téma: [téma]
💡 Ponaučenie: [Morálne ponaučenie]

---

📚 Čo sa v rozprávke dozvieš:
• [Bod 1 — čo sa deti naučia]
• [Bod 2]
• [Bod 3]

---

⏰ Časové značky:
0:00 — Úvod
[X:XX] — [Názov scény]
[X:XX] — [Názov scény]
[X:XX] — Ponaučenie

---

🔔 Odoberajte kanál pre nové rozprávky každý týždeň!
👍 Dajte like, ak sa vám rozprávka páčila!

🌐 Blog: [URL]

---

#rozprávky #rozprávkapredeti #slovensko #poučnérozprávky #predspaním #detskérozprávky #slovenčina

---

Poučné Slovenské Rozprávky — originálne slovenské rozprávky pre malých aj veľkých. Každá rozprávka obsahuje krásne ilustrácie, príjemné rozprávanie a jasné ponaučenie. Ideálne na počúvanie pred spaním.
```

### Tagy (YouTube tags)

Priprav **15–25 tagov** v slovenčine aj angličtine:

```
rozprávky, rozprávky pre deti, slovenské rozprávky, poučné rozprávky, 
rozprávka pred spaním, detské rozprávky, rozprávky na počúvanie,
[názov rozprávky], [téma], [morál kľúčové slovo],
fairy tales, slovak fairy tales, bedtime stories for kids,
stories for children, educational stories
```

### Časové značky (Timestamps)

Časové značky sa generujú z `assembly-plan.json` (plán zostrihania videa):

- Súbor `assembly-plan.json` obsahuje presné začiatočné časy (`start_seconds`) pre každý segment
- Sekundy sa prevedú na formát **MM:SS** (napr. 95 sekúnd → `1:35`)
- Časové značky sa vložia do YouTube popisu ako klikateľné kapitoly

**Príklad generovania**:

```
assembly-plan.json segment:
  { "label": "Úvod", "start_seconds": 0 }
  { "label": "Stretnutie s líškou", "start_seconds": 95 }
  { "label": "Ponaučenie", "start_seconds": 420 }

→ YouTube popis:
  ⏰ Časové značky:
  0:00 — Úvod
  1:35 — Stretnutie s líškou
  7:00 — Ponaučenie
```

### Kategória a nastavenia

| Nastavenie | Hodnota |
|------------|---------|
| **Kategória** | Education alebo Entertainment |
| **Jazyk** | Slovenčina |
| **Titulky** | Automatické (slovenčina) |
| **Pre deti** | Áno (Made for Kids) |
| **Viditeľnosť** | Verejné |
| **Komentáre** | Vypnuté (Made for Kids) |

## 5. Náhľadový obrázok (Thumbnail)

### Požiadavky

| Parameter | Hodnota |
|-----------|---------|
| **Rozlíšenie** | 1280×720 px (minimum) |
| **Pomer strán** | 16:9 |
| **Formát** | PNG alebo JPG |
| **Veľkosť** | do 2 MB |

### Dizajn thumbnailov

- Použi obálku rozprávky (`cover-16x9.png`) ako základ
- Pridaj **názov rozprávky** veľkým, čitateľným písmom
- Text musí byť čitateľný aj na malom mobile
- Používaj **kontrastné farby** — text na svetlom pozadí alebo naopak
- **Konzistentný štýl** naprieč všetkými rozprávkami:
  - Rovnaký font
  - Rovnaké umiestnenie textu
  - Logo série v rohu

### Šablóna thumbnailov

```
[Obálka rozprávky ako pozadie]
[Polopriehľadný pruh v spodnej tretine]
[Názov rozprávky — veľký biely text s tieňom]
[Logo „PSR" v pravom hornom rohu]
```

## 6. Konzistencia naprieč platformami

### Kontrolný zoznam konzistencie

- [ ] Názov rozprávky je **presne identický** na všetkých platformách (blog, YouTube)
- [ ] Morálne ponaučenie (text) je **zhodné** na všetkých platformách
- [ ] Dĺžka v minútach je **rovnaká** všade
- [ ] Tagy/kľúčové slová sa **prekrývajú** medzi platformami
- [ ] Obálka/thumbnail je **konzistentná** vizuálne
- [ ] Odkazy medzi platformami sú **správne a funkčné** (po dokončení všetkých publikácií)

### Poradie publikácie (overený postup)

1. **Blog (GitHub Pages)** — commit blog post + optimalizované obrázky do `docs/`, push → GitHub Pages auto-deploy
2. **YouTube** — nahraj video cez YouTube Studio brand kanála (Playwright MCP workflow)
3. **Aktualizuj blog** — pridaj YouTube URL do blog postu, commit + push
4. **Aktualizuj `katalog.json`** — pridaj všetky URL, commit + push
5. **Finálny commit + push** — overenie, že všetky zmeny sú v repozitári

## 7. Aktualizácia katalógu

Po úspešnej publikácii na všetkých platformách aktualizuj `katalog.json`:

```json
{
  "id": "[slug]",
  "title": "[Názov rozprávky]",
  "theme": "[téma]",
  "moral": "[morál]",
  "length_minutes": 15,
  "word_count": 2250,
  "age_group": "6+",
  "status": "fully_published",
  "created": "YYYY-MM-DD",
  "published": "YYYY-MM-DD",
  "tags": ["tag1", "tag2"],
  "characters": [
    {"name": "Janko", "role": "Hrdina"}
  ],
  "urls": {
    "blog": "https://petermilovcik.github.io/PoucneSlovenskeRozpravky/rozpravky/[slug]",
    "youtube": "https://www.youtube.com/watch?v=[VIDEO_ID]"
  }
}
```

## 8. Adresárová štruktúra po publikácii

```
rozpravky/[id-rozpravky]/
├── outline.md
├── rozpravka.md
├── audio-text.txt
├── audio/
│   ├── rozpravka.mp3
│   └── metadata.json
├── images/
│   ├── cover-16x9.png
│   ├── cover-1x1.png
│   ├── scene-01.png
│   ├── ...
│   ├── prompts.md
│   └── thumbnail.png
├── video/
│   ├── rozpravka.mp4
│   ├── assembly-plan.json
│   └── metadata.json
└── publish/
    ├── blog-post.md
    ├── youtube-metadata.json
    ├── youtube-result.json
    └── publish-log.md

docs/
├── _rozpravky/
│   └── [slug].md              # Blog post (Jekyll)
└── images/
    └── [slug]/
        ├── cover.jpg          # Optimalizovaná obálka
        ├── scene-01.jpg       # Optimalizované scény
        └── ...
```

### Publikačné súbory

| Súbor | Účel |
|-------|------|
| `publish/youtube-metadata.json` | Názov, popis, tagy, časové značky pre YouTube |
| `publish/youtube-result.json` | Video ID, URL videa, dátum nahratia |
| `publish/blog-post.md` | Kópia blog postu |
| `publish/publish-log.md` | Chronologický záznam publikácie |

## 9. Kontrolný zoznam publikácie

- [ ] Blog post je naformátovaný, obsahuje všetky metadáta a je v `docs/_rozpravky/`
- [ ] Obrázky pre blog sú optimalizované JPG v `docs/images/[slug]/`
- [ ] Blog post obsahuje ilustrácie s alt textom v slovenčine
- [ ] YouTube video má názov, popis, tagy a časové značky z `assembly-plan.json`
- [ ] YouTube nahrávanie prebehlo cez YouTube Studio brand kanála (Playwright MCP)
- [ ] Thumbnail je vytvorený a spĺňa požiadavky
- [ ] Všetky platformy majú vzájomné prepojenia (cross-links)
- [ ] `katalog.json` je aktualizovaný so statusom `fully_published` a všetkými URL
- [ ] `youtube-result.json` obsahuje video ID a URL
- [ ] Publikačný log je zapísaný v `publish-log.md`
- [ ] Finálny commit + push je vykonaný
