# 🔊 Inštrukcie: Generovanie audia (Zvukár)

> Tieto inštrukcie sú určené pre agenta **Zvukár**, ktorý riadi pipeline na vytvorenie audio verzie rozprávky.

## 1. Cieľ

Vytvoriť kvalitnú audio nahrávku rozprávky vo formáte vhodnom na publikáciu (Spotify podcast, YouTube video). Audio musí znieť prirodzene a pútavo pre detského poslucháča.

## 2. Príprava textu pre TTS

### 2.1 Odstránenie Markdown formátovania

Z textu `rozpravka.md` odstráň:

- YAML front matter (celý blok medzi `---`)
- Nadpisy (`#`, `##`, `###`) — nahraď ich pauzou
- Horizontálne čiary (`---`) — nahraď pauzou
- Tučné písmo (`**text**`) — ponechaj len text
- Kurzívu (`*text*`) — ponechaj len text
- Zoznamy a odrážky
- Akékoľvek ďalšie Markdown značky

### 2.2 Spracovanie dialógov

Dialógy vyžadujú špeciálnu prípravu:

```
PÔVODNE:
„Kam ideš, malý zajačik?" spýtala sa líška sladkým hlasom.

PRE TTS (clean-text.txt):
Kam ideš, malý zajačik? spýtala sa líška sladkým hlasom.
```

- Odstráň úvodzovky „ a "
- ElevenLabs automaticky spracuje intonáciu dialógov
- Pomenovacia veta zostáva (TTS ju prečíta ako rozprávač)

### 2.3 Značky pre pauzy

Používame `"..."` (tri bodky) ako univerzálnu značku pauzy v `clean-text.txt`:

- ElevenLabs prirodzene spracuje `"..."` ako pauzu v reči
- Skript `scripts/build-video.py` používa text medzi pauzami na výpočet časovania segmentov vo videu
- Pauzy umiestňuj medzi scénami, pred/za ponaučením a na začiatku/konci rozprávky

```
... Kde bolo, tam bolo, za siedmimi horami žil malý zajačik menom Ušiak. ...

Ušiak mal najväčšie uši zo všetkých zajacov v lese.

... Jedného dňa sa Ušiak rozhodol, že sa vydá na cestu. ...
```

### 2.4 Špeciálne prípady

- **Číslovky** — zapíš slovom: „3" → „tri"
- **Skratky** — rozpiš: „napr." → „napríklad"
- **Zvukomalebné slová** — ponechaj, TTS ich zvládne: „bác!", „šušššš"
- **Opakovacie motívy** — ponechaj opakovanie, dodáva rytmus
- **Ponaučenie** — pridaj `...` pred sekciu Ponaučenie

### 2.5 Výstupný súbor

Upravený text ulož ako `clean-text.txt` v podadresári `audio/`:

```
rozpravky/[id-rozpravky]/audio/clean-text.txt
```

### 2.6 Formát súboru `clean-text.txt`

- Žiadny YAML front matter
- Žiadne Markdown formátovanie (žiadne `#`, `**`, `*`, `---`)
- Úvodzovky dialógov odstránené (ElevenLabs spracuje intonáciu)
- `"..."` použité ako pauzy medzi sekciami
- Číslovky zapísané slovom: „3" → „tri"
- Skratky rozpísané: „napr." → „napríklad"

> **Dôležité**: Súbor `clean-text.txt` slúži dvom účelom:
> 1. Vstup pre ElevenLabs TTS
> 2. Zdrojový text pre `scripts/build-video.py` — skript používa text medzi pauzami na výpočet časovania segmentov vo videu
>
> Preto je dôležité, aby text presne zodpovedal zvukovej nahrávke.

## 3. ElevenLabs — výber modelu a hlasu

### Porovnanie modelov

| Model | ID | Kvalita | Slovenčina | Limit znakov | SSML | Emočné tagy |
|-------|----|---------|------------|-------------|------|-------------|
| **Eleven v3** | `eleven_v3` | Najvyššia — expresívny, emocionálny | ✅ (slk) | 5,000 (~5 min) | ❌ | ✅ `[whispers]`, `[sad]`, `[excited]` |
| **Multilingual v2** | `eleven_multilingual_v2` | Vysoká — stabilný, konzistentný | ✅ (sk) | 10,000 (~10 min) | ✅ `<break>` | ❌ |
| **Flash v2.5** | `eleven_flash_v2_5` | Dobrá — ultra-rýchly | ✅ (sk) | 40,000 (~40 min) | ❌ | ❌ |

### Odporúčanie pre rozprávky

- **Pre rozprávky ≤ 5 min**: `eleven_v3` — najlepšia expresivita a emočný rozsah
- **Pre rozprávky 5–10 min**: `eleven_multilingual_v2` — stabilný, väčší limit znakov, SSML pauzy
- **Pre rozprávky > 10 min**: `eleven_multilingual_v2` s chunking stratégiou (viď sekciu 5)

> **Aktuálne odporúčaný**: `eleven_multilingual_v2` — overený pre slovenčinu, stabilný, dostatočný limit pre väčšinu rozprávok.

### Požiadavky na hlas

- **Jazyk**: slovenčina (Slovak)
- **Typ**: teplý, príjemný, rozprávačský hlas
- **Pohlavie**: podľa preferencie — ženský hlas (rozprávanie babičky) alebo mužský hlas (rozprávanie deduška)
- **Tempo**: pomalšie, pokojné — deti potrebujú čas na spracovanie
- **Emocionalita**: hlas musí byť schopný vyjadriť emócie — radosť, prekvapenie, jemný smútok

### Odporúčaný hlas

**„George"** — otestovaný a odporúčaný ako predvolený hlas:

- Teplý, jasný rozprávačský tón
- Dobrá slovenská výslovnosť vrátane ž, š, č, ť, ď, ň, ľ, dz, dž
- Prirodzene znie pre detského poslucháča
- Zvláda emócie a striedanie tempa

### Výber alternatívneho hlasu

Ak je potrebný iný hlas:

1. Vyber hlas, ktorý podporuje slovenčinu alebo slovanské jazyky
2. Otestuj krátku ukážku (prvý odsek rozprávky)
3. Skontroluj, či hlas:
   - Správne vyslovuje slovenské hlásky (ž, š, č, ť, ď, ň, ľ, dz, dž)
   - Znie prirodzene, nie roboticky
   - Má príjemný tón pre deti
   - Zvláda rôzne emócie

## 4. API parametre

### Povinné parametre pre každý request

```python
{
    "text": "<text>",
    "model_id": "eleven_multilingual_v2",
    "voice_settings": {
        "stability": 0.55,
        "similarity_boost": 0.75,
        "style": 0.40,
        "use_speaker_boost": True
    },
    "language_code": "sk",           # Vynúti slovenskú text normalizáciu
    "output_format": "mp3_44100_192" # Query parameter — 192kbps, 44.1kHz
}
```

### Parametre pre kvalitu

| Parameter | Hodnota | Účel |
|-----------|---------|------|
| **`language_code`** | `"sk"` | Vynúti správnu text normalizáciu pre slovenčinu |
| **`output_format`** | `"mp3_44100_192"` | Najvyššia MP3 kvalita (192kbps, 44.1kHz) |
| **`speed`** | `0.85–0.95` | Spomalenie pre detského poslucháča (predvolené: 1.0) |
| **`seed`** | `<integer>` | Reprodukovateľnosť — rovnaký seed + text = podobný výstup |
| **`apply_text_normalization`** | `"auto"` | Automatická normalizácia čísiel, dátumov |

### Voice settings — ladenie

| Parameter | Odporúčaná hodnota | Poznámka |
|-----------|-------------------|----------|
| **Stability** | 0,50–0,65 | Nižšia = expresívnejší, vyššia = konzistentnejší |
| **Similarity Boost** | 0,70–0,80 | Vyššia = viac ako pôvodný hlas |
| **Style** | 0,30–0,50 | Mierne štylistické úpravy |
| **Speaker Boost** | zapnuté | Zlepšuje kvalitu hlasu |
| **Speed** | 0,85–0,95 | Pomalšie pre deti (min 0.7, max 1.2) |

> **Tip**: Pre rozprávky s emočne intenzívnymi scénami (strach, radosť) použi Stability 0.50. Pre pokojné časti a poučenie použi Stability 0.65.

## 5. Chunking stratégia (pre texty nad limit)

### Kedy chunkovať

- **Multilingual v2**: limit 10,000 znakov → chunk ak text > 9,000 znakov
- **Eleven v3**: limit 5,000 znakov → chunk ak text > 4,500 znakov

### Ako chunkovať

1. **Rozdeľ text na prirodzených hraniciach** — medzi scénami (na `...` pauzách)
2. **Nikdy nedeliť uprostred vety** ani dialógu
3. **Každý chunk** generuj ako samostatný API request

### Request stitching — plynulé spájanie

Použi `previous_request_ids` na zachovanie prozódie medzi chunkmi:

```python
# Chunk 1 — prvý segment
response_1 = client.text_to_speech.convert(
    voice_id="george-voice-id",
    text=chunk_1,
    model_id="eleven_multilingual_v2",
    language_code="sk",
    output_format="mp3_44100_192",
    speed=0.9
)
request_id_1 = response_1.headers["request-id"]

# Chunk 2 — nadväzuje na chunk 1
response_2 = client.text_to_speech.convert(
    voice_id="george-voice-id",
    text=chunk_2,
    model_id="eleven_multilingual_v2",
    language_code="sk",
    output_format="mp3_44100_192",
    speed=0.9,
    previous_request_ids=[request_id_1]
)
```

### Alternatíva: previous_text / next_text

Ak nemáš request_id, použi textový kontext:

```python
response_2 = client.text_to_speech.convert(
    text=chunk_2,
    previous_text=chunk_1[-300:],  # Posledných ~300 znakov predchádzajúceho chunku
    next_text=chunk_3[:300],        # Prvých ~300 znakov nasledujúceho chunku
    ...
)
```

### Spájanie chunkov

Po vygenerovaní všetkých chunkov spoj ich do jedného MP3 pomocou FFmpeg:

```bash
# Vytvor zoznam súborov
echo "file 'chunk-01.mp3'" > chunks.txt
echo "file 'chunk-02.mp3'" >> chunks.txt
echo "file 'chunk-03.mp3'" >> chunks.txt

# Spoj bez re-encodovania
ffmpeg -f concat -safe 0 -i chunks.txt -c copy rozpravka.mp3
```

## 6. Pauzy a interpunkcia

### Pauzy v Multilingual v2

| Metóda | Syntax | Trvanie | Spoľahlivosť |
|--------|--------|---------|--------------|
| **SSML break tag** | `<break time="1.5s" />` | Presné (0.1–3.0s) | ⭐⭐⭐⭐⭐ |
| **Trojbodky** | `...` | ~0.5–1.0s | ⭐⭐⭐⭐ |
| **Pomlčka** | `—` | ~0.3s | ⭐⭐⭐ |
| **Nový riadok** | `\n\n` | Variabilné | ⭐⭐ |

> **Odporúčanie**: Používaj `...` ako primárnu pauzu (funguje na všetkých modeloch a je kompatibilné s `build-video.py`). Pre presné pauzy na Multilingual v2 použi SSML `<break>`.
>
> **⚠️ Pozor**: Príliš veľa `<break>` tagov v jednom requeste môže spôsobiť nestabilitu (zrýchlenie reči, artefakty).

### Pauzy v Eleven v3

V3 **nepodporuje** SSML break tagy. Použi:
- `...` (trojbodky) — najspoľahlivejšie
- `—` (pomlčka) — krátka pauza
- Štruktúru textu a interpunkciu

## 7. Emočné tagy (Eleven v3)

Ak používaš model `eleven_v3`, môžeš pridať audio tagy pre emočnú kontrolu:

```
[whispers] Ale pozor, v tom lese žil aj veľký medveď...

[excited] A vtedy Janko konečne našiel kľúč!

[sad] Malá líška sedela sama pri potoku...
```

### Dostupné tagy pre rozprávky

| Tag | Použitie v rozprávke |
|-----|---------------------|
| `[whispers]` | Tajomné momenty, šepkanie postáv |
| `[excited]` | Radostné momenty, objavenie niečoho |
| `[sad]` | Smutné chvíle (ale vždy s nádejou) |
| `[curious]` | Rozprávač kladie otázku |
| `[sighs]` | Úľava, spokojnosť |
| `[laughs]` | Veselé momenty |

> **⚠️ Dôležité**: Audio tagy budú prečítané nahlas — v post-produkcii ich musíš odstrániť, ak chceš iba emočný efekt bez textu tagu.

## 8. Pronunciation dictionary (voliteľné)

Pre slovenské mená postáv a neobvyklé slová vytvor pronunciation dictionary:

### Formát (TXT alias):

```
Bodko=Bod-ko
Ušiak=U-šiak
Ryška=Riš-ka
```

### Použitie v API

```python
pronunciation_dictionary_locators=[
    {"pronunciation_dictionary_id": "...", "version_id": "..."}
]
```

> Pronunciation dictionary je užitočný keď TTS opakovane zle vyslovuje meno postavy.

## 9. Pacing a intonácia

### Rýchlosť reči

- **Úvod rozprávky** — mierne pomalšie, navodzuje atmosféru
- **Dialógy** — normálne tempo, živšie
- **Napínavé momenty** — mierne rýchlejšie
- **Ponaučenie** — pomalé, dôrazné
- **Celkovo**: radšej pomalšie než rýchlejšie — cieľová skupina sú deti
- **Speed parameter**: 0.85–0.95 (nižšia hodnota = pomalšie)

### Intonácia

- **Otázky** — stúpavá intonácia na konci
- **Výkričníky** — zvýšená hlasitosť a energia
- **Trojbodky (...)** — pomalšie tempo, pauza
- **Dialógy rôznych postáv** — mierne odlišná intonácia (nie dramaticky)

## 10. Pomenovanie súborov a ukladanie

### Adresárová štruktúra

```
rozpravky/[id-rozpravky]/
├── outline.md
├── rozpravka.md
├── audio/
│   ├── rozpravka.mp3       # Finálna audio nahrávka (MP3, 192kbps, 44.1kHz)
│   ├── clean-text.txt      # Čistý text pre TTS (bez markdownu, s "..." pauzami)
│   └── metadata.json       # Metadáta audio súboru (hlas, trvanie, nastavenia)
```

### metadata.json

```json
{
  "voice_id": "[ID hlasu z ElevenLabs]",
  "voice_name": "George",
  "model_id": "eleven_multilingual_v2",
  "language_code": "sk",
  "duration_seconds": 0,
  "duration_formatted": "00:00",
  "format": "mp3",
  "output_format": "mp3_44100_192",
  "bitrate": "192kbps",
  "sample_rate": 44100,
  "speed": 0.9,
  "seed": null,
  "created": "YYYY-MM-DD",
  "elevenlabs_settings": {
    "stability": 0.55,
    "similarity_boost": 0.75,
    "style": 0.40,
    "use_speaker_boost": true
  },
  "chunking": {
    "used": false,
    "chunks_count": 1,
    "stitching_method": "none"
  }
}
```

### Konvencie pomenovania

- Hlavný audio súbor: `rozpravka.mp3` (priamy výstup z ElevenLabs)
- Čistý text pre TTS: `clean-text.txt`
- Ak je rozprávka rozdelená na časti: `chunk-01.mp3`, `chunk-02.mp3`, ...
- Všetky audio súbory sú vo formáte **MP3, 192 kbps, 44.1 kHz**

## 11. Post-processing (voliteľný)

Ak je k dispozícii FFmpeg, vykonaj nasledovné úpravy:

1. **Normalizácia hlasitosti** — všetky audio súbory na rovnakú hlasitosť
2. **Odstránenie ticha** — skrátenie príliš dlhých páuz (nad 3 sekundy)
3. **Úvodný/záverečný tón** — voliteľne pridaj jemný zvuk zvončekov alebo tichú melódiu

## 12. Kontrolný zoznam

- [ ] Text je očistený od Markdown formátovania
- [ ] Dialógy sú správne spracované (úvodzovky odstránené)
- [ ] Pauzy `"..."` sú na správnych miestach
- [ ] Číslovky a skratky sú rozpísané
- [ ] `audio/clean-text.txt` je uložený v audio podadresári
- [ ] Model je zvolený podľa dĺžky rozprávky (v3 ≤5min, Multilingual v2 >5min)
- [ ] `language_code: "sk"` je nastavený
- [ ] `speed: 0.85–0.95` je nastavený pre detského poslucháča
- [ ] `output_format: "mp3_44100_192"` je nastavený
- [ ] Hlas je vybraný a otestovaný na slovenskú výslovnosť (predvolený: George)
- [ ] ElevenLabs voice settings sú nastavené podľa odporúčaní
- [ ] Pre texty nad limit: chunking s `previous_request_ids` je použitý
- [ ] Audio súbor je uložený ako `rozpravka.mp3`
- [ ] `metadata.json` je vyplnený (vrátane model_id, language_code, speed)
- [ ] Hlasitosť je normalizovaná
