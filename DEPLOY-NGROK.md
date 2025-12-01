# NGROK DEPLOYMENT SCRIPT
# ========================

# Step 1: Authenticate ngrok (chỉ cần làm 1 lần)
# Lấy authtoken từ: https://dashboard.ngrok.com/get-started/your-authtoken
# Chạy: ngrok config add-authtoken YOUR_AUTH_TOKEN

# Step 2: Start Backend
cd C:\Users\Admin\Downloads\e-commerce-web\backend\ECommerce.API
dotnet run

# Step 3: In another terminal, start ngrok
ngrok http 5000 --domain=YOUR-STATIC-DOMAIN.ngrok-free.app
# Hoặc nếu không có static domain:
# ngrok http 5000

# Copy ngrok URL (vd: https://abc123.ngrok-free.app)
# và cập nhật vào frontend/.env.production:
# VITE_API_URL=https://abc123.ngrok-free.app/api
