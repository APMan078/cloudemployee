using System;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace ChallengeWebScraping
{
    class Program
    {
        static readonly HttpClient client = new HttpClient();

        static async Task Main(string[] args)
        {
            bool found = false;
            int currentNumber = 0;
            string passcode = "";
            string name = "alvin";

            while (!found)
            {
                currentNumber++;
                passcode = await GetWebContentAsString($"https://challenge.longshotsystems.co.uk/submitgo?answer={currentNumber}&name={name}");

                if (passcode.Contains("Correct!"))
                {
                    Console.WriteLine($"The passcode is: {currentNumber}");
                    found = true;
                }
            }
        }

        private static async Task<string> GetWebContentAsString(string url)
        {
            var httpResponse = await client.GetAsync(url);
            httpResponse.EnsureSuccessStatusCode();
            var content = await httpResponse.Content.ReadAsStringAsync();
            return content;
        }

        private static string ExtractAnswerFromContent(string content)
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(content);
            var p = document.DocumentNode.Descendants("body")
                                       .FirstOrDefault(a => a.InnerHtml.Contains("body{color:green;}"));
            if (p != null)
            {
                return p.InnerHtml.Split(new string[] { "<br>" }, StringSplitOptions.None).LastOrDefault() ?? "";
            }

            return string.Empty;
        }
    }
}