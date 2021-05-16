@echo off
set PortToClose=%1
set THIS_PATH=%~dp0\
powershell -ExecutionPolicy Bypass -File "%THIS_PATH%pipe.ps1" %PortToClose% disconnect