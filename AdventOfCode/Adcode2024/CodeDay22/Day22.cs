using System.Diagnostics;
using AdventOfCode.Util;
using static System.Int64;

namespace AdventOfCode.Adcode2024.CodeDay22;

public class Day22
{
    private const int Mod = 16777216;

    private static long SecretNumber(long input)
    {
        var temp1 = (((input << 6) ^ input)) % Mod;
        var temp2 = ((temp1 >> 5) ^ temp1) % Mod;
        var temp3 = ((temp2 << 11) ^ temp2) % Mod;
        return temp3;
    }

    public static void ProgramA()
    {
        var cur = CustomUtil.GetSourceDir();
        var test = Path.Combine(cur, "Input-test.txt");
        var input = Path.Combine(cur, "Input.txt");
        var content = File.ReadAllLines(input);
        long ans = 0;
        foreach (var line in content)
        {
            var num = Parse(line);
            for (var i = 0; i < 2000; i++)
            {
                num = SecretNumber(num);
            }

            ans += num;
        }

        Console.WriteLine(ans);
    }

    public static void ProgramB()
    {
        var cur = CustomUtil.GetSourceDir();
        var input = Path.Combine(cur, "Input.txt");
        if (!File.Exists(input)) return;

        var content = File.ReadAllLines(input);
        int numBuyers = content.Length;
        int maxPrices = 2001; // 包含初始值和后续2000个

        // 使用 int 节省内存，价格和差值都很小
        var prices = new int[numBuyers, maxPrices];

        for (var i = 0; i < numBuyers; i++)
        {
            var num = Parse(content[i]);
            for (var j = 0; j < maxPrices; j++)
            {
                prices[i, j] = (int)(num % 10);
                num = SecretNumber(num); // 假设这是你的生成函数
            }
        }

        // Key: 4个连续差值的元组, Value: 该模式能带来的香蕉总数
        var scoreMap = new Dictionary<(int, int, int, int), long>();

        for (var i = 0; i < numBuyers; i++)
        {
            // 记录当前买家已经“见过”的模式，确保每个买家只贡献一次价格
            var seenInThisBuyer = new HashSet<(int, int, int, int)>();

            for (var j = 4; j < maxPrices; j++)
            {
                // 计算当前的四个差值
                var d1 = prices[i, j - 3] - prices[i, j - 4];
                var d2 = prices[i, j - 2] - prices[i, j - 3];
                var d3 = prices[i, j - 1] - prices[i, j - 2];
                var d4 = prices[i, j] - prices[i, j - 1];

                var pattern = (d1, d2, d3, d4);

                // 如果这个买家还没触发过这个模式
                if (seenInThisBuyer.Add(pattern))
                {
                    // 更新全局得分表
                    if (!scoreMap.TryAdd(pattern, prices[i, j]))
                    {
                        scoreMap[pattern] += prices[i, j];
                    }
                }
            }
        }

        // 最终结果就是 scoreMap 中最大的那个 Value
        long ans = 0;
        if (scoreMap.Count > 0)
        {
            ans = scoreMap.Values.Max();
        }

        Console.WriteLine(ans);
    }

    public static void ProgramC()
    {
        var inputPath = Path.Combine(CustomUtil.GetSourceDir(), "Input.txt");

        // 1. 使用流式读取文件，避免一次性加载所有行到内存 (如果行数极大)
        // 2. Dictionary<int, int>：Key 为压缩后的差值模式，Value 为该模式的累加总和
        var totalSums = new Dictionary<int, int>();

        using (var reader = new StreamReader(inputPath))
        {
            while (reader.ReadLine() is { } line)
            {
                // 3. 局部变量复用，避免每行创建新对象
                var num = Parse(line);

                // 4. 使用滑动窗口/队列记录最近 4 个差值对应的数字位，无需存储整个矩阵
                // 我们只需要记录当前生成的数位 (num % 10) 以及历史状态来计算差值
                var seenPatternsInLine = new HashSet<int>(); // 用于确保每行只统计该模式第一次出现的位置

                int d_prev4 = -99, d_prev3 = -99, d_prev2 = -99, d_curr = 0;

                // 预生成前 3 个数字，因为从第 5 个数（索引 4）才开始计算差值
                for (int j = 0; j < 4 && num > 0; j++)
                {
                    d_prev4 = (int)(num % 10);
                    num = SecretNumber(num);
                }

                // 从第 5 个数开始处理（对应原代码的 j=4，此时有前驱数据）
                // 注意：原逻辑是 j 从 0 到 2000，diff 计算从 j=4 开始。
                // 我们需要维护一个长度为 5 的滑动窗口来准确获取 diff

                // 为了简化逻辑，我们用队列模拟生成过程，或者用变量
                // 这里直接复用原逻辑：每步生成当前位，更新 history

                long currentNum = Parse(line);

                // 缓冲区存储最近生成的数位 (用于计算差值)
                int[] buffer = new int[4];
                for (int k = 0; k < 4; k++)
                {
                    buffer[k] = 0;
                }

                // 为了逻辑清晰，我们重新模拟生成过程：
                var tempNum = Parse(line);

                // 初始化前几个值到 buffer (对应 j=0,1,2,3)
                for (var k = 0; k < 4; k++)
                {
                    if (tempNum == 0 && k > 0) break; // 防止空处理
                    var digit = (int)(tempNum % 10);
                    buffer[k] = digit;
                    tempNum = SecretNumber(tempNum);
                }

                // 从 j=4 开始计算差值模式并累加
                // for (var j = 4; j < 2001; j++)
                // {
                //     if (tempNum == 0 && j >= buffer.Length) break; // 如果生成停止，视情况处理
                //
                //     var currentDigit = (int)(tempNum % 10);
                //
                //     // 计算差值：(val[j]-val[j-1], val[j-1]-val[j-2]...) 
                //     // 注意原代码逻辑是 prices[i,j] - prices[i,j-1]
                //     // buffer[3]=j-4, buffer[2]=j-3, buffer[1]=j-2, buffer[0]=j-1 (根据原数组索引对应关系)
                //
                //     var d1 = currentDigit - buffer[3]; // j - (j-1) ? 不对，看原代码：
                //     // prices[i,j] - prices[i,j-1]. 
                //     // 假设 buffer 按顺序存了 j-4, j-3, j-2, j-1.
                //
                //     var diffA = currentDigit - buffer[3];
                //     var diffB = buffer[3] - buffer[2];
                //     var diffC = buffer[2] - buffer[1];
                //     var diffD = buffer[1] - buffer[0]; // 注意这里原代码是 prices[i,j-1]-prices[i,j-2]
                //
                //     // 修正：根据原代码逻辑，diffs = (j-3 - j-4, j-2 - j-3...)
                //     // 即 (currentDigit - prev1), (prev1 - prev2)... 
                //     // 让我们严格对应原代码：
                //     // prices[i,j]   -> currentDigit
                //     // prices[i,j-1] -> buffer[3]
                //     // prices[i,j-2] -> buffer[2]
                //     // prices[i,j-3] -> buffer[1]
                //     // prices[i,j-4] -> buffer[0] (如果 buffer 存的是 j-4...j-1)
                //
                //     // 重新定义 buffer 逻辑：buffer[k] 存储第 k 个生成的数字（相对当前循环）
                //     // 实际上原代码是顺序生成。我们只需维护最近 5 个值即可计算差值
                //
                //     // 为了最大程度还原且优化，直接按原逻辑写但用局部变量：
                // }

                // --- 更清晰的流式处理实现 ---
                tempNum = Parse(line);
                var lastDigits = new int[4]; // 存储 j-4, j-3, j-2, j-1 的数位

                for (var k = 0; k < 4; k++)
                {
                    if (tempNum == 0 && k > 0) break; // 简单截断，视具体业务逻辑调整
                    lastDigits[k] = (int)(tempNum % 10);
                    tempNum = SecretNumber(tempNum);
                }

                for (var j = 4; j < 2001; j++)
                {
                    if (tempNum == 0) break; // 如果数字生成结束

                    var currentDigit = (int)(tempNum % 10);

                    // 计算差值：对应原代码 prices[i,j]...prices[i,j-4]
                    // diff pattern: (d[j]-d[j-1], d[j-1]-d[j-2], d[j-2]-d[j-3], d[j-3]-d[j-4])
                    var p0 = currentDigit - lastDigits[3];
                    var p1 = lastDigits[3] - lastDigits[2];
                    var p2 = lastDigits[2] - lastDigits[1];
                    var p3 = lastDigits[1] - lastDigits[0];

                    // 5. Key 压缩：将四个差值打包成一个 int。范围 [-9,9] -> [0,18]，每个占 5 bits，共 20 bits
                    // 偏移量 +9 转换为无符号
                    var key = (p0 + 9) | ((p1 + 9) << 5) | ((p2 + 9) << 10) | ((p3 + 9) << 15);

                    if (!seenPatternsInLine.Contains(key))
                    {
                        seenPatternsInLine.Add(key);

                        // 累加当前值 (原代码逻辑：只取每个模式在行内第一次出现的值)
                        if (!totalSums.TryGetValue(key, out var currentSum))
                            totalSums[key] = currentDigit;
                        else
                            totalSums[key] += currentDigit;
                    }

                    // 滑动窗口更新 lastDigits
                    lastDigits[0] = lastDigits[1];
                    lastDigits[1] = lastDigits[2];
                    lastDigits[2] = lastDigits[3];
                    lastDigits[3] = currentDigit;

                    tempNum = SecretNumber(tempNum);
                }
            }
        }

        // 6. 查找最大值
        var ans = totalSums.Values.Prepend(0).Max();

        Console.WriteLine(ans);
    }

    public static void Compare()
    {
        var cur = Stopwatch.StartNew();
        ProgramB();
        cur.Stop();
        Console.WriteLine($"{cur.ElapsedMilliseconds} ms");
        cur =  Stopwatch.StartNew();
        ProgramC();
        cur.Stop();
        Console.WriteLine($"{cur.ElapsedMilliseconds} ms");
    }
}