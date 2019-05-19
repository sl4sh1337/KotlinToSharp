call init.cmd
for /l %%i in (1,1,15) do ( 
.\res\KotlinToSharp.exe .\testsuite\test%%i.kt > .\testsuite\test%%i.cs
echo -----------------------------------------------------
echo Expected out: 
more .\testsuite\test%%i.out
echo Out: 
.\testsuite\test%%i.kt.exe
echo.
echo ------------------------------------------------------
)
pause