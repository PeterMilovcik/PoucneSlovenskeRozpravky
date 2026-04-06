# Plán zostrihania: Tomášova zlatá minca

## Základné informácie
- **Audio**: rozpravka.mp3 (8:35, 515 sekúnd)
- **Počet scén**: 6 + titulná karta + záverečná karta
- **Rozlíšenie**: 1920×1080 (Full HD)
- **FPS**: 30
- **Kodek**: H.264 (CRF 18, preset slow) + AAC 192kbps

## Mapovanie obrázkov na audio

| Poradie | Obrázok | Začiatok | Koniec | Trvanie | Scéna v príbehu |
|---------|---------|----------|--------|---------|-----------------|
| 1 | cover-16x9.png (title) | 0:00 | 0:05 | 5s | Titulná karta — názov rozprávky |
| 2 | scene-01.png | 0:05 | 1:15 | 70s | Úvod — Tomáš, jeho izba, hračky, dedko Jano |
| 3 | scene-02.png | 1:15 | 2:45 | 90s | Povala — nájdenie zlatej mince v truhlici |
| 4 | scene-03.png | 2:45 | 4:25 | 100s | Kúzlo — všetko sa mení na zlato, strata hračiek |
| 5 | scene-04.png | 4:25 | 5:45 | 80s | Kubko — zlatý bicykel, smútok, uvedomenie |
| 6 | scene-05.png | 5:45 | 7:10 | 85s | Narodeniny Adama — Tomáš vracia mincu |
| 7 | scene-06.png | 7:10 | 8:27 | 77s | Šťastný koniec — minca zmizne, veci sa vrátia |
| 8 | end-card (generovaný) | 8:19 | 8:27 | 8s | Záverečná karta s poučením |

## Postup zostrihania

1. Všetky scénové obrázky škálované na 1920×1080 (pad white)
2. Titulná karta: cover-16x9.png škálovaná na 1920×1080
3. Záverečná karta: vygenerovaná cez FFmpeg (svetlomodrý gradient + text)
4. Každý obrázok konvertovaný na H.264 klip zodpovedajúcej dĺžky
5. Klipy spojené cez FFmpeg concat demuxer
6. Audio pridané ako AAC 192kbps
7. `-movflags +faststart` pre YouTube streaming
