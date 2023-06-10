using CryptoInfoViewer.Models;
using CryptoInfoViewer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CryptoInfoViewer.Views
{
    /// <summary>
    /// Interaction logic for ConvertCryptoWindow.xaml
    /// </summary>
    public partial class ConvertCryptoWindow : Window
    {
        private CryptoService cryptoService;
        public ConvertCryptoWindow()
        {
            cryptoService = new CryptoService();
            InitializeComponent();
            LoadCurrencies();
        }
        private async void LoadCurrencies()
        {
            List<Rates> rates = await cryptoService.GetRates();
            SourceCurrencyComboBox.ItemsSource = rates;
            TargetCurrencyComboBox.ItemsSource = rates;
        }


        private async void Convert_Click(object sender, RoutedEventArgs e)
        {
            string sourceCurrency = ((Rates)SourceCurrencyComboBox.SelectedItem)?.id;
            string targetCurrency = ((Rates)TargetCurrencyComboBox.SelectedItem)?.id; 

            if (string.IsNullOrEmpty(sourceCurrency) || string.IsNullOrEmpty(targetCurrency))
            {
                MessageBox.Show("Please select both source and target currencies.");
                return;
            }

            if (!decimal.TryParse(AmountTextBox.Text, out decimal amount))
            {
                MessageBox.Show("Invalid amount.");
                return;
            }

            decimal convertedAmount = await ConvertCryptoCurrency(amount, sourceCurrency, targetCurrency);

            ResultLabel.Content = $"{amount} {sourceCurrency} = {convertedAmount} {targetCurrency}";
        }

        public async Task<decimal> ConvertCryptoCurrency(decimal amount, string sourceCurrency, string targetCurrency)
        {
            List<Rates> rates = await cryptoService.GetRates();

            Rates sourceRate = rates.FirstOrDefault(r => r.id == sourceCurrency);
            if (sourceRate == null)
            {
                MessageBox.Show("Exchange rate for the source currency not found.");
                return 0m;
            }

            Rates targetRate = rates.FirstOrDefault(r => r.id == targetCurrency);
            if (targetRate == null)
            {
                MessageBox.Show("Exchange rate for the target currency not found.");
                return 0m;
            }

            decimal amountInUSD = amount / sourceRate.rateUsd;
            decimal amountInTargetCurrency = amountInUSD * targetRate.rateUsd;

            return amountInTargetCurrency;
        }
    }
}
