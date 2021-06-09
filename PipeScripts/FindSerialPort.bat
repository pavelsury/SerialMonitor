@echo off
set ThisPath=%~dp0
set SerialMonitorPort=
for /f "delims=" %%a in ('powershell -ExecutionPolicy Bypass ". %ThisPath%\PipeFunctions.ps1; Find-SerialMonitorPort"') do set "SerialMonitorPort=%%a"
