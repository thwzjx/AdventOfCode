namespace AdventOfCode.Adcode2024.CodeDay2;

public class Day2
{
    public static void ProblemA()
    {
        const string test = "~/RiderProjects/AdventOfCode/AdventOfCode/Adcode2024/CodeDay2/Input-test.txt";
        const string input = "~/RiderProjects/AdventOfCode/AdventOfCode/Adcode2024/CodeDay2/Input.txt";
        var file = File.ReadLines(input);
        var ans = 0;
        foreach (var line in file)
        {
            var x = line.Split(' ').Select(int.Parse).ToArray();
            if (IsSafeB(x))
            {
                ans++;
            }
        }
        Console.WriteLine(ans);
    }
    
    static bool IsSafe(int[]? x)
    {
        if (x == null || x.Length < 2)
        {
            return false;
        }

        if (x.Length == 2)
        {
            return true;
        }
        var baseModel = x[1] - x[0];
        for (int j = 1; j <= x.Length - 1; j++)
        {
            var curDif =  x[j] - x[j - 1];
            if (Math.Abs(curDif) > 3 || curDif * baseModel <= 0)
            {
                return false;
            }
        }
        return true;
    }

    static bool IsSafeB(int[]? x)
    {
        if (x == null || x.Length < 2) return false;
        if (IsSafe(x)) return true;

        for (int i = 0; i < x.Length; i++)
        {
            int[] y = new int[x.Length - 1];
            int k = 0;

            for (int j = 0; j < x.Length; j++)
            {
                if (j == i) continue;
                y[k++] = x[j];
            }

            if (IsSafe(y)) return true;
        }

        return false;
    }
}