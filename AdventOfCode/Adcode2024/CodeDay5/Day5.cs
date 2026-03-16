namespace AdventOfCode.Adcode2024.CodeDay5;

public class Day5
{
    
    public static void ProgramA()
    {
        var test = "/Users/haowen/RiderProjects/AdventOfCode/AdventOfCode/Adcode2024/CodeDay5/Input-test";
        var input = "/Users/haowen/RiderProjects/AdventOfCode/AdventOfCode/Adcode2024/CodeDay5/Input";
        var file = File.ReadAllLines(input);

        var isBuild = true;
        var graph = new Dictionary<int, List<int>>();
        var ans = 0;

        foreach (var line in file)
        {
            if (line == "")
            {
                isBuild = false;
                continue;
            }

            if (isBuild)
            {
                var number = line.Split('|').Select(int.Parse).ToArray();
                var x = number[0];
                var y = number[1];

                if (!graph.ContainsKey(x))
                {
                    graph[x] = [];
                }

                graph[x].Add(y);
            }
            else
            {
                var nums = line.Split(',').Select(int.Parse).ToArray();
                var before = new HashSet<int>();
                var isValid = true;

                foreach (var num in nums)
                {
                    if (graph.ContainsKey(num))
                    {
                        if (graph[num].Any(mustAfter => before.Contains(mustAfter)))
                        {
                            isValid = false;
                        }
                    }

                    if (!isValid)
                    {
                        break;
                    }

                    before.Add(num);
                }

                if (!isValid) continue;
                var mid = nums[nums.Length / 2];
                ans += mid;
            }
        }

        Console.WriteLine(ans);
    }
/// <summary>
/// 本质上就是先构建一个依赖图，然后按照依赖顺序对错误关系的图进行修正即可。
/// </summary>
    public static void ProgramB()
    {
        var test = "/Users/haowen/RiderProjects/AdventOfCode/AdventOfCode/Adcode2024/CodeDay5/Input-test";
        var input = "/Users/haowen/RiderProjects/AdventOfCode/AdventOfCode/Adcode2024/CodeDay5/Input";
        var file = File.ReadAllLines(input);

        var isBuild = true;
        var graph = new Dictionary<int, List<int>>();
        var ans = 0;

        foreach (var line in file)
        {
            if (line == "")
            {
                isBuild = false;
                continue;
            }

            if (isBuild)
            {
                var number = line.Split('|').Select(int.Parse).ToArray();
                var x = number[0];
                var y = number[1];

                if (!graph.ContainsKey(x))
                {
                    graph[x] = [];
                }

                graph[x].Add(y);
            }
            else
            {
                var nums = line.Split(',').Select(int.Parse).ToArray();
                ans += Process(graph, nums);
            }
        }

        Console.WriteLine(ans);   
    }

    static int Process(Dictionary<int, List<int>> graph, int[] nums)
    {
        var before = new HashSet<int>();
        var isValid = true;

        foreach (var num in nums)
        {
            if (graph.ContainsKey(num))
            {
                if (graph[num].Any(mustAfter => before.Contains(mustAfter)))
                {
                    isValid = false;
                }
            }

            if (!isValid)
            {
                break;
            }

            before.Add(num);
        }

        if (isValid) return 0;
        Array.Sort(nums, (a, b) =>
        {
            if (graph.ContainsKey(a) && graph[a].Contains(b))
            {
                return -1;
            }

            if (graph.ContainsKey(b) && graph[b].Contains(a))
            {
                return 1;
            }

            return 0;
        });
        // nums.Sort();
        var mid = nums[nums.Length / 2];
        return mid;
    }
}