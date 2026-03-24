using AdventOfCode.Util;

namespace AdventOfCode.Adcode2024.CodeDay17;

public class Day17
{
    private class Computer
    {
        private long A { get; set; }
        private long B { get; set; }
        private long C { get; set; }

        private int[] Program { get; }
        private int Ip { get; set; }

        private readonly List<int> _output = new();

        public Computer(long a, long b, long c, int[] program)
        {
            A = a;
            B = b;
            C = c;
            Program = program;
            Ip = 0;
        }

        private long GetComboValue(int operand)
        {
            return operand switch
            {
                0 => 0,
                1 => 1,
                2 => 2,
                3 => 3,
                4 => A,
                5 => B,
                6 => C,
                7 => throw new Exception("Combo operand 7 is invalid."),
                _ => throw new Exception($"Invalid combo operand: {operand}")
            };
        }

        private long DivideByPowerOfTwo(long value, long exponent)
        {
            return value >> (int)exponent;
        }

        public List<int> Run()
        {
            while (Ip >= 0 && Ip < Program.Length)
            {
                if (Ip + 1 >= Program.Length)
                {
                    break;
                }

                var opcode = Program[Ip];
                var operand = Program[Ip + 1];
                var jumped = false;

                switch (opcode)
                {
                    case 0: // adv
                    {
                        var combo = GetComboValue(operand);
                        A = DivideByPowerOfTwo(A, combo);
                        break;
                    }
                    case 1: // bxl
                    {
                        B ^= operand;
                        break;
                    }
                    case 2: // bst
                    {
                        var combo = GetComboValue(operand);
                        B = combo % 8;
                        break;
                    }
                    case 3: // jnz
                    {
                        if (A != 0)
                        {
                            Ip = operand;
                            jumped = true;
                        }
                        break;
                    }
                    case 4: // bxc
                    {
                        B ^= C;
                        break;
                    }
                    case 5: // out || only here will be out values
                    {
                        var combo = GetComboValue(operand);
                        _output.Add((int)(combo % 8));
                        break;
                    }
                    case 6: // bdv
                    {
                        var combo = GetComboValue(operand);
                        B = DivideByPowerOfTwo(A, combo);
                        break;
                    }
                    case 7: // cdv
                    {
                        var combo = GetComboValue(operand);
                        C = DivideByPowerOfTwo(A, combo);
                        break;
                    }
                    default:
                        throw new Exception($"Invalid opcode: {opcode}");
                }

                if (!jumped)
                {
                    Ip += 2;
                }
            }

            return _output;
        }
    }

    private static (long A, long B, long C, int[] Program) Parse(string[] lines)
    {
        long a = 0, b = 0, c = 0;
        int[] program = Array.Empty<int>();

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            if (line.StartsWith("Register A:"))
            {
                a = long.Parse(line.Split(": ")[1]);
            }
            else if (line.StartsWith("Register B:"))
            {
                b = long.Parse(line.Split(": ")[1]);
            }
            else if (line.StartsWith("Register C:"))
            {
                c = long.Parse(line.Split(": ")[1]);
            }
            else if (line.StartsWith("Program:"))
            {
                program = line.Split(": ")[1]
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse)
                    .ToArray();
            }
        }

        return (a, b, c, program);
    }

    private static List<int> RunWithA(long a, long b, long c, int[] program)
    {
        var computer = new Computer(a, b, c, program);
        return computer.Run();
    }

    private static bool MatchSuffix(List<int> output, int[] program, int len)
    {
        if (output.Count != len)
        {
            return false;
        }

        int start = program.Length - len;
        for (int i = 0; i < len; i++)
        {
            if (output[i] != program[start + i])
            {
                return false;
            }
        }

        return true;
    }

    private static long SolvePart2(long b, long c, int[] program)
    {
        var candidates = new List<long> { 0 };

        for (int len = 1; len <= program.Length; len++)
        {
            var nextCandidates = new List<long>();

            foreach (var prefix in candidates)
            {
                for (int digit = 0; digit < 8; digit++)
                {
                    long a = prefix * 8 + digit;
                    var output = RunWithA(a, b, c, program);

                    if (MatchSuffix(output, program, len))
                    {
                        nextCandidates.Add(a);
                    }
                }
            }

            candidates = nextCandidates;
        }

        return candidates.Min();
    }

    public static void ProgramA()
    {
        var cur = CustomUtil.GetSourceDir();
        var input = Path.Combine(cur, "Input.txt");

        var lines = File.ReadAllLines(input);
        var (a, b, c, program) = Parse(lines);

        var output = RunWithA(a, b, c, program);
        Console.WriteLine(string.Join(",", output));
    }

    public static void ProgramB()
    {
        var cur = CustomUtil.GetSourceDir();
        var input = Path.Combine(cur, "Input.txt");

        var lines = File.ReadAllLines(input);
        var (_, b, c, program) = Parse(lines);

        var ans = SolvePart2(b, c, program);
        Console.WriteLine(ans);
    }
}