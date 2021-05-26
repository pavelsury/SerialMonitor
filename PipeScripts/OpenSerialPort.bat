@echo off
set PortToOpen=%1
set ThisPath=%~dp0
powershell -ExecutionPolicy Bypass ". %ThisPath%\PipeFunctions.ps1; Open-SerialMonitorPort %PortToOpen%"
