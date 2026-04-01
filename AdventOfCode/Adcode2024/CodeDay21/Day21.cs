using AdventOfCode.Util;

namespace AdventOfCode.Adcode2024.CodeDay21;

public class Day21
{
    private readonly record struct Pos(int Row, int Col)
        {
            public static Pos operator +(Pos a, Pos b) => new(a.Row + b.Row, a.Col + b.Col);
        }

        private static readonly Pos Up = new(-1, 0);
        private static readonly Pos Down = new(1, 0);
        private static readonly Pos Left = new(0, -1);
        private static readonly Pos Right = new(0, 1);

        private static readonly (char ch, Pos dir)[] Moves =
        {
            ('^', Up),
            ('v', Down),
            ('<', Left),
            ('>', Right),
        };

        // 数字键盘
        private static readonly Dictionary<char, Pos> NumPad = new()
        {
            ['7'] = new Pos(0, 0),
            ['8'] = new Pos(0, 1),
            ['9'] = new Pos(0, 2),
            ['4'] = new Pos(1, 0),
            ['5'] = new Pos(1, 1),
            ['6'] = new Pos(1, 2),
            ['1'] = new Pos(2, 0),
            ['2'] = new Pos(2, 1),
            ['3'] = new Pos(2, 2),
            ['0'] = new Pos(3, 1),
            ['A'] = new Pos(3, 2),
        };

        // 方向键盘
        private static readonly Dictionary<char, Pos> DirPad = new()
        {
            ['^'] = new Pos(0, 1),
            ['A'] = new Pos(0, 2),
            ['<'] = new Pos(1, 0),
            ['v'] = new Pos(1, 1),
            ['>'] = new Pos(1, 2),
        };

        // pair -> 所有合法最短路径（末尾已包含 A）
        private static Dictionary<(char from, char to), List<string>> _numPaths = null!;
        private static Dictionary<(char from, char to), List<string>> _dirPaths = null!;

        // 记忆化：输入某串 code，在某层 depth / 某种键盘类型上的最短总代价
        private static readonly Dictionary<(string code, int depth, bool isNumeric), long> Memo = new();

        public static void ProgramA()
        {
            var cur = CustomUtil.GetSourceDir();
            var test = Path.Combine(cur, "Input-test.txt");
            var inputFile = Path.Combine(cur, "Input.txt");
            var input = File.ReadAllLines(inputFile);

            _numPaths = BuildAllShortestPaths(NumPad);
            _dirPaths = BuildAllShortestPaths(DirPad);

            long ans = 0;

            foreach (var code in input)
            {
                var pressLen = Solve(code, depth: 2, isNumeric: true);
                var numericValue = long.Parse(code[..^1]); // 去掉末尾 A
                ans += pressLen * numericValue;
            }

            Console.WriteLine(ans);
        }
        public static void ProgramB()
        {
            var cur = CustomUtil.GetSourceDir();
            var test = Path.Combine(cur, "Input-test.txt");
            var inputFile = Path.Combine(cur, "Input.txt");
            var input = File.ReadAllLines(inputFile);

            _numPaths = BuildAllShortestPaths(NumPad);
            _dirPaths = BuildAllShortestPaths(DirPad);

            long ans = 0;

            foreach (var code in input)
            {
                var pressLen = Solve(code, depth: 25, isNumeric: true);
                var numericValue = long.Parse(code[..^1]); // 去掉末尾 A
                ans += pressLen * numericValue;
            }

            Console.WriteLine(ans);
        }
        /// <summary>
        /// 求：在当前层键盘上输入 code 的最短总代价。
        /// isNumeric = true 表示当前层是数字键盘；false 表示当前层是方向键盘。
        /// depth 表示当前层之上还有多少层方向键盘需要继续展开。
        /// </summary>
        private static long Solve(string code, int depth, bool isNumeric)
        {
            var key = (code, depth, isNumeric);
            if (Memo.TryGetValue(key, out var cached))
                return cached;

            var pathsMap = isNumeric ? _numPaths : _dirPaths;

            long total = 0;
            var cur = 'A';

            foreach (var target in code)
            {
                var candidates = pathsMap[(cur, target)];
                var best = long.MaxValue;
                // 每一小节路径的最短选择
                foreach (var path in candidates)
                {
                    long cost;
                    if (depth == 0)
                    {
                        // 最外层就是你自己按，path 有多长就按多少次
                        cost = path.Length;
                    }
                    else
                    {
                        // 还要让上一层方向键盘去“输入 path”
                        cost = Solve(path, depth - 1, isNumeric: false);
                    }

                    if (cost < best) best = cost;
                }

                total += best;
                cur = target;
            }

            Memo[key] = total;
            return total;
        }

        /// <summary>
        /// 对一个键盘，预处理任意 from -> to 的所有合法最短路径。
        /// 返回的字符串末尾都加了 A，表示移动到目标后按下目标键。
        /// </summary>
        private static Dictionary<(char from, char to), List<string>> BuildAllShortestPaths(Dictionary<char, Pos> pad)
        {
            var validPositions = pad.Values.ToHashSet();
            var posToChar = pad.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

            var result = new Dictionary<(char from, char to), List<string>>();

            foreach (var from in pad.Keys)
            {
                foreach (var to in pad.Keys)
                {
                    var fromPos = pad[from];
                    var toPos = pad[to];

                    var paths = GetAllShortestMoveStrings(fromPos, toPos, validPositions);

                    // 最后加 A，表示按下目标按钮
                    var finalPaths = paths.Select(p => p + "A").ToList();
                    result[(from, to)] = finalPaths;
                }
            }

            return result;
        }

        /// <summary>
        /// 求从 start 到 end 的所有合法最短移动串（只含 ^v<>，不含 A）。
        /// 键盘上不能走到空洞位置。
        /// </summary>
        private static List<string> GetAllShortestMoveStrings(Pos start, Pos end, HashSet<Pos> validPositions)
        {
            if (start == end) return [""];

            // 先 BFS 求最短距离
            var dist = new Dictionary<Pos, int>();
            var q = new Queue<Pos>();
            dist[start] = 0;
            q.Enqueue(start);

            while (q.Count > 0)
            {
                var cur = q.Dequeue();
                int d = dist[cur];

                foreach (var (_, dir) in Moves)
                {
                    var nxt = cur + dir;
                    if (!validPositions.Contains(nxt)) continue;
                    if (dist.ContainsKey(nxt)) continue;
                    dist[nxt] = d + 1;
                    q.Enqueue(nxt);
                }
            }

            var best = dist[end];
            var res = new List<string>();

            void Dfs(Pos cur, List<char> path)
            {
                if (cur == end)
                {
                    res.Add(new string(path.ToArray()));
                    return;
                }

                var curDist = dist[cur];

                foreach (var (ch, dir) in Moves)
                {
                    var nxt = cur + dir;
                    if (!validPositions.Contains(nxt)) continue;
                    if (!dist.TryGetValue(nxt, out int nxtDist)) continue;

                    // 只沿着最短路前进
                    if (nxtDist != curDist + 1) continue;
                    // 并且不能超过目标最短距离
                    if (nxtDist > best) continue;

                    path.Add(ch);
                    Dfs(nxt, path);
                    path.RemoveAt(path.Count - 1);
                }
            }

            Dfs(start, []);
            return res;
        }
    
}