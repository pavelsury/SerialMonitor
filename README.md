# Serial Monitor 2
This is a fork of Egor Grushko's SerialMonitor extension.\
This Visual Studio extension allows you to debug programs that work with serial port.

## Features
- Read data from serial port.
- Write data to serial port.
- A lot of settings.
- Userfriendly interface.
- Support Visual Studio color scheme.
- Redirect output from serial port to file.
- Save port settings
- Autoconnection
- Hex data view
- Pipe IPC
- Automatically switch to available port
- Available as a standalone application

## What's coming next
- VS2022 support
- Make the standalone app cross-platform
- Send-command variables (like %NOW%, %NOW_UTC%, %NOW_DATE%, %NOW_TIME%, ...)
- Text color settings

## Notes on Pipe IPC
Pipe IPC stands for "Pipe Inter Process Communication".\
Here it means that you can easily open/close given port from cmd, python or any other script or app.\
\
To give it a try, first download file [PipeScripts.zip](https://github.com/pavelsury/SerialMonitor2/releases/latest/download/PipeScripts_v1.9.0.zip) and unzip.\
Then open command line and type 'OpenSerialPort COM3' or 'CloseSerialPort COM3' where COM3 is an example of a port which is currently selected in this Serial Monitor 2 extension.\
\
This Pipe IPC feature comes in handy when you are developing for Arduino or other similar device and you need to close the port temporarily in order to upload new firmware into the device.

## Notes on standalone app
Serial Monitor 2 can now be run as a standalone application.
\
You can choose from two versions:
- [**SerialMonitor2.exe**](https://github.com/pavelsury/SerialMonitor2/releases/latest/download/SerialMonitor2_v1.9.0.exe) contains everything to run the app but is little bit bigger.
- [**SerialMonitor2_without_framework.exe**](https://github.com/pavelsury/SerialMonitor2/releases/latest/download/SerialMonitor2_v1.9.0_without_framework.exe) is pretty small but it doesn't contain .NET framework. Means, you can be asked to download and install .NET framework in case your Windows doesn't have it installed yet.

Command line options:
\
*-settings_file="your settings filename"*
