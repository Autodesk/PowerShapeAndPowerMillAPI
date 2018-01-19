// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using System.IO;
using System.Reflection;
using Autodesk.ProductInterface.PowerSHAPE;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

//using NUnit.Framework;

namespace Autodesk.ProductInterface.PowerSHAPETest
{
    /// <summary>
    /// This is a test class for DatabaseEntitiesCollectionTest and is intended
    /// to contain all DatabaseEntitiesCollectionTest Unit Tests
    /// </summary>
    [TestFixture]
    public abstract class DatabaseEntitiesCollectionTest<T> where T : PSDatabaseEntity
    {
        #region Fields

        protected static PSAutomation _powerSHAPE;
        protected TestContext _testContextInstance;
        protected readonly string _collectionType;
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

        protected DatabaseEntitiesCollectionTest()
        {
            ConnectToPowerShape();

            _collectionType = GetType().Name.Replace("sCollectionTest", "").ToLower();
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

        //Use TestCleanup to run code after each test has run
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

        #region DatabaseEntitiesCollection Tests

        #region Properties

        /// <summary>
        /// This test checks that the Identifier is as expected.  For use on collections containing DatabaseEntities, the
        /// test must be overriden.
        /// </summary>
        /// <param name="entityToImport">A single PSEntity to import</param>
        /// <param name="expectedIdentifier">The Identifier to check against</param>
        public virtual void IdentifierTest(FileSystem.File entityToImport, string expectedIdentifier)
        {
        }

        #endregion

        #region Operations

        public void AddTest(FileSystem.File entityToAddFile)
        {
        }

        public void ContainsTest()
        {
        }

        /// <summary>
        /// A test for Count
        /// </summary>
        public virtual void CountTest(FileSystem.File entityToAddFile)
        {
            // Check collection
            Assert.AreEqual(0, _powerSHAPECollection.Count, "Collection count is not 0");
        }

        /// <summary>
        /// A test for Equals
        /// </summary>
        public virtual void EqualsTest()
        {
            // Create comparison collection
            PSDatabaseEntitiesCollection<T> comparisonCollection = _powerSHAPECollection;

            // Compare collections
            Assert.IsTrue(_powerSHAPECollection.Equals(comparisonCollection),
                          "Incorrectly returned that collections were not equal");
        }

        public void IndexOfTest()
        {
        }

        public void InsertTest()
        {
        }

        public void ItemTest()
        {
        }

        public void LastItemTest()
        {
        }

        #endregion

        #endregion

        #region Private Accessors

        /// <summary>
        /// Accesses and runs the friend constructor that takes the powershape automation object as is parameter.  Used for testing the add operation as well as others.
        /// </summary>
        /// <param name="classType">The collection type to get the constructor on</param>
        /// <param name="automation">The powershape instance under test</param>
        /// <returns>A new collection of objects</returns>
        protected object RunFriendConstuctor(Type classType, PSAutomation automation)
        {
            Type[] parameters = {typeof(PSAutomation)};
            var constructorInfo = classType.GetConstructor(BindingFlags.NonPublic, null, parameters, null);
            object[] constructorParameters = {automation};
            return constructorInfo.Invoke(null, constructorParameters);
        }

        #endregion

        #region Operations

        private void ConnectToPowerShape()
        {
            // Initialise PowerSHAPE
            if (_powerSHAPE == null)
            {
                _powerSHAPE = new PSAutomation(InstanceReuse.UseExistingInstance, Modes.PShapeMode);
                _powerSHAPE.DialogsOff();
            }
        }

        #endregion
    }
}