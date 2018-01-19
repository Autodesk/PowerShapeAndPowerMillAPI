// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Autodesk.Geometry;
using Autodesk.ProductInterface.PowerSHAPE;
using NUnit.Framework;
using File = Autodesk.FileSystem.File;

namespace Autodesk.ProductInterface.PowerSHAPETest
{
    /// <summary>
    /// This is a test class for ModelTest and is intended
    /// to contain all ModelTest Unit Tests
    /// </summary>
    [TestFixture]
    public class ModelTest : DatabaseEntityTest<PSModel>
    {
        [SetUp]
        public override void MyTestInitialize()
        {
            base.MyTestInitialize();
            try
            {
                _powerSHAPECollection = _powerSHAPE.Models;
            }
            catch (Exception e)
            {
                Assert.Fail("_powerSHAPECollection could not be set to Active Model's SolidsCollection\n\n" + e.Message);
            }
        }

        /// <summary>
        /// Delete model test
        /// </summary>
        [Test]
        public void DeleteModelTest()
        {
            _powerSHAPE.Reset();
            Assert.AreEqual(1, _powerSHAPE.Models.Count);

            // Create a new model
            var activeModel = _powerSHAPE.Models.CreateEmptyModel();
            Assert.AreEqual(2, _powerSHAPE.Models.Count);

            // Close the new model
            activeModel.Delete();
            Assert.AreEqual(1, _powerSHAPE.Models.Count);
        }

        /// <summary>
        /// A test for CreateTemporaryWorkplane
        /// </summary>
        [Test]
        [Ignore("Need to handle temporary workplanes again - removed from code for some reason")]
        public void CreateTemporaryWorkplane()
        {
            Assert.Inconclusive("Need to handle temporary workplanes again - removed from code for some reason");

            // Define workplane parameters
            var workplaneOrigin = new Point(20, 30, 10);

            // Create temporary workplane
            _powerSHAPE.ActiveModel.CreateTemporaryWorkplane(workplaneOrigin);
            Assert.AreEqual("Temporary",
                            _powerSHAPE.ActiveModel.ActiveWorkplane.Name,
                            "Temporary workplane is not active");
            Assert.AreEqual(workplaneOrigin,
                            _powerSHAPE.ActiveModel.ActiveWorkplane.Origin,
                            "Temporary workplane origin is incorrect");
        }

        /// <summary>
        /// A test for DeleteTemporaryWorkplane
        /// </summary>
        [Test]
        [Ignore("Need to handle temporary workplanes again - removed from code for some reason")]
        public void DeleteTemporaryWorkplane()
        {
            // Define workplane parameters
            var workplaneOrigin = new Point(20, 30, 10);

            // Create temporary workplane
            _powerSHAPE.ActiveModel.CreateTemporaryWorkplane(workplaneOrigin);
            Assert.AreEqual("Temporary",
                            _powerSHAPE.ActiveModel.ActiveWorkplane.Name,
                            "Temporary workplane is not active");

            // Delete temporary workplane
            _powerSHAPE.ActiveModel.DeleteTemporaryWorkplane();
            Assert.IsNull(_powerSHAPE.ActiveModel.ActiveWorkplane, "World workplane is not active");
        }

        /// <summary>
        /// A test for Model Constructor
        /// </summary>
        [Test]
        [Ignore("")]
        public void ModelConstructorTest2()
        {
            //Automation powerSHAPE = null; // TODO: Initialize to an appropriate value
            //File file = null; // TODO: Initialize to an appropriate value
            //Model target = new Model(powerSHAPE, file);
            //Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        /// A test for Model Constructor
        /// </summary>
        [Test]
        [Ignore("")]
        public void ModelConstructorTest1()
        {
            //Automation powerSHAPE = null; // TODO: Initialize to an appropriate value
            //Model target = new Model(powerSHAPE);
            //Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        /// A test for Model Constructor
        /// </summary>
        [Test]
        [Ignore("")]
        public void ModelConstructorTest()
        {
            //Automation powerSHAPE = null; // TODO: Initialize to an appropriate value
            //string id = string.Empty; // TODO: Initialize to an appropriate value
            //string name = string.Empty; // TODO: Initialize to an appropriate value
            //Model target = new Model(powerSHAPE, id, name);
            //Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        /// A test for AbortIfDoesNotExist
        /// </summary>
        [Test]
        [Ignore("")]
        public void AbortIfDoesNotExistTest()
        {
            //PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            //Model_Accessor target = new Model_Accessor(param0); // TODO: Initialize to an appropriate value
            //target.AbortIfDoesNotExist();
            //Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        /// A test for Add
        /// </summary>
        [Test]
        [Ignore("")]
        public void AddTest()
        {
            //Automation powerSHAPE = null; // TODO: Initialize to an appropriate value
            //Model target = new Model(powerSHAPE); // TODO: Initialize to an appropriate value
            //Entity entityToAdd = null; // TODO: Initialize to an appropriate value
            //target.Add(entityToAdd);
            //Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        /// A test for ClearCreatedItems
        /// </summary>
        [Test]
        [Ignore("")]
        public void ClearCreatedItemsTest()
        {
            //Automation powerSHAPE = null; // TODO: Initialize to an appropriate value
            //Model target = new Model(powerSHAPE); // TODO: Initialize to an appropriate value
            //target.ClearCreatedItems();
            //Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        /// A test for ClearSelectedItems
        /// </summary>
        [Test]
        [Ignore("")]
        public void ClearSelectedItemsTest()
        {
            //Automation powerSHAPE = null; // TODO: Initialize to an appropriate value
            //Model target = new Model(powerSHAPE); // TODO: Initialize to an appropriate value
            //target.ClearSelectedItems();
            //Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        /// A test for ClearUpdatedItems
        /// </summary>
        [Test]
        [Ignore("")]
        public void ClearUpdatedItemsTest()
        {
            //Automation powerSHAPE = null; // TODO: Initialize to an appropriate value
            //Model target = new Model(powerSHAPE); // TODO: Initialize to an appropriate value
            //target.ClearUpdatedItems();
            //Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        /// A test for SelectEntities
        /// </summary>
        [Test]
        public void SelectEntitiesTest()
        {
            _powerSHAPE.Reset();
            var points = new List<PSPoint>();
            for (var i = 0; i < 100; i++)
            {
                points.Add(_powerSHAPE.ActiveModel.Points.CreatePoint(new Point()));
            }
            _powerSHAPE.ActiveModel.SelectEntities(points, true);
            Assert.AreEqual(100, _powerSHAPE.ActiveModel.SelectedItems.Count);

            var spheres = new List<PSSolid>();
            for (var i = 0; i < 100; i++)
            {
                spheres.Add(_powerSHAPE.ActiveModel.Solids.CreateSphere(new Point(), 3.0));
            }
            _powerSHAPE.ActiveModel.SelectEntities(spheres, true);
            Assert.AreEqual(100, _powerSHAPE.ActiveModel.SelectedItems.Count);

            var surfaces = new List<PSSurface>();
            for (var i = 0; i < 100; i++)
            {
                surfaces.Add((PSSurface) ImportAndGetEntity(TestFiles.SINGLE_SURFACEEXTRUSION));
            }
            _powerSHAPE.ActiveModel.SelectEntities(surfaces, true);
            Assert.AreEqual(100, _powerSHAPE.ActiveModel.SelectedItems.Count);

            var allEntitiesTogether = new List<PSEntity>();
            for (var i = 0; i < 100; i++)
            {
                allEntitiesTogether.Add(points[i]);
                allEntitiesTogether.Add(spheres[i]);
                allEntitiesTogether.Add(surfaces[i]);
            }
            _powerSHAPE.ActiveModel.SelectEntities(allEntitiesTogether, true);
            Assert.AreEqual(300, _powerSHAPE.ActiveModel.SelectedItems.Count);
        }

        [Test]
        public void BoundingBoxOfSelectedItemsTest()
        {
            _powerSHAPE.Reset();

            _powerSHAPE.ActiveModel.Solids.CreateBlock(new Point(), 10, 20, 30, 5);
            _powerSHAPE.ActiveModel.Points.CreatePoint(new Point(300, 0, 0));

            _powerSHAPE.ActiveModel.SelectAll();

            var boundingBox = _powerSHAPE.ActiveModel.BoundingBoxOfSelectedItems;

            Assert.That(boundingBox, Is.EqualTo(new BoundingBox(-12.62466, 300, -7.62466, 7.62466, 0, 30)));
        }

        [Test]
        public void BoundingBoxOfSelectedItemsTrimmedSurfaceTest()
        {
            _powerSHAPE.Reset();

            _powerSHAPE.ActiveModel.Import(new File(TestFiles.TRIMMED_SURFACE));

            _powerSHAPE.ActiveModel.SelectAll();

            var boundingBox = _powerSHAPE.ActiveModel.BoundingBoxOfSelectedItems;

            Assert.That(boundingBox, Is.EqualTo(new BoundingBox(-49.172922, 19.182409, -4.311589, 53.070873, 0, 0)));
        }

        /// <summary>
        /// A test for EndSurfaceComparison
        /// </summary>
        [Test]
        [Ignore("")]
        public void EndSurfaceComparisonTest()
        {
            //Automation powerSHAPE = null; // TODO: Initialize to an appropriate value
            //Model target = new Model(powerSHAPE); // TODO: Initialize to an appropriate value
            //target.EndSurfaceComparison();
            //Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        /// A test for Export all items
        /// </summary>
        [Test]
        public void ExportAllTest()
        {
            // Import a model with two compcurves
            _powerSHAPE.ActiveModel.Import(new File(TestFiles.COMPCURVES_TO_WRAP));

            // Unselect all
            _powerSHAPE.ActiveModel.ClearSelectedItems();

            // Do the export
            var tempFile = File.CreateTemporaryFile("igs");
            _powerSHAPE.ActiveModel.Export(tempFile);

            // Reset PowerSHAPE
            _powerSHAPE.Reset();

            // Import the exported file
            _powerSHAPE.ActiveModel.Import(tempFile);
            tempFile.Delete();

            // Test that the number of curves is 2
            Assert.AreEqual(2, _powerSHAPE.ActiveModel.Curves.Count);
        }

        /// <summary>
        /// A test for Export selected items
        /// </summary>
        [Test]
        public void ExportSelectedTest()
        {
            // Import a model with two compcurves
            _powerSHAPE.ActiveModel.Import(new File(TestFiles.COMPCURVES_TO_WRAP));

            // Select the first compcurve
            _powerSHAPE.ActiveModel.CompCurves[0].AddToSelection(true);

            // Do the export
            var tempFile = File.CreateTemporaryFile("dgk");
            _powerSHAPE.ActiveModel.Export(tempFile, ExportItemsOptions.Selected);

            // Reset PowerSHAPE
            _powerSHAPE.Reset();

            // Import the exported file
            _powerSHAPE.ActiveModel.Import(tempFile);
            tempFile.Delete();

            // Test that the number of comp curves is 1
            Assert.AreEqual(1, _powerSHAPE.ActiveModel.CompCurves.Count);
        }

        /// <summary>
        /// A test for Export selected items when nothing selection
        /// </summary>
        [Test]
        public void ExportSelectedExceptionTest()
        {
            // Import a model with two compcurves
            _powerSHAPE.ActiveModel.Import(new File(TestFiles.COMPCURVES_TO_WRAP));

            // Select the first compcurve
            _powerSHAPE.ActiveModel.ClearSelectedItems();

            // Do the export
            var tempFile = File.CreateTemporaryFile("dgk");
            Assert.Throws(typeof(Exception),
                          delegate { _powerSHAPE.ActiveModel.Export(tempFile, ExportItemsOptions.Selected); });
        }

        /// <summary>
        /// A test for Export visible items
        /// </summary>
        [Test]
        public void ExportVisibleTest()
        {
            if (_powerSHAPE.Version < new Version("12.0"))
            {
                // Import a model with two compcurves
                _powerSHAPE.ActiveModel.Import(new File(TestFiles.COMPCURVES_TO_WRAP));

                // Select the first compcurve
                _powerSHAPE.ActiveModel.CompCurves[0].Blank();

                // Do the export
                var tempFile = File.CreateTemporaryFile("dgk");
                Assert.Throws(typeof(ArgumentException),
                              delegate { _powerSHAPE.ActiveModel.Export(tempFile, ExportItemsOptions.Visible); });
            }
            else
            {
                // Import a model with two compcurves
                _powerSHAPE.ActiveModel.Import(new File(TestFiles.COMPCURVES_TO_WRAP));

                // Select the first compcurve
                _powerSHAPE.ActiveModel.CompCurves[0].Blank();

                // Do the export
                var tempFile = File.CreateTemporaryFile("dgk");
                _powerSHAPE.ActiveModel.Export(tempFile, ExportItemsOptions.Visible);

                // Reset PowerSHAPE
                _powerSHAPE.Reset();

                // Import the exported file
                _powerSHAPE.ActiveModel.Import(tempFile);
                tempFile.Delete();

                // Test that the number of comp curves is 1
                Assert.AreEqual(1, _powerSHAPE.ActiveModel.CompCurves.Count);
            }
        }

        /// <summary>
        /// A test for Exporting with form update on
        /// </summary>
        [Test]
        public void ExportWithFormUpdateOnTest()
        {
            // Import a model with two compcurves
            _powerSHAPE.ActiveModel.Import(new File(TestFiles.COMPCURVES_TO_WRAP));

            // Do the export
            var tempFile = File.CreateTemporaryFile("dgk");
            _powerSHAPE.FormUpdateOn();
            _powerSHAPE.ActiveModel.Export(tempFile, ExportItemsOptions.All);
            Assert.AreEqual(true, tempFile.Exists);
            _powerSHAPE.FormUpdateOff();
            tempFile.Delete();
            _powerSHAPE.Reset();
        }

        /// <summary>
        /// A test for ExportSurfaceComparisonErrorsFile
        /// </summary>
        [Test]
        [Ignore("")]
        public void ExportSurfaceComparisonErrorsFileTest()
        {
            //Automation powerSHAPE = null; // TODO: Initialize to an appropriate value
            //Model target = new Model(powerSHAPE); // TODO: Initialize to an appropriate value
            //File outputFile = null; // TODO: Initialize to an appropriate value
            //target.ExportSurfaceComparisonErrorsFile(outputFile);
            //Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        /// A test for Import
        /// </summary>
        [Test]
        public void ImportTest()
        {
            var importedItems = _powerSHAPE.ActiveModel.Import(new File(TestFiles.SINGLE_COMPCURVE));

            Assert.AreEqual(_powerSHAPE.ActiveModel.CompCurves.Count, importedItems.Count);
            Assert.IsNotNull(importedItems[0] as PSCompCurve);

            // Try to check if the returned instance is the same
            Assert.AreEqual(importedItems[0].GetHashCode(), _powerSHAPE.ActiveModel.CompCurves[0].GetHashCode());
        }

        /// <summary>
        /// A test for Initialise
        /// </summary>
        [Test]
        [Ignore("")]
        public void InitialiseTest()
        {
            //Automation powerSHAPE = null; // TODO: Initialize to an appropriate value
            //Model target = new Model(powerSHAPE); // TODO: Initialize to an appropriate value
            //target.Initialise();
            //Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        /// A test for Redo
        /// </summary>
        [Test]
        [Ignore("")]
        public void RedoTest()
        {
            //Automation powerSHAPE = null; // TODO: Initialize to an appropriate value
            //Model target = new Model(powerSHAPE); // TODO: Initialize to an appropriate value
            //target.Redo();
            //Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        /// A test for ResetUI
        /// </summary>
        [Test]
        [Ignore("")]
        public void ResetUITest()
        {
            //Automation powerSHAPE = null; // TODO: Initialize to an appropriate value
            //Model target = new Model(powerSHAPE); // TODO: Initialize to an appropriate value
            //target.ResetUI();
            //Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        /// A test for SelectAll
        /// </summary>
        [Test]
        [Ignore("")]
        public void SelectAllTest()
        {
            //Automation powerSHAPE = null; // TODO: Initialize to an appropriate value
            //Model target = new Model(powerSHAPE); // TODO: Initialize to an appropriate value
            //bool enableDeactivatedLevels = false; // TODO: Initialize to an appropriate value
            //target.SelectAll(enableDeactivatedLevels);
            //Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        /// A test for SetUndoMarker
        /// </summary>
        [Test]
        [Ignore("")]
        public void SetUndoMarkerTest()
        {
            //Automation powerSHAPE = null; // TODO: Initialize to an appropriate value
            //Model target = new Model(powerSHAPE); // TODO: Initialize to an appropriate value
            //target.SetUndoMarker();
            //Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        /// A test for StartSurfaceComparison
        /// </summary>
        [Test]
        [Ignore("")]
        public void StartSurfaceComparisonTest()
        {
            //Automation powerSHAPE = null; // TODO: Initialize to an appropriate value
            //Model target = new Model(powerSHAPE); // TODO: Initialize to an appropriate value
            //Surface surfaceToCompare = null; // TODO: Initialize to an appropriate value
            //Mesh meshToCompare = null; // TODO: Initialize to an appropriate value
            //target.StartSurfaceComparison(surfaceToCompare, meshToCompare);
            //Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        /// A test for Unblank
        /// </summary>
        [Test]
        [Ignore("")]
        public void UnblankTest()
        {
            //Automation powerSHAPE = null; // TODO: Initialize to an appropriate value
            //Model target = new Model(powerSHAPE); // TODO: Initialize to an appropriate value
            //target.Unblank();
            //Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        /// A test for Undo
        /// </summary>
        [Test]
        [Ignore("")]
        public void UndoTest()
        {
            //Automation powerSHAPE = null; // TODO: Initialize to an appropriate value
            //Model target = new Model(powerSHAPE); // TODO: Initialize to an appropriate value
            //target.Undo();
            //Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        /// A test for ActiveLevels
        /// </summary>
        [Test]
        public void ActiveLevelsTest()
        {
            var activeModel = _powerSHAPE.Reset();
            Assert.That(_powerSHAPE.ActiveModel.ActiveLevels.Count, Is.EqualTo(5));
            activeModel.Levels[50].IsActive = true;
            Assert.That(_powerSHAPE.ActiveModel.ActiveLevels.Count, Is.EqualTo(6));
            activeModel.Levels[1].IsActive = false;
            Assert.That(_powerSHAPE.ActiveModel.ActiveLevels.Count, Is.EqualTo(5));
        }

        /// <summary>
        /// A test for ActiveWorkplane
        /// </summary>
        [Test]
        [Ignore("")]
        public void ActiveWorkplaneTest()
        {
            //Automation powerSHAPE = null; // TODO: Initialize to an appropriate value
            //Model target = new Model(powerSHAPE); // TODO: Initialize to an appropriate value
            //Workplane expected = null; // TODO: Initialize to an appropriate value
            //Workplane actual;
            //target.ActiveWorkplane = expected;
            //actual = target.ActiveWorkplane;
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// A test for Annotations
        /// </summary>
        [Test]
        [Ignore("")]
        public void AnnotationsTest()
        {
            //Automation powerSHAPE = null; // TODO: Initialize to an appropriate value
            //Model target = new Model(powerSHAPE); // TODO: Initialize to an appropriate value
            //AnnotationsCollection actual;
            //actual = target.Annotations;
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// A test for Arcs
        /// </summary>
        [Test]
        [Ignore("")]
        public void ArcsTest()
        {
            //Automation powerSHAPE = null; // TODO: Initialize to an appropriate value
            //Model target = new Model(powerSHAPE); // TODO: Initialize to an appropriate value
            //ArcsCollection actual;
            //actual = target.Arcs;
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// A test for Assemblies
        /// </summary>
        [Test]
        [Ignore("")]
        public void AssembliesTest()
        {
            //Automation powerSHAPE = null; // TODO: Initialize to an appropriate value
            //Model target = new Model(powerSHAPE); // TODO: Initialize to an appropriate value
            //AssembliesCollection actual;
            //actual = target.Assemblies;
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// A test for CompCurves
        /// </summary>
        [Test]
        [Ignore("")]
        public void CompCurvesTest()
        {
            //Automation powerSHAPE = null; // TODO: Initialize to an appropriate value
            //Model target = new Model(powerSHAPE); // TODO: Initialize to an appropriate value
            //CompCurvesCollection actual;
            //actual = target.CompCurves;
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// A test for Curves
        /// </summary>
        [Test]
        [Ignore("")]
        public void CurvesTest()
        {
            //Automation powerSHAPE = null; // TODO: Initialize to an appropriate value
            //Model target = new Model(powerSHAPE); // TODO: Initialize to an appropriate value
            //CurvesCollection actual;
            //actual = target.Curves;
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// A test for Exists
        /// </summary>
        [Test]
        [Ignore("")]
        public void ExistsTest()
        {
            //Automation powerSHAPE = null; // TODO: Initialize to an appropriate value
            //Model target = new Model(powerSHAPE); // TODO: Initialize to an appropriate value
            //bool actual;
            //actual = target.Exists;
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// A test for Features
        /// </summary>
        [Test]
        [Ignore("")]
        public void FeaturesTest()
        {
            //Automation powerSHAPE = null; // TODO: Initialize to an appropriate value
            //Model target = new Model(powerSHAPE); // TODO: Initialize to an appropriate value
            //FeaturesCollection actual;
            //actual = target.Features;
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// A test for Id
        /// </summary>
        [Test]
        [Ignore("")]
        public void IdTest()
        {
            //Automation powerSHAPE = null; // TODO: Initialize to an appropriate value
            //Model target = new Model(powerSHAPE); // TODO: Initialize to an appropriate value
            //string actual;
            //actual = target.Id;
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// A test for IsCorrupt
        /// </summary>
        [Test]
        [Ignore("")]
        public void IsCorruptTest()
        {
            //Automation powerSHAPE = null; // TODO: Initialize to an appropriate value
            //Model target = new Model(powerSHAPE); // TODO: Initialize to an appropriate value
            //bool actual;
            //actual = target.IsCorrupt;
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// A test for IsInSurfaceComparison
        /// </summary>
        [Test]
        [Ignore("")]
        public void IsInSurfaceComparisonTest()
        {
            //PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            //Model_Accessor target = new Model_Accessor(param0); // TODO: Initialize to an appropriate value
            //bool actual;
            //actual = target.IsInSurfaceComparison;
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// A test for IsLevelActiveFilterOn
        /// </summary>
        [Test]
        [Ignore("")]
        public void IsLevelActiveFilterOnTest()
        {
            //Automation powerSHAPE = null; // TODO: Initialize to an appropriate value
            //Model target = new Model(powerSHAPE); // TODO: Initialize to an appropriate value
            //bool expected = false; // TODO: Initialize to an appropriate value
            //bool actual;
            //target.IsLevelActiveFilterOn = expected;
            //actual = target.IsLevelActiveFilterOn;
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// A test for IsLevelNamedFilterOn
        /// </summary>
        [Test]
        [Ignore("")]
        public void IsLevelNamedFilterOnTest()
        {
            //Automation powerSHAPE = null; // TODO: Initialize to an appropriate value
            //Model target = new Model(powerSHAPE); // TODO: Initialize to an appropriate value
            //bool expected = false; // TODO: Initialize to an appropriate value
            //bool actual;
            //target.IsLevelNamedFilterOn = expected;
            //actual = target.IsLevelNamedFilterOn;
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// A test for IsLevelUsedFilterOn
        /// </summary>
        [Test]
        [Ignore("")]
        public void IsLevelUsedFilterOnTest()
        {
            //Automation powerSHAPE = null; // TODO: Initialize to an appropriate value
            //Model target = new Model(powerSHAPE); // TODO: Initialize to an appropriate value
            //bool expected = false; // TODO: Initialize to an appropriate value
            //bool actual;
            //target.IsLevelUsedFilterOn = expected;
            //actual = target.IsLevelUsedFilterOn;
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// A test for IsSelected
        /// </summary>
        [Test]
        [Ignore("")]
        public void IsSelectedTest()
        {
            //Automation powerSHAPE = null; // TODO: Initialize to an appropriate value
            //Model target = new Model(powerSHAPE); // TODO: Initialize to an appropriate value
            //bool actual;
            //actual = target.IsSelected;
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// A test for Levels
        /// </summary>
        [Test]
        [Ignore("")]
        public void LevelsTest()
        {
            //Automation powerSHAPE = null; // TODO: Initialize to an appropriate value
            //Model target = new Model(powerSHAPE); // TODO: Initialize to an appropriate value
            //LevelsCollection actual;
            //actual = target.Levels;
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// A test for Lines
        /// </summary>
        [Test]
        [Ignore("")]
        public void LinesTest()
        {
            //Automation powerSHAPE = null; // TODO: Initialize to an appropriate value
            //Model target = new Model(powerSHAPE); // TODO: Initialize to an appropriate value
            //LinesCollection actual;
            //actual = target.Lines;
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// A test for Materials
        /// </summary>
        [Test]
        [Ignore("")]
        public void MaterialsTest()
        {
            //Automation powerSHAPE = null; // TODO: Initialize to an appropriate value
            //Model target = new Model(powerSHAPE); // TODO: Initialize to an appropriate value
            //MaterialsCollection actual;
            //actual = target.Materials;
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// A test for Meshes
        /// </summary>
        [Test]
        [Ignore("")]
        public void MeshesTest()
        {
            //Automation powerSHAPE = null; // TODO: Initialize to an appropriate value
            //Model target = new Model(powerSHAPE); // TODO: Initialize to an appropriate value
            //MeshesCollection actual;
            //actual = target.Meshes;
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// A test for Name
        /// </summary>
        [Test]
        [Ignore("")]
        public void NameTest()
        {
            //Automation powerSHAPE = null; // TODO: Initialize to an appropriate value
            //Model target = new Model(powerSHAPE); // TODO: Initialize to an appropriate value
            //string actual;
            //actual = target.Name;
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// A test for NamedLevels
        /// </summary>
        [Test]
        [Ignore("")]
        public void NamedLevelsTest()
        {
            //Automation powerSHAPE = null; // TODO: Initialize to an appropriate value
            //Model target = new Model(powerSHAPE); // TODO: Initialize to an appropriate value
            //List<Level> actual;
            //actual = target.NamedLevels;
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// A test for Path
        /// </summary>
        [Test]
        [Ignore("")]
        public void PathTest()
        {
            //Automation powerSHAPE = null; // TODO: Initialize to an appropriate value
            //Model target = new Model(powerSHAPE); // TODO: Initialize to an appropriate value
            //string actual;
            //actual = target.Path;
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// A test for Points
        /// </summary>
        [Test]
        [Ignore("")]
        public void PointsTest()
        {
            //Automation powerSHAPE = null; // TODO: Initialize to an appropriate value
            //Model target = new Model(powerSHAPE); // TODO: Initialize to an appropriate value
            //PointsCollection actual;
            //actual = target.Points;
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// A test for Solids
        /// </summary>
        [Test]
        public void SolidsTest()
        {
            //Automation powerSHAPE = null; // TODO: Initialize to an appropriate value
            //Model target = new Model(powerSHAPE); // TODO: Initialize to an appropriate value
            //SolidsCollection actual;
            //actual = target.Solids;
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// A test for Surfaces
        /// </summary>
        [Test]
        [Ignore("")]
        public void SurfacesTest()
        {
            //Automation powerSHAPE = null; // TODO: Initialize to an appropriate value
            //Model target = new Model(powerSHAPE); // TODO: Initialize to an appropriate value
            //SurfacesCollection actual;
            //actual = target.Surfaces;
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// A test for UsedLevels
        /// </summary>
        [Test]
        [Ignore("")]
        public void UsedLevelsTest()
        {
            //Automation powerSHAPE = null; // TODO: Initialize to an appropriate value
            //Model target = new Model(powerSHAPE); // TODO: Initialize to an appropriate value
            //List<Level> actual;
            //actual = target.UsedLevels;
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// A test for Workplanes
        /// </summary>
        [Test]
        [Ignore("")]
        public void WorkplanesTest()
        {
            //Automation powerSHAPE = null; // TODO: Initialize to an appropriate value
            //Model target = new Model(powerSHAPE); // TODO: Initialize to an appropriate value
            //WorkplanesCollection actual;
            //actual = target.Workplanes;
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        [Test]
        public void SetCreationLevel()
        {
            var point1 = _powerSHAPE.ActiveModel.Points.CreatePoint(new Point());
            _powerSHAPE.ActiveModel.SetCreationLevel(_powerSHAPE.ActiveModel.Levels[1]);
            var point2 = _powerSHAPE.ActiveModel.Points.CreatePoint(new Point());
            Assert.AreEqual(0, point1.LevelNumber);
            Assert.AreEqual(1, point2.LevelNumber);
        }

        [Test]
        public void SetCreationLevelWhenInactive()
        {
            var point1 = _powerSHAPE.ActiveModel.Points.CreatePoint(new Point());
            _powerSHAPE.ActiveModel.Levels[1].IsActive = false;
            _powerSHAPE.ActiveModel.SetCreationLevel(_powerSHAPE.ActiveModel.Levels[1]);
            var point2 = _powerSHAPE.ActiveModel.Points.CreatePoint(new Point());
            Assert.AreEqual(0, point1.LevelNumber);
            Assert.AreEqual(1, point2.LevelNumber);
        }

        [Test]
        public void SetCreationLevel2()
        {
            var point1 = _powerSHAPE.ActiveModel.Points.CreatePoint(new Point());
            _powerSHAPE.ActiveModel.SetCreationLevel(1);
            var point2 = _powerSHAPE.ActiveModel.Points.CreatePoint(new Point());
            Assert.AreEqual(0, point1.LevelNumber);
            Assert.AreEqual(1, point2.LevelNumber);
        }

        [Test]
        public void SetCreationLevelWhenInactive2()
        {
            var point1 = _powerSHAPE.ActiveModel.Points.CreatePoint(new Point());
            _powerSHAPE.ActiveModel.Levels[1].IsActive = false;
            _powerSHAPE.ActiveModel.SetCreationLevel(1);
            var point2 = _powerSHAPE.ActiveModel.Points.CreatePoint(new Point());
            Assert.AreEqual(0, point1.LevelNumber);
            Assert.AreEqual(1, point2.LevelNumber);
        }

        #region Refresh

        [Test]
        public void RefreshHandlesLevelChanges()
        {
            _powerSHAPE.Models.CreateEmptyModel();
            var activeModel = _powerSHAPE.ActiveModel;

            var point = activeModel.Points.CreatePoint(new Point(1, 0, 0));
            Assert.AreEqual(0, point.LevelNumber);

            point.AddToSelection(true);
            _powerSHAPE.Execute("LEVELSELECTOR INPUTLEVEL 1");
            Assert.AreEqual(0, point.LevelNumber);

            activeModel.Refresh();
            Assert.AreEqual(1, point.LevelNumber);
        }

        [Test]
        public void RefreshAfterWhenEntityBlank()
        {
            _powerSHAPE.Models.CreateEmptyModel();
            var activeModel = _powerSHAPE.ActiveModel;

            var point = activeModel.Points.CreatePoint(new Point(1, 0, 0));
            point.Blank();

            activeModel.Refresh();

            // Ensure that point wasn't deleted
            Assert.IsTrue(activeModel.Points.Any());

            // Ensure that is still blanked
            Assert.IsFalse(activeModel.SelectedItems.Any());

            // Ensure only one point in the list
            Assert.AreEqual(1, activeModel.Points.Count);
        }

        [Test]
        public void RefreshTwiceWhenEntitiesBlank()
        {
            _powerSHAPE.ActiveModel.Points.CreatePoint(new Point(0, 0, 0));
            _powerSHAPE.ActiveModel.Points.CreatePoint(new Point(0, 0, 0));
            _powerSHAPE.ActiveModel.Points.CreatePoint(new Point(0, 0, 0));
            _powerSHAPE.ActiveModel.Points.CreatePoint(new Point(0, 0, 0));

            Assert.IsTrue(_powerSHAPE.ActiveModel.Points.Count == 4);

            var points = _powerSHAPE.ActiveModel.Points;
            points[0].Blank();
            points[3].Blank();

            var blankedEntitiesIds = new List<string>();
            blankedEntitiesIds.Add(points[0].Id.ToString());
            blankedEntitiesIds.Add(points[3].Id.ToString());

            // Ensure that two points are blanked
            _powerSHAPE.ActiveModel.SelectAll();
            Assert.AreEqual(2, _powerSHAPE.ActiveModel.SelectedItems.Count);

            _powerSHAPE.ActiveModel.Refresh();
            _powerSHAPE.ActiveModel.Refresh();

            // Ensure that points weren't deleted
            Assert.AreEqual(4, _powerSHAPE.ActiveModel.Points.Count);

            // Ensure that is still blanked
            _powerSHAPE.ActiveModel.SelectAll();
            Assert.AreEqual(2, _powerSHAPE.ActiveModel.SelectedItems.Count);

            // Ensure that the right points are blanked
            var unblankedEntitiesIds = _powerSHAPE.ActiveModel.SelectedItems.Select(x => x.Id);
            Assert.IsFalse(
                unblankedEntitiesIds.Any(unblankedEntityId => blankedEntitiesIds.Contains(unblankedEntityId.ToString())));
        }

        [Test]
        public void RefreshWorksForSolids()
        {
            _powerSHAPE.Models.CreateEmptyModel();
            var activeModel = _powerSHAPE.ActiveModel;

            activeModel.Solids.CreateBlock(new Point(0, 0, 0), 10, 10, 10, 0);

            Assert.IsTrue(activeModel.Solids.Any());

            activeModel.Refresh();

            Assert.IsTrue(activeModel.Solids.Any());

            activeModel.SelectAll();
            _powerSHAPE.Execute("Delete");

            activeModel.Refresh();

            Assert.IsFalse(activeModel.Solids.Any());
        }

        [Test]
        public void RefreshAfterAddingNewEntity()
        {
            _powerSHAPE.Models.CreateEmptyModel();
            var activeModel = _powerSHAPE.ActiveModel;

            _powerSHAPE.Execute("CREATE POINT SINGLE");
            _powerSHAPE.Execute("0 0 0");

            Assert.IsFalse(activeModel.Points.Any());

            activeModel.Refresh();

            Assert.IsTrue(activeModel.Points.Any());
        }

        [Test]
        public void RefreshAfterAddingNewEntityAndDeactivatingItsLevel()
        {
            _powerSHAPE.Models.CreateEmptyModel();
            var activeModel = _powerSHAPE.ActiveModel;

            _powerSHAPE.Execute("CREATE POINT SINGLE");
            _powerSHAPE.Execute("0 0 0");
            _powerSHAPE.ActiveModel.Levels[0].IsActive = false;

            Assert.IsFalse(activeModel.Points.Any());

            activeModel.Refresh();

            Assert.IsTrue(activeModel.Points.Any());
        }

        [Test]
        public void RefreshAfterRemovingEntity()
        {
            _powerSHAPE.Models.CreateEmptyModel();
            var activeModel = _powerSHAPE.ActiveModel;

            var point1 = activeModel.Points.CreatePoint(new Point(0, 0, 0));

            Assert.IsTrue(activeModel.Points.Any());

            point1.AddToSelection();
            _powerSHAPE.Execute("DELETE");

            activeModel.Refresh();

            Assert.IsFalse(activeModel.Points.Any());
        }

        [Test]
        public void RefreshDoesNotAffectAnythingThatHasNotBeenAddedOrRemoved()
        {
            _powerSHAPE.Models.CreateEmptyModel();
            var activeModel = _powerSHAPE.ActiveModel;

            var point1 = activeModel.Points.CreatePoint(new Point(0, 0, 0));
            var point2 = activeModel.Points.CreatePoint(new Point(1, 1, 1));

            Assert.IsTrue(activeModel.Points.Count() == 2);

            point1.AddToSelection(true);
            _powerSHAPE.Execute("DELETE");

            activeModel.Refresh();

            Assert.IsFalse(activeModel.Points.Any(x => x.Name == point1.Name));
            Assert.IsTrue(activeModel.Points.Any(x => x.Name == point2.Name));
        }

        #endregion

        # region CreatedItems, SelectedItems, UpdatedItems

        [Test]
        public void CreatedItemsWhenItemInCache()
        {
            _powerSHAPE.Models.CreateEmptyModel();
            var activeModel = _powerSHAPE.ActiveModel;
            var line = activeModel.Lines.CreateLine(new Point(0, 0, 0), new Point(1, 0, 0));

            var createdItem = activeModel.CreatedItems.Single();
            Assert.AreEqual(line.GetHashCode(), createdItem.GetHashCode());
        }

        [Test]
        public void CreatedItemsWhenNoCache()
        {
            _powerSHAPE.Models.CreateEmptyModel();
            var activeModel = _powerSHAPE.ActiveModel;

            _powerSHAPE.Execute("CREATE POINT SINGLE");
            _powerSHAPE.Execute("0 0 0");

            Assert.IsFalse(activeModel.Points.Any());
            Assert.IsTrue(activeModel.CreatedItems.Any());
        }

        [Test]
        public void SelectedItemsWhenItemInCache()
        {
            _powerSHAPE.Models.CreateEmptyModel();
            var activeModel = _powerSHAPE.ActiveModel;
            var line = activeModel.Lines.CreateLine(new Point(0, 0, 0), new Point(1, 0, 0));
            line.AddToSelection();

            Assert.AreEqual(line.GetHashCode(), activeModel.SelectedItems.Single().GetHashCode());
        }

        [Test]
        public void SelectedItemsWhenNoCache()
        {
            _powerSHAPE.Models.CreateEmptyModel();
            var activeModel = _powerSHAPE.ActiveModel;

            _powerSHAPE.Execute("CREATE POINT SINGLE");
            _powerSHAPE.Execute("0 0 0");
            _powerSHAPE.Execute("SelectAll");

            Assert.IsFalse(activeModel.Points.Any());
            Assert.IsTrue(activeModel.SelectedItems.Any());
        }

        [Test]
        public void UpdatedItemsWhenItemInCache()
        {
            _powerSHAPE.Models.CreateEmptyModel();
            var activeModel = _powerSHAPE.ActiveModel;
            var line = activeModel.Lines.CreateLine(new Point(0, 0, 0), new Point(1, 0, 0));
            line.MoveByVector(new Vector(0, 0, 1), 0);

            Assert.AreEqual(line.GetHashCode(), activeModel.UpdatedItems.Single().GetHashCode());
        }

        [Test]
        public void UpdatedItemsWhenNoCache()
        {
            _powerSHAPE.Models.CreateEmptyModel();
            var activeModel = _powerSHAPE.ActiveModel;

            _powerSHAPE.Execute("CREATE POINT SINGLE");
            _powerSHAPE.Execute("0 0 0");
            _powerSHAPE.Execute("EDIT MOVE");
            _powerSHAPE.Execute("0 0 1");
            _powerSHAPE.Execute("APPLY");

            Assert.IsFalse(activeModel.Points.Any());
            Assert.IsTrue(activeModel.UpdatedItems.Any());
        }

        #endregion

        #region Save

        [Test]
        [Category("Save Tests")]
        public void SaveFirstTimeWithoutFile()
        {
            try
            {
                _powerSHAPE.Models.CreateEmptyModel();
                var newActive = _powerSHAPE.ActiveModel;
                newActive.Save();
            }
            catch (ArgumentException)
            {
                Assert.Pass();
            }
            Assert.Fail();
        }

        [Test]
        [Category("Save Tests")]
        public void SaveSecondTimeWithoutFile()
        {
            _powerSHAPE.Models.CreateEmptyModel();
            var newActive = _powerSHAPE.ActiveModel;

            var file = new File(string.Format("{0}\\{1}.psmodel", Path.GetTempPath(), "test"));

            newActive.Save(file);
            newActive.Save();

            Assert.IsTrue(file.Exists);

            // Delete file
            _powerSHAPE.Models.Clear();
            file.Delete();
        }

        [Test]
        [Category("Save Tests")]
        public void SaveFirstTimeWithFile()
        {
            _powerSHAPE.Models.CreateEmptyModel();
            var newActive = _powerSHAPE.ActiveModel;

            var file = new File(string.Format("{0}\\{1}.psmodel", Path.GetTempPath(), "test"));
            newActive.Save(file);

            Assert.IsTrue(file.Exists);

            // Delete file
            _powerSHAPE.Models.Clear();
            file.Delete();
        }

        [Test]
        [Category("Save Tests")]
        public void SaveSecondTimeWithFile()
        {
            _powerSHAPE.Models.CreateEmptyModel();
            var newActive = _powerSHAPE.ActiveModel;

            var file = new File(string.Format("{0}\\{1}.psmodel", Path.GetTempPath(), "test"));
            newActive.Save(file);
            var changedDate = file.LastChangedDate;
            Thread.Sleep(new TimeSpan(0, 0, 4));

            newActive.Save(file);

            Assert.AreNotEqual(changedDate, file.LastChangedDate);

            // Delete file
            _powerSHAPE.Models.Clear();
            file.Delete();
        }

        [Test]
        [Category("Save Tests")]
        public void SaveToAnotherOpenModelFile()
        {
            // Create 1st model and save it
            _powerSHAPE.Models.CreateEmptyModel();
            var newActive = _powerSHAPE.ActiveModel;
            var firstModelHashCode = newActive.GetHashCode();

            var file = new File(string.Format("{0}\\{1}.psmodel", Path.GetTempPath(), "test"));
            newActive.Save(file);
            var changedDate = file.LastChangedDate;

            Thread.Sleep(new TimeSpan(0, 0, 4));

            // Create 2nd model and save it to the previous file
            _powerSHAPE.Models.CreateEmptyModel();
            newActive = _powerSHAPE.ActiveModel;
            var secondModelHashCode = newActive.GetHashCode();
            newActive.Save(file);

            Assert.AreNotEqual(changedDate, file.LastChangedDate);

            // Ensure that the first model is closed but the second model stays open
            Assert.IsFalse(_powerSHAPE.Models.Any(x => x.GetHashCode() == firstModelHashCode));
            Assert.IsTrue(_powerSHAPE.Models.Any(x => x.GetHashCode() == secondModelHashCode));

            // Delete file
            _powerSHAPE.Models.Clear();
            file.Delete();
        }

        [Test]
        [Category("Save Tests")]
        public void SaveWithTheWrongFileExtension()
        {
            // Create 1st model and save it
            _powerSHAPE.Models.CreateEmptyModel();
            var newActive = _powerSHAPE.ActiveModel;

            var fileExtension = new File(string.Format("{0}\\{1}.psModel", Path.GetTempPath(), "testExtension"));
            newActive.Save(fileExtension);

            // Make sure that we are able to save files with the extension psModel instead of psmodel
            Assert.IsTrue(fileExtension.Exists);

            // Delete file
            _powerSHAPE.Models.Clear();
            fileExtension.Delete();
        }

        [Test]
        [Category("Save Tests")]
        public void SaveToExistingFile()
        {
            // Target file exists, but does not match selected model's name
            // Create a model
            _powerSHAPE.Models.CreateEmptyModel();
            var newActive = _powerSHAPE.ActiveModel;

            // Create a target file
            var targetFile = File.CreateTemporaryFile("psmodel");

            // Ensure that it doesn't have the same name as the model to be saved
            while (targetFile.NameWithoutExtension == newActive.Name)
                targetFile = File.CreateTemporaryFile("psmodel");

            targetFile.Create();

            // Make sure that the target file exists and is empty
            Assert.IsTrue(targetFile.Exists);
            Assert.AreEqual(0, new FileInfo(targetFile.Path).Length);

            // Try to save the model into the existing file (overwrite the file)
            newActive.Save(targetFile);

            // Delete file
            _powerSHAPE.Models.Clear();
            var newFileSize = new FileInfo(targetFile.Path).Length;
            targetFile.Delete();
            Assert.AreNotEqual(0, newFileSize);
        }

        #endregion

        #region Export

        [Test]
        public void ExportToWorld()
        {
            _powerSHAPE.Models.CreateEmptyModel();
            var activeModel = _powerSHAPE.ActiveModel;
            var entity = (PSEntity) activeModel.Points.CreatePoint(new Point(1, 1, 0));
            var workplane = activeModel.Workplanes.CreateWorkplaneAlignedToEntity(entity, new Point(1, 1, 0));

            var fileDGK = new File(string.Format("{0}\\{1}.dgk", Path.GetTempPath(), "geometry"));
            activeModel.Export(fileDGK, ExportItemsOptions.All, ExportWorkplanes.World);
            workplane.Delete();
            activeModel.Points.Clear();
            activeModel.Import(fileDGK);

            var point = activeModel.Points[0];
            Assert.IsTrue(point.X == 1);
            Assert.IsTrue(point.Y == 1);
            Assert.IsTrue(point.Z == 0);

            // Delete file
            _powerSHAPE.Models.Clear();
            fileDGK.Delete();
        }

        [Test]
        public void ExportToActiveWorkplane()
        {
            _powerSHAPE.Models.CreateEmptyModel();
            var activeModel = _powerSHAPE.ActiveModel;
            var entity = (PSEntity) activeModel.Points.CreatePoint(new Point(1, 1, 0));
            var workplane = activeModel.Workplanes.CreateWorkplaneAlignedToEntity(entity, new Point(1, 1, 0));

            var fileDGK = new File(string.Format("{0}\\{1}.dgk", Path.GetTempPath(), "geometry"));
            activeModel.Export(fileDGK, ExportItemsOptions.Visible, ExportWorkplanes.Active);
            workplane.Delete();
            activeModel.Points.Clear();
            activeModel.Import(fileDGK);

            var point = activeModel.Points[0];
            Assert.IsTrue(point.X == 0);
            Assert.IsTrue(point.Y == 0);
            Assert.IsTrue(point.Z == 0);

            // Delete file
            _powerSHAPE.Models.Clear();
            fileDGK.Delete();
        }

        [Test]
        public void ExportToActiveWorkplaneWhenADrawingExists()
        {
            _powerSHAPE.Models.CreateEmptyModel();
            var activeModel = _powerSHAPE.ActiveModel;
            var entity = (PSEntity) activeModel.Points.CreatePoint(new Point(1, 1, 0));
            var workplane = activeModel.Workplanes.CreateWorkplaneAlignedToEntity(entity, new Point(1, 1, 0));
            _powerSHAPE.Execute("DRAFTINGMODE OPENDRAWING ON", "NEWDRAWING", "WINDOW CLOSE");

            var fileDGK = new File(string.Format("{0}\\{1}.dgk", Path.GetTempPath(), "geometry"));
            activeModel.Export(fileDGK, ExportItemsOptions.Visible, ExportWorkplanes.Active);
            workplane.Delete();
            activeModel.Points.Clear();
            activeModel.Import(fileDGK);

            var point = activeModel.Points[0];
            Assert.IsTrue(point.X == 0);
            Assert.IsTrue(point.Y == 0);
            Assert.IsTrue(point.Z == 0);

            // Delete file
            _powerSHAPE.Models.Clear();
            fileDGK.Delete();
        }

        #endregion

        #region Units

        [Test]
        public void ReadUnitsFromMMFile()
        {
            _powerSHAPE.Models.Clear();

            PSModel activeModel = _powerSHAPE.Models.CreateModelFromFile(
                new File(AppDomain.CurrentDomain.BaseDirectory +
                         "\\..\\..\\DatabaseEntity Tests\\ModelTestFiles\\MMFile.psmodel"));

            Assert.That(activeModel.Units, Is.EqualTo(LengthUnits.MM));
        }

        [Test]
        public void ReadUnitsFromInchFile()
        {
            _powerSHAPE.Models.Clear();

            PSModel activeModel = _powerSHAPE.Models.CreateModelFromFile(
                new File(AppDomain.CurrentDomain.BaseDirectory +
                         "\\..\\..\\DatabaseEntity Tests\\ModelTestFiles\\InchFile.psmodel"));

            Assert.That(activeModel.Units, Is.EqualTo(LengthUnits.Inches));
        }

        [Test]
        public void ConvertMMFileToInchFile()
        {
            _powerSHAPE.Models.Clear();

            PSModel activeModel = _powerSHAPE.Models.CreateModelFromFile(
                new File(AppDomain.CurrentDomain.BaseDirectory +
                         "\\..\\..\\DatabaseEntity Tests\\ModelTestFiles\\MMFile.psmodel"));
            activeModel.Units = LengthUnits.Inches;

            Assert.That(activeModel.Units, Is.EqualTo(LengthUnits.Inches));
            Assert.That(activeModel.GeneralTolerance, Is.EqualTo(0.001));
        }

        [Test]
        public void ConvertInchFileToMMFile()
        {
            _powerSHAPE.Models.Clear();

            PSModel activeModel = _powerSHAPE.Models.CreateModelFromFile(
                new File(AppDomain.CurrentDomain.BaseDirectory +
                         "\\..\\..\\DatabaseEntity Tests\\ModelTestFiles\\InchFile.psmodel"));
            activeModel.Units = LengthUnits.MM;

            Assert.That(activeModel.Units, Is.EqualTo(LengthUnits.MM));
            Assert.That(activeModel.GeneralTolerance, Is.EqualTo(0.0254));
        }

        #endregion
    }
}