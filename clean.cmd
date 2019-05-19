@ECHO OFF
rem Очистка целевых директорий
@ECHO ON

del /Q /F res\*.exe
del /Q /F testsuite\*.exe
del /Q /F src\*.cs
del /Q /F testsuite\*.cs
del /Q /F src\*.tokens
del /Q /F src\*.interp

