using System.Text.RegularExpressions;

namespace AdventOfCode.Adcode2024.CodeDay3;
using System;
using System.Linq;
public class Day3
{
    public static void ProgramA()
    {
        const string test = "/Users/haowen/RiderProjects/AdventOfCode/AdventOfCode/Adcode2024/CodeDay3/Input-test.txt";
        const string input = "/Users/haowen/RiderProjects/AdventOfCode/AdventOfCode/Adcode2024/CodeDay3/Input.txt";
        var file = File.ReadAllLines(input);
        var ans = 0;
        foreach (var line in file)
        {
            var match = Regex.Matches(line, @"mul\((\d+)\,(\d+)\)");
            
            foreach (Match m in match)
            {
                var l = int.Parse(m.Groups[1].Value);
                var r = int.Parse(m.Groups[2].Value);
                ans += l * r;
            }
            
        }
        Console.WriteLine(ans);
        
    }

    public static void ProgramB()
    {
        const string test = "/Users/haowen/RiderProjects/AdventOfCode/AdventOfCode/Adcode2024/CodeDay3/Input-testB.txt";
        const string input = "/Users/haowen/RiderProjects/AdventOfCode/AdventOfCode/Adcode2024/CodeDay3/Input.txt";
        var file = File.ReadAllLines(input);
        var ans = 0;
        bool enabled = true;
        foreach (var line in file)
        {
            
            // 同时匹配三种合法指令
            var regex = new Regex(@"mul\((\d{1,3}),(\d{1,3})\)|do\(\)|don't\(\)");

            
            // long sum = 0;

            foreach (Match match in regex.Matches(line))
            {
                string token = match.Value;
                // Console.WriteLine(token);
                if (token == "do()")
                {
                    enabled = true;
                }
                else if (token == "don't()")
                {
                    enabled = false;
                }
                else if (enabled)
                {
                    int a = int.Parse(match.Groups[1].Value);
                    int b = int.Parse(match.Groups[2].Value);
                    ans += a * b;
                }
            }
        }

        Console.WriteLine(ans);
    }
}