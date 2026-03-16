namespace AdventOfCode.Adcode2024.CodeDay4;

public class Day4
{
    public static void ProgramA()
    {
        const string test = "/Users/haowen/RiderProjects/AdventOfCode/AdventOfCode/Adcode2024/CodeDay4/Input-test.txt";
        const string input = "/Users/haowen/RiderProjects/AdventOfCode/AdventOfCode/Adcode2024/CodeDay4/Input.txt";
        var file = File.ReadAllLines(input);
        var temp = new List<List<char>>();
        foreach (var line in file)
        {
            temp.Add(line.ToCharArray().ToList());
        }
        var ans = 0;
        var m = temp.Count;
        var n = temp[0].Count;
        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (temp[i][j] != 'X' && temp[i][j] != 'S')
                {
                    continue;
                }
                // 横着
                if (j + 3 <= n - 1)
                {
                    var cur = new List<char>
                    {temp[i][j],
                        temp[i][j + 1],
                        temp[i][j + 2],
                        temp[i][j + 3]
                    };
                    var s = new string(cur.ToArray());
                    if (s is "XMAS" or "SAMX")
                    {
                        ans++;
                    }
                }
                // 竖着
                if (i + 3 <= m - 1)
                {
                    var cur = new List<char>
                    {temp[i][j],
                        temp[i + 1][j],
                        temp[i + 2][j],
                        temp[i+3][j ]
                    };
                    var s = new string(cur.ToArray());
                    if (s is "XMAS" or "SAMX")
                    {
                        ans++;
                    }
                }
                // 左下
                if (i + 3 <= m - 1 && j - 3 >= 0)
                {
                    var cur = new List<char>
                    {
                        temp[i][j],
                        temp[i + 1][j - 1],
                        temp[i + 2][j - 2],
                        temp[i+3][j -3]
                    };
                    var s = new string(cur.ToArray());
                    if (s is "XMAS" or "SAMX")
                    {
                        ans++;
                    }
                }
                // 右下
                if (i + 3 <= m - 1 && j + 3 <= n-1)
                {
                    var cur = new List<char>
                    {
                        temp[i][j],
                        temp[i + 1][j + 1],
                        temp[i + 2][j + 2],
                        temp[i+3][j +3]
                    };
                    var s = new string(cur.ToArray());
                    // Console.WriteLine(s);
                    if (s is "XMAS" or "SAMX")
                    {
                        ans++;
                    }
                }
            }
            
        }

        Console.WriteLine(ans);
    }

    public static void ProgramB()
    {
        const string test = "/Users/haowen/RiderProjects/AdventOfCode/AdventOfCode/Adcode2024/CodeDay4/Input-test.txt";
        const string input = "/Users/haowen/RiderProjects/AdventOfCode/AdventOfCode/Adcode2024/CodeDay4/Input.txt";
        var file = File.ReadAllLines(input);
        var temp = new List<List<char>>();
        foreach (var line in file)
        {
            temp.Add(line.ToCharArray().ToList());
        }
        var ans = 0;
        var m = temp.Count;
        var n = temp[0].Count;
        for (int i = 1; i < m - 1; i++)
        {
            for (int j = 1; j < n - 1; j++)
            {
                if (temp[i][j] != 'A')
                {
                    continue;
                }

                var lu = new List<char> { temp[i - 1][j - 1], temp[i + 1][j + 1] };
                var ru = new List<char> { temp[i - 1][j+1], temp[i + 1][j - 1] };
                var ls = new string(lu.ToArray());
                var rs = new string(ru.ToArray());
                if ((ls is "SM" or "MS") && (rs is "MS" or "SM"))
                {
                    ans++;
                }
            }
        }

        Console.WriteLine(ans);
    }
}