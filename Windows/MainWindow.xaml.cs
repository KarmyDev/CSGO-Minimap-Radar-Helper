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

        public Dictionary<string, Image> existingRadarMarkers = new()
        {
            { "", new Image() }
        };

        public MainWindow()
        {
            InitializeComponent();
            defaultNoRadarImage = RadarImage.Source;

            radarMarkerButtons.Add(ct_spawn_button);
            radarMarkerButtons.Add(t_spawn_button);

            radarMarkerButtons.Add(bomb_a_button);
            radarMarkerButtons.Add(bomb_b_button);

            radarMarkerButtons.Add(hostage1_button);
            radarMarkerButtons.Add(hostage2_button);
            radarMarkerButtons.Add(hostage3_button);
            radarMarkerButtons.Add(hostage4_button);
            radarMarkerButtons.Add(hostage5_button);
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
            if (lastSelectedButton == null) return;
            if (string.IsNullOrEmpty(lastSelectedButton.Name)) return;
            lastSelectedButtonName = "Update";
            UpdateEverySelectionButtonState();

            string markerName = GetMarkerNameByItsButtonName(lastSelectedButton.Name);
            Image imgObject = new();

            string defResImg = markerName;

            switch (defResImg)
            {
                case "hostage1":
                case "hostage2":
                case "hostage3":
                case "hostage4":
                case "hostage5":
                    defResImg = "hostage";
                break;
            }

            imgObject.Source = defaultResourceImages[defResImg];
            imgObject.Width = 50;
            imgObject.Height = 50;

            Canvas.SetLeft(imgObject, (RadarCanvas.Width / 2) - imgObject.Width / 2);
            Canvas.SetTop(imgObject, (RadarCanvas.Height / 2) - imgObject.Height / 2);

            radarMarkers[markerName] = new RadarMarker(
                true,
                imgObject.Width,
                imgObject.Height,
                imgObject
            );
            // [THIS GUD CODE BUT DISABLED FOR DEBUG]
            RadarCanvas.Children.Add(imgObject);
           
            Label myText = new()
            {
                Content = $"X: {RadarCanvas.Width} Y: {RadarCanvas.Height}"
            };
            RadarCanvas.Children.Add(myText);
            
        }

        private void RadarImage_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            RadarImage.Source = defaultNoRadarImage;
        }

        public static List<Button> radarMarkerButtons = new();

        private void UpdateEverySelectionButtonState()
        {
            if (lastSelectedButton != null)
            {
                lastSelectedButton.Content = lastSelectedButtonName;
            }

            foreach (Button btn in radarMarkerButtons)
            {
                if ((string)btn.Content != "Add")
                {
                    btn.Content = "Update";
                }
                else
                {
                    btn.Content = "Add";
                }
            }

        }

        private Button GetMainButtonByItsRestartButton(string restartButtonName)
        {
            return radarMarkerButtons.Find(item => item.Name == restartButtonName.Substring(0, restartButtonName.Length - 2));
        }

        private string GetMarkerNameByItsButtonName(string buttonName)
        {
            switch (buttonName)
            {
                case "ct_spawn_button":
                    return "ct_spawn";
                case "t_spawn_button":
                    return "t_spawn";

                case "bomb_a_button":
                    return "bomb_a";
                case "bomb_b_button":
                    return "bomb_b";

                case "hostage1_button":
                    return "hostage1";
                case "hostage2_button":
                    return "hostage2";
                case "hostage3_button":
                    return "hostage3";
                case "hostage4_button":
                    return "hostage4";
                case "hostage5_button":
                    return "hostage5";
            }
            return null;
        }

        static string lastSelectedButtonName = string.Empty;
        static Button lastSelectedButton;

        private void OnStartSelectingRadar_Click(object sender, RoutedEventArgs e)
        {
            if ((string)((Button)sender).Content == "Cancel")
            {
                RadarImage.Cursor = Cursors.Arrow;
                ((Button)sender).Content = lastSelectedButtonName;

                lastSelectedButtonName = null;
            }
            else
            {
                RadarImage.Cursor = Cursors.Cross;

                UpdateEverySelectionButtonState();
                lastSelectedButtonName = (string)((Button)sender).Content;
                ((Button)sender).Content = "Cancel";

                lastSelectedButton = (Button)sender;
            }
        }

        private void OnCancelSelectingRadar_Click(object sender, ExceptionRoutedEventArgs e)
        {
            RadarImage.Cursor = Cursors.Arrow;

            ((Button)sender).Content = "Select";

            lastSelectedButton = null;
        }

        private void OnRestartRadarMarker_Click(object sender, RoutedEventArgs e)
        {
            Button desiredMainButton = GetMainButtonByItsRestartButton(((Button)sender).Name);
            string desiredRMName = GetMarkerNameByItsButtonName(desiredMainButton.Name);
            RadarCanvas.Children.Remove(radarMarkers[desiredRMName].OnScreenImage);
            radarMarkers[desiredRMName] = new();
            desiredMainButton.Content = "Add";
            if (lastSelectedButton == desiredMainButton)
            {
                lastSelectedButtonName = "Add";
            }
        }

        public Dictionary<string, RadarMarker> radarMarkers = new()
        {
            { "ct_spawn", new RadarMarker() },
            { "t_spawn", new RadarMarker() },
            { "bomb_a", new RadarMarker() },
            { "bomb_b", new RadarMarker() },

            { "hostage1", new RadarMarker() },
            { "hostage2", new RadarMarker() },
            { "hostage3", new RadarMarker() },
            { "hostage4", new RadarMarker() },
            { "hostage5", new RadarMarker() }
        };

        public struct RadarMarker 
        {
            public bool IsSet { get; set; }
            public double XPoint { get; set; }
            public double YPoint { get; set; }

            public Image OnScreenImage { get; set; }

            public RadarMarker(bool set, double x, double y, Image image)
            {
                IsSet = set;
                XPoint = x;
                YPoint = y;
                OnScreenImage = image;
            }
        }
    }
}