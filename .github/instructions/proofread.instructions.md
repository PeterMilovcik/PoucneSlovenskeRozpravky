# 🔍 Inštrukcie: Hĺbková kontrola slovenského pravopisu (Korektor)

> Tieto inštrukcie sú určené pre agenta **Korektor**, ktorý vykonáva hĺbkovú jazykovú a pravopisnú kontrolu slovenského textu nad rámec automatických nástrojov (LanguageTool).
>
> **Referenčná norma**: Pravidlá slovenského pravopisu (PSP), 4. vydanie, Veda 2013. Kodifikačná príručka podľa zákona NR SR č. 270/1995 Z. z.

## 1. Cieľ a rozsah

Korektor kontroluje súbor `rozpravka.md` v adresári rozprávky. Kontrola ide **hlbšie** než LanguageTool — zameriava sa na:

- Pravopisné pravidlá špecifické pre slovenčinu
- Kontextové chyby, ktoré automatické nástroje neodchytia
- Prirodzenosť slovenského jazyka
- Typické chyby generované AI modelmi v slovenčine

---

## 2. PRAVOPISNÉ PRAVIDLÁ SLOVENČINY

### 2.1 Vybrané slová (i/y po obojakých spoluhláskach)

Po obojakých spoluhláskach **b, m, p, r, s, v, z** sa vo vybraných slovách a ich odvodeninkách píše **y/ý**. Vo všetkých ostatných slovách po obojakých spoluhláskach sa píše **i/í**.

#### Kompletný zoznam vybraných slov (zapamätaj si ich — používaj pri kontrole):

| Spoluhláska | Vybrané slová (hlavné) |
|-------------|----------------------|
| **B** | byť (jestvovať), bývať, bydlo, byt, bytosť, bývalý, býk, bystrina, bystrý, bylina, byľ, byvol, kobyla, obyčaj, dobyť, nábytok, dobytok, zbytočný |
| **M** | my, mykať, mýliť sa, myslieť, myseľ, myš, myť, mydlo, mýto, hmyz, smyk, šmýkať |
| **P** | pýcha, pyšný, pykať, pýr, pysk, pýtať sa, pytliak, kopyto, pyžamo |
| **R** | ryba, rýchly, rýdzi, ryha, rys, ryť, rytier, ryža, koryto, kryť, strýko, trýzniť, hrýzť, prýštiť, bryndza |
| **S** | syn, sypať, syr, sýty, sýkorka, syčať, sychravý, syseľ |
| **V** | vy, vykať, vydra, výr, výskať, vysoký, vyť, zvyk, zvyšok |
| **Z** | jazyk, nazývať, pozývať, ozývať sa |

**Pravidlo**: Ak slovo nie je v zozname vybraných slov ani nie je od nich odvodené, po obojakej spoluhláske píš **i/í**.

**Typické chyby**:
- ❌ „byt" (keď myslíme „biť/udierať") — „biť" je s i (nie je vybrané slovo)
- ❌ „mýr" — neexistuje; „mier" je správne
- ❌ „sýn" — „syn" je bez dĺžňa
- ❌ „rýba" — „ryba" je bez dĺžňa na y

### 2.2 Rytmický zákon (rytmické krátenie)

**Pravidlo**: V slovenčine po dlhej slabike nasleduje krátka slabika. Dve dlhé slabiky za sebou sa v slovenčine spravidla nevyskytujú.

Za **dlhé slabiky** sa považujú:
- slabiky s dlhou samohláskou (á, é, í, ó, ú, ý)
- slabiky s dvojhláskou (ia, ie, iu, ô)
- slabiky s dlhým ĺ alebo ŕ

**Príklady**:
- knihám (dlhé á, lebo predchádzajúca slabika je krátka) → ale: láskam (krátke a, lebo „lás-" je dlhá)
- krásny (krátke y, lebo „krás-" je dlhá) → ale: múdry je výnimka
- pekného → ale: krásneho (nie *krásného)
- chválim → ale: líšim (nie *líším — pozor, „líš" je dlhá slabika)

**Výnimky z rytmického zákona** (zapamätaj — tieto sú povolené):
- Prípona **-ár** v niektorých slovách: *kuchár* (nie kuchár→kuchar)
- Prípona **-áreň**: *pekáreň*
- Prípona **-ián**: *kresťan* → ale prípona -ian: *Slovákian* nie!
- Zloženiny a niektoré cudzie slová
- Slová s predponami (predpona sa nepočíta): *ná-ležitý* — je OK

**Typické AI chyby**:
- ❌ „krásných" → správne „krásnych" (rytmické krátenie)
- ❌ „líšíme" → správne „líšime"
- ❌ „bábätká" → správne „bábätka"

### 2.3 Predpony s-/z-/zo-

**Pravidlo**:
- **s-** = pohyb zhora nadol, z povrchu preč, dokopy: *spadnúť, stiahnuť, sceliť, schovať*
- **z-** = zmena stavu, dokončenie deja: *zmeniť, zobudiť, zničiť, zrobiť, zjesť*
- **zo-** = vokalizovaná podoba z-: *zobrať, zobudiť, zohrať, zostať, zostaviť*

**Časté chyby**:
- ❌ „zpäť" → správne „späť" (s- = smer preč)
- ❌ „smiznúť" → správne „zmiznúť" (z- = zmena stavu)
- ❌ „zceliť" → správne „sceliť" (s- = dokopy)
- ❌ „schladnúť" → správne „zchladnúť"... nie, „schladnúť" je správne (s- pohyb z tepla)
  - Pozor, niektoré slovesá majú ustálenú predponu — overiť v slovníku!

### 2.4 Písanie čiarky — kľúčové pravidlá

#### Povinná čiarka PRED:
- **že**: „Vedel, *že* príde."
- **aby**: „Chcel, *aby* prišiel."
- **ktorý/á/é**: „Chlapec, *ktorý* bežal..."
- **keď**: „Vedel, *keď* príde." (ale nie vždy na začiatku vety!)
- **kde**: „Dom, *kde* býval..."
- **ako** (v porovnávacom význame po komparatíve): „väčší, *ako* si myslel"
- **lebo, pretože, preto, hoci, aj keď**
- **ale, no, avšak, však** (odporovacie)

#### Čiarka sa NEPÍŠE pred:
- **a, i, aj, ani** v jednoduchom vymenúvaní: „jablká a hrušky"
- **ako** v ustálených spojeniach: „biely ako sneh" (nie *„biely, ako sneh")
- **a** na začiatku vety v rozprávkovom texte: „A potom sa vrátil." (nie *„A, potom...")

#### Čiarka pri priamej reči:
- „Poď sem," povedal otec. (čiarka pred zatváracou úvodzovkou)
- „Poď sem!" zavolal. (výkričník nahrádza čiarku)
- „Kam ideš?" spýtala sa mama. (otáznik nahrádza čiarku)

### 2.5 Vokalizácia predložiek

Predložky **v, z, s, k** sa menia na **vo, zo, so, ku** pred určitými skupinami spoluhlások:

| Predložka | Vokalizovaná | Kedy |
|-----------|-------------|------|
| **v** → **vo** | pred v, f, a skupinami spoluhlások: „vo vode", „vo vreci", „vo februári" |
| **z** → **zo** | pred z, ž, š, s a skupinami: „zo zeme", „zo sna", „zo žartu", „zo školy" |
| **s** → **so** | pred s, š, z, ž a skupinami: „so sebou", „so slzami", „so synom" |
| **k** → **ku** | pred k, g a skupinami: „ku kamarátovi", „ku kvetu", „ku dnu" |

**Typické AI chyby**:
- ❌ „v vode" → správne „vo vode"
- ❌ „s sebou" → správne „so sebou"
- ❌ „k kamarátovi" → správne „ku kamarátovi"
- ❌ „z zeme" → správne „zo zeme"

### 2.6 Písanie veľkých písmen

- **Vlastné mená** osôb, zvierat, miest: „Tomáš, Kubko, Bratislava"
- **Prvé slovo vo vete** a po bodke, výkričníku, otázniku
- **Oslovenia v listoch**: „Milý Tomáš" — veľké M
- **Ulice, námestia**: „Ulica osloboditeľov" (veľké len prvé slovo od PSP 1991)
- **Sviatky**: „Vianoce, Veľká noc"
- **NEPÍŠE SA** veľké písmeno: názvy mesiacov, dní v týždni, národností (prídavné mená): „slovenský, pondelok, január"

### 2.7 Písanie i/í po mäkkých a tvrdých spoluhláskach

| Typ | Spoluhlásky | Pravidlo |
|-----|------------|---------|
| **Mäkké** | č, dž, ž, š, c, dz, j, ď, ť, ň, ľ | Vždy **i/í** (nikdy y): „čisto, žiadny, šikovný" |
| **Tvrdé** | d, t, n, l, h, ch, k, g | Vždy **y/ý** (nie i): „dym, tyč, nohy, lyže" |
| **Obojaké** | b, m, p, r, s, v, z, f | Podľa vybraných slov (viď 2.1) |

**Výnimky po tvrdých spoluhláskach** — i/í sa píše v:
- cudzie slová: „kino, gitara, hipopotam, chirurg"
- po **d, t, n, l** v mäkkých tvaroch: „dieťa" (ď+ie), „deti" (ď+e+ť+i)... ale toto je vlastne ď, ť, ň, ľ

**Typické chyby**:
- ❌ „chiba" → správne „chyba" (ch je tvrdé → y)
- ❌ „žyť" → správne „žiť" (ž je mäkké → i)
- ❌ „čyslo" → správne „číslo" (č je mäkké → í)

### 2.8 Diakritika — mäkčeň a dĺžeň

**Mäkčeň** (háčik): ž, š, č, ť, ď, ň, ľ, dž, dz
- Rozlišuje význam: „mat" (v šachu) vs. „mať" (matka)
- ď, ť, ň, ľ sa pred e, i, í, ia, ie, iu NEPÍŠU s mäkčeňom: „deti" (nie *ďeti), „niečo" (nie *ňiečo)
- ď, ť, ň, ľ sa PÍŠU s mäkčeňom pred a, o, u a na konci slova: „ďalší, ťava, kôň, soľ"

**Dĺžeň**: á, é, í, ó, ú, ý, ĺ, ŕ
- Rozlišuje význam: „rad" (poradie) vs. „rád" (s radosťou)
- Rešpektuj rytmický zákon (viď 2.2)

**Dvojhlásky**: ia, ie, iu, ô
- Počítajú sa ako **jedna dlhá slabika**
- „ô" sa vyskytuje len v koreni slov: „kôň, stôl, dôm, vôňa"

**Špeciálne** „ä":
- Vyskytuje sa len po **b, m, p, v**: „päť, mäso, väčší, bábätko"
- V ostatných pozíciách sa píše „e" alebo „a"

---

## 3. GRAMATICKÉ PRAVIDLÁ

### 3.1 Zhoda podstatných mien, prídavných mien a slovies

- **Rod, číslo, pád** — „malý zajac" (správne) vs. „malé zajac" (chyba)
- **Zhoda prísudku s podmetom** — „Deti sa hrali" (správne) vs. „Deti sa hral" (chyba)
- **Vzory podstatných mien** — skontroluj správne skloňovanie podľa vzoru:
  - Mužský rod: chlap, hrdina, dub, stroj
  - Ženský rod: žena, ulica, dlaň, kosť
  - Stredný rod: mesto, srdce, vysvedčenie, dievča
- **Časovanie slovies** — správne tvary minulého, prítomného a budúceho času

### 3.2 Skloňovanie — pády po predložkách

**Toto je kritická oblasť**, kde AI modely často chybujú. Skontroluj, či podstatné meno za predložkou je v správnom páde:

| Predložka | Pád | Príklad |
|-----------|-----|---------|
| **na** + akuzatív (kam/smer) | 4. pád | „idem **na povalu**" (nie „na poval" — „povala" je ženský rod) |
| **na** + lokál (kde/miesto) | 6. pád | „som **na povale**", „na **poličke**" |
| **v/vo** + lokál | 6. pád | „v **izbe**", „vo **vode**" |
| **do** + genitív | 2. pád | „do **dielne**", „do **lesa**" |
| **z/zo** + genitív | 2. pád | „z **cukrárne**", „zo **školy**" |
| **s/so** + inštrumentál | 7. pád | „s **Kubkom**", „so **sladkosťami**" |
| **k/ku** + datív | 3. pád | „ku **kamarátovi**", „k **poličkám**" |
| **pri** + lokál | 6. pád | „pri **potoku**", „pri **bráne**" |
| **za** + inštrumentál (kde) | 7. pád | „za **domom**" |
| **za** + akuzatív (kam) | 4. pád | „za **dom**" |
| **medzi** + akuzatív (kam) | 4. pád | „medzi **kmene**" |
| **medzi** + inštrumentál (kde) | 7. pád | „medzi **kmeňmi**" |
| **cez** + akuzatív | 4. pád | „cez **ulice**" |
| **po** + lokál | 6. pád | „po **tajnej ceste**" |
| **pod** + inštrumentál (kde) | 7. pád | „pod **horami**" |
| **nad** + inštrumentál (kde) | 7. pád | „nad **mestečkom**" |

**Typické chyby**:
- ❌ „idem na poval" → ✅ „idem na povalu" (povala = ženský rod, akuzatív = povalu)
- ❌ „sedel na stol" → ✅ „sedel na stole" (lokál) alebo „sadol na stôl" (akuzatív smeru)
- ❌ „v les" → ✅ „v lese" (lokál) alebo „do lesa" (genitív smeru)

**Postup pri kontrole**:
1. Nájdi každú predložku v texte
2. Urči, či vyjadruje **smer** (akuzatív) alebo **miesto** (lokál/inštrumentál)
3. Over, že nasledujúce podstatné meno je v správnom páde
4. Skontroluj, že aj prídavné mená a zámená v danej fráze súhlasia v páde

### 3.3 Kontextové chyby

Tieto chyby LanguageTool často prehliadne:

| Typ chyby | Príklad chyby | Správne |
|-----------|---------------|---------|
| Zámena slov | „prišiel k *nemu*" (keď ide o ženu) | „prišiel k *nej*" |
| Nesprávna predložka | „myslel *o* niečom" | „myslel *na* niečo" |
| Zlý vid slovesa | „*urobil* to každý deň" (dokonavý) | „*robil* to každý deň" (nedokonavý) |
| Reflexívne zámená | „*sa* pozrel *sa*" (zdvojenie) | „*sa* pozrel" |
| Nesprávna väzba | „pomôcť *niečo*" | „pomôcť *s niečím*" |

### 3.4 Pozícia klitík

Klitiky (sa, si, by, ma, mi, ho, mu, ju, ťa, ti...) musia byť spravidla na **druhom mieste** vo vete:

- ✅ „Tomáš **sa** pozrel na horu."
- ✅ „Pozrel **sa** na horu."
- ❌ „Tomáš pozrel **sa** na horu." (klitika príliš ďaleko)
- ✅ „A potom **sa** usmial."
- ❌ „A potom usmial **sa**." (klitika na konci)

---

## 4. PRIRODZENOSŤ SLOVENČINY

### 4.1 Typické AI chyby v slovenčine

#### Bohemizmy (české slová namiesto slovenských)

| ❌ České / bohemizmus | ✅ Slovenské |
|----------------------|-------------|
| ovšem | samozrejme, pravdaže |
| jenom, pouze | len, iba |
| pak | potom |
| barva | farba |
| příběh | príbeh |
| okamžik | okamih, chvíľa |
| záležitosť | záležitosť je OK, ale pozor na „věc" → „vec" |
| třeba | treba (s jedným r) |
| polévka | polievka |
| doporučiť | odporúčať (nie doporučiť) |
| zapomenúť | zabudnúť |
| drahý (cenový) | drahý je OK, ale „laciný" nie „levný" |
| obzvlášť | obzvlášť je OK, ale pozor na „zvlášt" → „zvlášť" |

#### Anglicizmy

| ❌ Anglicizmus | ✅ Slovenské |
|---------------|-------------|
| to robí zmysel | to dáva zmysel |
| mať fun | baviť sa |
| candy bar | sladký kútik / stôl so sladkosťami |
| cool | skvelý, super |
| fake | falošný, nepravý |

#### Neexistujúce tvary

AI niekedy vytvára tvary slov, ktoré neexistujú:
- Skontroluj, či každý tvar slovesa alebo podstatného mena skutočne existuje
- Pozor na nesprávne preponové slovesá: ❌ „zažnal" (neexistuje) → ✅ „zahnal"
- Pozor na nesprávne prípony: ❌ „narodeninkovú" → ✅ „narodeninovú"

#### Nesprávna diakritika

- Skontroluj všetky mäkčene (ž, š, č, ť, ď, ň, ľ) a dĺžne (á, é, í, ó, ú, ý, ô, ä)
- Pozor na „ľ" vs. „l" — „byľ" (rastlina) vs. „bol" (byť)
- Rozlišuj „ú" na začiatku slova vs. v strede: „úloha" ale „kúpiť"

#### Miešanie registrov

- Formálna slovenčina v dialógu detskej postavy je chyba
- Hovorová slovenčina vo vyrozprávaní je chyba
- Zachovaj konzistentný register: rozprávkový jazyk s občasným detským dialógom

---

## 5. TYPOGRAFIA

### 5.1 Úvodzovky

V slovenčine sa používajú **dolné a horné** úvodzovky:
- ✅ „text" (dolné otváracie „, horné zatváracie ")
- ❌ "text" (anglické úvodzovky)
- ❌ «text» (francúzske úvodzovky)
- Vnorené úvodzovky: „povedal ‚ahoj' a odišiel"

### 5.2 Pomlčka a spojovník

- **Spojovník** (-): spája slová: „modro-zelený, Banská Bystrica"
- **Pomlčka** (–): oddeľuje časti vety: „Prišiel domov – bolo už neskoro."
- **Dlhá pomlčka** (—): v slovenčine sa nepoužíva

### 5.3 Trojbodka

- Používaj typografický znak **…** (jeden znak), nie tri bodky **...**
- „Neviem…" (bez medzery pred trojbodkou v rámci slova)
- „Neviem, ale…" alebo „A potom… sa stalo niečo."

---

## 5. LOGICKÁ KONTROLA TEXTU

Okrem jazykových chýb kontroluj aj **logickú konzistenciu** textu. AI modely často generujú text, kde jednotlivé vety sú gramaticky správne, ale celok je logicky nezmyselný.

### 5.1 Priestorová logika

Skontroluj, či predmety a postavy sú na miestach, ktoré dávajú zmysel:

- ❌ „Na poličke stál drevený rytier. Vedľa neho ležal bicykel." — bicykel nemôže ležať na poličke
- ❌ „V skrinke stálo auto." — auto sa nezmestí do skrinky (pokiaľ nie je hračkárske — kontext!)
- ✅ „Na poličke stál drevený rytier. Pri plote pred domom stál starý bicykel." — logicky oddelené miesta

**Postup**: Pri každom opise miesta/predmetu si over:
1. Je fyzicky možné, aby predmet bol na danom mieste?
2. Je veľkosť predmetu kompatibilná s miestom?
3. Ak sú dva predmety „vedľa seba", sú na rovnakom type miesta?

### 5.2 Časová logika

- Denná doba musí plynúť logicky (ráno → deň → večer)
- Ak postava niekam ide, musí prejsť realistický čas
- Udalosti musia nasledovať v logickom poradí
- Ročné obdobie a počasie musia byť konzistentné

### 5.3 Konzistencia postáv a predmetov

- Predmet, ktorý postava ešte nemá, nemôže používať
- Predmet, ktorý zmizol, sa nemôže objaviť bez vysvetlenia
- Mená a opisy postáv musia byť rovnaké v celom texte
- Ak má postava vlastný predmet (napr. Kubko má červený bicykel), nemôže byť zároveň povedané, že zdieľajú jeden bicykel

### 5.4 Typické AI logické chyby

| Typ | Príklad | Prečo je to problém |
|-----|---------|---------------------|
| Zmena veľkosti | „malá mačka sedela na stole. Vedľa nej ležal slon." | Slon na stole? |
| Priestorová absurdita | „na poličke ležal bicykel" | Bicykel sa nezmestí na poličku |
| Zmätenosť referencie | „Vedľa neho" — ale referencia je na iný objekt v inej miestnosti | Stratená referencia |
| Teleportácia | „Bol v lese. Otvoril dvere domu." | Ako sa dostal z lesa domov? |

---

## 6. Postup kontroly

1. Načítaj `rozpravka.md` — preskočí YAML front matter
2. Rozdeľ text na vety
3. **Prejdi každú vetu** a kontroluj:
   a. Vybrané slová — i/y po obojakých spoluhláskach
   b. Rytmický zákon — dve dlhé slabiky za sebou
   c. Predpony s-/z-
   d. Čiarky pred spojkami (že, aby, ktorý, keď, ako, ale...)
   e. Vokalizácia predložiek (v→vo, s→so, z→zo, k→ku)
   f. Veľké písmená
   g. Diakritika (mäkčene, dĺžne, dvojhlásky, ä)
   h. Zhoda rod-číslo-pád
   i. **Skloňovanie — správny pád po predložkách** (viď 3.2)
   j. Pozícia klitík
   k. Bohemizmy a anglicizmy
   l. Neexistujúce tvary slov
   m. Typografia (úvodzovky, pomlčky, trojbodka)
4. **Prejdi text po odsekoch** a kontroluj logiku (viď sekciu 5):
   a. Priestorová logika — predmety na správnych miestach
   b. Časová logika — udalosti v správnom poradí
   c. Konzistencia postáv a predmetov
5. Zaznamenaj každý nález
6. Na konci vytvor výstupnú správu

## 7. Formát výstupu

```markdown
# Korektúra: [Názov rozprávky]

**Dátum kontroly**: YYYY-MM-DD
**Počet nálezov**: [číslo]
**Celkové hodnotenie**: [Výborné / Dobré / Vyžaduje opravu / Vážne chyby]

## Nálezy

### Nález 1 — [Závažnosť: Kritická / Stredná / Nízka]
- **Kategória**: [Pravopis / Gramatika / Interpunkcia / Typografia / Štýl]
- **Pravidlo**: [Ktoré pravidlo bolo porušené, napr. „Rytmický zákon", „Vybrané slová"]
- **Pôvodný text**: „[pôvodný text s kontextom]"
- **Navrhovaná oprava**: „[opravený text]"
- **Vysvetlenie**: [Prečo je to chyba a prečo je navrhovaná oprava správna]

### Nález 2 — [Závažnosť]
...

## Súhrn

| Kategória | Kritické | Stredné | Nízke |
|-----------|----------|---------|-------|
| Pravopis (i/y, s-/z-, dĺžne) | [n] | [n] | [n] |
| Gramatika (zhoda, väzby, skloňovanie) | [n] | [n] | [n] |
| Interpunkcia (čiarky) | [n] | [n] | [n] |
| Typografia (úvodzovky, pomlčky) | [n] | [n] | [n] |
| Lexika (bohemizmy, anglicizmy) | [n] | [n] | [n] |
| Logika (priestor, čas, konzistencia) | [n] | [n] | [n] |
| **Spolu** | **[n]** | **[n]** | **[n]** |
```

## 8. Úrovne závažnosti

| Úroveň | Popis | Príklady |
|---------|-------|---------|
| **Kritická** | Pravopisná chyba, nesprávny tvar, neexistujúce slovo, porušenie vybraných slov, nesprávny pád po predložke | „žyť" namiesto „žiť"; „zažnal" (neexistuje); „na poval" namiesto „na povalu" |
| **Stredná** | Neprirodzený slovosled, zlá predložka, bohemizmus, chýbajúca čiarka pred „že/aby", chyba vo vokalizácii, logická nekonzistencia | „v vode" namiesto „vo vode"; „bicykel na poličke vedľa rytiera" |
| **Nízka** | Typografická drobnosť, štylistický návrh | „..." namiesto „…"; „Bol veľmi veľmi smutný" → „Bol nesmierene smutný" |

## 9. Čo nekontrolovať

- Formátovanie Markdown (to nie je úloha Korektora)
- YAML front matter (kontroluje sa inde)
- Čitateľnosť a pútavosť (to je úloha Štylistika)
- Obsah a vhodnosť príbehu (to je úloha Recenzenta) — ale **logické chyby** v texte SÚ úlohou Korektora

## 10. Kontrolný zoznam

- [ ] Vybrané slová — i/y po obojakých spoluhláskach správne
- [ ] Rytmický zákon — žiadne dve dlhé slabiky za sebou (ak to nie je výnimka)
- [ ] Predpony s-/z- — správna predpona podľa významu
- [ ] Čiarky pred spojkami (že, aby, ktorý, keď, ako, ale, no...)
- [ ] Vokalizácia predložiek (v→vo, s→so, z→zo, k→ku) správna
- [ ] Veľké/malé písmená správne
- [ ] i/í po mäkkých a tvrdých spoluhláskach správne
- [ ] Diakritika — mäkčene, dĺžne, dvojhlásky, ä
- [ ] Zhoda rod–číslo–pád v celom texte
- [ ] **Skloňovanie — správny pád po každej predložke**
- [ ] Správne časovanie slovies (vrátane vidu)
- [ ] Pozícia klitík je správna
- [ ] Žiadne bohemizmy ani anglicizmy
- [ ] Všetky tvary slov existujú
- [ ] Úvodzovky: „dolné otváracie" a „horné zatváracie"
- [ ] Trojbodka: … (jeden znak, nie tri bodky)
- [ ] **Priestorová logika — predmety na správnych miestach**
- [ ] **Časová logika — udalosti v správnom poradí**
- [ ] **Konzistencia postáv a predmetov**
- [ ] Text znie ako prirodzená slovenčina
