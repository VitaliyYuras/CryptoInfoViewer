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
        private readonly HttpClient httpClient;

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
        public async Task<CryptoCurrency?> GetCryptoCurrencyById( string id)
        {
            try
            {
                string url = string.Format("https://api.coincap.io/v2/assets/{0}", id);
                HttpResponseMessage response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    dynamic result = JsonConvert.DeserializeObject(jsonResponse);

                    CryptoCurrency currency = ParseCryptoCurrency(result.data);

                    return currency;
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex);
            }

            return null;
        }
        //Метод пошуку криптовалюти за ім'ям або символом
        public async Task<List<CryptoCurrency>?> SearchCurrencies(string searchTerm)
        {
            try
            {

                List<CryptoCurrency> cryptoCurrencies = await GetCryptoCurrencies();

                if (cryptoCurrencies != null)
                {
                    List<CryptoCurrency> filteredCurrencies =cryptoCurrencies
                        .Where(c => c.name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                                    || c.symbol.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    return filteredCurrencies;
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex);
            }

            return null;
        }

        //Отримання даних для побудови японської свічкової діаграми
        public async Task<List<CandleData>?> GetDataFromApi(string symbol, string days, string currency)
        {
            try
            {
                string apiUrl = $"https://api.coingecko.com/api/v3/coins/{symbol}/ohlc?vs_currency={currency}&days={days}";
                HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    JArray result = JArray.Parse(jsonResponse);

                    List<CandleData> cryptoData = new List<CandleData>();
                    foreach (JArray candle in result)
                    {
                        decimal time = (decimal)candle[0];
                        decimal open = (decimal)candle[1];
                        decimal high = (decimal)candle[2];
                        decimal low = (decimal)candle[3];
                        decimal close = (decimal)candle[4];

                        CandleData currencyData = new CandleData
                        {
                            time = time,
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
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex);
            }

            return null;

        }

        //Отримання цін для обміну всі криптовалют
        public async Task<List<Rates>?> GetRates()
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
                        Rates rates = ParseRates(crypto);

                        ratesList.Add(rates);
                    }


                    return ratesList;
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex);
            }

            return null;
        }

        // Отримання всіх криптовалют
        public async Task<List<CryptoCurrency>?> GetCryptoCurrencies()
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
                        CryptoCurrency currency = ParseCryptoCurrency(crypto);
                        cryptoCurrencies.Add(currency);
                    }

                    return cryptoCurrencies;
                }
            }
            catch (Exception ex)
            {

                ShowErrorMessage(ex);
            }

            return null;
        }

        //Отримання ринків де можна купити дану криптовалюту

        public async Task<List<CryptoMarkets>?> GetMarkets(string id)
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
                        CryptoMarkets markets = ParseCryptoMarkets(crypto);
                        cryptoMarkets.Add(markets);
                    }

                    return cryptoMarkets;
                }
            }
            catch (Exception ex)
            {

                ShowErrorMessage(ex);
            }

            return null;
        }
        private CryptoCurrency ParseCryptoCurrency(dynamic data)
        {
            string id = data.id;
            string name = data.name;
            string symbol = data.symbol;
            int rank = data.rank;
            decimal supply = data.supply;
            decimal priceUsd = data.priceUsd;

            CryptoCurrency currency = new CryptoCurrency
            {
                id = id,
                name = name,
                symbol = symbol,
                rank = rank,
                supply = supply,
                priceUsd = priceUsd
            };

            return currency;
        }

        private Rates ParseRates(dynamic data)
        {
            string id = data.id;
            string symbol = data.symbol;
            string currencySymbol = data.currencySymbol;
            string type = data.type;
            decimal rateUsd = data.rateUsd;

            Rates rates = new Rates
            {
                id = id,
                symbol = symbol,
                currencySymbol = currencySymbol,
                type = type,
                rateUsd = rateUsd
            };

            return rates;
        }

        private CryptoMarkets ParseCryptoMarkets(dynamic data)
        {
            string exchangeId = data.exchangeId;
            string quoteSymbol = data.quoteSymbol;
            decimal priceUsd = data.priceUsd;

            CryptoMarkets markets = new CryptoMarkets
            {
                exchangeId = exchangeId,
                quoteSymbol = quoteSymbol,
                priceUsd = priceUsd
            };

            return markets;
        }
        private void ShowErrorMessage(Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}");
        }


    }
    
}
