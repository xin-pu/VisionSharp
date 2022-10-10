using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;

namespace CVLib.Models
{
    public struct ClassifyResult
    {
        public string Name { set; get; }

        public int Classify { set; get; }


        /// <summary>
        ///     获取数据集信息
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static List<ClassifyResult> GetClassifyResults(string filename)
        {
            using var reader = new StreamReader(filename);
            using var csv = new CsvReader(reader, CultureInfo.CurrentCulture);
            var res = csv.GetRecords<ClassifyResult>();
            return res.ToList();
        }


        public override string ToString()
        {
            return $"ClassifyResult:\t{Name}\t{Classify}";
        }
    }
}