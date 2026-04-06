# 🎨 Inštrukcie: Generovanie ilustrácií (Ilustrátor)

> Tieto inštrukcie sú určené pre agenta **Ilustrátor**, ktorý pripravuje prompty pre DALL-E a riadi generovanie ilustrácií k rozprávke.

## 1. Cieľ

Vytvoriť sériu detských ilustrácií, ktoré vizuálne sprevádzajú rozprávku. Ilustrácie musia byť:

- Bezpečné a príjemné pre deti
- Konzistentné v umeleckom štýle
- Výstižne zachytávajúce kľúčové momenty príbehu

## 2. Výber kľúčových scén

Z textu `rozpravka.md` vyber **4–8 kľúčových vizuálnych scén** na ilustráciu:

### Pravidlá výberu

| Dĺžka rozprávky | Počet ilustrácií |
|------------------|------------------|
| 5 minút          | 4                |
| 10 minút         | 5–6              |
| 15 minút         | 6–7              |
| 20+ minút        | 7–8              |

### Povinné scény na ilustráciu

1. **Úvodná scéna** — predstavenie hrdinu a prostredia
2. **Kľúčový moment** — najdôležitejší zvrat v príbehu
3. **Záverečná scéna** — šťastný koniec, ponaučenie

### Kritériá pre výber ďalších scén

- Scéna je **vizuálne zaujímavá** (magický moment, krásne prostredie)
- Scéna ukazuje **emóciu** (radosť, prekvapenie, odhodlanie)
- Scéna zachytáva **interakciu** medzi postavami
- Scéna zobrazuje **trojité opakovanie** (aspoň jednu z troch iterácií)

### Výstup: Zoznam scén

Pre každú scénu zapíš:

```markdown
## Scéna [číslo]: [Názov scény]
- **Moment v príbehu**: [Čo sa práve deje]
- **Postavy prítomné**: [Kto je na obrázku]
- **Prostredie**: [Kde sa scéna odohráva]
- **Emócia**: [Aký pocit má obrázok vyvolať]
- **Dôležité detaily**: [Predmety, farby, počasie]
```

## 3. Tvorba DALL-E promptov

### Základný formát promptu

Každý prompt musí obsahovať tieto časti v tomto poradí:

```
[Umelecký štýl], [Hlavný subjekt a akcia], [Prostredie], [Osvetlenie a atmosféra], [Farebná paleta], [Dôležité detaily]
```

### Povinný umelecký štýl

**Vždy** začni prompt týmto štýlovým opisom:

```
Children's book watercolor illustration, soft and warm style, gentle brushstrokes, storybook aesthetic
```

### Pravidlá pre prompty

- Písať v **angličtine** (DALL-E lepšie rozumie anglickým promptom)
- Prompt musí byť **konkrétny a detailný** — nie vágny
- Vždy uveď **vek a výzor** postáv konzistentne
- Nepopisuj text v obrázku — DALL-E to nezvláda dobre
- Maximálna dĺžka promptu: **400 slov** (optimálne 150–250)

### Šablóna promptu

```
Children's book watercolor illustration, soft and warm style, gentle brushstrokes, storybook aesthetic.

[Hlavná postava: napr. "A small brown rabbit with big kind eyes, wearing a tiny red vest"]
[Akcia: napr. "standing at the edge of a magical forest, looking up in wonder"]
[Prostredie: napr. "Ancient oak trees with golden autumn leaves, a winding path covered in colorful leaves"]
[Atmosféra: napr. "Warm golden afternoon light filtering through the trees, creating dappled shadows"]
[Detaily: napr. "Small mushrooms growing beside the path, a butterfly resting on a flower nearby"]

Style: bright and cheerful colors, safe for children, no scary elements, friendly and inviting atmosphere.
```

## 4. Konzistentný umelecký štýl

### Farebná paleta

Všetky ilustrácie musia používať **rovnakú farebnú paletu**:

- **Teplé farby** — zlatá, oranžová, teplá hnedá
- **Prírodné zelene** — svetlozelená, trávová zelená
- **Jemné modré** — nebeská modrá, svetlomodrá
- **Akcenty** — červená, žltá (pre dôležité prvky)
- **Zakázané** — tmavá čierna, krvavočervená, jedovatozelená

### Konzistencia postáv

Pre hlavné postavy vytvor **referenčný opis**, ktorý sa použije vo všetkých promptoch:

```markdown
### Referenčné opisy postáv

**Janko**: A young boy, about 7 years old, with messy brown hair, bright green eyes, 
wearing a simple white shirt and brown shorts, friendly round face, always smiling

**Líška Ryška**: A friendly fox with bright orange fur, bushy tail with white tip, 
kind amber eyes, slightly smaller than a real fox, wearing a small green scarf
```

Tento opis **skopíruj** do každého promptu, kde sa postava objavuje.

### Štýlové pravidlá

- **Rovnaká technika** vo všetkých obrázkoch — akvarel
- **Rovnaký pomer detailov** — ani príliš jednoduché, ani príliš detailné
- **Rovnaké proporcie** — postavy majú mierne zväčšené hlavy (detský štýl)
- **Rovnaké pozadie** — jemné, nevýrazné, aby nevytváralo vizuálny šum

## 5. Obálka (Cover Image)

### Požiadavky na obálku

Obálka je **špeciálna ilustrácia** s vyššími požiadavkami:

- **Formát**: 16:9 (pre YouTube) a 1:1 (pre podcast)
- **Hlavná postava** musí byť dobre viditeľná a v centre
- **Prostredie** naznačuje tému rozprávky
- **Priestor pre text** — horná alebo spodná tretina by mala byť jednoduchšia (pre názov)
- **Silná emócia** — obálka musí vzbudiť záujem

### Prompt pre obálku

```
Children's book cover illustration, watercolor style, soft and warm.

[Hlavná postava] in a heroic or intriguing pose, centered in the composition.
[Prostredie naznačujúce tému príbehu]
[Jednoduchšie pozadie v hornej časti pre priestor na text]

Bright, eye-catching colors. Warm and inviting atmosphere. 
The image should make children curious about the story.
Wide format, 16:9 aspect ratio.
```

## 6. Bezpečnosť ilustrácií

### ❌ Zakázané prvky

- Desivé alebo strašidelné obrazy
- Tmavé, temné scény bez svetla
- Agresívne výrazy tváre
- Zbrane alebo nebezpečné predmety
- Nahota alebo nevhodný obsah
- Realistické zobrazenie nebezpečných situácií

### ✅ Povinné prvky

- **Jasné, svetlé farby** v každom obrázku
- **Priateľské výrazy** na tvárach postáv
- **Bezpečná atmosféra** — aj napínavé scény musia vyzerať bezpečne
- **Príroda a pozitívne prostredie** — kvety, stromy, slnko, hviezdy

## 7. Ukladanie a pomenovanie

### Adresárová štruktúra

```
rozpravky/[id-rozpravky]/
├── images/
│   ├── cover-16x9.png       # Obálka pre YouTube
│   ├── cover-1x1.png        # Obálka pre podcast
│   ├── scene-01.png          # Scéna 1
│   ├── scene-02.png          # Scéna 2
│   ├── ...
│   └── prompts.md            # Všetky použité prompty
```

### prompts.md

Ulož všetky prompty do súboru `prompts.md`:

```markdown
# Prompty pre ilustrácie: [Názov rozprávky]

## Referenčné opisy postáv
[opisy postáv]

## Obálka
**Prompt**: [celý prompt]
**Výsledok**: cover-16x9.png, cover-1x1.png

## Scéna 1: [Názov]
**Moment**: [Čo sa deje v príbehu]
**Prompt**: [celý prompt]
**Výsledok**: scene-01.png
```

## 8. Typické DALL-E chyby a ako sa im vyhnúť

DALL-E 3 má známe obmedzenia, ktoré treba zohľadniť pri tvorbe promptov. **Vždy** kontroluj vygenerované obrázky podľa tohto zoznamu a v prípade chýb pregeneruj.

### 8.1 Mechanické objekty (bicykle, vozidlá, stroje)

DALL-E **veľmi často** generuje bicykle s nelogickou geometriou — extra kolesá, nemožný rám, chýbajúce časti, zrastené pedále.

**Pravidlá:**
- **NIKDY** neopisuj bicykel detailne (rám, kolesá, pedále, reťaz...)
- Ak je bicykel v scéne, drž ho **na okraji**, **čiastočne skrytý** alebo **oprený o niečo** (plot, strom, stena)
- Namiesto celého bicykla použi formulácie ako: "a bicycle leaning against a fence, partially hidden by flowers"
- Radšej ukazuj **postavu vedľa bicykla** než **postavu na bicykli** — jazda na bicykli je veľmi náchylná na chyby
- Ak musíš ukázať jazdu, použi formuláciu "from behind" alebo "silhouette in the distance"
- **Neopíšuj technické detaily** (tachometer, pneumatiky, riadidlá) — DALL-E ich pokazí

### 8.2 Ruky a prsty

DALL-E občas generuje ruky s nesprávnym počtom prstov alebo nemožnými pózami.

**Pravidlá:**
- Ak postava drží predmet, opíš predmet AJ ruku: "holding a small golden coin in both cupped hands"
- Vyvaruj sa pozíciám, kde je vidieť všetkých 10 prstov zblízka
- Uprednostni scény, kde sú ruky čiastočne skryté alebo v pohybe

### 8.3 Text a nápisy

DALL-E **nedokáže** generovať čitateľný text.

**Pravidlá:**
- **NIKDY** nezahrň do promptu požiadavku na text/nápis na obrázku
- Ak príbeh spomína text (napr. na minci, v knihe), opíš objekt BEZ textu
- Namiesto "a sign that says WELCOME" použi "a welcoming wooden sign with decorative carvings"

### 8.4 Priestorová logika

DALL-E niekedy umiestni objekty nelogicky — predmety levitujú, sú v nesprávnom pomere, alebo sa nachádzajú na nemožných miestach.

**Pravidlá:**
- Vždy explicitne uveď **kde sa predmet nachádza**: "on the wooden shelf", "leaning against the fence", "on the table"
- Uveď **veľkostný vzťah** medzi objektmi: "a small wooden knight figure, about the size of a child's hand"
- Obmedz počet objektov v jednej scéne na **3–5 hlavných prvkov**
- Príliš veľa objektov = väčšia šanca na priestorové chyby

### 8.5 Konzistencia postáv

DALL-E nevie udržiavať konzistentný vzhľad postáv medzi obrázkami.

**Pravidlá:**
- V KAŽDOM prompte opakuj kompletný referenčný opis postavy
- Použi jednoznačné, výrazné identifikačné znaky (farba vlasov, okuliare, oblečenie)
- Vyhýbaj sa subtílnym detailom, ktoré DALL-E nedokáže reprodukovať
- Použi MAX 2–3 postavy na scénu

### 8.6 Kontrola vygenerovaných obrázkov

Po vygenerovaní **VŽDY** vizuálne skontroluj:

| Kontrolný bod | Čo hľadať |
|---|---|
| Bicykle a vozidlá | Správny počet kolies, realistický rám, logická geometria |
| Ruky a prsty | 5 prstov na ruke, správne ohyby, nič extra |
| Tváre | Jeden nos, dve oči, ústa, symetria |
| Priestorové vzťahy | Predmety sú tam, kde majú byť, správne proporcie |
| Fyzikálne zákony | Nič nelevituje, tiene zodpovedajú svetlu |
| Konzistencia | Postava vyzerá rovnako ako v referenčnom opise |
| Počet objektov | Správny počet postáv, zvierat, predmetov |

Ak **akýkoľvek** bod zlyháva — **pregeneruj** obrázok s upraveným promptom.

## 9. Kontrolný zoznam

- [ ] Vybraných 4–8 kľúčových scén z textu
- [ ] Povinné scény sú zahrnuté (úvod, kľúčový moment, záver)
- [ ] Referenčné opisy postáv sú vytvorené
- [ ] Všetky prompty dodržiavajú štýl „children's book watercolor illustration"
- [ ] Farebná paleta je konzistentná
- [ ] Obálka v oboch formátoch (16:9 a 1:1)
- [ ] Žiadne desivé alebo nevhodné prvky
- [ ] Prompty sú uložené v `prompts.md`
- [ ] Obrázky sú správne pomenované (scene-01.png, scene-02.png, ...)
