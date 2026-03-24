using System;
using System.IO;
using System.Runtime.CompilerServices;
using static System.AppContext;

namespace AdventOfCode.Util;

public class CustomUtil
{
    public static string GetSourceDir([CallerFilePath] string file = "")
    {
        return Path.GetDirectoryName(file)!;
    }

    public static void Now()
    {
        Console.WriteLine(GetSourceDir());
    }
}