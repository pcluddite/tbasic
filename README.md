# TBASIC 2.0
A simple and extensible scripting language written in C#. The project is licensed under the LGPL, unless a specific source file is labled otherwise.

This project is split into two parts - a library (tbasic.dll) that can be added to any .NET application for a customizable BASIC language and an executer application (texecuter.exe) that uses that library, loads all standard symbols and runs a script.

***Special thanks to:***
- **railerb** whose expression evaluator is the root of the tbasic.dll runtime parser. (http://www.codeproject.com/Articles/9114/math-function-boolean-string-expression-evaluator)
- **drdandle** for registry operations located in RegistryUtilities.cs (http://www.codeproject.com/Articles/16343/Copy-and-Rename-Registry-Keys)
