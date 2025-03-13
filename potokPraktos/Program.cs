using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

class Program
{
    static void Main(string[] args)
    {
        string[] files = Directory.GetFiles("C:\\Users\\Олег\\Desktop\\text", "*.txt");

        // Синхронный подход
        long syncTime = Time(() =>
        {
            int totalChars = CountCharsSync(files);
            Console.WriteLine($"Синхронный подход: {totalChars} символов");
        });
        Console.WriteLine($"Время выполнения синхронного подхода: {syncTime} мс");

        // Асинхронный подход
        long asyncTime = Time(async () =>
        {
            int totalChars = await CountCharsAsync(files);
            Console.WriteLine($"Асинхронный подход: {totalChars} символов");
        }).Result;
        Console.WriteLine($"Время выполнения асинхронного подхода: {asyncTime} мс");

        // Многопоточный подход
        long multiThreadTime = Time(() =>
        {
            int totalChars = CountCharsMultiThread(files);
            Console.WriteLine($"Многопоточный подход: {totalChars} символов");
        });
        Console.WriteLine($"Время выполнения многопоточного подхода: {multiThreadTime} мс");

        // Сравнение скорости выполнения
        CompareExecutionTimes(syncTime, asyncTime, multiThreadTime);
    }

    static long Time(Action action)
    {
        var stopwatch = Stopwatch.StartNew();
        action();
        stopwatch.Stop();
        return stopwatch.ElapsedMilliseconds;
    }

    static async Task<long> Time(Func<Task> asyncAction)
    {
        var stopwatch = Stopwatch.StartNew();
        await asyncAction();
        stopwatch.Stop();
        return stopwatch.ElapsedMilliseconds;
    }

    static int CountCharsSync(string[] files)
    {
        int totalChars = 0;
        foreach (var file in files)
        {
            string content = File.ReadAllText(file);
            totalChars += content.Length;
        }
        return totalChars;
    }

    static async Task<int> CountCharsAsync(string[] files)
    {
        int totalChars = 0;
        foreach (var file in files)
        {
            string content = await File.ReadAllTextAsync(file);
            totalChars += content.Length;
        }
        return totalChars;
    }

    static int CountCharsMultiThread(string[] files)
    {
        int totalChars = 0;
        Parallel.ForEach(files, file =>
        {
            string content = File.ReadAllText(file);
            Interlocked.Add(ref totalChars, content.Length);
        });
        return totalChars;
    }

    static void CompareExecutionTimes(long syncTime, long asyncTime, long multiThreadTime)
    {
        Console.WriteLine("\nСравнение скорости выполнения:");
        Console.WriteLine($"Синхронный подход: {syncTime} мс");
        Console.WriteLine($"Асинхронный подход: {asyncTime} мс");
        Console.WriteLine($"Многопоточный подход: {multiThreadTime} мс");

        if (syncTime < asyncTime && syncTime < multiThreadTime)
        {
            Console.WriteLine("Синхронный подход был самым быстрым.");
        }
        else if (asyncTime < syncTime && asyncTime < multiThreadTime)
        {
            Console.WriteLine("Асинхронный подход был самым быстрым.");
        }
        else if (multiThreadTime < syncTime && multiThreadTime < asyncTime)
        {
            Console.WriteLine("Многопоточный подход был самым быстрым.");
        }
        else
        {
            Console.WriteLine("Невозможно определить самый быстрый подход, так как несколько подходов имеют одинаковое время выполнения.");
        }
    }
}