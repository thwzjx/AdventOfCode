using System.Collections;

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
        string basicPath = AppContext.BaseDirectory;
        // Console.WriteLine($"current working dir is {basicPath}");
        var file = System.IO.File.ReadLines("~/RiderProjects/AdventOfCode/AdventOfCode/Adcode2024/CodeDay1/InputA.txt");
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
        var filePath = "~/RiderProjects/AdventOfCode/AdventOfCode/Adcode2024/CodeDay1/InputA.txt";
        var testPath = "~/RiderProjects/AdventOfCode/AdventOfCode/Adcode2024/CodeDay1/Input-test.txt";
        var file = File.ReadAllLines(filePath);
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