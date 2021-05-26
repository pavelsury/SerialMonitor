@echo off
set PortToClose=%1
set ThisPath=%~dp0
powershell -ExecutionPolicy Bypass ". %ThisPath%\PipeFunctions.ps1; Close-SerialMonitorPort %PortToClose%"
