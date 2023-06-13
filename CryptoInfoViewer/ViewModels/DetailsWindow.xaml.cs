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
using LiveCharts;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using OxyPlot.Wpf;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System.Reflection.Emit;
using System.Diagnostics;

namespace CryptoInfoViewer.Views
{
    /// <summary>
    /// Interaction logic for DetailsWindow.xaml
    /// </summary>
    public partial class DetailsWindow : Window
    {
       
        private CryptoService cryptoService;
        public SeriesCollection CandleSeriesCollection { get; set; }

        public DetailsWindow(string id)
        {
            cryptoService = new CryptoService();
            InitializeComponent();
            LoadMarkets(id);
            LoadDetails(id);
            LoadCandlestickData();

        }
        // Завантаження всіх даних про певну криптовалюту
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
        // ЗАвантаження ринків де можна обміняти дану криптовалюту
        public async void LoadMarkets(string id)
        {
            try
            {
                List <CryptoMarkets> сryptoMarkets = await cryptoService.GetMarkets(id);
                ListMarkets.ItemsSource = сryptoMarkets;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        // завантаження діаграми криптовалюти
        public async void LoadCandlestickData()
        {
            List<CandleData> cryptoData = await cryptoService.GetDataFromApi("poloniex", "h8", "ethereum", "bitcoin");

            CandleSeriesCollection = new SeriesCollection();

            foreach (CandleData candle in cryptoData)
            {
                OhlcPoint ohlcPoint = new OhlcPoint((double)candle.open, (double)candle.high, (double)candle.low, (double)candle.close);
                CandleSeriesCollection.Add(new OhlcSeries
                {
                    Values = new ChartValues<OhlcPoint> { ohlcPoint }
                });
            }
        }

        // Кнопка для відкриття сайту певної криптовалюти
        private void WButton_Click(object sender, RoutedEventArgs e)
        {

            Button button = (Button)sender;
            Hyperlink? hyperlink = button.Template.FindName("PART_Hyperlink", button) as Hyperlink;

            if (hyperlink != null)
            {
                Uri explorerUri = hyperlink.NavigateUri;

                if (explorerUri != null)
                {
                    string url = explorerUri.AbsoluteUri;

                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
            }


        }

    }
}
