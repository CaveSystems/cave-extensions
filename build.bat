@echo off
chcp 1252
if "%VisualStudioVersion%"=="" call "%ProgramFiles(x86)%\Microsoft Visual Studio\2019\BuildTools\Common7\Tools\VsDevCmd.bat" 2> nul
if "%VisualStudioVersion%"=="" call "%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Enterprise\Common7\Tools\VsDevCmd.bat" 2> nul
if "%VisualStudioVersion%"=="" call "%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Professional\Common7\Tools\VsDevCmd.bat" 2> nul
if "%VisualStudioVersion%"=="" call "%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Community\Common7\Tools\VsDevCmd.bat" 2> nul
if "%VisualStudioVersion%"=="" call "%ProgramFiles(x86)%\Microsoft Visual Studio\2019\WDExpress\Common7\Tools\VsDevCmd.bat" 2> nul
if "%VisualStudioVersion%"=="" call "%ProgramFiles(x86)%\Microsoft Visual Studio\2017\BuildTools\Common7\Tools\VsDevCmd.bat" 2> nul
if "%VisualStudioVersion%"=="" call "%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Enterprise\Common7\Tools\VsDevCmd.bat" 2> nul
if "%VisualStudioVersion%"=="" call "%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Professional\Common7\Tools\VsDevCmd.bat" 2> nul
if "%VisualStudioVersion%"=="" call "%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Community\Common7\Tools\VsDevCmd.bat" 2> nul
if "%VisualStudioVersion%"=="" call "%ProgramFiles(x86)%\Microsoft Visual Studio\2017\WDExpress\Common7\Tools\VsDevCmd.bat" 2> nul

dotnet clean
if %errorlevel% neq 0 exit /b %errorlevel%

dotnet restore
if %errorlevel% neq 0 exit /b %errorlevel%

msbuild /p:Configuration=Release /p:Platform="Any CPU"
if %errorlevel% neq 0 exit /b %errorlevel%

msbuild /p:Configuration=Debug /p:Platform="Any CPU"
if %errorlevel% neq 0 exit /b %errorlevel%

rem msbuild /p:Configuration=Release /p:Platform="Any CPU" documentation.shfbproj
rem if %errorlevel% neq 0 exit /b %errorlevel%

Tests\bin\Release\net20\Tests.exe
Tests\bin\Release\net35\Tests.exe
Tests\bin\Release\net40\Tests.exe
Tests\bin\Release\net45\Tests.exe
Tests\bin\Release\net46\Tests.exe
Tests\bin\Release\net47\Tests.exe