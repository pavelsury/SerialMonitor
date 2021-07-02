Param($SolutionDir)

$deployDir = $SolutionDir + '_deploy\'
Remove-Item $deployDir -Recurse -ErrorAction Ignore
New-Item -ItemType directory -Path $deployDir | Out-Null

$manifestPath = $SolutionDir + 'SerialMonitor\source.extension.vsixmanifest'
$manifestXml = [xml](Get-Content $manifestPath -Raw)
$version = $manifestXml.PackageManifest.Metadata.Identity.Version
$vsixSrcFilename = $SolutionDir + 'SerialMonitor\bin\Deploy\SerialMonitor2.vsix'
$vsixDstFilename = $deployDir + 'SerialMonitor2_v' + $version + '.vsix'
Copy-Item $vsixSrcFilename -Destination $vsixDstFilename

$appSrcFilename = $SolutionDir + 'SerialMonitor.App\bin\Deploy\net5.0-windows\publish\framework_dependent\SerialMonitor2.exe'
$appDstFilename = $deployDir + 'SerialMonitor2_v' + $version + '_without_framework.exe'
Copy-Item $appSrcFilename -Destination $appDstFilename

$appSrcFilename = $SolutionDir + 'SerialMonitor.App\bin\Deploy\net5.0-windows\publish\self_contained\SerialMonitor2.exe'
$appDstFilename = $deployDir + 'SerialMonitor2_v' + $version + '.exe'
Copy-Item $appSrcFilename -Destination $appDstFilename

$global:ProgressPreference = 'SilentlyContinue'
$pipeScriptsSrc = $SolutionDir + 'PipeScripts\*.*'
$pipeScriptsDst = $deployDir + 'PipeScripts_v' + $version + '.zip'
Compress-Archive -Path $pipeScriptsSrc -DestinationPath $pipeScriptsDst