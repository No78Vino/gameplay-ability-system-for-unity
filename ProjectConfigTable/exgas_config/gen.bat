set OUTPUT_DIR=%~1
set CONF_ROOT=.
set WORKSPACE=..
set LUBAN_DLL=%WORKSPACE%\Tools\Luban\Luban.dll

dotnet %LUBAN_DLL% ^
    -t all ^
    -d json ^
    --conf %CONF_ROOT%\luban.conf ^
    -x outputDataDir=%OUTPUT_DIR%

pause