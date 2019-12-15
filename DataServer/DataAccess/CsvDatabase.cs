using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess
{
    public class CsvDatabase
    {
        public string DatabaseName { get; set; }
        public string TableName { get; set; }

        public CsvDatabase(string databaseName, string tableName)
        {
            DatabaseName = databaseName;
            TableName = tableName;
        }

        public void InsertCsv(List<BsonDocument> bsonDocList)
        {
            using var db = new LiteDatabase(DatabaseName);
            {
                var collection = db.GetCollection(TableName);
                var count = db.Engine.Insert(TableName, bsonDocList);
            }
        }

        /// <summary>
        /// Insert CSV data with or without Column Headers.
        /// </summary>
        /// <param name="csvRows">CSV List</param>
        /// <param name="headerList">Header List.  NULL if no Column Headers supplied.</param>
        public int InsertCsv(List<string> csvRows, List<string> headerList)
        {
            var insertedRows = 0;
            var bsonDocList = CreateBsonDocumentList(csvRows, headerList);

            using var db = new LiteDatabase(DatabaseName);
            {
                var collection = db.GetCollection(TableName);
                insertedRows = db.Engine.Insert(TableName, bsonDocList);
            }
            return insertedRows;
        }

        public int GetRowCount()
        {
            using var db = new LiteDatabase(DatabaseName);
            {
                var collection = db.GetCollection(TableName);
                var rowCount = collection.Count();
                return rowCount;
            }
        }

        public BsonDocument GetNext()
        {
            BsonDocument row;
            using var db = new LiteDatabase(DatabaseName);
            {
                var collection = db.GetCollection(TableName);

                row = collection.FindOne(Query.All(Query.Ascending));

                if (row != null)
                {
                    var deleted = collection.Delete(row.FirstOrDefault().Value.AsObjectId);
                    if (!deleted)
                    {
                        throw new Exception($"Row was not deleted!: {row.AsString}");
                    }
                }
            }
            return row;
        }

        private static List<BsonDocument> CreateBsonDocumentList(List<string> csvRows, List<string> headerList)
        {
            var docList = new List<BsonDocument>();
            List<string> headers = null;
            int headerRow = 0;

            if (headerList != null)
            {
                headers = new List<string>(headerList);
                headerRow = 1;
            }

            var rowCount = 0;
            foreach (var row in csvRows.Skip(headerRow))
            {
                var bsonArray = new BsonArray();

                // Split the csv row into columns
                var columns = row.Split(',').ToList();

                var headerCount = 0;
                foreach (var column in columns)
                {
                    var columnName = headerRow > 0
                        ? headers[headerCount]
                        : $"Field_{headerCount + 1}";

                    var dict = new Dictionary<string, BsonValue>
                    {
                        { columnName, new BsonValue(column) }
                    };
                    headerCount++;
                    bsonArray.Add(new BsonValue(dict));
                }

                rowCount++;
                var doc = new BsonDocument
                {
                    { "Row", bsonArray }
                };
                docList.Add(doc);
            }

            return docList;
        }

    }
}
