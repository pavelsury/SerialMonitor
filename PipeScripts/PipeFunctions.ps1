function Open-SerialMonitorPort
{
    Param($PortName)
    Send-SerialMonitorMessage -PortName $PortName -Message connect
}

function Close-SerialMonitorPort
{
    param($PortName)
    Send-SerialMonitorMessage -PortName $PortName -Message disconnect
}

function Send-SerialMonitorMessage
{
    Param($PortName, $Message)
    
    Try
    {
        Send-NamedPipeMessage -PipeName "SerialMonitorPipe$PortName" -Message $Message
    }
    Catch
    {
        Write-Output $_.Exception.Message
        exit 1
    }
}

function Send-NamedPipeMessage
{
    Param(
    [String]$PipeName,           # The named pipe to send the message on.
    [string]$Message,            # The message to send the named pipe on.
    [int]$ConnectTimeout = 1000  # The number of milliseconds before the connection times out
    )

    $pipe = New-Object -TypeName System.IO.Pipes.NamedPipeClientStream -ArgumentList ".", $PipeName, ([System.IO.Pipes.PipeDirection]::InOut)
    $pipe.Connect($ConnectTimeout)
    $pipe.ReadMode = [System.IO.Pipes.PipeTransmissionMode]::Message

    $bRequest = [System.Text.Encoding]::UTF8.GetBytes($Message)
    $pipe.Write($bRequest, 0, $bRequest.Length); 
    $pipe.WaitForPipeDrain();
    $pipe.Dispose()
}
