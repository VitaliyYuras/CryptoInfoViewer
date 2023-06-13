using CryptoInfoViewer.Models;
using Speckle.Newtonsoft.Json;
using Speckle.Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace CryptoInfoViewer.Services
{
    public class CryptoService
    {
        private HttpClient httpClient;

        public CryptoService()
        {
            httpClient = new HttpClient();
        }

        // Отримання топ 25 криптовалют за ціною
        public async Task<List<CryptoCurrency>> GetTop25CryptoCurrencies()
        {
            try
            {
                List<CryptoCurrency> сryptoCurrencies = await GetCryptoCurrencies();
                List<CryptoCurrency> top25CryptoCurrencies = сryptoCurrencies
                        .OrderByDescending(c => c.priceUsd)
                        .Take(25)
                        .ToList();
                   

                    return top25CryptoCurrencies;
                
            }
            catch (Exception ex)
            {
                
                MessageBox.Show($"Error: {ex.Message}");
            }

            return null;
        }
        //Отримання криптовалюти за id
        public async Task<CryptoCurrency> GetCryptoCurrencies( string id)
        {
            try
            {
                string url = string.Format("https://api.coincap.io/v2/assets/{0}", id);
                HttpResponseMessage response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    dynamic result = JsonConvert.DeserializeObject(jsonResponse);
 
                    string name = result.data.name;
                    string symbol = result.data.symbol;
                    int rank = result.data.rank;
                    decimal supply = result.data.supply;
                    decimal? maxSupply = result.data.maxSupply != null ? (decimal?)result.data.maxSupply : null;
                    decimal? marketCapUsd = result.data.marketCapUsd != null ? (decimal?)result.data.marketCapUsd : null;
                    decimal? volumeUsd24Hr = result.data.volumeUsd24Hr != null ? (decimal?)result.data.volumeUsd24Hr : null;
                    decimal? priceUsd = result.data.priceUsd != null ? (decimal?)result.data.priceUsd : null;
                    decimal? changePercent24Hr = result.data.changePercent24Hr != null ? (decimal?)result.data.changePercent24Hr : null;
                    decimal? vwap24Hr = result.data.vwap24Hr != null ? (decimal?)result.data.vwap24Hr : null;
                    string? explorer = result.data.explorer;


                    CryptoCurrency currency = new CryptoCurrency
                    {
                        id = id,
                        name = name,
                        symbol = symbol,
                        rank = rank,
                        supply = supply,
                        maxSupply=maxSupply,
                        marketCapUsd = marketCapUsd,
                        volumeUsd24Hr=volumeUsd24Hr,
                        priceUsd = priceUsd,
                        changePercent24Hr=changePercent24Hr,
                        vwap24Hr=vwap24Hr,
                        explorer=explorer

                    };

                    return currency;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }

            return null;
        }
        //Метод пошуку криптовалюти за ім'ям або символом
        public async Task<List<CryptoCurrency>> SearchCurrencies(string searchTerm)
        {
            try
            {

                List<CryptoCurrency> top10CryptoCurrencies = await GetCryptoCurrencies();

                if (top10CryptoCurrencies != null)
                {
                    List<CryptoCurrency> filteredCurrencies = top10CryptoCurrencies
                        .Where(c => c.name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                                    || c.symbol.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    return filteredCurrencies;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }

            return null;
        }

        //Отримання даних для побудови японської свічкової діаграми
        //public async Task<List<CandleData>> GetDataFromApi(string exchange,string interval,string baseId,string quoteId)
        //{
        //    using (HttpClient client = new HttpClient())
        //    {
        //        string apiUrl = "https://api.coingecko.com/api/v3/coins/bitcoin/ohlc?vs_currency=usd&days=7";
        //        HttpResponseMessage response = await client.GetAsync(apiUrl);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            string jsonResponse = await response.Content.ReadAsStringAsync();
        //            dynamic result = JsonConvert.DeserializeObject(jsonResponse);
        //            List<CandleData> cryptoData = new List<CandleData> ();
        //            foreach (var crypto in result)
        //            {

        //                decimal open= crypto.open;
        //                decimal high = crypto.high;
        //                decimal low = crypto.low;
        //                decimal close = crypto.close;

        //                CandleData currency = new CandleData
        //                {
        //                    open=open,
        //                    high=high,
        //                    low=low,
        //                    close=close,


        //                };

        //                cryptoData.Add(currency);
        //            }
        //            return cryptoData;

        //        }
        //        else
        //        {
        //            MessageBox.Show("Failed to retrieve data from API");
        //            return null;
        //        }
        //    }
        //}
        public async Task<List<CandleData>> GetDataFromApi(string symbol, string days, string currency)
        {
            using (HttpClient client = new HttpClient())
            {
                string apiUrl = $"https://api.coingecko.com/api/v3/coins/{symbol}/ohlc?vs_currency={currency}&days={days}";
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    JArray result = JArray.Parse(jsonResponse);

                    List<CandleData> cryptoData = new List<CandleData>();
                    foreach (JArray candle in result)
                    {
                       
                        decimal open = (decimal)candle[1];
                        decimal high = (decimal)candle[2];
                        decimal low = (decimal)candle[3];
                        decimal close = (decimal)candle[4];

                        CandleData currencyData = new CandleData
                        {
                            
                            open = open,
                            high = high,
                            low = low,
                            close = close
                        };

                        cryptoData.Add(currencyData);
                    }

                    return cryptoData;
                }
                else
                {
                    MessageBox.Show("Failed to retrieve data from API");
                    return null;
                }
            }
        }

        //Отримання цін для обміну всі криптовалют
        public async Task<List<Rates>> GetRates()
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync("https://api.coincap.io/v2/rates/");

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    dynamic result = JsonConvert.DeserializeObject(jsonResponse);

                    List<Rates> ratesList = new List<Rates>();

                    foreach (var crypto in result.data)
                    {
                        string id = crypto.id;
                        string symbol = crypto.symbol;
                        string currencySymbol = crypto.currencySymbol;
                        string type = crypto.type;
                        decimal rateUsd = crypto.rateUsd;
                        Rates rates = new Rates
                        {
                            id = id,
                            symbol = symbol,
                            currencySymbol = currencySymbol,
                            type = type,
                            rateUsd = rateUsd

                        };

                        ratesList.Add(rates);
                    }


                    return ratesList;
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show($"Error: {ex.Message}");
            }

            return null;
        }

        // Отримання всіх криптовалют
        public async Task<List<CryptoCurrency>> GetCryptoCurrencies()
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync("https://api.coincap.io/v2/assets/");

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    dynamic result = JsonConvert.DeserializeObject(jsonResponse);

                    List<CryptoCurrency> cryptoCurrencies = new List<CryptoCurrency>();

                    foreach (var crypto in result.data)
                    {
                        string id = crypto.id;
                        string name = crypto.name;
                        string symbol = crypto.symbol;
                        int rank = crypto.rank;
                        decimal supply = crypto.supply;

                        decimal priceUsd = crypto.priceUsd;


                        CryptoCurrency currency = new CryptoCurrency
                        {
                            id = id,
                            name = name,
                            symbol = symbol,
                            rank = rank,
                            supply = supply,
                            priceUsd = priceUsd

                        };

                        cryptoCurrencies.Add(currency);
                    }

                    return cryptoCurrencies;
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show($"Error: {ex.Message}");
            }

            return null;
        }

        //Отримання ринків де можна купити дану криптовалюту

        public async Task<List<CryptoMarkets>> GetMarkets(string id)
        {
            try
            {
                string url = string.Format("https://api.coincap.io/v2/assets/{0}/markets", id);
                HttpResponseMessage response = await httpClient.GetAsync(url);
               

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    dynamic result = JsonConvert.DeserializeObject(jsonResponse);

                    List<CryptoMarkets> cryptoMarkets = new List<CryptoMarkets>();

                    foreach (var crypto in result.data)
                    {
                        
                        string exchangeId = crypto.exchangeId;
                        string quoteSymbol = crypto.quoteSymbol;
                        decimal priceUsd = crypto.priceUsd;


                        CryptoMarkets currency = new CryptoMarkets
                        {
                            exchangeId = exchangeId,
                            quoteSymbol=quoteSymbol,
                            priceUsd = priceUsd

                        };

                        cryptoMarkets.Add(currency);
                    }

                    return cryptoMarkets;
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show($"Error: {ex.Message}");
            }

            return null;
        }


    }
    
}
