using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using NeuroMat_Application.ViewModel;
using NeuroMat_Application.Views;
using System.Windows.Media.Imaging;

namespace NeuroMat_Application.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel ViewModel;

        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new MainViewModel();
            DataContext = ViewModel;

        }

        //Load the results and display the image of the graph and show the latency number
        private void Load_History(object sender, RoutedEventArgs e)
        {
            if (Nervetype.SelectedItem.ToString() == "System.Windows.Controls.ComboBoxItem: Cervical" && this.Nervenumber.SelectedItem.ToString() == "System.Windows.Controls.ComboBoxItem: 1")
            {

                C1.Visibility = Visibility.Visible;
                T3.Visibility = Visibility.Hidden;
                C1.UpdateLayout();
            }
            else if (Nervetype.SelectedItem.ToString() == "System.Windows.Controls.ComboBoxItem: Thoracic" && this.Nervenumber.SelectedItem.ToString() == "System.Windows.Controls.ComboBoxItem: 3")
            {
                C1.Visibility = Visibility.Hidden;
                T3.Visibility = Visibility.Visible;
            }
            else
            {
                C1.Visibility = Visibility.Hidden;
                T3.Visibility = Visibility.Hidden;
            }
        }
    }
}
