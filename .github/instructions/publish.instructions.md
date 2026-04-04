# 📢 Inštrukcie: Publikácia (Vydavateľ)

> Tieto inštrukcie sú určené pre agenta **Vydavateľ**, ktorý riadi publikáciu rozprávky na všetky platformy — blog, Spotify podcast a YouTube.

## 1. Cieľ

Pripraviť a publikovať rozprávku na tri platformy s konzistentnými metadátami a formátovaním. Každá platforma má špecifické požiadavky, ale hlavný obsah musí byť zhodný.

## 2. Blog post

### Formát blog postu

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
cover_image: "/images/[slug]/cover.png"
audio_url: "[URL na audio súbor]"
video_url: "[URL na YouTube video]"
description: "[SEO opis, 150–160 znakov]"
---

# [Názov rozprávky]

![Obálka rozprávky](/images/[slug]/cover.png)

**Dĺžka**: [X] minút | **Vek**: 6+ | **Téma**: [téma]

> **Ponaučenie**: [Morálne ponaučenie jednou vetou]

---

[Celý text rozprávky s ilustráciami vloženými medzi odseky]

---

## 🎧 Počúvajte

- [Spotify](link)
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
| **cover_image** | Cesta k obálke v pomere 16:9 |

### Vkladanie ilustrácií do textu

Ilustrácie vlož medzi scény príbehu:

```markdown
[Text scény 1...]

![Popis scény](/images/[slug]/scene-01.png)

[Text scény 2...]
```

- Každý obrázok musí mať **alt text** popisujúci scénu
- Alt text musí byť v **slovenčine**
- Obrázky rozdeľujú text a zvyšujú čitateľnosť

## 3. Spotify podcast epizóda

### Metadáta epizódy

| Pole | Formát | Príklad |
|------|--------|---------|
| **Názov epizódy** | „[Číslo]. [Názov rozprávky]" | „5. Odvážny zajačik" |
| **Popis** | Slovenský opis, max 4000 znakov | Viď šablónu nižšie |
| **Season** | 1 (ak nie je inak) | 1 |
| **Episode number** | Poradové číslo v katalógu | 5 |
| **Episode type** | „full" | „full" |
| **Explicit** | false | false |
| **Language** | „sk" | „sk" |

### Šablóna popisu epizódy

```
🧸 [Názov rozprávky]

[1–2 vety popis príbehu — bez spoilerov!]

⏱️ Dĺžka: [X] minút
👶 Vhodné pre deti od 6 rokov
📖 Téma: [téma]

💡 Ponaučenie: [Morálne ponaučenie]

---

Poučné Slovenské Rozprávky — originálne slovenské rozprávky pre malých aj veľkých.

🌐 Blog: [URL]
📺 YouTube: [URL]

#rozprávky #deti #slovensko #poučné #predspaním
```

### Audio súbor pre podcast

- Formát: **MP3, 192 kbps, 44.1 kHz, stereo**
- Na začiatku: krátky úvodný zvuk (jingle) — ak existuje
- Na konci: krátky záverečný zvuk + „Ďakujeme za počúvanie!"
- Súbor: `audio/rozpravka.mp3` z adresára rozprávky

## 4. YouTube video

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
🎧 Spotify: [URL]

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

- [ ] Názov rozprávky je **identický** na všetkých platformách
- [ ] Dĺžka v minútach je **rovnaká** všade
- [ ] Morálne ponaučenie je **rovnaké** (formulácia môže byť mierne odlišná)
- [ ] Tagy/kľúčové slová sa **prekrývajú** medzi platformami
- [ ] Obálka/thumbnail je **konzistentná** vizuálne
- [ ] Odkazy medzi platformami sú **správne a funkčné**

### Poradie publikácie

1. **Blog** — publikuj text a obrázky
2. **YouTube** — nahraj video, nastav metadáta, pridaj link na blog
3. **Spotify** — nahraj audio epizódu, pridaj linky na blog a YouTube
4. **Aktualizuj blog** — pridaj linky na YouTube a Spotify

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
    "blog": "[URL]",
    "youtube": "[URL]",
    "spotify": "[URL]"
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
│   ├── assembly-plan.md
│   └── metadata.json
└── publish/
    ├── blog-post.md
    ├── youtube-metadata.json
    ├── spotify-metadata.json
    └── publish-log.md
```

## 9. Kontrolný zoznam publikácie

- [ ] Blog post je naformátovaný a obsahuje všetky metadáta
- [ ] Blog post obsahuje ilustrácie s alt textom
- [ ] Spotify epizóda má kompletný popis a metadáta
- [ ] YouTube video má názov, popis, tagy a časové značky
- [ ] Thumbnail je vytvorený a spĺňa požiadavky
- [ ] Všetky platformy majú vzájomné prepojenia (cross-links)
- [ ] `katalog.json` je aktualizovaný so statusom `fully_published`
- [ ] Publikačný log je zapísaný v `publish-log.md`
