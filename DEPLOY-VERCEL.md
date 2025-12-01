# VERCEL DEPLOYMENT GUIDE
# ========================

# Prerequisites:
# 1. Install Vercel CLI: npm install -g vercel
# 2. Login: vercel login

# Deployment Steps:

## Option 1: Deploy via CLI (Recommended)
cd C:\Users\Admin\Downloads\e-commerce-web\frontend

# Build the project
npm run build

# Deploy to Vercel
vercel --prod

# During deployment:
# - Set Project Name: beautyshop-ecommerce
# - Set Build Command: npm run build
# - Set Output Directory: dist
# - Add Environment Variable: VITE_API_URL = your-ngrok-url/api

## Option 2: Deploy via GitHub
1. Push code to GitHub repository: iristiqndt/beautyshop-ecommerce
2. Go to https://vercel.com/new
3. Import your GitHub repository
4. Configure:
   - Framework Preset: Vite
   - Root Directory: frontend
   - Build Command: npm run build
   - Output Directory: dist
5. Add Environment Variable:
   - Key: VITE_API_URL
   - Value: https://your-ngrok-url.ngrok-free.app/api
6. Click Deploy

## Important Notes:
- Ngrok URL sẽ thay đổi mỗi khi restart (nếu dùng free tier)
- Cần static domain trên Ngrok để giữ URL cố định
- Sau khi có Ngrok URL, cập nhật VITE_API_URL trong Vercel settings
