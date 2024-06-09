using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Devsense.PHP.Syntax;
using Devsense.PHP.Syntax.Ast;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PropertyCollectionTests
{
    [TestClass]
    public partial class ThreadingTests
    {
        public TestContext TestContext { get; set; }

        int _counter = 0;
        Dictionary<object, object> _data = CreateTestData();

        static Dictionary<object,object> CreateTestData()
        {
            var data = new Dictionary<object, object>();

            data[typeof(int)] = 123;
            data[typeof(long)] = 456L;
            data[typeof(string)] = "123";
            data[typeof(bool)] = true;
            data[typeof(double)] = 123.456;
            data[typeof(char[])] = new[] { 'a', 'b', 'c' };
            data[typeof(int[])] = new[] { 1, 2, 3 };
            data[typeof(double[])] = new[] { 1.1, 2.2, 3.3 };
            data[typeof(object)] = new object();

            return data;
        }

        [TestMethod]
        public void ThreadingTest()
        {
            var collection = new PropertyCollectionClass();

            // create threads accessing the collection

            for (int i = 0; i < 100; i++)
            {
                Interlocked.Increment(ref _counter);
                new Thread(AddRemoveItems).Start(collection);
            }

            // wait for all threads to finish
            while (_counter > 0)
            {
                Thread.Sleep(1);
                Interlocked.MemoryBarrier();
            }

            // done
            Assert.AreEqual(0, collection.Count);
        }

        [TestMethod]
        public void ReadWriteTest()
        {
            var collection = new PropertyCollectionClass();

            for (long i = 0; i < 100; i++)
            {
                AddRemoveItems(collection);
            }
        }

        void AddRemoveItems(object collectionObj)
        {
            var collection = (PropertyCollectionClass)collectionObj;
            Debug.Assert(collection != null);
            Debug.Assert(_data.Count != 0);

            //
            for (int i = 0; i < 100; i++)
            {
                foreach (var pair in _data)
                {
                    collection.SetProperty(pair.Key, pair.Value);
                }

                foreach (var pair in _data)
                {
                    object obj;
                    collection.TryGetProperty(pair.Key, out obj);
                }

                foreach (var pair in _data)
                {
                    collection.RemoveProperty(pair.Key);
                }

                foreach (var pair in _data)
                {
                    object obj;
                    collection.TryGetProperty(pair.Key, out obj);
                }
            }

            //
            Interlocked.Decrement(ref _counter);
        }
    }
}
