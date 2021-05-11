using Microsoft.Win32;
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

namespace csgo_minimap_radar_helper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ImageSource defaultNoRadarImage;
        public MainWindow()
        {
            InitializeComponent();
            defaultNoRadarImage = RadarImage.Source;
        }

        private void OnLoadRadarClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dial = new();
            if ((bool)dial.ShowDialog())
            {
                try
                {
                    RadarImage.Source = new BitmapImage(new Uri(dial.FileName));
                }
                catch (Exception)
                {
                    RadarImage.Source = defaultNoRadarImage;
                }
            }
        }

        private void OnRadarImageClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void RadarImage_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            RadarImage.Source = defaultNoRadarImage;
        }
    }
}
