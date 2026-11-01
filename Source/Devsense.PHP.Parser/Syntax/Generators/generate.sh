#!/bin/bash
# Generates PHP lexer and parser using CsLex and gppg tools.
# Must be run from the Generators directory.

# Get the directory of this script
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
GENERATORS_DIR="$SCRIPT_DIR"
GENERATED_DIR="$GENERATORS_DIR/../Generated"

# Paths to tools (relative to this script)
CSLEX="$GENERATORS_DIR/../../../../Tools/CsLex/bin/Release/net10.0/linux-x64/publish/CsLex"
GPPG="$GENERATORS_DIR/../../../../Tools/gppg/bin/Release/net10.0/linux-x64/publish/gppg"

# Ensure Generated directory exists
mkdir -p "$GENERATED_DIR"

# Change to Generators directory for relative paths to work
cd "$GENERATORS_DIR"

echo "Generating PHP Lexer..."
"$CSLEX" "PhpLexer.lex" "../Generated/Lexer.cs" "/version:2"
if [ $? -ne 0 ]; then
    echo "CsLex failed with exit code $?"
    exit $?
fi

echo "Generating PHP Parser..."
"$GPPG" "/l" "PhpParser.y" "../Generated/Parser.cs"
if [ $? -ne 0 ]; then
    echo "gppg failed with exit code $?"
    exit $?
fi

echo "Generation completed successfully!"