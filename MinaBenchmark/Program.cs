using MinaSignerPoseidon;
using System.Diagnostics;
using System.Numerics;
using System.Collections.Generic;

class Program
{
    static void BenchmarkHash()
    {
        BigInteger message = new BigInteger(3412);
        BigInteger message2 = new BigInteger(548748548);
        var list = new List<BigInteger> { message, message2 };

        for (int i = 0; i < 10_000; i++)
        {
            BigInteger hash = PoseidonHash.Hash(list);
        }
    }

    static void BenchmarkCompleteHash()
    {
        BigInteger message = new BigInteger(3412);
        BigInteger message2 = new BigInteger(548748548);
        var list = new List<BigInteger> { message, message2 };

        for (int i = 0; i < 10_000; i++)
        {
            BigInteger hash = MinaSignerNet.PoseidonHash.Hash(list);
        }
    }

    static void Hash_Batch_StressTest()
    {
        const int hashCount = 100_000;
        var inputs = new List<List<BigInteger>>(hashCount);

        for (int i = 0; i < hashCount; i++)
        {
            inputs.Add(new List<BigInteger>
            {
                new BigInteger(i),
                new BigInteger(i + 1)
            });
        }

        // Warm-up JIT
        PoseidonHash.Hash(inputs[0]);

        var sw = Stopwatch.StartNew();

        BigInteger last = BigInteger.Zero;
        for (int i = 0; i < hashCount; i++)
        {
            last = PoseidonHash.Hash(inputs[i]);
        }

        sw.Stop();

        double elapsedMs = sw.Elapsed.TotalMilliseconds;
        double hashesPerSecond = hashCount / sw.Elapsed.TotalSeconds;

        Console.WriteLine("Poseidon batch test (many small hashes)");
        Console.WriteLine($"Hashes           : {hashCount}");
        Console.WriteLine($"Total time       : {elapsedMs:F2} ms");
        Console.WriteLine($"Hashes/sec       : {hashesPerSecond:F2}");
        Console.WriteLine($"Last hash        : {last}");
        Console.WriteLine();
    }

    static void Hash_Batch_StressTest_One_Hash()
    {
        const int elementCount = 100_000;
        var inputs = new List<BigInteger>(elementCount);

        for (int i = 0; i < elementCount; i++)
            inputs.Add(new BigInteger(i));

        // Warm-up JIT
        PoseidonHash.Hash(new List<BigInteger> { 1, 2 });

        var sw = Stopwatch.StartNew();
        BigInteger last = PoseidonHash.Hash(inputs);
        sw.Stop();

        double elapsedMs = sw.Elapsed.TotalMilliseconds;
        double elementsPerSecond = elementCount / sw.Elapsed.TotalSeconds;

        Console.WriteLine("Poseidon single huge hash");
        Console.WriteLine($"Elements         : {elementCount}");
        Console.WriteLine($"Total time       : {elapsedMs:F2} ms");
        Console.WriteLine($"Elements/sec     : {elementsPerSecond:F2}");
        Console.WriteLine($"Last hash        : {last}");
        Console.WriteLine();
    }

    static void Main()
    {
        Console.WriteLine("=== Poseidon CPU Benchmarks ===");
        Console.WriteLine();

        var sw = Stopwatch.StartNew();
        BenchmarkHash();
        sw.Stop();
        Console.WriteLine($"BenchmarkHash (10k)              : {sw.ElapsedMilliseconds} ms");

        sw = Stopwatch.StartNew();
        BenchmarkCompleteHash();
        sw.Stop();
        Console.WriteLine($"BenchmarkCompleteHash (10k)      : {sw.ElapsedMilliseconds} ms");

        Console.WriteLine();

        Hash_Batch_StressTest();
        Hash_Batch_StressTest_One_Hash();

        Console.WriteLine("Done.");
        Console.ReadKey();
    }
}
