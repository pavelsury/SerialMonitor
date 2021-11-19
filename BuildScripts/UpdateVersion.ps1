Param($SolutionDir, $ProjectDir)

$appInfoPath = $SolutionDir + 'SerialMonitor.Business\AppInfo.cs'
$version = (Select-String -Path $appInfoPath -Pattern '(\d+\.)?(\d+\.)?(\*|\d+)').Matches.Value

$manifestPath = $ProjectDir + 'source.extension.vsixmanifest'
$manifestXml = [xml](Get-Content $manifestPath -Raw)
$manifestXml.PackageManifest.Metadata.Identity.Version = $version
$manifestXml.save($manifestPath)
