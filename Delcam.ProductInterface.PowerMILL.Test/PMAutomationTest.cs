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
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Autodesk.ProductInterface.PowerMILL;
using NUnit.Framework;

namespace Autodesk.ProductInterface.PowerMILLTest
{
    [TestFixture]
    public class PMAutomationTest
    {
        #region Fields

        private PMAutomation _powerMill;

        #endregion

        #region Operations

        [SetUp]
        public void MyTestInitialize()
        {
            _powerMill = new PMAutomation(InstanceReuse.UseExistingInstance);
        }

        [TestFixtureTearDown]
        public void LeavingAutomationTest()
        {
            _powerMill = new PMAutomation(InstanceReuse.UseExistingInstance);
        }

        #endregion

        [Ignore("Fail on Build Server")]
        [Test]
        public void Quit()
        {
            var newInstance = new PMAutomation(InstanceReuse.CreateNewInstance,
                                               new Version(_powerMill.Version.Major, 0, 0),
                                               new Version(_powerMill.Version.Major, 0, 99));
            Assert.IsNotNull(newInstance, "PowerMILL has not been started");

            newInstance.Quit();

            var processes = Process.GetProcessesByName("pmill");
            Assert.That(processes.Select(p => p.Id).ToList(),
                        Has.None.EqualTo(newInstance.ProcessId),
                        "Expected PowerMILL process to have terminated.");
        }

        [Test]
        public void LoadProject()
        {
            // #1
            if (_powerMill.Version < new Version(15, 0))
            {
                Assert.Inconclusive("This test requires PowerMILL 15 or greater");
            }

            _powerMill.IsVisible = true;
            _powerMill.LoadProject(Files.TestFiles.SimplePmProject1);
            Assert.IsTrue(Process.GetProcessesByName("pmill")[0].MainWindowTitle.Contains("Editable Project - SimpleProject1"),
                          "Loading of 'SimplePmProject1' failed.");

            _powerMill.Execute("PROJECT RESET");

            // #2
            _powerMill.LoadProject(Files.TestFiles.SimplePmProject1, true);
            Assert.IsTrue(Process.GetProcessesByName("pmill")[0].MainWindowTitle.Contains("Read-Only Project - SimpleProject1"),
                          "Loading of 'SimplePmProject1' in readonly mode failed.");
        }

        [Test]
        public void CloseProject()
        {
            _powerMill.Execute(string.Format("PROJECT OPEN {0}", Files.TestFiles.SimplePmProject1.Path));
            Assert.AreEqual("SimpleProject1",
                            _powerMill.ExecuteEx(@"PRINT PAR terse ""project_pathname(1)"""),
                            "Expected project to have been opened.");
            _powerMill.CloseProject();
            Assert.AreEqual(string.Empty,
                            _powerMill.ExecuteEx(@"PRINT PAR terse ""project_pathname(1)"""),
                            "Expected project to have been closed.");
        }

        [Test]
        public void RunMacro()
        {
            _powerMill.RunMacro(Files.TestFiles.MacroCreateNcProgram);
            var ncprograms = _powerMill.ExecuteEx("PRINT ENTITY NCPROGRAM").ToString().Split('\r');
            Assert.AreEqual(2, ncprograms.Count(), "Expected macro to run, creating an NC program.");
        }

        [Ignore("Fail on Build Server")]
        [Test]
        public void CreateNewInstanceTest()
        {
            PMAutomation automation = new PMAutomation(InstanceReuse.CreateNewInstance,
                                                       new Version(_powerMill.Version.Major, 0, 0),
                                                       new Version(_powerMill.Version.Major, 0, 99));
            Assert.IsNotNull(automation, "PowerMILL has not been started");
            automation.Quit();
        }

        [Ignore("Fail on Build Server")]
        [Test]
        public void CreateTwoNewInstanceTest()
        {
            // Create new instance
            PMAutomation automation = new PMAutomation(InstanceReuse.CreateNewInstance,
                                                       new Version(_powerMill.Version.Major, 0, 0),
                                                       new Version(_powerMill.Version.Major, 0, 99));
            Assert.IsNotNull(automation, "PowerMILL has not been started");

            // Create second instance
            PMAutomation automation2 = new PMAutomation(InstanceReuse.CreateNewInstance,
                                                        new Version(_powerMill.Version.Major, 0, 0),
                                                        new Version(_powerMill.Version.Major, 0, 99));
            Assert.IsNotNull(automation2, "PowerMILL has not been started");

            automation.Quit();
            automation2.Quit();
        }

        [Ignore("Fail on Build Server")]
        [Test]
        public void CreateSingleInstanceTest()
        {
            var previousProcessId = _powerMill.ProcessId;

            var singleInstance = new PMAutomation(InstanceReuse.CreateSingleInstance,
                                                  new Version(_powerMill.Version.Major, 0, 0),
                                                  new Version(_powerMill.Version.Major, 0, 99));
            _powerMill = singleInstance;

            var processesIds = Process.GetProcesses().Select(p => p.Id);
            Assert.That(processesIds, Has.None.EqualTo(previousProcessId));
            Assert.That(processesIds, Contains.Item(singleInstance.ProcessId));
        }

        [Ignore("Fail on Build Server")]
        [Test]
        public void ProcessIdTest()
        {
            var singleInstance = new PMAutomation(InstanceReuse.CreateNewInstance);
            var newProcessId = singleInstance.ProcessId;
            singleInstance.Quit();

            Assert.That(newProcessId, Is.Not.EqualTo(_powerMill.ProcessId));
        }

        [Ignore("Fail on Build Server")]
        [Test]
        public void MultipleVersionsTest_WhenUsing2018Notation()
        {
            var versionUnderTest = _powerMill.Version;
            var twenty17 = new PMAutomation(InstanceReuse.CreateNewInstance, new Version(21, 0, 0), new Version(21, 0, 99));
            Assert.That(twenty17.Version.Major, Is.EqualTo(21));
            twenty17.Quit();
            var twenty18 = new PMAutomation(InstanceReuse.CreateNewInstance, new Version(2018, 0, 0), new Version(2018, 0, 99));
            Assert.That(twenty18.Version.Major, Is.EqualTo(2018));
            twenty18.Quit();

            // Ensure that the version under test is properly registered as the current version so other tests carry on ok...
            var testVersion = new PMAutomation(InstanceReuse.CreateNewInstance, versionUnderTest);
            testVersion.Quit();
        }

        [Test]
        public void AttachToCOMObjectTest()
        {
            var comObject =
                _powerMill.GetType().GetField("_powerMILL", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(_powerMill);
            var powerMill = new PMAutomation(comObject);
            powerMill.LoadProject(Files.TestFiles.SimplePmProject1);
            Assert.That(powerMill.ActiveProject.Directory.Path, Is.EqualTo(_powerMill.ActiveProject.Directory.Path));
        }

        [Test]
        public void ExecutablePath_WhenVersionIs2018()
        {
            var pathMethod = _powerMill.GetType().GetMethod("ExecutablePath", BindingFlags.Instance | BindingFlags.NonPublic);
            var path = pathMethod.Invoke(_powerMill, new object[] {"PowerMill", new Version(2018, 0), null});
            Assert.That(path, Is.EqualTo("C:\\Program Files\\Autodesk\\PowerMill 2018\\sys\\exec64\\pmill.exe"));
        }

        [Test]
        public void ExecutablePath_WhenVersionIs2018AndMaxSpecified()
        {
            var pathMethod = _powerMill.GetType().GetMethod("ExecutablePath", BindingFlags.Instance | BindingFlags.NonPublic);
            var path = pathMethod.Invoke(_powerMill,
                                         new object[] {"PowerMill", new Version(2018, 0, 0), new Version(2018, 0, 99)});
            Assert.That(path, Is.EqualTo("C:\\Program Files\\Autodesk\\PowerMill 2018\\sys\\exec64\\pmill.exe"));
        }

        [Test]
        public void ExecutablePath_WhenVersionIs2017()
        {
            var pathMethod = _powerMill.GetType().GetMethod("ExecutablePath", BindingFlags.Instance | BindingFlags.NonPublic);
            var path = pathMethod.Invoke(_powerMill, new object[] {"PowerMill", new Version(21, 0, 30), null});
            Assert.That(path, Is.EqualTo("C:\\Program Files\\Autodesk\\PowerMill 21.0.30\\sys\\exec64\\pmill.exe"));
        }

        [Test]
        public void ExecutablePath_WhenVersionIs2017AndMaxSpecified()
        {
            var pathMethod = _powerMill.GetType().GetMethod("ExecutablePath", BindingFlags.Instance | BindingFlags.NonPublic);
            var path = pathMethod.Invoke(_powerMill, new object[] {"PowerMill", new Version(21, 0, 30), new Version(21, 0, 99)});
            Assert.That(path, Is.EqualTo("C:\\Program Files\\Autodesk\\PowerMill 21.0.30\\sys\\exec64\\pmill.exe"));
        }

        [Test]
        public void CheckCollisionStatus_OFF()
        {
            _powerMill.CollisionsOff();
            Assert.That(_powerMill.CheckCollisionStatus(), Is.False);
        }

        [Test]
        public void CheckCollisionStatus_ON()
        {
            _powerMill.CollisionsOn();
            Assert.That(_powerMill.CheckCollisionStatus(), Is.True);
        }

        [Test]
        public void GetListOfPmComObjectsTest()
        {
            //Creating 1 more PowerMill instance
            PMAutomation automation2 = new PMAutomation(InstanceReuse.CreateNewInstance);

            //Get the number of Com Object instances
            List<object> instances = PMAutomation.GetListOfPmComObjects();
            Assert.That(instances.Count, Is.EqualTo(2));

            //Quit
            automation2.Quit();
        }

        [Test]
        public void UndrawAllTest()
        {            
            MyTestInitialize();
            var testPattern = _powerMill.ActiveProject.Patterns.CreatePattern(Files.TestFiles.CurvesFiles);
            testPattern.IsVisible = true;            
            _powerMill.UndrawAll();
            Assert.IsTrue(true);
        }
    }
}