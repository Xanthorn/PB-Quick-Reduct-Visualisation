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
        public List<string> attributes =  new List<string>  { "twardość", "kolor", "wielkość", "dec" };
        public List<List<string>> dataSets = new List<List<string>>
        {
            new List<string>{"miękkie", "fioletowe", "duże", "dojrzałe"},
            new List<string>{"miękkie", "żółte", "duże", "dojrzałe"},
            new List<string>{"miękkie", "zielone", "średnie", "dojrzałe"},
            new List<string>{"miękkie", "zielone", "średnie", "niedojrzałe"},
            new List<string>{"średnie", "fioletowe", "duże", "dojrzałe"},
            new List<string>{"średnie", "żółte", "małe", "dojrzałe"},
            new List<string>{"średnie", "żółte", "małe", "niedojrzałe"},
            new List<string>{"średnie", "zielone", "średnie", "niedojrzałe"},
            new List<string>{"twarde", "fioletowe", "średnie", "dojrzałe"},
            new List<string>{"twarde", "żółte", "małe", "dojrzałe"},
            new List<string>{"twarde", "żółte", "małe", "niedojrzałe"},
            new List<string>{"twarde", "zielone", "duże", "niedojrzałe"}
        };
        //public List<string>[][] 
        public List<List<List<string>>> differenceTable = new();
        public Dictionary<string, int> differencesCount = new();
        private string filePath;

        private void GetFilePath()
        {
            // Create open file dialog
            Microsoft.Win32.OpenFileDialog fileDialog = new();

            // Set filter for file extension and default file extension
            fileDialog.DefaultExt = ".txt";
            fileDialog.Filter = "txt files (*.txt)|*.txt";

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
            GetFilePath();
            using(var reader = new StreamReader(filePath))
            {

            }

        }
    }
}
