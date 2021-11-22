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
using System.Collections.Generic;
using Autodesk.Geometry;
using Autodesk.ProductInterface.PowerMILL;
using Autodesk.ProductInterface.PowerMILLTest.Files;
using NUnit.Framework;

namespace Autodesk.ProductInterface.PowerMILLTest.PMEntityTests
{
    [TestFixture]
    public class PMStockmodelTests
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


        #region Test properties

        #endregion


        #region Test operations

        [Test]
        public void DrawAndUndrawAllStockmodelsTest()
        {
            _powerMill.ActiveProject.StockModels.CreateStockmodel("Test stockmodel 1");
            _powerMill.ActiveProject.StockModels.CreateStockmodel("Test stockmodel 2");
            _powerMill.ActiveProject.StockModels.DrawAll();
            _powerMill.ActiveProject.StockModels.UndrawAll();
            Assert.IsTrue(true);
        }
                
        [Test]
        public void ApplyFunctionsTest()
        {
            // Create Test environment
            _powerMill.LoadProject(TestFiles.SimplePmProject1);
            _powerMill.ActiveProject.StockModels.CreateStockmodel("Test stockmodel 1");
            
            // Test ApplyBlock function
            _powerMill.ActiveProject.StockModels.ActiveItem.ApplyBlock();
            List<string> stockModelStates = _powerMill.ActiveProject.StockModels.ActiveItem.States;
            Assert.AreEqual(1, stockModelStates.Count());

            // Test ApplyToolpathFirst function
            _powerMill.ActiveProject.Toolpaths.GetByName("alpha").IsActive = true;
            _powerMill.ActiveProject.StockModels.ActiveItem.ApplyToolpathFirst();
            stockModelStates = _powerMill.ActiveProject.StockModels.ActiveItem.States;
            Assert.AreEqual(2, stockModelStates.Count());

            // Test ApplyToolpathLast function
            _powerMill.ActiveProject.Toolpaths.GetByName("brava").IsActive = true;
            _powerMill.ActiveProject.StockModels.ActiveItem.ApplyToolpathLast();
            stockModelStates = _powerMill.ActiveProject.StockModels.ActiveItem.States;
            Assert.AreEqual(3, stockModelStates.Count());

            // Test ApplyToolFirst function
            _powerMill.ActiveProject.Tools.GetByName("12 ball").IsActive = true;
            _powerMill.ActiveProject.StockModels.ActiveItem.ApplyToolFirst();
            stockModelStates = _powerMill.ActiveProject.StockModels.ActiveItem.States;
            Assert.AreEqual(4, stockModelStates.Count());

            // Test ApplyToolLast function
            _powerMill.ActiveProject.Tools.GetByName("10+1.6 tiprad").IsActive = true;
            _powerMill.ActiveProject.StockModels.ActiveItem.ApplyToolLast();
            stockModelStates = _powerMill.ActiveProject.StockModels.ActiveItem.States;
            Assert.AreEqual(5, stockModelStates.Count());
        }

        [Test]
        public void GetStockmodelStatesTest()
        {
            // Create Test environment
            _powerMill.LoadProject(TestFiles.SimplePmProject1);
            _powerMill.ActiveProject.StockModels.CreateStockmodel("Test stockmodel 1");
            _powerMill.ActiveProject.StockModels.ActiveItem.ApplyBlock();
            _powerMill.ActiveProject.Toolpaths.GetByName("alpha").IsActive = true;
            _powerMill.ActiveProject.StockModels.ActiveItem.ApplyToolpathFirst();
            _powerMill.ActiveProject.StockModels.ActiveItem.ApplyToolLast();

            // Test States function
            List<string> stockModelStates = _powerMill.ActiveProject.StockModels.ActiveItem.States;
            Assert.AreEqual(3, stockModelStates.Count());                        
        }

        [Test]
        public void ToleranceTest()
        {
            // Create Test environment            
            PMStockModel testStockModel = _powerMill.ActiveProject.StockModels.CreateStockmodel("Test stockmodel 1");
            // Set test
            testStockModel.Tolerance = 0.2;
            // Get test
            Assert.That(testStockModel.Tolerance, Is.EqualTo((MM)0.2));
        }

        [Test]
        public void StepoverTest()
        {
            // Create Test environment            
            PMStockModel testStockModel = _powerMill.ActiveProject.StockModels.CreateStockmodel("Test stockmodel 1");
            // Set test
            testStockModel.Stepover = 0.2;
            // Get test
            Assert.That(testStockModel.Stepover, Is.EqualTo((MM)0.2));
        }

        [Test]
        public void RestThicknessTest()
        {
            // Create Test environment            
            PMStockModel testStockModel = _powerMill.ActiveProject.StockModels.CreateStockmodel("Test stockmodel 1");
            // Set test
            testStockModel.RestThickness = 0.2;
            // Get test
            Assert.That(testStockModel.RestThickness, Is.EqualTo((MM)0.2));
        }

        [Test]
        public void DetectOverhangTest()
        {
            // Create Test environment            
            PMStockModel testStockModel = _powerMill.ActiveProject.StockModels.CreateStockmodel("Test stockmodel 1");
            // Set test
            testStockModel.DetectOverhang = true;
            // Get test
            Assert.That(testStockModel.DetectOverhang, Is.EqualTo(true));
            // Set test
            testStockModel.DetectOverhang = false;
            // Get test
            Assert.That(testStockModel.DetectOverhang, Is.EqualTo(false));
        }

        [Test]
        public void WorkplaneTest()
        {
            // Create Test environment            
            _powerMill.LoadProject(TestFiles.SimplePmProject1);
            PMStockModel testStockModel = _powerMill.ActiveProject.StockModels.CreateStockmodel("Test stockmodel 1");
            // Get test
            Assert.That(testStockModel.Workplane.Name, Is.EqualTo("bravo"));
            //Set test
            PMWorkplane newWorkplane = _powerMill.ActiveProject.Workplanes.GetByName("charlie");
            testStockModel.Workplane = newWorkplane;
            Assert.That(testStockModel.Workplane.Name, Is.EqualTo("charlie"));
        }

        #endregion

    }
}
