using System.Text;
using System.Text.RegularExpressions;

string source = File.ReadAllText("FL.cs");
StringBuilder docs = new("# Fraglib Documentation\n");

MatchCollection? comments = Regex.Matches(source, @"///.*\n");

foreach (Match comment in comments) {
    string commentText = comment.Value.Trim();

    string region = Regex.Match(commentText, @"<region>(.*?)<\/region>").Groups[1].Value.Trim();
    if (!string.IsNullOrEmpty(region)) {
        docs.AppendLine($"## {region}");
    }

    string name = Regex.Match(commentText, @"<name>(.*?)<\/name>").Groups[1].Value.Trim();
    if (!string.IsNullOrEmpty(name)) {
        docs.Append($"### {name}");
    }
    
    string summary = Regex.Match(commentText, @"<summary>(.*?)<\/summary>").Groups[1].Value.Trim();
    if (!string.IsNullOrEmpty(summary)) {
        docs.AppendLine($"{summary} ");
    }

    MatchCollection? parameters = Regex.Matches(commentText, @"<param name=""(.*?)"".*?>(.*?)<\/param>");
    foreach (Match parameter in parameters) {
        string paramName = parameter.Groups[1].Value.Trim();
        string paramDescription = parameter.Groups[2].Value.Trim();

        docs.AppendLine($"- **{paramName}**: {paramDescription}");
    }

    string returns = Regex.Match(commentText, @"<returns>(.*?)<\/returns>").Groups[1].Value.Trim();
    if (!string.IsNullOrEmpty(returns)) {
        docs.AppendLine($" ({returns})");
    }
}

File.WriteAllText("Documentation.md", docs.ToString());

Console.WriteLine("Completed.");