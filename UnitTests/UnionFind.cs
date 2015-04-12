using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using hw2;

namespace UnitTests
{
    [TestClass]
    public class UnionFindTests
    {
        [TestMethod]
        public void UnionTwoRoots()
        {
            var uf = new UnionFind(2);

            Assert.AreEqual(0, uf.Find(0));
            Assert.AreEqual(1, uf.Find(1));

            uf.Union(0, 1);

            Assert.AreEqual(uf.Find(1), uf.Find(0));
        }

        [TestMethod]
        public void UnionDifferentSizeRoots()
        {
            var uf = new UnionFind(3);
            uf.Union(0, 1);
            var newUnionRoot = uf.Find(0);

            uf.Union(newUnionRoot, 2);
            Assert.AreEqual(newUnionRoot, uf.Find(2));
        }

        [TestMethod]
        public void UnionDifferentSizeNoRoots()
        {
            var uf = new UnionFind(7);
            uf.Union(0, 1);
            uf.Union(0, 2); // now 2 is definetly not a root

            uf.Union(3, 4);
            var biggerUnionRoot = uf.Find(3);
            uf.Union(4, 5);
            uf.Union(5, 6);
            
            uf.Union(2, 6); // union deep children of two unions of different size
            Assert.AreEqual(biggerUnionRoot, uf.Find(0));
            Assert.AreEqual(biggerUnionRoot, uf.Find(1));
            Assert.AreEqual(biggerUnionRoot, uf.Find(2));
            Assert.AreEqual(biggerUnionRoot, uf.Find(3));
            Assert.AreEqual(biggerUnionRoot, uf.Find(4));
            Assert.AreEqual(biggerUnionRoot, uf.Find(5));
            Assert.AreEqual(biggerUnionRoot, uf.Find(6));
        }
    }
}
