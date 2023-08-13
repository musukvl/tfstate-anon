using System.Text.RegularExpressions;

namespace Amba.TfstateAnon;

public class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("Please provide a file path as an argument.");
            return;
        }

        string filePath = args[0];

        if (!File.Exists(filePath))
        {
            Console.WriteLine($"The file '{filePath}' does not exist.");
            return;
        }

        string fileContent = File.ReadAllText(filePath);

        var newGuids = new Dictionary<string, string>();
        string guidRegexPattern = @"\b[0-9a-f]{8}\-[0-9a-f]{4}\-[0-9a-f]{4}\-[0-9a-f]{4}\-[0-9a-f]{12}\b";

        fileContent = Regex.Replace(fileContent, guidRegexPattern, match =>
        {
            string originalGuid = match.Value;

            if (!newGuids.ContainsKey(originalGuid))
            {
                string newGuid = Guid.NewGuid().ToString();
                newGuids[originalGuid] = newGuid;
            }

            return newGuids[originalGuid];
        });

        fileContent = Regex.Replace(fileContent, "\"private\":\\s*\"[^\"]+\"", match =>
        {
            return $"\"private\": \"{GenerateRandomString(175)}=\"";
        });

        fileContent = Regex.Replace(fileContent, "\"(secondary_access_key|primary_access_key|secondary_connection_string|primary_connection_string|primary_blob_connection_string)\":\\s*\"[^\"]+\"", match =>
        {
            return $"\"{match.Groups[1].Value}\": \"{GenerateRandomString(100)}==\"";
        });
        
        Console.WriteLine(fileContent);
    }

    static Random random = new Random();

    static string GenerateRandomString(int length)
    {
        const string chars = "ABCXYZabcxyz";

        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

}