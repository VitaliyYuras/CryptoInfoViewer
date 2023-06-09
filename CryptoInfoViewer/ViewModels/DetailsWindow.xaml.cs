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
using CryptoInfoViewer.Models;
using CryptoInfoViewer.Services;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using OxyPlot.Wpf;

namespace CryptoInfoViewer.Views
{
    /// <summary>
    /// Interaction logic for DetailsWindow.xaml
    /// </summary>
    public partial class DetailsWindow : Window
    {
       
        private CryptoService cryptoService;
        public DetailsWindow(string id)
        {
            cryptoService = new CryptoService();
            InitializeComponent();
            LoadDetails(id);

        }
        public async void LoadDetails(string id)
        {
            try
            {
                CryptoCurrency сryptoCurrencies = await cryptoService.GetCryptoCurrencies(id);
                DataContext = сryptoCurrencies;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

    }
}
