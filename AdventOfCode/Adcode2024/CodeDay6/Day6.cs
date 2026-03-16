using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode.Adcode2024.CodeDay6;

public class Day6
{
    public static void ProgramA()
    {
        var dirs = new List<(int,int)>{(-1,0),(0,1),(1,0),(0,-1)};
        const string test = "/Users/haowen/RiderProjects/AdventOfCode/AdventOfCode/Adcode2024/CodeDay6/Input-test.txt";
        const string input_file = "/Users/haowen/RiderProjects/AdventOfCode/AdventOfCode/Adcode2024/CodeDay6/Input.txt";
        var input = File.ReadLines(input_file).ToArray();
        var m = input.Length;
        var n = input[0].Length;
        var path = new HashSet<(int, int)>();
        var nowPos = (0,0);
        var notAvail = new HashSet<(int, int)>();
        for (int i = 0; i < input.Length; i++)
        {
            for (int j = 0; j < input[i].Length; j++)
            {
                if (input[i][j] == '#')
                {
                    notAvail.Add((i, j));
                }
                if  (input[i][j] == '^')
                {
                    path.Add((i, j));
                    nowPos = (i, j);
                }
            }
        }
        var cur = 0;
        // Console.WriteLine(nowPos);
        while (true)
        {
            var curX = dirs[cur%4].Item1;
            var curY = dirs[cur%4].Item2;
            var newPos = (nowPos.Item1 + curX, nowPos.Item2 + curY);
            // 边界条件
            if (newPos.Item1 >= m || newPos.Item1 < 0 || newPos.Item2 >= n || newPos.Item2 < 0)
            {
                break;
            }
            // 改变方向
            if (notAvail.Contains(newPos))
            {
                cur += 1;
                continue;
            }
            // 更新位置
            path.Add(newPos);
            nowPos = newPos;
        }

        // foreach (var c in path)
        // {
        //     Console.WriteLine(c);
        // }
        Console.WriteLine(path.Count);
    }

    public static void ProgramB()
    {
        var dirs = new List<(int, int)>
        {
            (-1, 0), (0, 1), (1, 0), (0, -1)
        };

        const string input_file = "/Users/haowen/RiderProjects/AdventOfCode/AdventOfCode/Adcode2024/CodeDay6/Input.txt";
        var input = File.ReadLines(input_file).ToArray();

        int m = input.Length;
        int n = input[0].Length;

        var notAvail = new HashSet<(int, int)>();
        var path = new HashSet<(int, int)>();
        var start = (0, 0);

// 读图
        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (input[i][j] == '#')
                    notAvail.Add((i, j));
                else if (input[i][j] == '^')
                    start = (i, j);
            }
        }

// Part 1: 先跑出原始路径，作为候选点
        {
            var pos = start;
            int dir = 0;
            path.Add(pos);

            while (true)
            {
                var next = (pos.Item1 + dirs[dir].Item1, pos.Item2 + dirs[dir].Item2);

                if (next.Item1 < 0 || next.Item1 >= m || next.Item2 < 0 || next.Item2 >= n)
                    break;

                if (notAvail.Contains(next))
                {
                    dir = (dir + 1) % 4;
                    continue;
                }

                pos = next;
                path.Add(pos);
            }
        }

        bool HasLoop(HashSet<(int, int)> blocks, (int, int) s, int rows, int cols)
        {
            var seen = new HashSet<(int, int, int)>();
            var pos = s;
            int dir = 0;

            while (true)
            {
                var state = (pos.Item1, pos.Item2, dir);
                if (seen.Contains(state))
                    return true;

                seen.Add(state);

                var next = (pos.Item1 + dirs[dir].Item1, pos.Item2 + dirs[dir].Item2);

                if (next.Item1 < 0 || next.Item1 >= rows || next.Item2 < 0 || next.Item2 >= cols)
                    return false;

                if (blocks.Contains(next))
                {
                    dir = (dir + 1) % 4;
                }
                else
                {
                    pos = next;
                }
            }
        }

// Part 2
        int ans = 0;
        foreach (var p in path)
        {
            if (p == start) continue;
            if (notAvail.Contains(p)) continue;

            notAvail.Add(p);
            if (HasLoop(notAvail, start, m, n))
                ans++;
            notAvail.Remove(p);
        }

        Console.WriteLine(ans);
    }
}