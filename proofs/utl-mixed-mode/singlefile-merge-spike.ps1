param(
    [string]$MixedModeDll = (Join-Path $PSScriptRoot 'artifacts\MixedModeProof.dll')
)

$ErrorActionPreference = 'Stop'
$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$artifacts = Join-Path $PSScriptRoot 'artifacts'
$utlOutput = Join-Path $artifacts 'Utl'
$mergedOutput = Join-Path $artifacts 'Merged.dll'
$toolPath = Join-Path $artifacts 'tools'
$ilrepack = Join-Path $toolPath 'ilrepack.exe'

if (-not (Test-Path $MixedModeDll)) {
    throw "Build MixedModeProof.vcxproj first or pass its DLL path: $MixedModeDll"
}

dotnet build (Join-Path $repoRoot 'src\Quire.Engine.Utl\Quire.Engine.Utl.csproj') `
    -c Release --no-incremental -o $utlOutput
if ($LASTEXITCODE -ne 0) { throw 'Release build of Quire.Engine.Utl failed.' }

if (-not (Test-Path $ilrepack)) {
    dotnet tool install --tool-path $toolPath dotnet-ilrepack --version 2.0.48
    if ($LASTEXITCODE -ne 0) { throw 'Installing the local ILRepack tool failed.' }
}

$utlDll = Join-Path $utlOutput 'Quire.Engine.Utl.dll'
$plainOutput = Join-Path $artifacts 'Merged-plain.dll'
$plain = (& $ilrepack "/out:$plainOutput" $MixedModeDll $utlDll "/lib:$utlOutput" 2>&1 | Out-String)
$plainExit = $LASTEXITCODE
if ($plainExit -eq 0 -or $plain -notmatch 'Zero PeKind') {
    throw "Expected ILRepack to reject the mixed-mode primary without /zeropekind. Exit=$plainExit`n$plain"
}

$zero = (& $ilrepack '/zeropekind' "/out:$mergedOutput" $MixedModeDll $utlDll "/lib:$utlOutput" 2>&1 | Out-String)
$zeroExit = $LASTEXITCODE
if ($zeroExit -eq 0 -or $zero -notmatch 'Writing mixed-mode assemblies is not supported') {
    throw "Expected ILRepack's writer to reject mixed-mode output. Exit=$zeroExit`n$zero"
}

Write-Output 'Without /zeropekind:'
Write-Output $plain.Trim()
Write-Output 'With /zeropekind:'
Write-Output $zero.Trim()
