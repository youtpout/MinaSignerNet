using MinaSignerPoseidon;
using System.Diagnostics;
using System.Numerics;

class Program
{
    static void BenchmarkHash()
    {

        BigInteger message = BigInteger.Parse("3412");
        BigInteger message2 = BigInteger.Parse("548748548");
        var list = new List<BigInteger> { message, message2 };
        for (int i = 0; i < 10_000; i++)
        {
            BigInteger hash = PoseidonHash.Hash(list);
        }
    }

    static void BenchmarkCompleteHash()
    {

        BigInteger message = BigInteger.Parse("3412");
        BigInteger message2 = BigInteger.Parse("548748548");
        var list = new List<BigInteger> { message, message2 };
        for (int i = 0; i < 10_000; i++)
        {
            BigInteger hash = MinaSignerNet.PoseidonHash.Hash(list);
        }
    }

    static void Main()
    {
        Stopwatch sw = Stopwatch.StartNew();

        BenchmarkHash();

        sw.Stop();
        Console.WriteLine($"BenchmarkHash : {sw.ElapsedMilliseconds} ms");

        sw = Stopwatch.StartNew();

        BenchmarkCompleteHash();

        sw.Stop();
        Console.WriteLine($"BenchmarkCompleteHash : {sw.ElapsedMilliseconds} ms");

        Console.ReadKey();
    }
}