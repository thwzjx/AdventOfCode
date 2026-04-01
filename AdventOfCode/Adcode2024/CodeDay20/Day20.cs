using AdventOfCode.Util;

namespace AdventOfCode.Adcode2024.CodeDay20;

public class Day20
{
    private readonly record struct Position(int Row, int Column)
    {
        public static Position operator +(Position a, Position b)
        {
            return new Position(a.Row + b.Row, a.Column + b.Column);
        }
    }

    private static readonly List<Position> Dirs =
    [
        new Position { Row = -1, Column = 0 },
        new Position { Row = 1, Column = 0 },
        new Position { Row = 0, Column = -1 },
        new Position { Row = 0, Column = 1 }
    ];

    private static bool IsOutOfBound(Position position, int row, int col)
    {
        return position.Row < 0 || position.Row >= row || position.Column < 0 || position.Column >= col;
    }

    private static int Bfs(string[] content, Position start, Position end, int maxRow, int maxCol,Dictionary<Position,int> memos)
    {
        var q = new Queue<(Position,int)>();
        q.Enqueue((start,0));
        memos[start] = 0;
        if (memos.TryGetValue(end, out var count))
        {
            return count;
        }
        // var level = 0;
        while (q.Count > 0)
        {
            var (pos, cost) = q.Dequeue();
            if (pos.Row == end.Row && pos.Column == end.Column)
            {
                return cost;
            }
            
            foreach (var dir in Dirs)
            {
                var nxt = pos + dir;
                // 越界
                if (IsOutOfBound(nxt, maxRow, maxCol))
                {
                    continue;
                }
                // 遇到墙了
                if (content[nxt.Row][nxt.Column] == '#')
                {
                    continue;
                }

                if (memos.ContainsKey(nxt)) continue; // 只有未访问过才处理
                memos[nxt] = cost + 1;
                q.Enqueue((nxt, cost + 1));
            }
        }

        return -1;
    }
    public static void ProgramA()
    {
        var cur = CustomUtil.GetSourceDir();
        var test = Path.Combine(cur, "Input-test.txt");
        var input = Path.Combine(cur, "Input.txt");
        var content = File.ReadAllLines(input);
        var m = content.Length;
        var n = content[0].Length;
        var start = new Position(0, 0);
        var end = new Position(n - 1, m - 1);
        var memos = new Dictionary<Position, int>();
        for (var i = 0; i < m; i++)
        {
            for (var j = 0; j < n; j++)
            {
                if (content[i][j] == 'S')
                {
                    start = new Position(i, j);
                }

                if (content[i][j] == 'E')
                {
                    end = new Position(i, j);
                }
            }
        }
        Bfs(content, end, start, m,n,memos);
        var dict = new Dictionary<Position, int>();
        for (var i = 0; i < m; i++)
        {
            for (var j = 0; j < n; j++)
            {
                if (content[i][j]!='#')
                {
                    var curP = new Position(i, j);
                    foreach (var dir in Dirs)
                    {
                        var np = curP + dir;
                        var nxtP = curP + dir + dir;
                        if (IsOutOfBound(nxtP, m, n))
                        {
                            continue;
                        }
                        if (content[np.Row][np.Column] == '#' && content[nxtP.Row][nxtP.Column] != '#')
                        {
                            var saveTime = Bfs(content, end, curP, m, n,memos) - (2+Bfs(content, end,nxtP, m, n,memos));
                            if (saveTime <= 0) continue;
                            if (!dict.TryAdd(np, saveTime))
                            {
                                dict[np] = Math.Min(dict[np], saveTime);
                            }
                        }
                    }
                }
            }
        }

        var finalRes = dict.Count(kvp => kvp.Value >= 100);
        Console.WriteLine(finalRes);
    }
    private static Dictionary<Position, int> BfsAll(string[] content, Position start, int maxRow, int maxCol)
    {
        var dist = new Dictionary<Position, int>();
        var q = new Queue<Position>();
        q.Enqueue(start);
        dist[start] = 0;

        while (q.Count > 0)
        {
            var pos = q.Dequeue();
            var cost = dist[pos];

            foreach (var dir in Dirs)
            {
                var nxt = pos + dir;
                if (IsOutOfBound(nxt, maxRow, maxCol)) continue;
                if (content[nxt.Row][nxt.Column] == '#') continue;
                if (dist.ContainsKey(nxt)) continue;

                dist[nxt] = cost + 1;
                q.Enqueue(nxt);
            }
        }

        return dist;
    }
    public static void ProgramB()
    {
        var cur = CustomUtil.GetSourceDir();
        var input = Path.Combine(cur, "Input.txt");
        if (!File.Exists(input)) return;

        var content = File.ReadAllLines(input);
        var m = content.Length;
        var n = content[0].Length;

        var start = new Position(0, 0);
        var end = new Position(0, 0);

        var tracks = new List<Position>();

        for (var i = 0; i < m; i++)
        {
            for (var j = 0; j < n; j++)
            {
                if (content[i][j] != '#')
                {
                    var p = new Position(i, j);
                    tracks.Add(p);

                    if (content[i][j] == 'S') start = p;
                    if (content[i][j] == 'E') end = p;
                }
            }
        }

        var distFromStart = BfsAll(content, start, m, n);
        var distToEnd = BfsAll(content, end, m, n);

        var normalDist = distFromStart[end];
        var ans = 0;

        foreach (var a in tracks)
        {
            if (!distFromStart.TryGetValue(a, out var da)) continue;

            for (var dr = -20; dr <= 20; dr++)
            {
                var remain = 20 - Math.Abs(dr);
                for (var dc = -remain; dc <= remain; dc++)
                {
                    var b = new Position(a.Row + dr, a.Column + dc);

                    if (IsOutOfBound(b, m, n)) continue;
                    if (content[b.Row][b.Column] == '#') continue;

                    if (!distToEnd.TryGetValue(b, out var db)) continue;

                    var cheatCost = Math.Abs(dr) + Math.Abs(dc);
                    var total = da + cheatCost + db;
                    var save = normalDist - total;

                    if (save >= 100)
                    {
                        ans++;
                    }
                }
            }
        }

        Console.WriteLine(ans);
    }

       
}