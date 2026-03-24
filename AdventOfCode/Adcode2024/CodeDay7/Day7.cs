using System;
using System.IO;
using System.Linq;
using AdventOfCode.Util;
namespace AdventOfCode.Adcode2024.CodeDay7;

public class Day7
{
    static (long target, long[] numbers) ParseInput(string inputLine)
    {
        
        var info = inputLine.Split(":");
        var target = long.Parse(info[0]);
        var nums = info[1].Trim().Split(" ",StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();
        return (target, nums);
    }

    private static bool CanBeOp(long target, long[] numbers, int idx, bool canUseOtherOp)
    {
        if (idx == 1)
        {
            string combined = $"{numbers[0]}{numbers[1]}";
            bool ok = target.ToString() == combined;
            if (target == numbers[0] * numbers[1] || target == numbers[0] + numbers[1] || ok)
            {
                return true;
            }
            return false;
        }

        var res = false;
        if (target >= numbers[idx])
        {
            res = res || CanBeOp(target - numbers[idx], numbers, idx - 1,canUseOtherOp);
        }
        // var res = 

        if (target % numbers[idx] == 0)
        {
            res = res || CanBeOp(target / numbers[idx], numbers, idx - 1,canUseOtherOp);
        }

        if (canUseOtherOp)
        {
            string cur = numbers[idx].ToString();
            var targetStr = target.ToString();
            bool isEndOfCurNum = targetStr.EndsWith(cur) && targetStr.Length > cur.Length;
        
            if (isEndOfCurNum)
            {
                string prefix = targetStr.Substring(0, targetStr.Length - cur.Length);
                res = res || CanBeOp(long.Parse(prefix), numbers, idx - 1,canUseOtherOp);
            }
        }
        
        return res;
    }

    public static void ProgramA()
    {
        string cur = CustomUtil.GetSourceDir();
        var test = Path.Combine(cur,"Input-test.txt");
        var inputFile = Path.Combine(cur,"Input.txt");
        var input = File.ReadLines(inputFile);
        long res = 0;
        foreach (var line in input)
        {
            var (target, nums) = ParseInput(line);
            if (CanBeOp(target, nums, nums.Length - 1,false))
            {
                res += target;
            }
        }
        Console.WriteLine(res);
    }
    public static void ProgramB()
    {
        string cur = CustomUtil.GetSourceDir();
        // Console.WriteLine(cur);
        var test = Path.Combine(cur,"Input-test.txt");
        var inputFile = Path.Combine(cur,"Input.txt");
        var input = File.ReadLines(inputFile);
        long res = 0;
        foreach (var line in input)
        {
            var (target, nums) = ParseInput(line);
            if (CanBeOp(target, nums, nums.Length - 1,true))
            {
                res += target;
            }
        }
        Console.WriteLine(res);
    }
}