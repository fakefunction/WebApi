@echo off
cls

set curr_dir=%cd%

chdir /D src\app

.paket\paket.exe install --redirects --clean-redirects

if errorlevel 1 (
  exit /b %errorlevel%
)
 
.paket\paket.exe install --redirects --clean-redirects

dotnet build

chdir /D CalculatorLibWeb.Function

dotnet restore
dotnet build
dotnet publish

chdir /D %curr_dir%

.paket\paket.exe install --redirects --clean-redirects

if errorlevel 1 (
  exit /b %errorlevel%
)

.paket\paket.exe install --redirects --clean-redirects

build.bat
