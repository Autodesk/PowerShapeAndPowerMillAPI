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
using Autodesk.Geometry;
using Autodesk.ProductInterface.PowerSHAPE;
using NUnit.Framework;

namespace Autodesk.ProductInterface.PowerSHAPETest
{
    /// <summary>
    /// This is a test class for LinesCollectionTest and is intended
    /// to contain all LinesCollectionTest Unit Tests
    /// </summary>
    [TestFixture]
    public class LinesCollectionTest : EntitiesCollectionTest<PSLine>
    {
        [SetUp]
        public override void MyTestInitialize()
        {
            base.MyTestInitialize();
            try
            {
                _powerSHAPECollection = _powerSHAPE.ActiveModel.Lines;
            }
            catch (Exception e)
            {
                Assert.Fail("_powerSHAPECollection could not be set to Active Model's LinesCollection\n\n" + e.Message);
            }
        }

        #region LinesCollection Tests

        /// <summary>
        /// A test for CreateLine
        /// </summary>
        [Test]
        public void CreateLineTest()
        {
            // Create line from points
            _powerSHAPE.ActiveModel.Lines.CreateLine(new Point(10, 20, 0),
                                                     new Point(30, 20, 0));
            Assert.AreEqual(_powerSHAPE.ActiveModel.Lines.Count, 1, "Failed to add line to collection");
            Assert.AreEqual(_powerSHAPE.ActiveModel.CreatedItems.Count, 1, "Failed to create line in PowerSHAPE");
        }

        /// <summary>
        /// A test for CreateLines
        /// </summary>
        [Test]
        public void CreateLinesTest()
        {
            // Create line from points
            _powerSHAPE.ActiveModel.Lines.CreateLines(new[]
            {
                new Point(10, 20, 0),
                new Point(20, 20, 0),
                new Point(30, 10, 0),
                new Point(30, 20, 0)
            });
            Assert.AreEqual(_powerSHAPE.ActiveModel.Lines.Count, 3, "Failed to add lines to collection");
        }

        #endregion

        #region EntitiesCollection Tests

        /// <summary>
        /// A test for RemoveAt
        /// </summary>
        [Test]
        public void RemoveLineAtTest()
        {
            RemoveAtTest(TestFiles.THREE_LINES, "Line2");
        }

        /// <summary>
        /// A test for Remove
        /// </summary>
        [Test]
        public void RemoveLineTest()
        {
            RemoveTest(TestFiles.THREE_LINES, "Line2");
        }

        /// <summary>
        /// A test for Clear
        /// </summary>
        [Test]
        public void ClearLinesTest()
        {
            ClearTest(TestFiles.THREE_LINES, TestFiles.SINGLE_SURFACE);
        }

        /// <summary>
        /// A test for AddToSelection
        /// </summary>
        [Test]
        public void AddLinesToSelectionTest()
        {
            AddToSelectionTest(TestFiles.THREE_LINES, TestFiles.SINGLE_SURFACE);
        }

        /// <summary>
        /// A test for RemoveToSelection
        /// </summary>
        [Test]
        public void RemoveLinesFromSelectionTest()
        {
            RemoveFromSelectionTest(TestFiles.THREE_LINES, TestFiles.SINGLE_SURFACE);
        }

        #endregion

        #region DatabaseEntitiesCollection Tests

        #region Properties

        /// <summary>
        /// A test for the Identifier
        /// </summary>
        [Test]
        public void IdentifierLineTest()
        {
            IdentifierTest(new File(TestFiles.SINGLE_LINE), "LINE");
        }

        [Test]
        public void GetLineByNameTest()
        {
            // Import file with multiple entities
            _powerSHAPE.ActiveModel.Import(new File(TestFiles.THREE_LINES));

            // Get an entity by its name
            var namedEntity = _powerSHAPECollection.GetByName("Line2");

            // Check that the correct entity was returned
            Assert.IsTrue(namedEntity.Length == 65, "Incorrect entity returned");
        }

        #endregion

        #region Operations

        [Test]
        public void ContainsLineTest()
        {
            ContainsTest(new File(TestFiles.SINGLE_LINE));
        }

        [Test]
        public void CountLineTest()
        {
            CountTest(new File(TestFiles.SINGLE_LINE));
        }

        [Test]
        public void EqualsLineTest()
        {
            EqualsTest();
        }

        [Test]
        public void LineItemTest()
        {
            ItemTest(new File(TestFiles.SINGLE_LINE));
        }

        [Test]
        public void LineLastItemTest()
        {
            LastItemTest(new File(TestFiles.THREE_LINES));
        }

        #endregion

        #endregion
    }
}