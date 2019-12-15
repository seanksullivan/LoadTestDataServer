using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace LoadTestData.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoadDataController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly string _databaseFolder;

        public LoadDataController(IConfiguration config)
        {
            _config = config;
            _databaseFolder = _config.GetValue<string>("DatabaseFolder");
        }

        /// <summary>
        /// https://localhost:44351/api/loaddata/getnextrow
        /// http://localhost:5000/api/LoadData/getnextrow
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetNextRow")]
        public string GetNextRow(string databaseName = "Test4.db")
        {
            var dbFile = Path.Combine(_databaseFolder, databaseName);
            var db = new CsvDatabase(dbFile, "CsvData");
            var row = db.GetNext();
            return string.IsNullOrWhiteSpace(row) ? "" : row.ToString();
        }
    }
}