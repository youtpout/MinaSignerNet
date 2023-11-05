// See https://aka.ms/new-console-template for more information
using MinaSignerNet;
using System.CodeDom.Compiler;
using System.Text;

Console.WriteLine("Hello, World!");

//StringBuilder str = new StringBuilder();
//str.AppendLine("RoundConstants = new List<List<BigInteger>>{");
//for (int i = 0; i < PoseidonHash.PoseidonConfigKimchiFp.RoundConstants.Count; i++)
//{
//    var list = PoseidonHash.PoseidonConfigKimchiFp.RoundConstants[i];
//    str.AppendLine("new List<BigInteger>{");
//    for (int j = 0; j < list.Count; j++)
//    {
//        var data = list[j];
//        str.AppendLine($" BigInteger.Parse(\"{data}\")");
//        if (j < list.Count - 1)
//        {
//            str.AppendLine(",");
//        }      
//    }
//    str.AppendLine("}");
//    if (i < PoseidonHash.PoseidonConfigKimchiFp.RoundConstants.Count - 1)
//    {
//        str.AppendLine(",");
//    }
//}
//str.AppendLine("}");
//var txt = str.ToString();
//Console.WriteLine(txt);
//Console.ReadKey();
