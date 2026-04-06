# Šablóna pre generovanie rozprávky

## Premenné

| Premenná          | Popis                              | Príklad                          |
| ----------------- | ---------------------------------- | -------------------------------- |
| `{{dlzka_minut}}` | Cieľová dĺžka rozprávky v minútach| 5                                |
| `{{tema}}`        | Hlavná téma rozprávky              | priateľstvo medzi zvieratami     |
| `{{moral}}`       | Morálne ponaučenie                 | Pravý priateľ ti pomôže v núdzi  |
| `{{postavy}}`     | Hlavné postavy rozprávky           | malý zajačik Ušiak, múdra sova  |
| `{{prostredie}}`  | Prostredie, kde sa rozprávka odohráva| čarovný les na úpätí hôr       |

---

## Prompt

```
Napíš originálnu slovenskú rozprávku pre deti od 6 rokov.

### Základné parametre
- **Dĺžka:** {{dlzka_minut}} minút (približne {{dlzka_minut × 140}} slov)
- **Téma:** {{tema}}
- **Morálne ponaučenie:** {{moral}}
- **Hlavné postavy:** {{postavy}}
- **Prostredie:** {{prostredie}}

### Štruktúra rozprávky
1. **Úvod (15%)** – Predstav prostredie a hlavné postavy. Začni klasickým rozprávkovým úvodom (napr. „Kde bolo, tam bolo..." alebo „Za siedmimi horami, za siedmimi dolinami...").
2. **Zápletka (40%)** – Predstav hlavný problém alebo výzvu. Použi pravidlo troch (tri pokusy, tri úlohy, tri prekážky).
3. **Vyvrcholenie (20%)** – Hlavná postava čelí najväčšej výzve. Použi odvahu, múdrosť alebo láskavosť na prekonanie problému.
4. **Rozuzlenie (15%)** – Šťastný koniec. Problém je vyriešený, postavy sa poučili.
5. **Ponaučenie (10%)** – Jasne vyjadrené morálne ponaučenie na záver.

### Štýl písania

> **Referencia**: Pred písaním textu načítaj `config/writing-style-prompt.md` — kompletný štýlový sprievodca.

- Jednoduché, krátke vety (priemerne 8–12 slov, max 25 slov)
- Slovenčina primeraná deťom od 6 rokov
- Žiadny trpný rod – používaj činný rod
- Žiadna irónia, sarkazmus ani dvojzmyselný humor
- Teplý, priateľský a pútavý tón rozprávania
- Živé dialógy medzi postavami
- Obrazný jazyk a opisy prírody
- Opakovanie fráz pre lepšie zapamätanie

### Rozprávkové prvky
- Čarovné predmety, hovoriace zvieratá alebo magické miesta
- Pravidlo troch (tri výzvy, tri postavy, tri opakovania)
- Jasný kontrast medzi dobrom a zlom (ale záporná postava sa môže polepšiť)
- Transformácia hlavnej postavy (niečo sa naučí alebo zmení)

### Zakázaný obsah
- Násilie, krutosť, smrť
- Opustenie detí, strašidelné scény
- Stereotypy akéhokoľvek druhu
- Reklama alebo komerčný obsah

### Formát výstupu
- Nadpis rozprávky
- Text rozprávky rozdelený na odseky
- Na konci: morálne ponaučenie oddelené čiarou
- Celkový počet slov
```

---

## Príklad použitia

```
Napíš originálnu slovenskú rozprávku pre deti od 6 rokov.

### Základné parametre
- **Dĺžka:** 5 minút (približne 700 slov)
- **Téma:** priateľstvo medzi zvieratami
- **Morálne ponaučenie:** Pravý priateľ ti pomôže v núdzi
- **Hlavné postavy:** malý zajačik Ušiak, múdra sova Hedviga, medvedík Bručko
- **Prostredie:** čarovný les na úpätí Tatier

...
```
