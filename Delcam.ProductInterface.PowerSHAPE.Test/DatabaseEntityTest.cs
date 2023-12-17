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
using System.Reflection;
using System.Threading;
using Autodesk.ProductInterface.PowerSHAPE;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace Autodesk.ProductInterface.PowerSHAPETest
{
    /// <summary>
    /// This is a test class for DatabaseEntityTest and is intended
    /// to contain all DatabaseEntityTest Unit Tests
    /// </summary>
    [TestFixture]
    public abstract class DatabaseEntityTest<T> where T : PSDatabaseEntity
    {
        #region Fields

        protected PSAutomation _powerSHAPE;
        protected TestContext _testContextInstance;
        protected PSDatabaseEntitiesCollection<T> _powerSHAPECollection;

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

        protected DatabaseEntityTest()
        {
            ConnectToPowerShape();
        }

        #endregion

        #region Additional test attributes

        //Use TestInitialize to run code before running each test
        [SetUp]
        public virtual void MyTestInitialize()
        {
            try
            {
                // Start PowerSHAPE
                _powerSHAPE.Reset();

                // Switch FormUpdate and Dialogs off
                _powerSHAPE.FormUpdateOff();
                _powerSHAPE.DialogsOff();
            }
            catch (Exception)
            {
                // If something has gone wrong whilst running the tests in this TestFixture
                // then this will ensure we reconnect to PowerShape
                _powerSHAPE = null;
                ConnectToPowerShape();

                // Start PowerSHAPE
                _powerSHAPE.Reset();

                // Switch FormUpdate and Dialogs off
                _powerSHAPE.FormUpdateOff();
                _powerSHAPE.DialogsOff();
            }
        }

        [TearDown]
        public void MyTestCleanup()
        {
            try
            {
                if (TestContext.CurrentContext.Result.Outcome.Equals(ResultState.Failure))
                {
                    FileInfo[] files = new DirectoryInfo(Path.GetTempPath()).GetFiles("w_*.log");

                    //this section is what's really important for your application.
                    foreach (FileInfo file in files)
                    {
                        file.CopyTo(@"c:\temp\testfail\" + file.Name, true);
                    }
                }

                // Check that no dialogs need to be closed
                _powerSHAPE.Execute("CANCEL");

                // Close all models
                _powerSHAPE.Models.Clear();
                // Add a wait to see if we can solve an issue on the build server where PowerShape hangs when closing models
                Thread.Sleep(2000);

                // Switch FormUpdate and Dialogs back on
                _powerSHAPE.FormUpdateOn();
                _powerSHAPE.DialogsOn();
            }
            catch (Exception)
            {
            }
        }

        #endregion

        #region DatabaseEntity Tests

        /// <summary>
        /// This test checks that the Entity's Identifier is as expected
        /// </summary>
        public virtual void IdentifierTest(string fileToImport, string expectedIdentifier)
        {
            // Import entity
            PSEntity importedEntity = ImportAndGetEntity(fileToImport);

            // Check that the collection Identifier matches the entity type
            string actualIdentifier = (string) importedEntity
                .GetType().GetProperty("Identifier", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(importedEntity, null);
            Assert.AreEqual(expectedIdentifier, actualIdentifier);
        }

        /// <summary>
        /// A test for Id
        /// </summary>
        public virtual void IdTest(string file)
        {
            // Import entity
            PSEntity importedEntity = ImportAndGetEntity(file);

            // Check that the id is correct
            Assert.AreEqual(_powerSHAPE.ExecuteEx(IdentifierAccessor(importedEntity) + "[" + importedEntity.Name + "].ID"),
                            importedEntity.Id,
                            "Returned incorrect id");
        }

        protected string IdentifierAccessor(PSDatabaseEntity entity)
        {
            PropertyInfo propertyInfo =
                entity.GetType().GetProperty("Identifier", BindingFlags.NonPublic | BindingFlags.Instance);
            return (string) propertyInfo.GetValue(entity, null);
        }

        /// <summary>
        /// A test for Equals
        /// </summary>
        public virtual void EqualsTest(string file)
        {
            // Create comparison
            PSEntity comparisonDatabaseEntity = ImportAndGetEntity(file);

            // Compare collections
            Assert.IsTrue(_powerSHAPECollection[0].Equals(comparisonDatabaseEntity),
                          "Incorrectly returned that entities were not equal");
        }

        /// <summary>
        /// A test for Exists
        /// </summary>
        public virtual void ExistsTest(string file)
        {
            // Import entity
            PSEntity entity = ImportAndGetEntity(file);

            // Check that correct value is returned
            Assert.IsTrue(entity.Exists, "Incorrectly stated that entity did not exist");

            // Delete entity and check again
            entity.Delete();
            Assert.IsFalse(entity.Exists, "Incorrectly stated that entity existed");
        }

        #endregion

        #region Helper Functions

        private void ConnectToPowerShape()
        {
            // Initialise PowerSHAPE
            if (_powerSHAPE == null)
            {
                _powerSHAPE = new PSAutomation(InstanceReuse.UseExistingInstance, Modes.PShapeMode);
                _powerSHAPE.DialogsOff();
            }
        }

        protected PSEntity ImportAndGetEntity(string fileToImport)
        {
            // Import entity
            _powerSHAPE.ActiveModel.Import(new FileSystem.File(fileToImport));

            // Get entity
            if (_powerSHAPE.ActiveModel.SelectedItems.Count > 0)
            {
                return _powerSHAPE.ActiveModel.SelectedItems[0];
            }
            return _powerSHAPE.ActiveModel.CreatedItems[0];
        }

        protected List<PSEntity> ImportAndGetEntities(string fileToImport)
        {
            // Import entities
            _powerSHAPE.ActiveModel.Import(new FileSystem.File(fileToImport));

            // Get entities
            return _powerSHAPE.ActiveModel.SelectedItems.ToList();
        }

        #endregion
    }
}