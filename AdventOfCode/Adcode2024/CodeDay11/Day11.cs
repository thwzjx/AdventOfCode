using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode.Util;

namespace AdventOfCode.Adcode2024.CodeDay11;

public class Day11
{
    private static int NumbersCount(long n)
    {
        if (n == 0)
        {
            return 1;
        }

        var res = 0;
        while (n > 0)
        {
            res++;
            n /= 10;
        }

        return res;
    }

    private static long PowInt(long a, int b)
    {
        long result = 1;
        for (var i = 0; i < b; i++)
        {
            result *= a;
        }

        return result;
    }

    private sealed class StoneNode
    {
        public long Value { get; set; }
        public StoneNode? Next { get; set; }
    }

    private static StoneNode BuildList(long[] input)
    {
        StoneNode? head = null;
        StoneNode? tail = null;

        foreach (var n in input)
        {
            var node = new StoneNode
            {
                Value = n
            };

            if (head == null)
            {
                head = node;
                tail = node;
            }
            else
            {
                tail!.Next = node;
                tail = node;
            }
        }

        return head!;
    }

    private static void PrintList(StoneNode? head)
    {
        var temp = head;
        while (temp != null)
        {
            Console.Write(temp.Value);
            if (temp.Next != null)
            {
                Console.Write(" ");
            }

            temp = temp.Next;
        }

        Console.WriteLine();
    }

    private static int CountNodes(StoneNode? head)
    {
        var count = 0;
        var temp = head;

        while (temp != null)
        {
            count++;
            temp = temp.Next;
        }

        return count;
    }

    private static void BlinkOnce(StoneNode? head)
    {
        var current = head;

        while (current != null)
        {
            if (current.Value == 0)
            {
                current.Value = 1;
                current = current.Next;
                continue;
            }

            var digits = NumbersCount(current.Value);

            if ((digits & 1) == 1)
            {
                current.Value *= 2024;
                current = current.Next;
            }
            else
            {
                var half = digits / 2;
                var divisor = PowInt(10, half);

                var left = current.Value / divisor;
                var right = current.Value % divisor;

                current.Value = left;

                var inserted = new StoneNode
                {
                    Value = right,
                    Next = current.Next
                };

                current.Next = inserted;

                // 本轮 blink 中，新插入的节点不再继续处理
                current = inserted.Next;
            }
        }
    }

    public static void ProgramA()
    {
        var cur = CustomUtil.GetSourceDir();
        var inputFile = Path.Combine(cur, "Input.txt");

        var input = File.ReadAllLines(inputFile)[0]
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(long.Parse)
            .ToArray();

        var head = BuildList(input);

        for (var i = 0; i < 25; i++)
        {
            BlinkOnce(head);
        }

        Console.WriteLine(CountNodes(head));
    }
    /// <summary>
    /// 这个好像跟顺序没有关系，可以不关注顺序
    /// </summary>
    public static void ProgramB()
    {
        var cur = CustomUtil.GetSourceDir();
        var inputFile = Path.Combine(cur, "Input.txt");

        var input = File.ReadAllLines(inputFile)[0]
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(long.Parse)
            .ToArray();
        var nums = new Dictionary<long, long>();
        foreach (var n in input)
        {
            nums.TryAdd(n, 0);
            nums[n]++;
        }

        for (int i = 0; i < 75; i++)
        {
            var nxt = new Dictionary<long, long>();
            foreach (var n in nums)
            {
                var digits = NumbersCount(n.Key);
                if (n.Key == 0)
                {
                    nxt.TryAdd(1, 0);
                    nxt[1] += n.Value;
                } else if (digits % 2 == 1)
                {
                    nxt.TryAdd(n.Key * 2024, 0);
                    nxt[n.Key * 2024] += n.Value;
                }
                else
                {
                    var half = digits / 2;
                    var divisor = PowInt(10, half);

                    var left = n.Key / divisor;
                    var right = n.Key % divisor;
                    nxt.TryAdd(left, 0);
                    nxt.TryAdd(right, 0);
                    nxt[left] += n.Value;
                    nxt[right] += n.Value;
                    // nxt[left] = n.Value;
                    // nxt[right] = n.Value;
                }
            }

            nums = nxt;
        }

        long ans = 0;
        foreach (var n in nums)
        {
            ans += n.Value;
        }

        Console.WriteLine(ans);
    }
}