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
using Autodesk.FileSystem;
using NUnit.Framework;

namespace Utilities.Test
{
    /// <summary>
    /// This is a test class for DirectoryTest and is intended
    /// to contain all DirectoryTest Unit Tests
    /// </summary>
    [TestFixture]
    public class DirectoryTest
    {
        private string thisTestDirectory;

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
            TestDirectory = Environment.GetEnvironmentVariable("TEMP") + @"\delcamTmp\DelcamDirectoryTest";
            System.IO.Directory.CreateDirectory(TestDirectory);
        }

        [TearDown]
        protected void DeleteTestDirectory()
        {
            if (System.IO.Directory.Exists(TestDirectory))
            {
                try
                {
                    System.IO.Directory.Delete(Environment.GetEnvironmentVariable("TEMP") + @"\delcamTmp\DelcamDirectoryTest",
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

        [Test]
        public void ParentDirectoryTest()
        {
            string path = @"c:\testdir";
            Directory target = new Directory(path);

            Directory parent = target.ParentDirectory;

            Assert.AreEqual(@"c:\", parent.Path, "expected directory incorrect");

            Directory again = parent.ParentDirectory;

            if (again != null)
            {
                Assert.Fail("expecting null result");
            }

            path = "oiuytre";

            target = new Directory(path);
            again = target.ParentDirectory;

            if (again != null)
            {
                Assert.Fail("expecting null result");
            }
        }

        /// <summary>
        /// A test for Path
        /// </summary>
        [Test]
        public void PathTest()
        {
            string path = @"c:\testdir"; //made up dir
            Directory target = new Directory(path);
            string actual;
            actual = target.Path;
            Assert.AreEqual(path + System.IO.Path.DirectorySeparatorChar, actual, "Directory path not equal");

            Assert.AreNotEqual(@"c:\differentDir", actual, "Directories should not be equal");
        }

        /// <summary>
        /// A test for Name
        /// </summary>
        [Test]
        public void NameTest()
        {
            string dirName = "testdir";
            string path = System.IO.Path.Combine(@"c:\", dirName);
            Directory target = new Directory(path);
            string actual;
            actual = target.Name;
            Assert.AreEqual(dirName, actual, "Directory Name not correct");

            // now try deeper directory

            path = System.IO.Path.Combine(@"c:\testFolder\testFolder2\", dirName);
            target = new Directory(path);
            actual = target.Name;
            Assert.AreEqual(dirName, actual, "Directory Name not correct");
        }

        /// <summary>
        /// A test for Files
        /// </summary>
        [Test]
        public void FilesTest()
        {
            string dirtmp = "dir1";
            string path = System.IO.Path.Combine(TestDirectory, dirtmp);
            createRandomFiles(path, 20);

            Directory target = new Directory(path);
            List<File> actual;
            actual = target.Files;

            string[] fnames = System.IO.Directory.GetFiles(path);

            Assert.IsTrue(fnames.Length == actual.Count, "File count not equal");

            // check list of files is present in dir
            foreach (File df in actual)
            {
                int iff = 0;
                for (int i = 0; i < fnames.Length; i++)
                {
                    if (df.Path == fnames[i])
                    {
                        iff = 1;
                        break;
                    }
                }
                Assert.IsTrue(iff == 1, "All files are not present");
            }
        }

        /// <summary>
        /// A test for Exists
        /// </summary>
        [Test]
        public void ExistsTest()
        {
            string path = TestDirectory;
            Directory target = new Directory(path);
            bool actual;
            actual = target.Exists;
            Assert.IsTrue(actual, "Folder should exist");

            // now prove folder does not exist
            path = System.IO.Path.Combine(TestDirectory, "doesNotExist");
            if (System.IO.Directory.Exists(path))
            {
                System.IO.Directory.Delete(path);
            }

            target = new Directory(path);
            actual = target.Exists;
            Assert.IsFalse(actual, "Folder should not exist");

            path = System.IO.Path.Combine(TestDirectory, "existdir");
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }

            target = new Directory(path);

            target.Delete();

            Assert.IsFalse(target.Exists, "Should be false");
        }

        /// <summary>
        /// A test for Empty
        /// </summary>
        [Test]
        public void EmptyTest()
        {
            string dirName = @"TestDirectory";

            string path = System.IO.Path.Combine(TestDirectory, dirName);

            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }

            // dir should be empty
            Directory target = new Directory(path);
            bool actual;
            actual = target.IsEmpty;
            Assert.IsTrue(actual, "Folder should be empty");

            // now check what happens if folder is not empty.
            string tmpFile = System.IO.Path.Combine(path, "test.txt");
            File t = new File(tmpFile);
            t.Create(); // assuming this works.
            Assert.IsTrue(System.IO.File.Exists(tmpFile), "File should exist");

            actual = target.IsEmpty;
            Assert.IsFalse(actual, "Folder should not be empty");
        }

        /// <summary>
        /// A test for Directories
        /// </summary>
        [Test]
        public void DirectoriesTest()
        {
            string path = System.IO.Path.Combine(TestDirectory, "TestDirectoryA");
            Directory target = new Directory(path);

            // check that list should be empty

            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }

            List<Directory> actual;
            actual = target.Directories;

            Assert.IsTrue(actual.Count == 0 ? true : false, " Directory should be empty");

            // create a list of random directories
            int i = 0;
            List<Directory> tmpL = new List<Directory>();
            string tmpF;
            while (i < 10)
            {
                tmpF = System.IO.Path.Combine(path, System.IO.Path.GetRandomFileName());

                if (!System.IO.Directory.Exists(tmpF))
                {
                    System.IO.Directory.CreateDirectory(tmpF);
                }

                tmpL.Add(new Directory(tmpF));
                i++;
            }

            // now check list contents
            actual.Clear();
            actual = target.Directories;

            Assert.AreEqual(tmpL.Count, actual.Count, "Directory Count should be equal");

            //now check list names are equal
            for (int j = 0; j < i; j++)
            {
                // since it will not be in order check all
                int iff = 0;
                foreach (Directory d in actual)
                {
                    if (tmpL[j].Path == d.Path)
                    {
                        iff = 1;
                        break;
                    }
                }

                Assert.IsTrue(iff == 1, "Directory path not found");
            }
        }

        /// <summary>
        /// A test for Move
        /// </summary>
        [Test]
        public void MoveTest()
        {
            string path = System.IO.Path.Combine(TestDirectory, "TestDirectoryA");
            string newPath = System.IO.Path.Combine(TestDirectory, "TestDirectoryB");
            string newPath2 = System.IO.Path.Combine(TestDirectory, "TestDirectoryC");

            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }

            if (!System.IO.Directory.Exists(newPath2))
            {
                System.IO.Directory.CreateDirectory(newPath2);
            }

            for (int i = 0; i < 10; i++)
            {
                string fname = System.IO.Path.Combine(path, System.IO.Path.GetRandomFileName());

                File tmpF = new File(fname);
                tmpF.Create();
            }

            Directory target = new Directory(path);
            if (System.IO.Directory.Exists(newPath))
            {
                System.IO.Directory.Delete(newPath, true);
            }
            Directory newParentDirectory = new Directory(newPath);

            target.Move(newParentDirectory); // 
            Assert.AreEqual(target.Path, newPath + System.IO.Path.DirectorySeparatorChar, "Move directory path not correct");
        }

        /// <summary>
        /// A test for Delete
        /// </summary>
        [Test]
        public void DeleteTest()
        {
            string path = System.IO.Path.Combine(TestDirectory, "TestDirectoryA");
            Directory target = new Directory(path);

            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }

            // populate with random files
            for (int i = 0; i < 10; i++)
            {
                string fn = System.IO.Path.Combine(path, System.IO.Path.GetRandomFileName());
                File f = new File(fn);
                f.Create();
            }

            // now delete files
            target.Delete();

            Assert.IsFalse(System.IO.Directory.Exists(path), "Directory shouldn't exist");
        }

        /// <summary>
        /// A test for Create
        /// </summary>
        [Test]
        public void CreateTest()
        {
            string path = System.IO.Path.Combine(TestDirectory, "TestDirectoryA");
            Directory target = new Directory(path);
            if (System.IO.Directory.Exists(path))
            {
                System.IO.Directory.Delete(path, true);
            }
            target.Create();
            Assert.IsTrue(System.IO.Directory.Exists(target.Path), "Directory should exist");
        }

        [Test]
        public void CreateTempFolderTest()
        {
            Directory tmpFolder = Directory.CreateTemporaryDirectory();

            Assert.AreEqual(tmpFolder.Name.Length, 8, "expected directory name format TMP00000");
            Assert.AreEqual(tmpFolder.Exists, false, "Tmp folder should not exist");

            tmpFolder = Directory.CreateTemporaryDirectory(true);
            Assert.AreEqual(tmpFolder.Exists, true, "Tmp folder should exist");
            tmpFolder.Delete();
        }

        /// <summary>
        /// Create a number of random files
        /// </summary>
        /// <param name="dirName"></param>
        public void createRandomFiles(string dirName, int nFiles)
        {
            string path;

            if (System.IO.Path.IsPathRooted(dirName))
            {
                path = dirName;
            }
            else
            {
                path = System.IO.Path.Combine(TestDirectory, dirName);
            }

            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            for (int i = 0; i < nFiles; i++)
            {
                string fname = System.IO.Path.Combine(path, System.IO.Path.GetRandomFileName());
                File f = new File(fname);
                f.Create();
            }
        }

        /// <summary>
        /// A test for CopyAll
        /// </summary>
        [Test]
        public void CopyTest()
        {
            string oldDirName = "TestAbc";
            Directory sourceDir = new Directory(new Directory(TestDirectory), oldDirName);

            string newDirName = "TestDEF";
            Directory targetDir = new Directory(new Directory(TestDirectory), newDirName);

            createRandomFiles(sourceDir.Path, 10);

            sourceDir.Copy(targetDir);

            List<File> sourceFiles = sourceDir.Files;
            List<File> targetFiles = targetDir.Files;

            Assert.IsTrue(sourceFiles.Count == targetFiles.Count, "File number should be equal");

            for (int i = 0; i < sourceFiles.Count; i++)
            {
                if (sourceFiles[i].Name != targetFiles[i].Name)
                {
                    Assert.Fail("Files should be identical");
                }

                Assert.IsTrue(targetFiles[i].Exists, "File should exist");
            }
        }

        /// <summary>
        /// A test for CopyAll
        /// </summary>
        [Test]
        public void CopyToNewParentTest()
        {
            string sourceDirectoryName = "TestAbc";
            Directory sourceDirectory = new Directory(new Directory(TestDirectory), sourceDirectoryName);
            createRandomFiles(sourceDirectory.Path, 10);

            string newParentDirectoryName = "TestDEF";
            Directory newParentDirectory = new Directory(new Directory(TestDirectory), newParentDirectoryName);
            if (newParentDirectory.Exists)
            {
                newParentDirectory.Delete();
            }

            sourceDirectory.CopyToNewParent(newParentDirectory);

            List<File> filesInSource = sourceDirectory.Files;
            List<File> filesInDestination = new Directory(newParentDirectory, sourceDirectoryName).Files;

            Assert.IsTrue(filesInSource.Count == filesInDestination.Count, "File number should be equal");

            for (int i = 0; i < filesInSource.Count; i++)
            {
                if (filesInSource[i].Name != filesInDestination[i].Name)
                {
                    Assert.Fail("Files should be identical");
                }

                Assert.IsTrue(filesInDestination[i].Exists, "File should exist");
            }
        }

        /// <summary>
        /// A test for Directory Constructor
        /// </summary>
        [Test]
        public void DirectoryConstructorTest1()
        {
            string path = TestDirectory;
            Directory target = new Directory(path);
            Assert.AreEqual(path + System.IO.Path.DirectorySeparatorChar, target.Path, "Pathnames not equal");
        }

        /// <summary>
        /// A test for Directory Constructor
        /// </summary>
        [Test]
        public void DirectoryConstructorTest()
        {
            string path = TestDirectory;
            Directory parentDirtory = new Directory(path);
            string directoryName = "Test123";
            Directory target = new Directory(parentDirtory, directoryName);

            string fullpath = System.IO.Path.Combine(path, directoryName) + System.IO.Path.DirectorySeparatorChar;

            Assert.AreEqual(fullpath, target.Path, "Pathname not equal");
        }
    }
}