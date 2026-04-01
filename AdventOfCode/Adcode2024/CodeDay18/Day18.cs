using AdventOfCode.Util;

namespace AdventOfCode.Adcode2024.CodeDay18;

public class Day18
{
    private record struct Position
    {
        public int Row { get;init; }
        public int Column { get; init; }
        public static Position operator +(Position a, Position b) => new Position{Row = a.Row + b.Row,Column = a.Column + b.Column};
    }
    private static int SolvePartOne(string[] content, int cnt, int cordinatesX, int cordinatesY)
    {
        var dirs = new List<Position>
        {
            new Position{Row = -1, Column = 0},
            new Position{Row = 1, Column = 0},
            new Position{Row = 0, Column = -1},
            new Position{Row = 0, Column = 1},
        };
        var canVisit = new bool[cordinatesX, cordinatesY];
        for (var i = 0; i < cordinatesX; i++)
        {
            for (var j = 0; j < cordinatesY; j++)
            {
                canVisit[i, j] = true;
            }
        }
        
        for (var i = 0; i < cnt; i++)
        {
            var line = content[i].Split(",", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
            canVisit[line[0], line[1]] = false;
        }
        var level = new Queue<Position>();
        level.Enqueue(new Position { Row = 0, Column = 0 });
        canVisit[0, 0] = false;
        var levelCount = 0;
        while (level.Count > 0)
        {
            levelCount++;
            var lc = level.Count;
            for (var i = 0; i < lc; i++)
            {
                var pos = level.Dequeue();
                foreach (var dir in dirs)
                {
                    var nxtPos = pos + dir;
                    // 是否越界和能否访问
                    if (nxtPos.Row < 0 || nxtPos.Row >= cordinatesX || nxtPos.Column < 0 ||
                        nxtPos.Column >= cordinatesY || !canVisit[nxtPos.Row, nxtPos.Column])
                    {
                        continue;
                    }

                    if (nxtPos == new Position { Row = cordinatesX - 1, Column = cordinatesY - 1 })
                    {
                        return levelCount;
                    }
                    canVisit[nxtPos.Row, nxtPos.Column] = false;
                    level.Enqueue(nxtPos);
                }
            }
        }

        return -1;
    }
    public static void ProgramA()
    {
        var cur = CustomUtil.GetSourceDir();
        var test = Path.Combine(cur,"Input-test.txt");
        var input = Path.Combine(cur,"Input.txt");
        var content = File.ReadAllLines(input);
        var ans = SolvePartOne(content, 1024, 71, 71);
        Console.WriteLine(ans);
    }
    
    public static void ProgramB()
    {
        var cur = CustomUtil.GetSourceDir();
        var test = Path.Combine(cur,"Input-test.txt");
        var input = Path.Combine(cur,"Input.txt");
        var content = File.ReadAllLines(input);
        var left = 0;
        var right = content.Length;
        while (left < right)
        {
            var middle = (left + right) / 2;
            var ans =  SolvePartOne(content, middle, 71, 71);
            if (ans != -1)
            {
                left = middle+1;
            }
            else
            {
                right = middle;
            }
        }

        // Console.WriteLine(SolvePartOne(content, left-1,7,7));
        Console.WriteLine(left);
        Console.WriteLine(content[left-1]);
        // var ans = SolvePartOne(content, 1024, 71, 71);
        // Console.WriteLine(ans);
    }
}