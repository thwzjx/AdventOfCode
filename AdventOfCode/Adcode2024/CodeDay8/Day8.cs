using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AdventOfCode.Util;

namespace AdventOfCode.Adcode2024.CodeDay8;

public class Day8
{
    public static void ProgramA()
    {
        var sw = Stopwatch.StartNew();
        var cur = CustomUtil.GetSourceDir();
        var test = Path.Combine(cur,"Input-test.txt");
        var inputFile = Path.Combine(cur,"Input.txt");
        var input = File.ReadLines(inputFile).ToArray();
        var posi = new Dictionary<char, List<(int, int)>>();
        var r = input.Length;
        var c = input[0].Length;
        var antenna = new HashSet<(int, int)>();
        for (var i = 0; i < input.Length; i++)
        {
            for (var j = 0; j < c; j++)
            {
                if (input[i][j] == '.') continue;
                if (!posi.ContainsKey(input[i][j]))
                {
                    posi[input[i][j]] = [];
                }
                else
                {
                    foreach (var item in posi[input[i][j]])
                    {
                        var newX = i + i - item.Item1;
                        var newY = j + j - item.Item2;
                        if (newX >= 0 && newX < r && newY >= 0 && newY < c )
                        {
                            antenna.Add((newX, newY));
                        }
                        var newX2 = item.Item1 + item.Item1 - i;
                        var newY2 = item.Item2 + item.Item2 - j;
                        if (newX2 >= 0 && newX2 < r && newY2 >= 0 && newY2 < c )
                        {
                            antenna.Add((newX2, newY2));
                        }
                    }
                }
                posi[input[i][j]].Add((i, j));
            }
        }

        // Console.WriteLine($"{sw.Elapsed.Microseconds} ");
        Console.WriteLine(antenna.Count);
    }

    public static void ProgramB()
    {
        var cur = CustomUtil.GetSourceDir();
        var test = Path.Combine(cur, "Input-test.txt");
        var inputFile = Path.Combine(cur, "Input.txt");

        var input = File.ReadLines(inputFile).ToArray();
        var posi = new Dictionary<char, List<(int, int)>>();

        var r = input.Length;
        var c = input[0].Length;
        var antenna = new HashSet<(int, int)>();

        for (var i = 0; i < r; i++)
        {
            for (var j = 0; j < c; j++)
            {
                if (input[i][j] == '.') continue;

                var ch = input[i][j];
                if (!posi.ContainsKey(ch))
                {
                    posi[ch] = new List<(int, int)>();
                }

                posi[ch].Add((i, j));
            }
        }

        foreach (var kv in posi)
        {
            var list = kv.Value;
            if (list.Count < 2) continue;

            for (int i = 0; i < list.Count; i++)
            {
                for (int j = i + 1; j < list.Count; j++)
                {
                    var (x1, y1) = list[i];
                    var (x2, y2) = list[j];

                    var dx = x2 - x1;
                    var dy = y2 - y1;

                    // 从第一个点向反方向一直走
                    int nx = x1;
                    int ny = y1;
                    while (nx >= 0 && nx < r && ny >= 0 && ny < c)
                    {
                        antenna.Add((nx, ny));
                        nx -= dx;
                        ny -= dy;
                    }

                    // 从第二个点向正方向一直走
                    nx = x2;
                    ny = y2;
                    while (nx >= 0 && nx < r && ny >= 0 && ny < c)
                    {
                        antenna.Add((nx, ny));
                        nx += dx;
                        ny += dy;
                    }
                }
            }
        }

        Console.WriteLine(antenna.Count);
    }
}