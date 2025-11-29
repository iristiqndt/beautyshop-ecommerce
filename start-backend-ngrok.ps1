# PowerShell Script to Start Backend + Ngrok
Write-Host "===============================================" -ForegroundColor Cyan
Write-Host "Starting BeautyShop Backend + Ngrok Tunnel" -ForegroundColor Cyan
Write-Host "===============================================" -ForegroundColor Cyan
Write-Host ""

# Start Backend
Write-Host "Starting Backend API..." -ForegroundColor Yellow
$backendPath = Join-Path $PSScriptRoot "backend\ECommerce.API"
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$backendPath'; dotnet run"

# Wait for backend to start
Write-Host "Waiting for backend to initialize (10 seconds)..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

# Start Ngrok
Write-Host "Starting Ngrok Tunnel..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "ngrok http 5000 --log=stdout"

Write-Host ""
Write-Host "===============================================" -ForegroundColor Green
Write-Host "Both services started!" -ForegroundColor Green
Write-Host "===============================================" -ForegroundColor Green
Write-Host ""
Write-Host "Backend API: http://localhost:5000" -ForegroundColor White
Write-Host "Ngrok Web UI: http://localhost:4040" -ForegroundColor White
Write-Host ""
Write-Host "Wait 5 seconds then check ngrok URL..." -ForegroundColor Yellow
Start-Sleep -Seconds 5

# Try to get ngrok URL
try {
    $response = Invoke-RestMethod -Uri "http://localhost:4040/api/tunnels"
    $ngrokUrl = $response.tunnels[0].public_url
    Write-Host ""
    Write-Host "===============================================" -ForegroundColor Cyan
    Write-Host "NGROK URL: $ngrokUrl" -ForegroundColor Green
    Write-Host "===============================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Update this URL in Vercel:" -ForegroundColor Yellow
    Write-Host "$ngrokUrl/api" -ForegroundColor White
} catch {
    Write-Host "Opening ngrok web UI..." -ForegroundColor Yellow
    Start-Process "http://localhost:4040"
}

Write-Host ""
Write-Host "IMPORTANT: Keep both PowerShell windows open!" -ForegroundColor Red
Write-Host "Press any key to exit this script (services will keep running)..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
