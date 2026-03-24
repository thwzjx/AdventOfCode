using AdventOfCode.Util;

namespace AdventOfCode.Adcode2024.CodeDay15;

public class Day15
{
    private readonly record struct Dir(int Row, int Col)
    {
        
        public static Dir operator +(Dir a, Dir b) => new(a.Row + b.Row, a.Col + b.Col);
    }

    private readonly record struct Box(int Row, int Col); // Col 是 '[' 的位置

    private static readonly Dir Up = new(-1, 0);
    private static readonly Dir Down = new(1, 0);
    private static readonly Dir Left = new(0, -1);
    private static readonly Dir Right = new(0, 1);

    private static Dir ParseOp(char op)
    {
        return op switch
        {
            '^' => Up,
            'v' => Down,
            '<' => Left,
            '>' => Right,
            _ => new Dir(0, 0)
        };
    }

    public static void ProgramA()
    {
        var cur = CustomUtil.GetSourceDir();
        var input = Path.Combine(cur, "Input.txt");
        var text = File.ReadAllText(input).Replace("\r\n", "\n");
        var parts = text.Split("\n\n", StringSplitOptions.RemoveEmptyEntries);

        var rawMap = parts[0].Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var ops = parts[1].Replace("\n", "").Trim().Select(ParseOp).ToArray();

        var (map, robot) = ParseMapPart1(rawMap);
        var currentRobot = robot;

        foreach (var op in ops)
        {
            MovePart1(map, ref currentRobot, op);
        }

        long ans = ScorePart1(map);
        Console.WriteLine(ans);
    }

    public static void ProgramB()
    {
        var cur = CustomUtil.GetSourceDir();
        var input = Path.Combine(cur, "Input.txt");
        var text = File.ReadAllText(input).Replace("\r\n", "\n");
        var parts = text.Split("\n\n", StringSplitOptions.RemoveEmptyEntries);

        var rawMap = parts[0].Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var ops = parts[1].Replace("\n", "").Trim().Select(ParseOp).ToArray();

        var expanded = ExpandMap(rawMap);
        var (map, robot) = ParseMapPart2(expanded);
        var currentRobot = robot;

        foreach (var op in ops)
        {
            MovePart2(map, ref currentRobot, op);
        }

        long ans = ScorePart2(map);
        Console.WriteLine(ans);
    }

    private static (char[,] map, Dir robot) ParseMapPart1(string[] rawMap)
    {
        var m = rawMap.Length;
        var n = rawMap[0].Length;
        var map = new char[m, n];
        var robot = new Dir(0, 0);

        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                var ch = rawMap[i][j];
                if (ch == '@')
                {
                    robot = new Dir(i, j);
                    map[i, j] = '.';
                }
                else
                {
                    map[i, j] = ch;
                }
            }
        }

        return (map, robot);
    }

    private static (char[,] map, Dir robot) ParseMapPart2(string[] rawMap)
    {
        var m = rawMap.Length;
        var n = rawMap[0].Length;
        var map = new char[m, n];
        var robot = new Dir(0, 0);

        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                var ch = rawMap[i][j];
                if (ch == '@')
                {
                    robot = new Dir(i, j);
                    map[i, j] = '.';
                }
                else
                {
                    map[i, j] = ch;
                }
            }
        }

        return (map, robot);
    }

    private static string[] ExpandMap(string[] rawMap)
    {
        return rawMap
            .Select(line => string.Concat(line.SelectMany(ch => ch switch
            {
                '#' => "##",
                'O' => "[]",
                '.' => "..",
                '@' => "@.",
                _ => throw new Exception($"Invalid char: {ch}")
            })))
            .ToArray();
    }

    private static void MovePart1(char[,] map, ref Dir robot, Dir op)
    {
        var first = robot + op;
        var next = first;

        while (map[next.Row, next.Col] == 'O')
        {
            next += op;
        }

        if (map[next.Row, next.Col] == '#')
        {
            return;
        }

        if (first != next)
        {
            map[next.Row, next.Col] = 'O';
            map[first.Row, first.Col] = '.';
        }

        robot = first;
    }

    private static void MovePart2(char[,] map, ref Dir robot, Dir op)
    {
        var front = robot + op;
        var cell = map[front.Row, front.Col];

        if (cell == '#')
        {
            return;
        }

        if (cell == '.')
        {
            robot = front;
            return;
        }

        if (op == Left || op == Right)
        {
            TryMoveHorizontal(map, ref robot, op);
        }
        else
        {
            TryMoveVertical(map, ref robot, op);
        }
    }

    private static void TryMoveHorizontal(char[,] map, ref Dir robot, Dir op)
    {
        var front = robot + op;
        var cur = front;

        while (map[cur.Row, cur.Col] == '[' || map[cur.Row, cur.Col] == ']')
        {
            cur += op;
        }

        if (map[cur.Row, cur.Col] == '#')
        {
            return;
        }

        // cur 现在是空地，把 [front..cur) 整体平移一格
        if (op == Right)
        {
            for (var c = cur.Col; c > front.Col; c--)
            {
                map[cur.Row, c] = map[cur.Row, c - 1];
            }
        }
        else
        {
            for (var c = cur.Col; c < front.Col; c++)
            {
                map[cur.Row, c] = map[cur.Row, c + 1];
            }
        }

        map[front.Row, front.Col] = '.';
        robot = front;
    }

    private static void TryMoveVertical(char[,] map, ref Dir robot, Dir op)
    {
        var front = robot + op;
        if (map[front.Row, front.Col] != '[' && map[front.Row, front.Col] != ']')
        {
            return;
        }

        var startBox = GetBoxAt(map, front.Row, front.Col);
        var queue = new Queue<Box>();
        var boxes = new HashSet<Box>();

        queue.Enqueue(startBox);
        boxes.Add(startBox);

        while (queue.Count > 0)
        {
            var box = queue.Dequeue();

            var targetLeft = new Dir(box.Row + op.Row, box.Col + op.Col);
            var targetRight = new Dir(box.Row + op.Row, box.Col + 1 + op.Col);

            var targets = new[] { targetLeft, targetRight };

            foreach (var t in targets)
            {
                var ch = map[t.Row, t.Col];
                if (ch == '#')
                {
                    return;
                }

                if (ch == '[' || ch == ']')
                {
                    var nextBox = GetBoxAt(map, t.Row, t.Col);
                    if (!boxes.Contains(nextBox))
                    {
                        boxes.Add(nextBox);
                        queue.Enqueue(nextBox);
                    }
                }
            }
        }

        // 先清空
        foreach (var box in boxes)
        {
            map[box.Row, box.Col] = '.';
            map[box.Row, box.Col + 1] = '.';
        }

        // 再写入新位置
        foreach (var box in boxes)
        {
            var newRow = box.Row + op.Row;
            var newCol = box.Col + op.Col;
            map[newRow, newCol] = '[';
            map[newRow, newCol + 1] = ']';
        }

        robot = front;
    }

    private static Box GetBoxAt(char[,] map, int row, int col)
    {
        return map[row, col] switch
        {
            '[' => new Box(row, col),
            ']' => new Box(row, col - 1),
            _ => throw new Exception($"No box at ({row},{col})")
        };
    }

    private static long ScorePart1(char[,] map)
    {
        long ans = 0;
        var m = map.GetLength(0);
        var n = map.GetLength(1);

        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (map[i, j] == 'O')
                {
                    ans += 100L * i + j;
                }
            }
        }

        return ans;
    }

    private static long ScorePart2(char[,] map)
    {
        long ans = 0;
        var m = map.GetLength(0);
        var n = map.GetLength(1);

        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (map[i, j] == '[')
                {
                    ans += 100L * i + j;
                }
            }
        }

        return ans;
    }

    // 调试用
    private static void PrintMap(char[,] map, Dir robot)
    {
        var m = map.GetLength(0);
        var n = map.GetLength(1);

        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (robot.Row == i && robot.Col == j)
                {
                    Console.Write('@');
                }
                else
                {
                    Console.Write(map[i, j]);
                }
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }
}