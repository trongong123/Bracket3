@echo off

echo Cleaning bin, obj and .vs folders in current directory and subdirectories...
for /d /r %%d in (bin, obj, .vs) do @if exist "%%d" rd /s /q "%%d"

echo Cleaning temporary files in current directory and subdirectories...
del /s /q *.tmp

echo Cleaning log files in current directory and subdirectories...
del /s /q *.log

echo Cleaning .cs.bak files in current directory and subdirectories...
del /s /q *.cs.bak

echo Unblock all files
powershell -command "Get-ChildItem *.* -Recurse | Unblock-File"

echo Cleanup completed hehe!
pause
