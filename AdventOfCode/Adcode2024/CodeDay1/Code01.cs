using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using AdventOfCode.Util;

namespace AdventOfCode.Adcode2024.CodeDay1;

public class Code01
{
    static int Abs(int x)
    {
        if (x > 0)
        {
            return x;
        } 
        return -x;
    }
    public static void ProblemA()
    {
        string cur = CustomUtil.GetSourceDir();
        var test = Path.Combine(cur,"Input-test.txt");
        var input = Path.Combine(cur,"Input.txt");
        var file = System.IO.File.ReadLines(input);
        var ans = 0;
        // System.Collections
        var left = new PriorityQueue<int,int>();
        var right = new PriorityQueue<int,int>();
        foreach (var line in file)
        {
            var sline = line.Split("   ");
            left.Enqueue(int.Parse(sline[0]),int.Parse(sline[0]));
            right.Enqueue(int.Parse(sline[1]),int.Parse(sline[1]));
        }

        while (left.Count > 0 && right.Count > 0)
        {
            
            var x = left.Dequeue();
            var y = right.Dequeue();
            ans += Abs(x-y);
        }
        Console.WriteLine(ans);
    }

    public static void ProblemB()
    {
        string cur = CustomUtil.GetSourceDir();
        var test = Path.Combine(cur,"Input-test.txt");
        var input = Path.Combine(cur,"Input.txt");
        var file = File.ReadAllLines(input);
        var left = new Dictionary<int,int>();
        var right = new Dictionary<int,int>();
        var ans = 0;
        foreach (var line in file)
        {
            var sline = line.Split("  ");
            // Console.WriteLine(line);
            var l = int.Parse(sline[0]);
            var r = int.Parse(sline[1]);
            right[r] = right.GetValueOrDefault(r, 0) + 1;
            left[l] = left.GetValueOrDefault(l, 0) + 1;
            // Console.WriteLine($"{l},{r},{left[l]},{right[r]}");
        }

        // foreach (var l in left.Keys)
        // {
        //     Console.WriteLine($"{l},{left[l]}");
        // }
        foreach (var val in left.Keys)
        {
                ans += val * left.GetValueOrDefault(val, 0) * right.GetValueOrDefault(val, 0);
        }

        Console.WriteLine(ans);
    }
}