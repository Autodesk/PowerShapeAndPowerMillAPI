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
using Directory = Autodesk.FileSystem.Directory;
using File = Autodesk.FileSystem.File;

namespace Autodesk.ProductInterface.PowerMILLTest.Files
{
    /// <summary>
    /// Holds directory paths for PM projects
    /// </summary>
    public static class TestFiles
    {
        private static readonly string FOLDER_NAME = "Files";
        public static readonly string DIRECTORY = SetSolutionFolder();

        public static Directory ExistingDentMillMasterProject = FetchDirectory("ExistingDentMillMasterProject");
        public static Directory PMwithNcProgram = FetchDirectory("PMwithNcProgram");
        public static Directory PMwithNcProgram1 = FetchDirectory("PMwithNcPrograms1");
        public static Directory FeatureGroupsProject = FetchDirectory("FeatureGroupsProject");
        public static Directory SetupsProject = FetchDirectory("SetupsProject");
        public static Directory LevelsSetOrClampProject = FetchDirectory("LevelSetOrClamp");

        public static File TestMacro1File = new File(FetchTestFile("TestMacro1.mac"));
        public static File TestMacro2File = new File(FetchTestFile("TestMacro2.mac"));
        public static File TestMacro3File = new File(FetchTestFile("TestMacro3.mac"));
        public static File CreateThreeWorkplaneMacro = new File(FetchTestFile("CreateThreeWorkplane.mac"));
        public static File MacroCreateNcProgram = new File(FetchTestFile("CreateNCProgram.mac"));
        public static File TestMacroStepEventFires = new File(FetchTestFile("TestMacroStepEventFires.mac"));
        public static File CreateBoundaries = new File(FetchTestFile("CreateBoundaries.mac"));
        public static File OptionFileRoedersRxd5RcsDpp48 = new File(FetchTestFile("Roeders_RXD5_RCS_DPP48.pmopt"));

        #region Model Files

        public static Directory PMModelDir = FetchDirectory("Models");
        public static File TestModelSurfaceFile1 = new File(FetchTestFile("Mesh.dgk"));
        public static File SingleMesh = new File(FetchTestFile("SingleMesh.dmt"));

        #endregion

        public static File SimpleProjectBravoWorkPlane = new File(FetchTestFile("SimpleProject_bravo_WP.txt"));
        public static File CurvesFiles = new File(FetchTestFile("Curves.pic"));
        public static File SihouetteFile = new File(FetchTestFile("silhouette.dgk"));
        public static File ImportTemplateTestFile = new File(FetchTestFile("ImportPTFTest.ptf"));

        public static Directory SimplePmProject1 = FetchDirectory("SimpleProject1");
        public static Directory SimplePmProjectWithGouges1 = FetchDirectory("ToolAndHolderCollides1");
        public static Directory ToolProperties = FetchDirectory("ToolProperties");
        public static Directory ToolProperties2018 = FetchDirectory("ToolProperties2018");
        public static Directory ToolPathStrategies = FetchDirectory("ToolpathStrategies");
        public static Directory BoundaryTypes = FetchDirectory("BoundaryTypes");
        public static Directory BasicToolpath = FetchDirectory("BasicToolpath");

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

        public static Directory FetchDirectory(string fullName)
        {
            var dir = new DirectoryInfo(DIRECTORY);
            var result = dir.GetDirectories(fullName, SearchOption.AllDirectories);
            return new Directory(result.FirstOrDefault()?.FullName);
        }
    }
}