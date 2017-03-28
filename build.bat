@echo Off
set config=%1
if "%config%" == "" (
   set config=Release
)

set version=
if not "%PackageVersion%" == "" (
   set version=-Version %PackageVersion%
)

set nunit="tools\nunit\nunit-console.exe"

REM Build
"%programfiles(x86)%\MSBuild\14.0\Bin\MSBuild.exe" Source\PhpParser.sln /p:Configuration="%config%" /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false
if not "%errorlevel%"=="0" goto failure

REM Unit tests
%MsTestExe% Source\UnitTests\bin\%config%\UnitTests.dll
if not "%errorlevel%"=="0" goto failure

REM Package
mkdir Build
call %nuget% "Source\Devsense.PHP.Parser\Devsense.PHP.Parser.nuspec"
if not "%errorlevel%"=="0" goto failure

:success
exit 0

:failure
exit -1