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
    /// This is a test class for DirectoryWatcherTest and is intended
    /// to contain all DirectoryWatcherTest Unit Tests
    /// </summary>
    [TestFixture]
    public class DirectoryWatcherTest
    {
        private AutoResetEvent _TestTrigger;
        private bool bHandlerRaised;

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
            TestDirectory = Environment.GetEnvironmentVariable("TEMP") + @"\delcamTmp\DirectoryWatcherTests";
            System.IO.Directory.CreateDirectory(TestDirectory);
        }

        [TearDown]
        protected void DeleteTestDirectory()
        {
            if (System.IO.Directory.Exists(TestDirectory))
            {
                try
                {
                    System.IO.Directory.Delete(Environment.GetEnvironmentVariable("TEMP") + @"\delcamTmp\DirectoryWatcherTests",
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

        /// <summary>
        /// A test for EnableRaisingEvents
        /// </summary>
        [Test]
        public void EnableRaisingEventsTest()
        {
            string dirName = "testDir";
            Directory directoryToWatch = fact_createTestDirectory(dirName);
            DirectoryWatcher target = new DirectoryWatcher(directoryToWatch);

            bHandlerRaised = false;
            _TestTrigger = new AutoResetEvent(false);

            File df = new File(directoryToWatch, "test1.txt");
            target.FileCreated += dw_FileCreated;
            target.EnableRaisingEvents = false;
            df.Create();
            _TestTrigger.WaitOne(3000); // should time out
            Assert.IsFalse(bHandlerRaised, "Event should not be raised");

            //now prove that event is raised by recreating file.
            bHandlerRaised = false;
            GC.WaitForPendingFinalizers();
            System.IO.File.Delete(df.Path);
            target.EnableRaisingEvents = true;
            df.Create();
            _TestTrigger.WaitOne(3000); // should expect event
            Assert.IsTrue(bHandlerRaised, "Event should be raised");

            target.FileCreated -= dw_FileCreated;
            _TestTrigger = null;
        }

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
            string dirName = "DelDirTest2";
            Directory mydir = fact_createTestDirectory(dirName);
            string dirPath = mydir.Path;
            DirectoryWatcher dw;

            bHandlerRaised = false;
            try
            {
                File df = new File(mydir, "DelFile.txt");
                df.Create();
                dw = new DirectoryWatcher(mydir, "delFile.txt", false);
                dw.EnableRaisingEvents = false;
                dw.FileDeleted += dw_FileDeleted;
                dw.EnableRaisingEvents = true;
                _TestTrigger = new AutoResetEvent(false);
                GC.WaitForPendingFinalizers();
                df.Delete(); // should raise trigger
                _TestTrigger.WaitOne(5000);

                // test for del
                Assert.IsTrue(bHandlerRaised, "Handler not raised");

                bHandlerRaised = false;

                //Test that delete of different file type should not be raised.
                File df2 = new File(mydir, "DelFile.log");
                df2.Create();
                df2.Delete();
                _TestTrigger.WaitOne(2000);
                Assert.IsFalse(bHandlerRaised, "No Event should be raised");

                //now check what happens if directory is deleted                
                dw.DirectoryUnavailableTimerStatus = true;
                Assert.IsTrue(dw.DirectoryUnavailableTimerStatus, "Timer should be enabled");
                bHandlerRaised = false;

//                dw.FileDeleted -= dw_FileDeleted; // remove event handler
                dw.DirectoryUnavailable +=
                    dw_DirectoryUnavailable;
                dw.DirectoryUnavailableTimerStatus = true;
                Assert.IsTrue(dw.DirectoryUnavailableTimerStatus, "Timer should be enabled");
                dw.EnableRaisingEvents = false; //stop other events firing
                mydir.Delete();
                _TestTrigger.WaitOne(8000);
                Assert.IsTrue(bHandlerRaised, "Del Dir Event not raised");
                Assert.IsFalse(System.IO.Directory.Exists(mydir.Path), "Directory should not exist");

                dw.DirectoryUnavailable -= dw_DirectoryUnavailable;

                dw = null;
            }
            catch (Exception e)
            {
                Assert.Fail("Delete File in dir test failed e={0}", e);
            }
            finally
            {
                dw = null;
                _TestTrigger = null;
            }
        }

        void dw_DirectoryUnavailable(object sender, WatcherEventArgs e)
        {
            bHandlerRaised = true;
            _TestTrigger.Set();
        }

        void dw_FileDeleted(object sender, System.IO.FileSystemEventArgs e)
        {
            bHandlerRaised = true;
            _TestTrigger.Set();
        }

        /// <summary>
        /// A test for _watcher_Created
        /// </summary>
        [Test]
        public void _watcher_CreatedTest()
        {
            string dirName = "DelDirTest";
            Directory mydir = fact_createTestDirectory(dirName);
            string dirPath = mydir.Path;
            _TestTrigger = new AutoResetEvent(false);
            bHandlerRaised = false;

            DirectoryWatcher dw = new DirectoryWatcher(mydir);
            dw.FileCreated += dw_FileCreated;
            dw.EnableRaisingEvents = true;
            Directory myd2 = new Directory(mydir, "subDir");
            myd2.Create();
            _TestTrigger.WaitOne(2000);

            Assert.IsTrue(bHandlerRaised, "Dir Create Event not raised");
            bHandlerRaised = false;

            //unassign event handlers for other tests.
            dw.FileCreated -= dw_FileCreated;
            dw = null;
        }

        void dw_FileCreated(object sender, System.IO.FileSystemEventArgs e)
        {
            bHandlerRaised = true;

            _TestTrigger.Set();
        }

        /// <summary>
        /// A test for DirectoryWatcher Constructor
        /// </summary>
        [Test]
        public void DirectoryWatcherConstructorTest1()
        {
            try
            {
                string dirPath = System.IO.Path.Combine(TestDirectory, "dir2");

                Directory directoryToWatch = new Directory(dirPath);
                if (!directoryToWatch.Exists)
                {
                    directoryToWatch.Create();
                }
                DirectoryWatcher target = new DirectoryWatcher(directoryToWatch);

                target = null;
            }
            catch (Exception e)
            {
                Assert.Fail("Constructor Failed e{0}", e);
            }
        }

        /// <summary>
        /// A test for DirectoryWatcher Constructor
        /// </summary>
        [Test]
        public void DirectoryWatcherConstructorTest()
        {
            try
            {
                string dirPath = System.IO.Path.Combine(TestDirectory, "dir1");

                Directory directoryToWatch = new Directory(dirPath);
                if (!directoryToWatch.Exists)
                {
                    directoryToWatch.Create();
                }

                DirectoryWatcher target = new DirectoryWatcher(directoryToWatch, "*.txt", true);

                target = null;
            }
            catch (Exception e)
            {
                Assert.Fail("Constructor Failed e{0}", e);
            }
        }
    }
}