// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using Autodesk.ProductInterface.PowerSHAPE;
using NUnit.Framework;

namespace Autodesk.ProductInterface.PowerSHAPETest
{
    /// <summary>
    /// This is a test class for GenericCurvesCollectionTest and is intended
    /// to contain all GenericCurvesCollectionTest Unit Tests
    /// </summary>
    [TestFixture]
    public abstract class GenericCurvesCollectionTest<T> : EntitiesCollectionTest<T> where T : PSGenericCurve
    {
        #region Fields

        protected PSGenericCurvesCollection<T> _genericCurvesCollection;

        #endregion

        [SetUp]
        public override void MyTestInitialize()
        {
            base.MyTestInitialize();
            _genericCurvesCollection = (PSGenericCurvesCollection<T>) _powerSHAPECollection;
        }

        #region GenericCurvesCollection Tests

        public virtual void CreateCurveFromBreakTest(string fileToImport)
        {
            // Import generic curve
            _powerSHAPE.ActiveModel.Import(new FileSystem.File(fileToImport));

            // Get generic curve
            PSGenericCurve genericCurve = _powerSHAPE.ActiveModel.SelectedItems[0] as PSGenericCurve;

            // Break generic curve
            PSGenericCurve newCurve = _genericCurvesCollection.CreateCurveFromBreakPoint((T) genericCurve, 2);

            // Check that curve has been created
            Assert.AreEqual(2, _genericCurvesCollection.Count, "Curve was not broken");
            Assert.AreEqual(3, genericCurve.NumberPoints, "Curve was not broken at Point 3");
            Assert.IsTrue(genericCurve.Length > newCurve.Length, "New curve was set to be start of original curve, not end");
        }

        #endregion
    }
}