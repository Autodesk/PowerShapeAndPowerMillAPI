// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using System.Text;
using Autodesk.FileSystem;
using NUnit.Framework;

namespace Utilities.Test
{
    /// <summary>
    /// This is a test class for BinaryFileWriterTest and is intended
    /// to contain all BinaryFileWriterTest Unit Tests
    /// </summary>
    [TestFixture]
    public class BinaryFileWriterTest
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
            TestDirectory = Environment.GetEnvironmentVariable("TEMP") + @"\delcamTmp\BinaryFileWriterTests";
            System.IO.Directory.CreateDirectory(TestDirectory);
        }

        [TearDown]
        protected void DeleteTestDirectory()
        {
            if (System.IO.Directory.Exists(TestDirectory))
            {
                try
                {
                    string s = Environment.GetEnvironmentVariable("TEMP") + @"\delcamTmp\BinaryFileWriterTests";
                    System.IO.Directory.Delete(s, true);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Print(ex.Message);
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
        /// A test for WriteByte
        /// </summary>
        [Test]
        public void WriteByteTest()
        {
            File file = new File(System.IO.Path.Combine(TestDirectory, "Test3.Txt"));
            try
            {
                BinaryFileWriter target =
                    new BinaryFileWriter(file, Encoding.UTF32);

                byte b = 10;
                target.WriteByte(b);
                target.Close();

                BinaryFileReader r = new BinaryFileReader(file);

                byte c = r.ReadByte();

                Assert.AreEqual(b, c, "Should be equal");
                r.Close();
            }
            catch (Exception e)
            {
                Assert.Fail(e.ToString());
            }
        }

        /// <summary>
        /// A test for BinaryFileWriter Constructor
        /// </summary>
        [Test]
        public void BinaryFileWriterConstructorTest1()
        {
            File file = new File(System.IO.Path.Combine(TestDirectory, "Test2.Txt"));
            try
            {
                BinaryFileWriter target =
                    new BinaryFileWriter(file);

                byte b = 10;
                target.WriteByte(b);
                target.Close();
            }
            catch (Exception e)
            {
                Assert.Fail(e.ToString());
            }
        }

        /// <summary>
        /// A test for BinaryFileWriter Constructor
        /// </summary>
        [Test]
        public void BinaryFileWriterConstructorTest()
        {
            File file = new File(System.IO.Path.Combine(TestDirectory, "Test1.Txt"));
            try
            {
                BinaryFileWriter target =
                    new BinaryFileWriter(file, Encoding.UTF32);

                byte b = 10;
                target.WriteByte(b);
                target.Close();

                BinaryFileReader r = new BinaryFileReader(file);

                byte c = r.ReadByte();

                Assert.AreEqual(b, c, "Should be equal");
            }
            catch (Exception e)
            {
                Assert.Fail(e.ToString());
            }
        }
    }
}