Param($SolutionDir)

$appVersionPath = $SolutionDir + 'SerialMonitor.Business\AppVersion.cs'
$version = (Select-String -Path $appVersionPath -Pattern '(\d+\.)?(\d+\.)?(\*|\d+)').Matches.Value
$manifestPath = $SolutionDir + 'SerialMonitor\source.extension.vsixmanifest'
$manifestXml = [xml](Get-Content $manifestPath -Raw)
$manifestXml.PackageManifest.Metadata.Identity.Version = $version
$manifestXml.save($manifestPath)
