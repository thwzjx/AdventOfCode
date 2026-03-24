using AdventOfCode.Util;

namespace AdventOfCode.Adcode2024.CodeDay16;

public class Day16
{
    private readonly record struct Dir(int Row, int Col)
    {
        public static readonly Dir Up = new(-1, 0);
        public static readonly Dir Right = new(0, 1);
        public static readonly Dir Down = new(1, 0);
        public static readonly Dir Left = new(0, -1);

        public static Dir operator +(Dir a, Dir b) => new(a.Row + b.Row, a.Col + b.Col);
    }

    private readonly record struct State(int Row, int Col, int DirIndex);

    // 0: Up, 1: Right, 2: Down, 3: Left
    private static readonly Dir[] Dirs =
    [
        Dir.Up, Dir.Right, Dir.Down, Dir.Left
    ];

    private static int RotateLeft(int dirIndex) => (dirIndex + 3) % 4;
    private static int RotateRight(int dirIndex) => (dirIndex + 1) % 4;

    private static bool InBounds(string[] grid, int row, int col)
    {
        return row >= 0 && row < grid.Length && col >= 0 && col < grid[0].Length;
    }

    private static (string[] grid, Dir start, Dir end) Parse(string path)
    {
        var grid = File.ReadAllLines(path);
        var start = new Dir();
        var end = new Dir();

        for (int i = 0; i < grid.Length; i++)
        {
            for (int j = 0; j < grid[0].Length; j++)
            {
                if (grid[i][j] == 'S')
                {
                    start = new Dir(i, j);
                }
                else if (grid[i][j] == 'E')
                {
                    end = new Dir(i, j);
                }
            }
        }

        return (grid, start, end);
    }

    private static int[,,] Dijkstra(string[] grid, Dir start)
    {
        var m = grid.Length;
        var n = grid[0].Length;

        var dist = new int[m, n, 4];
        for (var i = 0; i < m; i++)
        {
            for (var j = 0; j < n; j++)
            {
                for (var d = 0; d < 4; d++)
                {
                    dist[i, j, d] = int.MaxValue;
                }
            }
        }

        var pq = new PriorityQueue<State, int>();

        // 起始朝向向右
        const int startDir = 1;
        dist[start.Row, start.Col, startDir] = 0;
        pq.Enqueue(new State(start.Row, start.Col, startDir), 0);

        while (pq.Count > 0)
        {
            pq.TryDequeue(out var cur, out var curCost);

            if (curCost != dist[cur.Row, cur.Col, cur.DirIndex])
            {
                continue;
            }

            // 1. 直走
            Relax(grid, dist, pq, cur, cur.DirIndex, 1, curCost);

            // 2. 左转再走
            var leftDir = RotateLeft(cur.DirIndex);
            Relax(grid, dist, pq, cur, leftDir, 1001, curCost);

            // 3. 右转再走
            var rightDir = RotateRight(cur.DirIndex);
            Relax(grid, dist, pq, cur, rightDir, 1001, curCost);
        }
        return dist;
    }

    private static void Relax(
        string[] grid,
        int[,,] dist,
        PriorityQueue<State, int> pq,
        State cur,
        int nextDirIndex,
        int edgeCost,
        int curCost)
    {
        int nr = cur.Row + Dirs[nextDirIndex].Row;
        int nc = cur.Col + Dirs[nextDirIndex].Col;

        if (!InBounds(grid, nr, nc))
        {
            return;
        }

        if (grid[nr][nc] == '#')
        {
            return;
        }

        var nextCost = curCost + edgeCost;
        if (nextCost >= dist[nr, nc, nextDirIndex]) return;
        dist[nr, nc, nextDirIndex] = nextCost;
        pq.Enqueue(new State(nr, nc, nextDirIndex), nextCost);
    }

    public static void ProgramA()
    {
        var cur = CustomUtil.GetSourceDir();
        var input = Path.Combine(cur, "Input.txt");

        var (grid, start, end) = Parse(input);
        var dist = Dijkstra(grid, start);

        var ans = int.MaxValue;
        for (int d = 0; d < 4; d++)
        {
            ans = Math.Min(ans, dist[end.Row, end.Col, d]);
        }

        Console.WriteLine(ans);
    }

    public static void ProgramB()
    {
        var cur = CustomUtil.GetSourceDir();
        var input = Path.Combine(cur, "Input.txt");

        var (grid, start, end) = Parse(input);
        var dist = Dijkstra(grid, start);

        int best = int.MaxValue;
        for (int d = 0; d < 4; d++)
        {
            best = Math.Min(best, dist[end.Row, end.Col, d]);
        }

        // 从所有“以最优代价到达终点”的状态反向回溯
        var queue = new Queue<State>();
        var seenState = new HashSet<State>();

        for (var d = 0; d < 4; d++)
        {
            if (dist[end.Row, end.Col, d] != best) continue;
            var s = new State(end.Row, end.Col, d);
            queue.Enqueue(s);
            seenState.Add(s);
        }

        var bestTiles = new HashSet<(int Row, int Col)>();

        while (queue.Count > 0)
        {
            var curState = queue.Dequeue();
            bestTiles.Add((curState.Row, curState.Col));

            var r = curState.Row;
            var c = curState.Col;
            var dir = curState.DirIndex;
            var curCost = dist[r, c, dir];

            // 当前状态一定是从某个前驱状态“走一步”过来的
            // 前驱位置 = 当前位置 - 当前朝向
            var pr = r - Dirs[dir].Row;
            var pc = c - Dirs[dir].Col;

            if (!InBounds(grid, pr, pc) || grid[pr][pc] == '#')
            {
                continue;
            }

            // 三种可能的前驱方向：
            // 1) 前驱方向 == dir     -> 直走 +1
            // 2) 前驱方向右转后变 dir -> 前驱方向 = RotateRight(dir)? 不对，要反推
            //    若 prev 左转后到 dir，则 prev = RotateRight(dir)
            // 3) 若 prev 右转后到 dir，则 prev = RotateLeft(dir)

            // A. 直走来的
            TryBacktrack(dist, queue, seenState, pr, pc, dir, curCost - 1);

            // B. 前驱左转再走到当前方向
            var prevFromLeftTurn = RotateRight(dir);
            TryBacktrack(dist, queue, seenState, pr, pc, prevFromLeftTurn, curCost - 1001);

            // C. 前驱右转再走到当前方向
            var prevFromRightTurn = RotateLeft(dir);
            TryBacktrack(dist, queue, seenState, pr, pc, prevFromRightTurn, curCost - 1001);
        }

        Console.WriteLine(bestTiles.Count);
    }

    private static void TryBacktrack(
        int[,,] dist,
        Queue<State> queue,
        HashSet<State> seenState,
        int row,
        int col,
        int dir,
        int expectedCost)
    {
        if (expectedCost < 0)
        {
            return;
        }

        if (dist[row, col, dir] != expectedCost)
        {
            return;
        }

        var prev = new State(row, col, dir);
        if (seenState.Add(prev))
        {
            queue.Enqueue(prev);
        }
    }
}