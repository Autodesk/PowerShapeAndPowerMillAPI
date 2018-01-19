// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using System.Reflection;
using Autodesk.FileSystem;
using Autodesk.Geometry;
using Autodesk.ProductInterface.PowerSHAPE;
using NUnit.Framework;

namespace Autodesk.ProductInterface.PowerSHAPETest
{
    /// <summary>
    /// This is a test class for ElectrodeTest and is intended
    /// to contain all ElectrodeTest Unit Tests
    /// </summary>
    [TestFixture]
    public class ElectrodeTest : EntityTest<PSElectrode>
    {
        [SetUp]
        public override void MyTestInitialize()
        {
            base.MyTestInitialize();
            try
            {
                _powerSHAPECollection = _powerSHAPE.ActiveModel.Electrodes;
            }
            catch (Exception e)
            {
                Assert.Fail("_entityCollection could not be set to Active Model's ElectrodesCollection\n\n" + e.Message);
            }
        }

        #region DatabaseEntity Tests

        /// <summary>
        /// A test for Id
        /// </summary>
        [Test]
        public void ElectrodeIdTest()
        {
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.SINGLE_ELECTRODE_MODEL));
            _powerSHAPECollection = _powerSHAPE.ActiveModel.Electrodes;
            var importedEntity = _powerSHAPE.ActiveModel.Electrodes[0];

            // Check that the id is correct
            Assert.AreEqual(_powerSHAPE.ExecuteEx(IdentifierAccessor(importedEntity) + "[" + importedEntity.Name + "].ID"),
                            importedEntity.Id,
                            "Returned incorrect id");
        }

        /// <summary>
        /// A test for Identifier
        /// </summary>
        [Test]
        public void ElectrodeIdentifierTest()
        {
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.SINGLE_ELECTRODE_MODEL));
            _powerSHAPECollection = _powerSHAPE.ActiveModel.Electrodes;
            var importedEntity = _powerSHAPE.ActiveModel.Electrodes[0];

            // Check that the collection Identifier matches the entity type
            string actualIdentifier = (string) importedEntity
                .GetType().GetProperty("Identifier", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(importedEntity, null);
            Assert.AreEqual("ELECTRODE", actualIdentifier);
        }

        /// <summary>
        /// A test for Equals
        /// </summary>
        [Test]
        public void ElectrodeEqualsTest()
        {
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.SINGLE_ELECTRODE_MODEL));
            _powerSHAPECollection = _powerSHAPE.ActiveModel.Electrodes;
            var importedEntity = _powerSHAPE.ActiveModel.Electrodes[0];

            // Compare collections
            Assert.IsTrue(_powerSHAPECollection[0].Equals(importedEntity), "Incorrectly returned that entities were not equal");
        }

        /// <summary>
        /// A test for Exists
        /// </summary>
        [Test]
        public void ElectrodeExistsTest()
        {
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.SINGLE_ELECTRODE_MODEL));
            _powerSHAPECollection = _powerSHAPE.ActiveModel.Electrodes;
            var importedEntity = _powerSHAPE.ActiveModel.Electrodes[0];

            // Check that correct value is returned
            Assert.IsTrue(importedEntity.Exists, "Incorrectly stated that entity did not exist");

            // Delete entity and check again
            importedEntity.Delete();
            Assert.IsFalse(importedEntity.Exists, "Incorrectly stated that entity existed");
        }

        #endregion

        #region Entity Tests

        [Test]
        public void AddElectrodeToSelectionTest()
        {
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.SINGLE_ELECTRODE_MODEL));
            _powerSHAPECollection = _powerSHAPE.ActiveModel.Electrodes;
            var importedEntity = _powerSHAPE.ActiveModel.Electrodes[0];
        }

        [Test]
        public void BlankElectrodeTest()
        {
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.SINGLE_ELECTRODE_MODEL));
            _powerSHAPECollection = _powerSHAPE.ActiveModel.Electrodes;
            var importedEntity = _powerSHAPE.ActiveModel.Electrodes[0];

            // Select everything
            _powerSHAPE.ActiveModel.SelectAll(true);

            // Add desired entity to selection, cancelling current selection
            importedEntity.AddToSelection(true);

            var count = _powerSHAPE.ActiveModel.SelectedItems.Count;

            // Check the selection contains the correct entity
            if (count == 0)
            {
                Assert.Fail("Failed to select anything in PowerSHAPE");
            }
            else if (count == 2)
            {
                Assert.Fail("Selected everything in PowerSHAPE");
            }

            // Check the other entity is not selected
            Assert.IsTrue(_powerSHAPE.ActiveModel.SelectedItems.Contains(importedEntity), "Select incorrect entity");
        }

        [Test]
        public void ElectrodeBoundingBoxTest()
        {
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.SINGLE_ELECTRODE_MODEL));
            _powerSHAPECollection = _powerSHAPE.ActiveModel.Electrodes;
            var importedEntity = _powerSHAPE.ActiveModel.Electrodes[0];

            // Check bounding box
            Assert.AreEqual(new Point(-23.514009, 21.918362, 6.5),
                            importedEntity.BoundingBox.MaximumBounds,
                            "Bounding box maximum bounds are incorrect");
            Assert.AreEqual(new Point(-35.514009, -26.081648, -5.5),
                            importedEntity.BoundingBox.MinimumBounds,
                            "Bounding box minimum bounds are incorrect");
        }

        [Test]
        public void CopyElectrodeTest()
        {
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.SINGLE_ELECTRODE_MODEL));
            _powerSHAPECollection = _powerSHAPE.ActiveModel.Electrodes;
            var importedEntity = _powerSHAPE.ActiveModel.Electrodes[0];

            // Copy and paste entity
            importedEntity.Duplicate();
            _powerSHAPE.ActiveModel.SelectAll(false);
            if (_powerSHAPE.ActiveModel.SelectedItems.Count == 0)
            {
                _powerSHAPE.ActiveModel.Workplanes.AddToSelection(false);
            }
            Assert.AreEqual(2, _powerSHAPE.ActiveModel.SelectedItems.Count, "Entity was not pasted");
        }

        [Test]
        public void DeleteElectrodeTest()
        {
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.SINGLE_ELECTRODE_MODEL));
            _powerSHAPECollection = _powerSHAPE.ActiveModel.Electrodes;
            var importedEntity = _powerSHAPE.ActiveModel.Electrodes[0];

            // Delete entity
            importedEntity.Delete();
            _powerSHAPE.ActiveModel.SelectAll(false);
            Assert.AreEqual(0, _powerSHAPE.ActiveModel.SelectedItems.Count, "Entity not deleted from PowerSHAPE");
            Assert.AreEqual(0, _powerSHAPECollection.Count, "Entity not removed from collection");
        }

        [Test]
        public void ElectrodeLevelTest()
        {
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.SINGLE_ELECTRODE_MODEL));
            _powerSHAPECollection = _powerSHAPE.ActiveModel.Electrodes;
            var importedEntity = _powerSHAPE.ActiveModel.Electrodes[0];

            // Check level
            Assert.AreEqual(_powerSHAPE.ActiveModel.Levels[0], importedEntity.Level, "Returned level is incorrect");

            // Change level
            var newLevel = _powerSHAPE.ActiveModel.Levels[2];
            importedEntity.Level = newLevel;
            Assert.AreEqual(newLevel, importedEntity.Level, "Failed to change level");
        }

        [Test]
        public void ElectrodeLevelNumberTest()
        {
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.SINGLE_ELECTRODE_MODEL));
            _powerSHAPECollection = _powerSHAPE.ActiveModel.Electrodes;
            var importedEntity = _powerSHAPE.ActiveModel.Electrodes[0];

            // Check level
            Assert.AreEqual(0, importedEntity.LevelNumber, "Returned level number is incorrect");

            // Change level number
            importedEntity.LevelNumber = 2;
            Assert.AreEqual(2, importedEntity.LevelNumber, "Failed to change level");
        }

        [Test]
        public void MirrorElectrodeTest()
        {
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.SINGLE_ELECTRODE_MODEL));
            _powerSHAPECollection = _powerSHAPE.ActiveModel.Electrodes;
            var importedEntity = _powerSHAPE.ActiveModel.Electrodes[0];
        }

        [Test]
        public void MoveElectrodeTest()
        {
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.SINGLE_ELECTRODE_MODEL));
            _powerSHAPECollection = _powerSHAPE.ActiveModel.Electrodes;
            var importedEntity = _powerSHAPE.ActiveModel.Electrodes[0];

            importedEntity.Mirror(Planes.YZ, new Point(0, 0, 0));

            // Check that operation did not duplicate entity
            Assert.AreEqual(1, _powerSHAPECollection.Count, "Entity was duplicated in PowerSHAPE");
        }

        [Test]
        public void MoveElectrodeCreatingCopiesTest()
        {
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.SINGLE_ELECTRODE_MODEL));
            _powerSHAPECollection = _powerSHAPE.ActiveModel.Electrodes;
            var importedEntity = _powerSHAPE.ActiveModel.Electrodes[0];

            // Get expected centre of the entity after the operation
            var moveVector = new Vector(0, 10, 0);
            var expectedMovedCentre = importedEntity.BoundingBox.VolumetricCentre + moveVector;

            // Move entity
            importedEntity.MoveByVector(moveVector, 0);
            Assert.AreEqual(expectedMovedCentre,
                            importedEntity.BoundingBox.VolumetricCentre,
                            "Entity was not moved");
        }

        [Test]
        public void MoveElectrodeDifferentOriginTest()
        {
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.SINGLE_ELECTRODE_MODEL));
            _powerSHAPECollection = _powerSHAPE.ActiveModel.Electrodes;
            var importedEntity = _powerSHAPE.ActiveModel.Electrodes[0];

            // Get entity keypoint
            var origin = importedEntity.BoundingBox.VolumetricCentre;

            // Move entity
            importedEntity.MoveBetweenPoints(origin, new Point(), 0);

            // Get new entity keypoint and check that operation was successful
            origin = importedEntity.BoundingBox.VolumetricCentre;
            Assert.AreEqual(new Point(), origin, "Entity was not moved");
        }

        [Test]
        public void ElectrodeNameTest()
        {
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.SINGLE_ELECTRODE_MODEL));
            _powerSHAPECollection = _powerSHAPE.ActiveModel.Electrodes;
            var importedEntity = _powerSHAPE.ActiveModel.Electrodes[0];
            importedEntity.Name = "NewName";

            // Ensures that PowerShape entity was updated
            Assert.AreEqual(1, _powerSHAPE.ExecuteEx("Electrode[NewName].EXISTS"));
        }

        [Test]
        public void RemoveElectrodeFromSelectionTest()
        {
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.SINGLE_ELECTRODE_MODEL));
            _powerSHAPECollection = _powerSHAPE.ActiveModel.Electrodes;
            var entityToDeselect = _powerSHAPE.ActiveModel.Electrodes[0];

            // Get other entity
            var otherEntity = ImportAndGetEntity(TestFiles.SINGLE_SURFACE);

            // Select everything
            _powerSHAPE.ActiveModel.SelectAll(true);
            if (_powerSHAPE.ActiveModel.SelectedItems.Count == 0)
            {
                _powerSHAPE.ActiveModel.Workplanes.AddToSelection(false);
            }

            // Remove desired entity from selection, cancelling current selection
            entityToDeselect.RemoveFromSelection();

            // Check the selection doesn't contain the entity
            if (_powerSHAPE.ActiveModel.SelectedItems.Count == 0) // Everything was deselected
            {
                Assert.Fail("Deselected everything in PowerSHAPE");
            }
            else if (_powerSHAPE.ActiveModel.SelectedItems.Count == 2) // Nothing was deselected
            {
                Assert.Fail("Nothing was deselected in PowerSHAPE");
            }

            // Check the entity is not selected
            Assert.IsTrue(_powerSHAPE.ActiveModel.SelectedItems.Contains(otherEntity), "Deselected incorrect entity");
        }

        [Test]
        public void RotateElectrodeTest()
        {
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.SINGLE_ELECTRODE_MODEL));
            _powerSHAPECollection = _powerSHAPE.ActiveModel.Electrodes;
            var importedEntity = _powerSHAPE.ActiveModel.Electrodes[0];

            // Rotate entity
            importedEntity.Rotate(Axes.Z, 20, 0, importedEntity.BoundingBox.VolumetricCentre);
            Assert.AreEqual(1, _powerSHAPE.ActiveModel.UpdatedItems.Count, "Entity was not rotated");
        }

        #endregion

        #region Electrode Tests

        [Test]
        public void UndersizeRoughTest()
        {
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.SINGLE_ELECTRODE_MODEL));
            var electrode = _powerSHAPE.ActiveModel.Electrodes[0];
            electrode.UndersizeRough = 1.2;
            Assert.AreEqual(1.2, electrode.UndersizeRough);
        }

        [Test]
        public void UndersizeSemiTest()
        {
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.SINGLE_ELECTRODE_MODEL));
            var electrode = _powerSHAPE.ActiveModel.Electrodes[0];
            electrode.UndersizeSemi = 1.3;
            Assert.AreEqual(1.3, electrode.UndersizeSemi);
        }

        [Test]
        public void UndersizeFinishTest()
        {
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.SINGLE_ELECTRODE_MODEL));
            var electrode = _powerSHAPE.ActiveModel.Electrodes[0];
            electrode.UndersizeFinish = 1.4;
            Assert.AreEqual(1.4, electrode.UndersizeFinish);
        }

        [Test]
        public void QuantityRoughTest()
        {
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.SINGLE_ELECTRODE_MODEL));
            var electrode = _powerSHAPE.ActiveModel.Electrodes[0];
            electrode.UndersizeRough = 1.1;
            electrode.QuantityRough = 2;
            Assert.AreEqual(2, electrode.QuantityRough);
        }

        [Test]
        public void QuantitySemiTest()
        {
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.SINGLE_ELECTRODE_MODEL));
            var electrode = _powerSHAPE.ActiveModel.Electrodes[0];
            electrode.UndersizeSemi = 1.1;
            electrode.QuantitySemi = 3;
            Assert.AreEqual(3, electrode.QuantitySemi);
        }

        [Test]
        public void QuantityFinishTest()
        {
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.SINGLE_ELECTRODE_MODEL));
            var electrode = _powerSHAPE.ActiveModel.Electrodes[0];
            electrode.UndersizeFinish = 1.1;
            electrode.QuantityFinish = 4;
            Assert.AreEqual(4, electrode.QuantityFinish);
        }

        [Test]
        public void SparkGapTest()
        {
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.SINGLE_ELECTRODE_MODEL));
            var electrode = _powerSHAPE.ActiveModel.Electrodes[0];
            electrode.SparkGap = 4.7;
            Assert.AreEqual(4.7, electrode.SparkGap);
        }

        [Test]
        public void SurfaceFinishTest()
        {
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.SINGLE_ELECTRODE_MODEL));
            var electrode = _powerSHAPE.ActiveModel.Electrodes[0];
            electrode.SurfaceFinish = "Lovely";
            Assert.AreEqual("Lovely", electrode.SurfaceFinish);
        }

        [Test]
        public void ProjectedAreaTest()
        {
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.SINGLE_ELECTRODE_MODEL));
            var electrode = _powerSHAPE.ActiveModel.Electrodes[0];
            Assert.AreEqual(8.3870000000000005, electrode.ProjectedArea);
        }

        [Test]
        public void BurnVectorTest()
        {
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.SINGLE_ELECTRODE_MODEL));
            var electrode = _powerSHAPE.ActiveModel.Electrodes[0];
            Assert.AreEqual(new Vector(0, -1, 0), electrode.BurnVector);
        }

        [Test]
        public void VectorClearanceTest()
        {
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.SINGLE_ELECTRODE_MODEL));
            var electrode = _powerSHAPE.ActiveModel.Electrodes[0];
            Assert.AreEqual((MM) 0.0, electrode.VectorClearance);
        }

        [Test]
        public void AngleATest()
        {
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.SINGLE_ELECTRODE_MODEL));
            var electrode = _powerSHAPE.ActiveModel.Electrodes[0];
            Assert.AreEqual((Degree) 270, electrode.AngleA);
        }

        [Test]
        public void AngleBTest()
        {
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.SINGLE_ELECTRODE_MODEL));
            var electrode = _powerSHAPE.ActiveModel.Electrodes[0];
            Assert.AreEqual((Degree) 90, electrode.AngleB);
        }

        [Test]
        public void AngleCTest()
        {
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.SINGLE_ELECTRODE_MODEL));
            var electrode = _powerSHAPE.ActiveModel.Electrodes[0];
            Assert.AreEqual((Degree) 90, electrode.AngleC);
        }

        [Test]
        public void BlankNameTest()
        {
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.SINGLE_ELECTRODE_MODEL));
            var electrode = _powerSHAPE.ActiveModel.Electrodes[0];
            Assert.AreEqual("3R-494-S12x50", electrode.BlankName);
        }

        [Test]
        public void IsBlankRectangularTest()
        {
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.SINGLE_ELECTRODE_MODEL));
            var electrode = _powerSHAPE.ActiveModel.Electrodes[0];
            Assert.AreEqual(true, electrode.IsBlankRectangular);
        }

        [Test]
        public void BlankLengthTest()
        {
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.SINGLE_ELECTRODE_MODEL));
            var electrode = _powerSHAPE.ActiveModel.Electrodes[0];
            Assert.AreEqual((MM) 12, electrode.BlankLength);
        }

        [Test]
        public void BlankWidthTest()
        {
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.SINGLE_ELECTRODE_MODEL));
            var electrode = _powerSHAPE.ActiveModel.Electrodes[0];
            Assert.AreEqual((MM) 12, electrode.BlankWidth);
        }

        [Test]
        public void BlankDiameterTest()
        {
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.SINGLE_ELECTRODE_MODEL));
            var electrode = _powerSHAPE.ActiveModel.Electrodes[0];
            Assert.AreEqual((MM) (-1), electrode.BlankDiameter);
        }

        [Test]
        public void BlankMaterialTest()
        {
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.SINGLE_ELECTRODE_MODEL));
            var electrode = _powerSHAPE.ActiveModel.Electrodes[0];
            Assert.AreEqual(null, electrode.BlankMaterial);
        }

        [Test]
        public void BaseHeightTest()
        {
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.SINGLE_ELECTRODE_MODEL));
            var electrode = _powerSHAPE.ActiveModel.Electrodes[0];
            Assert.AreEqual((MM) 41.482, electrode.BaseHeight);
        }

        [Test]
        public void RotationTest()
        {
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.SINGLE_ELECTRODE_MODEL));
            var electrode = _powerSHAPE.ActiveModel.Electrodes[0];
            Assert.AreEqual((Degree) 0.0, electrode.Rotation);
        }

        [Test]
        public void IsCopyTest()
        {
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.THREE_ELECTRODES_MODEL));
            var electrode = _powerSHAPE.ActiveModel.Electrodes[0];
            Assert.IsFalse(electrode.IsCopy);
            var electrode1 = _powerSHAPE.ActiveModel.Electrodes[1];
            Assert.IsTrue(electrode1.IsCopy);
        }

        [Test]
        public void ParentTest()
        {
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.THREE_ELECTRODES_MODEL));
            var electrode = _powerSHAPE.ActiveModel.Electrodes[1];
            var parent = electrode.Parent;
            Assert.AreSame(_powerSHAPE.ActiveModel.Electrodes[0], parent);
        }

        [Test]
        public void CopiesTest()
        {
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.THREE_ELECTRODES_MODEL));
            var electrode = _powerSHAPE.ActiveModel.Electrodes[0];
            var copies = electrode.Copies;
            Assert.AreSame(_powerSHAPE.ActiveModel.Electrodes[1], copies[0]);
            Assert.AreSame(_powerSHAPE.ActiveModel.Electrodes[2], copies[1]);
        }

        [Test]
        public void CopiesTest2()
        {
            // Test only works for later PowerShapes
            if (_powerSHAPE.Version.Major >= 16)
            {
                _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.MANY_ELECTRODES_MODEL));
                var electrode = _powerSHAPE.ActiveModel.Electrodes.GetByName("M10015_varsikeerna_elkku1");
                var copies = electrode.Copies;
                Assert.AreEqual(1, copies.Count);
            }
        }

        [Test]
        public void CopiesTest3()
        {
            // Test only works for later PowerShapes
            if (_powerSHAPE.Version.Major >= 16)
            {
                _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.MANY_ELECTRODES_MODEL));
                var electrode = _powerSHAPE.ActiveModel.Electrodes.GetByName("M10015_varsikeerna_elkku3");
                var copies = electrode.Copies;
                Assert.AreEqual(0, copies.Count);
            }
        }

        [Test]
        public void MachiningWorkplaneNullTest()
        {
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.SINGLE_ELECTRODE_MODEL));
            var electrode = _powerSHAPE.ActiveModel.Electrodes[0];
            Assert.IsNull(electrode.MachiningWorkplane);
        }

        [Test]
        public void ActiveWorkplaneNullTest()
        {
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.SINGLE_ELECTRODE_MODEL));
            var electrode = _powerSHAPE.ActiveModel.Electrodes[0];
            Assert.IsNull(electrode.ActiveWorkplane);
        }

        [Test]
        public void ShouldRead_ActiveWorkplane()
        {
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.ELECTRODES_MODEL));
            var electrode = _powerSHAPE.ActiveModel.Electrodes[0];
            Assert.AreEqual("1", electrode.ActiveWorkplane.Name);
        }

        [Test]
        public void ShouldRead_MachiningWorkplane()
        {
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.ELECTRODES_MODEL));
            var electrode = _powerSHAPE.ActiveModel.Electrodes[0];
            Assert.AreEqual("electrode2", electrode.MachiningWorkplane.Name);
        }

        [Test]
        public void ShouldRead_ActiveSolid()
        {
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.ELECTRODES_MODEL));
            var electrode = _powerSHAPE.ActiveModel.Electrodes[0];
            Assert.AreEqual("1", electrode.ActiveSolid.Name);
        }

        [Test]
        public void Read_ActiveSolidNullTest()
        {
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.SINGLE_ELECTRODE_MODEL));
            var electrode = _powerSHAPE.ActiveModel.Electrodes[0];
            Assert.IsNull(electrode.ActiveSolid);
        }

        #endregion
    }
}