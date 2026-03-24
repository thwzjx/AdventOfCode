using System;
using System.Collections.Generic;
using System.IO;
using AdventOfCode.Util;

namespace AdventOfCode.Adcode2024.CodeDay10;

public class Day10
{
    private readonly record struct Position(int Row, int Column)
    {
        public int X { get; } = Row;
        public int Y { get; } = Column;
    }

    private static readonly List<Position> Dirs =
    [
        new Position(-1,0),
        new Position(0, 1),
        new Position(1, 0),
        new Position(0, -1)
    ];
    // private var dirs = new List<()> { };
    public static void ProgramA()
    {
        var cur = CustomUtil.GetSourceDir();
        var test = Path.Combine(cur, "Input-test.txt");
        var file = Path.Combine(cur, "Input.txt");
        var input = File.ReadAllLines(file);
        var m = input.Length;
        var n = input[0].Length;
        var nums = new int[m, n];
        var cache = new int[m, n];
        var visit = new bool[m, n];
        var zeros = new List<Position>();
        for (var i = 0; i < m; i++)
        {
            for (var j = 0; j < n; j++)
            {
                nums[i, j] = input[i][j]-'0';
                cache[i, j] = -1;
                if (nums[i, j] == 0)
                {
                    zeros.Add(new Position(i, j));
                }
            }
        }
        var ans = 0;
        foreach (var start in zeros)
        {
            var reachedNines = new HashSet<Position>();
            DfsPartA(nums, start.X, start.Y, reachedNines);
            ans += reachedNines.Count;
        }

        Console.WriteLine(ans);
        // var ans = 0;
        // foreach (var i in zeros)
        // {
        //     ans += Dfs(nums, i.X, i.Y,cache);
        // }
        //
        // Console.WriteLine(ans);
    }
    private static void DfsPartA(int[,] nums, int x, int y, HashSet<Position> reachedNines)
    {
        if (nums[x, y] == 9)
        {
            reachedNines.Add(new Position(x, y));
            return;
        }

        var cur = nums[x, y];
        foreach (var d in Dirs)
        {
            var nx = x + d.X;
            var ny = y + d.Y;

            if (nx < 0 || nx >= nums.GetLength(0) || ny < 0 || ny >= nums.GetLength(1))
            {
                continue;
            }

            if (nums[nx, ny] != cur + 1)
            {
                continue;
            }

            DfsPartA(nums, nx, ny, reachedNines);
        }
    }
    static int Dfs(int[,] nums, int x, int y, int[,] cache)
    {
        var cur = nums[x,y];
        var canBeNext = false;
        if (nums[x, y] == 9)
        {
            cache[x,y] = 1;
            return 1;
        }
        var res = 0;
        // visited[x,y] = true;
        foreach (var d in Dirs)
        {
            
            var nxt = new Position(x+d.X, y+d.Y);
            // 是否越界
            if (nxt.X < 0 || nxt.X >= nums.GetLength(0) || nxt.Y < 0 || nxt.Y >= nums.GetLength(1))
            {
                continue;
            }
            // 能否有后继
            if (nums[nxt.X, nxt.Y] - cur != 1)
            {
                continue;
            }
            canBeNext = true;
            if (cache[nxt.X, nxt.Y] != -1)
            {
                res += cache[nxt.X, nxt.Y];
            }
            else
            {
                res += Dfs(nums, nxt.X, nxt.Y, cache);
            }
        }

        if (!canBeNext)
        {
            return 0;
        }
        cache[x,y] = res;
        return res;
    }
    
    public static void ProgramB()
    {
        var cur = CustomUtil.GetSourceDir();
        var test = Path.Combine(cur, "Input-test.txt");
        var file = Path.Combine(cur, "Input.txt");
        var input = File.ReadAllLines(file);
        var m = input.Length;
        var n = input[0].Length;
        var nums = new int[m, n];
        var cache = new int[m, n];
        var visit = new bool[m, n];
        var zeros = new List<Position>();
        for (var i = 0; i < m; i++)
        {
            for (var j = 0; j < n; j++)
            {
                nums[i, j] = input[i][j]-'0';
                cache[i, j] = -1;
                if (nums[i, j] == 0)
                {
                    zeros.Add(new Position(i, j));
                }
            }
        }
        // var ans = 0;
        // foreach (var start in zeros)
        // {
        //     var reachedNines = new HashSet<Position>();
        //     DfsPartA(nums, start.X, start.Y, reachedNines);
        //     ans += reachedNines.Count;
        // }
        //
        // Console.WriteLine(ans);
        var ans = 0;
        foreach (var i in zeros)
        {
            ans += Dfs(nums, i.X, i.Y,cache);
        }
        
        Console.WriteLine(ans);
    }
}