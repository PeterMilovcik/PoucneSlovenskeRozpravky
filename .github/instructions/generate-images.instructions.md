# 🎨 Inštrukcie: Generovanie ilustrácií (Ilustrátor)

> Tieto inštrukcie sú určené pre agenta **Ilustrátor**, ktorý pripravuje prompty a riadi generovanie ilustrácií k rozprávke pomocou **GPT Image (gpt-image-1)** cez OpenAI Image API.

> ⚠️ **DALL-E 3 je DEPRECATED** (koniec podpory 05/12/2026). Používame výhradne model **gpt-image-1**.
> ⚠️ **Responses API je BLOKOVANÉ** bez verifikácie organizácie. Nepoužívaj ho — iba Image API.

## 1. Cieľ

Vytvoriť sériu detských ilustrácií, ktoré vizuálne sprevádzajú rozprávku. Ilustrácie musia byť:

- Bezpečné a príjemné pre deti
- Konzistentné v umeleckom štýle (zabezpečené **referenčným obrázkom**)
- Výstižne zachytávajúce **každú vizuálnu scénu** príbehu
- Pripravené na použitie vo **videu** (1 ilustrácia = 1 segment audia)

## 2. Výber scén na ilustráciu

Z textu `rozpravka.md` identifikuj **každú vizuálne odlišnú scénu**. Pre video produkciu potrebujeme výrazne viac ilustrácií než pôvodných 4–8 — každá scéna sa mapuje na segment audia.

### Pravidlá výberu

| Dĺžka rozprávky | Počet ilustrácií | Poznámka |
|------------------|------------------|----------|
| 5 minút          | 8–10             | ~1 ilustrácia na 30–40 s audia |
| 10 minút         | 12–15            | ~1 ilustrácia na 40–50 s audia |
| 15 minút         | 15–20            | ~1 ilustrácia na 45–60 s audia |
| 20+ minút        | 20–25            | ~1 ilustrácia na 50–60 s audia |

**Pravidlo**: Spočítaj vizuálne odlišné scény v príbehu — pre každú vytvor jednu ilustráciu. Ak sa zmení prostredie, pribudne/odíde postava, alebo sa dej výrazne posunie, je to nová scéna.

### Povinné scény na ilustráciu

1. **Úvodná scéna** — predstavenie hrdinu a prostredia
2. **Každý krok trojitého opakovania** — všetky tri iterácie (nie len jednu!)
3. **Kľúčový moment** — najdôležitejší zvrat v príbehu
4. **Záverečná scéna** — šťastný koniec, ponaučenie

### Kritériá pre výber ďalších scén

- Scéna je **vizuálne odlišná** od predchádzajúcej (iné prostredie, iná akcia)
- Scéna ukazuje **emóciu** (radosť, prekvapenie, odhodlanie)
- Scéna zachytáva **interakciu** medzi postavami
- Scéna zodpovedá **segmentu audia** vo videu — ak je segment dlhší než ~60 s, rozdeľ ho na dva obrázky

### Výstup: Zoznam scén

Pre každú scénu zapíš:

```markdown
## Scéna [číslo]: [Názov scény]
- **Moment v príbehu**: [Čo sa práve deje]
- **Postavy prítomné**: [Kto je na obrázku]
- **Prostredie**: [Kde sa scéna odohráva]
- **Emócia**: [Aký pocit má obrázok vyvolať]
- **Dôležité detaily**: [Predmety, farby, počasie]
- **Audio segment**: [Približný začiatok a koniec v texte]
```

## 3. Generovací pipeline — referenčný obrázok

Toto je **najdôležitejšia časť** celého procesu. Konzistencia postáv a štýlu sa dosahuje pomocou **referenčného obrázku** (obálky), nie iba textovými promptmi.

### 3.1 Poradie generovania

```
1. Vygeneruj OBÁLKU (cover) → images.generate()
2. Vygeneruj VŠETKY SCÉNY → images.edit() s obálkou ako referenciou
```

### 3.2 API volania

**Obálka** — bez referenčného obrázku:

```python
response = client.images.generate(
    model="gpt-image-1",
    prompt="[štýlový prefix] + [popis obálky]",
    size="1536x1024",       # landscape 16:9
    quality="high",         # produkčná kvalita
)
# Výstup je base64 (b64_json), NIE URL
image_data = base64.b64decode(response.data[0].b64_json)
```

**Scény** — s obálkou ako referenciou:

```python
with open("images/cover-16x9.png", "rb") as cover_file:
    response = client.images.edit(
        model="gpt-image-1",
        image=cover_file,       # referenčný obrázok = obálka
        prompt="[štýlový prefix] + [popis scény]",
        size="1536x1024",
        quality="high",
    )
image_data = base64.b64decode(response.data[0].b64_json)
```

### 3.3 Prečo referenčný obrázok?

- `images.edit()` s obálkou ako vstupom zachováva **farebnú paletu, štýl štetca a proporcie postáv**
- Výrazne lepšia konzistencia než samotné textové opisy
- Obálka obsahuje hlavnú postavu — model ju „rozpozná" a reprodukuje v ďalších scénach

### 3.4 Technické detaily API

| Parameter | `images.generate()` | `images.edit()` |
|-----------|---------------------|-----------------|
| **Použitie** | Obálka (prvý obrázok) | Všetky scény |
| **Referencia** | Žiadna | `image=cover_file` |
| **Veľkosť** | `1536x1024` | `1536x1024` |
| **Kvalita** | `high` | `high` |
| **Výstup** | `b64_json` | `b64_json` |
| **Model** | `gpt-image-1` | `gpt-image-1` |

### 3.5 Ceny za obrázok (landscape 1536×1024)

| Kvalita | Cena za obrázok | Kedy použiť |
|---------|-----------------|--------------|
| `low`   | $0,016          | Testovanie promptov, rýchle prototypy |
| `medium`| $0,063          | Náhľady, interná kontrola |
| `high`  | $0,25           | **Finálna produkcia** — vždy pre publikáciu |

### 3.6 Nastavenie klienta

```python
import openai
import httpx

# OPENAI_API_KEY musí byť nastavený ako premenná prostredia
client = openai.OpenAI(
    http_client=httpx.Client(verify=False)  # SSL workaround pre firemné proxy
)
```

> ⚠️ `verify=False` je nutný pre firemné proxy, ktoré vykonávajú SSL interception. V produkčnom prostredí zvážte nastavenie vlastného CA certifikátu.

## 4. Povinný štýlový prefix

**KAŽDÝ** prompt (obálka aj scény) MUSÍ začínať rovnakým štýlovým prefixom. Toto zabezpečuje vizuálnu konzistenciu naprieč celou sériou obrázkov.

### Overený štýlový prefix

```
Soft watercolor children's book illustration in warm storybook style. Gentle rounded
brushstrokes, dreamy color blending, soft edges. Warm golden and earthy color palette
with touches of green and blue. Characters have friendly rounded proportions with
expressive eyes. European small-town setting. Safe, magical, inviting atmosphere for
children. No text, no letters, no words in the image. Wide format, 16:9 aspect ratio.
```

### Pravidlá pre prompty

- Písať v **angličtine** (model lepšie rozumie anglickým promptom)
- **Vždy** začni štýlovým prefixom, potom pridaj popis konkrétnej scény
- Prompt musí byť **konkrétny a detailný** — nie vágny
- Vždy uveď **vek a výzor** postáv konzistentne
- **NIKDY** nepožaduj text, nápisy ani písmená v obrázku
- Maximálna dĺžka promptu: **400 slov** (optimálne 150–250)

### Šablóna promptu pre scénu

```
Soft watercolor children's book illustration in warm storybook style. Gentle rounded
brushstrokes, dreamy color blending, soft edges. Warm golden and earthy color palette
with touches of green and blue. Characters have friendly rounded proportions with
expressive eyes. European small-town setting. Safe, magical, inviting atmosphere for
children. No text, no letters, no words in the image. Wide format, 16:9 aspect ratio.

[Hlavná postava: napr. "A small brown rabbit with big kind eyes, wearing a tiny red vest"]
[Akcia: napr. "standing at the edge of a magical forest, looking up in wonder"]
[Prostredie: napr. "Ancient oak trees with golden autumn leaves, a winding path covered in colorful leaves"]
[Atmosféra: napr. "Warm golden afternoon light filtering through the trees, creating dappled shadows"]
[Detaily: napr. "Small mushrooms growing beside the path, a butterfly resting on a flower nearby"]
```

## 5. Konzistentný umelecký štýl

### Farebná paleta

Všetky ilustrácie musia používať **rovnakú farebnú paletu** (zabezpečenú štýlovým prefixom + referenčným obrázkom):

- **Teplé farby** — zlatá, oranžová, teplá hnedá
- **Prírodné zelene** — svetlozelená, trávová zelená
- **Jemné modré** — nebeská modrá, svetlomodrá
- **Akcenty** — červená, žltá (pre dôležité prvky)
- **Zakázané** — tmavá čierna, krvavočervená, jedovatozelená

### Konzistencia postáv (referenčný obrázok + textový opis)

Konzistencia sa dosahuje **dvoma mechanizmami**:

1. **Referenčný obrázok** (obálka) — cez `images.edit()` — zabezpečuje vizuálnu konzistenciu
2. **Textový opis** v prompte — dopĺňa detaily, ktoré referenčný obrázok nemusí zachytiť

Pre hlavné postavy vytvor **referenčný opis**, ktorý sa použije vo všetkých promptoch:

```markdown
### Referenčné opisy postáv

**Janko**: A young boy, about 7 years old, with messy brown hair, bright green eyes,
wearing a simple white shirt and brown shorts, friendly round face, always smiling

**Líška Ryška**: A friendly fox with bright orange fur, bushy tail with white tip,
kind amber eyes, slightly smaller than a real fox, wearing a small green scarf
```

Tento opis **skopíruj** do každého promptu, kde sa postava objavuje — aj keď používaš referenčný obrázok.

### Štýlové pravidlá

- **Rovnaká technika** vo všetkých obrázkoch — akvarel (zabezpečená prefixom)
- **Rovnaký pomer detailov** — ani príliš jednoduché, ani príliš detailné
- **Rovnaké proporcie** — postavy majú mierne zväčšené hlavy (detský štýl)
- **Rovnaké pozadie** — jemné, nevýrazné, aby nevytváralo vizuálny šum

## 6. Obálka (Cover Image)

### Dôležitosť obálky

Obálka je **kľúčový obrázok** celého procesu — slúži nielen ako marketingový materiál, ale aj ako **referenčný obrázok** pre generovanie všetkých scén. Preto musí byť vygenerovaná **ako prvá** a musí obsahovať hlavnú postavu v plnej kráse.

### Požiadavky na obálku

- **Formát**: 1536×1024 px (landscape 16:9)
- **Hlavná postava** musí byť dobre viditeľná a v centre — bude slúžiť ako referencia
- **Prostredie** naznačuje tému rozprávky
- **Priestor pre text** — horná alebo spodná tretina by mala byť jednoduchšia (pre názov)
- **Silná emócia** — obálka musí vzbudiť záujem

### Generovanie obálky

```python
response = client.images.generate(
    model="gpt-image-1",
    prompt="""Soft watercolor children's book illustration in warm storybook style.
Gentle rounded brushstrokes, dreamy color blending, soft edges. Warm golden and
earthy color palette with touches of green and blue. Characters have friendly
rounded proportions with expressive eyes. European small-town setting. Safe,
magical, inviting atmosphere for children. No text, no letters, no words in the
image. Wide format, 16:9 aspect ratio.

[Hlavná postava] in a heroic or intriguing pose, centered in the composition.
[Prostredie naznačujúce tému príbehu]
[Jednoduchšie pozadie v hornej časti pre priestor na text]

Bright, eye-catching colors. Warm and inviting atmosphere.
The image should make children curious about the story.""",
    size="1536x1024",
    quality="high",
)
```

## 7. Bezpečnosť ilustrácií

### ❌ Zakázané prvky

- Desivé alebo strašidelné obrazy
- Tmavé, temné scény bez svetla
- Agresívne výrazy tváre
- Zbrane alebo nebezpečné predmety
- Nahota alebo nevhodný obsah
- Realistické zobrazenie nebezpečných situácií
- Text, nápisy alebo písmená v obrázku

### ✅ Povinné prvky

- **Jasné, svetlé farby** v každom obrázku
- **Priateľské výrazy** na tvárach postáv
- **Bezpečná atmosféra** — aj napínavé scény musia vyzerať bezpečne
- **Príroda a pozitívne prostredie** — kvety, stromy, slnko, hviezdy

## 8. Povinná kontrola obrázkov (Image Review)

Po vygenerovaní **VŠETKÝCH** obrázkov vykonaj **štvorkrokovú kontrolu** ešte PRED pokračovaním k video produkcii.

### Krok 1: Náhľadové miniatúry

Vygeneruj náhľadové JPG (800px šírka) pre rýchlu vizuálnu kontrolu:

```bash
ffmpeg -i images/scene-XX.png -vf "scale=800:-1" -q:v 4 -y images/preview/scene-XX.jpg
```

### Krok 2: Kontrola logických chýb

| Kontrolný bod | Čo hľadať |
|---|---|
| Počet objektov | Správny počet postáv, zvierat, predmetov podľa promptu |
| Priestorová logika | Nič nelevituje, predmety sú tam kde majú byť |
| Geometria | Bicykle/vozidlá majú správnu štruktúru, žiadne extra kolesá |
| Ruky a prsty | 5 prstov na ruke, správne ohyby |
| Tváre | Jeden nos, dve oči, ústa, symetria |
| Fyzikálne zákony | Tiene zodpovedajú svetlu, proporcie sedia |

### Krok 3: Kontrola konzistencie postáv

Porovnaj hlavnú postavu naprieč VŠETKÝMI scénami:

- **Farba vlasov/srsti** — rovnaká v každom obrázku
- **Oblečenie** — rovnaké v každom obrázku (farba, typ)
- **Okuliare/doplnky** — ak má postava okuliare, musí ich mať všade
- **Proporcie** — postava by mala byť rovnako veľká voči prostrediu

### Krok 4: Kontrola konzistencie štýlu

- **Akvarelovú techniku** — rovnaký typ štetcových ťahov v každom obrázku
- **Farebnú paletu** — rovnaké teplé tóny, žiadny obrázok nie je „studený" alebo „tmavý"
- **Úroveň detailov** — žiadny obrázok nie je výrazne detailnejší/jednoduchší než ostatné

### Rozhodnutie

Ak **akýkoľvek** obrázok zlyháva v ktoromkoľvek kroku — **pregeneruj** ho cez `images.edit()` s upraveným promptom. Pokračuj k videu až keď **VŠETKY** obrázky prejdú kontrolou.

## 9. Typické chyby generovania a ako sa im vyhnúť

Model GPT Image (gpt-image-1) je výrazne lepší než DALL-E 3, ale niektoré problémy pretrvávajú. **Vždy** kontroluj vygenerované obrázky podľa tohto zoznamu.

### 9.1 Mechanické objekty (bicykle, vozidlá, stroje)

GPT Image je lepší než DALL-E 3, ale bicykle stále bývajú problematické.

**Pravidlá:**
- Ak je bicykel v scéne, drž ho **na okraji**, **čiastočne skrytý** alebo **oprený o niečo** (plot, strom, stena)
- Namiesto celého bicykla použi formulácie ako: "a bicycle leaning against a fence, partially hidden by flowers"
- Radšej ukazuj **postavu vedľa bicykla** než **postavu na bicykli**
- **Neopíšuj technické detaily** (tachometer, pneumatiky, riadidlá)

### 9.2 Ruky a prsty

GPT Image je v tejto oblasti výrazne lepší, ale stále kontroluj.

**Pravidlá:**
- Ak postava drží predmet, opíš predmet AJ ruku: "holding a small golden coin in both cupped hands"
- Vyvaruj sa pozíciám, kde je vidieť všetkých 10 prstov zblízka
- Uprednostni scény, kde sú ruky čiastočne skryté alebo v pohybe

### 9.3 Text a nápisy

GPT Image **nedokáže spoľahlivo** generovať čitateľný text — a v detských ilustráciách ho ani nechceme.

**Pravidlá:**
- **NIKDY** nezahrň do promptu požiadavku na text/nápis v obrázku
- Štýlový prefix obsahuje explicitný zákaz: "No text, no letters, no words in the image"
- Ak príbeh spomína text (napr. na ceduli, v knihe), opíš objekt BEZ textu
- Namiesto "a sign that says WELCOME" použi "a welcoming wooden sign with decorative carvings"

### 9.4 Priestorová logika

**Pravidlá:**
- Vždy explicitne uveď **kde sa predmet nachádza**: "on the wooden shelf", "leaning against the fence"
- Uveď **veľkostný vzťah** medzi objektmi: "a small wooden knight figure, about the size of a child's hand"
- Obmedz počet objektov v jednej scéne na **3–5 hlavných prvkov**
- Príliš veľa objektov = väčšia šanca na priestorové chyby

### 9.5 Konzistencia postáv

Vďaka referenčnému obrázku (obálke) je konzistencia výrazne lepšia, ale stále:

**Pravidlá:**
- V KAŽDOM prompte opakuj kompletný referenčný opis postavy — aj s referenčným obrázkom
- Použi jednoznačné, výrazné identifikačné znaky (farba vlasov, okuliare, oblečenie)
- Použi MAX 2–3 postavy na scénu
- Po vygenerovaní porovnaj postavu s obálkou — ak sa líši, pregeneruj

## 10. Optimalizácia obrázkov pre blog

Produkčné PNG obrázky sú príliš veľké pre web. Vytvor optimalizované JPG verzie:

```bash
# Konverzia jednej scény
ffmpeg -i images/scene-XX.png -vf "scale=1200:-1" -q:v 4 -y images/scene-XX.jpg

# Konverzia obálky
ffmpeg -i images/cover-16x9.png -vf "scale=1200:-1" -q:v 4 -y images/cover-16x9.jpg
```

| Parameter | Hodnota | Poznámka |
|-----------|---------|----------|
| **Šírka** | 1200 px | Dostatočná pre blog, rýchle načítanie |
| **Kvalita** | `-q:v 4` | Dobrý pomer kvalita/veľkosť |
| **Formát** | JPG | Menšia veľkosť než PNG |

## 11. Generovací skript

Na automatizáciu celého pipeline použi skript `scripts/generate-images.py`.

### CLI parametre

| Parameter | Popis |
|-----------|-------|
| `--model` | Model na generovanie (predvolený: `gpt-image-1`) |
| `--quality` | Kvalita obrázkov: `low`, `medium`, `high` (predvolená: `high`) |
| `--scene N` | Vygeneruj iba scénu číslo N |
| `--all` | Vygeneruj všetky scény (obálka + všetky scény) |
| `--cover-only` | Vygeneruj iba obálku |
| `--no-reference` | Generuj scény bez referenčného obrázku (iba textový prompt) |
| `--dry-run` | Zobraz prompty bez generovania (na kontrolu) |

### Typický workflow

```bash
# 1. Najprv vygeneruj obálku
python scripts/generate-images.py --cover-only --quality high

# 2. Skontroluj obálku vizuálne — ak je OK, pokračuj

# 3. Vygeneruj všetky scény s obálkou ako referenciou
python scripts/generate-images.py --all --quality high

# 4. Ak treba pregenerovať jednu scénu
python scripts/generate-images.py --scene 5 --quality high
```

## 12. Ukladanie a pomenovanie

### Adresárová štruktúra

```
rozpravky/[id-rozpravky]/
├── images/
│   ├── cover-16x9.png        # Obálka (landscape, generovaná PRVÁ)
│   ├── scene-01.png           # Scéna 1
│   ├── scene-02.png           # Scéna 2
│   ├── ...
│   ├── scene-14.png           # (12–15 scén pre 10-min rozprávku)
│   ├── preview/               # Náhľadové miniatúry (800px JPG)
│   │   ├── cover-16x9.jpg
│   │   ├── scene-01.jpg
│   │   ├── scene-02.jpg
│   │   └── ...
│   └── prompts.md             # Všetky prompty so štýlovým prefixom + referenciami
```

### prompts.md

Ulož všetky prompty do súboru `prompts.md`:

```markdown
# Prompty pre ilustrácie: [Názov rozprávky]

## Štýlový prefix (použitý vo VŠETKÝCH promptoch)

Soft watercolor children's book illustration in warm storybook style. Gentle rounded
brushstrokes, dreamy color blending, soft edges. Warm golden and earthy color palette
with touches of green and blue. Characters have friendly rounded proportions with
expressive eyes. European small-town setting. Safe, magical, inviting atmosphere for
children. No text, no letters, no words in the image. Wide format, 16:9 aspect ratio.

## Referenčné opisy postáv
[opisy postáv]

## Obálka
**API**: `images.generate(model="gpt-image-1", size="1536x1024", quality="high")`
**Prompt**: [štýlový prefix] + [popis obálky]
**Výsledok**: cover-16x9.png
**Kontrola**: ✅/❌ [poznámky]

## Scéna 1: [Názov]
**API**: `images.edit(model="gpt-image-1", image=cover, size="1536x1024", quality="high")`
**Moment**: [Čo sa deje v príbehu]
**Audio segment**: [Časový rozsah v texte]
**Prompt**: [štýlový prefix] + [popis scény]
**Výsledok**: scene-01.png
**Kontrola**: ✅/❌ [poznámky]

## Scéna 2: [Názov]
...
```

## 13. Kontrolný zoznam

- [ ] Identifikované VŠETKY vizuálne odlišné scény (12–15 pre 10-min rozprávku)
- [ ] Povinné scény sú zahrnuté (úvod, trojité opakovanie, kľúčový moment, záver)
- [ ] Referenčné opisy postáv sú vytvorené
- [ ] Obálka je vygenerovaná PRVÁ cez `images.generate()`
- [ ] Všetky scény sú vygenerované cez `images.edit()` s obálkou ako referenciou
- [ ] Štýlový prefix je identický vo VŠETKÝCH promptoch
- [ ] **Review krok 1**: Náhľadové miniatúry vytvorené (800px JPG)
- [ ] **Review krok 2**: Logické chyby skontrolované
- [ ] **Review krok 3**: Konzistencia postáv overená naprieč scénami
- [ ] **Review krok 4**: Konzistencia štýlu overená naprieč scénami
- [ ] Chybné obrázky boli pregenerované
- [ ] Žiadne desivé alebo nevhodné prvky
- [ ] Žiadny text, nápisy ani písmená v obrázkoch
- [ ] Prompty sú uložené v `prompts.md`
- [ ] Obrázky sú správne pomenované (scene-01.png, scene-02.png, ...)
- [ ] Blog verzie vytvorené (1200px JPG, kvalita 4)
