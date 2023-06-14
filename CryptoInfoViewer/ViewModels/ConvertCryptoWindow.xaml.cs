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
        // Завантиаження  криптовалют для конвертування
        private async void LoadCurrencies()
        {
            List<Rates> rates = await cryptoService.GetRates();
            SourceCurrencyComboBox.ItemsSource = rates;
            TargetCurrencyComboBox.ItemsSource = rates;
        }

        // Кнопка для конверутвання криптовалют
        private async void Convert_Click(object sender, RoutedEventArgs e)
        {
            string? sourceCurrency = GetSelectedCurrencyId(SourceCurrencyComboBox);
            string? targetCurrency = GetSelectedCurrencyId(TargetCurrencyComboBox);

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
        private string? GetSelectedCurrencyId(ComboBox comboBox)
        {
            return (comboBox.SelectedItem as Rates)?.id;
        }

        //Метод для конвертації криптовалют
        public async Task<decimal> ConvertCryptoCurrency(decimal amount, string sourceCurrency, string targetCurrency)
        {
            List<Rates> rates = await cryptoService.GetRates();

            Rates? sourceRate = GetRateById(rates, sourceCurrency);
            if (sourceRate == null)
            {
                throw new Exception("Exchange rate for the source currency not found.");
            }

            Rates? targetRate = GetRateById(rates, targetCurrency);
            if (targetRate == null)
            {
                throw new Exception("Exchange rate for the target currency not found.");
            }

            decimal amountInUSD = amount / sourceRate.rateUsd;
            decimal amountInTargetCurrency = amountInUSD * targetRate.rateUsd;

            return amountInTargetCurrency;
        }
        private Rates? GetRateById(List<Rates> rates, string currencyId)
        {
            return rates.FirstOrDefault(r => r.id == currencyId);
        }

        // Забороняєм ввід не числових значень
        private void AmountTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
            }
        }
    }
}
