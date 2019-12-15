using CommandLine;
using DataAccess;
using ImportCsv.Args;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ImportCsv
{
    public class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(Execute)
                .WithNotParsed(HandleParsedError);
        }

        private static void Execute(Options option)
        {
            // Get the CSV Rows
            var csvRows = File.ReadAllLines(option.PathToCsv).ToList();

            // Get the HeaderList if exists
            var headerList = option.ContainsHeaderNames ? csvRows.FirstOrDefault().Split(',').ToList() : null;

            // Create the Database, or add to it.
            var db = new CsvDatabase(option.DatabaseName, "CsvData");

            var insertedRows = db.InsertCsv(csvRows, headerList);

            var rowCount = db.GetRowCount();
            Console.WriteLine($"Database Name: {option.DatabaseName}");
            Console.WriteLine($"Inserted Rows: {insertedRows}");
            Console.WriteLine($"Total Rows: {rowCount}");

            //var row = db.GetNext();
        }

        private static void HandleParsedError(IEnumerable<Error> errors)
        {
            foreach (var error in errors)
            {
                Console.WriteLine(error.ToString());
            }
        }

        private class CsvData
        {
            public List<string> RowItems { get; set; }
        }
    }
}
