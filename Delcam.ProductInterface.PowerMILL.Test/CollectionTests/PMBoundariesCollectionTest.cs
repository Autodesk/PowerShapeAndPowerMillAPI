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
        public void CreateRestBoundaryTest()
        {
            _powerMill.LoadProject(TestFiles.ToolProperties);
            var tool = _powerMill.ActiveProject.Tools.GetByName("EndMill");
            var refTool = _powerMill.ActiveProject.Tools.GetByName("TipRadiused");
            var before = _powerMill.ActiveProject.Boundaries.Count;
            
            //Create RestBoundary with normal thickness
            var rest = _powerMill.ActiveProject.Boundaries.CreateRestBoundary(0.1, 0.2, 0.3, false, 0.4, 0.5, tool, refTool);
            Assert.IsNotNull(rest, "A rest boundary wasn't not created, please fix it.");
            Assert.AreEqual(before + 1, _powerMill.ActiveProject.Boundaries.Count);

            //Create RestBoundary with axial thickness
            rest = _powerMill.ActiveProject.Boundaries.CreateRestBoundary(0.1, 0.2, 0.3, true, 0.4, 0.5, tool, refTool);
            Assert.IsNotNull(rest, "A rest boundary wasn't not created, please fix it.");
            Assert.AreEqual(before + 2, _powerMill.ActiveProject.Boundaries.Count);
        }

        [Test]
        public void CreateBlockBoundaryTest()
        {
            _powerMill.LoadProject(TestFiles.SimplePmProject1);            
            var before = _powerMill.ActiveProject.Boundaries.Count;

            //Test boundary creation with no block
            _powerMill.ActiveProject.DeleteBlock();
            var block = _powerMill.ActiveProject.Boundaries.CreateBlockBoundary();
            Assert.IsNull(block, "Block boundary created although there is no block");

            //Test boundary creation with block
            _powerMill.ActiveProject.CreateBlock("cowling", 0);
            block = _powerMill.ActiveProject.Boundaries.CreateBlockBoundary();
            Assert.IsNotNull(block, "A block boundary wasn't not created, please fix it.");
            Assert.AreEqual(before + 1, _powerMill.ActiveProject.Boundaries.Count);
        }

        [Test]
        public void CreateSelectedSurfaceBoundaryTest()
        {
            _powerMill.LoadProject(TestFiles.ToolProperties);
            var tool = _powerMill.ActiveProject.Tools.GetByName("EndMill");           
            var before = _powerMill.ActiveProject.Boundaries.Count;

            //Create Selected Surface Boundary with normal thickness, top and roll over
            var selected = _powerMill.ActiveProject.Boundaries.CreateSelectedSurfaceBoundary(true, true, 0.1, false, 0.2, 0.3, tool);
            Assert.IsNotNull(selected, "A selected surface boundary wasn't not created, please fix it.");
            Assert.AreEqual(before + 1, _powerMill.ActiveProject.Boundaries.Count);

            //Create Selected Surface Boundary with axial thickness, without top and without roll over            
            selected = _powerMill.ActiveProject.Boundaries.CreateSelectedSurfaceBoundary(false, false, 0.1, true, 0.2, 0.3, tool);
            Assert.IsNotNull(selected, "A selected surface boundary wasn't not created, please fix it.");
            Assert.AreEqual(before + 2, _powerMill.ActiveProject.Boundaries.Count);
        }

        [Test]
        public void CreateShallowBoundaryTest()
        {
            _powerMill.LoadProject(TestFiles.ToolProperties);
            var tool = _powerMill.ActiveProject.Tools.GetByName("EndMill");
            var before = _powerMill.ActiveProject.Boundaries.Count;

            //Create Shallow Boundary with normal thickness, top and roll over
            var shallow = _powerMill.ActiveProject.Boundaries.CreateShallowBoundary(20.0, 10.0, 0.1, false, 0.2, 0.3, tool);
            Assert.IsNotNull(shallow, "A Shallow boundary wasn't not created, please fix it.");
            Assert.AreEqual(before + 1, _powerMill.ActiveProject.Boundaries.Count);

            //Create Shallow Boundary with axial thickness, without top and without roll over            
            shallow = _powerMill.ActiveProject.Boundaries.CreateShallowBoundary(20.0, 10.0, 0.1, true, 0.2, 0.3, tool);
            Assert.IsNotNull(shallow, "A Shallow boundary wasn't not created, please fix it.");
            Assert.AreEqual(before + 2, _powerMill.ActiveProject.Boundaries.Count);
        }

        [Test]
        public void CreateCollisionSafeBoundaryTest()
        {
            _powerMill.LoadProject(TestFiles.ToolProperties);
            var tool = _powerMill.ActiveProject.Tools.GetByName("EndMill");
            var before = _powerMill.ActiveProject.Boundaries.Count;

            //Create Collision Safe Boundary with normal thickness
            var collSafe = _powerMill.ActiveProject.Boundaries.CreateCollisionSafeBoundary(20.0, 10.0, 0.1, false, 0.2, 0.3, tool);
            Assert.IsNotNull(collSafe, "A Collision Safe boundary wasn't not created, please fix it.");
            Assert.AreEqual(before + 1, _powerMill.ActiveProject.Boundaries.Count);

            //Create Collision SAfe Boundary with axial thickness           
            collSafe = _powerMill.ActiveProject.Boundaries.CreateCollisionSafeBoundary(20.0, 10.0, 0.1, true, 0.2, 0.3, tool);
            Assert.IsNotNull(collSafe, "A Collision Safe boundary wasn't not created, please fix it.");
            Assert.AreEqual(before + 2, _powerMill.ActiveProject.Boundaries.Count);
        }

        [Test]
        public void CreateStockModelRestBoundaryTest()
        {
            // Create Test environment
            _powerMill.LoadProject(TestFiles.SimplePmProject1);
            _powerMill.ActiveProject.StockModels.CreateStockmodel("Test stockmodel 1");
            _powerMill.ActiveProject.StockModels.ActiveItem.ApplyBlock();
            _powerMill.ActiveProject.Toolpaths.GetByName("alpha").IsActive = true;
            _powerMill.ActiveProject.StockModels.ActiveItem.ApplyToolpathFirst();
            _powerMill.ActiveProject.StockModels.ActiveItem.ApplyToolLast();

            var stockmodel = _powerMill.ActiveProject.StockModels.ActiveItem;
            var tool = _powerMill.ActiveProject.Tools.GetByName("12 ball");
            var before = _powerMill.ActiveProject.Boundaries.Count;

            //Create StockModel Rest Boundary with normal thickness
            var stockRest = _powerMill.ActiveProject.Boundaries.CreateStockModelRestBoundary(stockmodel, "12 ball", 0.1, 0.2, 0.3, false, 0.4, 0.5, tool);
            Assert.IsNotNull(stockRest, "A StockModel Rest boundary wasn't not created, please fix it.");
            Assert.AreEqual(before + 1, _powerMill.ActiveProject.Boundaries.Count);

            //Create StockModel Rest Boundary with axial thickness           
            stockRest = _powerMill.ActiveProject.Boundaries.CreateStockModelRestBoundary(stockmodel, "12 ball", 0.1, 0.2, 0.3, true, 0.4, 0.5, tool);
            Assert.IsNotNull(stockRest, "A Collision Safe boundary wasn't not created, please fix it.");
            Assert.AreEqual(before + 2, _powerMill.ActiveProject.Boundaries.Count);
        }

        [Test]
        public void CreateContactPointBoundaryTest()
        {
            _powerMill.LoadProject(TestFiles.PMwithNcProgram);            
            var before = _powerMill.ActiveProject.Boundaries.Count;

            //Create Contact Point Boundary from Boundary
            var boundary = _powerMill.ActiveProject.Boundaries.GetByName("toolpath_job_Boundary");
            var contactPoint = _powerMill.ActiveProject.Boundaries.CreateContactPointBoundary(boundary, 0.1, 0.2);
            Assert.IsNotNull(contactPoint, "A Contact Point boundary from Boundary wasn't not created, please fix it.");
            Assert.AreEqual(before + 1, _powerMill.ActiveProject.Boundaries.Count);
            
            //Create Contact Point Boundary from Pattern
            var pattern = _powerMill.ActiveProject.Patterns.GetByName("PreppArea");
            contactPoint = _powerMill.ActiveProject.Boundaries.CreateContactPointBoundary(pattern, 0.1, 0.2);
            Assert.IsNotNull(contactPoint, "A Contact Point boundary from Pattern wasn't not created, please fix it.");
            Assert.AreEqual(before + 2, _powerMill.ActiveProject.Boundaries.Count);

            //Create Contact Point Boundary from Toolpath
            var toolpath = _powerMill.ActiveProject.Toolpaths.GetByName("PIN_MACHINE_1");
            contactPoint = _powerMill.ActiveProject.Boundaries.CreateContactPointBoundary(toolpath, 0.1, 0.2);
            Assert.IsNotNull(contactPoint, "A Contact Point boundary from Toolpath wasn't not created, please fix it.");
            Assert.AreEqual(before + 3, _powerMill.ActiveProject.Boundaries.Count);

            //Create Contact Point Boundary from Model
            var model = _powerMill.ActiveProject.Models.GetByName("toolpath_job_Pin1");
            model.AddToSelection(true);
            contactPoint = _powerMill.ActiveProject.Boundaries.CreateContactPointBoundary(0.1, 0.2);
            Assert.IsNotNull(contactPoint, "A Contact Point boundary from Model wasn't not created, please fix it.");
            Assert.AreEqual(before + 4, _powerMill.ActiveProject.Boundaries.Count);
        }

        [Test]
        public void CreateContactConversionBoundaryTest()
        {
            _powerMill.LoadProject(TestFiles.PMwithNcProgram);
            var tool = _powerMill.ActiveProject.Tools.GetByName("1mm_Ballnose");
            var boundary = _powerMill.ActiveProject.Boundaries.GetByName("toolpath_job_Boundary");
            var conBoundary = _powerMill.ActiveProject.Boundaries.CreateContactPointBoundary(boundary, 0.1, 0.1);
            var before = _powerMill.ActiveProject.Boundaries.Count;

            //Create Contact Conversion Boundary with normal thickness
            var conConversion = _powerMill.ActiveProject.Boundaries.CreateContactConversionBoundary(conBoundary, 0.1, 0.2, false, 0.3, 0.4, tool);
            Assert.IsNotNull(conConversion, "A Contact Conversion boundary wasn't not created, please fix it.");
            Assert.AreEqual(before + 1, _powerMill.ActiveProject.Boundaries.Count);

            //Create Contact Conversion Boundary with axial thickness           
            conConversion = _powerMill.ActiveProject.Boundaries.CreateContactConversionBoundary(conBoundary, 0.1, 0.2, true, 0.3, 0.4, tool);
            Assert.IsNotNull(conConversion, "A Collision Safe boundary wasn't not created, please fix it.");
            Assert.AreEqual(before + 2, _powerMill.ActiveProject.Boundaries.Count);
        }

        [Test]
        public void CreateBooleanOperationBoundaryTest()
        {
            _powerMill.RunMacro(Files.TestFiles.CreateBoundaries);

            var boundaryA = _powerMill.ActiveProject.Boundaries.GetByName("Boundary A");
            var boundaryB = _powerMill.ActiveProject.Boundaries.GetByName("Boundary B");
            var before = _powerMill.ActiveProject.Boundaries.Count;

            //Create Boolean Operation Boundary with Addition
            var boolBoundary = _powerMill.ActiveProject.Boundaries.CreateBooleanOperationBoundary(boundaryA, boundaryB, BoundaryBooleanTypes.Addition, 0.1);
            Assert.IsNotNull(boolBoundary, "A Boolean Operation boundary with Addition wasn't not created, please fix it.");
            Assert.AreEqual(before + 1, _powerMill.ActiveProject.Boundaries.Count);

            //Create Boolean Operation Boundary with Subtraction         
            boolBoundary = _powerMill.ActiveProject.Boundaries.CreateBooleanOperationBoundary(boundaryA, boundaryB, BoundaryBooleanTypes.Subtraction, 0.1);
            Assert.IsNotNull(boolBoundary, "A Boolean Operation boundary with Subtraction wasn't not created, please fix it.");
            Assert.AreEqual(before + 2, _powerMill.ActiveProject.Boundaries.Count);

            //Create Boolean Operation Boundary with Intersection         
            boolBoundary = _powerMill.ActiveProject.Boundaries.CreateBooleanOperationBoundary(boundaryA, boundaryB, BoundaryBooleanTypes.Intersection, 0.1);
            Assert.IsNotNull(boolBoundary, "A Boolean Operation boundary with Intersection wasn't not created, please fix it.");
            Assert.AreEqual(before + 3, _powerMill.ActiveProject.Boundaries.Count);
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
