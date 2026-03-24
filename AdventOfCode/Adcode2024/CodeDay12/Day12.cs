using AdventOfCode.Util;

namespace AdventOfCode.Adcode2024.CodeDay12;

public class Day12
{
    private struct Position
    {
        public int Row { get; set; }
        public int Column { get; set; }
    }

    private static readonly List<Position> Dirs =
    [
        new Position { Row = 0, Column = -1 },
        new Position { Row = 0, Column = 1 },
        new Position { Row = 1, Column = 0 },
        new Position { Row = -1, Column = 0 }
    ];
/// <summary>
/// 核心在于找到周长的计算
/// 这里采用的算法就是当我向另一个方向扩展的时候不符合条件的时候，就说明我的周长应该加一了。
/// <remarks>周长计算（参考的chatgpt的说明）</remarks>
/// </summary>
    public static void ProgramA()
    {
        var cur = CustomUtil.GetSourceDir();
        var test1 = Path.Combine(cur, "Input-test1.txt");
        var test2 = Path.Combine(cur, "Input-test2.txt");
        var test3 = Path.Combine(cur, "Input-test3.txt");
        var input = Path.Combine(cur, "Input.txt");
        var content =  File.ReadAllLines(input);
        var m = content.Length;
        var n = content[0].Length;
        bool[,] visited = new bool[m, n];
        var ans = 0;
        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (visited[i, j])
                {
                    continue;
                }
                // visited[i, j] = true;
                // var cell = new List<Position>();
                var c = content[i][j];
                var area = 0;
                var premier = 0;

                var nxt = new List<Position>{new Position{Row = i,Column = j}};
                while (nxt.Count > 0)
                {
                    var first = nxt[0];
                    // cell.Add(first);
                    nxt.RemoveAt(0);
                    area += 1;
                    visited[first.Row, first.Column] = true;
                    foreach (var dir in Dirs)
                    {
                        var after = new Position { Row = first.Row + dir.Row, Column = first.Column + dir.Column };
                        // 越界和不是一块的
                        if (after.Row < 0 || after.Column < 0 || after.Row >= m || after.Column >= n || content[after.Row][after.Column] != c)
                        {
                            premier++;
                            continue;
                        }
                        
                        // 已经访问过
                        if (visited[after.Row, after.Column])
                        {
                            continue;
                        }
                        visited[after.Row, after.Column] = true;
                        nxt.Add(after);
                    }
                }
                ans += area * premier;
            }
        }
        Console.WriteLine(ans);
    }
    public static void ProgramB()
    {
        var cur = CustomUtil.GetSourceDir();
        var test1 = Path.Combine(cur, "Input-test1.txt");
        var test2 = Path.Combine(cur, "Input-test2.txt");
        var test3 = Path.Combine(cur, "Input-test3.txt");
        var input = Path.Combine(cur, "Input.txt");
        var content =  File.ReadAllLines(input);
        var m = content.Length;
        var n = content[0].Length;
        bool[,] visited = new bool[m, n];
        var ans = 0;
        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (visited[i, j])
                {
                    continue;
                }
                // visited[i, j] = true;
                var cell = new List<Position>();
                var c = content[i][j];
                var area = 0;
                // var premier = 0;

                var nxt = new List<Position>{new Position{Row = i,Column = j}};
                while (nxt.Count > 0)
                {
                    var first = nxt[0];
                    cell.Add(first);
                    nxt.RemoveAt(0);
                    area += 1;
                    visited[first.Row, first.Column] = true;
                    foreach (var dir in Dirs)
                    {
                        var after = new Position { Row = first.Row + dir.Row, Column = first.Column + dir.Column };
                        // 越界和不是一块的
                        if (after.Row < 0 || after.Column < 0 || after.Row >= m || after.Column >= n || content[after.Row][after.Column] != c)
                        {
                            // premier++;
                            continue;
                        }
                        
                        // 已经访问过
                        if (visited[after.Row, after.Column])
                        {
                            continue;
                        }
                        visited[after.Row, after.Column] = true;
                        nxt.Add(after);
                    }
                }

                var sides = 0;
                foreach (var p in cell)
                {
                    bool up = IsSame(content, p.Row - 1, p.Column, c);
                    bool right = IsSame(content, p.Row, p.Column + 1, c);
                    bool down = IsSame(content, p.Row + 1, p.Column, c);
                    bool left = IsSame(content, p.Row, p.Column - 1, c);

                    // 四个对角
                    bool upLeft = IsSame(content, p.Row - 1, p.Column - 1, c);
                    bool upRight = IsSame(content, p.Row - 1, p.Column + 1, c);
                    bool downRight = IsSame(content, p.Row + 1, p.Column + 1, c);
                    bool downLeft = IsSame(content, p.Row + 1, p.Column - 1, c);
                    
                    // 左上角
                    if (!up && !left) sides++;          // 外角
                    else if (up && left && !upLeft) sides++; // 内角

                    // 右上角
                    if (!up && !right) sides++;
                    else if (up && right && !upRight) sides++;

                    // 右下角
                    if (!down && !right) sides++;
                    else if (down && right && !downRight) sides++;

                    // 左下角
                    if (!down && !left) sides++;
                    else if (down && left && !downLeft) sides++;
                }
                ans += area * sides;
            }
        }

        Console.WriteLine(ans);
    }
    private static bool IsSame(string[] grid, int x, int y, char target)
    {
        int m = grid.Length;
        int n = grid[0].Length;

        return x >= 0 && x < m && y >= 0 && y < n && grid[x][y] == target;
    }
}