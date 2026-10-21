$ErrorActionPreference = 'Stop'

$spikeRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$artifacts = Join-Path $spikeRoot 'artifacts'
New-Item -ItemType Directory -Force $artifacts | Out-Null

$sdk = Get-ChildItem (Join-Path $env:ProgramFiles 'dotnet\sdk') -Directory |
    Where-Object Name -Match '^\d+\.\d+' |
    Sort-Object { [version]$_.Name } -Descending | Select-Object -First 1
$referencePack = Get-ChildItem (Join-Path $env:ProgramFiles 'dotnet\packs\Microsoft.NETCore.App.Ref') -Directory |
    Where-Object Name -Match '^\d+\.\d+' |
    Sort-Object { [version]$_.Name } -Descending | Select-Object -First 1
if (-not $sdk -or -not $referencePack) {
    throw 'A .NET 10 SDK and reference pack are required.'
}

$compiler = Join-Path $sdk.FullName 'Roslyn\bincore\csc.dll'
$references = Join-Path $referencePack.FullName 'ref\net10.0'
$response = Join-Path $artifacts 'references.rsp'
Get-ChildItem $references -Filter '*.dll' | ForEach-Object {
    '/reference:"' + $_.FullName + '"'
} | Set-Content $response -Encoding utf8

$module = Join-Path $artifacts 'ModuleApi.netmodule'
dotnet $compiler /noconfig /nostdlib /target:module "/out:$module" "@$response" (Join-Path $spikeRoot 'ModuleApi.cs')
if ($LASTEXITCODE -ne 0) { throw 'C# module build failed.' }

$facade = Join-Path $artifacts 'Facade.dll'
dotnet $compiler /noconfig /nostdlib /target:library "/out:$facade" "/addmodule:$module" "@$response" (Join-Path $spikeRoot 'Facade.cs')
if ($LASTEXITCODE -ne 0) { throw 'Manifest assembly build failed.' }

$consumer = Join-Path $spikeRoot 'Consumer\Consumer.csproj'
dotnet build $consumer -c Release
if ($LASTEXITCODE -ne 0) { throw 'Managed consumer build failed.' }

$consumerDll = Join-Path $spikeRoot 'Consumer\bin\Release\net10.0-windows\Consumer.dll'
$output = (& dotnet $consumerDll 2>&1 | Out-String)
$exitCode = $LASTEXITCODE
if ($exitCode -eq 0 -or $output -notmatch 'only single file assemblies are supported') {
    throw "Expected the .NET 10 multi-module load failure; exit=$exitCode output=$output"
}
Write-Output $output.Trim()
