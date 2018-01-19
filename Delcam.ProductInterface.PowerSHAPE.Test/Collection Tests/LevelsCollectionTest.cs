// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using Autodesk.ProductInterface.PowerSHAPE;
using NUnit.Framework;

namespace Autodesk.ProductInterface.PowerSHAPETest
{
    /// <summary>
    /// This is a test class for LevelsCollectionTest and is intended
    /// to contain all LevelsCollectionTest Unit Tests.
    /// </summary>
    [TestFixture]
    public class LevelsCollectionTest
    {
        #region Fields

        private readonly PSAutomation _powerSHAPE;
        private TestContext _testContextInstance;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext
        {
            get { return _testContextInstance; }
            set { _testContextInstance = value; }
        }

        #endregion

        #region Constructors

        public LevelsCollectionTest()
        {
            // Initialise PowerSHAPE
            if (_powerSHAPE == null)
            {
                _powerSHAPE = new PSAutomation(InstanceReuse.UseExistingInstance, Modes.PShapeMode);
                _powerSHAPE.DialogsOff();
            }
        }

        #endregion

        #region Additional test attributes

        //Use TestInitialize to run code before running each test
        [SetUp]
        public void MyTestInitialize()
        {
            // Close all PowerSHAPE models and create a new one
            _powerSHAPE.Reset();
        }

        //Use TestCleanup to run code after each test has run
        [TearDown]
        public void MyTestCleanup()
        {
            try
            {
                // Switch FormUpdate and Dialogs back on
                _powerSHAPE.FormUpdateOn();
                _powerSHAPE.DialogsOn();

                // Close model
                _powerSHAPE.Models.Clear();
            }
            catch (Exception)
            {
            }
        }

        #endregion

        /// <summary>
        /// A test for ActivateAllLevels
        /// </summary>
        [Test]
        [Ignore("Fails on build server but works on clients, investigation required")]
        public void ActivateAllLevelsTest()
        {
            // Deactivate all levels
            _powerSHAPE.ActiveModel.Levels.DeactivateAllLevels();

            // Activate all levels that are named and used
            _powerSHAPE.ActiveModel.IsLevelActiveFilterOn = false;
            _powerSHAPE.ActiveModel.IsLevelNamedFilterOn = true;
            _powerSHAPE.ActiveModel.IsLevelUsedFilterOn = true;
            _powerSHAPE.ActiveModel.Levels.ActivateAllLevels();

            // Check that all are deactivated
            Assert.AreEqual(6,
                            _powerSHAPE.ActiveModel.ActiveLevels.Count,
                            _powerSHAPE.ActiveModel.Levels.Count - _powerSHAPE.ActiveModel.ActiveLevels.Count +
                            " levels are still inactive.");
        }

        /// <summary>
        /// A test for DeactivateAllLevels
        /// </summary>
        [Test]
        public void DeactivateAllLevelsTest()
        {
            // Deactivate all levels
            _powerSHAPE.ActiveModel.Levels.DeactivateAllLevels();

            // Check that all are deactivated
            Assert.AreEqual(0,
                            _powerSHAPE.ActiveModel.ActiveLevels.Count,
                            _powerSHAPE.ActiveModel.ActiveLevels.Count + " levels are still active.");
        }

        /// <summary>
        /// A test for ReadLevels
        /// </summary>
        [Test]
        public void ReadLevelsTest()
        {
            // Check that collection contains them all
            Assert.AreEqual(1000, _powerSHAPE.ActiveModel.Levels.Count, "Not all levels are added to the collection");
        }

        /// <summary>
        /// A test for getting an Item by name
        /// </summary>
        [Test]
        public void GetLevelByName()
        {
            //Open test model
            _powerSHAPE.Models.CreateModelFromFile(new FileSystem.File(TestFiles.LEVELS_MODEL));

            // Check method works
            Assert.AreEqual(10, _powerSHAPE.ActiveModel.Levels.Item("Test Level 1").Number, "Did not return correct level");
        }

        /// <summary>
        /// A test for getting an Item by number
        /// </summary>
        [Test]
        public void GetLevelByNumber()
        {
            //Open test model
            _powerSHAPE.Models.CreateModelFromFile(new FileSystem.File(TestFiles.LEVELS_MODEL));

            // Check method works
            Assert.AreEqual("Test Level 1", _powerSHAPE.ActiveModel.Levels.Item(10).Name, "Did not return correct level");
        }
    }
}