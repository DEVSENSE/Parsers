#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Generates PHP lexer and parser using CsLex and gppg tools.
.DESCRIPTION
    This script runs CsLex to generate the lexer from PhpLexer.lex
    and gppg to generate the parser from PhpParser.y.
#>

# Get the directory of this script
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
$GeneratorsDir = $ScriptDir
$GeneratedDir = Join-Path $GeneratorsDir "..\Generated"

# Paths to tools (relative to this script)
$CsLex = Join-Path $GeneratorsDir "..\..\..\Tools\CsLex\bin\Release\net10.0\linux-x64\publish\CsLex"
$Gppg = Join-Path $GeneratorsDir "..\..\..\Tools\gppg\bin\Release\net10.0\linux-x64\publish\gppg"

# On Windows, use .exe extension
if ($IsWindows) {
    $CsLex = "$CsLex.exe"
    $Gppg = "$Gppg.exe"
}

# Ensure Generated directory exists
if (-not (Test-Path $GeneratedDir)) {
    New-Item -ItemType Directory -Path $GeneratedDir -Force | Out-Null
}

Write-Host "Generating PHP Lexer..." -ForegroundColor Green
& $CsLex "PhpLexer.lex" (Join-Path $GeneratedDir "Lexer.cs") "/version:2"
if ($LASTEXITCODE -ne 0) {
    Write-Error "CsLex failed with exit code $LASTEXITCODE"
    exit $LASTEXITCODE
}

Write-Host "Generating PHP Parser..." -ForegroundColor Green
& $Gppg "/l" "PhpParser.y" (Join-Path $GeneratedDir "Parser.cs")
if ($LASTEXITCODE -ne 0) {
    Write-Error "gppg failed with exit code $LASTEXITCODE"
    exit $LASTEXITCODE
}

Write-Host "Generation completed successfully!" -ForegroundColor Green