
@echo off
cls
set curr_dir=%cd%

chdir /D CredentialProvider

CredentialProvider.VSS.exe -U https://pkgs.dev.azure.com/CalculatorCompanyDev/_packaging/CalculatorLibWeb/nuget/v3/index.json

if errorlevel 1 (
  exit /b %errorlevel%
)

chdir /D %curr_dir%

".paket/paket.exe" config add-credentials https://pkgs.dev.azure.com/CalculatorCompanyDev/_packaging/CalculatorLibWeb/nuget/v3/index.json  --verify