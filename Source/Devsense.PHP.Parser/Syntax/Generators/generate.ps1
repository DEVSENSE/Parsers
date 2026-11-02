# PowerShell script to generate PHP parser lexer and parser
# Cross-platform replacement for generate.bat

# Get the directory of this script
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition

# Paths to the tools (assuming they're in the same directory as this script or in PATH)
$cslex = Join-Path $scriptDir "CsLex.exe"
$gppg = Join-Path $scriptDir "gppg.exe"

# If not found locally, try from PATH
if (-not (Test-Path $cslex)) {
    $cslex = "CsLex.exe"
}
if (-not (Test-Path $gppg)) {
    $gppg = "gppg.exe"
}

# Input/output files
$lexerInput = Join-Path $scriptDir "PhpLexer.lex"
$lexerOutput = Join-Path $scriptDir "..\Generated\Lexer.cs"
$parserInput = Join-Path $scriptDir "PhpParser.y"
$parserOutput = Join-Path $scriptDir "..\Generated\Parser.cs"

# Run CsLex
Write-Host "Running CsLex..."
& $cslex $lexerInput $lexerOutput /version:2
if ($LASTEXITCODE -ne 0) {
    Write-Error "CsLex failed with exit code $LASTEXITCODE"
    exit $LASTEXITCODE
}

# Run gppg
Write-Host "Running gppg..."
& $gppg /l $parserInput $parserOutput
if ($LASTEXITCODE -ne 0) {
    Write-Error "gppg failed with exit code $LASTEXITCODE"
    exit $LASTEXITCODE
}

Write-Host "Generation completed successfully."