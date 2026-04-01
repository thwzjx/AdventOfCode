using AdventOfCode.Util;

namespace AdventOfCode.Adcode2024.CodeDay19;

public class Day19
{
    private static string[] PartParse(string input)
    {   
        return input.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(x=>x.Trim()).ToArray();
    }

    private static bool IsPossible(string[] part, string target,
        Dictionary<string,bool> memo)
    {
        if (memo.TryGetValue(target, out var cached))
            return cached;

        bool res = false;
        if (target.Length == 0)
        {
            res = true;                     // 基础情况：空串可拆分
        }
        else
        {
            foreach (var p in part)
            {
                if (!target.StartsWith(p)) continue;
                res = IsPossible(part, target.Substring(p.Length), memo);
                if (res) break;             // 找到一种拆分就可以结束
            }
        }

        memo[target] = res;                  // 缓存结果
        return res;
    }

    // private static bool IsPossible(string[] part, string target)
    // {
    //     var res = false;
    //     if (target.Length == 0)
    //     {
    //         return true;
    //     }
    //     foreach (var p in part)
    //     {
    //         if (!target.StartsWith(p)) continue;
    //         var newTarget = target.Substring(p.Length);
    //         res |= IsPossible(part, newTarget);
    //     }
    //     return res;
    // }
    public static void ProgramA()
    {
        var cur = CustomUtil.GetSourceDir();
        var test = Path.Combine(cur, "Input-test.txt");
        var input = Path.Combine(cur, "Input.txt");
        var content = File.ReadAllLines(input);
        var p = PartParse(content[0]);
        var memo = new Dictionary<string,bool>();
        var ans = 0;
        for (var i = 2; i < content.Length; i++)
        {
            var curS = content[i].Trim();
            if (IsPossible(p, curS,memo))
            {
                ans++;
            }
        }
        Console.WriteLine(ans);
    }
    /* ---------- 递归计数 + 缓存 ---------- */
    private static long CountWays(string[] patterns, string target,
        Dictionary<string,long> memo)
    {
        if (memo.TryGetValue(target, out var cached))
            return cached;

        long ways = 0;
        if (target.Length == 0)                 // 空串：一种拆分方式
            ways = 1;
        else
        {
            foreach (var p in patterns)
                if (target.StartsWith(p))
                    ways += CountWays(patterns,
                        target.Substring(p.Length),
                        memo);
        }

        memo[target] = ways;   // 缓存结果
        return ways;
    }

    /* ---------- 主程序 ---------- */
    public static void ProgramB()
    {
        var cur = CustomUtil.GetSourceDir();
        var inputFile = Path.Combine(cur, "Input.txt");
        var lines     = File.ReadAllLines(inputFile);

        // 第 1 行：所有可用图案
        var patterns = PartParse(lines[0]);

        // 之后的每一行（跳过第 2 行空白）都是一个设计
        long total = 0;
        var memo   = new Dictionary<string,long>();    // 全局缓存

        for (int i = 2; i < lines.Length; i++)
        {
            var design = lines[i].Trim();
            if (design == "") continue;   // 防止意外多余空行
            total += CountWays(patterns, design, memo);
        }

        Console.WriteLine(total);   // 这是你要找的答案
    }
}