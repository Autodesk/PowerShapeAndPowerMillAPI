// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using Autodesk.Geometry;
using Autodesk.ProductInterface.PowerMILL;
using NUnit.Framework;

namespace Autodesk.ProductInterface.PowerMILLTest.CollectionTests
{
    public class PMPatternsCollectionTest
    {
        private PMAutomation _powerMILL;

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            _powerMILL = new PMAutomation(InstanceReuse.UseExistingInstance);
            _powerMILL.DialogsOff();
            _powerMILL.CloseProject();
        }

        [TearDown]
        public void TearDown()
        {
            try
            {
                _powerMILL.CloseProject();
            }
            catch (Exception)
            {
            }
        }

        [Test]
        public void CreatePatternTest()
        {
            var beforeCount = _powerMILL.ActiveProject.Patterns.Count;
            var polyline = new Polyline {new Point(), new Point(100, 100, 0), new Point(-100, 100, 0), new Point()};
            var pattern = _powerMILL.ActiveProject.Patterns.CreatePattern(polyline);

            Assert.IsNotNull(pattern);
            Assert.AreEqual(beforeCount + 1, _powerMILL.ActiveProject.Patterns.Count);
            Assert.AreEqual((MM) 482.842712474619, pattern.ToPolylines()[0].Length);
        }

        [Test]
        public void CreateTwoPatternsTest()
        {
            var beforeCount = _powerMILL.ActiveProject.Patterns.Count;
            var polyline = new Polyline {new Point(), new Point(100, 100, 0), new Point(-100, 100, 0), new Point()};
            var pattern = _powerMILL.ActiveProject.Patterns.CreatePattern(polyline);

            Assert.IsNotNull(pattern);
            Assert.AreEqual(beforeCount + 1, _powerMILL.ActiveProject.Patterns.Count);

            var pattern2 = _powerMILL.ActiveProject.Patterns.CreatePattern(polyline);

            Assert.IsNotNull(pattern2);
            Assert.AreEqual(beforeCount + 2, _powerMILL.ActiveProject.Patterns.Count);
        }

        [Test]
        public void CreateEmptyPatternTest()
        {
            var pattern = _powerMILL.ActiveProject.Patterns.CreateEmptyPattern();
            Assert.That(pattern, Is.Not.Null);
            Assert.That(pattern.Exists, Is.True);
        }

        [Test]
        public void GetNewEntityNameTest()
        {
            //No entity exists, should return "1" and "NewName"
            Assert.That(_powerMILL.ActiveProject.Patterns.GetNewEntityName(), Is.EqualTo("1"));
            Assert.That(_powerMILL.ActiveProject.Patterns.GetNewEntityName("NewName"), Is.EqualTo("NewName"));

            //Create a new entity
            var pattern = _powerMILL.ActiveProject.Patterns.CreateEmptyPattern();

            //1 entity exists, should return "2"
            Assert.That(_powerMILL.ActiveProject.Patterns.GetNewEntityName(), Is.EqualTo("2"));

            //Rename the entity
            pattern.Name = "MyPrefix";

            //1 entity called "MyPrefix" exists, should return "NewName" and "MyPrefix_1"
            Assert.That(_powerMILL.ActiveProject.Patterns.GetNewEntityName("NewName"), Is.EqualTo("NewName"));
            Assert.That(_powerMILL.ActiveProject.Patterns.GetNewEntityName("MyPrefix"), Is.EqualTo("MyPrefix_1"));

            //Delete the entity
            _powerMILL.ActiveProject.Patterns.Remove(pattern);
        }
    }
}