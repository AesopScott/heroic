param(
    [string]$UnityExe = "C:\Users\scott\Unity\Hub\Editor\6000.5.3f1\Editor\Unity.exe",
    [switch]$SkipWebGL
)

$ErrorActionPreference = "Stop"

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot "..")
$logDir = Join-Path $repoRoot "Logs"
New-Item -ItemType Directory -Force -Path $logDir | Out-Null

$method = if ($SkipWebGL) {
    "Heroic.Editor.HeroicAutomation.BuildPrototypeAndValidate"
} else {
    "Heroic.Editor.HeroicAutomation.BuildPrototypeValidateAndWebGL"
}

$logFile = Join-Path $logDir "heroic-1.0-build.log"

if (-not (Test-Path -LiteralPath $UnityExe)) {
    throw "Unity executable not found at $UnityExe"
}

$arguments = @(
    "-batchmode",
    "-quit",
    "-projectPath", $repoRoot,
    "-executeMethod", $method,
    "-logFile", $logFile
)

$process = Start-Process -FilePath $UnityExe -ArgumentList $arguments -Wait -PassThru -NoNewWindow
$exitCode = $process.ExitCode
Write-Host "Unity exit code: $exitCode"
Write-Host "Unity log: $logFile"
exit $exitCode
