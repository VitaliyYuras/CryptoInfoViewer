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
                HttpResponseMessage response = await httpClient.GetAsync("https://api.coincap.io/v2/assets");

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    dynamic result = JsonConvert.DeserializeObject(jsonResponse);

                    List<CryptoCurrency> cryptoCurrencies = new List<CryptoCurrency>();

                    foreach (var crypto in result.data)
                    {
                        
                        string name = crypto.name;
                        string symbol = crypto.symbol;
                        int rank = crypto.rank;
                        decimal supply = crypto.supply;
                        
                        decimal priceUsd = crypto.priceUsd;
                        

                        CryptoCurrency currency = new CryptoCurrency
                        {
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
    }
}
