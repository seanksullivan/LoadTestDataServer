using DataAccess.UnitTests.TestObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;

namespace DataAccess.UnitTests
{
    [TestClass]
    public class DataAccessTests
    {
        private static Random _random = new Random();
        private static List<string> _databaseFilenameList = new List<string>();

        [ClassCleanup()]
        public static void ClassCleanup()
        {
            // Let's cycle through the list of 'test' Database files, and delete them
            foreach (var filename in _databaseFilenameList)
            {
                try
                {
                    if (File.Exists(filename))
                    {
                        File.Delete(filename);
                    }
                }
                catch
                {
                    // well, we tried...
                }
            }
        }

        [TestMethod]
        public void LoadBulk_VerifyCount()
        {
            // Arrange
            var databaseFilepath = GetDatabaseFile();

            // Create a bulk Customer List
            var customerList = new List<Customer>();

            for (var cnt = 0; cnt < 10000; cnt++)
            {
                var name = $"Bob{RandomString(10)}";
                customerList.Add(new Customer { CustomerId = Guid.NewGuid(), Name = name });
            }

            // Construct the class
            var data = new Data<Customer>(databaseFilepath, "customers");

            // Act
            var recordsInserted = data.InsertBulk(customerList);

            // Assert
            Assert.AreEqual(customerList.Count(), recordsInserted);
        }

        [TestMethod]
        public void LoadBulk_GetRecord_VerifyRecord()
        {
            // Arrange
            var databaseFilepath = GetDatabaseFile();

            // Create a bulk Customer List
            var customerList = new List<Customer>();

            for (var cnt = 0; cnt < 10000; cnt++)
            {
                var name = $"Bob{RandomString(10)}";
                customerList.Add(new Customer { CustomerId = Guid.NewGuid(), Name = name });
            }

            // Construct the class
            var data = new Data<Customer>(databaseFilepath, "customers");

            // Act
            var recordsInserted = data.InsertBulk(customerList);

            // Get one record
            var customerAcquired = data.Get(c => c.CustomerId == customerList[100].CustomerId);

            // Assert
            Assert.AreEqual(customerList.Count(), recordsInserted);
            Assert.AreEqual(customerList[100].CustomerId, customerAcquired.CustomerId);
            Assert.AreEqual(customerList[100].Name, customerAcquired.Name);
        }

        [TestMethod]
        public void GetNext_VerifyDelete()
        {
            // Arrange
            var databaseFilepath = GetDatabaseFile();

            // Create a bulk Customer List
            var customerList = new List<Customer>();

            for (var cnt = 0; cnt < 10000; cnt++)
            {
                var name = $"Bob{RandomString(10)}";
                customerList.Add(new Customer { CustomerId = Guid.NewGuid(), Name = name, Count = cnt + 1 });
            }

            // Construct the class
            var data = new Data<Customer>(databaseFilepath, "customers");

            // Act
            var recordsInserted = data.InsertBulk(customerList);

            // Get one record

            var customerAcquired1 = data.GetNext();

            var customerAcquired2 = data.GetNext();

            // Assert
            Assert.AreEqual(1, customerAcquired1.Id);
            Assert.AreEqual(2, customerAcquired2.Id);
        }


        private static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        private static string GetDatabaseFile()
        {
            var databaseFilepath = Path.GetTempFileName();
            _databaseFilenameList.Add(databaseFilepath);

            return databaseFilepath;
        }
    }
}
