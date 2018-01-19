// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using Autodesk.Geometry;
using Autodesk.ProductInterface.PowerSHAPE;
using NUnit.Framework;

namespace Autodesk.ProductInterface.PowerSHAPETest.DatabaseEntity_Tests.Entity_Tests
{
    [TestFixture]
    public class LevelTests
    {
        #region Fields

        private PSAutomation _powerSHAPE;

        #endregion

        #region Constructors

        public LevelTests()
        {
            // Initialise PowerSHAPE
            if (_powerSHAPE == null)
            {
                _powerSHAPE = new PSAutomation(InstanceReuse.UseExistingInstance, Modes.PShapeMode);
                _powerSHAPE.DialogsOff();
            }
        }

        #endregion

        #region Operation tests

        [Test]
        public void LevelClearTest()
        {
            _powerSHAPE.Reset();

            // Create a point on level 1 and one on level 2
            // Clear level 1 and check that only one point was removed
            var point1 = _powerSHAPE.ActiveModel.Points.CreatePoint(new Point());
            point1.LevelNumber = 1;
            var point2 = _powerSHAPE.ActiveModel.Points.CreatePoint(new Point());
            point2.LevelNumber = 2;

            _powerSHAPE.ActiveModel.Levels[1].Clear();

            Assert.IsFalse(point1.Exists);
            Assert.IsTrue(point2.Exists);
        }

        [Test]
        public void AddContentsToSelectionTest()
        {
            _powerSHAPE.Reset();

            // Create a point on level 1 and one on level 2
            // Clear level 1 and check that only one point was removed
            var point1 = _powerSHAPE.ActiveModel.Points.CreatePoint(new Point());
            point1.LevelNumber = 1;
            var point2 = _powerSHAPE.ActiveModel.Points.CreatePoint(new Point(10, 10, 10));
            point2.LevelNumber = 2;

            _powerSHAPE.ActiveModel.ClearSelectedItems();

            _powerSHAPE.ActiveModel.Levels[1].AddContentsToSelection();

            Assert.AreEqual(1, _powerSHAPE.ActiveModel.SelectedItems.Count);
            Assert.AreEqual(point1.Name, _powerSHAPE.ActiveModel.SelectedItems[0].Name);

            // Ensure the selection filter is turned off
            point2.AddToSelection();
            Assert.AreEqual(2, _powerSHAPE.ActiveModel.SelectedItems.Count);
        }

        #endregion
    }
}