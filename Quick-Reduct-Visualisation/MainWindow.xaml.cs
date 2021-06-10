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

namespace Quick_Reduct_Visualisation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Algorithms algorithms = new();
        public MainWindow()
        {
            InitializeComponent();
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
            for (int i = 0; i < algorithms.data.dataSets[i].Length - 1; i++)
            {
                algorithms.CalculateQuickReduct();
            }

            UpdateDataGrid();
            algorithms.k = 0;
        }
    }
}
