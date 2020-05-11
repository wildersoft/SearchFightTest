using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;

using SearchFight.BusinessLogic.Model;

namespace SearchFight
{
    class Program
    {
        private static IConfigurationRoot configuration { get; }
        private static HttpClient httpClient { get; }
        private static bool keepRunning { get; set;  } = true;

        /// <summary>
        /// static constructor
        /// </summary>
        static Program()
        {
            Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e)
            {
                Console.WriteLine("preparing to exit...");
                e.Cancel = true;
                keepRunning = false;
                Environment.Exit(0);
            };

            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();

            string api = configuration["ApiUrl"];
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(api);
        }

        static void Main(string[] args)
        {
            while (keepRunning)
            {
                List<SearchResult> results = Search(ref args);

                var groupResults = results.GroupBy(item => item.ProgrammingLanguage,
                    (key, g) => new
                    {
                        ProgrammingLanguage = key,
                        SearchEngineRating = g.Select(item => new { item.SearchEngine, item.Rating })
                    })
                .Select
                (
                    item => new
                    {
                        item.ProgrammingLanguage,
                        Results = item.SearchEngineRating
                            .Select(subItem => $"{subItem.SearchEngine} : {subItem.Rating}")
                            .Join(" ")
                    }
                )
                .ToList();

                Console.WriteLine("------------------Results------------------");

                groupResults.ForEach(
                    item =>
                    {
                        Console.WriteLine($"{item.ProgrammingLanguage} : {item.Results}");
                    }
                );

                var searchEngineWinners = results
                    .Where(item =>
                        item.Rating ==
                            results
                                .Where(subItem => subItem.SearchEngine == item.SearchEngine)
                                .Max(subItem => subItem.Rating)
                    )
                    .GroupBy(item => new
                    {
                        item.ProgrammingLanguage,
                        item.SearchEngine,
                        item.Rating
                    })
                    .Select(item => new
                    {
                        item.Key.SearchEngine,
                        ProgrammingLanguage = item.Key.ProgrammingLanguage,
                        Rating = item.Key.Rating
                    })
                    .ToList();

                searchEngineWinners.ForEach(item => {
                    Console.WriteLine($"{item.SearchEngine} winner: {item.ProgrammingLanguage}");
                });

                string totalWinner = searchEngineWinners
                    .SingleOrDefault(item => item.Rating == searchEngineWinners.Max(subItem => subItem.Rating))
                        .ProgrammingLanguage;

                Console.WriteLine($"Total winner: {totalWinner}");

                Console.WriteLine("------------------End------------------");
                Console.WriteLine(string.Empty);
            }
        }

        /// <summary>
        /// Request new user entry to launch the search
        /// </summary>
        /// <param name="args">contains programming languages</param>
        /// <returns></returns>
        private static List<SearchResult> Search(ref string[] args)
        {
            string[] results = null;
            List<SearchResult> result;

            if (args == null || args.Length == 0)
            {
                Console.WriteLine("Enter a program language for search...");
                results = splitParameters(Console.ReadLine());
            }
            else
            {
                results = splitParameters(args);
                args = null;
            }

            result = SendRequest(results).Result;
            return result;
        }

        /// <summary>
        /// Call api
        /// </summary>
        /// <param name="args">contains the programming languages</param>
        /// <returns>List of results</returns>
        static async Task<List<SearchResult>> SendRequest(string[] args)
        {
            string serialisedData = JsonConvert.SerializeObject(args);
            var content = new StringContent(serialisedData, Encoding.UTF8, "application/json");
            HttpResponseMessage responseMessage = await httpClient.PostAsync($"SearchFight/Search", content);

            if (responseMessage == null || responseMessage.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            string dataResult = await responseMessage.Content.ReadAsStringAsync();
            List<SearchResult> lstResult = JsonConvert.DeserializeObject<List<SearchResult>>(dataResult);

            return lstResult;
        }

        /// <summary>
        /// Join the array to a single line
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns>call to overload method</returns>
        public static string[] splitParameters(string[] parameters)
        {
            string result = string.Join(" ", parameters);
            return splitParameters(result);
        }

        /// <summary>
        /// Split single-line parameters with regular expression validation to avoid splitting words in quotes
        /// </summary>
        /// <param name="parameters">single-line parameters with programming languages</param>
        /// <returns>array of programming languages</returns>
        public static string[] splitParameters(string parameters)
        {
            Regex re = new Regex("(?<=\")[^\"]*(?=\")|[^\" ]+");
            var result = re.Matches(parameters).Cast<Match>().Select(m => m.Value).ToArray();
            return result;
        }
    }
}