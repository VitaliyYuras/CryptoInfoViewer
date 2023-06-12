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
            //LoadMarkets(id);
            LoadCandlestickData();

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


        public async void LoadCandlestickData()
        {
            List<CandleData> cryptoData = await cryptoService.GetDataFromApi("poloniex", "h8", "ethereum", "bitcoin");

            //Label1.Content = cryptoData.Count.ToString();

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


    }
}
