# 📝 Inštrukcie: Generovanie osnovy rozprávky

> Tieto inštrukcie sú určené pre agenta **Architekt**, ktorý vytvára osnovu (outline) novej rozprávky.

## 1. Kontrola existujúcich tém

Pred generovaním nových nápadov **vždy** skontroluj súbor `katalog.json` v koreňovom adresári projektu:

- Načítaj pole `stories` a prezri všetky existujúce záznamy
- Zameraj sa na polia `theme`, `title`, `moral` a `tags`
- Nový nápad **nesmie** duplikovať existujúcu tému, morál ani zápletku
- Ak je `katalog.json` prázdny (`totalCount: 0`), môžeš pokračovať voľne

## 2. Generovanie nápadov

Vygeneruj **3–5 unikátnych nápadov** na rozprávku. Pre každý nápad:

1. Navrhni pracovný názov a hlavnú tému
2. Porovnaj s existujúcimi rozprávkami v `katalog.json`
3. Ak sa téma alebo morál prekrývajú — zahoď nápad a vygeneruj nový
4. Označ každý nápad ako ✅ unikátny alebo ❌ duplicitný

### Kritériá pre dobrý nápad

- Jasný **morálny odkaz** vhodný pre deti od 6 rokov
- Zaujímavá **zápletka** s prekvapivým zvratom
- Príležitosť pre **poučenie** (nie kázanie)
- Slovenské alebo stredoeurópske **prostredie** (les, dedina, hory, rieka...)
- Možnosť využiť **trojité opakovanie** (klasický rozprávkový motív)

## 3. Štruktúra osnovy

Po výbere najlepšieho nápadu vytvor osnovu v nasledujúcom formáte:

```markdown
# Osnova: [Názov rozprávky]

## Základné informácie

- **Názov**: [Názov rozprávky]
- **Téma**: [Hlavná téma, napr. "odvaha", "priateľstvo", "úcta k prírode"]
- **Morál**: [Jasne formulované ponaučenie, 1–2 vety]
- **Dĺžka**: [čas v minútach] minút (~[počet] slov)
- **Cieľová skupina**: deti od 6 rokov

## Postavy

### [Meno postavy] — [Archetyp]
- **Popis**: [Stručný fyzický popis a povaha]
- **Archetyp**: [napr. Hrdina, Mentor, Pomocník, Protivník, Trickster]
- **Motivácia**: [Čo postava chce/potrebuje]
- **Vývoj**: [Ako sa postava zmení počas príbehu]

### [Ďalšia postava...]

## Scény

### Scéna 1: [Názov scény]
- **Prostredie**: [Kde sa scéna odohráva]
- **Postavy**: [Kto je prítomný]
- **Dej**: [Čo sa deje, 2–3 vety]
- **Účel**: [Čo scéna dosahuje v príbehu]

### Scéna 2: [Názov scény]
...
```

## 4. Počet scén podľa dĺžky

| Dĺžka rozprávky | Počet slov | Počet scén |
|------------------|------------|------------|
| 5 minút          | ~750       | 3–4        |
| 10 minút         | ~1 500     | 5–6        |
| 15 minút         | ~2 250     | 6–8        |
| 20 minút         | ~3 000     | 8–10       |
| 30 minút         | ~4 500     | 10–14      |

> Počítaj priemerne **150 slov za minútu** pre detského poslucháča.

## 5. Archetypy postáv

Použi tieto zavedené archetypy:

- **Hrdina** – hlavná postava, ktorá sa vydáva na cestu
- **Mentor** – múdra postava, ktorá radí (starý otec, lesná víla, múdra sova)
- **Pomocník** – postava, ktorá pomáha hrdinovi (zvieratko, kamarát)
- **Protivník** – postava alebo sila, ktorá stojí v ceste (nie desivá!)
- **Trickster** – šibalská postava, ktorá prináša humor
- **Strážca prahu** – postava, ktorá testuje hrdinu pred pokrokom

## 6. Klasická rozprávková štruktúra

Každá osnova by mala sledovať tento oblúk:

1. **Úvod** – predstavenie hrdinu a jeho sveta
2. **Výzva** – niečo sa zmení, hrdina musí konať
3. **Cesta** – hrdina prekonáva prekážky (trojité opakovanie!)
4. **Kríza** – najväčšia prekážka, moment pochybností
5. **Rozuzlenie** – hrdina uspeje vďaka naučenej lekcii
6. **Záver** – návrat domov, zdieľanie ponaučenia

## 7. Uloženie

Osnovu ulož ako `outline.md` do adresára rozprávky:

```
rozpravky/[id-rozpravky]/outline.md
```

Kde `[id-rozpravky]` je URL-friendly identifikátor (malé písmená, pomlčky, bez diakritiky), napríklad:

- `odvazny-zajacik`
- `strateny-kluc-od-lesa`
- `tri-zelania-rybara`

## 8. Kontrolný zoznam pred odovzdaním

- [ ] Skontrolovaný `katalog.json` — téma je unikátna
- [ ] Morál je jasne formulovaný a vhodný pre deti
- [ ] Všetky postavy majú meno, popis a archetyp
- [ ] Počet scén zodpovedá požadovanej dĺžke
- [ ] Trojité opakovanie je zabudované do scén
- [ ] Osnova je uložená v správnom adresári
- [ ] Dĺžka v minútach a cieľový počet slov sú uvedené
