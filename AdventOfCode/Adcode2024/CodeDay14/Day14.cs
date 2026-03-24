using System.Text.RegularExpressions;
using AdventOfCode.Util;

namespace AdventOfCode.Adcode2024.CodeDay14;

public class Day14
{
    private struct Robot
    {
        public Robot(int row, int column, int vx, int vy, int maxR, int maxC)
        {
            Row = row;         // y
            Column = column;   // x
            MaxR = maxR;
            MaxC = maxC;

            Vx = ((vx % maxC) + maxC) % maxC; // x方向对列数取模
            Vy = ((vy % maxR) + maxR) % maxR; // y方向对行数取模
        }

        private int Row { get; set; }
        private int Column { get; set; }
        private int Vx { get; init; }
        private int Vy { get; init; }
        private int MaxR { get; init; }
        private int MaxC { get; init; }

        public int Quadrant()
        {
            if (Row < (MaxR - 1) / 2 && Column < (MaxC - 1) / 2) return 1;
            if (Row < (MaxR - 1) / 2 && Column > (MaxC - 1) / 2) return 2;
            if (Row > (MaxR - 1) / 2 && Column < (MaxC - 1) / 2) return 3;
            if (Row > (MaxR - 1) / 2 && Column > (MaxC - 1) / 2) return 4;
            return 0;
        }
        public (int row, int col) PositionAt(int t)
        {
            var row = ((Row + Vy * t) % MaxR + MaxR) % MaxR;
            var col = ((Column + Vx * t) % MaxC + MaxC) % MaxC;
            return (row, col);
        }
        public void Update()
        {
            Row += Vy;       // y方向
            Column += Vx;    // x方向
            Row = (Row + MaxR) % MaxR;
            Column = (Column + MaxC) % MaxC;
        }
    }
    private static double Variance(IEnumerable<int> nums)
    {
        var arr = nums.ToArray();
        var avg = arr.Average();
        double sum = 0;

        foreach (var x in arr)
        {
            var d = x - avg;
            sum += d * d;
        }

        return sum / arr.Length;
    }

    private static Robot ParseRobot(string line, int maxR, int maxC)
    {
        var match = Regex.Match(line.Trim(), @"p=(\-?\d+),(\-?\d+)\s+v=(\-?\d+),(\-?\d+)");
        if (!match.Success)
        {
            throw new Exception($"Invalid line: [{line}]");
        }

        return new Robot(
            row: int.Parse(match.Groups[2].Value),     // y
            column: int.Parse(match.Groups[1].Value),  // x
            vx: int.Parse(match.Groups[3].Value),      // x速度
            vy: int.Parse(match.Groups[4].Value),      // y速度
            maxR: maxR,
            maxC: maxC
        );
    }

    public static void ProgramA()
    {
        var cur = CustomUtil.GetSourceDir();
        var test = Path.Combine(cur, "Input-test.txt");
        var input = Path.Combine(cur, "Input.txt");
        var isTest = false;

        var maxR = 7;
        var maxC = 11;
        if (!isTest)
        {
            maxR = 103;
            maxC = 101;
        }

        var file = isTest ? test : input;

        var content = File.ReadAllLines(file)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => ParseRobot(x, maxR, maxC))
            .ToArray();

        var seconds = 100;
        for (var i = 0; i < seconds; i++)
        {
            for (var j = 0; j < content.Length; j++)
            {
                content[j].Update();
            }
        }

        var temp = new int[5];
        foreach (var item in content)
        {
            temp[item.Quadrant()]++;
        }

        var ans = temp[1] * temp[2] * temp[3] * temp[4];
        Console.WriteLine(ans);
    }
    public static void ProgramB()
    {
        var cur = CustomUtil.GetSourceDir();
        var input = Path.Combine(cur, "Input.txt");

        var maxR = 103;
        var maxC = 101;

        var robots = File.ReadAllLines(input)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => ParseRobot(x, maxR, maxC))
            .ToArray();

        var period = maxR * maxC; // 10403

        var bestT = 0;
        var bestScore = double.MaxValue;

        for (var t = 0; t < period; t++)
        {
            var positions = robots.Select(r => r.PositionAt(t)).ToArray();

            var rows = positions.Select(p => p.row);
            var cols = positions.Select(p => p.col);

            var score = Variance(rows) + Variance(cols);

            if (!(score < bestScore)) continue;
            bestScore = score;
            bestT = t;
        }

        Console.WriteLine($"Best time = {bestT}");
        // PrintMap(robots, bestT, maxR, maxC);
    }
}