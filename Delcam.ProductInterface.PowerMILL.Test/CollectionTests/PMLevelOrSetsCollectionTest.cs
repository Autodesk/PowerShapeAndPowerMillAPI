// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using Autodesk.ProductInterface.PowerMILL;
using NUnit.Framework;

namespace Autodesk.ProductInterface.PowerMILLTest.CollectionTests
{
    [TestFixture]
    public class PMLevelOrSetsCollectionTest
    {
        private PMAutomation _powerMill;

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            _powerMill = new PMAutomation(InstanceReuse.UseExistingInstance);
            _powerMill.DialogsOff();
            _powerMill.CloseProject();
        }

        [TearDown]
        public void TearDown()
        {
            _powerMill.Reset();
        }

        [Test]
        public void AddLevelTest()
        {
            var itemName = "TestItem";
            var level = _powerMill.ActiveProject.LevelsAndSets.CreateLevel(itemName);

            Assert.That(level, Is.Not.Null);
            Assert.That(level.Name, Is.EqualTo(itemName));
            Assert.That(level.Exists, Is.True);
        }

        [Test]
        public void AddSetTest()
        {
            var itemName = "TestItem";
            var set = _powerMill.ActiveProject.LevelsAndSets.CreateSet(itemName);

            Assert.That(set, Is.Not.Null);
            Assert.That(set.Name, Is.EqualTo(itemName));
            Assert.That(set.Exists, Is.True);
        }

        [Test]
        public void GetNewEntityNameTest()
        {
            //No entity exists, should return "1" and "NewName"
            Assert.That(_powerMill.ActiveProject.LevelsAndSets.GetNewEntityName(), Is.EqualTo("1"));
            Assert.That(_powerMill.ActiveProject.LevelsAndSets.GetNewEntityName("NewName"), Is.EqualTo("NewName"));

            //Create a new entity
            var level = _powerMill.ActiveProject.LevelsAndSets.CreateLevel("1");

            //1 entity exists, should return "2"
            Assert.That(_powerMill.ActiveProject.LevelsAndSets.GetNewEntityName(), Is.EqualTo("2"));

            //Rename the entity
            level.Name = "MyPrefix";

            //1 entity called "MyPrefix" exists, should return "NewName" and "MyPrefix_1"
            Assert.That(_powerMill.ActiveProject.LevelsAndSets.GetNewEntityName("NewName"), Is.EqualTo("NewName"));
            Assert.That(_powerMill.ActiveProject.LevelsAndSets.GetNewEntityName("MyPrefix"), Is.EqualTo("MyPrefix_1"));

            //Delete the entity
            _powerMill.ActiveProject.LevelsAndSets.Remove(level);
        }
    }
}