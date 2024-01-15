// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using Autodesk.FileSystem;
using Autodesk.ProductInterface.PowerMILL;
using Autodesk.ProductInterface.PowerMILLTest.Files;
using NUnit.Framework;

namespace Autodesk.ProductInterface.PowerMILLTest.CollectionTests
{
    public class PMModelsCollectionTest
    {
        private PMAutomation _powerMill;

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            _powerMill = new PMAutomation(InstanceReuse.UseExistingInstance);
            _powerMill.DialogsOff();
            _powerMill.CloseProject();
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
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

        #region Tests

        [Test]
        public void CreateModelTest()
        {
            var model = _powerMill.ActiveProject.Models.CreateModel(TestFiles.SingleMesh);
            Assert.IsNotNull(model);
        }

        [Test]
        public void DeleteSelectedSurfacesTest()
        {
            var model = _powerMill.ActiveProject.Models.CreateModel(TestFiles.SingleMesh);
            model.AddToSelection(true);
            _powerMill.ActiveProject.Models.DeleteSelectedSurfaces();
            _powerMill.ActiveProject.Models.DeleteEmptyModels();
            Assert.AreEqual(0, _powerMill.ActiveProject.Models.Count);
        }

        [Test]
        public void DeleteSelectedSurfacesNothingSelectedTest()
        {
            var model = _powerMill.ActiveProject.Models.CreateModel(TestFiles.SingleMesh);
            _powerMill.ActiveProject.Models.DeleteSelectedSurfaces();
            _powerMill.ActiveProject.Models.DeleteEmptyModels();
            Assert.AreEqual(1, _powerMill.ActiveProject.Models.Count);
        }

        [Test]
        public void DeleteEmptyModelsTest()
        {
            var model = _powerMill.ActiveProject.Models.CreateModel(TestFiles.SingleMesh);
            model.AddToSelection(true);
            _powerMill.ActiveProject.Models.DeleteSelectedSurfaces();
            _powerMill.ActiveProject.Models.DeleteEmptyModels();
            Assert.AreEqual(0, _powerMill.ActiveProject.Models.Count);
        }

        [Test]
        public void DeleteEmptyModelsWithModelNotEmptyTest()
        {
            var model = _powerMill.ActiveProject.Models.CreateModel(TestFiles.SingleMesh);
            _powerMill.ActiveProject.Models.DeleteSelectedSurfaces();
            _powerMill.ActiveProject.Models.DeleteEmptyModels();
            Assert.AreEqual(1, _powerMill.ActiveProject.Models.Count);
        }

        [Test]
        public void ExportModelsTest()
        {
            var project = _powerMill.LoadProject(TestFiles.ExistingDentMillMasterProject);
            var exportFile = File.CreateTemporaryFile("dmt");
            project.Models.ExportAllModels(exportFile);

            // Reset quickly to ensure the busy flag is probably working
            _powerMill.Reset();

            // Check the existance of the DMT file
            Assert.That(exportFile.Exists);
            exportFile.Delete();
        }

        [Test]
        public void GetNewEntityNameTest()
        {
            //No entity exists, should return "1" and "NewName"
            Assert.That(_powerMill.ActiveProject.Models.GetNewEntityName(), Is.EqualTo("1"));
            Assert.That(_powerMill.ActiveProject.Models.GetNewEntityName("NewName"), Is.EqualTo("NewName"));

            //Create a new entity
            var model = _powerMill.ActiveProject.Models.CreateModel(TestFiles.SingleMesh);
            model.Name = "1";

            //1 entity exists, should return "2"
            Assert.That(_powerMill.ActiveProject.Models.GetNewEntityName(), Is.EqualTo("2"));

            //Rename the entity
            model.Name = "MyPrefix";

            //1 entity called "MyPrefix" exists, should return "NewName" and "MyPrefix_1"
            Assert.That(_powerMill.ActiveProject.Models.GetNewEntityName("NewName"), Is.EqualTo("NewName"));
            Assert.That(_powerMill.ActiveProject.Models.GetNewEntityName("MyPrefix"), Is.EqualTo("MyPrefix_1"));

            //Delete the entity
            _powerMill.ActiveProject.Models.Remove(model);
        }

        [Test]
        public void DuplicateModelTest()
        {
            // Build server has a weird thing where you need to put dialogs on for the test to work
            _powerMill.DialogsOn();
            var model = _powerMill.ActiveProject.Models.CreateModel(TestFiles.SingleMesh);
            var duplicatedModel = model.Duplicate();
            Assert.IsNotNull(duplicatedModel.BoundingBox);
        }
        #endregion
    }
}