using System.ComponentModel.Design;
using yagg_vhf.Parser;
using static System.Net.Mime.MediaTypeNames;

namespace yagg_vhf
{


    internal class Program
    {

        static string pwd()
        {
            return Directory.GetCurrentDirectory();
        }

        static void Main(string[] args)
        {

            var conf = new Config();

            if (args.Length > 0)
            {

                IEnumerable<String> matchingFilePaths2 = System.IO.Directory.EnumerateFiles(pwd(), conf.TargetFiles, System.IO.SearchOption.AllDirectories);

                foreach (string file in matchingFilePaths2)
                {
                    var txt = File.ReadAllText(file);
                    var parse = new ResultsParser(txt, conf, file);
                }


            }
        }
    }
}
