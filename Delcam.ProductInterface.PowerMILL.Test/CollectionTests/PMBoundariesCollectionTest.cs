// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using System.Linq;
using Autodesk.ProductInterface.PowerMILL;
using Autodesk.ProductInterface.PowerMILLTest.Files;
using NUnit.Framework;

namespace Autodesk.ProductInterface.PowerMILLTest.CollectionTests
{
    [TestFixture]
    public class PMBoundariesCollectionTest
    {
        private PMAutomation _powerMill;
        private const double TOLERANCE = 0.00001;

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
            try
            {
                _powerMill.CloseProject();
            }
            catch (Exception)
            {
            }
        }

        [Test]
        public void CreateSilhouetteBoundaryTest()
        {
            var before = _powerMill.ActiveProject.Boundaries.Count;
            _powerMill.ActiveProject.Models.CreateModel(TestFiles.SingleMesh);
            var importedModelName = _powerMill.ActiveProject.Models.Last().Name;
            var silhouette = _powerMill.ActiveProject.Boundaries.CreateSilhouetteBoundary(importedModelName, 10, 5, 0.1);

            Assert.IsNotNull(silhouette, "A silhouette boundary wasn't not created, please fix it.");
            Assert.AreEqual(before + 1, _powerMill.ActiveProject.Boundaries.Count);
            Assert.That(silhouette.ToPolylines()[0].Length.Value, Is.EqualTo(391.312086465231).Within(TOLERANCE));
        }

        [Test]
        public void CreateTwoSilhouettesBoundaryTest()
        {
            var before = _powerMill.ActiveProject.Boundaries.Count;
            _powerMill.ActiveProject.Models.CreateModel(TestFiles.SingleMesh);
            var importedModelName = _powerMill.ActiveProject.Models.Last().Name;
            var silhouette = _powerMill.ActiveProject.Boundaries.CreateSilhouetteBoundary(importedModelName, 10, 5, 0.1);

            Assert.IsNotNull(silhouette, "A silhouette boundary wasn't created, please fix it.");
            Assert.AreEqual(before + 1, _powerMill.ActiveProject.Boundaries.Count);

            var silhouette2 = _powerMill.ActiveProject.Boundaries.CreateSilhouetteBoundary(importedModelName, 10, 5, 0.1);

            Assert.IsNotNull(silhouette2, "A silhouette boundary wasn't created, please fix it.");
            Assert.AreEqual(before + 2, _powerMill.ActiveProject.Boundaries.Count);
        }

        [Test]
        public void CreateEmptyBoundaryTest()
        {
            var boundary = _powerMill.ActiveProject.Boundaries.CreateEmptyBoundary();
            Assert.That(boundary, Is.Not.Null);
            Assert.That(boundary.Exists, Is.True);
        }

        [Test]
        public void GetNewEntityNameTest()
        {
            //No entity exists, should return "1" and "NewName"
            Assert.That(_powerMill.ActiveProject.Boundaries.GetNewEntityName(), Is.EqualTo("1"));
            Assert.That(_powerMill.ActiveProject.Boundaries.GetNewEntityName("NewName"), Is.EqualTo("NewName"));

            //Create a new entity
            var boundary = _powerMill.ActiveProject.Boundaries.CreateEmptyBoundary();

            //1 entity exists, should return "2"
            Assert.That(_powerMill.ActiveProject.Boundaries.GetNewEntityName(), Is.EqualTo("2"));

            //Rename the entity
            boundary.Name = "MyPrefix";

            //1 entity called "MyPrefix" exists, should return "NewName" and "MyPrefix_1"
            Assert.That(_powerMill.ActiveProject.Boundaries.GetNewEntityName("NewName"), Is.EqualTo("NewName"));
            Assert.That(_powerMill.ActiveProject.Boundaries.GetNewEntityName("MyPrefix"), Is.EqualTo("MyPrefix_1"));

            //Delete the entity
            _powerMill.ActiveProject.Boundaries.Remove(boundary);
        }
    }
}