using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Newtonsoft.Json; // DOWNLOAD FROM NUGET
// ADD Costura.Fody for Embedding Dll References

namespace LinkvertiseBypass
{
    class Program
    {
        private static CultureInfo ci = CultureInfo.CurrentCulture;
        static void Main(string[] args)
        {
            Console.Title = "Linkvertise Bypass";
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("\n  Write URL : ");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            string[] result = Bypass(Console.ReadLine());
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n  Input URL : " + result[0]);
            Console.WriteLine("  URL       : " + result[1]);
            //Console.WriteLine("  URL, Copied to clipboard");                                                                                              ┌> Form
            //Clipboard.SetText(result[1]); // System.Threading.ThreadStateException: - Solution for WinForms : Control.CheckForIllegalCrossThreadCalls = false;
            Console.ReadLine();
        }

        static Random random = new Random();
        public static string GetRandomEnding()
        {
            return string.Format("{0}.{1}.{2}.{3}", new object[]
            {
                random.Next(1, 99),
                random.Next(0, 99),
                random.Next(0, 99),
                random.Next(0, 99)
            });
        }

        public static string[] Bypass(string url)
        {
            string resultUrl = url.Replace("https://linkvertise.com/", string.Empty);
            resultUrl = resultUrl.Replace("https://up-to-down.net/", string.Empty);
            resultUrl = resultUrl.Replace("https://link-to.net/", string.Empty);
            resultUrl = resultUrl.Replace("https://direct-link.net/", string.Empty);
            resultUrl = resultUrl.Replace("https://file-link.net", string.Empty);
            resultUrl = resultUrl.Replace("?o=sharing", string.Empty);

            string randomProxy = GetRandomEnding();
            randomProxy = "/" + randomProxy.Replace(".", string.Empty);
            string address = @"https://beautiful-code.glasstea.repl.co/" + resultUrl + randomProxy;
            try
            {
                string[] results = new string[3];
                WebClient wClient = new WebClient();
                string value = wClient.DownloadString(address);
                API apiResult = JsonConvert.DeserializeObject<API>(value);

                if(apiResult.NewLink.Contains("invalid")) // If return error occurs, try again
                {
                    Bypass(url);
                }
                results[0] = apiResult.InputLink;
                results[1] = (ci.Name == "tr-TR") ? (results[1] = apiResult.NewLink.Replace("pastebin.com", "pastebinp.com")) : (results[1] = apiResult.NewLink); // cuz pastebin forbidden in Turkey :(
                results[2] = apiResult.TimesTried.ToString();
                return results;
            }
            catch
            {
                Bypass(url);
                return new string[] { "ERROR"}; // -> Not returnable
            }
        }

        #region Result
        public class API
        {
            [JsonProperty("input_link")]
            public string InputLink { get; set; }

            [JsonProperty("new_link")]
            public string NewLink { get; set; }

            [JsonProperty("times_tried")]
            public int TimesTried { get; set; }
        }

        #endregion
    }
}
