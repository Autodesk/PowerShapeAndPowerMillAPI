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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Directory = Autodesk.FileSystem.Directory;
using File = Autodesk.FileSystem.File;

namespace Autodesk.Utilities.Test.FileSystem
{
    /// <summary>
    /// This is a test class for FileTest and is intended
    /// to contain all FileTest Unit Tests
    /// </summary>
    [TestFixture]
    public class FileTest
    {
        private readonly string _testDirectory = Environment.GetEnvironmentVariable("TEMP") + @"\delcamTmp\DelcamFileTests";

        [SetUp]
        protected void SetUp()
        {
            System.IO.Directory.CreateDirectory(_testDirectory);
        }

        [TearDown]
        protected void TearDown()
        {
            if (System.IO.Directory.Exists(_testDirectory))
            {
                try
                {
                    System.IO.Directory.Delete(_testDirectory, true);
                }
                catch
                {
                }
            }
        }

        private File CreateFileInTempDirectory()
        {
            string filepath = Path.Combine(_testDirectory, Path.GetFileName(Path.GetTempFileName()));
            while (System.IO.File.Exists(filepath))
                filepath = Path.Combine(_testDirectory, Path.GetFileName(Path.GetTempFileName()));

            return new File(filepath);
        }

        #region Test properties

        /// <summary>
        /// A test for Exists
        /// </summary>
        [Test]
        public void ExistsTest()
        {
            string fname = "test123.txt";
            string filePath = Path.Combine(_testDirectory, fname);

            File target = new File(filePath);

            System.IO.File.Delete(filePath);
            Assert.IsFalse(target.Exists, "File should not exist");

            //now create file
            System.IO.File.WriteAllText(filePath, "Test");
            Assert.IsTrue(target.Exists, "File should exist");
        }

        /// <summary>
        /// A test for ParentDirectory
        /// </summary>
        [Test]
        public void ParentDirectoryTest()
        {
            File target = new File(@"c:\abcdefg\test1.txt");
            Assert.AreEqual(@"c:\abcdefg\", target.ParentDirectory.Path, "Parent directory of file is wrong.");

            target = new File(@"c:\test2.txt");
            Assert.AreEqual(@"c:\", target.ParentDirectory.Path, "Parent directory of file is wrong.");

            target = new File(@"c:\dir1\dir2\test2.txt");
            Assert.AreEqual(@"c:\dir1\dir2\", target.ParentDirectory.Path, "Parent directory of file is wrong.");
        }

        /// <summary>
        /// A test for Path
        /// </summary>
        [Test]
        public void PathTest()
        {
            string filePath = Path.Combine(_testDirectory, "TestABC.txt");
            File target = new File(Path.Combine(_testDirectory, "TestABC.txt"));
            Assert.AreEqual(filePath, target.Path, "Filepath is incorrect.");
        }

        /// <summary>
        /// A test for NameWithoutExtension
        /// </summary>
        [Test]
        public void NameWithoutExtensionTest()
        {
            File file = new File(Path.Combine(_testDirectory, "TestFile.txt"));
            Assert.AreEqual("TestFile", file.NameWithoutExtension, "Short name should be 'TestFile'");

            file = new File(Path.Combine(_testDirectory, "TestFile2"));
            Assert.AreEqual("TestFile2", file.NameWithoutExtension, "Short name should be 'TestFile2'");

            file = new File(Path.Combine(_testDirectory, "TestFile3.txt.tmp"));
            Assert.AreEqual("TestFile3.txt", file.NameWithoutExtension, "Short name should be 'TestFile3.txt'");
        }

        /// <summary>
        /// A test for Extension
        /// </summary>
        [Test]
        public void ExtensionTest()
        {
            File target = new File(Path.Combine(_testDirectory, "test123.txt"));
            Assert.AreEqual("txt", target.Extension, "File extension should be txt.");

            target = new File(Path.Combine(_testDirectory, "test123"));
            Assert.AreEqual("", target.Extension, "File should have no extension.");
        }

        /// <summary>
        /// A test for LastChangedDate
        /// </summary>
        [Test]
        public void LastChangedDateTest()
        {
            File file = new File(Path.Combine(_testDirectory, "TestLastChangedDate.Txt"));
            file.WriteText("testing");
            DateTime time1 = new FileInfo(file.Path).LastWriteTime;

            Thread.Sleep(100);
            file.WriteText("more text");
            DateTime time2 = new FileInfo(file.Path).LastWriteTime;

            Assert.AreNotEqual(time1, time2, "File modified datetimes should be different.");
        }

        #endregion

        #region Test text writing

        /// <summary>
        /// A test for WriteText
        /// </summary>
        [Test]
        public void WriteTextTest2()
        {
            string fname = "test123.txt";
            string filePath = Path.Combine(_testDirectory, fname);
            File target = new File(filePath);

            string line1 = "This is Line 1";
            System.IO.File.Delete(filePath);

            target.WriteText(line1, false, Encoding.UTF8);

            string[] fileLines = System.IO.File.ReadAllLines(filePath);

            if (fileLines == null)
            {
                Assert.Fail("Could not retrieve text file lines");
            }
            else if (fileLines.Length != 1)
            {
                Assert.Fail("Expecting one line");
            }
            else if (fileLines[0] != line1)
            {
                Assert.Fail("Read 1st line not equal to expected line");
            }
            else
            {
                // ok now append line
                string[] lines = new string[2];

                string line2 = "This is second line";
                target.WriteText(line2, true, Encoding.UTF8); // should put this on 1st line/

                fileLines = System.IO.File.ReadAllLines(filePath);

                lines[0] = line1;
                lines[1] = line2;

                //int i = 0;
                Assert.IsNotNull(fileLines, "Could not retrieve file lines");
                Assert.AreEqual(1, fileLines.Length, "Should only be 1 line");

                string linecomb = line1 + line2;
                Assert.AreEqual(linecomb, fileLines[0], "Should be one 1 line");
                /*
                foreach(string s in fileLines)
                {

                    Assert.AreEqual(lines[i],s,string.Format("Line {0} not equal",i));
                    i++;
                }
                 * */
            }
        }

        /// <summary>
        /// A test for WriteText
        /// </summary>
        [Test]
        public void WriteTextTest1()
        {
            string fname = "test123.txt";
            string filePath = Path.Combine(_testDirectory, fname);
            File target = new File(filePath);

            string line1 = "This is Line 1";
            System.IO.File.Delete(filePath);

            target.WriteText(line1);

            string linein = System.IO.File.ReadAllText(filePath);

            Assert.AreEqual(line1, linein, "Lines should be equal");

            string line2 = "This is line 2";
            target.WriteText(line2); // should not append

            linein = System.IO.File.ReadAllText(filePath);

            Assert.AreEqual(line2, linein, "Lines should be equal");
        }

        /// <summary>
        /// A test for WriteText
        /// </summary>
        [Test]
        public void WriteTextTest()
        {
            string fname = "test123.txt";
            string filePath = Path.Combine(_testDirectory, fname);
            File target = new File(filePath);

            string line1 = "This is Line 1";
            System.IO.File.Delete(filePath);

            bool appendText = false;
            target.WriteText(line1, appendText);

            target.WriteText(line1, true);

            string combLine = line1 + line1;

            string linein = System.IO.File.ReadAllText(filePath);
            Assert.AreEqual(combLine, linein, "Lines should be equal");
        }

        #endregion

        #region Test text reading

        /// <summary>
        /// A test for ReadTextLines
        /// </summary>
        [Test]
        public void ReadTextLinesTest1()
        {
            string fname = "test123.txt";
            string filePath = Path.Combine(_testDirectory, fname);
            File target = new File(filePath);

            string[] stlines = {"This is line1", "This is line2", "This is line3"};

            System.IO.File.WriteAllLines(filePath, stlines);

            List<string> expected = new List<string>();
            expected.AddRange(stlines);

            List<string> actual;
            actual = target.ReadTextLines();

            if (expected.Count != actual.Count)
            {
                Assert.Fail("No of Lines not equal");
            }

            int i = 0;
            foreach (string l in expected)
            {
                if (l != actual[i++])
                {
                    Assert.Fail("String line not equal");
                }
            }

            //Assert.AreEqual(expected, actual,"Text Lines should be equal");
        }

        /// <summary>
        /// A test for ReadTextLines
        /// </summary>
        [Test]
        public void ReadTextLinesTest()
        {
            string fname = "test123.txt";
            string filePath = Path.Combine(_testDirectory, fname);
            File target = new File(filePath);

            string[] stlines = {"This is line1", "This is line2", "This is line3"};
            Encoding encoding = Encoding.UTF32;
            System.IO.File.WriteAllLines(filePath, stlines, encoding);

            List<string> expected = new List<string>();
            expected.AddRange(stlines);

            List<string> actual;
            actual = target.ReadTextLines(encoding);

            if (expected.Count != actual.Count)
            {
                Assert.Fail("No of Lines not equal");
            }

            int i = 0;
            foreach (string l in expected)
            {
                if (l != actual[i++])
                {
                    Assert.Fail("String line not equal");
                }
            }
        }

        /// <summary>
        /// A test for ReadText
        /// </summary>
        [Test]
        public void ReadTextTest1()
        {
            ReadTextLinesTest(); // already tested previously
        }

        [Test]
        public void ReadTextLinesTestWhenFileBeingAccessedByAnotherProcess()
        {
            string fname = "ReadTextLinesTest.txt";
            string filePath = Path.Combine(_testDirectory, fname);
            File target = new File(filePath);

            var task = new Task(() => UseFileStream3Second(filePath));
            task.Start();
            Delay(200).Wait(); // Let the task run and acquire the file lock
            Assert.DoesNotThrow(() => target.ReadTextLines());
        }

        private void UseFileStream3Second(string filePath)
        {
            using (new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                // This line should appear in output to verfity the file lock is acquired
                Console.WriteLine("UseFileStream3Second started");
                Delay(3000).Wait();

                // This line should NOT appear in output as the test should be completed already
                Console.WriteLine("UseFileStream3Second finished");
            }
        }

        /// <summary>
        /// .NET 4 fallback method for Task.Delay
        /// </summary>
        /// <param name="milliseconds"></param>
        /// <returns></returns>
        private static Task Delay(double milliseconds)
        {
            var tcs = new TaskCompletionSource<bool>();
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Elapsed += (obj, args) => { tcs.TrySetResult(true); };
            timer.Interval = milliseconds;
            timer.AutoReset = false;
            timer.Start();
            return tcs.Task;
        }

        /// <summary>
        /// A test for ReadText
        /// </summary>
        [Test]
        public void ReadTextTest()
        {
            string fname = "test123.txt";
            string filePath = Path.Combine(_testDirectory, fname);
            File target = new File(filePath);

            string expected = "Single string";
            System.IO.File.WriteAllText(filePath, expected);

            string actual = target.ReadText();
            Assert.AreEqual(expected, actual, "String should be equal");
        }

        #endregion

        #region Test file operations

        /// <summary>
        /// A test for Touch
        /// </summary>
        [Test]
        public void CreateTest()
        {
            string filePath = Path.Combine(_testDirectory, "test123.txt");
            System.IO.File.Delete(filePath);

            File target = new File(filePath);
            target.Create();
            Assert.IsTrue(System.IO.File.Exists(filePath), "File should exist");
        }

        /// <summary>
        /// A test for Delete
        /// </summary>
        [Test]
        public void DeleteTest()
        {
            string filePath = Path.Combine(_testDirectory, "test123.txt");
            if (!System.IO.File.Exists(filePath))
            {
                System.IO.File.Create(filePath).Dispose();
            }
            File target = new File(filePath);
            target.Delete();
            Assert.IsFalse(System.IO.File.Exists(target.Path), "File should not exist");
        }

        /// <summary>
        /// A test for CopyTo
        /// </summary>
        [Test]
        public void CopyTest()
        {
            string fileContents = "testing";
            File file = CreateFileInTempDirectory();
            System.IO.File.WriteAllText(file.Path, fileContents);

            // Test that copying to an unused location succeeds.
            File destination = CreateFileInTempDirectory();
            file.Copy(destination);
            Assert.IsTrue(System.IO.File.Exists(destination.Path), "File should exist");
            Assert.AreEqual(fileContents,
                            System.IO.File.ReadAllLines(destination.Path)[0],
                            "File contents should have been copied.");

            // Test that copying to an existing location succeeds.
            System.IO.File.Delete(destination.Path);
            System.IO.File.Create(destination.Path).Dispose();
            file.Copy(destination);
            Assert.AreEqual(fileContents,
                            System.IO.File.ReadAllLines(destination.Path)[0],
                            "File contents should have been copied.");
        }

        /// <summary>
        /// A test for CopyToDirectory
        /// </summary>
        [Test]
        public void CopyToDirectoryTest()
        {
            string fname = "test123.txt";
            string filePath = Path.Combine(_testDirectory, fname);
            File source = new File(filePath);
            source.Create();

            File destination = new File(new Directory(Path.Combine(_testDirectory, "CopyToDirectory")), fname);
            destination.ParentDirectory.Delete();
            destination.ParentDirectory.Create();
            source.CopyToDirectory(destination.ParentDirectory);
            Assert.AreEqual(true, destination.Exists);
        }

        /// <summary>
        /// A test for Rename
        /// </summary>
        [Test]
        public void RenameTest()
        {
            string fname = "test123.txt";
            string filePath = Path.Combine(_testDirectory, fname);
            File target = new File(filePath);

            target.Create();

            string fname2 = "test123d.txt";
            string filePath2 = Path.Combine(_testDirectory, fname2);

            target.Rename(filePath2);

            Assert.IsTrue(System.IO.File.Exists(filePath2), "File should exist");
            Assert.IsFalse(System.IO.File.Exists(filePath), "File should not exist");
            Assert.AreEqual(target.Path, filePath2, "File name should be equal");
        }

        /// <summary>
        /// A test for Move
        /// </summary>
        [Test]
        public void MoveTest()
        {
            string fname = "test123.txt";
            string filePath = Path.Combine(_testDirectory, fname);
            string newDir = Path.Combine(_testDirectory, "testDir");
            string newFile = Path.Combine(newDir, fname);

            File target = new File(filePath);

            if (!System.IO.Directory.Exists(newDir))
            {
                System.IO.Directory.CreateDirectory(newDir);
            }

            target.Create();

            System.IO.File.Delete(newFile);

            target.Move(new File(newFile));
            Assert.IsTrue(System.IO.File.Exists(newFile), "File has not been moved");

            //check pathname is correct
            Assert.AreEqual(newFile, target.Path, "Filename has not been updated internally");

            System.IO.File.Delete(newFile);
        }

        /// <summary>
        /// A test for MoveToDirectory
        /// </summary>
        [Test]
        public void MoveToDirectoryTest()
        {
            File file = new File(Path.GetTempFileName());
            string filepathBeforeMove = file.Path;
            string filename = Path.GetFileName(filepathBeforeMove);

            string destinationDirName = "testdir";
            Directory destinationDir = new Directory(Path.Combine(_testDirectory, destinationDirName));
            if (destinationDir.Exists)
            {
                System.IO.Directory.Delete(destinationDir.Path, true);
            }
            System.IO.Directory.CreateDirectory(destinationDir.Path);
            string destinationDirPath = destinationDir.Path;
            if (destinationDirPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                destinationDirPath = destinationDirPath.Remove(destinationDirPath.Length - 1, 1);
            }

            file.MoveToDirectory(destinationDir);

            Assert.AreEqual(1,
                            System.IO.Directory.GetFiles(destinationDir.Path)
                                  .Count(x => Path.GetFileName(x) == filename),
                            "File should have been relocated.");
            Assert.AreEqual(destinationDirPath,
                            Path.GetDirectoryName(file.Path),
                            "File should have updated its location.");
            Assert.IsFalse(System.IO.File.Exists(filepathBeforeMove), "File should not exist at original location.");
        }

        #endregion

        /// <summary>
        /// A test for ToString
        /// </summary>
        [Test]
        public void ToStringTest()
        {
            string fname = "test123.txt";
            string filePath = Path.Combine(_testDirectory, fname);

            File target = new File(filePath);
            string expected = filePath;
            string actual;
            actual = target.ToString();
            Assert.AreEqual(expected, actual, "Values should be equal");
        }

        /// <summary>
        /// A test for File Constructor
        /// </summary>
        [Test]
        public void FileConstructorTest1()
        {
            string fname = "test123.txt";
            string filePath = Path.Combine(_testDirectory, fname);
            try
            {
                File target = new File(filePath);
            }
            catch (Exception e)
            {
                Assert.Fail("Constructor Failed: {0}", e);
            }
        }

        /// <summary>
        /// A test for File Constructor
        /// </summary>
        [Test]
        public void FileConstructorTest()
        {
            string fname = "test123.txt";
            string filePath = Path.Combine(_testDirectory, fname);
            Directory parentDirectory = new Directory(Path.GetDirectoryName(filePath));

            File target = new File(parentDirectory, fname);

            Assert.AreEqual(filePath, target.Path, "Expected same filepath");
        }

        /// <summary>
        /// A test for CreateTemporaryFile
        /// </summary>
        [Test]
        public void CreateTemporaryFileTest()
        {
            string extension = ".tmp";
            File actual;
            actual = File.CreateTemporaryFile(extension);

            Assert.IsFalse(System.IO.File.Exists(actual.Path), "File should not exist");
            actual.Create();

            Assert.IsTrue(System.IO.File.Exists(actual.Path), "File should exist");
            string dir = Path.GetDirectoryName(actual.Path) + "\\";
            Assert.AreEqual(Path.GetTempPath(), dir, "Temp Directory different");

            actual.Delete();
            Assert.IsFalse(System.IO.File.Exists(actual.Path), "Temp File should not exist");
        }
    }
}