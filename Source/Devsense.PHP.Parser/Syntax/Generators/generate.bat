SET TOOLS_PATH=../../../../Tools/
dotnet run --project %TOOLS_PATH%/CsLex/CsLex.csproj -- PhpLexer.lex ..\Generated\Lexer.cs /version:2
dotnet run --project %TOOLS_PATH%/gppg/gppg.csproj -- /l PhpParser.y ..\Generated\Parser.cs
pause