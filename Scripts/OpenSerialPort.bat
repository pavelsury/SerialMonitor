@echo off
set portName=%1
set THIS_PATH=%~dp0\
powershell -ExecutionPolicy Bypass -File "%THIS_PATH%pipe.ps1" %portName% connect