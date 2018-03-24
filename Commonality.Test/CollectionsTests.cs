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
            roc.CollectionChanged += Coll_CollectionChanged;
        }
        [TestMethod]
        public void ROC_Add()
        {
            roc.AddRange(new[] { "A", "B", "C" });

            Assert.AreEqual(1, changedevents.Count);
            Assert.AreEqual("Reset", changedevents.Last());
        }

        [TestMethod]
        public void ROC_Insert()
        {
            roc.AddRange(new[] { "A", "B", "C" });
            roc.InsertRange(1,new[] { "D", "E", "F" });

            Assert.AreEqual(2, changedevents.Count);
            Assert.AreEqual("Reset", changedevents.Last());
            Assert.AreEqual("A", roc[0]);
            Assert.AreEqual("D", roc[1]);
            Assert.AreEqual("B", roc[4]);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ROC_InsertNull()
        {
            roc.InsertRange(0,null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ROC_AddNull()
        {
            roc.AddRange(null);
        }

        private void Coll_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            changedevents.Add(e.Action.ToString());
        }
    }

}
