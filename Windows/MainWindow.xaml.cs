using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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

            MapName.Text = "de_fault";
            TextureFileName.Text = "overviews/de_fault";
            ConsoleOverviewInput.Text = "Overview: scale 0.00, pos_x 0, pos_y 0";

            UpdateRadarConfigOutput();
        }

        private void OnLoadRadarClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dial = new();
            if ((bool)dial.ShowDialog())
            {
                try
                {
                    FileInfo file = new FileInfo(dial.FileName);
                    RadarImage.Source = new BitmapImage(new Uri(dial.FileName));
                    if (file.Name.Contains('.')) TextureFileName.Text = $"overviews/{file.Name.Split('.')[0]}";
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

            if (radarMarkers[markerName].IsSet) RadarCanvas.Children.Remove(radarMarkers[markerName].OnScreenImage);

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

            var pos = e.GetPosition(RadarCanvas);

            imgObject.Source = defaultResourceImages[defResImg];
            imgObject.Width = 50;
            imgObject.Height = 50;

            Canvas.SetLeft(imgObject, pos.X - imgObject.Width / 2);
            Canvas.SetTop(imgObject, pos.Y - imgObject.Height / 2);

            radarMarkers[markerName] = new RadarMarker(
                true,
                pos.X,
                pos.Y,
                imgObject
            );
            RadarCanvas.Children.Add(imgObject);
            lastSelectedButton = null;
            lastSelectedButtonName = null;
            UpdateRadarConfigOutput();
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

                lastSelectedButton = null;
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
            UpdateRadarConfigOutput();
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

        private void OnTextInputDataChanged(object sender, TextChangedEventArgs e)
        {
            UpdateRadarConfigOutput();
        }

        private void UpdateRadarConfigOutput()
        {
            string finalGeneratedData = "// Radar data generated by Karmel's CSGO Minimap radar helper\n\n";
            string finalOverview = string.Empty;

            string[] globalProcess = ConsoleOverviewInput.Text.Split(',');

            bool hitAnyErrorWithOverview = false;

            if (string.IsNullOrWhiteSpace(MapName.Text))
            {
                finalGeneratedData = "// Cannot generate radar config file without knowing the map name.\n// Please input the map name to proceed.";
                RadarOutput.Text = $"// ===[generator errors]=======================\n{finalGeneratedData}\n// ============================================";
                return;
            }
            if (string.IsNullOrWhiteSpace(TextureFileName.Text))
            {
                finalGeneratedData = "// Cannot generate radar config file without knowing the texture file name.\n// Please input the texture file name to proceed.";
                RadarOutput.Text = $"// ===[generator errors]=======================\n{finalGeneratedData}\n// ============================================";
                return;
            }

            finalGeneratedData += $"\"{MapName.Text.Replace(' ', '_')}\"\n{{\n";
            finalGeneratedData += $"\t\"material\" \"{TextureFileName.Text.Replace(' ', '_')}\"\n";

            

            if (globalProcess.Length >= 3)
            {
                string[] preProcess = globalProcess[1].Split();

                if (preProcess.Length >= 2)
                {
                    if (preProcess[1] == "pos_x")
                    {
                        finalOverview += $"\t\"pos_x\" \"{preProcess[2]}\"\n";
                    }
                    else hitAnyErrorWithOverview = true;
                }
                else hitAnyErrorWithOverview = true;

                preProcess = globalProcess[2].Split();

                if (preProcess.Length >= 2)
                {
                    if (preProcess[1] == "pos_y")
                    {
                        finalOverview += $"\t\"pos_y\" \"{preProcess[2]}\"\n";
                    }
                    else hitAnyErrorWithOverview = true;
                }
                else hitAnyErrorWithOverview = true;

                preProcess = globalProcess[0].Split();

                if (preProcess[1] == "scale")
                {
                    finalOverview += $"\t\"scale\" \"{preProcess[2]}\"\n";
                }
                else hitAnyErrorWithOverview = true;
            }

            if (hitAnyErrorWithOverview)
            {
                finalGeneratedData += "\n// ====[console overview error]===========\n// Cannot read data from Console Overview\n// =======================================\n";
            }
            else
            {
                finalGeneratedData += finalOverview;
            }

            RadarMarker thisRM = radarMarkers["ct_spawn"];
            if (thisRM.IsSet) finalGeneratedData += $"\t\"CTSpawn_x\" \"{String.Format(CultureInfo.InvariantCulture, "{0:F2}", thisRM.XPoint / RadarCanvas.Width)}\"\n";
            if (thisRM.IsSet) finalGeneratedData += $"\t\"CTSpawn_y\" \"{String.Format(CultureInfo.InvariantCulture, "{0:F2}", thisRM.YPoint / RadarCanvas.Height)}\"\n";

            thisRM = radarMarkers["t_spawn"];
            if (thisRM.IsSet) finalGeneratedData += $"\t\"TSpawn_x\" \"{String.Format(CultureInfo.InvariantCulture, "{0:F2}", thisRM.XPoint / RadarCanvas.Width)}\"\n";
            if (thisRM.IsSet) finalGeneratedData += $"\t\"TSpawn_y\" \"{String.Format(CultureInfo.InvariantCulture, "{0:F2}", thisRM.YPoint / RadarCanvas.Height)}\"\n";

            finalGeneratedData += "\n";

            thisRM = radarMarkers["bomb_a"];
            if (thisRM.IsSet) finalGeneratedData += $"\t\"bombA_x\" \"{String.Format(CultureInfo.InvariantCulture, "{0:F2}", thisRM.XPoint / RadarCanvas.Width)}\"\n";
            if (thisRM.IsSet) finalGeneratedData += $"\t\"bombA_y\" \"{String.Format(CultureInfo.InvariantCulture, "{0:F2}", thisRM.YPoint / RadarCanvas.Height)}\"\n";

            foreach (int i in Enumerable.Range(1, 5))
            {
                thisRM = radarMarkers["hostage" + i];
                if (thisRM.IsSet) finalGeneratedData += $"\t\"Hostage{i}_x\" \"{String.Format(CultureInfo.InvariantCulture, "{0:F2}", thisRM.XPoint / RadarCanvas.Width)}\"\n";
                if (thisRM.IsSet) finalGeneratedData += $"\t\"Hostage{i}_y\" \"{String.Format(CultureInfo.InvariantCulture, "{0:F2}", thisRM.YPoint / RadarCanvas.Height)}\"\n";
            }

            RadarOutput.Text = $"{finalGeneratedData}\n}}";
        }
    }
}