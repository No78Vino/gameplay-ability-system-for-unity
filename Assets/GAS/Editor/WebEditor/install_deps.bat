@echo off  
chcp 65001 >nul  
echo [EX-GAS Web Editor] 安装 Python 依赖...  
  
python --version >nul 2>&1  
if errorlevel 1 (  
    echo [ERROR] 未找到 Python 3.x，请先安装：https://www.python.org/downloads/  
    pause  
    exit /b 1  
)  
  
pip install openpyxl  
if errorlevel 1 (  
    echo [ERROR] openpyxl 安装失败，请检查网络或手动运行: pip install openpyxl  
    pause  
    exit /b 1  
)  
  
echo [OK] 所有依赖安装完成！  
pause