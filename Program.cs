using System.ComponentModel.Design;
using yagg_vhf.Contract;
using yagg_vhf.Output.JSON;
using yagg_vhf.Parser;
using yagg_vhf.Parser.qso;
using yagg_vhf.Parser.rezultati;
using static System.Net.Mime.MediaTypeNames;

namespace yagg_vhf
{


    internal class Program
    {

        static string pwd()
        {
            return Directory.GetCurrentDirectory();
        }

        static string LocateAndLoadQSO(string resultsath)
        {
            var qsoPath = Path.GetDirectoryName(resultsath) + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar + "QSOBase.dat";
            return File.ReadAllText(qsoPath);

        }

        static void Main(string[] args)
        {

            var conf = new Config();

            if (args.Length > 0)
            {

                IEnumerable<String> matchingFilePaths2 = System.IO.Directory.EnumerateFiles(pwd(), conf.TargetFiles, System.IO.SearchOption.AllDirectories);

                var QSOBases = matchingFilePaths2.Select( file => new { file, contents = LocateAndLoadQSO(file) } ).ToDictionary(x => x.file, x => new QsoParser(x.contents, conf, x.file));
                var resultsCSVs = matchingFilePaths2.Select( file => new ResultsParser(File.ReadAllText(file), conf, file, QSOBases[file].QsoLog )).SelectMany( parser => parser.scores).ToArray();

                var results = new CompleteResultSet(resultsCSVs, conf);

                var jo = new JsonOutput(results);
                jo.Produce();


            }
        }
    }
}
