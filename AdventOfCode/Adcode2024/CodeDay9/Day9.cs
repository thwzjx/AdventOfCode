using System;
using System.Collections.Generic;
using System.IO;
using AdventOfCode.Util;

namespace AdventOfCode.Adcode2024.CodeDay9;

public class Day9
{
    public static void ProgramA()
    {
        var cur = CustomUtil.GetSourceDir();
        var test = Path.Combine(cur, "Input-test.txt");
        var inputFile = Path.Combine(cur, "Input.txt");
        var content = File.ReadAllLines(inputFile)[0];
        var totalL = 0;
        foreach (var c in content)
        {
            totalL += int.Parse(c.ToString());
        }
        var numbers = new int[totalL];
        var isFile = true;
        var index = 0;
        var currentIndex = 0;
        // 初始化数组
        foreach (var c in content)
        {
            var n = int.Parse(c.ToString());
            if (isFile)
            {
                for (int i = 0; i < n; i++)
                {
                    numbers[currentIndex+i] = index;
                }
                // 改变状态
                isFile = false;
                index += 1;
            }
            else
            {
                for (int i = 0; i < n; i++)
                {
                    numbers[currentIndex+i] = -1;
                }
                isFile = true;
            }
            // 更新下一个index的位置
            currentIndex += n;
        }
        // 开始改变状态
        long ans = 0;
        var left = 0;
        var right = totalL-1;
        // Console.WriteLine(numbers);
        while (left < right)
        {
            while (left < totalL && numbers[left] >= 0)
            {
                ans += numbers[left] * left;
                left++;
            }

            while (right > 0 && numbers[right] < 0)
            {
                right--;
            }

            if (left >= right)
            {
                break;
            }
            CustomUtil.Swap(numbers, left, right);
            // left++;
            // right--;
        }

        while (numbers[left] >= 0)
        {
            ans += numbers[left] * left;
            left++;
        }
        Console.WriteLine(ans);
    }
    public readonly record struct FileBlock(int Start, int Id, int Length);
    public readonly record struct SpaceBlock(int Start, int Length);
    public sealed class DiskLayout
    {
        public List<FileBlock> Files { get; } = new();
        public List<SpaceBlock> Spaces { get; } = new();
    }
    public static DiskLayout ParseDiskMap(string diskMap)
    {
        var layout = new DiskLayout();

        var isFile = true;
        var fileId = 0;
        var currentPosition = 0;

        foreach (var ch in diskMap)
        {
            var length = ch - '0';

            if (isFile)
            {
                layout.Files.Add(new FileBlock(currentPosition, fileId, length));
                fileId++;
            }
            else
            {
                layout.Spaces.Add(new SpaceBlock(currentPosition, length));
            }

            currentPosition += length;
            isFile = !isFile;
        }

        return layout;
    }
    public static void CompactFiles(List<FileBlock> files, List<SpaceBlock> spaces)
    {
        // Files are processed from right to left.
        // Each file can move at most once, into the leftmost free space
        // that is large enough and lies strictly to its left.
        for (var i = files.Count - 1; i >= 0; i--)
        {
            var file = files[i];

            for (var j = 0; j < spaces.Count; j++)
            {
                var space = spaces[j];

                if (space.Start >= file.Start)
                    break;

                if (space.Length < file.Length)
                    continue;

                files[i] = file with { Start = space.Start };
                spaces[j] = new SpaceBlock(Start: space.Start + file.Length, Length: space.Length - file.Length);
                break;
            }
        }
    }
    public static long CalculateChecksum(IEnumerable<FileBlock> files)
    {
        long checksum = 0;

        foreach (var file in files)
        {
            long start = file.Start;
            long length = file.Length;
            long id = file.Id;

            var positionSum = (2 * start + length - 1) * length / 2;
            checksum += positionSum * id;
        }

        return checksum;
    }
    public static void ProgramB()
    {
        var cur = CustomUtil.GetSourceDir();
        var test = Path.Combine(cur, "Input-test.txt");
        var inputFile = Path.Combine(cur, "Input.txt");
        var diskMap = File.ReadAllText(inputFile).Trim();
        var layout = ParseDiskMap(diskMap);
        CompactFiles(layout.Files, layout.Spaces);
        var ans = CalculateChecksum(layout.Files);
        Console.WriteLine(ans);
    }
}