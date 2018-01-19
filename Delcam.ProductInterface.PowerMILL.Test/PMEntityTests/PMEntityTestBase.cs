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
using Autodesk.ProductInterface.PowerMILL;
using NUnit.Framework;

namespace Autodesk.ProductInterface.PowerMILLTest.PMEntityTests
{
    [TestFixture]
    public class PMEntityTestBase
    {
        #region Fields

        protected PMAutomation _powerMILL;
        private Version _defaultVersion = new Version("16.0.17");
        protected Directory _tmpPowerMillProject = null;
        protected List<Directory> _TmpFoldersToDelete = new List<Directory>();
        protected List<File> _tmpFilesToDelete = new List<File>();

        #endregion

        #region Constructor

        public PMEntityTestBase()
        {
            // Initialise PowerMill
            if (_powerMILL == null)
            {
                _powerMILL = new PMAutomation(InstanceReuse.UseExistingInstance);
            }
        }

        #endregion

        #region Test Setup and Close

        [TearDown]
        public virtual void TearDown()
        {
            try
            {
                if (_powerMILL != null)
                {
                    _powerMILL.CloseProject();
                    if (_tmpPowerMillProject != null && _tmpPowerMillProject.Exists)
                    {
                        _tmpPowerMillProject.Delete();
                    }

                    if (_TmpFoldersToDelete.Count > 0)
                    {
                        foreach (Directory dir in _TmpFoldersToDelete)
                        {
                            if (dir.Exists)
                            {
                                try
                                {
                                    dir.Delete();
                                }
                                catch
                                {
                                }
                            }
                        }
                        _TmpFoldersToDelete.Clear();
                    }

                    if (_tmpFilesToDelete.Count > 0)
                    {
                        foreach (File fi in _tmpFilesToDelete)
                        {
                            try
                            {
                                fi.Delete();
                            }
                            catch
                            {
                            }
                        }
                        _tmpFilesToDelete.Clear();
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        [SetUp]
        public virtual void Setup()
        {
            // Start PowerMill
            ResetPowerMill();
            _TmpFoldersToDelete.Clear();
        }

        protected void ResetPowerMill()
        {
            // Reset PowerMill
            _powerMILL.CloseProject();

            // Check _PowerMill has started correctly
            Assert.IsNotNull(_powerMILL, "PowerMill has not been started");
            _powerMILL.IssueEchoOffCommands = false;
            _powerMILL.EchoCommandsOff();
            _powerMILL.DialogsOff();
        }

        /// <summary>
        /// Creates a copy of the passed powerMill project and loads it into PowerMill.
        /// </summary>
        /// <param name="pmFolder"></param>
        /// <returns></returns>
        protected Directory LoadCopyOfPowerMillProject(Directory pmFolder, bool deleteTmpPmProjectWhenTestFinished)
        {
            Directory tmpFolder = Directory.CreateTemporaryDirectory();
            if (pmFolder.Exists == false)
            {
                Assert.Fail("PowerMill project {0} does not exist!", pmFolder.Path);
            }

            pmFolder.Copy(tmpFolder);
            if (deleteTmpPmProjectWhenTestFinished)
            {
                _TmpFoldersToDelete.Add(tmpFolder);
            }
            _powerMILL.CloseProject();
            _powerMILL.LoadProject(tmpFolder);

            return tmpFolder;
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            try
            {
                // Switch FormUpdate and Dialogs back on
                _powerMILL.DialogsOn();

                // Close all models
                _powerMILL.CloseProject();
            }
            catch (Exception)
            {
            }
        }

        #endregion
    }
}