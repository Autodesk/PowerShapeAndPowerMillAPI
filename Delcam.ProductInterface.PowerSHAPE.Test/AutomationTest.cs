// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using Autodesk.FileSystem;
using Autodesk.Geometry;
using Autodesk.ProductInterface.PowerSHAPE;
using Autodesk.ProductInterface.PowerSHAPETest.HelperClassesTests;
using NUnit.Framework;

namespace Autodesk.ProductInterface.PowerSHAPETest
{
    /// <summary>
    /// This is a test class for AutomationTest and is intended
    /// to contain all AutomationTest Unit Tests
    /// </summary>
    [TestFixture]
    public class AutomationTest
    {
        #region Fields

        private PSAutomation _powerSHAPE;

        #endregion

        #region Constructors

        public AutomationTest()
        {
            //Initialise PowerSHAPE
            if (_powerSHAPE == null)
            {
                _powerSHAPE = new PSAutomation(InstanceReuse.UseExistingInstance, Modes.PShapeMode);
                _powerSHAPE.DialogsOff();
            }
        }

        #endregion

        #region Base Automation Tests

        /// <summary>
        /// A test for ExecutableName
        /// </summary>
        [Test]
        public void ExecutableNameTest()
        {
            // Get executable name
            var executableName =
                (string)
                _powerSHAPE.GetType()
                           .GetProperty("ExecutableName", BindingFlags.NonPublic | BindingFlags.Instance)
                           .GetValue(_powerSHAPE, null);

            // Check that it's correct
            Assert.AreEqual("powershape", executableName.ToLower(), "Executable name is incorrect");
        }

        #endregion

        #region Additional test attributes

        [SetUp]
        public void MyTestInitialize()
        {
            // Start PowerSHAPE
            StartPowerSHAPETest();
        }

        [TearDown]
        public void MyTestCleanup()
        {
            try
            {
                // Check that no dialogs need to be closed
                _powerSHAPE.Execute("CANCEL");

                // Switch FormUpdate and Dialogs back on
                _powerSHAPE.FormUpdateOn();
                _powerSHAPE.DialogsOn();

                // Close all models
                _powerSHAPE.Models.Clear();
            }
            catch (Exception)
            {
            }
        }

        #endregion

        #region Automation Tests

        #region Properties

        /// <summary>
        /// A test for ActiveDocument
        /// </summary>
        [Test]
        public void ActiveWindowTest()
        {
            // Open known model
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.LEVELS_MODEL));

            // Check that the active window is correct
            Assert.AreEqual(_powerSHAPE.ActiveModel.Window,
                            _powerSHAPE.ActiveWindow,
                            "PowerSHAPE has identified the wrong window as being active");
        }

        /// <summary>
        /// A test for ActiveModel
        /// </summary>
        [Test]
        public void ActiveModelTest()
        {
            // Open known model
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.LEVELS_MODEL));

            // Check that the active model is correct
            Assert.AreEqual("LevelTests",
                            _powerSHAPE.ActiveModel.Name,
                            "PowerSHAPE has identified the wrong model as being active");
        }

        /// <summary>
        /// A test for chekcing models refresh function.
        /// </summary>
        [Test]
        public void RefreshModelsListTest()
        {
            // Open known model
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.LEVELS_MODEL));

            // Get the number of models
            var icount = _powerSHAPE.Models.Count;

            // Create a new model with the old method to simulate a model imported/created manually
            _powerSHAPE.Execute("FILE NEW");

            // Refresh the list of models
            _powerSHAPE.RefreshWindowsAndModelsList();

            // Check the number of models
            Assert.That(icount + 1, Is.EqualTo(_powerSHAPE.Models.Count));
        }

        #endregion

        #region Operations

        [Test]
        public void CancelPointPickingTest()
        {
            var cancelPointPickerHelper = new PSCancelPointPicker();

            var testHarness = new PointPickerCancelTestHarness(cancelPointPickerHelper, _powerSHAPE);
            testHarness.Run();
            Thread.Sleep(1000); //give time for thread to start up.
            Assert.IsTrue(testHarness.IsMyThreadAlive(), "Point Picker Thread is not running");

            cancelPointPickerHelper.CanelPointPicking();
            Thread.Sleep(1000); //give time for thread to start up.

            Assert.IsFalse(testHarness.IsMyThreadAlive(), "Thread should have finished");

            Assert.IsNull(testHarness.PickedPoint);
        }

        [Test]
        public void RecordMacroTest()
        {
            var tmpMacFile = File.CreateTemporaryFile(".mac");
            tmpMacFile.Delete();

            try
            {
                _powerSHAPE.RecordMacro(tmpMacFile.Path);

                _powerSHAPE.Execute("PRINT 'fred'");

                _powerSHAPE.StopMacroRecording();

                Assert.True(tmpMacFile.Exists, "macro file should exist");

                var lines = System.IO.File.ReadAllLines(tmpMacFile.Path);

                if (lines == null || lines.Length < 1)
                {
                    Assert.Fail("Macro file is empty");
                }

                _powerSHAPE.Reset();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
            finally
            {
                _powerSHAPE.Reset();
                tmpMacFile.Delete();
            }
        }

        /// <summary>
        /// A test for Automation Constructor
        /// </summary>
        [Test]
        public void StartPowerSHAPETest()
        {
            // Reset PowerSHAPE
            _powerSHAPE.Reset();
            _powerSHAPE.DialogsOff();

            // Check _powerSHAPE has started correctly
            Assert.IsNotNull(_powerSHAPE, "PowerSHAPE has not been started");
            Assert.AreNotEqual(0, _powerSHAPE.Windows, "No windows were opened");
            Assert.AreNotEqual(0, _powerSHAPE.Models, "No models were opened");
        }

        [Test]
        public void AttachToCOMObjectTest()
        {
            var comObject =
                _powerSHAPE.GetType().GetField("_powerSHAPE", BindingFlags.NonPublic | BindingFlags.Instance)
                           .GetValue(_powerSHAPE);
            var powerShape = new PSAutomation(comObject);
            var windowsBefore = (double) _powerSHAPE.ExecuteEx("WINDOW.NUMBER");
            powerShape.Models.CreateModelFromFile(new File(TestFiles.LEVELS_MODEL));
            Assert.That(powerShape.Windows.Count, Is.EqualTo(windowsBefore + 1));
            Assert.That((double) _powerSHAPE.ExecuteEx("WINDOW.NUMBER"), Is.EqualTo(windowsBefore + 1));
            powerShape.Models.Last().Delete();
        }

        /// <summary>
        /// A test for CreateNewInstance
        /// </summary>
        [Test]
        public void CreateNewInstanceTest()
        {
            var versionUnderTest = _powerSHAPE.Version;

            // Create new instance
            var automation = new PSAutomation(InstanceReuse.CreateNewInstance,
                                              new Version(versionUnderTest.Major, versionUnderTest.Minor, 0),
                                              new Version(versionUnderTest.Major, versionUnderTest.Minor, 99),
                                              Modes.PShapeMode);
            Assert.IsNotNull(automation, "PowerSHAPE has not been started");
            Assert.Less(new Version(versionUnderTest.Major, versionUnderTest.Minor, 0),
                        automation.Version,
                        "PowerSHAPE version is incorrect");
            Assert.Greater(new Version(versionUnderTest.Major, versionUnderTest.Minor, 99),
                           automation.Version,
                           "PowerSHAPE version is incorrect");
            Assert.AreNotEqual(0, automation.Windows, "No windows were opened");
            Assert.AreNotEqual(0, automation.Models, "No models were opened");
            automation.Quit();
        }

        /// <summary>
        /// A test for CreateNewInstance
        /// </summary>
        [Test]
        [Ignore("")]
        public void CreateNewInstanceTest2()
        {
            var versionUnderTest = _powerSHAPE.Version;

            // Create new instance
            var automation = new PSAutomation(InstanceReuse.CreateNewInstance,
                                              new Version(versionUnderTest.Major, versionUnderTest.Minor, 0),
                                              new Version(versionUnderTest.Major, versionUnderTest.Minor, 99),
                                              Modes.PShapeMode);
            Assert.IsNotNull(automation, "PowerSHAPE has not been started");
            Assert.Less(new Version(versionUnderTest.Major, versionUnderTest.Minor, 0),
                        automation.Version,
                        "PowerSHAPE version is incorrect");
            Assert.Greater(new Version(versionUnderTest.Major, versionUnderTest.Minor, 99),
                           automation.Version,
                           "PowerSHAPE version is incorrect");

            // Create second instance
            var automation2 = new PSAutomation(InstanceReuse.CreateNewInstance,
                                               new Version(versionUnderTest.Major, versionUnderTest.Minor, 0),
                                               new Version(versionUnderTest.Major, versionUnderTest.Minor, 99),
                                               Modes.PShapeMode);
            Assert.IsNotNull(automation2, "PowerSHAPE has not been started");
            Assert.Less(new Version(versionUnderTest.Major, versionUnderTest.Minor, 0),
                        automation2.Version,
                        "PowerSHAPE version is incorrect");
            Assert.Greater(new Version(versionUnderTest.Major, versionUnderTest.Minor, 99),
                           automation2.Version,
                           "PowerSHAPE version is incorrect");

            automation.Quit();
            automation2.Quit();
        }

        /// <summary>
        /// A test for CreateSingleInstance
        /// </summary>
        [Test]
        [Ignore("")]
        public void CreateSingleInstanceTest()
        {
            // What version are we currently testing?
            var versionUnderTest = _powerSHAPE.Version;
            var psProcess = Process.GetProcessById(_powerSHAPE.ProcessId);

            // Create new instance
            var automation = new PSAutomation(InstanceReuse.CreateSingleInstance,
                                              new Version(versionUnderTest.Major, versionUnderTest.Minor, 0),
                                              new Version(versionUnderTest.Major, versionUnderTest.Minor, 99),
                                              Modes.PShapeMode);

            // Check that the previous one exited
            var processExited = psProcess.HasExited;

            // Set to null incase the asserts fail
            _powerSHAPE = null;

            Assert.IsNotNull(automation, "PowerSHAPE has not been started");
            var startedVersion = automation.Version;
            automation.Quit();

            // Start the version under test again so that things carry on as before
            _powerSHAPE = new PSAutomation(InstanceReuse.CreateSingleInstance, versionUnderTest);

            Assert.Less(new Version(versionUnderTest.Major, versionUnderTest.Minor, 0),
                        startedVersion,
                        "PowerSHAPE version is incorrect");
            Assert.Greater(new Version(versionUnderTest.Major, versionUnderTest.Minor, 99),
                           startedVersion,
                           "PowerSHAPE version is incorrect");
            Assert.AreEqual(true, processExited);
        }

        /// <summary>
        /// A test for UseExistingInstance when no windows opened (bespokework#428)
        /// </summary>
        [Test]
        public void UseExistingInstanceWhenNoWindowsOpenTest()
        {
            _powerSHAPE.Windows.Clear();

            var automation = new PSAutomation(InstanceReuse.UseExistingInstance);

            Assert.Pass("Succeeded to UseExistingInstance with no windows open.");
        }

        [Test]
        public void ExecutablePath_WhenVersionIs2018()
        {
            var pathMethod = _powerSHAPE.GetType().GetMethod("ExecutablePath", BindingFlags.Instance | BindingFlags.NonPublic);
            var path = pathMethod.Invoke(_powerSHAPE, new object[] {"PowerShape", new Version(2018, 0), null});
            Assert.That(path, Is.EqualTo("C:\\Program Files\\Autodesk\\PowerShape 2018\\sys\\exec64\\powershape.exe"));
        }

        [Test]
        public void ExecutablePath_WhenVersionIs2018AndMaxSpecified()
        {
            var pathMethod = _powerSHAPE.GetType().GetMethod("ExecutablePath", BindingFlags.Instance | BindingFlags.NonPublic);
            var path = pathMethod.Invoke(_powerSHAPE,
                                         new object[] {"PowerShape", new Version(2018, 0, 0), new Version(2018, 0, 99)});
            Assert.That(path, Is.EqualTo("C:\\Program Files\\Autodesk\\PowerShape 2018\\sys\\exec64\\powershape.exe"));
        }

        [Test]
        public void ExecutablePath_WhenVersionIs2017()
        {
            var pathMethod = _powerSHAPE.GetType().GetMethod("ExecutablePath", BindingFlags.Instance | BindingFlags.NonPublic);
            var path = pathMethod.Invoke(_powerSHAPE, new object[] {"PowerShape", new Version(17, 1, 36), null});
            Assert.That(path, Is.EqualTo("C:\\Program Files\\Autodesk\\PowerShape17136\\sys\\exec64\\powershape.exe"));
        }

        [Test]
        public void ExecutablePath_WhenVersionIs2017AndMaxSpecified()
        {
            var pathMethod = _powerSHAPE.GetType().GetMethod("ExecutablePath", BindingFlags.Instance | BindingFlags.NonPublic);
            var path = pathMethod.Invoke(_powerSHAPE, new object[] {"PowerShape", new Version(17, 1, 0), new Version(17, 1, 99)});
            Assert.That(path, Is.EqualTo("C:\\Program Files\\Autodesk\\PowerShape17136\\sys\\exec64\\powershape.exe"));
        }

        [Test]
        public void UseExistingInstance_WhenUsing18NotationWithoutSpecifyingMaximumVersion()
        {
            var versionUnderTest = _powerSHAPE.Version;
            var twenty18 = new PSAutomation(InstanceReuse.UseExistingInstance, new Version("18.1.0"));
            Assert.That(twenty18.Version.Major, Is.EqualTo(18));
            twenty18.Quit();

            // Ensure that the version under test is properly registered as the current version so other tests carry on ok...
            var testVersion = new PSAutomation(InstanceReuse.CreateNewInstance, versionUnderTest);
            testVersion.Quit();
        }

        [Test]
        public void UseExistingInstance_WhenUsing2018NotationWithoutSpecifyingMaximumVersion()
        {
            var versionUnderTest = _powerSHAPE.Version;
            var twenty18 = new PSAutomation(InstanceReuse.UseExistingInstance, new Version("2018.0.0"));
            Assert.That(twenty18.Version.Major, Is.EqualTo(2018));
            twenty18.Quit();

            // Ensure that the version under test is properly registered as the current version so other tests carry on ok...
            var testVersion = new PSAutomation(InstanceReuse.CreateNewInstance, versionUnderTest);
            testVersion.Quit();
        }

        [Test]
        public void UseExistingInstance_WhenUsing2018NotationWithoutSpecifyingMaximumVersion_And_Build()
        {
            var versionUnderTest = _powerSHAPE.Version;
            var twenty18 = new PSAutomation(InstanceReuse.UseExistingInstance, new Version("2018.0"));
            Assert.That(twenty18.Version.Major, Is.EqualTo(2018));
            twenty18.Quit();

            // Ensure that the version under test is properly registered as the current version so other tests carry on ok...
            var testVersion = new PSAutomation(InstanceReuse.CreateNewInstance, versionUnderTest);
            testVersion.Quit();
        }

        [Test]
        public void CreateSingleInstance_WhenUsing2018NotationWithoutSpecifyingMaximumVersion_And_Build()
        {
            var versionUnderTest = _powerSHAPE.Version;
            var twenty18 = new PSAutomation(InstanceReuse.CreateSingleInstance, new Version("2018.0"));
            Assert.That(twenty18.Version.Major, Is.EqualTo(18));
            twenty18.Quit();

            // Ensure that the version under test is properly registered as the current version so other tests carry on ok...
            var testVersion = new PSAutomation(InstanceReuse.CreateNewInstance, versionUnderTest);
            testVersion.Quit();
        }

        [Test]
        public void CreateSingleInstance_WhenUsing2018NotationWithoutSpecifyingMaximumVersion()
        {
            var versionUnderTest = _powerSHAPE.Version;
            var twenty18 = new PSAutomation(InstanceReuse.CreateSingleInstance, new Version("2018.0.0"));
            Assert.That(twenty18.Version.Major, Is.EqualTo(18));
            twenty18.Quit();

            // Ensure that the version under test is properly registered as the current version so other tests carry on ok...
            var testVersion = new PSAutomation(InstanceReuse.CreateNewInstance, versionUnderTest);
            testVersion.Quit();
        }

        [Test]
        public void CreateSingleInstance_WhenUsing18NotationWithoutSpecifyingMaximumVersion()
        {
            var versionUnderTest = _powerSHAPE.Version;
            var twenty18 = new PSAutomation(InstanceReuse.CreateSingleInstance, new Version("18.1.0"));
            Assert.That(twenty18.Version.Major, Is.EqualTo(18));
            twenty18.Quit();

            // Ensure that the version under test is properly registered as the current version so other tests carry on ok...
            var testVersion = new PSAutomation(InstanceReuse.CreateNewInstance, versionUnderTest);
            testVersion.Quit();
        }


        [Test]
        public void CreateNewInstance_WhenUsing18NotationWithoutSpecifyingMaximumVersion()
        {
            var versionUnderTest = _powerSHAPE.Version;
            var twenty18 = new PSAutomation(InstanceReuse.CreateNewInstance, new Version("18.1.0"));
            Assert.That(twenty18.Version.Major, Is.EqualTo(18));
            twenty18.Quit();

            // Ensure that the version under test is properly registered as the current version so other tests carry on ok...
            var testVersion = new PSAutomation(InstanceReuse.CreateNewInstance, versionUnderTest);
            testVersion.Quit();
        }


        [Test]
        public void CreateNewInstance_WhenUsing2018NotationWithoutSpecifyingMaximumVersion_And_Build()
        {
            var versionUnderTest = _powerSHAPE.Version;
            var twenty18 = new PSAutomation(InstanceReuse.CreateNewInstance, new Version("2018.0"));
            Assert.That(twenty18.Version.Major, Is.EqualTo(18));
            twenty18.Quit();

            // Ensure that the version under test is properly registered as the current version so other tests carry on ok...
            var testVersion = new PSAutomation(InstanceReuse.CreateNewInstance, versionUnderTest);
            testVersion.Quit();
        }

        [Test]
        public void CreateNewInstance_WhenUsing2018NotationWithoutSpecifyingMaximumVersion()
        {
            var versionUnderTest = _powerSHAPE.Version;
            var twenty18 = new PSAutomation(InstanceReuse.CreateNewInstance, new Version("2018.0.0"));
            Assert.That(twenty18.Version.Major, Is.EqualTo(18));
            twenty18.Quit();

            // Ensure that the version under test is properly registered as the current version so other tests carry on ok...
            var testVersion = new PSAutomation(InstanceReuse.CreateNewInstance, versionUnderTest);
            testVersion.Quit();
        }

        [Test]
        public void MultipleVersionsTest_WhenUsing2018Notation()
        {
            var versionUnderTest = _powerSHAPE.Version;
            var twenty17 = new PSAutomation(InstanceReuse.CreateNewInstance, new Version(17, 1, 0), new Version(17, 1, 99));
            Assert.That(twenty17.Version.Major, Is.EqualTo(17));
            twenty17.Quit();
            var twenty18 = new PSAutomation(InstanceReuse.CreateNewInstance, new Version(2018, 0, 0), new Version(2018, 0, 99));
            Assert.That(twenty18.Version.Major, Is.EqualTo(18));
            twenty18.Quit();

            // Ensure that the version under test is properly registered as the current version so other tests carry on ok...
            var testVersion = new PSAutomation(InstanceReuse.CreateNewInstance, versionUnderTest);
            testVersion.Quit();
        }

        [Test]
        public void MultipleVersionsTest_WhenUsing18Notation()
        {
            var versionUnderTest = _powerSHAPE.Version;
            var twenty17 = new PSAutomation(InstanceReuse.CreateNewInstance, new Version(17, 1, 0), new Version(17, 1, 99));
            Assert.That(twenty17.Version.Major, Is.EqualTo(17));
            twenty17.Quit();
            var twenty18 = new PSAutomation(InstanceReuse.CreateNewInstance, new Version(18, 1, 0), new Version(18, 1, 99));
            Assert.That(twenty18.Version.Major, Is.EqualTo(18));
            twenty18.Quit();

            // Ensure that the version under test is properly registered as the current version so other tests carry on ok...
            var testVersion = new PSAutomation(InstanceReuse.CreateNewInstance, versionUnderTest);
            testVersion.Quit();
        }

        /// <summary>
        /// Test for DrawingTolerance
        /// </summary>
        [Test]
        public void DrawingToleranceTest()
        {
            // Get the current drawwing tolerance
            var originalDrawingTolerance = _powerSHAPE.DrawingTolerance;
            Assert.IsFalse(originalDrawingTolerance == 0, "Drawing Tolerance is not being returned correctly");

            // Change the  drawing tolerance
            _powerSHAPE.DrawingTolerance = originalDrawingTolerance + 0.3F;
            Assert.IsTrue(_powerSHAPE.DrawingTolerance == originalDrawingTolerance + 0.3,
                          "Drawing Tolerance was not set correctly");
        }

        /// <summary>
        /// A test for Reset
        /// </summary>
        [Test]
        public void ResetTest()
        {
            _powerSHAPE.Models.Clear();
            Assert.AreEqual(0, _powerSHAPE.Models.Count);
            _powerSHAPE.Reset();
            Assert.AreEqual(1, _powerSHAPE.Models.Count);
        }

        [Test]
        public void ModelWithNumericNameTest()
        {
            try
            {
                _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.NUMERIC_NAMED_MODEL));
                var activeModel = _powerSHAPE.ActiveModel.Name;
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to handle model with numeric name: " + ex.Message);
            }
            Assert.Pass();
        }

        [Test]
        public void ArcFitPicFilesTest()
        {
            _powerSHAPE.ArcFitPicFiles = true;
            var curve = (PSCompCurve) _powerSHAPE.ActiveModel.Import(new File(TestFiles.PIC_FILE_FOR_ARCFIT))[0];
            Assert.That(curve.ToSpline().Count, Is.EqualTo(5));
        }

        [Test]
        public void DontArcFitPicFilesTest()
        {
            _powerSHAPE.ArcFitPicFiles = false;
            var curve = (PSCompCurve) _powerSHAPE.ActiveModel.Import(new File(TestFiles.PIC_FILE_FOR_ARCFIT))[0];
            Assert.That(curve.ToSpline().Count, Is.EqualTo(3));
        }

        [Test]
        public void CurveApplySmoothnessTest()
        {
            _powerSHAPE.IsCurveApplySmoothness = false;
            var curveWithoutSmoothness = _powerSHAPE.ActiveModel.Lines.CreateLine(new Point(0, 0, 0), new Point(50, 0, 0));
            var compCurveWithoutSmoothness = _powerSHAPE.ActiveModel.CompCurves.CreateCompCurveFromWireframe(new PSWireframe[] {curveWithoutSmoothness});
            compCurveWithoutSmoothness.RepointCurveBetweenPoints(0,1,5);
            compCurveWithoutSmoothness.EditPositionOfPointRelative(2,new Vector(0,10,0));
            var entryMagnitudeWithoutSmoothness = compCurveWithoutSmoothness.GetEntryMagnitudeOfPoint(2);

            _powerSHAPE.IsCurveApplySmoothness = true;
            var curveWithSmoothness = _powerSHAPE.ActiveModel.Lines.CreateLine(new Point(0, 0, 0), new Point(50, 0, 0));
            var compCurveWithSmoothness = _powerSHAPE.ActiveModel.CompCurves.CreateCompCurveFromWireframe(new PSWireframe[] { curveWithSmoothness });
            compCurveWithSmoothness.RepointCurveBetweenPoints(0, 1, 5);
            compCurveWithSmoothness.EditPositionOfPointRelative(2, new Vector(0, 10, 0));
            var entryMagnitudeWithSmoothness = compCurveWithSmoothness.GetEntryMagnitudeOfPoint(2);

            Assert.That(entryMagnitudeWithSmoothness, Is.Not.EqualTo(entryMagnitudeWithoutSmoothness));
        }

        #endregion

        #region User Tests

        [Test]
        public void UserLoginTest()
        {
            Assert.That(_powerSHAPE.UserLogin, Is.EqualTo(Environment.UserName));
        }

        [Test]
        public void UserNameTest()
        {
            Assert.That(_powerSHAPE.UserName,
                        Is.EqualTo(System.DirectoryServices.AccountManagement.UserPrincipal.Current.DisplayName));
        }

        #endregion

        #endregion
    }
}