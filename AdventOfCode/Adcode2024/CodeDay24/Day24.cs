using AdventOfCode.Util;

namespace AdventOfCode.Adcode2024.CodeDay24;

public class Day24
{
    private class GateTask()
    {
        public string Left { get; set; }
        public string Right { get; set; }
        public string Op { get; set; }
        public int Value { get; set; }

        public int Compute(int l, int r)
        {
            return Op switch
            {
                "AND" => l & r,
                "OR" => l | r,
                _ => l ^ r
            };
        }
    }

    public static void ProgramA()
    {
        var cur = CustomUtil.GetSourceDir();
        var test = Path.Combine(cur, "Input-test.txt");
        var test2 = Path.Combine(cur, "Input-test2.txt");
        var input = Path.Combine(cur, "Input.txt");
        var content = File.ReadAllText(input).Split("\n\n", StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Split("\n", StringSplitOptions.TrimEntries)).ToList();
        var part1 = content[0];
        var g = content[1];
        // var valMap = new Dictionary<string, int>();
        var valMap = new Dictionary<string, int>();
        var degreeMap = new Dictionary<string, int>();
        var nxtTask = new Dictionary<string, HashSet<string>>();
        var gt = new Dictionary<string, GateTask>();
        var zeroDegreeMap = new Queue<string>();
        foreach (var line in part1)
        {
            var s = line.Split(':', StringSplitOptions.TrimEntries);
            var key = s[0];
            var value = int.Parse(s[1]);
            valMap.Add(key, value);
            zeroDegreeMap.Enqueue(key);
        }

    

        foreach (var line in g)
        {
            var s = line.Split(' ', StringSplitOptions.TrimEntries);
            gt.Add(s[4], new GateTask { Left = s[0], Op = s[1], Right = s[2] });
            degreeMap.TryAdd(s[4], 2);
            nxtTask.TryAdd(s[0], []);
            nxtTask[s[0]].Add(s[4]);
            nxtTask.TryAdd(s[2], []);
            nxtTask[s[2]].Add(s[4]);
        }




        while (zeroDegreeMap.Count > 0)
        {
            var first = zeroDegreeMap.Dequeue();
            // zeroDegreeMap.Remove(first);
            if (!nxtTask.ContainsKey(first))
            {
                continue;
            }

            foreach (var nxt in nxtTask[first])
            {
                degreeMap[nxt]--;
                if (degreeMap[nxt] != 0) continue;
                // 添加新的
                zeroDegreeMap.Enqueue(nxt);
                // 计算值
                var nxtD = gt[nxt];
                valMap[nxt] = nxtD.Compute(valMap[nxtD.Left], valMap[nxtD.Right]);
            }
        }

        var temp = valMap.Where(x => x.Key.StartsWith('z'));
        long ans = 0;
        foreach (var val in temp)
        {
            var k = int.Parse(val.Key[1..]);
            var v = val.Value;
            ans |= (long)v << k;
        }
        Console.WriteLine(ans);
    }
    private class Gate
    {
        public string Left { get; set; } = "";
        public string Right { get; set; } = "";
        public string Op { get; set; } = "";
        public string Output { get; set; } = "";
    }

    public static void ProgramB()
    {
        var cur = CustomUtil.GetSourceDir();
        var input = Path.Combine(cur, "Input.txt");

        var text = File.ReadAllText(input).Replace("\r\n", "\n");
        var parts = text.Split("\n\n", StringSplitOptions.RemoveEmptyEntries);

        var gateLines = parts[1]
            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var gates = new List<Gate>();
        foreach (var line in gateLines)
        {
            var s = line.Split(' ', StringSplitOptions.TrimEntries);
            if (s.Length != 5 || s[3] != "->")
                throw new FormatException($"Invalid gate line: {line}");

            gates.Add(new Gate
            {
                Left = s[0],
                Op = s[1],
                Right = s[2],
                Output = s[4]
            });
        }

        var swapped = new HashSet<string>();

        // 找最高 z 位，例如 z45
        var maxZ = gates
            .Select(g => g.Output)
            .Where(x => x.StartsWith('z'))
            .Select(x => int.Parse(x[1..]))
            .Max();

        // 建索引
        Dictionary<string, string> byInputs;
        Dictionary<string, Gate> byOutput;
        RebuildIndexes(gates, out byInputs, out byOutput);

        // 第0位
        var x0 = Wire('x', 0);
        var y0 = Wire('y', 0);
        var z0 = Wire('z', 0);

        var sum0 = FindOutput(byInputs, x0, "XOR", y0);
        var carry = FindOutput(byInputs, x0, "AND", y0);

        if (sum0 != z0)
        {
            SwapOutputs(gates, sum0, z0);
            swapped.Add(sum0);
            swapped.Add(z0);
            RebuildIndexes(gates, out byInputs, out byOutput);

            sum0 = FindOutput(byInputs, x0, "XOR", y0);
            carry = FindOutput(byInputs, x0, "AND", y0);
        }

        // 从第1位开始逐位检查
        for (int i = 1; i < maxZ; i++)
        {
            while (true)
            {
                var xi = Wire('x', i);
                var yi = Wire('y', i);
                var zi = Wire('z', i);

                var xyXor = FindOutput(byInputs, xi, "XOR", yi);
                var xyAnd = FindOutput(byInputs, xi, "AND", yi);

                // 这一位的 sum
                var sum = TryFindOutput(byInputs, xyXor, "XOR", carry);
                if (sum == null)
                {
                    // 通常说明 xyXor 这个输出名被交换了
                    // 尝试从 z_i 对应的 gate 反推
                    var zGate = byOutput[zi];
                    var candidate = zGate.Left == carry || zGate.Right == carry
                        ? (zGate.Left == carry ? zGate.Right : zGate.Left)
                        : null;

                    if (candidate == null)
                        throw new Exception($"Cannot repair bit {i}, sum gate missing.");

                    SwapOutputs(gates, xyXor, candidate);
                    swapped.Add(xyXor);
                    swapped.Add(candidate);
                    RebuildIndexes(gates, out byInputs, out byOutput);
                    continue;
                }

                if (sum != zi)
                {
                    // sum 这根线和 z_i 被交换
                    SwapOutputs(gates, sum, zi);
                    swapped.Add(sum);
                    swapped.Add(zi);
                    RebuildIndexes(gates, out byInputs, out byOutput);
                    continue;
                }

                var carryMix = FindOutput(byInputs, xyXor, "AND", carry);
                var carryOut = FindOutput(byInputs, xyAnd, "OR", carryMix);

                carry = carryOut;
                break;
            }
        }

        // 最后一位 z_max 应该就是最终 carry
        // 这题通常不需要额外修最后一位，但保险起见可以检查
        var lastZ = Wire('z', maxZ);
        if (carry != lastZ)
        {
            SwapOutputs(gates, carry, lastZ);
            swapped.Add(carry);
            swapped.Add(lastZ);
            RebuildIndexes(gates, out byInputs, out byOutput);
        }

        var ans = swapped.OrderBy(x => x, StringComparer.Ordinal);
        Console.WriteLine(string.Join(",", ans));
    }

    private static string Wire(char prefix, int i) => $"{prefix}{i:00}";

    private static string MakeKey(string a, string op, string b)
    {
        if (string.CompareOrdinal(a, b) > 0)
            (a, b) = (b, a);
        return $"{a}|{op}|{b}";
    }

    private static void RebuildIndexes(
        List<Gate> gates,
        out Dictionary<string, string> byInputs,
        out Dictionary<string, Gate> byOutput)
    {
        byInputs = new Dictionary<string, string>();
        byOutput = new Dictionary<string, Gate>();

        foreach (var g in gates)
        {
            var key = MakeKey(g.Left, g.Op, g.Right);
            byInputs[key] = g.Output;
            byOutput[g.Output] = g;
        }
    }

    private static string FindOutput(Dictionary<string, string> byInputs, string a, string op, string b)
    {
        var key = MakeKey(a, op, b);
        if (!byInputs.TryGetValue(key, out var output))
            throw new Exception($"Gate not found: {a} {op} {b}");
        return output;
    }

    private static string? TryFindOutput(Dictionary<string, string> byInputs, string a, string op, string b)
    {
        var key = MakeKey(a, op, b);
        return byInputs.GetValueOrDefault(key);
    }

    private static void SwapOutputs(List<Gate> gates, string out1, string out2)
    {
        var g1 = gates.First(g => g.Output == out1);
        var g2 = gates.First(g => g.Output == out2);
        (g1.Output, g2.Output) = (g2.Output, g1.Output);
    }
}