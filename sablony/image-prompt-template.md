# Šablóna pre generovanie obrázkov (DALL-E)

## Premenné

| Premenná          | Popis                                | Príklad                                  |
| ----------------- | ------------------------------------ | ---------------------------------------- |
| `{{popis_sceny}}` | Detailný popis scény na ilustráciu   | Zajačik sedí na lúke a pozoruje dúhu     |
| `{{styl}}`        | Umelecký štýl ilustrácie             | akvarel, pastelové farby                 |
| `{{postavy}}`     | Postavy zobrazené na obrázku         | malý biely zajačik s modrým šálom        |
| `{{nalada}}`      | Celková nálada a atmosféra obrázku   | veselá, teplá, priateľská                |

---

## Konzistentný umelecký štýl

Každý obrázok musí dodržiavať tento základný štýl:

```
children's book watercolor illustration, bright colors, friendly characters, no text
```

---

## Prompt

```
Children's book watercolor illustration.

Scene: {{popis_sceny}}
Characters: {{postavy}}
Style: {{styl}}, children's book watercolor illustration
Mood: {{nalada}}

Art direction:
- Bright, warm, saturated colors
- Soft watercolor textures with visible brushstrokes
- Friendly, rounded character designs with big expressive eyes
- Simple, uncluttered composition
- Warm lighting, gentle shadows
- Nature elements: trees, flowers, meadows, mountains
- No text, letters, words, or numbers in the image
- No dark, scary, or threatening elements
- Safe and welcoming atmosphere for young children
- Slovak countryside or fairy tale forest setting
```

---

## Bezpečnostné pravidlá

### ✅ Povolené

- Priateľské zvieratá a rozprávkové postavy
- Prírodné prostredie (les, lúka, hory, rieka)
- Čarovné predmety (čarovný prútik, žiariace kamene)
- Veselé a pokojné scény
- Jednoduché čarovné efekty (iskry, dúha, žiara)

### 🚫 Zakázané

- Strašidelné alebo desivé postavy
- Temné alebo ponuré prostredie
- Zbrane alebo nástroje násilia
- Realistické zobrazenie ľudí (radšej štylizované postavy)
- Text alebo nápisy v obrázku
- Komerčné značky alebo logá

---

## Príklady promptov

### Úvodná scéna

```
Children's book watercolor illustration.

Scene: A small white rabbit sitting on a green meadow at the edge of an enchanted forest, looking at a rainbow after rain.
Characters: A cute white rabbit wearing a tiny blue scarf, with big curious eyes.
Style: Soft watercolor, pastel and bright colors, children's book illustration.
Mood: Peaceful, magical, full of wonder.

Art direction:
- Bright, warm, saturated colors
- Soft watercolor textures with visible brushstrokes
- Friendly, rounded character designs with big expressive eyes
- Simple, uncluttered composition
- Warm lighting, gentle shadows
- Slovak mountain landscape in the background
- No text, letters, words, or numbers in the image
```

### Scéna priateľstva

```
Children's book watercolor illustration.

Scene: Three forest animals sitting together around a small campfire in a forest clearing, sharing berries.
Characters: A white rabbit, a wise brown owl, and a small friendly bear cub, all smiling.
Style: Warm watercolor, golden hour lighting, children's book illustration.
Mood: Warm, friendly, cozy.

Art direction:
- Bright, warm, saturated colors
- Soft watercolor textures with visible brushstrokes
- Friendly, rounded character designs with big expressive eyes
- Simple, uncluttered composition
- Golden evening light
- Fireflies and small flowers around the scene
- No text, letters, words, or numbers in the image
```

### Thumbnail (YouTube náhľad)

```
Children's book watercolor illustration, thumbnail style.

Scene: {{popis_sceny}} — close-up composition suitable for a video thumbnail.
Characters: {{postavy}} — large, centered, expressive faces.
Style: Vibrant watercolor, high contrast, eye-catching colors.
Mood: {{nalada}}, inviting, curiosity-sparking.

Art direction:
- Extra bright and saturated colors for thumbnail visibility
- Large, centered character(s) filling most of the frame
- Simple background with soft bokeh effect
- Big expressive eyes and clear emotions
- No text, letters, words, or numbers in the image
```
