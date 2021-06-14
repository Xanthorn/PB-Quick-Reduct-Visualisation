using Quick_Reduct_Visualisation.Models;
using System;
using System.Collections.Generic;
using System.Data;
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
        public Algorithms algorithms = new();
        public DispatcherTimer myTimer;
        public bool stopTheCount = false;
        public MainWindow()
        {
            InitializeComponent();
            myTimer = new DispatcherTimer();
            myTimer.Tick += new EventHandler(AutomaticCells);
            myTimer.Interval = new TimeSpan(0,0,0,0,1);
            myTimer.Start();
        }

        public void UpdateDataGrid()
        {
            DataTable dt = new DataTable();
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

            DataTable dt2 = new DataTable();
            int nbColumns2 = 2;
            int nbRows2 = algorithms.data.differenceTableCount.Count - 1;
            for (int i = 0; i < nbColumns2; i++)
            {
                if (i == 0)
                {
                    dt2.Columns.Add("●");
                    continue;
                }
                dt2.Columns.Add("Value");
            }

            for (int row = 0; row < nbRows2; row++)
            {
                DataRow dr2 = dt2.NewRow();
                for (int col = 0; col < nbColumns2; col++)
                {
                    if (col == 0)
                    {
                        dr2[col] = $"{algorithms.data.attributes[row]}";
                        continue;
                    }
                    dr2[col] = algorithms.data.differenceTableCount[$"{algorithms.data.attributes[row]}"];
                }
                dt2.Rows.Add(dr2);
            }

            dataCountGrid.ItemsSource = dt2.DefaultView;
            if (stopTheCount==true)
                return;
            DataTable dt3 = new DataTable();
            int nbColumns3 = 1;
            int nbRows3 = algorithms.data.reduct.Count;
            for (int i = 0; i < nbColumns3; i++)
            {
                
                dt3.Columns.Add("Result");
            }

            for (int row = 0; row < nbRows3; row++)
            {
                DataRow dr3 = dt3.NewRow();
                for (int col = 0; col < nbColumns3; col++)
                {
                    dr3[col] = algorithms.data.reduct[row];
                }
                dt3.Rows.Add(dr3);
            }

            dataResultGrid.ItemsSource = dt3.DefaultView;

        }

        private void CellByCell(object sender, RoutedEventArgs e)
        {
            if (stopTheCount == true)
            {
                myTimer.Stop();
                action.Visibility = Visibility.Hidden;
                starter.Visibility = Visibility.Hidden;
                stopper.Visibility = Visibility.Hidden;
                resultText.Text = "(";
                foreach(string s in algorithms.data.reduct)
                {
                    resultText.Text += s + ",";
                }
                resultText.Text = resultText.Text.Remove(resultText.Text.Length - 1);
                resultText.Text +=")";
                MessageBox.Show("Reduct is: " + resultText.Text);
                return;
            }
                
            for (int i = 0; i < algorithms.data.dataSets[i].Length - 1; i++)
            {
                algorithms.CalculateQuickReduct();
            }
            int zeroCount = 0;
            foreach (int i in algorithms.data.differenceTableCount.Values)
            {
                if (i == 0 && algorithms.tryMeNow==true)
                    zeroCount++;
            }
            if (zeroCount == algorithms.data.differenceTableCount.Count)
            {
                stopTheCount = true;
            }
            UpdateDataGrid();
            algorithms.k = 0;
            
        }
        private void AutomaticCells(object sender, EventArgs e)
        {
            if (stopTheCount == true)
            {
                myTimer.Stop();
                action.Visibility = Visibility.Hidden;
                starter.Visibility = Visibility.Hidden;
                stopper.Visibility = Visibility.Hidden;
                resultText.Text = "(";
                foreach (string s in algorithms.data.reduct)
                {
                    resultText.Text += s + ",";
                }
                resultText.Text = resultText.Text.Remove(resultText.Text.Length - 1);
                resultText.Text += ")";
                MessageBox.Show("Reduct is: " + resultText.Text);
                return;
            }
            textBlox.Text = (Convert.ToInt32(textBlox.Text) + 1).ToString(); 
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
                stopTheCount = true;
            }
            UpdateDataGrid();
            algorithms.k = 0;
            
        }
        private void StopAuto(object sender, RoutedEventArgs e)
        {
            myTimer.Stop();
        }
        private void StartAuto(object sender, RoutedEventArgs e)
        {
            myTimer.Start();
        }

    }
}
