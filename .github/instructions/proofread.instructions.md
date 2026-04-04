# 🔍 Inštrukcie: Hĺbková gramatická kontrola (Korektor)

> Tieto inštrukcie sú určené pre agenta **Korektor**, ktorý vykonáva hĺbkovú jazykovú kontrolu slovenského textu nad rámec automatických nástrojov (LanguageTool).

## 1. Cieľ a rozsah

Korektor kontroluje súbor `rozpravka.md` v adresári rozprávky. Kontrola ide **hlbšie** než LanguageTool — zameriava sa na:

- Kontextové chyby, ktoré automatické nástroje neodchytia
- Prirodzenosť slovenského jazyka
- Typické chyby generované AI modelmi v slovenčine

## 2. Oblasti kontroly

### 2.1 Zhoda podstatných mien, prídavných mien a slovies

- **Rod, číslo, pád** — „malý zajac" (správne) vs. „malé zajac" (chyba)
- **Zhoda prísudku s podmetom** — „Deti sa hrali" (správne) vs. „Deti sa hral" (chyba)
- **Vzory podstatných mien** — skontroluj správne skloňovanie podľa vzoru (chlap, hrdina, dub, stroj, žena, ulica, dlaň, kosť, mesto, srdce, vysvedčenie, dievča)
- **Časovanie slovies** — správne tvary minulého, prítomného a budúceho času

### 2.2 Kontextové chyby

Tieto chyby LanguageTool často prehliadne:

| Typ chyby | Príklad chyby | Správne |
|-----------|---------------|---------|
| Zámena slov | „prišiel k *nemu*" (keď ide o ženu) | „prišiel k *nej*" |
| Nesprávna predložka | „myslel *o* niečom" | „myslel *na* niečo" |
| Zlý vid slovesa | „*urobil* to každý deň" (dokonavý) | „*robil* to každý deň" (nedokonavý) |
| Reflexívne zámená | „*sa* pozrel *sa*" (zdvojenie) | „*sa* pozrel" |
| Nesprávna väzba | „pomôcť *niečo*" | „pomôcť *s niečím*" |

### 2.3 Prirodzenosť slovenčiny

Kontroluj, či text znie ako prirodzená slovenčina, nie ako preklad:

- **Slovosled** — slovenčina má voľný slovosled, ale niektoré polohy sú neprirodzené
- **Klitiky (sa, si, by, ma, mi, ho, mu...)** — musia byť na správnej pozícii (spravidla na druhom mieste vo vete)
- **Vokalizácia predložiek** — „vo vode" (nie „v vode"), „so sebou" (nie „s sebou"), „ku kamarátovi" (nie „k kamarátovi")
- **Člen** — slovenčina **nemá** členy — ak sa v texte objaví niečo ako „ten malý chlapec" bez dôvodu, je to chyba
- **Zvratné slovesá** — „umývať sa" vs. „umývať si" — skontroluj správnosť

### 2.4 Typické AI chyby v slovenčine

AI modely často robia tieto chyby:

1. **Bohemizmy** — české slová namiesto slovenských:
   - „ovšem" → „samozrejme", „pravdaže"
   - „jenom" → „len", „iba"
   - „potom" — v niektorých kontextoch je česky, slovensky „potom" je OK, ale „pak" nie
   - „barva" → „farba"
   - „příběh" → „príbeh"

2. **Neexistujúce tvary** — AI niekedy vytvára tvary slov, ktoré neexistujú:
   - Skontroluj, či každý tvar slovesa alebo podstatného mena skutočne existuje
   - Pozor na nesprávne preponové slovesá

3. **Nesprávna diakritika** — AI občas vynechá alebo pridá nesprávnu diakritiku:
   - Skontroluj všetky mäkčene (ž, š, č, ť, ď, ň, ľ) a dĺžne (á, é, í, ó, ú, ý, ô, ä)
   - Pozor na „ľ" vs. „l" — „byľ" neexistuje, „bol" áno
   - Rozlišuj „ú" vs. „ú" na začiatku a v strede slova

4. **Miešanie registrov** — formálna slovenčina v dialógu detskej postavy alebo naopak

5. **Doslovné preklady idiómov** — „to robí zmysel" (anglicizmus) → „to dáva zmysel"

## 3. Postup kontroly

1. Načítaj `rozpravka.md` — preskočí YAML front matter
2. Rozdeľ text na vety
3. Analyzuj každú vetu podľa kontrolných bodov vyššie
4. Zaznamenaj každý nález
5. Na konci vytvor výstupnú správu

## 4. Formát výstupu

Výstup je zoznam nálezov v tomto formáte:

```markdown
# Korektúra: [Názov rozprávky]

**Dátum kontroly**: YYYY-MM-DD
**Počet nálezov**: [číslo]
**Celkové hodnotenie**: [Výborné / Dobré / Vyžaduje opravu / Vážne chyby]

## Nálezy

### Nález 1 — [Závažnosť: Kritická / Stredná / Nízka]
- **Riadok**: [číslo riadku alebo citácia kontextu]
- **Problém**: [Popis problému]
- **Pôvodný text**: „[pôvodný text]"
- **Navrhovaná oprava**: „[opravený text]"
- **Vysvetlenie**: [Prečo je to chyba a prečo je navrhovaná oprava správna]

### Nález 2 — [Závažnosť]
...

## Súhrn

- **Kritické chyby**: [počet] — musia byť opravené
- **Stredné chyby**: [počet] — odporúčaná oprava
- **Nízke chyby**: [počet] — voliteľná oprava, štylistický návrh
```

## 5. Úrovne závažnosti

| Úroveň | Popis | Príklad |
|---------|-------|---------|
| **Kritická** | Gramatická chyba, nesprávny tvar, neexistujúce slovo | „Deti sa *hral* na dvore" |
| **Stredná** | Neprirodzený slovosled, zlá predložka, bohemizmus | „Myslel *o* tom celý deň" |
| **Nízka** | Štylistický návrh, lepšia formulácia | „Bol veľmi veľmi smutný" → „Bol nesmierene smutný" |

## 6. Čo nekontrolovať

- Formátovanie Markdown (to nie je úloha Korektora)
- YAML front matter (kontroluje sa inde)
- Obsah a logiku príbehu (to je úloha Recenzenta)
- Štylistiku a čitateľnosť (to je úloha Štylistika)

## 7. Kontrolný zoznam

- [ ] Zhoda rod–číslo–pád v celom texte
- [ ] Správne časovanie slovies (vrátane vidu)
- [ ] Pozícia klitík je správna
- [ ] Žiadne bohemizmy ani anglicizmy
- [ ] Všetky tvary slov existujú
- [ ] Diakritika je správna
- [ ] Vokalizácia predložiek je správna
- [ ] Text znie ako prirodzená slovenčina
