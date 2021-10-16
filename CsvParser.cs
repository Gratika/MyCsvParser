using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCsvParser
{
    public class CsvParser<T1, T2> where T2 : ClassMap<T1>
    {
        public List<T1> read(string path)
        {
            List<T1> val = new List<T1>();


            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {

                csv.Context.RegisterClassMap<T2>();
                var records = csv.GetRecords<T1>();
                val = records.ToList();
            }

            return val;
        }

        public void write(string path, Dictionary<string, Dictionary<String, int>> records, List<string> headers)
        {
            using (var writer = new StreamWriter(path))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteField("");
                foreach (string h in headers)
                {
                    csv.WriteField(h);
                }
                csv.NextRecord();

                foreach (var res in records)
                {
                    csv.WriteField(res.Key);
                    foreach (var b in res.Value)
                    {
                        csv.WriteField(b.Value);
                    }
                    csv.NextRecord();
                }
            }
        }
    }
}
