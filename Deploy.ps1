Param($SolutionDir)

$deployDir = $SolutionDir + '_deploy\'
Remove-Item $deployDir -Recurse -ErrorAction Ignore
New-Item -ItemType directory -Path $deployDir | Out-Null

$manifestPath = $SolutionDir + 'SerialMonitor\source.extension.vsixmanifest'
$manifestXml = [xml](Get-Content $manifestPath -Raw)
$version = $manifestXml.PackageManifest.Metadata.Identity.Version
$vsixSrcFilename = $SolutionDir + 'SerialMonitor\bin\Release\SerialMonitor2.vsix'
$vsixDstFilename = $deployDir + 'SerialMonitor2_v' + $version + '.vsix'
Copy-Item $vsixSrcFilename -Destination $vsixDstFilename

$global:ProgressPreference = 'SilentlyContinue'
$pipeScriptsSrc = $SolutionDir + 'PipeScripts\*.*'
$pipeScriptsDst = $deployDir + 'PipeScripts_v' + $version + '.zip'
Compress-Archive -Path $pipeScriptsSrc -DestinationPath $pipeScriptsDst