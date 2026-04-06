# Copilot Inštrukcie — Poučné Slovenské Rozprávky

> **Tento workspace slúži na generovanie poučných slovenských rozprávok pre deti vo veku 6+.**
> Všetky inštrukcie, komunikácia a generovaný obsah MUSIA byť v slovenskom jazyku (slovenčina).

---

## 1. Kontext projektu

Tento projekt je kreatívny workspace na tvorbu **vzdelávacích rozprávok v slovenčine** pre deti od 6 rokov. Každá rozprávka má:

- Zabaviť dieťa príbehom primeraným jeho veku
- Sprostredkovať jasné morálne poučenie
- Rozvíjať slovnú zásobu a jazykový cit
- Podporovať emocionálny a sociálny rozvoj

**VŽDY** píš, komunikuj a generuj obsah **výhradne v slovenčine**. Ak používateľ zadá prompt v inom jazyku, odpovedaj v slovenčine a rozprávku generuj v slovenčine.

---

## 2. Štruktúra príbehu

Každá rozprávka MUSÍ dodržiavať nasledujúcu šesťčasťovú štruktúru:

### 2.1 Názov

- Krátky, pútavý, zrozumiteľný pre deti
- Naznačuje tému alebo hlavnú postavu
- Príklady: *„Líška, čo sa naučila deliť"*, *„O statočnom ježkovi"*, *„Tri zlaté oriešky"*

### 2.2 Úvod

- Predstavenie hlavnej postavy a prostredia
- Klasické rozprávkové otvorenie (viď sekciu „Štýl písania")
- Stanovenie výchozej situácie a nálady
- Dĺžka: cca 10–15 % celkového textu

### 2.3 Zápletka

- Problém, výzva alebo úloha, ktorú musí hlavná postava riešiť
- Motivácia postavy konať
- Dĺžka: cca 15–20 % celkového textu

### 2.4 Vyvrcholenie

- Najnapínavejší moment príbehu
- Postava čelí hlavnej prekážke
- Tu sa uplatňuje trojité opakovanie (viď nižšie)
- Dĺžka: cca 30–40 % celkového textu

### 2.5 Rozuzlenie

- Vyriešenie problému
- Postava sa mení alebo rastie
- Dĺžka: cca 15–20 % celkového textu

### 2.6 Poučenie

- Explicitné morálne posolstvo
- Zrozumiteľné pre deti od 6 rokov
- Môže byť formulované ako priama veta alebo krátky záverečný odstavec
- Príklad: *„A tak sa líška naučila, že zdieľať s priateľmi je to najkrajšie, čo môžeme urobiť."*
- Dĺžka: cca 5–10 % celkového textu

> **Poznámka k ilustráciám**: Každá scéna, ktorá bude vizuálne ilustrovaná, musí byť jasne oddelená v texte. Pre video je potrebná minimálne 1 ilustrácia na každý významný textový segment (typicky 12–15 scén pre 10-minútovú rozprávku).

---

## 3. Štýl písania

> **⚡ DÔLEŽITÉ**: Kompletný štýlový sprievodca je v súbore `config/writing-style-prompt.md`.
> Tento súbor je **povinné čítanie** pre každého agenta, ktorý generuje text rozprávky.
> Nižšie uvedené pravidlá sú stručný prehľad — plný štýl je v referenčnom súbore.

### 3.1 Rozprávač

- **3. osoba, vševediaci rozprávač** s teplým, priateľským tónom
- Rozprávač môže občas osloviť čitateľa priamo:
  - *„Viete čo sa stalo?"*
  - *„A hádajte, koho tam stretol!"*
  - *„Čo myslíte, podarilo sa mu to?"*

### 3.2 Jazyk

- **Jednoduchý a zrozumiteľný** pre deti od 6 rokov
- **Krátke vety**: priemerne 8–12 slov, maximálne 25 slov na vetu
- **Činný rod** (aktívne slovesá) — vyhýbaj sa trpnému rodu
- **Konkrétne podstatné mená** namiesto abstraktných pojmov
- **Živé opisy** — farby, zvuky, vône, textúry
- Používaj **prirovnania** zrozumiteľné deťom: *„Mačka bola chlpatá ako vankúš."*

### 3.3 Rozprávkové prvky

#### Klasické otvorenia

Používaj tradičné slovenské rozprávkové otvorenia:

- *„Bol raz jeden..."*
- *„Za siedmimi horami, za siedmimi dolinami..."*
- *„Kde bolo, tam bolo..."*
- *„Dávno-predávno, keď ešte..."*
- *„V jednej malej dedinke..."*

#### Trojité opakovanie

Trojité opakovanie je kľúčový rozprávkový prvok. Používaj ho vždy, keď je to vhodné:

- Postava sa trikrát pokúsi o niečo (prvé dva pokusy zlyhajú, tretí uspeje)
- Tri úlohy na splnenie
- Tri postavy s rovnakou výzvou
- Tri predmety alebo dary

**Príklad:**

> Medvedík zaklopal na prvé dvere. „Nemáme med," povedala líška.
> Zaklopal na druhé dvere. „Nemáme med," povedal zajac.
> Zaklopal na tretie dvere. Otvorila babička Sova. „Poď ďalej, maličký. Med mám dosť pre oboch."

#### Klasické zakončenia

- *„A žili šťastne, až kým nepomreli."*
- *„A ak nepomreli, žijú dodnes."*
- *„A rozprávke je koniec."*
- Alebo vlastné pozitívne zakončenie naviazané na poučenie.

---

## 4. Podporované témy

Rozprávky MUSIA podporovať jednu alebo viacero z týchto tém:

| Téma | Príklad zápletky |
|---|---|
| **Priateľstvo** | Zvieratká sa naučia spolupracovať |
| **Odvaha** | Malé zvieratko prekoná svoj strach |
| **Čestnosť** | Postava sa prizná k chybe a napraví ju |
| **Láskavosť** | Pomoc cudziemu prinesie nečakanú odmenu |
| **Trpezlivosť** | Pomalý, ale vytrvalý prístup vedie k cieľu |
| **Úcta k prírode** | Ochrana lesa, rieky, zvierat |
| **Tolerancia** | Prijatie odlišnosti ako hodnoty |
| **Vytrvalosť** | Nevzdávať sa po neúspechu |
| **Zodpovednosť** | Staranie sa o niečo/niekoho |
| **Vďačnosť** | Uvedomenie si hodnoty toho, čo máme |

---

## 5. Zakázaný obsah

Nasledujúci obsah je **PRÍSNE ZAKÁZANÝ** v akejkoľvek rozprávke:

### 5.1 Absolútne zakázané

- ❌ **Násilie** — žiadne bitkové scény, zranenia, krv
- ❌ **Krutosť voči zvieratám** — žiadne ubližovanie, zanedbávanie, týranie
- ❌ **Smrť bez spracovania** — ak sa téma smrti vyskytne, musí byť citlivo spracovaná s nádejou a vysvetlením
- ❌ **Opustenie** — žiadne opustenie dieťaťa rodičmi bez vyriešenia
- ❌ **Stereotypy** — rodové, etnické, sociálne alebo iné predsudky
- ❌ **Nadmerný strach** — žiadne hororové prvky, desivé popisy, beznádejné situácie

### 5.2 Štylisticky zakázané

- ❌ **Irónia a sarkazmus** — deti vo veku 6+ ich nerozumejú
- ❌ **Nadmerný trpný rod** — používaj činný rod
- ❌ **Abstraktná filozofia** — žiadne zložité úvahy neprimerané veku
- ❌ **Dlhé súvetia** — maximálne 25 slov na vetu
- ❌ **Cudzie slová** bez vysvetlenia — ak je nutné, vysvetli v texte

---

## 6. Výpočet dĺžky

### Rýchlosť čítania

Pre detské rozprávky čítané nahlas sa počíta s rýchlosťou **130–150 slov za minútu**. Pre TTS (ElevenLabs) počítaj s rýchlosťou ~135 slov/min.

### Predvolená dĺžka

- **Predvolená**: 10–15 minút (1 300–2 250 slov)
- **Rozsah**: 5–30 minút (650–4 500 slov)

### Tabuľka dĺžok

| Čas čítania | Min. slov | Max. slov | Typ |
|---|---|---|---|
| 5 min | 650 | 750 | Krátka rozprávka |
| 10 min | 1 300 | 1 500 | Štandardná rozprávka |
| 15 min | 1 950 | 2 250 | Dlhšia rozprávka |
| 20 min | 2 600 | 3 000 | Rozprávka s kapitolami |
| 30 min | 3 900 | 4 500 | Dlhá rozprávka s kapitolami |

Ak používateľ zadá požadovaný čas, prepočítaj dĺžku podľa tejto tabuľky.

---

## 7. Konvencie súborov

### 7.1 Adresárová štruktúra

Každá rozprávka má vlastný adresár v priečinku `rozpravky/`:

```
rozpravky/
└── YYYY-MM-DD-slug-nazov/
    ├── rozpravka.md          # Hlavný text rozprávky
    ├── outline.md            # Osnova / štruktúra príbehu
    ├── metadata.json         # Metadáta rozprávky
    ├── kapitoly/             # Iba pre rozprávky > 15 min
    │   ├── 01-nazov.md
    │   ├── 02-nazov.md
    │   └── ...
    ├── audio/
    │   ├── rozpravka.mp3
    │   ├── clean-text.txt    # TTS text bez páuz (pre video sync)
    │   └── metadata.json
    ├── images/
    │   ├── cover-16x9.png
    │   ├── scene-01.png ... scene-14.png
    │   ├── preview/          # Náhľady 800px JPG
    │   └── prompts.md
    ├── video/
    │   ├── rozpravka.mp4
    │   └── assembly-plan.json
    └── publish/
        ├── youtube-metadata.json
        ├── youtube-result.json
        └── publish-log.md
```

**Formát slug-u**: malé písmená, bez diakritiky, slová oddelené pomlčkami.

**Príklad**: `rozpravky/2025-01-15-o-statocnom-jezkovi/`

### 7.2 Súbor `rozpravka.md`

Hlavný súbor rozprávky s YAML front matter:

```markdown
---
nazov: "O statočnom ježkovi"
tema: ["odvaha", "priateľstvo"]
vekova_skupina: "6+"
cas_citania_min: 12
pocet_slov: 1620
poucenie: "Aj ten najmenší môže byť najstatočnejší."
datum_vytvorenia: "2025-01-15"
autor: "Copilot"
verzia: 1
---

# O statočnom ježkovi

Bol raz jeden malý ježko menom Bodko. Žil na okraji veľkého lesa...

## Poučenie

Aj ten najmenší môže byť najstatočnejší. Dôležité nie je, aký si veľký, ale aké veľké máš srdce.
```

### 7.3 Súbor `outline.md`

Osnova príbehu pred generovaním textu:

```markdown
# Osnova: O statočnom ježkovi

## Nastavenia
- **Téma**: odvaha, priateľstvo
- **Cieľová dĺžka**: 12 minút (~1 620 slov)
- **Prostredie**: Les na úpätí hôr, jeseň
- **Hlavná postava**: Ježko Bodko — malý, bojazlivý, dobrosrdečný

## Štruktúra

### 1. Úvod (~200 slov)
- Predstavenie Bodka a jeho lesného domova
- Bodko je najmenší zo všetkých lesných zvieratiek

### 2. Zápletka (~250 slov)
- V lese vyschne studnička — zvieratá nemajú vodu
- Nikto sa neodváži ísť cez Temný les hľadať nový prameň

### 3. Vyvrcholenie (~600 slov)
- Bodko sa rozhodne ísť sám (trojité opakovanie — tri prekážky)
  - 1. prekážka: Hlboký potok — prebrodiť sa cez kamene
  - 2. prekážka: Hustý trnitý krík — Bodko sa pretlačí cez tŕne
  - 3. prekážka: Vysoká skala — s pomocou motýľa nájde priechod

### 4. Rozuzlenie (~350 slov)
- Bodko nájde prameň a privedie zvieratá k vode
- Zvieratá uznajú jeho statočnosť

### 5. Poučenie (~100 slov)
- „Aj ten najmenší môže byť najstatočnejší."
```

### 7.4 Súbor `metadata.json`

```json
{
  "nazov": "O statočnom ježkovi",
  "slug": "o-statocnom-jezkovi",
  "datum_vytvorenia": "2025-01-15",
  "tema": ["odvaha", "priateľstvo"],
  "vekova_skupina": "6+",
  "cas_citania_min": 12,
  "pocet_slov": 1620,
  "poucenie": "Aj ten najmenší môže byť najstatočnejší.",
  "postavy": [
    {
      "meno": "Bodko",
      "typ": "ježko",
      "rola": "hlavná postava"
    }
  ],
  "prostredie": "Les na úpätí hôr",
  "obdobie": "jeseň",
  "verzia": 1,
  "qa_skore": null,
  "stav": "koncept"
}
```

### 7.5 Kapitoly (pre rozprávky > 15 minút)

Pre dlhšie rozprávky sa text rozdelí do kapitol:

```
kapitoly/
├── 01-bodko-a-jeho-domov.md
├── 02-vysychajuca-studnicka.md
├── 03-cesta-cez-temny-les.md
└── 04-novy-pramen.md
```

Každá kapitola má vlastný YAML front matter:

```markdown
---
kapitola: 1
nazov_kapitoly: "Bodko a jeho domov"
pocet_slov: 400
---

# Kapitola 1: Bodko a jeho domov

Bol raz jeden malý ježko menom Bodko...
```

---

## 8. Pracovný postup (Workflow)

### 8.1 Poradie krokov

Pri generovaní každej rozprávky VŽDY dodržuj tento postup:

1. **Analýza zadania** — Pochop tému, požadovanú dĺžku, vekový cieľ
2. **Kontrola jedinečnosti** — Skontroluj `katalog.json`, či podobná rozprávka ešte neexistuje
3. **Generovanie osnovy** (`outline.md`) — Vytvor podrobnú osnovu podľa šesťčasťovej štruktúry
4. **Schválenie osnovy** — Predlož osnovu na kontrolu
5. **Generovanie textu** (`rozpravka.md`) — Napíš rozprávku podľa schválenej osnovy
6. **Kontrola kvality** — Spusti kontrolné skills (viď sekciu 10)
7. **Vytvorenie metadát** (`metadata.json`) — Vyplň všetky polia
8. **Finalizácia** — Aktualizuj `katalog.json`
9. **Generovanie audia** — Príprava textu pre TTS, generovanie v ElevenLabs
10. **Generovanie ilustrácií** — Cover + scény cez GPT Image s referenčným obrázkom
11. **Review ilustrácií** — Vizuálna kontrola logiky, konzistencie postáv a štýlu
12. **Zostavenie videa** — `scripts/build-video.py` pre audio-synced slideshow
13. **Publikácia na blog** — GitHub Pages s optimalizovanými JPG obrázkami
14. **Publikácia na YouTube** — Upload cez YouTube Studio, thumbnail, metadáta
15. **Aktualizácia katalógu** — Finálny stav a URL vo všetkých záznamoch

### 8.2 Kontrola jedinečnosti

Pred generovaním novej rozprávky skontroluj `katalog.json`:

- Rovnaká alebo veľmi podobná téma
- Rovnaká hlavná postava rovnakého typu
- Rovnaké prostredie + téma kombinácia
- Ak sa nájde zhoda, navrhni úpravu témy alebo originálny twist

---

## 9. Hybridný prístup generovania

### 9.1 Krátke rozprávky (≤ 15 minút, do ~2 250 slov)

Použi **jednorázové generovanie (single-pass)**:

1. Vygeneruj celý text naraz v jednom súbore `rozpravka.md`
2. Skontroluj konzistentnosť, dĺžku a kvalitu
3. Oprav prípadné problémy

### 9.2 Dlhé rozprávky (> 15 minút, nad ~2 250 slov)

Použi **paralelné generovanie kapitol + zlúčenie**:

1. Vygeneruj detailnú osnovu s rozdelením na kapitoly
2. Vygeneruj každú kapitolu samostatne (paralelne) v priečinku `kapitoly/`
3. Skontroluj konzistentnosť medzi kapitolami:
   - Mená postáv sa zhodujú
   - Časová línia je logická
   - Tón a štýl sú konzistentné
   - Zápletka plynulo pokračuje
4. Zlúč kapitoly do finálneho `rozpravka.md`
5. Spusti finálnu kontrolu kvality

### 9.3 Generovanie ilustrácií

Použi **GPT Image (gpt-image-1)** s referenčným obrázkom pre konzistentnosť:

1. Vygeneruj **obálku** (cover) ako prvý obrázok bez referencie
2. Vygeneruj **všetky scény** s obálkou ako referenčným obrázkom (cez `images.edit()`)
3. Vytvor **preview náhľady** (800px JPG) pre rýchlu kontrolu
4. **Vizuálna kontrola**: logika scén, konzistencia postáv, štýl
5. Pregeneruj chybné obrázky
6. Optimalizuj pre blog: PNG → JPG 1200px

**Skript**: `python scripts/generate-images.py --story-dir [cesta] --all`

---

## 10. Povinné kontrolné Skills

Pri generovaní KAŽDEJ rozprávky spusti nasledujúce skills:

### 10.1 `grammar-check` — Gramatická kontrola

- Kontrola pravopisu a gramatiky slovenčiny
- Kontrola interpunkcie
- Kontrola správnych tvarov slov (skloňovanie, časovanie)

### 10.2 `word-count` — Počet slov

- Overenie, že celkový počet slov zodpovedá požadovanému času čítania
- Kontrola pomeru sekcií (Úvod / Zápletka / Vyvrcholenie / Rozuzlenie / Poučenie)
- Kontrola priemernej dĺžky vety (cieľ: 8–12 slov, maximum 25 slov)

### 10.3 `style-guard` — Strážca štýlu

- Detekcia trpného rodu — nahradiť činným
- Detekcia príliš dlhých viet — rozdeliť
- Detekcia abstraktných pojmov — nahradiť konkrétnymi
- Detekcia nevhodného tónu (irónia, sarkazmus)
- Overenie prítomnosti rozprávkových prvkov (otvorenie, trojité opakovanie, zakončenie)

### 10.4 `age-check` — Veková primeranosť

- Kontrola slovnej zásoby — slová musia byť zrozumiteľné pre deti 6+
- Kontrola zakázaného obsahu (viď sekciu 5)
- Kontrola emočnej primeranosti — žiadne scény vyvolávajúce úzkosť
- Kontrola zložitosti viet

---

## 11. Kvalitná brána (Quality Gate)

Každá rozprávka MUSÍ prejsť **trojstupňovou kontrolou kvality** a dosiahnuť skóre **≥ 80 zo 100**.

### Stupeň 1: Automatické Skills (viď sekciu 10)

| Skill | Váha | Kritérium pre úspech |
|---|---|---|
| `grammar-check` | 25 % | Žiadne gramatické chyby |
| `word-count` | 15 % | Dĺžka v tolerancii ±10 % |
| `style-guard` | 30 % | Max. 2 drobné štýlové odchýlky |
| `age-check` | 30 % | Žiadny zakázaný obsah, vek-primerané |

### Stupeň 2: C# Pipeline

- Spustenie automatizovaného C# pipeline, ktorý overí:
  - YAML front matter je kompletný a validný
  - `metadata.json` je konzistentný s `rozpravka.md`
  - Štruktúra súborov dodržiava konvencie
  - Počet slov zodpovedá deklarovanému času čítania

### Stupeň 3: Agent Review

- Agent prečíta rozprávku ako celok a hodnotí:
  - **Príbehová logika**: Je príbeh konzistentný a logický?
  - **Emocionálny oblúk**: Má postava jasný vývoj?
  - **Poučenie**: Je morálne posolstvo jasné a prirodzene vyplýva z príbehu?
  - **Čitateľnosť**: Je text plynulý a pútavý?
  - **Originalita**: Je príbeh dostatočne originálny?

### Výsledné skóre

```
Celkové skóre = (Skills × 0.4) + (Pipeline × 0.2) + (Agent Review × 0.4)
```

- **≥ 80/100** — Rozprávka je schválená ✅
- **60–79/100** — Potrebné opravy, konkrétne odporúčania sa vygenerujú
- **< 60/100** — Rozprávka sa prepíše od osnovy

---

## 12. Rýchla referencia

### Kontrolný zoznam pred odovzdaním

- [ ] Text je výhradne v slovenčine
- [ ] Dodržaná šesťčasťová štruktúra (Názov → Úvod → Zápletka → Vyvrcholenie → Rozuzlenie → Poučenie)
- [ ] Klasické rozprávkové otvorenie
- [ ] Trojité opakovanie je prítomné
- [ ] Poučenie je explicitné a zrozumiteľné
- [ ] Priemerná dĺžka vety: 8–12 slov (max. 25)
- [ ] Činný rod (nie trpný)
- [ ] Žiadny zakázaný obsah
- [ ] Počet slov zodpovedá času čítania (130–150 slov/min)
- [ ] YAML front matter je kompletný
- [ ] `metadata.json` je vyplnený
- [ ] `outline.md` existuje
- [ ] Kontrola proti `katalog.json` prebehla
- [ ] QA skóre ≥ 80/100
- [ ] Ilustrácie prešli vizuálnou kontrolou (logika, konzistencia, štýl)
- [ ] Video je synchronizované s audiom (±1s tolerancia)
- [ ] Blog obrázky sú optimalizované (JPG, 1200px)
- [ ] YouTube video je publikované s metadátami a thumbnailom

### Príklad promptu pre novú rozprávku

```
Vytvor rozprávku na tému „priateľstvo" pre deti 6+.
Čas čítania: 10 minút.
Prostredie: slovenská dedinka v lete.
Hlavné postavy: dva psíky — veľký a malý.
```
