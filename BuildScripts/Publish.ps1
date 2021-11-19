Param($SolutionDir)

$publishDir = $SolutionDir + '_publish\'
Remove-Item $publishDir -Recurse -ErrorAction Ignore
New-Item -ItemType directory -Path $publishDir | Out-Null

$manifestPath = $SolutionDir + 'SerialMonitor.VS2019\source.extension.vsixmanifest'
$manifestXml = [xml](Get-Content $manifestPath -Raw)
$version = $manifestXml.PackageManifest.Metadata.Identity.Version
$vsixSrcFilename = $SolutionDir + 'SerialMonitor.VS2019\bin\Release\SerialMonitor2_VS2019.vsix'
$vsixDstFilename = $publishDir + 'SerialMonitor2_v' + $version + '.vsix'
Copy-Item $vsixSrcFilename -Destination $vsixDstFilename

$manifestPath = $SolutionDir + 'SerialMonitor.VS2022\source.extension.vsixmanifest'
$manifestXml = [xml](Get-Content $manifestPath -Raw)
$version = $manifestXml.PackageManifest.Metadata.Identity.Version
$vsixSrcFilename = $SolutionDir + 'SerialMonitor.VS2022\bin\Release\SerialMonitor2_VS2022.vsix'
$vsixDstFilename = $publishDir + 'SerialMonitor2_v' + $version + '_VS2022.vsix'
Copy-Item $vsixSrcFilename -Destination $vsixDstFilename

$appSrcFilename = $SolutionDir + 'SerialMonitor.Win.App\bin\Release\net6.0-windows\publish\framework_dependent\SerialMonitor2.exe'
$appDstFilename = $publishDir + 'SerialMonitor2_v' + $version + '_without_framework.exe'
Copy-Item $appSrcFilename -Destination $appDstFilename

$appSrcFilename = $SolutionDir + 'SerialMonitor.Win.App\bin\Release\net6.0-windows\publish\self_contained\SerialMonitor2.exe'
$appDstFilename = $publishDir + 'SerialMonitor2_v' + $version + '.exe'
Copy-Item $appSrcFilename -Destination $appDstFilename

$global:ProgressPreference = 'SilentlyContinue'
$pipeScriptsSrc = $SolutionDir + 'PipeScripts\*.*'
$pipeScriptsDst = $publishDir + 'PipeScripts_v' + $version + '.zip'
Compress-Archive -Path $pipeScriptsSrc -DestinationPath $pipeScriptsDst