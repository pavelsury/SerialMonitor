Param($SolutionDir, $ProjectDir)

$versionPattern = '(\d+)\.(\d+)\.(\d+)'

$appInfoPath = $SolutionDir + 'SerialMonitor.Business\AppInfo.cs'
$version = (Select-String -Path $appInfoPath -Pattern $versionPattern).Matches.Value

$manifestPath = $ProjectDir + 'source.extension.vsixmanifest'
$manifestXml = [xml](Get-Content $manifestPath -Raw)
$manifestXml.PackageManifest.Metadata.Identity.Version = $version
$manifestXml.save($manifestPath)

$readmePath = $SolutionDir + 'README.md'
$readmeContent = Get-Content -path $readmePath -Raw
$readmeContent = $readmeContent -Replace $versionPattern, $version
[system.io.file]::WriteAllText($readmePath, $readmeContent)
