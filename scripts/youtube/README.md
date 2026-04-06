# YouTube Upload — Nastavenie

Automatizovaný upload na YouTube vyžaduje **Google Cloud OAuth 2.0** prístup.

## Kroky na nastavenie (jednorazovo):

### 1. Google Cloud Console
1. Choď na [console.cloud.google.com](https://console.cloud.google.com/)
2. Vytvor nový projekt (napr. "PoucneRozpravky")
3. Zapni **YouTube Data API v3**:
   - Menu → APIs & Services → Library → hľadaj "YouTube Data API v3" → Enable

### 2. OAuth 2.0 Credentials
1. Menu → APIs & Services → Credentials → Create Credentials → **OAuth client ID**
2. Application type: **Desktop app**
3. Názov: "PoucneRozpravky Upload"
4. Stiahni JSON súbor → ulož ako `scripts/youtube/client_secret.json`

### 3. Prvé spustenie
```powershell
pip install google-auth-oauthlib google-api-python-client
python scripts/youtube/upload.py --file "rozpravky/.../video/rozpravka.mp4"
```
Pri prvom spustení sa otvorí prehliadač na autorizáciu. Token sa uloží do `scripts/youtube/token.json`.

### 4. Ďalšie uploady
Po prvej autorizácii funguje upload automaticky bez interakcie.

## Súbory
- `client_secret.json` — OAuth credentials (NIKDY necommitovať!)
- `token.json` — refresh token (NIKDY necommitovať!)
- `upload.py` — upload skript
