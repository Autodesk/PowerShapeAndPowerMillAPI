// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System.IO;
using NUnit.Framework;
using File = Autodesk.FileSystem.File;

namespace Autodesk.Geometry.Test.GeometricEntities
{
    [TestFixture]
    public class DMTModelWriterTest

    {
        [Test]
        public void WhenWritingSTLFile_ThenCheckOutput()
        {
            DMTModel importedModel = DMTModelReader.ReadFile(new File(TestFiles.NormalStl));
            var outputFile = new File(string.Format("{0}\\output.stl", Path.GetTempPath()));
            DMTModelWriter.WriteFile(importedModel, outputFile);
            DMTModel writtenModel = DMTModelReader.ReadFile(outputFile);

            // Ensure that model is written correctly
            Assert.AreEqual(importedModel.BoundingBox.MaxX, writtenModel.BoundingBox.MaxX);
            Assert.AreEqual(importedModel.BoundingBox.MaxY, writtenModel.BoundingBox.MaxY);
            Assert.AreEqual(importedModel.BoundingBox.MaxZ, writtenModel.BoundingBox.MaxZ);
            Assert.AreEqual(importedModel.BoundingBox.MinX, writtenModel.BoundingBox.MinX);
            Assert.AreEqual(importedModel.BoundingBox.MinY, writtenModel.BoundingBox.MinY);
            Assert.AreEqual(importedModel.BoundingBox.MinZ, writtenModel.BoundingBox.MinZ);

            outputFile.Delete();
        }

        [Test]
        public void WhenWritingDMTFile_ThenCheckOutput()
        {
            DMTModel importedModel = DMTModelReader.ReadFile(new File(TestFiles.NormalDmt));
            var outputFile = new File(string.Format("{0}\\output.dmt", Path.GetTempPath()));
            DMTModelWriter.WriteFile(importedModel, outputFile);
            DMTModel writtenModel = DMTModelReader.ReadFile(outputFile);

            // Ensure that model is written correctly
            Assert.AreEqual(importedModel.BoundingBox.MaxX, writtenModel.BoundingBox.MaxX);
            Assert.AreEqual(importedModel.BoundingBox.MaxY, writtenModel.BoundingBox.MaxY);
            Assert.AreEqual(importedModel.BoundingBox.MaxZ, writtenModel.BoundingBox.MaxZ);
            Assert.AreEqual(importedModel.BoundingBox.MinX, writtenModel.BoundingBox.MinX);
            Assert.AreEqual(importedModel.BoundingBox.MinY, writtenModel.BoundingBox.MinY);
            Assert.AreEqual(importedModel.BoundingBox.MinZ, writtenModel.BoundingBox.MinZ);

            outputFile.Delete();
        }

        [Test]
        public void WhenWritingSTLFileFromDMTFile_ThenCheckOutput()
        {
            DMTModel importedModel = DMTModelReader.ReadFile(new File(TestFiles.NormalDmt));
            var outputFile = new File(string.Format("{0}\\output.stl", Path.GetTempPath()));
            DMTModelWriter.WriteFile(importedModel, outputFile);
            DMTModel writtenModel = DMTModelReader.ReadFile(outputFile);

            // Ensure that model is written correctly
            Assert.AreEqual(importedModel.BoundingBox.MaxX, writtenModel.BoundingBox.MaxX);
            Assert.AreEqual(importedModel.BoundingBox.MaxY, writtenModel.BoundingBox.MaxY);
            Assert.AreEqual(importedModel.BoundingBox.MaxZ, writtenModel.BoundingBox.MaxZ);
            Assert.AreEqual(importedModel.BoundingBox.MinX, writtenModel.BoundingBox.MinX);
            Assert.AreEqual(importedModel.BoundingBox.MinY, writtenModel.BoundingBox.MinY);
            Assert.AreEqual(importedModel.BoundingBox.MinZ, writtenModel.BoundingBox.MinZ);

            outputFile.Delete();
        }
    }
}