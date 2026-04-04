# 📖 Inštrukcie: Generovanie textu rozprávky

> Tieto inštrukcie sú určené pre agenta **Rozprávkár**, ktorý píše samotný text rozprávky podľa schválenej osnovy.

## 1. Štylistický sprievodca — prísne dodržiavať

### Rozprávač

- **Teplý, láskavý hlas** — ako keby rozprávku rozprával milovaný starý rodič
- **Tretia osoba** — „Janko sa pozrel na horu a usmial sa."
- Rozprávač občas osloví poslucháča: „A viete, čo sa stalo potom?"
- Rozprávač **nemoralizuje** v priebehu príbehu — morál príde až na konci

### Vety

- Priemerná dĺžka vety: **8–12 slov**
- Maximálna dĺžka vety: **25 slov** (výnimočne pri opise prostredia)
- Striedaj krátke a dlhšie vety pre prirodzený rytmus
- Vyhýbaj sa zloženým súvetiam s viacerými vedľajšími vetami

### Slovná zásoba

- Jednoduchá, zrozumiteľná slovenčina pre deti od 6 rokov
- Keď použiješ menej bežné slovo, vysvetli ho v kontexte
- Používaj **konkrétne** slová namiesto abstraktných (nie „bol smutný", ale „slzička mu stiekla po líčku")
- Slovenské ľudové výrazy a frazeológia sú vítané

## 2. Klasické rozprávkové otváranie

Každá rozprávka **musí** začínať tradičným rozprávkovým úvodom. Vyber jeden z týchto alebo vytvor podobný:

- „Kde bolo, tam bolo..."
- „Za siedmimi horami, za siedmimi dolinami..."
- „Dávno, predávno, keď ešte..."
- „V jednej malej dedinke pod vysokými horami..."
- „Bolo raz jedno kráľovstvo, kde..."

## 3. Trojité opakovanie

**Povinný motív** v každej rozprávke. Trojité opakovanie musí byť prítomné aspoň raz:

- Tri úlohy, ktoré musí hrdina splniť
- Tri pokusy o niečo (dva neúspešné, tretí úspešný)
- Tri stretnutia s rôznymi postavami
- Tri dary alebo rady od mentora

### Pravidlá pre opakovanie

- Každé opakovanie je **mierne odlišné** — stupňuje sa napätie
- Tretie opakovanie prináša **zvrat alebo úspech**
- Použij podobnú, ale nie identickú formuláciu — variácie udržia pozornosť

## 4. Opisy — zmyslové a živé

Každá scéna musí obsahovať aspoň **dva zmyslové detaily**:

| Zmysel | Príklady |
|--------|----------|
| Zrak | „Lúka kvitla stovkami farieb — žltých, modrých a červených." |
| Sluch | „Potok zurčal ako zvonivý smiech." |
| Čuch | „Vzduch voňal po čerstvom chlebe a lete." |
| Hmat | „Kôra stromu bola drsná pod jeho prstami." |
| Chuť | „Jahoda bola sladká ako medové kvapky." |

### Prirovnania a metafory

- Používaj prirovnania zrozumiteľné deťom: „rýchly ako zajac", „silný ako medveď"
- Prírodné metafory: „slnko sa usmievalo", „vietor šepkal"
- Vyhýbaj sa klišé — hľadaj originálne obrazy

## 5. Dialógy

### Formát

Dialógy píš na samostatné riadky s úvodzovkami a pomenovacou vetou:

```
„Kam ideš, malý zajačik?" spýtala sa líška sladkým hlasom.
„Idem hľadať stratený kľúč," odpovedal Zajko odvážne.
```

### Pravidlá

- Každá postava má **rozpoznateľný spôsob reči**
- Detské postavy hovoria jednoducho a priamo
- Zvieracie postavy môžu mať charakteristické slovné spojenia
- Mentor hovorí v krátkych, múdrych vetách
- **Žiadny sarkazmus** — deti ho nechápú
- Dialógy posúvajú dej alebo odhaľujú charakter — žiadne „prázdne" rozhovory

## 6. Generovanie podľa dĺžky

### Krátke rozprávky (≤15 minút, do ~2 250 slov)

Generuj **jedným ťahom** (single-pass):

1. Načítaj `outline.md` z adresára rozprávky
2. Napíš celý text od začiatku do konca
3. Dodržuj počet slov podľa osnovy (±10 %)
4. Ulož ako `rozpravka.md`

### Dlhé rozprávky (>15 minút, nad ~2 250 slov)

Generuj **po kapitolách** (chapter-by-chapter):

1. Načítaj `outline.md` z adresára rozprávky
2. Rozdeľ scény do logických kapitol (3–5 scén na kapitolu)
3. Každú kapitolu generuj samostatne
4. **Medzi kapitolami kontroluj konzistenciu**:
   - Mená postáv sa nezmenili
   - Fyzické opisy sú rovnaké
   - Predmety a miesta sú konzistentné
   - Čas príbehu plynie logicky
   - Vzťahy medzi postavami zodpovedajú predchádzajúcemu deju
5. Na konci spoj kapitoly do jedného súboru `rozpravka.md`

### Kontrola konzistencie medzi kapitolami

Pred písaním ďalšej kapitoly si pripomeň:

- Ako vyzerá hlavná postava? (farba vlasov, oblečenie, atď.)
- Aké predmety postava nesie?
- Aká je denná doba a počasie?
- Aké vzťahy boli nadviazané?
- Čo presne postava povedala/sľúbila?

## 7. Formát súboru rozpravka.md

Každá rozprávka **musí** začínať YAML front matter:

```yaml
---
title: "Názov rozprávky"
slug: "nazov-rozpravky"
author: "AI Rozprávkár"
created: "YYYY-MM-DD"
length_minutes: 15
word_count: 2250
theme: "priateľstvo"
moral: "Pravé priateľstvo sa pozná v ťažkých chvíľach."
age_group: "6+"
status: "text_draft"
characters:
  - name: "Janko"
    role: "Hrdina"
  - name: "Líška Ryška"
    role: "Pomocník"
tags:
  - priateľstvo
  - les
  - zvieratá
---
```

### Formát samotného textu

```markdown
# Názov rozprávky

Kde bolo, tam bolo...

[Text rozprávky]

---

## Ponaučenie

[Explicitný morál na konci, 2–3 vety, adresovaný priamo deťom]
```

## 8. Morál na konci

Každá rozprávka **musí** končiť explicitným ponaučením:

- Oddeľ ho od textu horizontálnou čiarou (`---`)
- Nadpis: `## Ponaučenie`
- Formuluj ho priamo a jednoducho: „A tak sa Janko naučil, že..."
- Alebo otázkou pre deti: „Čo myslíte, prečo bolo dôležité, aby Janko..."
- Morál musí zodpovedať tomu, čo je uvedené v `outline.md`
- Maximálne **2–3 vety**

## 9. Kontrolný zoznam pred odovzdaním

- [ ] YAML front matter je kompletný a správny
- [ ] Rozprávka začína klasickým rozprávkovým úvodom
- [ ] Trojité opakovanie je prítomné
- [ ] Priemerná dĺžka vety je 8–12 slov
- [ ] Žiadna veta nepresahuje 25 slov
- [ ] Každá scéna má aspoň dva zmyslové detaily
- [ ] Dialógy sú na samostatných riadkoch s úvodzovkami
- [ ] Morál je explicitne uvedený na konci
- [ ] Počet slov zodpovedá požadovanej dĺžke (±10 %)
- [ ] Text je uložený ako `rozpravka.md` v adresári rozprávky
- [ ] Pre dlhé rozprávky: konzistencia medzi kapitolami je overená
