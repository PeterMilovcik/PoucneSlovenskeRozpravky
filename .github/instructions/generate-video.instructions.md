# 🎬 Inštrukcie: Generovanie videa (Strihač)

> Tieto inštrukcie sú určené pre agenta **Strihač**, ktorý vytvára video verziu rozprávky zo zvukového záznamu a ilustrácií.

## 1. Cieľ

Vytvoriť slideshow video, ktoré kombinuje audio nahrávku rozprávky s ilustráciami. Video musí byť pripravené na publikáciu na YouTube v kvalite 1080p.

## 2. Plán zostrihania (Slideshow Assembly)

### 2.1 Mapovanie obrázkov na audio

Pre každú ilustráciu urči, **kedy sa má objaviť** vo videu:

```markdown
## Plán zostrihania: [Názov rozprávky]

| Poradie | Obrázok | Začiatok | Koniec | Trvanie | Scéna v príbehu |
|---------|---------|----------|--------|---------|-----------------|
| 1 | title-card.png | 0:00 | 0:05 | 5s | Titulná karta |
| 2 | scene-01.png | 0:05 | 1:30 | 85s | Úvod — predstavenie Janka |
| 3 | scene-02.png | 1:30 | 3:15 | 105s | Stretnutie s líškou |
| ... | ... | ... | ... | ... | ... |
| N | end-card.png | X:XX | X:XX | 8s | Záverečná karta |
```

### 2.2 Výpočet trvania obrázkov

- Celkovú dĺžku audia rozdeľ medzi ilustrácie **podľa dĺžky scén**
- Každý obrázok by mal byť viditeľný **minimálne 30 sekúnd** (inak je to príliš rýchle pre deti)
- Maximálna dĺžka jedného obrázku: **3 minúty** (potom je to nudné)
- Titulná karta: **5 sekúnd**
- Záverečná karta: **8 sekúnd**

### 2.3 Priraďovanie obrázkov ku scénam

1. Prečítaj `rozpravka.md` a identifikuj začiatok a koniec každej scény
2. Prečítaj `audio/metadata.json` pre celkovú dĺžku audia
3. Podľa pomeru textu v každej scéne vypočítaj časové rozpätie
4. Priraď ilustráciu z `images/scene-XX.png` k zodpovedajúcej scéne

## 3. Titulná karta (Title Card)

### Obsah titulnej karty

```
[Názov rozprávky]
Poučné Slovenské Rozprávky
```

### Dizajn

- **Pozadie**: jemný akvarel gradient (svetlomodrá → svetlozelená)
- **Názov rozprávky**: veľký, čitateľný font, tmavá farba
- **Séria**: menší font pod názvom
- **Trvanie**: 5 sekúnd
- **Animácia**: jemný fade-in z čiernej (1 sekunda)

### Generovanie titulnej karty

Vytvor titulnú kartu pomocou FFmpeg:

```bash
ffmpeg -f lavfi -i color=c=#E8F4FD:s=1920x1080:d=5 \
  -vf "drawtext=text='Názov rozprávky':fontsize=72:fontcolor=#2C3E50:x=(w-text_w)/2:y=(h-text_h)/2-40, \
       drawtext=text='Poučné Slovenské Rozprávky':fontsize=36:fontcolor=#7F8C8D:x=(w-text_w)/2:y=(h-text_h)/2+50" \
  -c:v libx264 -pix_fmt yuv420p title-card.mp4
```

## 4. Záverečná karta (End Card)

### Obsah

```
Ďakujeme za počúvanie!
[Morál rozprávky — 1 veta]

Poučné Slovenské Rozprávky
Odoberajte pre viac rozprávok ❤️
```

### Dizajn

- **Pozadie**: rovnaký gradient ako titulná karta
- **Poďakovanie**: veľký, priateľský font
- **Morál**: menší, kurzívou
- **Výzva na odber**: malý text
- **Trvanie**: 8 sekúnd
- **Animácia**: jemný fade-out do čiernej (1 sekunda)

## 5. Prechody (Transitions)

### Typ prechodu

Používaj **výhradne jemný crossfade (prelínanie)**:

- **Trvanie prechodu**: 1,0–1,5 sekundy
- **Žiadne** ostré rezy
- **Žiadne** špeciálne efekty (wipe, zoom, rotate)
- **Žiadne** blikanie alebo rýchle prechody

### Prečo crossfade

- Je to jemné a neinvazívne
- Neruší detského diváka
- Evokuje „otáčanie stránok" v knižke
- Je technicky jednoduché a spoľahlivé

## 6. FFmpeg konfigurácia

### Základné nastavenia videa

| Parameter | Hodnota | Poznámka |
|-----------|---------|----------|
| **Rozlíšenie** | 1920×1080 (Full HD) | YouTube štandard |
| **FPS** | 30 | Plynulé prechody |
| **Video kodek** | H.264 (libx264) | Univerzálna kompatibilita |
| **Pixel formát** | yuv420p | YouTube kompatibilita |
| **CRF** | 18 | Vysoká kvalita |
| **Preset** | slow | Lepšia kompresia |

### Základné nastavenia audia

| Parameter | Hodnota | Poznámka |
|-----------|---------|----------|
| **Audio kodek** | AAC | YouTube štandard |
| **Bitrate** | 192 kbps | Dobrá kvalita pre hlas |
| **Sample rate** | 44100 Hz | Štandard |
| **Kanály** | Stereo | YouTube odporúčanie |

### FFmpeg príkaz — slideshow s crossfade

Vytvor najprv zoznam vstupov a potom použi komplexný filter:

```bash
# Krok 1: Priprav obrázky na správne rozlíšenie
ffmpeg -i images/scene-01.png -vf "scale=1920:1080:force_original_aspect_ratio=decrease,pad=1920:1080:(ow-iw)/2:(oh-ih)/2:color=#FFFFFF" -y images/scene-01-hd.png

# Krok 2: Vytvor video zo slideshow s crossfade
ffmpeg \
  -loop 1 -t [trvanie1] -i images/scene-01-hd.png \
  -loop 1 -t [trvanie2] -i images/scene-02-hd.png \
  -loop 1 -t [trvanie3] -i images/scene-03-hd.png \
  -i audio/rozpravka.mp3 \
  -filter_complex " \
    [0:v]fade=t=in:st=0:d=1,fade=t=out:st=[trvanie1-1]:d=1[v0]; \
    [1:v]fade=t=in:st=0:d=1,fade=t=out:st=[trvanie2-1]:d=1[v1]; \
    [2:v]fade=t=in:st=0:d=1,fade=t=out:st=[trvanie3-1]:d=1[v2]; \
    [v0][v1][v2]concat=n=3:v=1:a=0[outv]" \
  -map "[outv]" -map 3:a \
  -c:v libx264 -crf 18 -preset slow -pix_fmt yuv420p \
  -c:a aac -b:a 192k \
  -movflags +faststart \
  -y video/rozpravka.mp4
```

### Dôležité FFmpeg vlajky

- `-movflags +faststart` — umožní streamovanie na YouTube
- `-pix_fmt yuv420p` — kompatibilita so všetkými prehrávačmi
- `-crf 18` — vysoká kvalita (nižšie číslo = vyššia kvalita)
- `-preset slow` — lepšia kompresia, dlhší čas kódovania

## 7. Ken Burns efekt (voliteľný)

Pre dynamickejšie video môžeš pridať jemný Ken Burns efekt (pomalý zoom a posun):

```bash
# Jemný zoom-in na obrázok (1.0 → 1.05 za 60 sekúnd)
-vf "zoompan=z='min(zoom+0.0005,1.05)':x='iw/2-(iw/zoom/2)':y='ih/2-(ih/zoom/2)':d=1800:s=1920x1080:fps=30"
```

- Zoom musí byť **veľmi jemný** — maximálne 5 % za celú scénu
- Striedaj zoom-in a zoom-out medzi scénami
- Tento efekt je voliteľný, ale výrazne zlepšuje vizuálny zážitok

## 8. Adresárová štruktúra

```
rozpravky/[id-rozpravky]/
├── video/
│   ├── rozpravka.mp4         # Finálne video
│   ├── title-card.mp4        # Titulná karta
│   ├── end-card.mp4          # Záverečná karta
│   ├── assembly-plan.md      # Plán zostrihania
│   └── metadata.json         # Video metadáta
```

### metadata.json

```json
{
  "resolution": "1920x1080",
  "fps": 30,
  "duration_seconds": 0,
  "duration_formatted": "00:00",
  "codec_video": "H.264",
  "codec_audio": "AAC",
  "file_size_mb": 0,
  "scenes_count": 0,
  "transition_type": "crossfade",
  "transition_duration": 1.0,
  "created": "YYYY-MM-DD"
}
```

## 9. Kontrola kvality

Pred odovzdaním skontroluj:

- [ ] Video sa prehrá bez artefaktov od začiatku do konca
- [ ] Audio a obrázky sú synchronizované — obrázok zodpovedá scéne
- [ ] Prechody sú plynulé a jemné
- [ ] Titulná karta je čitateľná a zobrazí sa na 5 sekúnd
- [ ] Záverečná karta je čitateľná a zobrazí sa na 8 sekúnd
- [ ] Rozlíšenie je 1920×1080
- [ ] Žiadne čierne snímky ani vizuálne chyby
- [ ] Audio nie je orezané ani neobsahuje šum

## 10. Kontrolný zoznam

- [ ] Plán zostrihania je vytvorený a uložený v `assembly-plan.md`
- [ ] Všetky obrázky sú v rozlíšení 1920×1080
- [ ] Titulná karta je vygenerovaná
- [ ] Záverečná karta je vygenerovaná
- [ ] Prechody sú crossfade, 1,0–1,5 sekundy
- [ ] Video je vo formáte H.264, 1080p, 30 fps
- [ ] Audio je AAC, 192 kbps
- [ ] `metadata.json` je vyplnený
- [ ] Video prešlo kontrolou kvality
