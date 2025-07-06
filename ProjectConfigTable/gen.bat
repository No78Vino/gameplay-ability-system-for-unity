set WORKSPACE=..
set LUBAN_DLL= .\Tools\Luban\Luban.dll
set CONF_ROOT=.
set PROJECT_WORKSPACE = ..

dotnet %LUBAN_DLL% ^
    -t all ^
    -d json ^
    --conf %CONF_ROOT%\luban.conf ^
    -x outputDataDir= %WORKSPACE%\Assets\DemoForESC\Resources\Tables

pause