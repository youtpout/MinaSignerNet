// See https://aka.ms/new-console-template for more information
using MinaConsole;
using MinaSignerNet;
using System.CodeDom.Compiler;
using System.Text;
using System.Text.Json;

Console.WriteLine("Convert json to c# config file");

string content = File.ReadAllText("poseidon-const.json");
JsonSerializerOptions options = new JsonSerializerOptions();
options.PropertyNameCaseInsensitive = true;
PoseidonConf poseidonConfig = JsonSerializer.Deserialize<PoseidonConf>(content, options);

StringBuilder str = new StringBuilder();
str.AppendLine("RoundConstants = new List<List<BigInteger>>{");
for (int i = 0; i < poseidonConfig.RoundConstants.Count; i++)
{
    var list = poseidonConfig.RoundConstants[i];
    str.AppendLine("new List<BigInteger>{");
    for (int j = 0; j < list.Count; j++)
    {
        var data = list[j];
        str.AppendLine($" BigInteger.Parse(\"{data}\")");
        if (j < list.Count - 1)
        {
            str.AppendLine(",");
        }
    }
    str.AppendLine("}");
    if (i < poseidonConfig.RoundConstants.Count - 1)
    {
        str.AppendLine(",");
    }
}
str.AppendLine("}");
var txt = str.ToString();
Console.WriteLine(txt);
Console.ReadKey();
