using CommandLine;

namespace ImportCsv.Args
{
    public class Options
    {
        [Option('p', "PathToCsv", Required = true, HelpText = "Input the fully qualified path to a CSV file, encased in double-quotes.")]
        public string PathToCsv { get; set; }

        [Option('d', "DatabaseName", Required = true, HelpText = "Input the name of the LiteDb Database encased in double-quotes; if it doesn't exist a new Db will be created.")]
        public string DatabaseName { get; set; }

        [Option('c', "ContainsHeaderNames", Required = true, HelpText = "")]
        public bool ContainsHeaderNames { get; set; }
    }
}
