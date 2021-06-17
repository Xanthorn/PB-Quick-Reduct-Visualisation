using Newtonsoft.Json;
using Quick_Reduct_Visualisation.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace Quick_Reduct_Visualisation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int reduct = 0;
        public Algorithms algorithms = new();
        public DispatcherTimer myTimer, preductTimer;
        public MainWindow()
        {
            InitializeComponent();
            myTimer = new DispatcherTimer();
            myTimer.Tick += new EventHandler(AutomaticCells);
            myTimer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            preductTimer = new DispatcherTimer();
            preductTimer.Tick += new EventHandler(CellToPrereduct);
            preductTimer.Interval = new TimeSpan(0, 0, 0, 0, 1);
        }
        public void DataToJson(object sender, RoutedEventArgs e)
        {
            string jsonData = JsonConvert.SerializeObject(algorithms);
            //string programPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            //string savePath = programPath + "\\savestate.kek";
            //File.WriteAllText(savePath, jsonData);

            Microsoft.Win32.SaveFileDialog fileDialog = new Microsoft.Win32.SaveFileDialog();
            fileDialog.FileName = "savestate";
            fileDialog.DefaultExt = ".kek";
            fileDialog.Filter = "kek savestates (*.kek)|*.kek";

            bool? result = fileDialog.ShowDialog();

            if (result == true)
            {
                File.WriteAllText(fileDialog.FileName, jsonData);
            }

        }
        public void UpdateDataGrid()
        {
            // Main table
            DataTable dt = new();
            int nbColumns = algorithms.data.dataSets.Count();
            int nbRows = algorithms.data.dataSets.Count();
            for (int i = 0; i < nbColumns + 1; i++)
            {
                if (i == 0)
                {
                    dt.Columns.Add("●");
                    continue;
                }
                dt.Columns.Add($"x{i}");
            }
            for (int row = 0; row < nbRows; row++)
            {
                DataRow dr = dt.NewRow();
                for (int col = 0; col < nbColumns + 1; col++)
                {
                    if (col == 0)
                    {
                        dr[col] = $"x{row + 1}";
                        continue;
                    }
                    dr[col] = algorithms.data.differenceTableResults[row, col - 1];
                }
                dt.Rows.Add(dr);
            }
            dataGrid.ItemsSource = dt.DefaultView;

            // Table with differences between each set

            DataTable dt2 = new();
            int nbColumns2 = 2;
            int nbRows2 = 3;

            dt2.Columns.Add("●");
            dt2.Columns.Add("Values");

            for (int row = 0; row < nbRows2; row++)
            {
                DataRow dr = dt2.NewRow();
                for (int col = 0; col < nbColumns2; col++)
                {
                    if (col == 0)
                    {
                        if (row == 0)
                            dr[col] = $"Dataset #{algorithms.i + 1}";
                        else if (row == 1)
                            dr[col] = $"Dataset #{algorithms.j + 1}";
                        else
                            dr[col] = $"Differences";
                    }
                    else
                    {
                        string values = "";
                        for (int i = 0; i < algorithms.data.dataSets[i].Length - 1; i++)
                        {
                            if (row == 0)
                                values += $"{algorithms.data.dataSets[algorithms.i][i]}, ";

                            else if (row == 1)
                                values += $"{algorithms.data.dataSets[algorithms.j][i]}, ";

                            else
                            {
                                if (algorithms.data.dataSets[algorithms.i][i] != algorithms.data.dataSets[algorithms.j][i])
                                    values += $"{algorithms.data.attributes[i]}, ";
                            }

                        }
                        if (values != "")
                            values = values.Remove(values.Length - 2);
                        dr[col] = values;
                    }
                }
                dt2.Rows.Add(dr);
            }

            dataDifferenceGrid.ItemsSource = dt2.DefaultView;

            // Table with counters 

            DataTable dt3 = new();
            int nbColumns3 = 2;
            int nbRows3 = algorithms.data.differenceTableCount.Count - 1;
            for (int i = 0; i < nbColumns3; i++)
            {
                if (i == 0)
                {
                    dt3.Columns.Add("●");
                    continue;
                }
                dt3.Columns.Add("Value");
            }
            for (int row = 0; row < nbRows3; row++)
            {
                DataRow dr = dt3.NewRow();
                for (int col = 0; col < nbColumns3; col++)
                {
                    if (col == 0)
                    {
                        dr[col] = $"{algorithms.data.attributes[row]}";
                        continue;
                    }
                    dr[col] = algorithms.data.differenceTableCount[$"{algorithms.data.attributes[row]}"];
                }
                dt3.Rows.Add(dr);
            }
            dataCountGrid.ItemsSource = dt3.DefaultView;

            ShowResults();
        }
        private void ShowResults()
        {
            if(reduct < algorithms.data.reduct.Count)
            {
                resultText.Text = "R = { ";
                foreach (string s in algorithms.data.reduct)
                {
                    resultText.Text += $"{s}, ";
                }
                resultText.Text = resultText.Text.Remove(resultText.Text.Length - 2);
                resultText.Text += " }";
            }
            if (algorithms.stopTheCount == true)
            {
                starter.Visibility = Visibility.Visible;
                stopper.Visibility = Visibility.Hidden;
                oneStep.IsEnabled = false;
                starter.IsEnabled = false;
                stopper.IsEnabled = false;
                prereduct.IsEnabled = false;
                save.IsEnabled = true;
                loadData.IsEnabled = true;
                restart.IsEnabled = true;
                MessageBox.Show(resultText.Text);
            }
        }
        private void CalculateAndDisplay()
        {
            for (int i = 0; i < algorithms.data.dataSets[i].Length - 1; i++)
            {
                algorithms.CalculateQuickReduct();
            }
            int zeroCount = 0;
            foreach (int i in algorithms.data.differenceTableCount.Values)
            {
                if (i == 0 && algorithms.tryMeNow == true)
                    zeroCount++;
            }
            if (zeroCount == algorithms.data.differenceTableCount.Count)
            {
                algorithms.stopTheCount = true;
            }
            UpdateDataGrid();
            if (algorithms.tryMeNow == true && algorithms.stopTheCount == false)
            {
                string tempString = "";
                foreach (KeyValuePair<string, string> pair in algorithms.edges)
                {
                    if (pair.Key == algorithms.currentNode)
                    {
                        if (pair.Value.Contains(algorithms.data.reduct.Last()))
                            tempString = pair.Value;
                    }
                }
                KeyValuePair<string, string> kvp = new KeyValuePair<string, string>(algorithms.currentNode, tempString);
                algorithms.currentNode = tempString;
                foreach (KeyValuePair<string, string> pair in algorithms.edges)
                {
                    if (pair.Key == algorithms.currentNode)
                    {
                        DrawGreenEdge(pair);
                    }
                }
                DrawRedEdge(kvp);
                algorithms.recreateRoute.Add(kvp);

            }
            if(algorithms.tryMeNow == true && algorithms.stopTheCount == true)
            {
                foreach (KeyValuePair<string, string> pair in algorithms.edges)
                {
                    if (pair.Key == algorithms.currentNode)
                    {
                        DrawGrayEdge(pair);
                    }
                }
            }
            algorithms.k = 0;
        }
        private void CellByCell(object sender, RoutedEventArgs e)
        {
            CalculateAndDisplay();
        }
        private void AutomaticCells(object sender, EventArgs e)
        {
            CalculateAndDisplay();
            if (algorithms.stopTheCount == true)
            {
                myTimer.Stop();
            }
        }
        private void CellToPrereduct(object sender, EventArgs e)
        {
            CalculateAndDisplay();
            if (algorithms.tryMeNow == true)
            {
                preductTimer.Stop();
                starter.IsEnabled = true;
                EnableCommonButtons();
            }
        }
        private void CellToPrereductTimer(object sender, RoutedEventArgs e)
        {
            preductTimer.Start();
            starter.IsEnabled = false;
            DisableCommonButtons();
        }
        private void StopAuto(object sender, RoutedEventArgs e)
        {
            myTimer.Stop();
            stopper.Visibility = Visibility.Hidden;
            starter.Visibility = Visibility.Visible;
            EnableCommonButtons();
        }
        private void StartAuto(object sender, RoutedEventArgs e)
        {
            myTimer.Start();
            starter.Visibility = Visibility.Hidden;
            stopper.Visibility = Visibility.Visible;
            DisableCommonButtons();
        }
        private void EnableCommonButtons()
        {
            oneStep.IsEnabled = true;
            prereduct.IsEnabled = true;
            restart.IsEnabled = true;
            save.IsEnabled = true;
            loadData.IsEnabled = true;
            showModify.IsEnabled = true;
            hideModify.IsEnabled = true;
        }
        private void DisableCommonButtons()
        {
            oneStep.IsEnabled = false;
            prereduct.IsEnabled = false;
            restart.IsEnabled = false;
            save.IsEnabled = false;
            loadData.IsEnabled = false;
            showModify.IsEnabled = false;
            hideModify.IsEnabled = false;
        }

        private void Restart(object sender, RoutedEventArgs e)
        {
            algorithms.Restart();
            UpdateDataGrid();
            EnableCommonButtons();
            stopper.Visibility = Visibility.Hidden;
            starter.Visibility = Visibility.Visible;
            starter.IsEnabled = true;
            stopper.IsEnabled = true;
            resultText.Text = "";
            IEnumerable<TextBlock> rtexts = canvas.Children.OfType<TextBlock>();
            IEnumerable<Rectangle> rrects = canvas.Children.OfType<Rectangle>();
            IEnumerable<Line> rlines = canvas.Children.OfType<Line>();
            List<TextBlock> texts = rtexts.ToList();
            List<Line> lines = rlines.ToList();
            List<Rectangle> rects = rrects.ToList();
            foreach (TextBlock item in texts)
            {
                canvas.Children.Remove(item);
            }
            foreach (Rectangle item in rects)
            {
                canvas.Children.Remove(item);
            }
            foreach (Line item in lines)
            {
                canvas.Children.Remove(item);
            }
            DrawGraph();
        }

        private void LoadData(object sender, RoutedEventArgs e)
        {
            algorithms = new Algorithms();
            algorithms.data.GetData();
            if (algorithms.data.filePath == null)
                return;
            string tempFile = algorithms.data.filePath;
            if(!algorithms.data.filePath.Contains(".kek"))
                algorithms.FillGraphContains();
            else if (algorithms.data.filePath.Contains(".kek"))
            {
                algorithms = JsonConvert.DeserializeObject<Algorithms>(File.ReadAllText(algorithms.data.filePath));
                UpdateDataGrid();
            }
            else UpdateDataGrid();
            if (algorithms.stopTheCount == true)
                return;
            starter.IsEnabled = true;
            stopper.IsEnabled = true;
            EnableCommonButtons();
            resultText.Text = "";
            DrawGraph();
            if(tempFile.Contains(".kek"))
            {
                RedrawGraph();
            }
        }
        public void DrawGraph()
        {
            double top = 0;
            double left = 40;
            TextBlock tempTextBlock = new TextBlock();
            tempTextBlock.Text = "\u2205";
            Canvas.SetTop(tempTextBlock, top);
            Canvas.SetLeft(tempTextBlock, left);
            algorithms.graphPosX[""] = left + 3;
            algorithms.graphPosY[""] = top;

            Rectangle temprect = new Rectangle();
            temprect.Fill = Brushes.Red;
            temprect.Width = 15;
            temprect.Height = 20;
            Canvas.SetTop(temprect, top);
            Canvas.SetLeft(temprect, left - 2);
            canvas.Children.Add(temprect);
            canvas.Children.Add(tempTextBlock);

            
            for (int i = algorithms.graphContains.Count() - 2; i >= 0; i--)
            {
                top += 100;
                left = 40;
                foreach (List<string> list in algorithms.graphContains[i].Values)
                {
                    string keyer = "";
                    foreach (string es in list)
                    {
                        keyer += es;
                    }
                    string teststring = "";
                    foreach (string s in list)
                    {
                        var chars = s.Split(" ").Select(y => y[0]).ToList();
                        for (int m = 0; m < chars.Count; m++)
                        {
                            teststring += chars[m];
                        }
                        teststring += ",";
                    }
                    if (teststring.Length > 0)
                        teststring = teststring.Remove(teststring.Length - 1);

                    FormattedText textSize = new FormattedText(
                        teststring,
                        CultureInfo.GetCultureInfo("en-us"),
                        FlowDirection.LeftToRight,
                        new Typeface("Verdana"),
                        32,
                        Brushes.Black);

                    TextBlock dynamicTextBlock = new TextBlock();
                    dynamicTextBlock.Text = teststring;
                    keyer = keyer.Replace(" ", "_");
                    dynamicTextBlock.Name = keyer;
                    Canvas.SetTop(dynamicTextBlock, top);
                    Canvas.SetLeft(dynamicTextBlock, left);
                    Rectangle rect = new Rectangle();
                    rect.Fill = Brushes.Gray;
                    rect.Width = textSize.Width / 2;
                    rect.Height = 20;
                    rect.Name = keyer;
                    keyer = keyer.Replace("_", " ");
                    Canvas.SetTop(rect, top);
                    Canvas.SetLeft(rect, left - (textSize.Width / 12));
                    canvas.Children.Add(rect);
                    canvas.Children.Add(dynamicTextBlock);
                    algorithms.graphPosX[keyer] = left + textSize.Width / 4;
                    algorithms.graphPosY[keyer] = top;
                    left += textSize.Width / 2 + 50;

                }

            }
            foreach (KeyValuePair<string, string> pair in algorithms.edges)
            {
                Line line = new Line();

                line.X1 = algorithms.graphPosX[pair.Key];
                line.X2 = algorithms.graphPosX[pair.Value];

                line.Y1 = algorithms.graphPosY[pair.Key] + 20;
                line.Y2 = algorithms.graphPosY[pair.Value];

                line.SnapsToDevicePixels = true;
                line.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
                line.StrokeThickness = 2;
                line.Stroke = Brushes.LightSteelBlue;
                line.Visibility = Visibility.Visible;

                canvas.Children.Add(line);
            }
            foreach (KeyValuePair<string, string> pair in algorithms.edges)
            {
                if (pair.Key == "")
                {
                    DrawGreenEdge(pair);
                }
            }
        }
        public void RedrawGraph()
        {
            foreach (KeyValuePair<string,string> kpair in algorithms.recreateRoute)
            {
                string tempString = "";
                foreach (KeyValuePair<string, string> pair in algorithms.edges)
                {
                    if (pair.Key == kpair.Key)
                    {
                        if (pair.Value.Contains(kpair.Value))
                            tempString = pair.Value;
                    }
                }
                KeyValuePair<string, string> kvp = new KeyValuePair<string, string>(kpair.Key, tempString);
                algorithms.currentNode = tempString;
                foreach (KeyValuePair<string, string> pair in algorithms.edges)
                {
                    if (pair.Key == algorithms.currentNode)
                    {
                        DrawGreenEdge(pair);
                    }
                }
                DrawRedEdge(kvp); 
            }
            if (algorithms.stopTheCount == true)
            {
                foreach (KeyValuePair<string, string> pair in algorithms.edges)
                {
                    if (pair.Key == algorithms.currentNode)
                    {
                        DrawGrayEdge(pair);
                    }
                }
            }
        }
        public void DrawRedEdge(KeyValuePair<string, string> pair)
        {
            Line line = new Line();

            line.X1 = algorithms.graphPosX[pair.Key];
            line.X2 = algorithms.graphPosX[pair.Value];

            line.Y1 = algorithms.graphPosY[pair.Key] + 20;
            line.Y2 = algorithms.graphPosY[pair.Value];

            line.SnapsToDevicePixels = true;
            line.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
            line.StrokeThickness = 2;
            line.Stroke = Brushes.Red;
            line.Visibility = Visibility.Visible;

            string stringHelper = pair.Value;
            stringHelper = stringHelper.Replace(" ", "_");
            canvas.Children.Add(line);
            Rectangle rect = new Rectangle();
            IEnumerable<Rectangle> rectangles = canvas.Children.OfType<Rectangle>();
            foreach (var rekt in rectangles)
            {
                if(rekt.Name==pair.Value)
                {
                    rect = rekt;
                    canvas.Children.Remove(rekt);
                    break;
                }
            }
            rect.Fill = Brushes.Red;
            canvas.Children.Add(rect);

            TextBlock txt = new TextBlock();
            IEnumerable<TextBlock> texts = canvas.Children.OfType<TextBlock>();
            foreach (var txtes in texts)
            {
                if (txtes.Name == pair.Value)
                {
                    txt = txtes;
                    canvas.Children.Remove(txtes);
                    break;
                }
            }
            
            canvas.Children.Add(txt);
        }
        public void DrawGreenEdge(KeyValuePair<string, string> pair)
        {
            Line line = new Line();

            line.X1 = algorithms.graphPosX[pair.Key];
            line.X2 = algorithms.graphPosX[pair.Value];

            line.Y1 = algorithms.graphPosY[pair.Key] + 20;
            line.Y2 = algorithms.graphPosY[pair.Value];

            line.SnapsToDevicePixels = true;
            line.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
            line.StrokeThickness = 2;
            line.Stroke = Brushes.Green;
            line.Visibility = Visibility.Visible;

            canvas.Children.Add(line);
        }
        public void DrawGrayEdge(KeyValuePair<string, string> pair)
        {
            Line line = new Line();

            line.X1 = algorithms.graphPosX[pair.Key];
            line.X2 = algorithms.graphPosX[pair.Value];

            line.Y1 = algorithms.graphPosY[pair.Key] + 20;
            line.Y2 = algorithms.graphPosY[pair.Value];

            line.SnapsToDevicePixels = true;
            line.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
            line.StrokeThickness = 2;
            line.Stroke = Brushes.LightSteelBlue;
            line.Visibility = Visibility.Visible;

            canvas.Children.Add(line);
            
        }

        private void ShowModifyData(object sender, RoutedEventArgs e)
        {
            DisableCommonButtons();
            showModify.Visibility = Visibility.Hidden;
            hideModify.Visibility = Visibility.Visible;
            hideModify.IsEnabled = true;
            starter.IsEnabled = false;
            modifyDataGrid.Visibility = Visibility.Visible;
            DataTable dt = new();
            int nbColumns = algorithms.data.attributes.Length + 1;
            int nbRows = algorithms.data.dataSets.Count();
            for (int i = 0; i < nbColumns; i++)
            {
                if (i == 0)
                {
                    dt.Columns.Add("●");
                    continue;
                }   
                else
                dt.Columns.Add($"{algorithms.data.attributes[i - 1]}");
                
            }
            for (int row = 0; row < nbRows; row++)
            {
                DataRow dr = dt.NewRow();
                for (int col = 0; col < nbColumns; col++)
                {
                    if (col == 0)
                    {
                        dr[col] = $"Dataset #{row + 1}";
                        continue;
                    }
                    else
                        dr[col] = algorithms.data.dataSets[row][col - 1];
                }
                dt.Rows.Add(dr);
            }
            modifyDataGrid.ItemsSource = dt.DefaultView;
        }
        private void HideModifyData(object sender, RoutedEventArgs e)
        {
            EnableCommonButtons();
            hideModify.Visibility = Visibility.Hidden;
            showModify.Visibility = Visibility.Visible;
            hideModify.IsEnabled = false;
            starter.IsEnabled = true;
            modifyDataGrid.Visibility = Visibility.Hidden;
            algorithms.Restart();
            UpdateDataGrid();
            EnableCommonButtons();
            stopper.Visibility = Visibility.Hidden;
            starter.Visibility = Visibility.Visible;
            starter.IsEnabled = true;
            stopper.IsEnabled = true;
            resultText.Text = "";
        }

        private void modifyDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            int rowIndex = modifyDataGrid.Items.IndexOf(e.Row.Item);
            int colIndex = e.Column.DisplayIndex;
            algorithms.data.dataSets[rowIndex][colIndex - 1] = e.EditingElement.ToString().Remove(0, 33);
        }
    }
}
