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

## Notes
Pipe IPC stands for "Pipe Inter Process Communication".\
Here it means that you can easily open/close given port from cmd, python or any other script or app.\
\
To give it a try, first download file PipeScripts_v1.7.0.zip from latest release's assets and unzip.\
Then open command line and type 'OpenSerialPort COM3' or 'CloseSerialPort COM3' where COM3 is an example of a port which is currently selected in this Serial Monitor 2 extension.\
\
This Pipe IPC feature comes in handy when you are developing for Arduino or other similar device and you need to close the port temporarily in order to upload new firmware into the device.
