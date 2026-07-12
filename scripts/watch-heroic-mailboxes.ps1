param(
    [string[]]$MailboxPaths = @(
        "C:\Users\scott\Code\Heroic\heroic.md",
        "C:\Users\scott\Code\Heroic-skills\heroic.md",
        "C:\Users\scott\Code\Heroic\heroic-build\heroic.md"
    ),
    [int]$IntervalSeconds = 300
)

$ErrorActionPreference = "Stop"

function Write-Heartbeat {
    param(
        [string]$Path,
        [string]$Summary
    )

    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    Add-Content -Path $Path -Value ""
    Add-Content -Path $Path -Value "- $timestamp: $Summary"
}

while ($true) {
    foreach ($mailbox in $MailboxPaths) {
        if (-not (Test-Path -LiteralPath $mailbox)) {
            continue
        }

        $content = Get-Content -LiteralPath $mailbox -Raw
        $lineCount = ($content -split "`n").Length
        $summary = "heartbeat; read $lineCount lines from $(Split-Path -Leaf $mailbox)"
        Write-Heartbeat -Path $mailbox -Summary $summary
    }

    Start-Sleep -Seconds $IntervalSeconds
}
