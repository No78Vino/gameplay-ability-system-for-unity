@echo off  
chcp 65001 >nul  
echo [EX-GAS Tag Editor] 启动中...  
  
:: 优先使用命令行参数传入路径  
set XLSX_PATH=%~1  
  
:: 如果没有传参，弹出输入提示  
if "%XLSX_PATH%"=="" (  
    set /p XLSX_PATH="请输入 #exgas.gameplayTags.xlsx 的完整路径: "  
)  
  
:: 检查 Python  
python --version >nul 2>&1  
if errorlevel 1 (  
    echo [ERROR] 未找到 Python，请先安装 Python 3.x  
    pause  
    exit /b 1  
)  
  
:: 检查并安装依赖  
pip show openpyxl >nul 2>&1  
if errorlevel 1 (  
    echo [INFO] 安装 openpyxl...  
    pip install openpyxl  
)  
  
:: 启动服务（路径动态传入 server.py）  
python "%~dp0server.py" --xlsx "%XLSX_PATH%" --port 8765  
pause