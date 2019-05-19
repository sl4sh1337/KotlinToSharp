@ECHO OFF
rem Генерация кода транслятора по грамматике
@ECHO ON

java -jar antlr-4.7.1-complete.jar Kotlin.g4 -Dlanguage=CSharp -no-listener -visitor -o src

pause