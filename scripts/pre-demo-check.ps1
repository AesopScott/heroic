param(
    [int]$Port = 5199,
    [string]$HostName = "127.0.0.1",
    [switch]$SkipBuild
)

$ErrorActionPreference = "Stop"

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot "..")
$buildRoot = Join-Path $repoRoot "Builds\WebGL"
$serverScript = Join-Path $PSScriptRoot "serve-webgl.py"
$buildScript = Join-Path $PSScriptRoot "run-unity-1.0.ps1"
$url = "http://$HostName`:$Port/"

if (-not $SkipBuild) {
    & $buildScript
    if ($LASTEXITCODE -ne 0) {
        throw "Unity 1.0 build failed with exit code $LASTEXITCODE."
    }
}

if (-not (Test-Path -LiteralPath $buildRoot)) {
    throw "WebGL build directory not found at $buildRoot."
}

$listenerOwners = Get-NetTCPConnection -LocalPort $Port -State Listen -ErrorAction SilentlyContinue |
    Select-Object -ExpandProperty OwningProcess -Unique

foreach ($ownerProcessId in $listenerOwners) {
    if ($ownerProcessId -gt 0) {
        Stop-Process -Id $ownerProcessId -Force -ErrorAction SilentlyContinue
    }
}

$server = Start-Process -FilePath "python" `
    -ArgumentList @($serverScript, "--host", $HostName, "--port", "$Port", "--directory", $buildRoot) `
    -WindowStyle Hidden `
    -PassThru

Start-Sleep -Seconds 1

$headers = & curl.exe -I "$url`Build/WebGL.data.gz"
if ($LASTEXITCODE -ne 0) {
    throw "WebGL server header check failed."
}

$headerText = $headers -join "`n"
if ($headerText -notmatch "Content-Encoding:\s*gzip") {
    Stop-Process -Id $server.Id -Force -ErrorAction SilentlyContinue
    throw "WebGL server is missing Content-Encoding: gzip for Unity .gz assets."
}

if ($headerText -notmatch "Cross-Origin-Embedder-Policy:\s*require-corp") {
    Stop-Process -Id $server.Id -Force -ErrorAction SilentlyContinue
    throw "WebGL server is missing Cross-Origin-Embedder-Policy: require-corp."
}

Write-Host ""
Write-Host "Heroic 1.0 pre-demo check passed."
Write-Host "Server PID: $($server.Id)"
Write-Host "Open: $url"
Write-Host ""
Write-Host "Demo controls:"
Write-Host "- Move: WASD / Arrow Keys"
Write-Host "- Movement skills: 1 Blink, 2 Lunge, 3 Teleport"
Write-Host "- Pause: Esc"
Write-Host "- Music: M mute/unmute"
Write-Host "- Master volume: - / +"
Write-Host "- Prototype safety: F8 force defeat, F9 force victory"
Write-Host ""
Write-Host "Recommended flow:"
Write-Host "1. Start Run"
Write-Host "2. Move immediately and use 1/2/3 once each"
Write-Host "3. Let XP pull in and choose the first draft"
Write-Host "4. Survive to the 02:00 Warden result, or use F8/F9 if needed"
Write-Host ""
Write-Host "Stop server after demo:"
Write-Host "Stop-Process -Id $($server.Id) -Force"
