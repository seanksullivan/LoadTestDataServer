using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DataAccess
{
    public class Data<T> where T: ILiteDbRow
    {
        public string DatabaseName { get; set; }
        public T DataObject { get; set; }
        public string TableName { get; set; }

        public Data(string databaseName, string tableName)
        {
            DatabaseName = databaseName;
            TableName = tableName;
        }

        public void Insert(T value)
        {
            LiteCollection<T> collection = null;
            using var db = new LiteDatabase(DatabaseName);
            {
                collection = db.GetCollection<T>(TableName);
                collection.Insert(value);
            }
        }

        public int InsertBulk(IEnumerable<T> objectList)
        {
            int recordsInserted = 0;
            LiteCollection<T> collection = null;
            using var db = new LiteDatabase(DatabaseName);
            {
                collection = db.GetCollection<T>(TableName);
                recordsInserted = collection.InsertBulk(objectList);
            }
            return recordsInserted;
        }

        public T GetNext()
        {
            T row;
            LiteCollection<T> collection = null;
            using var db = new LiteDatabase(DatabaseName);
            {
                collection = db.GetCollection<T>(TableName);
                row = collection.FindOne(Query.All(Query.Ascending));

                if (row != null)
                {
                    var wow = collection.Delete(r => r.Id == row.Id);
                }
            }
            return row;
        }

        /// <summary>
        /// Example: var value = Get(c => c.CustomerId == 15)
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public T Get(Expression<Func<T, bool>> predicate)
        {
            T row;
            LiteCollection<T> collection = null;
            using var db = new LiteDatabase(DatabaseName);
            {
                collection = db.GetCollection<T>(TableName);
                row = collection.FindOne(predicate);
            }
            return row;
        }
    }
}
