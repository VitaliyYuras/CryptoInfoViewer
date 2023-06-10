using CryptoInfoViewer.Models;
using Speckle.Newtonsoft.Json;
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

        public async Task<List<CryptoCurrency>> GetTop10CryptoCurrencies()
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
                            id=id,
                            name = name,
                            symbol = symbol,
                            rank = rank,
                            supply = supply,
                            priceUsd = priceUsd
                           
                        };

                        cryptoCurrencies.Add(currency);
                    }

                    List<CryptoCurrency> top10CryptoCurrencies = cryptoCurrencies
                        .OrderByDescending(c => c.priceUsd)
                        .Take(25)
                        .ToList();
                   

                    return top10CryptoCurrencies;
                }
            }
            catch (Exception ex)
            {
                
                MessageBox.Show($"Error: {ex.Message}");
            }

            return null;
        }
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
                    //string explorer = result.data.explorer;
                    string? explorer = result.data.explorer != null ? $"<Hyperlink NavigateUri='{result.data.explorer}'>Explorer Link</Hyperlink>" : null;


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

        public async Task<List<CandleData>> GetDataFromApi(string exchange,string interval,string baseId,string quoteId)
        {
            using (HttpClient client = new HttpClient())
            {
                string apiUrl = "https://api.coincap.io/v2/candles?exchange={exchange}&interval={interval}&baseId={basedId}&quoteId={quoteId}";
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    dynamic result = JsonConvert.DeserializeObject(jsonResponse);
                    List<CandleData> cryptoData = new List<CandleData> ();
                    foreach (var crypto in result.data)
                    {
                        
                        decimal open= crypto.open;
                        decimal high = crypto.high;
                        decimal low = crypto.low;
                        decimal close = crypto.close;
                        decimal volume = crypto.volume;
                        decimal period = crypto.period;
                        CandleData currency = new CandleData
                        {
                            open=open,
                            high=high,
                            low=low,
                            close=close,
                            volume=volume,
                            period=period

                        };

                        cryptoData.Add(currency);
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

    }
    
}
