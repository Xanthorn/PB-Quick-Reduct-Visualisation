using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick_Reduct_Visualisation.Models
{
    public class Data
    {
        public string[] attributes; // Attributes which describes values in datasets
        public List<string[]> dataSets;
        public string[,,] differenceTable; // Store differences between of each attribute of dataset
        public string[,] differenceTableResults; // Store all of differences in trimmed string (for showing in data grid purpose)
        public Dictionary<string, int> differenceTableCount = new(); // Used to count which attribute appears most frequently in the results of the difference table 
        public List<string> reduct;
        public string filePath;

        private void GetFilePath()
        {
            // Create open file dialog
            Microsoft.Win32.OpenFileDialog fileDialog = new();

            // Set filter for file extension and default file extension
            fileDialog.DefaultExt = ".txt";
            fileDialog.Filter = "txt files (*.txt)|*.txt|kek savestates (*.kek)|*.kek";

            // Open File Dialog
            bool? result = fileDialog.ShowDialog();

            // Get selected file name
            if (result == true)
            {
                filePath = fileDialog.FileName;
            }

        }

        public void GetData()
        {
            // Get file path
            GetFilePath();
            if (filePath==null||filePath.Contains(".kek"))
            {
                return;
            }

            using(StreamReader reader = new StreamReader(filePath))
            {
                if (attributes == null)
                {
                    attributes = reader.ReadLine().Split(" | ");
                }

                reduct = new();

                dataSets = new List<string[]>();

                string line;
                while((line = reader.ReadLine()) != null)
                {
                    string[] dataSet = line.Split(" | ");
                    dataSets.Add(dataSet);
                }

                differenceTable = new string[dataSets.Count(), dataSets.Count(), attributes.Length - 1];
                differenceTableResults = new string[dataSets.Count(), dataSets.Count()];

                for (int i = 0; i < attributes.Length; i++)
                    differenceTableCount.Add(attributes[i], 0);
            }
        }
    }
}
