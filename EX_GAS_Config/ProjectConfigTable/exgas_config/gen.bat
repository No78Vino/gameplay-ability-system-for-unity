set OUTPUT_JSON_DIR=%~1
set OUTPUT_CODE_DIR=%~2
set CONF_ROOT=.
set WORKSPACE=..
set LUBAN_DLL=%WORKSPACE%\Tools\Luban\Luban.dll

dotnet %LUBAN_DLL% ^
    -t client ^
    -c cs-simple-json ^
    -d json ^
    --conf %CONF_ROOT%\luban.conf ^
    -x outputCodeDir=%OUTPUT_CODE_DIR% ^
    -x outputDataDir=%OUTPUT_JSON_DIR%
    
pause