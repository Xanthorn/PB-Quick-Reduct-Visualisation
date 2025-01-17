﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick_Reduct_Visualisation.Models
{
    public class Algorithms
    {
        public Data data = new();
        public int i, j, k, cycles;
        public bool tryMeNow;
        public bool stopTheCount = false;
        public Dictionary<string, List<string>>[] graphContains;
        public Dictionary<string, double> graphPosX=new();
        public Dictionary<string, double> graphPosY=new();
        public List<KeyValuePair<string,string>> edges = new();
        public string currentNode = "";
        public List<KeyValuePair<string,string>> recreateRoute = new();
        public Algorithms()
        {
            data = new();
            i = 0;
            j = 0;
            k = 0;
            tryMeNow = false;
            cycles = 0;
        }
        public void FillGraphContains()
        {
            string stringer = "";
            graphContains = new Dictionary< string, List<string>>[data.attributes.Count()];
            for (int i = 0; i < data.attributes.Count() ; i++)
            {
                graphContains[i] = new Dictionary<string, List<string>>();
            }
            graphContains[0].Add(stringer, data.attributes.ToList());
            for(int i=0;i< data.attributes.Count() - 1;i++)
            {
                foreach(List<string> list in graphContains[i].Values)
                {
                    if(list.Contains(data.attributes[data.attributes.Count()-1]))
                    {
                        list.Remove(data.attributes[data.attributes.Count() - 1]);
                    }
                    foreach(string s in list)
                    {
                        List<string> lister = list.ToList();
                        lister.Remove(s);
                        string keyer = "";
                        foreach(string es in lister)
                        {
                            keyer += es;
                        }
                        string upperkeyer = "";
                        foreach (string es in list)
                        {
                            upperkeyer += es;
                        }
                        if (!graphContains[i+1].ContainsKey(keyer))
                        {
                            graphContains[i + 1].Add(keyer, lister);
                            graphPosX.Add(keyer, 0);
                            graphPosY.Add(keyer, 0);
                            
                        }

                        edges.Add(new KeyValuePair<string, string>(keyer, upperkeyer));
                    }
                }
            }
        }

        public void CalculateQuickReduct()
        {
            CalculateDifference();
        }

        public void ReinitialiseParameters()
        {
            cycles = 0;
            i = 0;
            j = 0;
            k = 0;
        }
        public void Restart()
        {
            cycles = 0;
            i = 0;
            j = 0;
            k = 0;
            tryMeNow = false;
            recreateRoute = new List<KeyValuePair<string,string>>();
            currentNode = "";
            data.differenceTable = new string[data.dataSets.Count(), data.dataSets.Count(), data.attributes.Length - 1];
            data.differenceTableResults = new string[data.dataSets.Count(), data.dataSets.Count()];
            data.reduct = new();
            stopTheCount = false;
            for (int i = 0; i < data.attributes.Length; i++)
                data.differenceTableCount[$"{data.attributes[i]}"] = 0;
        }

        private void CalculateDifference()
        {
            tryMeNow = false;
            if (i == j || data.dataSets[i][^1] == data.dataSets[j][^1]) // if on diagonal or datasets have same outputs
            {
                data.differenceTable[i, j, k] = "0";
                data.differenceTableResults[i, j] = "\u2205";
            }
            else
            {
                if (data.dataSets[i][k] == data.dataSets[j][k]) // if parameter has same value then 0
                {
                    data.differenceTable[i, j, k] = "0";
                }
                else if(data.differenceTableResults[i, j] != "\u2205") // if parameter has different value then 1
                {
                    data.differenceTable[i, j, k] = "1";
                }

            }

            if (data.differenceTable[i, j, k] == "1")  // if parameter was 1
            {
                if (data.reduct.Contains(data.attributes[k]) && data.reduct.Count > 0) // if reduct contains attribute then set all differences to 0 and result to empty set char
                {
                    data.differenceTableResults[i, j] = "\u2205";
                    for (int m = 0; m < data.attributes.Length - 1; m++)
                    {
                        data.differenceTable[i, j, m] = "0";
                    }
                }
                else
                {
                    string result = "";
                    var chars = data.attributes[k].Split(" ").Select(y => y[0]).ToList();

                    for (int m = 0; m < chars.Count; m++)
                    {
                        result += chars[m];
                    }

                    data.differenceTableResults[i, j] += $"{result} ";
                }
            }

            if (k < data.dataSets[i].Length - 2)
                k++;
            else if(j < data.dataSets.Count - 1)
            {
                k = 0;
                j++;
            }
            else if(i < data.dataSets.Count - 1)
            {
                k = 0;
                j = 0;
                i++;
            }
            else
            {
                CalculateQualityOfTheClassificationApproximation();
                int zeroCount = 0;
                foreach (int i in data.differenceTableCount.Values)
                {
                    if(i==0)
                        zeroCount++;
                }
                if (zeroCount != data.differenceTableCount.Count)
                    FindMostFrequentlyAppearedAttribute();
                ReinitialiseParameters();
                data.differenceTableResults = new string[data.dataSets.Count(), data.dataSets.Count()];
                cycles++;
                tryMeNow = true;
            }
        }

        private void CalculateQualityOfTheClassificationApproximation()
        {
            for (int i = 0; i < data.attributes.Count(); i++)
            {
                data.differenceTableCount[$"{data.attributes[i]}"] = 0;
            }    

            for (int i = 0; i < data.dataSets.Count(); i++)
            {
                for (int j = 0; j < data.dataSets.Count(); j++)
                {
                    for (int k = 0; k < data.dataSets[i].Length - 1; k++)
                    {
                        if (data.differenceTable[i, j, k] == "1")
                        {
                            data.differenceTableCount[$"{data.attributes[k]}"]++;
                        }
                    }
                }
            }
        }

        public void FindMostFrequentlyAppearedAttribute()
        {
            data.reduct.Add(data.differenceTableCount.Aggregate((l, r) => l.Value > r.Value ? l : r).Key);
        }
    }
}

