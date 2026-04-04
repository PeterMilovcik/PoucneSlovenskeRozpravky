# Počítadlo slov

## Popis

Sledovanie počtu slov a odhadovanej dĺžky rozprávky. Táto zručnosť pomáha udržiavať správnu dĺžku príbehu podľa cieľovej minutáže.

## Prepočet

- Priemerná rýchlosť rozprávania: **~140 slov/minútu**
- Cieľová dĺžka sa počíta ako: `počet_minút × 140 = cieľový_počet_slov`

### Tabuľka orientačných dĺžok

| Dĺžka (min) | Počet slov | Typ rozprávky       |
| ------------ | ---------- | -------------------- |
| 3            | ~420       | Krátka rozprávka     |
| 5            | ~700       | Štandardná rozprávka |
| 7            | ~980       | Dlhšia rozprávka     |
| 10           | ~1400      | Rozsiahla rozprávka  |

## Sledovanie počas generovania

### Priebežné hlásenia

Počas generovania textu pravidelne uvádzať stav:

```
📊 Stav: {{aktuálny_počet}} / {{cieľový_počet}} slov ({{percento}}%)
⏱️ Odhadovaná dĺžka: {{odhadované_minúty}} min
```

### Míľniky

- **25%** – Úvod by mal byť dokončený, postavy predstavené
- **50%** – Zápletka by mala byť v plnom prúde
- **75%** – Príbeh smeruje ku vyvrcholeniu
- **90%** – Začať uzatváranie príbehu a morálne ponaučenie

## Upozornenia

### Blíženie sa k cieľovej dĺžke

Keď text dosiahne **80%** cieľového počtu slov:

```
⚠️ UPOZORNENIE: Blížite sa k cieľovej dĺžke ({{aktuálny_počet}}/{{cieľový_počet}} slov).
Začnite uzatvárať príbeh a pripravte záverečné ponaučenie.
```

### Prekročenie maximálnej dĺžky

Keď text prekročí **110%** cieľového počtu slov:

```
🚨 PREKROČENIE: Text presahuje cieľovú dĺžku o {{prekročenie}}%.
Aktuálne: {{aktuálny_počet}} slov | Cieľ: {{cieľový_počet}} slov.
Skráťte text alebo upravte cieľovú dĺžku.
```

## Štrukturálne rozloženie

Odporúčané rozloženie slov podľa častí rozprávky:

| Časť rozprávky      | Podiel | Príklad (5 min / 700 slov) |
| -------------------- | ------ | --------------------------- |
| Úvod                 | 15%    | ~105 slov                   |
| Predstavenie postáv  | 10%    | ~70 slov                    |
| Zápletka             | 40%    | ~280 slov                   |
| Vyvrcholenie         | 20%    | ~140 slov                   |
| Rozuzlenie a poučenie| 15%    | ~105 slov                   |
