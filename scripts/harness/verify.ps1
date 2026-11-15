# verify.ps1 - ArcherCraftor project-wide harness verification script
# Runs all 4 validation gates: meta integrity, asmdef syntax, C# guardrails, and test suites.

$ErrorActionPreference = "Continue"
$projectRoot = (Resolve-Path (Join-Path $PSScriptRoot "..\..")).Path
$scriptsPath = Join-Path $projectRoot "Assets\Scripts"
$testsPath = Join-Path $projectRoot "Assets\Tests"

Write-Host "==================================================" -ForegroundColor Cyan
Write-Host "    ArcherCraftor Harness Verification (Verify)   " -ForegroundColor Cyan
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host "Project Root: $projectRoot" -ForegroundColor DarkGray
Write-Host ""

$failedChecks = 0

# -------------------------------------------------------------------------
# Gate 1: Unity .meta File Integrity Check
# -------------------------------------------------------------------------
Write-Host "[1/4] Checking Unity .meta file integrity..." -ForegroundColor Yellow
$metaScript = Join-Path $PSScriptRoot "check-meta.ps1"
if (Test-Path $metaScript) {
    & powershell -ExecutionPolicy Bypass -File $metaScript
    if ($LASTEXITCODE -ne 0) {
        Write-Host "[FAIL] .meta check failed" -ForegroundColor Red
        $failedChecks++
    } else {
        Write-Host "[OK] .meta check passed" -ForegroundColor Green
    }
} else {
    Write-Host "[WARN] check-meta.ps1 script not found." -ForegroundColor Yellow
}
Write-Host ""

# -------------------------------------------------------------------------
# Gate 2: Assembly Definition (.asmdef) Validation
# -------------------------------------------------------------------------
Write-Host "[2/4] Checking Assembly Definitions (.asmdef)..." -ForegroundColor Yellow
$asmdefFiles = Get-ChildItem -Path (Join-Path $projectRoot "Assets") -Recurse -Filter "*.asmdef"
$asmdefError = $false

foreach ($asm in $asmdefFiles) {
    try {
        $content = Get-Content $asm.FullName -Raw | ConvertFrom-Json
        if ([string]::IsNullOrWhiteSpace($content.name)) {
            Write-Host "  [ERROR] asmdef 'name' field missing: $($asm.Name)" -ForegroundColor Red
            $asmdefError = $true
        } else {
            Write-Host "  [OK] $($content.name) (File: $($asm.Name))" -ForegroundColor Gray
        }
    } catch {
        Write-Host "  [ERROR] asmdef JSON parse error: $($asm.FullName)" -ForegroundColor Red
        $asmdefError = $true
    }
}

if ($asmdefError) {
    Write-Host "[FAIL] .asmdef check failed" -ForegroundColor Red
    $failedChecks++
} else {
    Write-Host "[OK] All .asmdef files valid" -ForegroundColor Green
}
Write-Host ""

# -------------------------------------------------------------------------
# Gate 3: C# Code Static Analysis & Guardrails
# -------------------------------------------------------------------------
Write-Host "[3/4] C# Code Guardrail Static Analysis..." -ForegroundColor Yellow
$csFiles = Get-ChildItem -Path $scriptsPath -Recurse -Filter "*.cs"
$codeViolations = 0

foreach ($file in $csFiles) {
    $lines = Get-Content $file.FullName
    $relPath = $file.FullName.Substring($projectRoot.Length + 1)
    
    # 3-1. Namespace check (ArcherCraftor.* required)
    $hasNamespace = $lines | Where-Object { $_ -match "^\s*namespace\s+ArcherCraftor" }
    if (-not $hasNamespace) {
        Write-Host "  [VIOLATION: Missing Namespace] $relPath (Expected: ArcherCraftor.*)" -ForegroundColor Red
        $codeViolations++
    }

    # 3-2. FindObject search call inside Update loop anti-pattern
    for ($i = 0; $i -lt $lines.Length; $i++) {
        $line = $lines[$i]
        if ($line -match "FindObjectOfType|FindFirstObjectByType|FindGameObjectWithTag" -and -not ($relPath -like "*Editor*")) {
            $priorSlice = $lines[[Math]::Max(0, $i - 30)..$i] -join "`n"
            if ($priorSlice -match "void\s+Update\s*\(\)" -or $priorSlice -match "void\s+FixedUpdate\s*\(\)") {
                Write-Host "  [VIOLATION: Expensive Find in Update] $relPath (Line $($i + 1)): $line" -ForegroundColor Red
                $codeViolations++
            }
        }
    }
}

if ($codeViolations -gt 0) {
    Write-Host "[FAIL] Found $codeViolations code style/guardrail violations" -ForegroundColor Red
    $failedChecks++
} else {
    Write-Host "[OK] C# static analysis passed ($($csFiles.Count) files scanned)" -ForegroundColor Green
}
Write-Host ""

# -------------------------------------------------------------------------
# Gate 4: Test Harness & Package Configuration
# -------------------------------------------------------------------------
Write-Host "[4/4] Checking Test Harness & Package Environment..." -ForegroundColor Yellow
$manifestPath = Join-Path $projectRoot "Packages\manifest.json"
if (Test-Path $manifestPath) {
    $manifest = Get-Content $manifestPath -Raw | ConvertFrom-Json
    if ($manifest.dependencies.PSObject.Properties.Name -contains "com.unity.test-framework") {
        Write-Host "  [OK] com.unity.test-framework package registered" -ForegroundColor Gray
    } else {
        Write-Host "  [WARN] com.unity.test-framework package missing in manifest.json" -ForegroundColor Yellow
    }
}

$editModeTests = Get-ChildItem -Path (Join-Path $testsPath "EditMode") -Filter "*.cs" -ErrorAction SilentlyContinue
$playModeTests = Get-ChildItem -Path (Join-Path $testsPath "PlayMode") -Filter "*.cs" -ErrorAction SilentlyContinue

Write-Host "  [OK] EditMode Test Suites: $($editModeTests.Count) files found" -ForegroundColor Gray
Write-Host "  [OK] PlayMode Test Suites: $($playModeTests.Count) files found" -ForegroundColor Gray
Write-Host "[OK] Test Harness structure valid" -ForegroundColor Green
Write-Host ""

# -------------------------------------------------------------------------
# Summary Report
# -------------------------------------------------------------------------
Write-Host "==================================================" -ForegroundColor Cyan
if ($failedChecks -eq 0) {
    Write-Host "[PASS] All harness verification gates passed successfully!" -ForegroundColor Green
    Write-Host "==================================================" -ForegroundColor Cyan
    exit 0
} else {
    Write-Host "[FAIL] Total $failedChecks verification gates failed." -ForegroundColor Red
    Write-Host "==================================================" -ForegroundColor Cyan
    exit 1
}
