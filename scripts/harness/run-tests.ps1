# run-tests.ps1 - Unity UTF 테스트 실행 스크립트
# Unity 에디터를 배치모드로 실행하여 EditMode / PlayMode 테스트를 수행하고 결과를 확인합니다.

param (
    [string]$TestPlatform = "EditMode", # EditMode, PlayMode
    [string]$UnityEditorPath = "",
    [string]$TestResultsFile = "TestResults.xml"
)

$ErrorActionPreference = "Continue"
$projectRoot = (Resolve-Path (Join-Path $PSScriptRoot "..\..")).Path

Write-Host "=== ArcherCraftor Unity 테스트 하네스 실행 ===" -ForegroundColor Cyan
Write-Host "테스트 플랫폼: $TestPlatform" -ForegroundColor Yellow
Write-Host "프로젝트 경로: $projectRoot" -ForegroundColor DarkGray

# Unity 에디터 실행 파일 경로 탐색
if ([string]::IsNullOrEmpty($UnityEditorPath)) {
    $candidates = @(
        "C:\Program Files\Unity\Hub\Editor\6000.3.22f1\Editor\Unity.exe",
        "C:\Program Files\Unity\Editor\Unity.exe",
        "C:\Program Files\Unity Hub\Editor\*\Editor\Unity.exe"
    )
    foreach ($pattern in $candidates) {
        $found = Get-Item -Path $pattern -ErrorAction SilentlyContinue | Select-Object -First 1
        if ($found) {
            $UnityEditorPath = $found.FullName
            break
        }
    }
}

if (-not (Test-Path $UnityEditorPath)) {
    Write-Host "⚠️ 로컬 Unity 6 에디터 실행 경로가 기본 위치에서 발견되지 않았습니다." -ForegroundColor Yellow
    Write-Host "Unity Hub에서 에디터가 설치된 경로를 -UnityEditorPath 인자로 전달하거나," -ForegroundColor Gray
    Write-Host "Unity 에디터 내부의 [Window] -> [General] -> [Test Runner] 창에서 테스트를 직접 실행할 수 있습니다." -ForegroundColor Gray
    Write-Host ""
    Write-Host "수동 실행 명령어 예시:" -ForegroundColor DarkGray
    Write-Host "& '$UnityEditorPath' -batchmode -projectPath '$projectRoot' -runTests -testPlatform $TestPlatform -testResults '$projectRoot\$TestResultsFile'" -ForegroundColor Cyan
    exit 0
}

Write-Host "Unity 실행기 감지: $UnityEditorPath" -ForegroundColor Green
$logFile = Join-Path $projectRoot "Logs\TestRunner.log"
$resultsPath = Join-Path $projectRoot $TestResultsFile

$args = @(
    "-batchmode",
    "-projectPath", "`"$projectRoot`"",
    "-runTests",
    "-testPlatform", $TestPlatform,
    "-testResults", "`"$resultsPath`"",
    "-logFile", "`"$logFile`""
)

Write-Host "테스트 배치모드 실행 중... (로그: $logFile)" -ForegroundColor Cyan
$process = Start-Process -FilePath $UnityEditorPath -ArgumentList $args -Wait -PassThru -NoNewWindow

if ($process.ExitCode -eq 0) {
    Write-Host "✅ 테스트 통과!" -ForegroundColor Green
    if (Test-Path $resultsPath) {
        Write-Host "결과 파일: $resultsPath" -ForegroundColor DarkGray
    }
    exit 0
} else {
    Write-Host "❌ 테스트 실패 또는 에러 발생 (ExitCode: $($process.ExitCode))" -ForegroundColor Red
    if (Test-Path $logFile) {
        Get-Content $logFile -Tail 30
    }
    exit $process.ExitCode
}

