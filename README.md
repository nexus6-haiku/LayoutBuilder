# LayoutBuilder
This is an implementation of the BLayoutBuilder classes for C#.
It aims to be an exact match of the LayoutBuilder.h header with a few exceptions and improvements:

* Non generic root classes (e.g. Group)
* Public properties over accessor methods (e.g. View and Parent)

All the builders instantiated in the hierarchy are stored in a List<> to keep the reference alive and prevent the Garbage Collector from wiping them out at any point in time.

Utils.cs contains a P/Invoke declaration for ui_color which is not exported by CppSharp in the
Haiku.dll assembly shipped with the [dotnet-haiku workflow](https://github.com/trungnt2910/dotnet-haiku).

## Usage

Copy LayoutBuilder.cs and Utils.cs in your project and add

`using Haiku.Interface.LayoutBuilder;`

In MainWindow.app there an example of how to use the class. Don't forget to assign the layout builder object to a class variable to keep the reference alive!

Please report any bug, inconsinstency with the original C++ API and feel free to submit patches to improve it.