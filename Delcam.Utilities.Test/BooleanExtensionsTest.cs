// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using Autodesk.Extensions;
using NUnit.Framework;

namespace Utilities.Test
{
    /// <summary>
    /// This is a test class for BooleanExtensionsTest and is intended
    /// to contain all BooleanExtensionsTest Unit Tests
    /// </summary>
    [TestFixture]
    public class BooleanExtensionsTest
    {
        private string thisTestDirectory = Environment.GetEnvironmentVariable("TEMP") + @"\tmpTestDirectory";

        //private string tmpDir = ".\testDelCamDir";
        //couldn't use test instance test directory - instead hard coded temporary directory
        /// <summary>
        /// Gets or sets the test context test directory.
        /// </summary>
        public string TestDirectory
        {
            get { return thisTestDirectory; }
            set { thisTestDirectory = value; }
        }

        [SetUp]
        protected void SetTestDirectory()
        {
            TestDirectory = Environment.GetEnvironmentVariable("TEMP") + @"\delcamTmp\BooleanExtensionsTest";
            System.IO.Directory.CreateDirectory(TestDirectory);
        }

        [TearDown]
        protected void DeleteTestDirectory()
        {
            if (System.IO.Directory.Exists(TestDirectory))
            {
                try
                {
                    System.IO.Directory.Delete(TestDirectory, true);
                }
                catch
                {
                }
            }
        }

        #region Additional test attributes

        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //

        #endregion

        /// <summary>
        /// A test for ToYesNo
        /// </summary>
        [Test]
        public void ToYesNoTest()
        {
            bool inputBoolean = false;
            string expected = string.Empty;
            string actual;
            expected = "NO";
            actual = inputBoolean.ToYesNo();
            Assert.AreEqual(expected, actual, "Expected NO");

            // try instanstance
            actual = inputBoolean.ToYesNo();
            Assert.AreEqual(expected, actual, "Expected NO on instance");

            inputBoolean = true;
            expected = "YES";
            actual = inputBoolean.ToYesNo();
            Assert.AreEqual(expected, actual, "Expected YES");

            // try instanstance
            actual = inputBoolean.ToYesNo();
            Assert.AreEqual(expected, actual, "Expected YES on instance");
        }

        /// <summary>
        /// A test for ToOnOff
        /// </summary>
        [Test]
        public void ToOnOffTest()
        {
            bool inputBoolean = false;
            string expected = string.Empty;
            string actual;
            expected = "OFF";
            actual = inputBoolean.ToOnOff();
            Assert.AreEqual(expected, actual, "Expected OFF");

            // try instanstance
            actual = inputBoolean.ToOnOff();
            Assert.AreEqual(expected, actual, "Expected OFF on instance");

            inputBoolean = true;
            expected = "ON";
            actual = inputBoolean.ToOnOff();
            Assert.AreEqual(expected, actual, "Expected ON");

            // try instanstance
            actual = inputBoolean.ToOnOff();
            Assert.AreEqual(expected, actual, "Expected ON on instance");
        }

        /// <summary>
        /// A test for ToOneZero
        /// </summary>
        [Test]
        public void ToOneZeroTest()
        {
            bool inputBoolean = false;
            int expected = 0;
            int actual;
            expected = 0;
            actual = inputBoolean.ToOneZero();
            Assert.AreEqual(expected, actual, "Expected 0");

            // try instanstance
            actual = inputBoolean.ToOneZero();
            Assert.AreEqual(expected, actual, "Expected 1 on instance");

            inputBoolean = true;
            expected = 1;
            actual = inputBoolean.ToOneZero();
            Assert.AreEqual(expected, actual, "Expected 1");

            // try instanstance
            actual = inputBoolean.ToOneZero();
            Assert.AreEqual(expected, actual, "Expected 1 on instance");
        }
    }
}