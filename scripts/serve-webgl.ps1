param(
    [int]$Port = 5177,
    [string]$HostName = "127.0.0.1"
)

$ErrorActionPreference = "Stop"

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot "..")
$buildRoot = Join-Path $repoRoot "Builds\WebGL"
$serverScript = Join-Path $PSScriptRoot "serve-webgl.py"

if (-not (Test-Path -LiteralPath $buildRoot)) {
    throw "WebGL build directory not found at $buildRoot. Run .\scripts\run-unity-1.0.ps1 first."
}

python $serverScript --host $HostName --port $Port --directory $buildRoot
