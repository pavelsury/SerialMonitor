@echo off
set PortToOpen=%1
set ThisPath=%~dp0
powershell -ExecutionPolicy Bypass -File "%ThisPath%\pipe.ps1" %PortToOpen% connect