@echo off  
chcp 65001 >nul  
setlocal  
  
set "DIR=%~dp0"  
  
echo [EX-GAS] 启动 GameplayEffect 网页编辑器...  
start "" python "%DIR%server.py" %*  
pause