// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using Autodesk.Geometry;
using Autodesk.ProductInterface.PowerMILL;
using Autodesk.ProductInterface.PowerMILLTest.Files;
using NUnit.Framework;

namespace Autodesk.ProductInterface.PowerMILLTest.PMEntityTests
{
    [TestFixture]
    public class PMToolTest
    {
        private PMAutomation _powerMill;
        private const double TOLERANCE = 0.00001;

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            _powerMill = new PMAutomation(InstanceReuse.UseExistingInstance);
            _powerMill.DialogsOff();
            _powerMill.CloseProject();
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
        }

        [SetUp]
        public void SetUp()
        {
            _powerMill.CloseProject();
            if (_powerMill.Version.Major >= 2018)
            {
                _powerMill.LoadProject(TestFiles.ToolProperties2018);
            }
            else
            {
                _powerMill.LoadProject(TestFiles.ToolProperties);
            }
        }

        [TearDown]
        public void TearDown()
        {
            try
            {
                _powerMill.CloseProject();
            }
            catch (Exception)
            {
            }
        }

        #region Properties tests

        [Test]
        public void NameTest()
        {
            var tool = _powerMill.ActiveProject.Tools[0];
            Assert.AreEqual("EndMill", tool.Name);
        }

        [Test]
        public void EndMillTest()
        {
            var tool = _powerMill.ActiveProject.Tools[0];
            Assert.NotNull(tool as PMToolEndMill);
        }

        [Test]
        public void BallNosedTest()
        {
            var tool = _powerMill.ActiveProject.Tools[1];
            Assert.NotNull(tool as PMToolBallNosed);
        }

        [Test]
        public void TipRadiusedTest()
        {
            var tool = _powerMill.ActiveProject.Tools[2];
            Assert.NotNull(tool as PMToolTipRadiused);
        }

        [Test]
        public void TaperedSphericalTest()
        {
            var tool = _powerMill.ActiveProject.Tools[3];
            Assert.NotNull(tool as PMToolTaperedSpherical);
        }

        [Test]
        public void TaperedTippedTest()
        {
            var tool = _powerMill.ActiveProject.Tools[4];
            Assert.NotNull(tool as PMToolTaperedTipped);
        }

        [Test]
        public void DrillTest()
        {
            var tool = _powerMill.ActiveProject.Tools[5];
            Assert.NotNull(tool as PMToolDrill);
        }

        [Test]
        public void TippedDiscTest()
        {
            var tool = _powerMill.ActiveProject.Tools[6];
            Assert.NotNull(tool as PMToolTippedDisc);
        }

        [Test]
        public void OffCentreTipRadiusedTest()
        {
            var tool = _powerMill.ActiveProject.Tools[7];
            Assert.NotNull(tool as PMToolOffCentreTipRadiused);
        }

        [Test]
        public void TapTest()
        {
            var tool = _powerMill.ActiveProject.Tools[8];
            Assert.NotNull(tool as PMToolTap);
        }

        [Test]
        public void ThreadMillTest()
        {
            var tool = _powerMill.ActiveProject.Tools[9];
            Assert.NotNull(tool as PMToolThreadMill);
        }

        [Test]
        public void FormTest()
        {
            var tool = _powerMill.ActiveProject.Tools[10];
            Assert.NotNull(tool as PMToolForm);
        }

        [Test]
        public void RoutingTest()
        {
            var tool = _powerMill.ActiveProject.Tools[11];
            Assert.NotNull(tool as PMToolRouting);
        }

        [Test]
        public void ProfilingTurning()
        {
            if (_powerMill.Version.Major >= 2018)
            {
                var tool = _powerMill.ActiveProject.Tools[13];
                Assert.NotNull(tool as PMToolProfilingTurning);
            }
            else
            {
                Assert.Pass();
            }
        }

        [Test]
        public void GroovingTurning()
        {
            if (_powerMill.Version.Major >= 2018)
            {
                var tool = _powerMill.ActiveProject.Tools[14];
                Assert.NotNull(tool as PMToolGroovingTurning);
            }
            else
            {
                Assert.Pass();
            }
        }

        [Test]
        public void LengthGetTest()
        {
            var tool = _powerMill.ActiveProject.Tools[0];
            Assert.AreEqual((MM) 10, tool.Length);
        }

        [Test]
        public void LengthSetTest()
        {
            var tool = _powerMill.ActiveProject.Tools[0];
            tool.Length = 15;
            Assert.AreEqual((MM) 15, tool.Length);
        }

        [Test]
        public void ToolNumberGetTest()
        {
            var tool = _powerMill.ActiveProject.Tools[0];
            Assert.AreEqual(25, tool.ToolNumber);
        }

        [Test]
        public void ToolNumberSetTest()
        {
            var tool = _powerMill.ActiveProject.Tools[0];
            tool.ToolNumber = 15;
            Assert.AreEqual(15, tool.ToolNumber);
        }

        [Test]
        public void NumberOfFlutesGetTest()
        {
            var tool = _powerMill.ActiveProject.Tools[0];
            Assert.AreEqual(3, tool.NumberOfFlutes);
        }

        [Test]
        public void NumberOfFlutesSetTest()
        {
            var tool = _powerMill.ActiveProject.Tools[0];
            tool.NumberOfFlutes = 15;
            Assert.AreEqual(15, tool.NumberOfFlutes);
        }

        [Test]
        public void DiameterGetTest()
        {
            var tool = _powerMill.ActiveProject.Tools[0];
            Assert.AreEqual((MM) 5.0, tool.Diameter);
        }

        [Test]
        public void DiameterSetTest()
        {
            var tool = _powerMill.ActiveProject.Tools[0];
            tool.Diameter = 8.2;
            Assert.AreEqual((MM) 8.2, tool.Diameter);
        }

        [Test]
        public void OverhangGetTest()
        {
            var tool = _powerMill.ActiveProject.Tools[0];
            Assert.AreEqual((MM) 7.1, tool.Overhang);
        }

        [Test]
        public void OverhangSetTest()
        {
            var tool = _powerMill.ActiveProject.Tools[0];
            tool.Overhang = 5.6;
            Assert.AreEqual((MM) 5.6, tool.Overhang);
        }

        [Test]
        public void DescriptionReadTest()
        {
            var tool = _powerMill.ActiveProject.Tools[0];
            Assert.AreEqual("1234\r\n4567", tool.Description);
        }

        [Test]
        public void DescriptionEmptyTest()
        {
            var tool = _powerMill.ActiveProject.Tools[1];
            Assert.AreEqual(String.Empty, tool.Description);
        }

        [Test]
        public void DescriptionTest()
        {
            var tool = _powerMill.ActiveProject.Tools[1];
            tool.Description = "This is my description";
            Assert.AreEqual("This is my description", tool.Description);
        }

        [Test]
        public void HolderNameTest()
        {
            var tool = _powerMill.ActiveProject.Tools[0];
            tool.HolderName = "Dave";
            Assert.AreEqual("Dave", tool.HolderName);
        }

        [Test]
        public void CoolantNoneTest()
        {
            var tool = _powerMill.ActiveProject.Tools[0];
            tool.Coolant = CoolantOptions.None;
            Assert.AreEqual(CoolantOptions.None, tool.Coolant);
        }

        [Test]
        public void CoolantStandardTest()
        {
            var tool = _powerMill.ActiveProject.Tools[0];
            tool.Coolant = CoolantOptions.Standard;
            Assert.AreEqual(CoolantOptions.Standard, tool.Coolant);
        }

        [Test]
        public void CoolantFloodTest()
        {
            var tool = _powerMill.ActiveProject.Tools[0];
            tool.Coolant = CoolantOptions.Flood;
            Assert.AreEqual(CoolantOptions.Flood, tool.Coolant);
        }

        [Test]
        public void CoolantMistTest()
        {
            var tool = _powerMill.ActiveProject.Tools[0];
            tool.Coolant = CoolantOptions.Mist;
            Assert.AreEqual(CoolantOptions.Mist, tool.Coolant);
        }

        [Test]
        public void CoolantTapTest()
        {
            var tool = _powerMill.ActiveProject.Tools[0];
            tool.Coolant = CoolantOptions.Tap;
            Assert.AreEqual(CoolantOptions.Tap, tool.Coolant);
        }

        [Test]
        public void CoolantAirTest()
        {
            var tool = _powerMill.ActiveProject.Tools[0];
            tool.Coolant = CoolantOptions.Air;
            Assert.AreEqual(CoolantOptions.Air, tool.Coolant);
        }

        [Test]
        public void CoolantThroughTest()
        {
            var tool = _powerMill.ActiveProject.Tools[0];
            tool.Coolant = CoolantOptions.Through;
            Assert.AreEqual(CoolantOptions.Through, tool.Coolant);
        }

        [Test]
        public void CoolantDoubleTest()
        {
            var tool = _powerMill.ActiveProject.Tools[0];
            tool.Coolant = CoolantOptions.Double;
            Assert.AreEqual(CoolantOptions.Double, tool.Coolant);
        }

        [Test]
        public void PitchTest()
        {
            var tool = (PMToolThreadMill) _powerMill.ActiveProject.Tools[9];
            tool.Pitch = 1.0;
            Assert.AreEqual((MM) 1.0, tool.Pitch);
        }

        [Test]
        public void TipRadiusTest()
        {
            var tool = (PMToolTippedDisc) _powerMill.ActiveProject.Tools[6];            
            tool.LowerTipRadius = 1.5;
            tool.UpperTipRadius = 1.0;
            Assert.AreEqual((MM)1.5, tool.LowerTipRadius);
            Assert.AreEqual((MM)1.0, tool.UpperTipRadius);
        }

        [Test]
        public void TaperAngleTest()
        {
            var tool = (PMToolTaperedTipped) _powerMill.ActiveProject.Tools[4];
            tool.TaperAngle = 1.0;
            Assert.AreEqual((Degree) 1.0, tool.TaperAngle);
        }

        [Test]
        public void DrillTaperAngleTest()
        {
            var tool = (PMToolDrill) _powerMill.ActiveProject.Tools[5];
            tool.TaperAngle = 1.0;
            Assert.AreEqual((Degree) 1.0, tool.TaperAngle);
        }

        [Test]
        public void TaperHeightTest()
        {
            var tool = (PMToolTaperedTipped) _powerMill.ActiveProject.Tools[4];
            tool.TaperHeight = 1.0;
            Assert.AreEqual((MM) 1.0, tool.TaperHeight);
        }

        [Test]
        public void NumberOfShankElementsTest()
        {
            if (_powerMill.Version < new Version("15.0"))
            {
                Assert.Inconclusive("Test not available for this version of PowerMILL");
            }
            var tool = _powerMill.ActiveProject.Tools[12];
            Assert.AreEqual(3, tool.NumberOfShankElements);
        }

        [Test]
        public void ShankElementUpperDiameterTest()
        {
            if (_powerMill.Version < new Version("15.0"))
            {
                Assert.Inconclusive("Test not available for this version of PowerMILL");
            }
            var tool = _powerMill.ActiveProject.Tools[12];
            Assert.AreEqual((MM) 15.0, tool.ShankElementUpperDiameter(2));
        }

        [Test]
        public void ShankElementLowerDiameterTest()
        {
            if (_powerMill.Version < new Version("15.0"))
            {
                Assert.Inconclusive("Test not available for this version of PowerMILL");
            }
            var tool = _powerMill.ActiveProject.Tools[12];
            Assert.AreEqual((MM) 10.0, tool.ShankElementLowerDiameter(1));
        }

        [Test]
        public void ShankElementLengthTest()
        {
            if (_powerMill.Version < new Version("15.0"))
            {
                Assert.Inconclusive("Test not available for this version of PowerMILL");
            }
            var tool = _powerMill.ActiveProject.Tools[12];
            Assert.AreEqual((MM) 5.0, tool.ShankElementLength(0));
        }

        [Test]
        public void ShankElementOutOfBoundsTest()
        {
            if (_powerMill.Version < new Version("15.0"))
            {
                Assert.Inconclusive("Test not available for this version of PowerMILL");
            }
            var tool = _powerMill.ActiveProject.Tools[12];
            Assert.Throws<IndexOutOfRangeException>(() => tool.ShankElementLength(5));
        }

        [Test]
        public void NumberOfHolderElementsTest()
        {
            if (_powerMill.Version < new Version("15.0"))
            {
                Assert.Inconclusive("Test not available for this version of PowerMILL");
            }
            var tool = _powerMill.ActiveProject.Tools[12];
            Assert.AreEqual(1, tool.NumberOfHolderElements);
        }

        [Test]
        public void HolderElementUpperDiameterTest()
        {
            if (_powerMill.Version < new Version("15.0"))
            {
                Assert.Inconclusive("Test not available for this version of PowerMILL");
            }
            var tool = _powerMill.ActiveProject.Tools[12];
            Assert.AreEqual((MM) 25.0, tool.HolderElementUpperDiameter(0));
        }

        [Test]
        public void HolderElementLowerDiameterTest()
        {
            if (_powerMill.Version < new Version("15.0"))
            {
                Assert.Inconclusive("Test not available for this version of PowerMILL");
            }
            var tool = _powerMill.ActiveProject.Tools[12];
            Assert.AreEqual((MM) 13.0, tool.HolderElementLowerDiameter(0));
        }

        [Test]
        public void HolderElementLengthTest()
        {
            if (_powerMill.Version < new Version("15.0"))
            {
                Assert.Inconclusive("Test not available for this version of PowerMILL");
            }
            var tool = _powerMill.ActiveProject.Tools[12];
            Assert.AreEqual((MM) 50.0, tool.HolderElementLength(0));
        }

        [Test]
        public void HolderElementOutOfBoundsTest()
        {
            if (_powerMill.Version < new Version("15.0"))
            {
                Assert.Inconclusive("Test not available for this version of PowerMILL");
            }
            var tool = _powerMill.ActiveProject.Tools[12];
            Assert.Throws<IndexOutOfRangeException>(() => tool.HolderElementLength(5));
        }

        [Test]
        public void HolderOverHangTest()
        {
            if (_powerMill.Version < new Version("15.0"))
            {
                Assert.Inconclusive("Test not available for this version of PowerMILL");
            }
            var tool = _powerMill.ActiveProject.Tools[12];
            Assert.AreEqual((MM) 45.0, tool.Overhang);
        }

        [Test]
        public void TaperDiameterTest()
        {
            if (_powerMill.Version < new Version("15.0"))
            {
                Assert.Inconclusive("Test not available for this version of PowerMILL");
            }
            var tool = (PMToolTaperedTipped) _powerMill.ActiveProject.Tools[4];
            Assert.That(tool.TaperDiameter.Value, Is.EqualTo(4.860317).Within(TOLERANCE));
        }

        #endregion

        #region Test operations

        [Test]
        public void DrawAndUndrawAllToolsTest()
        {
            _powerMill.LoadProject(TestFiles.ToolProperties);
            _powerMill.ActiveProject.Tools.DrawAll();
            _powerMill.ActiveProject.Tools.UndrawAll();
            Assert.IsTrue(true);
        }

        #endregion

    }
}