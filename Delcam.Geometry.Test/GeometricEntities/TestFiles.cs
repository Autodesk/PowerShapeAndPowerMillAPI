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
using System.Linq;
using NUnit.Framework;

namespace Autodesk.Geometry.Test.GeometricEntities
{
    public static class TestFiles
    {
        public static readonly string FOLDER_NAME = "TestFiles";
        public static readonly string DIRECTORY = SetSolutionFolder();
        public static readonly string TEMP_DIRECTORY = Directory.CreateDirectory(Path.Combine(DIRECTORY, "Temp")).FullName;
        public static string BoundariesTestSquare = FetchTestFile("BoundariesTestSquare.stl");
        public static string SmallModel = FetchTestFile("smallModel.dmt");
        public static string NormalDmt = FetchTestFile("NormalDmt.dmt");
        public static string NormalStl = FetchTestFile("NormalSTL.stl");

        private static string SetSolutionFolder()
        {
            var currentLocation = TestContext.CurrentContext.TestDirectory;
            var solutionDirector = currentLocation.Split(new[] {"\\bin\\"}, StringSplitOptions.RemoveEmptyEntries).First();
            return Path.Combine(solutionDirector, FOLDER_NAME);
        }

        public static string FetchTestFile(string fullName)
        {
            var directoryInfo = new DirectoryInfo(DIRECTORY);
            var results = directoryInfo.GetFiles(fullName, SearchOption.AllDirectories);
            return results.FirstOrDefault()?.FullName;
        }
    }
}