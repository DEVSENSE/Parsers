REM @echo Off
set config=%1
if "%config%" == "" (
  set config=Release
)

set version="1.0.0.0"
if not "%PackageVersion%" == "" (
  set version=-Version %PackageVersion%
)

REM Build
%nuget% restore Source\PhpParser.sln
"%msbuildexe%" Source\PhpParser.sln /p:Configuration="%config%" /p:SkipAfterBuild="true" /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false
if not "%errorlevel%"=="0" goto failure

REM Unit tests
"%programfiles(x86)%\Microsoft Visual Studio\2017\Community\Common7\IDE\MSTest.exe" /testcontainer:Source\UnitTests\bin\Release\UnitTests.dll
if not "%errorlevel%"=="0" goto failure

REM Package
mkdir Build
call "%nuget%" pack "Source\Devsense.PHP.Parser\Devsense.PHP.Parser.nuspec" -o Build -p Configuration=%config% %version%
if not "%errorlevel%"=="0" goto failure

:success
exit 0

:failure
exit -1