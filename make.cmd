call init.cmd

del /Q /F res\*.exe
del /Q /F src\*.cs
del /Q /F src\*.tokens
del /Q /F src\*.interp

java -jar antlr-4.7.1-complete.jar Kotlin.g4 -Dlanguage=CSharp -no-listener -visitor -o src

cd src
csc /r:..\res\Antlr4.Runtime.Standard.dll /out:..\res\KotlinToSharp.exe ..\res\*.cs *.cs
cd ..

pause

