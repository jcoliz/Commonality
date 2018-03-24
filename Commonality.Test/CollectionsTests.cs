using Commonality;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ManiaLabs.Portable.Tests
{
    [TestClass]
    public class CollectionsTests
    {
        List<String> changedevents;
        RangeObservableCollection<string> roc;

        [TestInitialize]
        public void Setup()
        {
            changedevents = new List<string>();

            roc = new RangeObservableCollection<string>();
            roc.AddRange(new[] { "A1", "B1", "B2", "C1", "C2", "C3" });
        }
        [TestMethod]
        public void ROC_Add()
        {
            var coll = new RangeObservableCollection<string>();
            coll.CollectionChanged += Coll_CollectionChanged;

            coll.AddRange(new[] { "A", "B", "C" });

            Assert.AreEqual(1, changedevents.Count);
            Assert.AreEqual("Reset", changedevents.Last());
        }

        [TestMethod]
        public void ROC_Insert()
        {
            var coll = new RangeObservableCollection<string>();
            coll.CollectionChanged += Coll_CollectionChanged;

            coll.AddRange(new[] { "A", "B", "C" });
            coll.InsertRange(1,new[] { "D", "E", "F" });

            Assert.AreEqual(2, changedevents.Count);
            Assert.AreEqual("Reset", changedevents.Last());
            Assert.AreEqual("A", coll[0]);
            Assert.AreEqual("D", coll[1]);
            Assert.AreEqual("B", coll[4]);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ROC_InsertNull()
        {
            var coll = new RangeObservableCollection<string>();
            coll.CollectionChanged += Coll_CollectionChanged;

            coll.InsertRange(0,null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ROC_AddNull()
        {
            var coll = new RangeObservableCollection<string>();

            coll.AddRange(null);
        }


        private void Coll_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            changedevents.Add(e.Action.ToString());
        }
    }

}
