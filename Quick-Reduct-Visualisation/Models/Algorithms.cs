using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick_Reduct_Visualisation.Models
{
    public class Algorithms
    {        
        public Data data = new();
        public void SetDifferenceTable()
        {
            for (int i = 0; i <= data.dataSets.Count; i++)
            { 

                for(int j=0;j<=data.dataSets[i].Count;j++)
                {
                    for(int k=i; k <= data.dataSets.Count;k++)
                    {
                        if(data.dataSets[i][j] != data.dataSets[k][j])
                        {
                            data.differenceTable[i][k].Add(data.attributes[j]);
                            data.differenceTable[k][i].Add(data.attributes[j]);
                        }
                    }
                }
            }
        }
        public void CountDifferences()
        {
            for (int i = 0; i <= data.differenceTable.Count(); i++)
            {

                for (int j = 0; j <= data.differenceTable[i].Count(); j++)
                {
                    for (int k = i; k <= data.differenceTable[i][j].Count; k++)
                    {
                        data.differencesCount[data.differenceTable[i][j][k]]++;
                    }
                }
            }
        }
    }
}
/*
    "twardość", "kolor", "wielkość", "dec" 
    "miękkie", "fioletowe", "duże", "dojrzałe" 
    "miękkie", "żółte", "duże", "dojrzałe"
    "miękkie", "zielone", "średnie", "dojrzałe"
    "miękkie", "zielone", "średnie", "niedojrzałe"
    "średnie", "fioletowe", "duże", "dojrzałe"
    "średnie", "żółte", "małe", "dojrzałe"
    "średnie", "żółte", "małe", "niedojrzałe"
    "średnie", "zielone", "średnie", "niedojrzałe"
    "twarde", "fioletowe", "średnie", "dojrzałe"
    "twarde", "żółte", "małe", "dojrzałe"
    "twarde", "żółte", "małe", "niedojrzałe"
    "twarde", "zielone", "duże", "niedojrzałe"
*/
