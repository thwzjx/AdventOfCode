using AdventOfCode.Util;

namespace AdventOfCode.Adcode2024.CodeDay25;

public class Day25
{
    private class KeyAndLcok
    {
        public string Type { get; set; }
        public List<int> Vals { get; set; }
        public int maxH;

        public bool IsMatch(KeyAndLcok kl)
        {
            var res = Vals.Zip(kl.Vals, (a,b)=>(a+b) <= maxH).All(x=>x);
            return res;
        }
    }
    

    private static KeyAndLcok ParseInput(string[] input)
    {
        var m = input.Length;
        var n = input[0].Length;
        var vals = new int[n];
        var isLock = true;
        for (var j = 0; j < n; j++)
        {
            if (input[0][j] == '.')
            {
                isLock = false;
                break;
            }
        }

        if (isLock)
        {
            for (var j = 0; j < n; j++)
            {
                var i = 1;
                while (i < m && input[i][j] == '#')
                {
                    vals[j]++;
                    i++;
                }
            }

            return new KeyAndLcok { Vals = vals.ToList(),Type = "lock",maxH = m-2 };
        }

        for (var j = 0; j < n; j++)
        {
            var i = m - 2;
            while (i >= 0 && input[i][j] == '#')
            {
                vals[j]++;
                i--;
            }
        }

        return new KeyAndLcok { Vals = vals.ToList(),Type = "key",maxH = m-2};
    }

    public static void ProgramA()
    {
        var cur = CustomUtil.GetSourceDir();
        var test = Path.Combine(cur, "Input-test.txt");
        var input = Path.Combine(cur, "Input.txt");
        var content = File.ReadAllText(input)
            .Split("\n\n", StringSplitOptions.TrimEntries)
            .Select(x => x.Split("\n", StringSplitOptions.TrimEntries))
            .Select(x=>ParseInput(x)).ToArray();
        var keys = content.Where(x => x.Type == "key").ToArray();
        var locks = content.Where(x => x.Type == "lock").ToArray();
        var ans = 0;
        foreach (var key in keys)
        {
            foreach (var l in locks)
            {
                if (key.IsMatch(l))
                {
                    ans++;
                }
            }
        }

        // Console.WriteLine(keys.Length);
        // Console.WriteLine(locks.Length);
        Console.WriteLine(ans);
    }
}