using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyCsvParser
{
    public class BugAnalisys
    {
        static AutoResetEvent waitHandler = new AutoResetEvent(true);
        static Semaphore sem = new Semaphore(3, 3);

        private readonly List<string> SummaryField = new List<string> { "Total", "Unresolved" };
        private readonly List<string> DefaultResFileField = new List<string> { "TEAM_BEAUJOLAIS", "TEAM_BORDEAUX", "TEAM_BURGUNDY", "TEAM_LOIRE", "TEAM_PROVENCE", "TEAM_RHONE", "MISC", "Total" };
        public string Path { get; set; }
        private CsvParser<BugRow, BugMap> Parser;
        private List<BugRow> DataList;        
        private Dictionary<string, Dictionary<String, int>> Result;
        private int cnt;
        public BugAnalisys(string path)
        {
            this.Path = path;
            this.Parser = new CsvParser<BugRow, BugMap>();
            cnt = 0;
            getData();
            initializeResult();

        }
        private void addKeyResult(string mainKey)
        {
            if (!Result.ContainsKey(mainKey))
            {
                Dictionary<string, int> tmp = new Dictionary<string, int>();
                foreach (string field in DefaultResFileField)
                {
                    tmp.Add(field, 0);
                }
                Result.Add(mainKey, tmp);
            }
        }
        private void initializeResult()
        {
            this.Result = new Dictionary<string, Dictionary<string, int>>();
            foreach (string mainfield in SummaryField)
            {
                addKeyResult(mainfield);
            }
        }

        public void getData()
        {
            this.DataList = this.Parser.read(this.Path);
        }
        public void getSummaryBag()
        {
            foreach (BugRow b in DataList)
            {
                new Thread(analisysRowBag).Start(b);               
            }
        }
        private void analisysRowBag(object bugRow)
        {
            
            sem.WaitOne();
            BugRow bug = bugRow as BugRow;
            if (bug != null)
            {
                string priority = bug.Priority.ToLower();
                waitHandler.WaitOne();
                addKeyResult(priority);
                waitHandler.Set();
                String[] labels = bug.Labels.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                foreach (string str in labels)
                {
                    string label = str.Trim();

                    if (Result[priority].ContainsKey(label))
                    {
                        waitHandler.WaitOne();                            
                        Result[priority][label]++;
                        Result[priority]["Total"]++;
                        Result["Total"][label]++;
                        Result["Total"]["Total"]++;
                        waitHandler.Set();
                        if (bug.Status!=null && !bug.Status.Trim().StartsWith("Closed"))
                        {
                             waitHandler.WaitOne();
                            Result["Unresolved"][label]++;
                            Result["Unresolved"]["Total"]++;
                            waitHandler.Set();



                        }

                    }

                }
                waitHandler.WaitOne();
                cnt++;
                waitHandler.Set();

                if (cnt == DataList.Count)
                {
                    printBug();
                }
                sem.Release();

            }

        }
       
        private void printBug()
        {

            string directoryPath = System.IO.Path.GetDirectoryName(this.Path);
            string newFileName = System.IO.Path.Combine(directoryPath, String.Concat("resParse_", DateTime.Now.ToString().Replace(":","_"),".csv"));
            Parser.write(newFileName, Result,DefaultResFileField);
           Console.WriteLine( "Файл с результатами успешно сохранен: "+newFileName);
        }

    }
}
