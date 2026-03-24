using System.Text.RegularExpressions;
using AdventOfCode.Util;

namespace AdventOfCode.Adcode2024.CodeDay13;

public class Day13
{
    private struct Position
    {
        public long X { get; set; }
        public long Y { get; set; }
        // public int Cost {get; set; }
        public override string ToString()
        {
            return $"({X},{Y})";
        }
    }

    private struct Operation
    {
        public Position Position { get; set; }
        public int Cost { get; set; }
    }

    private struct TargetWithSolve
    {
        public Operation OperationA { get; set; }
        public Operation OperationB { get; set; }
        public Position Position { get; set; }

        public int MinCost()
        {
            var minCost = int.MaxValue;

            for (int a = 0; a <= 100; a++)
            {
                for (int b = 0; b <= 100; b++)
                {
                    var x = a * OperationA.Position.X + b * OperationB.Position.X;
                    var y = a * OperationA.Position.Y + b * OperationB.Position.Y;

                    if (x == Position.X && y == Position.Y)
                    {
                        minCost = Math.Min(minCost, a * OperationA.Cost + b * OperationB.Cost);
                    }
                }
            }

            return minCost;
        }
        public long MinCostPart2()
        {
            long ax = OperationA.Position.X;
            long ay = OperationA.Position.Y;
            long bx = OperationB.Position.X;
            long by = OperationB.Position.Y;
            long px = Position.X;
            long py = Position.Y;

            var d = ax * by - ay * bx;
            if (d == 0)
                return long.MaxValue;

            var aNumerator = px * by - py * bx;
            var bNumerator = ax * py - ay * px;

            if (aNumerator % d != 0 || bNumerator % d != 0)
                return long.MaxValue;

            var a = aNumerator / d;
            var b = bNumerator / d;

            if (a < 0 || b < 0)
                return long.MaxValue;

            return a * OperationA.Cost + b * OperationB.Cost;
        }
    }

    private static Operation ParseOperation(string operation)
    {
        var p = @"Button\s+([A-Z]):\s*X\+(\d+),\s*Y\+(\d+)";
        var match = Regex.Match(operation, p);
        if (!match.Success)
        {
            throw new Exception("Invalid operation");
        }
        return new Operation
        {
            Position = new Position
            {
                X = int.Parse(match.Groups[2].Value),
                Y = int.Parse(match.Groups[3].Value),
            },
            Cost = match.Groups[1].Value == "A" ? 3 : 1,
        };
    }

    private static Position ParsePosition(string position)
    {
        var p = @"Prize:\s*X\=(\d+),\s*Y\=(\d+)";
        var match = Regex.Match(position, p);
        if (!match.Success)
        {
            throw new Exception("Invalid position");
        }

        return new Position
        {
            X = int.Parse(match.Groups[1].Value),
            Y = int.Parse(match.Groups[2].Value),
        };
    }
    private static TargetWithSolve Parse(string[] lines)
    {
        return new TargetWithSolve
        {
            OperationA = ParseOperation(lines[0]),
            OperationB = ParseOperation(lines[1]),
            Position = ParsePosition(lines[2]),
        };
    }
    
    public static void ProgramA()
    {
        var cur = CustomUtil.GetSourceDir();
        var test = Path.Combine(cur, "Input-test.txt");
        var input = Path.Combine(cur, "Input.txt");
        var content = File.ReadAllText(input).Split("\n\n").Select(x=> Parse(x.Split("\n").ToArray())).ToArray();
        var ans = 0;
        foreach (var targetGroup in content)
        {
            if (targetGroup.MinCost() != int.MaxValue)
            {
                ans += targetGroup.MinCost();
            }
        }
        Console.WriteLine(ans);
    }
    public static void ProgramB()
    {
        var cur = CustomUtil.GetSourceDir();
        var test = Path.Combine(cur, "Input-test.txt");
        var input = Path.Combine(cur, "Input.txt");
        var content = File.ReadAllText(input).Split("\n\n").Select(x=> Parse(x.Split("\n").ToArray())).ToArray();
        long ans = 0;
        const long offset = 10000000000000L;

        foreach (var target in content)
        {
            var updated = target;
            updated.Position = new Position
            {
                X = target.Position.X + offset,
                Y = target.Position.Y + offset
            };

            var cost = updated.MinCostPart2();
            if (cost != long.MaxValue)
            {
                ans += cost;
            }
        }

        Console.WriteLine(ans);
    }
}