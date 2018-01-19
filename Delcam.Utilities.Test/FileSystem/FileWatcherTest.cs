// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using System.Threading;
using Autodesk.FileSystem;
using NUnit.Framework;

namespace Utilities.Test
{
    /// <summary>
    /// This is a test class for FileWatcherTest and is intended
    /// to contain all FileWatcherTest Unit Tests
    /// </summary>
    [TestFixture]
    public class FileWatcherTest
    {
        private AutoResetEvent _TestTrigger;
        private bool bHandlerRaised;
        private string tmpFileName;

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
            TestDirectory = Environment.GetEnvironmentVariable("TEMP") + @"\delcamTmp\FileWatcherTests";
            System.IO.Directory.CreateDirectory(TestDirectory);
        }

        [TearDown]
        protected void DeleteTestDirectory()
        {
            if (System.IO.Directory.Exists(TestDirectory))
            {
                try
                {
                    System.IO.Directory.Delete(Environment.GetEnvironmentVariable("TEMP") + @"\delcamTmp\BinaryFileWriterTests",
                                               true);
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

        private Directory fact_createTestDirectory(string dirName)
        {
            string dirPath = System.IO.Path.Combine(TestDirectory, dirName);

            if (System.IO.Directory.Exists(dirPath))
            {
                System.IO.Directory.Delete(dirPath, true);
            }
            System.IO.Directory.CreateDirectory(dirPath);

            Directory myDir = new Directory(dirPath);

            return myDir;
        }

        /// <summary>
        /// A test for _watcher_Deleted
        /// </summary>
        [Test]
        public void _watcher_DeletedTest()
        {
            string dirName = "TestDirectory4";
            Directory d = fact_createTestDirectory(dirName);
            string fname = System.IO.Path.Combine(d.Path, "Test1.txt");
            File df = new File(fname);
            df.Create();

            FileWatcher fw = new FileWatcher(fname);

            // add deleted event and test if fires when file is deleted.
            _TestTrigger = new AutoResetEvent(false);
            bHandlerRaised = false;
            tmpFileName = "";
            fw.FileDeleted += fw_FileDeleted;

            df.Delete();
            _TestTrigger.WaitOne(2000);

            Assert.IsTrue(bHandlerRaised, "File Deleted Event not raised");
            Assert.AreEqual(fname, tmpFileName, "Wrong File Delete Event raised");

            // now prove event is not raised if file is deleted
            df.Create();

            fw.FileDeleted -= fw_FileDeleted; // remove event
            _TestTrigger = null;
            _TestTrigger = new AutoResetEvent(false);
            bHandlerRaised = false;
            GC.WaitForPendingFinalizers();
            df.Delete();
            _TestTrigger.WaitOne(2000); // should time out

            Assert.IsFalse(bHandlerRaised, "File Deleted Event should not raised");
            _TestTrigger = null;

            fw = null;
        }

        void fw_FileDeleted(object sender, System.IO.FileSystemEventArgs e)
        {
            bHandlerRaised = true;
            tmpFileName = e.FullPath;
            _TestTrigger.Set(); // clear pause on main thread.
        }

        /// <summary>
        /// A test for _watcher_Created
        /// </summary>
        [Test]
        public void _watcher_CreatedTest()
        {
            string dirName = "TestDirectory";
            Directory d = fact_createTestDirectory(dirName);
            string fname = System.IO.Path.Combine(d.Path, "Test2.txt");
            File df = new File(fname);

            FileWatcher fw = new FileWatcher(fname);

            // add created event and test if fires when file is created.
            _TestTrigger = new AutoResetEvent(false);
            bHandlerRaised = false;
            tmpFileName = "";
            fw.FileCreated += fw_FileCreated;

            df.Create(); // create file
            _TestTrigger.WaitOne(2000);
            Assert.IsTrue(bHandlerRaised, "File Created Event not raised");
            Assert.AreEqual(fname, tmpFileName, "Wrong File Create Event raised");

            //now check that event not passed on
            df.Delete();
            fw.FileCreated -= fw_FileCreated;

            bHandlerRaised = false;
            tmpFileName = "";
            df.Create(); // create file
            _TestTrigger.WaitOne(2000); // should time out
            Assert.IsFalse(bHandlerRaised, "File Created Event should not raised");

            fw = null;
        }

        void fw_FileCreated(object sender, System.IO.FileSystemEventArgs e)
        {
            bHandlerRaised = true;
            tmpFileName = e.FullPath;
            _TestTrigger.Set(); // clear pause on main thread.
        }

        /// <summary>
        /// A test for _watcher_Changed
        /// </summary>
        [Test]
        public void _watcher_ChangedTest()
        {
            string dirName = "TestDirectory3";
            Directory d = fact_createTestDirectory(dirName);
            string fname = System.IO.Path.Combine(d.Path, "Test2.txt");

            File df = new File(fname);

            df.Create();

            FileWatcher fw = new FileWatcher(fname);

            // add created event and test if fires when file is created.
            _TestTrigger = new AutoResetEvent(false);
            bHandlerRaised = false;
            tmpFileName = "";
            fw.FileChanged += fw_FileChanged;

            df.WriteTextLine("Changing text file", true, System.Text.Encoding.UTF8);

            _TestTrigger.WaitOne(2000);
            Assert.IsTrue(bHandlerRaised, "File Changed Event not raised");
            Assert.AreEqual(fname, tmpFileName, "Wrong File Changed Event raised");

            //now check that event not passed on
            df.Delete();
            fw.FileChanged -= fw_FileChanged;

            bHandlerRaised = false;
            tmpFileName = "";
            df.WriteTextLine("Changing text file again", true, System.Text.Encoding.UTF8);

            _TestTrigger.WaitOne(2000); // should time out
            Assert.IsFalse(bHandlerRaised, "File Created Event should not raised");

            fw = null;
        }

        void fw_FileChanged(object sender, System.IO.FileSystemEventArgs e)
        {
            bHandlerRaised = true;
            tmpFileName = e.FullPath;
            _TestTrigger.Set(); // clear pause on main thread.
        }

        /// <summary>
        /// A test for FileWatcher Constructor
        /// </summary>
        [Test]
        public void FileWatcherConstructorTest1()
        {
            string dirName = "TestDirectory2";
            Directory d = fact_createTestDirectory(dirName);
            string fname = System.IO.Path.Combine(d.Path, "Test2.txt");
            File df = new File(fname);

            FileWatcher fw = null;
            try
            {
                fw = new FileWatcher(df);
            }
            catch (Exception e)
            {
                Assert.Fail("Constructor Failed e={0}", e);
            }
            finally
            {
                if (fw != null)
                {
                    fw = null;
                }
            }
        }

        /// <summary>
        /// A test for FileWatcher Constructor
        /// </summary>
        [Test]
        public void FileWatcherConstructorTest()
        {
            string dirName = "TestDirectory1";
            Directory d = fact_createTestDirectory(dirName);
            string fname = System.IO.Path.Combine(d.Path, "Test2.txt");
            File df = new File(fname);

            FileWatcher fw = null;
            try
            {
                fw = new FileWatcher(fname);
            }
            catch (Exception e)
            {
                Assert.Fail("Constructor Failed e={0}", e);
            }
            finally
            {
                if (fw != null)
                {
                    fw = null;
                }
            }
        }
    }
}