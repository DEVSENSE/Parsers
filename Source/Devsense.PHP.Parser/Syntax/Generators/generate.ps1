#SET TOOLS=../../../../Tools/
#dotnet run --project %TOOLS%/CsLex/CsLex.csproj -- PhpLexer.lex ..\Generated\Lexer.cs /version:2
#dotnet run --project %TOOLS%/gppg/gppg.csproj -- /l PhpParser.y ..\Generated\Parser.cs
#pause

$Tools = "$PSScriptRoot/../../../../Tools/"
& dotnet run --project "$Tools/CsLex/CsLex.csproj" -- PhpLexer.lex ..\Generated\Lexer.cs /version:2
& dotnet run --project "$Tools/gppg/gppg.csproj" -- /l PhpParser.y ..\Generated\Parser.cs

