# setup-external-assets.ps1 - Prepare local Assets/ExternalAssets folder and optionally extract a shared archive
# Allows sharing heavy assets (video, PSD, audio packs) via manual zip archives without Git LFS or cloud sync locks.

param (
    [string]$ArchivePath = ""
)

$ErrorActionPreference = "Stop"

$projectRoot = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$assetsDir = Join-Path $projectRoot "Assets"
$linkPath = Join-Path $assetsDir "ExternalAssets"
$metaPath = Join-Path $assetsDir "ExternalAssets.meta"

# Fixed folder GUID so all team members resolve the ExternalAssets folder consistently
$fixedFolderGuid = "e8f0a1b2c3d44e5f8a9b0c1d2e3f4a5b"

if (-not (Test-Path -LiteralPath $assetsDir)) {
    Write-Error "Assets directory not found: $assetsDir"
    exit 1
}

Write-Host "=== ArcherCraftor ExternalAssets Setup ===" -ForegroundColor Cyan
Write-Host "Local Folder : $linkPath" -ForegroundColor DarkGray
if (-not [string]::IsNullOrWhiteSpace($ArchivePath)) {
    Write-Host "Archive File : $ArchivePath" -ForegroundColor DarkGray
}

# 1. Remove legacy NTFS Junction safely without touching remote target data, then ensure normal local folder
$existingItem = Get-Item -LiteralPath $linkPath -Force -ErrorAction SilentlyContinue
if ($null -ne $existingItem) {
    $isReparsePoint = ($existingItem.Attributes -band [System.IO.FileAttributes]::ReparsePoint) -ne 0
    if ($isReparsePoint) {
        $rmdirOutput = cmd /c rmdir "`"$linkPath`"" 2>&1
        if ($LASTEXITCODE -ne 0) {
            Write-Error "Failed to remove existing Junction '$linkPath'. Output: $rmdirOutput"
            exit 1
        }
        Write-Host "[UNLINKED] Removed legacy Junction: $linkPath" -ForegroundColor Yellow
    } elseif (-not $existingItem.PSIsContainer) {
        Write-Error "A file already exists at '$linkPath'. Remove or move it and re-run this script."
        exit 1
    }
}

if (-not (Test-Path -LiteralPath $linkPath)) {
    New-Item -ItemType Directory -Path $linkPath -Force | Out-Null
    Write-Host "[CREATED] Local directory: $linkPath" -ForegroundColor Green
} else {
    Write-Host "[OK] Local directory exists: $linkPath" -ForegroundColor Gray
}

# 2. Ensure consistent Assets/ExternalAssets.meta exists
if (-not (Test-Path -LiteralPath $metaPath)) {
    $metaContent = @(
        "fileFormatVersion: 2",
        "guid: $fixedFolderGuid",
        "folderAsset: yes",
        "DefaultImporter:",
        "  externalObjects: {}",
        "  userData: ",
        "  assetBundleName: ",
        "  assetBundleVariant: ",
        ""
    ) -join "`n"
    [System.IO.File]::WriteAllText($metaPath, $metaContent, [System.Text.UTF8Encoding]::new($false))
    Write-Host "[CREATED] Folder meta generated with shared GUID: $fixedFolderGuid" -ForegroundColor Green
} else {
    Write-Host "[OK] ExternalAssets.meta already exists." -ForegroundColor Gray
}

# 3. Extract optional zip archive into Assets/ExternalAssets
if (-not [string]::IsNullOrWhiteSpace($ArchivePath)) {
    if (-not (Test-Path -LiteralPath $ArchivePath -PathType Leaf)) {
        Write-Error "Archive file not found: $ArchivePath"
        exit 1
    }

    Write-Host "Extracting archive into $linkPath..." -ForegroundColor Cyan
    Expand-Archive -LiteralPath $ArchivePath -DestinationPath $linkPath -Force
    Write-Host "[EXTRACTED] $ArchivePath -> $linkPath" -ForegroundColor Green
}

Write-Host "----------------------------------------" -ForegroundColor DarkGray
Write-Host "[DONE] Local ExternalAssets setup complete." -ForegroundColor Green
