using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace LinkvertiseBypass
{
    public static class Program
    {
        private static readonly CultureInfo ci = CultureInfo.CurrentCulture;

        private static readonly Regex LinkRegex = new Regex(
            @"(https:\/\/linkvertise\.com\/|https:\/\/up-to-down\.net\/|https:\/\/link-to\.net\/|https:\/\/direct-link\.net\/|https:\/\/file-link\.net\/|\?o=sharing)");

        private static void Main()
        {
            MainAsync().GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
        {
            Console.Title = "Linkvertise Bypass";
            Console.ForegroundColor = ConsoleColor.White;

            Console.Write("Write Url: ");
            Console.ForegroundColor = ConsoleColor.DarkGray;

            API result = await BypassAsync(Console.ReadLine(), true);
            Console.ForegroundColor = ConsoleColor.Green;

            if (result.Url.Contains("invalid") | result is null)
            {
                Console.WriteLine($"Failed! Please try again.");
            }
            else
            {
                string link = result.Url;

                if (ci.Name == "tr-TR")
                {
                    link = link.Replace("pastebin.com", "pastebinp.com");
                }

                ClipboardSetText(link);

                Console.WriteLine($"Input Url: {result.InputUrl}");
                Console.WriteLine($"Url: {link}");
                Console.WriteLine($"\nCopied to clipboard!");
            }

            await Task.Delay(-1);
        }

        #region Methods

        private static void ClipboardSetText(string text)
        {
            Thread clipboardThread = new Thread(() => ClipboardThreadWorker(text));
            clipboardThread.SetApartmentState(ApartmentState.STA);
            clipboardThread.IsBackground = false;

            clipboardThread.Start();
        }

        private static void ClipboardThreadWorker(string text)
        {
            Clipboard.SetText(text);
        }

        public static string Remove(this string remove, string replace)
        {
            return remove.Replace(replace, string.Empty);
        }

        public static string Base64Encode(this string plainText)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static async Task<string> GetAsync(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.Method = "GET";

            using HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
            using Stream stream = response.GetResponseStream();
            using StreamReader reader = new StreamReader(stream);

            return await Task.FromResult(await reader.ReadToEndAsync());
        }

        #endregion Methods

        public static async Task<API> BypassAsync(string url, bool text)
        {
            if (text is true)
            {
                bool complete = false;
                API apiResult = null;

                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;

                _ = Task.Run(async () =>
                {
                    Random Random = new Random();

                    string resultUrl = LinkRegex.Replace(url, string.Empty);

                    string randomProxy = string.Format($"{0}{1}{2}{3}", new int[]
                    {
                        Random.Next(1, 99),
                        Random.Next(0, 99),
                        Random.Next(0, 99),
                        Random.Next(0, 99)
                    });

                    try
                    {
                        string value = await GetAsync($"https://beautiful-code.glasstea.repl.co/{resultUrl}/{randomProxy}");
                        apiResult = JsonSerializer.Deserialize<API>(value);
                        complete = true;
                    }
                    catch
                    {
                        complete = true;
                    }
                });

                while (complete is false)
                {
                    Console.Write("\rBypassing.");
                    await Task.Delay(100);
                    Console.Write("\rBypassing..");
                    await Task.Delay(100);
                    Console.Write("\rBypassing...");
                    await Task.Delay(100);
                }

                Console.Clear();
                return apiResult;
            }
            else
            {
                Random Random = new Random();

                string resultUrl = LinkRegex.Replace(url, string.Empty);

                string randomProxy = string.Format($"{0}{1}{2}{3}", new int[]
                {
                Random.Next(1, 99),
                Random.Next(0, 99),
                Random.Next(0, 99),
                Random.Next(0, 99)
                });

                try
                {
                    string value = await GetAsync($"https://beautiful-code.glasstea.repl.co/{resultUrl}/{randomProxy}");
                    API apiResult = JsonSerializer.Deserialize<API>(value);

                    return apiResult;
                }
                catch
                {
                    return null;
                }
            }
        }

        public class API
        {
            [JsonPropertyName("input_link")]
            public string InputUrl { get; set; }

            [JsonPropertyName("new_link")]
            public string Url { get; set; }

            [JsonPropertyName("times_tried")]
            public int TimesTried { get; set; }
        }
    }
}