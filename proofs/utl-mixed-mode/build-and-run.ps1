$ErrorActionPreference = 'Stop'

$proofRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
$installations = (& $vswhere -all -products '*' -format json | ConvertFrom-Json)
$vs = $installations | Where-Object {
    Test-Path (Join-Path $_.installationPath 'VC\Auxiliary\Build\vcvars64.bat')
} | Select-Object -First 1
if (-not $vs) {
    throw 'Visual Studio with the x64 MSVC toolset is required.'
}

$vsRoot = $vs.installationPath
$msbuild = Join-Path $vsRoot 'MSBuild\Current\Bin\MSBuild.exe'
$kitsRoot = (Get-ItemProperty 'HKLM:\SOFTWARE\Microsoft\Windows Kits\Installed Roots').KitsRoot10
$sdkDesktopLib = Join-Path $kitsRoot 'Lib\10.0.26100.0\um\x64\gdi32.lib'
if (-not (Test-Path $sdkDesktopLib)) {
    throw "Windows 10 SDK 10.0.26100.0 x64 desktop libraries are missing: $sdkDesktopLib"
}

$vcxproj = Join-Path $proofRoot 'MixedModeProof.vcxproj'
& $msbuild $vcxproj /m /p:Configuration=Release /p:Platform=x64 /v:minimal
if ($LASTEXITCODE -ne 0) {
    throw "C++/CLI proof build failed with exit code $LASTEXITCODE."
}

$consumer = Join-Path $proofRoot 'Consumer\Consumer.csproj'
dotnet build $consumer -c Release
if ($LASTEXITCODE -ne 0) {
    throw "Managed consumer build failed with exit code $LASTEXITCODE."
}

$consumerDll = Join-Path $proofRoot 'Consumer\bin\Release\net10.0-windows\Consumer.dll'
dotnet $consumerDll
if ($LASTEXITCODE -ne 0) {
    throw "Managed consumer failed with exit code $LASTEXITCODE."
}
