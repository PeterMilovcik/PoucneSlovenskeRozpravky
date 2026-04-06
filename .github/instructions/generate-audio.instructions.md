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

## 3. ElevenLabs — výber hlasu

### Požiadavky na hlas

- **Jazyk**: slovenčina (Slovak)
- **Typ**: teplý, príjemný, rozprávačský hlas
- **Pohlavie**: podľa preferencie — ženský hlas (rozprávanie babičky) alebo mužský hlas (rozprávanie deduška)
- **Tempo**: pomalšie, pokojné — deti potrebujú čas na spracovanie
- **Emocionalita**: hlas musí byť schopný vyjadriť emócie — radosť, prekvapenie, jemný smútok

### Nastavenia ElevenLabs

| Parameter | Odporúčaná hodnota | Poznámka |
|-----------|-------------------|----------|
| **Stability** | 0,50–0,65 | Nižšia hodnota = expresívnejší hlas |
| **Similarity Boost** | 0,70–0,80 | Vyššia hodnota = konzistentnejší hlas |
| **Style** | 0,30–0,50 | Mierne štylistické úpravy |
| **Speaker Boost** | zapnuté | Zlepšuje kvalitu hlasu |

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

## 4. Pacing a intonácia

### Rýchlosť reči

- **Úvod rozprávky** — mierne pomalšie, navodzuje atmosféru
- **Dialógy** — normálne tempo, živšie
- **Napínavé momenty** — mierne rýchlejšie
- **Ponaučenie** — pomalé, dôrazné
- **Celkovo**: radšej pomalšie než rýchlejšie — cieľová skupina sú deti

### Intonácia

- **Otázky** — stúpavá intonácia na konci
- **Výkričníky** — zvýšená hlasitosť a energia
- **Trojbodky (...)** — pomalšie tempo, pauza
- **Dialógy rôznych postáv** — mierne odlišná intonácia (nie dramaticky)

## 5. Pomenovanie súborov a ukladanie

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
  "voice_name": "[Názov hlasu]",
  "duration_seconds": 0,
  "duration_formatted": "00:00",
  "format": "mp3",
  "bitrate": "192kbps",
  "sample_rate": 44100,
  "created": "YYYY-MM-DD",
  "elevenlabs_settings": {
    "stability": 0.55,
    "similarity_boost": 0.75,
    "style": 0.40,
    "speaker_boost": true
  }
}
```

### Konvencie pomenovania

- Hlavný audio súbor: `rozpravka.mp3` (priamy výstup z ElevenLabs)
- Čistý text pre TTS: `clean-text.txt`
- Ak je rozprávka rozdelená na časti: `rozpravka-01.mp3`, `rozpravka-02.mp3`, ...
- Všetky audio súbory sú vo formáte **MP3, 192 kbps, 44.1 kHz**

## 6. Post-processing (voliteľný)

Ak je k dispozícii FFmpeg, vykonaj nasledovné úpravy:

1. **Normalizácia hlasitosti** — všetky audio súbory na rovnakú hlasitosť
2. **Odstránenie ticha** — skrátenie príliš dlhých páuz (nad 3 sekundy)
3. **Úvodný/záverečný tón** — voliteľne pridaj jemný zvuk zvončekov alebo tichú melódiu

## 7. Kontrolný zoznam

- [ ] Text je očistený od Markdown formátovania
- [ ] Dialógy sú správne spracované (úvodzovky odstránené)
- [ ] Pauzy `"..."` sú na správnych miestach
- [ ] Číslovky a skratky sú rozpísané
- [ ] `audio/clean-text.txt` je uložený v audio podadresári
- [ ] Hlas je vybraný a otestovaný na slovenskú výslovnosť (predvolený: George)
- [ ] ElevenLabs parametre sú nastavené podľa odporúčaní
- [ ] Audio súbor je uložený ako `rozpravka.mp3`
- [ ] `metadata.json` je vyplnený
- [ ] Hlasitosť je normalizovaná
