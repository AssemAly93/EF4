@echo off
cd /d "%~dp0"
set LOG=%~dp0update-database.log
echo === %date% %time% === > "%LOG%"
where dotnet >> "%LOG%" 2>&1 || (echo ERROR: .NET SDK not found >> "%LOG%" & type "%LOG%" & pause & exit /b 1)
set PATH=%PATH%;%USERPROFILE%\.dotnet\tools
dotnet ef --version >> "%LOG%" 2>&1
if errorlevel 1 (
  echo Installing dotnet-ef 8.0.11... >> "%LOG%"
  dotnet tool install --global dotnet-ef --version 8.0.11 >> "%LOG%" 2>&1
)
echo. >> "%LOG%"
echo === dotnet ef database update === >> "%LOG%"
dotnet ef database update --project EF4\EF4.csproj >> "%LOG%" 2>&1
echo EXITCODE=%errorlevel% >> "%LOG%"
type "%LOG%"
echo.
echo Done. You can close this window.
pause
