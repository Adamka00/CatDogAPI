using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json.Linq;

class Program
{
    static async Task Main()
    {
        Console.WriteLine("Cica vagy kutya? (c/d)  | Kilépés: 'exit'");

        while (true)
        {
            Console.Write("Választás: ");
            var input = Console.ReadLine()?.ToLower();

            if (input == "exit")
                break;

            string url = input switch
            {
                "c" => "https://api.thecatapi.com/v1/images/search",
                "d" => "https://dog.ceo/api/breeds/image/random",
                _ => null
            };

            if (url == null)
            {
                Console.WriteLine("Érvénytelen válasz (csak 'c' vagy 'd').\n");
                continue;
            }

            string imageUrl = await GetAnimalImage(url);
            Console.WriteLine($"Kép URL: {imageUrl}\n");

            OpenInBrowser(imageUrl);
        }
    }

    static async Task<string> GetAnimalImage(string apiUrl)
    {
        var client = new RestClient(apiUrl);
        var request = new RestRequest("", Method.Get);

        var response = await client.ExecuteAsync(request);

        if (!response.IsSuccessful)
            return "[Hiba a kérés során]";

        var json = JToken.Parse(response.Content);

        if (apiUrl.Contains("thecatapi"))
            return json[0]?["url"]?.ToString() ?? "[Nem talált kép]";
        else
            return json["message"]?.ToString() ?? "[Nem talált kép]";
    }

    static void OpenInBrowser(string url)
    {
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                Process.Start("open", url);
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                Process.Start("xdg-open", url);
            else
                Console.WriteLine("Nem támogatott operációs rendszer.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Nem sikerült megnyitni a képet: {ex.Message}");
        }
    }
}
