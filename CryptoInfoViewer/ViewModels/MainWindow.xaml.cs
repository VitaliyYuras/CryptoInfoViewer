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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Http;
using System.Diagnostics;
using Speckle.Newtonsoft.Json;
using CryptoInfoViewer.Services;
using CryptoInfoViewer.Models;
using CryptoInfoViewer.Views;

namespace CryptoInfoViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>'

    
    public partial class MainWindow : Window
    {
        private CryptoService cryptoService;
        private string searchTerm;
        public MainWindow()
        {
            cryptoService = new CryptoService();
            InitializeComponent();
            LoadData();
        }
        public async void LoadData()
        {
            try
            {
                List<CryptoCurrency> top10CryptoCurrencies = await cryptoService.GetTop25CryptoCurrencies();

                MyListBox.ItemsSource = top10CryptoCurrencies;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void Details_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string id = button.CommandParameter.ToString();

            // Створюємо нове вікно детальної інформації
            DetailsWindow detailWindow = new DetailsWindow(id);

            // Відкриваємо нове вікно
            detailWindow.Show();
        }

        private async void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            searchTerm = SearchBox.Text;
            await SearchCurrencies(searchTerm);
        }

        private async Task SearchCurrencies(string searchTerm)
        {
            try
            {
                List<CryptoCurrency> searchResults = await cryptoService.SearchCurrencies(searchTerm);
                MyListBox.ItemsSource = searchResults;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void OpenConvert_Click(object sender, RoutedEventArgs e)
        {
           

            // Створюємо нове вікно детальної інформації
            ConvertCryptoWindow detailWindow = new ConvertCryptoWindow();

            // Відкриваємо нове вікно
            detailWindow.Show();
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Ви дійсно бажаєте вийти?", "Підтвердження виходу", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

    }
}

