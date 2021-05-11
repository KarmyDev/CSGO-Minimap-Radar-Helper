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

        public Dictionary<string, ImageSource> defaultResourceImages = new()
        {
            { "ct_spawn", new BitmapImage(new Uri("pack://application:,,,/Resources/ct_spawn.png")) },
            { "t_spawn", new BitmapImage(new Uri("pack://application:,,,/Resources/t_spawn.png")) },
            { "bomb_a", new BitmapImage(new Uri("pack://application:,,,/Resources/bomb_a.png")) },
            { "bomb_b", new BitmapImage(new Uri("pack://application:,,,/Resources/bomb_b.png")) },
            { "hostage", new BitmapImage(new Uri("pack://application:,,,/Resources/hostage.png")) }
        };

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
            RadarImage.Cursor = Cursors.Arrow;

            switch (selectedSelectionButton)
            {

            }
            Image test = new()
            {
                Source = defaultResourceImages["ct_spawn"],
                Width = 32,
                Height = 32
            };

            Canvas.SetLeft(test, RadarCanvas.Width / 2);
            Canvas.SetTop(test, RadarCanvas.Height / 2);

            RadarCanvas.Children.Add(test);
        }

        private void RadarImage_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            RadarImage.Source = defaultNoRadarImage;
        }

        private void UpdateEverySelectionButtonState(string state)
        { 
            
        }

        private Button GetMainButtonBasedOfHisRestartButton(string restartButtonName)
        {
            switch (restartButtonName)
            { 
                
            }
            return null;
        }

        static string selectedSelectionButton = "";

        private void OnStartSelectingRadar_Click(object sender, ExceptionRoutedEventArgs e)
        {
            RadarImage.Cursor = Cursors.Cross;

            UpdateEverySelectionButtonState("Select");
            ((Button)sender).Content = "Cancel";

            selectedSelectionButton = ((Button)sender).Name;
        }

        private void OnCancelSelectingRadar_Click(object sender, ExceptionRoutedEventArgs e)
        {
            RadarImage.Cursor = Cursors.Arrow;

            ((Button)sender).Content = "Select";

            selectedSelectionButton = "";
        }

        private void OnRestartRadarMarker_Click(object sender, ExecutedRoutedEventArgs e)
        {
            
        }

        public Dictionary<string, RadarMarker> radarMarkers = new()
        {
            { "ct_spawn", new RadarMarker() },
            { "t_spawn", new RadarMarker()  },
            { "bomb_a", new RadarMarker()  },
            { "bomb_b", new RadarMarker()  },

            { "hostage1", new RadarMarker()  },
            { "hostage2", new RadarMarker()  },
            { "hostage3", new RadarMarker()  },
            { "hostage4", new RadarMarker()  },
            { "hostage5", new RadarMarker()  }
        };

        public struct RadarMarker 
        {
            public bool IsSet { get; set; }
            public double XPoint { get; set; }
            public double YPoint { get; set; }

            public RadarMarker(bool set, double x, double y)
            {
                IsSet = set;
                XPoint = x;
                YPoint = y;
            }
        }
    }
}