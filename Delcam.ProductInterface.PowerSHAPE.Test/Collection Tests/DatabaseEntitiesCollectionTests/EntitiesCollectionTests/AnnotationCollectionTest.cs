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

namespace Autodesk.ProductInterface.PowerSHAPETest.Collection_Tests.DatabaseEntitiesCollectionTests.
    EntitiesCollectionTests
{
    /// <summary>
    /// This is a test class for AnnotationCollectionTest and is intended
    /// to contain all AnnotationCollectionTest Unit Tests
    /// </summary>
    [TestFixture]
    public class AnnotationCollectionTest : EntitiesCollectionTest<PSAnnotation>
    {
        [Test]
        public void CreateAnnotationText()
        {
            _powerSHAPE.ActiveModel.Annotations.CreateAnnotation("Hello",
                                                                 "Arial",
                                                                 12,
                                                                 new Point(0, 0, 0));

            Assert.AreEqual(_powerSHAPE.ActiveModel.Annotations.Count, 1, "Failed to add annotation to collection");
            Assert.AreEqual(_powerSHAPE.ActiveModel.CreatedItems.Count, 1, "Failed to create annotation in PowerSHAPE");

            _powerSHAPE.ActiveModel.Annotations.CreateAnnotation("Hello",
                                                                 "Arial",
                                                                 12,
                                                                 new Point(0, 0, 0),
                                                                 2,
                                                                 TextJustifications.Right,
                                                                 TextOrigins.LeftCentre);

            Assert.AreEqual(_powerSHAPE.ActiveModel.Annotations.Count, 2, "Failed to add annotation to collection");
            Assert.AreEqual(_powerSHAPE.ActiveModel.CreatedItems.Count, 1, "Failed to create annotation in PowerSHAPE");
        }
    }
}