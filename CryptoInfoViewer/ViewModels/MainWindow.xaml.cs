﻿using System;
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
        private bool isFirstSelection = true;
        public MainWindow()
        {
            cryptoService = new CryptoService();
            InitializeComponent();
            LanguageComboBox.SelectionChanged += LanguageComboBox_SelectionChanged;
            LoadData();
        }

        // Завантаження топ 25 криптовалют
        public async void LoadData()
        {
            try
            {
                List<CryptoCurrency> top25CryptoCurrencies = await cryptoService.GetTop25CryptoCurrencies();

                MyListBox.ItemsSource = top25CryptoCurrencies;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        //Кнопка для вдкриття детальної інформації про криптовалюту
        private void Details_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string? id = button.CommandParameter.ToString();
            OpenDetailsWindow(id);
        }
        private void OpenDetailsWindow(string id)
        {
            DetailsWindow detailWindow = new DetailsWindow(id);
            detailWindow.Show();
        }


        // Виклик методу пошуку криптовалюти
        private async void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            searchTerm = SearchBox.Text;
            await SearchCurrencies(searchTerm);
        }

        // метод пошуку криптовалюти 
        private async Task SearchCurrencies(string searchTerm)
        {
            try
            {
                List<CryptoCurrency>? searchResults = await cryptoService.SearchCurrencies(searchTerm);
                MyListBox.ItemsSource = searchResults;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        // Відкриття вікна для конвертування криптовалюти
        private void OpenConvert_Click(object sender, RoutedEventArgs e)
        {
          
            ConvertCryptoWindow convertWindow = new ConvertCryptoWindow();

            convertWindow.Show();
        }
        
        //Кнопка для виходу з програми
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Do you really want to log out?", "Logout confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
             MessageBox.Show("A program that  display various information\r\nrelated to cryptocurrencies \r\nVitali Yuras");
    
        }
        private void LanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isFirstSelection)
            {
                isFirstSelection = false;
                ComboBox comboBox = (ComboBox)sender;
                ComboBoxItem selectedItem = (ComboBoxItem)comboBox.SelectedItem;
                string selectedLanguage = (string)selectedItem.Tag;
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(selectedLanguage);
                ReloadWindow();
            }
        }
        private void ReloadWindow()
        {
            MainWindow newWindow = new MainWindow();
            newWindow.Show();
            Close();
        }
    }
}

