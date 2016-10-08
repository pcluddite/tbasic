# NOTICE: This project has been reset to an earlier version
The latest actively developed version has been moved to a private repository due to licensing concerns. The source code available here is subject to the terms of the LGPL, but the newest and most updated code will only be availale for non-commercial use, which has yet to be accurately defined by the license.

As of 24 September 2016, the language has advanced considerably, making this repository nearly 150 commits behind the current branch. 3rd party code has been removed and the level of flexibility has increased dramatically. Large performance improvements have made it competative with other fully interpreted languages. The latest features being developed include object oriented programming, various language dialects (BASIC, Python, and C/C++), and the ability to translate standard library code into C++ for compilation and execution at native speeds. The tokenizer has been completely rewritten to accommodate this enhanced flexibility and without the use of regular expressions which incur huge performance costs.

Library improvements include the ability to add C# methods directly instead of having to write a custom wrapper. Operators can be added, removed, and replaced, and T4 text templates have helped considerably with the generation of consitant boilerplate code.

If you are a contributer to the project, it can be located at:
https://gitlab.com/pcluddite/tbasic

If you would like to be a contributer, please email me, Tim Baxendale, pcluddite@hotmail.com

--- END NOTICE ---

--- BEGIN OLD README ---
# TBASIC
A simple and extensible scripting language written in C#. The project is licensed under the LGPL, unless a specific source file is labled otherwise.

This project is split into two parts - a library (tbasic.dll) that can be added to any .NET application for a customizable BASIC language and an executer application (texecuter.exe) that uses that library, loads all standard symbols and runs a script.

***Special thanks to:***
- **Brent Railey**, Tech-Experts, Inc., Baton Rouge, LA, USA. whose expression evaluator was the basis of the TBASIC expression evaluator. (http://www.codeproject.com/Articles/9114/math-function-boolean-string-expression-evaluator)
- **drdandle** for registry operations located in RegistryUtilities.cs (http://www.codeproject.com/Articles/16343/Copy-and-Rename-Registry-Keys)
- **James Crowley** for ScreenCapture.cs (http://www.developerfusion.com/code/4630/capture-a-screen-shot/)
