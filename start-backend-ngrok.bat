@echo off
echo ===============================================
echo Starting BeautyShop Backend + Ngrok Tunnel
echo ===============================================
echo.

REM Start Backend in a new window
echo Starting Backend API...
start "Backend API" cmd /k "cd /d %~dp0backend\ECommerce.API && dotnet run"

REM Wait for backend to start
echo Waiting for backend to initialize (10 seconds)...
timeout /t 10 /nobreak

REM Start Ngrok in a new window
echo Starting Ngrok Tunnel...
start "Ngrok Tunnel" cmd /k "ngrok http 5000 --log=stdout"

echo.
echo ===============================================
echo Both services started!
echo ===============================================
echo.
echo Backend API: http://localhost:5000
echo Ngrok Web UI: http://localhost:4040
echo.
echo Wait 5 seconds then open http://localhost:4040 to see your ngrok URL
echo.
echo Press any key to open Ngrok Web UI...
pause >nul
start http://localhost:4040

echo.
echo IMPORTANT: Keep both windows open!
echo Close this window when you're done.
pause
