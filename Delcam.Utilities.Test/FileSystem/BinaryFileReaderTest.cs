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
    /// This is a test class for BinaryFileReaderTest and is intended
    /// to contain all BinaryFileReaderTest Unit Tests
    /// </summary>
    [TestFixture]
    public class BinaryFileReaderTest
    {
        private string _thisTestDirectory = Environment.GetEnvironmentVariable("TEMP") + @"\tmpTestDirectory";

        //private string tmpDir = ".\testDelCamDir";
        //couldn't use test instance test directory - instead hard coded temporary directory
        /// <summary>
        /// Gets or sets the test context test directory.
        /// </summary>
        public string TestDirectory
        {
            get { return _thisTestDirectory; }
            set { _thisTestDirectory = value; }
        }

        [SetUp]
        protected void SetTestDirectory()
        {
            TestDirectory = Environment.GetEnvironmentVariable("TEMP") + @"\delcamTmp\BinaryFileReaderTests";
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

        /// <summary>
        /// A test for ReadByte
        /// </summary>
        [Test]
        public void ReadByteTest()
        {
            File file =
                new File(System.IO.Path.Combine(TestDirectory, "Test1.bin"));
            try
            {
                BinaryFileWriter target =
                    new BinaryFileWriter(file, Encoding.UTF32);

                byte b = 10;
                target.WriteByte(b);
                target.Close();

                BinaryFileReader r =
                    new BinaryFileReader(file);

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
        /// A test for ReadStringUntil
        /// </summary>
        [Test]
        public void ReadStringUntilTest()
        {
            File file =
                new File(System.IO.Path.Combine(TestDirectory, "Test1"));
            try
            {
                BinaryFileWriter target =
                    new BinaryFileWriter(file);

                string actual = "abcdefghijk;";

                char ch = ';';
                byte terminator = (byte) ch;

                int maximumLength = actual.Length;

                //target.writer.Write(actual);

                target.WriteString(actual);

                target.Close();

                BinaryFileReader r =
                    new BinaryFileReader(file);

                string expected = r.ReadStringUntil(terminator, maximumLength, true);

                r.Close();
                Assert.AreEqual(expected, actual, "Should be equal");
            }
            catch (Exception e)
            {
                Assert.Fail(e.ToString());
            }
        }

        /// <summary>
        /// A test for BinaryFileReader Constructor
        /// </summary>
        [Test]
        public void BinaryFileReaderConstructorTest()
        {
            File file =
                new File(System.IO.Path.Combine(TestDirectory, "Test1.Txt"));
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
    }
}