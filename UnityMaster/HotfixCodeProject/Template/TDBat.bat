cd /D %~dp0
@echo off
set vs=C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\IDE\devenv.exe
set pro=E:\UnityMaster\HotfixCodeProject\

:begin
set /p csCourse= ‰»ÎøŒ≥ÃID:
for /r %pro%%csCourse%\ %%i in (*.csproj) do (
echo "%vs%"%%i
"%vs%" %%i /rebuild DEBUG /out output.txt
)
echo over
goto begin
pause