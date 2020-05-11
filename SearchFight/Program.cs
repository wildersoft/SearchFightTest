using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using log4net;
using log4net.Config;
using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;

using SearchFight.Model;

namespace SearchFight
{
    class Program
    {
        private static IConfigurationRoot configuration { get; }
        private static HttpClient httpClient { get; }
        private static bool keepRunning { get; set;  } = true;
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));


        /// <summary>
        /// static constructor
        /// </summary>
        static Program()
        {
            Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e)
            {
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

            XmlDocument log4netConfig = new XmlDocument();
            log4netConfig.Load(File.OpenRead("log4net.config"));
            var repo = log4net.LogManager.CreateRepository(Assembly.GetEntryAssembly(),
                       typeof(log4net.Repository.Hierarchy.Hierarchy));
            log4net.Config.XmlConfigurator.Configure(repo, log4netConfig["log4net"]);
        }

        static void Main(string[] args)
        {
            try
            {
                while (keepRunning)
                {
                    List<SearchResult> results = Search(ref args);
                    //group results to single line
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
                            Results = string.Join(" ", item.SearchEngineRating.Select(subItem => $"{subItem.SearchEngine} : {subItem.Rating}").ToArray())
                        }
                    )
                    .ToList();

                    Console.WriteLine("------------------Results------------------");
                    //print group results
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
                    //print programming language winner
                    searchEngineWinners.ForEach(item =>
                    {
                        Console.WriteLine($"{item.SearchEngine} winner: {item.ProgrammingLanguage}");
                    });

                    string totalWinner = searchEngineWinners
                        .SingleOrDefault(item => item.Rating == searchEngineWinners.Max(subItem => subItem.Rating))
                            .ProgrammingLanguage;
                    //printing total winner
                    Console.WriteLine($"Total winner: {totalWinner}");
                    Console.WriteLine("------------------End------------------");
                    Console.WriteLine(string.Empty);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("some problem was found, please try later");
                log.Warn(ex);
            }
        }

        /// <summary>
        /// Request new user entry to launch the search
        /// </summary>
        /// <param name="args">contains programming languages</param>
        /// <returns></returns>
        private static List<SearchResult> Search(ref string[] args)
        {
            string[] results = { };
            List<SearchResult> result;

            if (args == null || args.Length == 0)
            {
                Console.WriteLine("Enter some programming languages for search the popularity...");
                string newParams = Console.ReadLine();
                results = splitParameters(newParams);
                result = SendRequest(results).Result;
            }
            else
            {
                result = SendRequest(args).Result;
                args = null;
            }

            return result;
        }

        /// <summary>
        /// Call api
        /// </summary>
        /// <param name="args">contains the programming languages</param>
        /// <returns>List of results</returns>
        private static async Task<List<SearchResult>> SendRequest(string[] args)
        {
            try
            {
                string serialisedData = JsonConvert.SerializeObject(args);
                HttpContent content = new StringContent(serialisedData, Encoding.UTF8, "application/json");
                HttpResponseMessage responseMessage = await httpClient.PostAsync("SearchFight/Search", content);

                if (responseMessage == null || responseMessage.StatusCode != HttpStatusCode.OK)
                {
                    return null;
                }

                string dataResult = await responseMessage.Content.ReadAsStringAsync();
                List<SearchResult> lstResult = JsonConvert.DeserializeObject<List<SearchResult>>(dataResult);

                return lstResult;
            }
            catch (Exception ex)
            {
                log.Warn(ex);
                return null;
            }
        }

        /// <summary>
        /// Split single-line parameters with regular expression validation to avoid splitting words in quotes
        /// </summary>
        /// <param name="parameters">single-line parameters with programming languages</param>
        /// <returns>array of programming languages</returns>
        private static string[] splitParameters(string parameters)
        {
            try
            {
                Regex re = new Regex("(?<=\")[^\"]*(?=\")|[^\" ]+");
                var result = re.Matches(parameters).Cast<Match>().Select(m => m.Value).ToArray();
                return result;
            }
            catch (Exception ex)
            {
                return null;
                log.Warn(ex);
            }
        }
    }
}