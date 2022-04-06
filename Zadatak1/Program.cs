using System;
using RestSharp;
using System.Text.Json;
using System.IO;
using System.Collections.Generic;
using System.Globalization;



namespace Aplikacija1
{
    class Program
    {
        static void get_all_curencies()
        {
            var client = new RestClient("https://api.exchange.coinbase.com");
            var request = new RestRequest("currencies", Method.Get);

            request.AddHeader("Accept", "application/json");

            var result = client.ExecuteAsync(request).Result;

            if (result.IsSuccessful)
            {
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    File.WriteAllText("file4.txt", result.Content);
                }
                else
                {
                    Console.Error.WriteLine("[API Error] Status code: {0}", result.StatusCode);
                }
            }
            else
            {
                Console.Error.WriteLine("[API Error] Rest error: {0}", result.ErrorMessage);
            }
        }
        

        public class Ticker
        {
            public string ask { get; set; }
            public string bid { get; set; }
            public string volume { get; set; }
            public ulong trade_id { get; set; }
            public string price { get; set; }
            public string size { get; set; }
            public string time { get; set; }
        }

        static void GetTicker(string product_id)
        {
            var client = new RestClient("https://api.exchange.coinbase.com");
            var request = new RestRequest("products/{product_id}/ticker").AddUrlSegment("product_id", product_id);
            request.AddHeader("Accept", "application/json");

            var result = client.ExecuteAsync(request).Result;

            if (result.IsSuccessful)
            {
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    try
                    {
                        Ticker ticker = JsonSerializer.Deserialize<Ticker>(result.Content);
                        string[] data = new string[] { ticker.bid, ticker.size };
                        File.WriteAllLines("file2.txt", data);
                    }
                    catch (JsonException ex)
                    {
                        Console.Error.WriteLine("[API Error] JSon Exception: {0}", ex.Message);
                    }
                }
                else
                {
                    Console.Error.WriteLine("[API Error] Status code: {0}", result.StatusCode);
                }
            }
            else
            {
                Console.Error.WriteLine("[API Error] Rest error: {0}", result.ErrorMessage);
            }
        }


        class Trade
        {
            public string time { get; set; }
            public ulong trade_id { get; set; }
            public string price { get; set; }
            public string size { get; set; }
            public string side { get; set; }
        }

        static void GetResult(string product_id)
        {
            var client = new RestClient("https://api.exchange.coinbase.com");
            var request = new RestRequest("/products/{p_id}/trades").AddUrlSegment("p_id", product_id);
            request.AddHeader("Accept", "application/json");
            string response = client.ExecuteAsync(request).Result.Content;
            var result = client.ExecuteAsync(request).Result;
            if (result.IsSuccessful)
            {
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    try
                    {
                        CultureInfo culture = new CultureInfo("en-US");

                        var trades = JsonSerializer.Deserialize<List<Trade>>(response);

                        decimal sum_size = 0.0m;
                        decimal sum_price = 0.0m;
                        foreach (var trade in trades)
                        {
                            sum_size += decimal.Parse(trade.size, culture);
                            sum_price += decimal.Parse(trade.price, culture);
                        }
                        decimal avg_price = sum_price / trades.Count;
                        Console.WriteLine("Sum of size:{0}\nAverage price:{1}", sum_size, avg_price);
                    }
                    catch (JsonException ex)
                    {
                        Console.Error.WriteLine("[API Error] JSon Exception: { 0}", ex.Message);
                    }
                }
                else
                {
                    Console.Error.WriteLine("[API Error] Status code: {0}", result.StatusCode);
                }
            }
            else
            {
                Console.Error.WriteLine("[API Error] Rest error: {0}", result.ErrorMessage);
            }
        }

        static void Main(string[] args)
        {
            get_all_curencies();

            GetTicker("ETH-USD");

            GetResult("ETH-USD");
        }
    }
}
