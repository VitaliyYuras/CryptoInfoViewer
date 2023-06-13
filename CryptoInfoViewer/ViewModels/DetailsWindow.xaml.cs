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
using System.Windows.Shapes;
using CryptoInfoViewer.Models;
using CryptoInfoViewer.Services;

using System.Diagnostics;


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
            LoadMarkets(id);
            LoadDetails(id);
            LoadCandlestickData(id);


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
        public async void LoadCandlestickData(string symbol)
        {
            List<CandleData> candleData = await cryptoService.GetDataFromApi(symbol, "7", "usd");

            DrawCandleChart(candleData);
        }
        private void DrawCandleChart(List<CandleData> data)
        {
            // Очистити попередню діаграму, якщо є
            canvas.Children.Clear();

            // Встановити розміри графіку
            double chartWidth = canvas.ActualWidth;
            double chartHeight = canvas.ActualHeight;

            // Розрахувати ширину свічки
            double candleWidth = chartWidth / data.Count;

            // Знайти максимальне та мінімальне значення ціни для встановлення масштабу графіку
            decimal maxPrice = data.Max(c => c.high);
            decimal minPrice = data.Min(c => c.low);
            decimal priceRange = maxPrice - minPrice;

            // Проходження по кожному запису і побудова свічки
            for (int i = 0; i < data.Count; i++)
            {
                CandleData candle = data[i];

                // Розрахувати координати свічки
                double candleX = i * candleWidth;
                double candleY = chartHeight * (1 - (double)(candle.close - minPrice) / (double)priceRange);
                double candleHeight = chartHeight * (double)(candle.close - candle.open) / (double)priceRange;

                // Визначити колір свічки в залежності від напрямку руху ціни
                SolidColorBrush candleColor = candle.close >= candle.open ? Brushes.Green : Brushes.Red;

                // Створити прямокутник для свічки
                Rectangle candleRectangle = new Rectangle
                {
                    Fill = candleColor,
                    Width = candleWidth,
                    Height = Math.Abs(candleHeight),
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };

                // Встановити позицію свічки
                Canvas.SetLeft(candleRectangle, candleX);
                Canvas.SetTop(candleRectangle, candleY - (candle.close >= candle.open ? 0 : Math.Abs(candleHeight)));

                // Додати свічку на графік
                canvas.Children.Add(candleRectangle);
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
