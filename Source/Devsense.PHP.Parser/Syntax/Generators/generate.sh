#!/bin/bash
# Generates PHP lexer and parser using CsLex and gppg tools.

# $Tools = "$PSScriptRoot/../../../../Tools/"
# & dotnet run --project "$Tools/CsLex/CsLex.csproj" -- PhpLexer.lex ..\Generated\Lexer.cs /version:2
# & dotnet run --project "$Tools/gppg/gppg.csproj" -- /l PhpParser.y ..\Generated\Parser.cs

SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
TOOLS="$SCRIPT_DIR/../../../../Tools/"

dotnet run --project "$TOOLS/CsLex/CsLex.csproj" -- PhpLexer.lex "$SCRIPT_DIR/../Generated/Lexer.cs" /version:2
dotnet run --project "$TOOLS/gppg/gppg.csproj" -- /l PhpParser.y "$SCRIPT_DIR/../Generated/Parser.cs"