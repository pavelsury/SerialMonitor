# Serial Monitor 2
This is a fork of Egor Grushko's SerialMonitor extension.\
This Visual Studio extension allows you to debug programs that work with serial port.\
It can be also downloaded as a standalone application.

## Features
- Read data from serial port.
- Write data to serial port.
- A lot of settings.
- User-friendly interface.
- Support Visual Studio color scheme.
- Redirect output from serial port to file.
- Save port settings
- Autoconnection
- Hex data view
- Pipe IPC
- Automatically switch to available port
- Available as a standalone application
- VS2022 support
- Send file
- Command variables

## What's coming next
- Custom send buttons
- Custom command variables
- Text color settings
- Make the standalone app cross-platform

## Notes on Pipe IPC
Pipe IPC stands for "Pipe Inter Process Communication".\
Here it means that you can easily open/close given port from cmd, python or any other script or app.\
\
To give it a try, first download file [PipeScripts.zip](https://github.com/pavelsury/SerialMonitor2/releases/latest/download/PipeScripts_v1.14.0.zip) and unzip.\
Then open command line and type 'OpenSerialPort COM3' or 'CloseSerialPort COM3' where COM3 is an example of a port which is currently selected in this Serial Monitor 2 extension.\
\
This Pipe IPC feature comes in handy when you are developing for Arduino or other similar device and you need to close the port temporarily in order to upload new firmware into the device.

## Notes on standalone app
Serial Monitor 2 can now be run as a standalone application.
\
You can choose from two versions:
- [**SerialMonitor2.exe**](https://github.com/pavelsury/SerialMonitor2/releases/latest/download/SerialMonitor2_v1.14.0.exe) contains everything to run the app but is little bit bigger.
- [**SerialMonitor2_without_framework.exe**](https://github.com/pavelsury/SerialMonitor2/releases/latest/download/SerialMonitor2_v1.14.0_without_framework.exe) is pretty small but it doesn't contain .NET framework. Means, you can be asked to download and install .NET framework in case your Windows doesn't have it installed yet.

Command line options:
\
*-settings_file="your settings filename"*
\
*-port=selected COM port name*

## Other notes
- when sending command, press Ctrl+Enter to send it without erasing
- command variables are case-insensitive
- 'Custom EOL' setting supports command variables. If for example you want to send byte 0x00 at the end of each command then use %DATA,0% (and keep 'Resolve command variables' setting checked)

## EOL command variables
- %EOL_CR%, %EOL_LF%, %EOL_CRLF%, %EOL_SKIP%
- the EOL command variables can be used anywhere in the command but if they are placed at its end then they override the 'Send EOL' setting

## Data command variables
- %DATA[default_bitwidth][default_base],value[\][value_bitwidth][value_base],value[\][value_bitwidth][value_base],...%
- attributes default_bitwidth and value_bitwidth can be: nothing, 8, 16, 32 or 64
- attributes default_base and value_base can be: nothing -> decimal, b -> binary, o -> octal, x -> hexadecimal
- base can also be specified as 0b, 0o or 0x value's prefix, see examples below
- value_bitwidth always overrides default_bitwidth
- value_base always overrides default_base
- if value has no value_bitwidth attribute and there is also no default_bitwidth attribute then the actual bitwidth is calculated automatically as necessary minimum
- if value is resolved with bitwidth 16 or bigger then its endianness is taken from port's Endianness setting (if port's Endianness is set to Default then the endianness is taken from 'Default endianness' setting)
- use 'Write resolved command to console' setting to see how the command was resolved
- use 'Write sent bytes to console' setting to see what bytes were actually sent (shown as hex values)
- Examples
  - %DATA,14,15% -> sent bytes: 0x0E 0x0F
  - %DATA16,14,15% -> sent bytes: 0x0E 0x00 0x0F 0x00
  - %DATA16,14\8,15% -> sent bytes: 0x0E 0x0F 0x00
  - %DATAo,14,15% -> sent bytes: 0x0C 0x0D
  - %DATAb,10,11% -> sent bytes: 0x02 0x03
  - %DATAb,10,11\x% -> sent bytes: 0x02 0x11
  - %DATA,0x10,0b11% -> sent bytes: 0x10 0x03
  - %DATAx16,10,11,12\8,12\32,13\64d,0b11\8% -> sent bytes: 0x10 0x00 0x11 0x00 0x12 0x12 0x00 0x00 0x00 0x0D 0x00 0x00 0x00 0x00 0x00 0x00 0x00 0x03

## Time command variables
- local time: %NOW%, %NOW_DATE%, %NOW_TIME%, %NOW_YEAR%, %NOW_MONTH%, %NOW_DAY%, %NOW_HOUR%, %NOW_MINUTE%, %NOW_SECOND%
- winter time: %WINTER_NOW%, %WINTER_NOW_DATE%, %WINTER_NOW_TIME%, %WINTER_NOW_YEAR%, %WINTER_NOW_MONTH%, %WINTER_NOW_DAY%, %WINTER_NOW_HOUR%, %WINTER_NOW_MINUTE%, %WINTER_NOW_SECOND%
- utc time: %UTC_NOW%, %UTC_NOW_DATE%, %UTC_NOW_TIME%, %UTC_NOW_YEAR%, %UTC_NOW_MONTH%, %UTC_NOW_DAY%, %UTC_NOW_HOUR%, %UTC_NOW_MINUTE%, %UTC_NOW_SECOND%

## Other command variables
- %EOF% - it is defined as %DATA,4%%EOL_SKIP%, which means that it sends just one byte 0x04 and skips any EOL
