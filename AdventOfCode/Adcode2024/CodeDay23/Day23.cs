using AdventOfCode.Util;

namespace AdventOfCode.Adcode2024.CodeDay23;

public class Day23
{
    public static void ProgramA()
    {
        var cur = CustomUtil.GetSourceDir();
        var test = Path.Combine(cur, "Input-test.txt");
        var input = Path.Combine(cur, "Input.txt");
        var content = File.ReadAllLines(input);
        var dict = new Dictionary<string, HashSet<string>>();
        var temp = new HashSet<string>();
        foreach (var line in content)
        {
            var parts = line.Split('-');
            var key = parts[0];
            var val = parts[1];
            if (!dict.ContainsKey(key))
            {
                dict[key] = new HashSet<string>();
            }
            dict[key].Add(val);
            if (!dict.ContainsKey(val))
            {
                dict[val] = [];
            }
            dict[val].Add(key);
        }

        foreach (var key in dict.Keys)
        {
            if (!key.StartsWith('t'))
            {
                continue;
            }

            foreach (var val in dict[key])
            {
                var tempset = dict[val].Intersect(dict[key]).ToHashSet();
                foreach (var set in tempset)
                {
                    var triangleNodes = new List<string> { key, val, set };
                    // 2. 排序，确保顺序一致
                    triangleNodes.Sort();
                    // 3. 拼接成唯一的字符串标识
                    var identifier = string.Join(",", triangleNodes);
                    // temp.Add([key, val, set]);
                    temp.Add(identifier);
                }
            }
        }

        Console.WriteLine(temp.Count);
    }
    public static void ProgramB()
{
    // --- 1. 初始化与读取 (完全仿照你的 Part A 风格) ---
    var cur = CustomUtil.GetSourceDir();
    var input = Path.Combine(cur, "Input.txt");
    var content = File.ReadAllLines(input);
    
    var dict = new Dictionary<string, HashSet<string>>();
    // 用于存储所有找到的“极大团”集合
    var allCliques = new List<HashSet<string>>();

    // --- 2. 构建邻接表 (完全仿照你的 Part A 逻辑) ---
    foreach (var line in content)
    {
        var parts = line.Split('-');
        if (parts.Length < 2) continue;
        var key = parts[0];
        var val = parts[1];

        if (!dict.ContainsKey(key)) dict[key] = [];
        if (!dict.ContainsKey(val)) dict[val] = [];

        dict[key].Add(val);
        dict[val].Add(key);
    }

    // --- 3. 定义递归算法 (使用局部函数，保持 ProgramB 的封装性) ---
    // Bron-Kerbosch 算法通过 R, P, X 三个集合寻找所有极大团
    void BronKerbosch(HashSet<string> R, HashSet<string> P, HashSet<string> X)
    {
        if (P.Count == 0 && X.Count == 0)
        {
            // 找到了一个无法再扩大的“极大团”
            allCliques.Add(new HashSet<string>(R));
            return;
        }

        // 为了防止在遍历过程中修改集合导致异常，先转为 List
        var candidates = P.ToList();

        foreach (var v in candidates)
        {
            // 下一步的 R: 当前团 + 节点 v
            var nextR = new HashSet<string>(R) { v };
            
            // 下一步的 P: P 中与 v 相连的节点 (交集)
            var nextP = new HashSet<string>(P.Intersect(dict[v]));
            
            // 下一步的 X: X 中与 v 相连的节点 (交集)
            var nextX = new HashSet<string>(X.Intersect(dict[v]));

            BronKerbosch(nextR, nextP, nextX);

            // 回溯过程：从候选集中移除 v，并将其放入已排除集合 X
            P.Remove(v);
            X.Add(v);
        }
    }

    // --- 4. 执行算法 ---
    var initialP = new HashSet<string>(dict.Keys);
    BronKerbosch([], initialP, []);

    // --- 5. 处理结果并输出密码 (符合 Part Two 的格式要求) ---
    if (allCliques.Count > 0)
    {
        // 找到成员数量最多的那个集合 (最大团)
        var maxClique = allCliques.OrderByDescending(c => c.Count).First();

        // 按照题目要求：字母排序 -> 用逗号连接
        var sortedNames = maxClique.OrderBy(name => name);
        var password = string.Join(",", sortedNames);

        Console.WriteLine(password);
    }
    else
    {
        Console.WriteLine("No cliques found.");
    }
}

}