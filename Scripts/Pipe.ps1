param($portName, $message)

function Send-NamedPipeMessage
{
    param(
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

try {
    Send-NamedPipeMessage -PipeName "SerialMonitorPipe$portName" -Message $message
} catch {
    exit 1
}
