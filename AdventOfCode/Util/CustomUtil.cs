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

    public static bool IsEnglishLetter(char c)
    {
        return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
    }

    public static void Swap(int[] arr, int i, int j)
    {
        (arr[i], arr[j]) = (arr[j], arr[i]);
    }
    
    public static void Now()
    {
        Console.WriteLine(GetSourceDir());
    }
}