# BeautyShop Backend + Ngrok Setup Guide

## Quick Start

### Option 1: Using Batch Script (Recommended for Windows)
1. Double-click `start-backend-ngrok.bat`
2. Two windows will open (Backend + Ngrok)
3. Wait for ngrok URL to appear at http://localhost:4040

### Option 2: Using PowerShell Script
1. Right-click `start-backend-ngrok.ps1` → "Run with PowerShell"
2. If blocked, run: `Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass`
3. Then run: `.\start-backend-ngrok.ps1`

### Option 3: Manual Start
```bash
# Terminal 1 - Backend
cd backend\ECommerce.API
dotnet run

# Terminal 2 - Ngrok
ngrok http 5000
```

## After Starting

1. **Get Ngrok URL:**
   - Open: http://localhost:4040
   - Copy the HTTPS URL (e.g., `https://xxxx.ngrok-free.dev`)

2. **Update Vercel:**
   - Go to: https://vercel.com/iristiqndt/beautyshop-ecommerce-seven/settings/environment-variables
   - Edit `VITE_API_URL` to: `https://xxxx.ngrok-free.dev/api`
   - Click "Save" and "Redeploy"

3. **Test Your Site:**
   - Visit: https://beautyshop-ecommerce-seven.vercel.app/
   - Try login, add to cart, etc.

## Important Notes

⚠️ **Keep Services Running:**
- Don't close Backend or Ngrok windows
- Both must run for the website to work

⚠️ **Ngrok URL Changes:**
- Free ngrok URLs change each restart
- Update Vercel whenever you restart ngrok

⚠️ **Computer Must Stay On:**
- Backend runs on your local machine
- Computer must be on + connected to internet

## Troubleshooting

### Ngrok keeps stopping:
- Don't run multiple ngrok commands
- Only one ngrok tunnel at a time
- Check http://localhost:4040 to verify it's running

### Backend won't start:
- Check SQL Server is running
- Check port 5000 is not in use
- Run: `netstat -ano | findstr :5000`

### Vercel site not connecting:
- Verify ngrok URL in Vercel settings
- Check CORS is enabled (already configured)
- Look for errors in browser console (F12)

## Ngrok Dashboard
- View tunnel status: http://localhost:4040
- View request logs and traffic
- Shows current public URL

## Stopping Services
- Close both PowerShell/CMD windows
- Or press Ctrl+C in each window
