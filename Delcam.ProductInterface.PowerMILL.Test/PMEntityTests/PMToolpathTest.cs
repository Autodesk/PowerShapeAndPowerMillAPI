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
using System.Linq;
using Autodesk.Geometry;
using Autodesk.ProductInterface.PowerMILL;
using Autodesk.ProductInterface.PowerMILLTest.Files;
using NUnit.Framework;

namespace Autodesk.ProductInterface.PowerMILLTest
{
    [TestFixture]
    public partial class PMToolpathTest
    {
        private PMAutomation _powerMill;
        private double TOLERANCE = 0.00001;

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            _powerMill = new PMAutomation(InstanceReuse.UseExistingInstance);
            _powerMill.DialogsOff();
            _powerMill.CloseProject();
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

        #region Test properties

        [Test]
        public void IsActiveTest()
        {
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var toolpath = _powerMill.ActiveProject.Toolpaths.First();
            Assert.IsFalse(toolpath.IsActive);
            toolpath.IsActive = true;
            Assert.IsTrue(toolpath.IsActive);
        }

        #endregion

        #region Test operations

        [Test]
        public void Calculate()
        {
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var toolpath = _powerMill.ActiveProject.Toolpaths.First();
            toolpath.MakeInvalid();
            Assert.IsFalse(toolpath.IsCalculated, "Expected toolpath 'Rough-U_1' to be uncalculated.");
            toolpath.Calculate();
            Assert.IsTrue(toolpath.IsCalculated, "Expected toolpath to be calculated.");
        }

        [Test]
        public void MakeInvalid()
        {
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var toolpath = _powerMill.ActiveProject.Toolpaths.First();
            toolpath.MakeInvalid();
            Assert.IsFalse(toolpath.IsCalculated, "Expected toolpath 'Rough-U_1' to be uncalculated.");
        }

        [Test]
        public void ToolSafetyReportTest()
        {
            _powerMill.LoadProject(TestFiles.SimplePmProject1);
            var toolpath = _powerMill.ActiveProject.Toolpaths["brava"];

            var numberOfToolpathsBeforeRunningSafety = _powerMill.ActiveProject.Toolpaths.Count;
            var report = toolpath.SafetyReport();
            var numberOfToolpathsAfterRunningSafety = _powerMill.ActiveProject.Toolpaths.Count;
            var lines = report.Replace("\r\n", "\n").Split('\n').ToList();

            Assert.AreEqual("Tool Cutting Moves: Safe", lines[0]);
            Assert.AreEqual("Tool Links: Safe", lines[1]);
            Assert.AreEqual("Tool Leads: Safe", lines[2]);
            Assert.AreEqual("Holder Cutting Moves: Not Checked", lines[3]);
            Assert.AreEqual("Holder Links: Not Checked", lines[4]);
            Assert.AreEqual("Holder Leads: Not Checked", lines[5]);
            Assert.AreEqual(ToolpathSafety.Safe, toolpath.ToolCuttingMovesSafety);
            Assert.AreEqual(ToolpathSafety.Safe, toolpath.ToolLinksSafety);
            Assert.AreEqual(ToolpathSafety.Safe, toolpath.ToolLeadsSafety);
            Assert.AreEqual(ToolpathSafety.NotChecked, toolpath.HolderCuttingMovesSafety);
            Assert.AreEqual(ToolpathSafety.NotChecked, toolpath.HolderLinksSafety);
            Assert.AreEqual(ToolpathSafety.NotChecked, toolpath.HolderLeadsSafety);
            Assert.AreEqual(numberOfToolpathsBeforeRunningSafety, numberOfToolpathsAfterRunningSafety);
        }

        [Test]
        public void HolderSafetyReport()
        {
            _powerMill.LoadProject(TestFiles.SimplePmProject1);
            var toolpath = _powerMill.ActiveProject.Toolpaths["brava"];
            var numberOfToolpathsBeforeRunningSafety = _powerMill.ActiveProject.Toolpaths.Count;
            var report = toolpath.HolderSafetyReport();
            var numberOfToolpathsAfterRunningSafety = _powerMill.ActiveProject.Toolpaths.Count;
            var lines = report.Replace("\r\n", "\n").Split('\n').ToList();

            Assert.AreEqual("Holder Cutting Moves: Not Checked", lines[0]);
            Assert.AreEqual("Holder Links: Not Checked", lines[1]);
            Assert.AreEqual("Holder Leads: Not Checked", lines[2]);
            Assert.AreEqual(ToolpathSafety.NotChecked, toolpath.HolderCuttingMovesSafety);
            Assert.AreEqual(ToolpathSafety.NotChecked, toolpath.HolderLinksSafety);
            Assert.AreEqual(ToolpathSafety.NotChecked, toolpath.HolderLeadsSafety);
            Assert.AreEqual(numberOfToolpathsBeforeRunningSafety, numberOfToolpathsAfterRunningSafety);

            _powerMill.CloseProject();

            // 2nd project will fail.
            _powerMill.LoadProject(TestFiles.SimplePmProjectWithGouges1, true);

            var tpFail = _powerMill.ActiveProject.Toolpaths[0];

            report = tpFail.HolderSafetyReport();
            lines = report.Replace("\r\n", "\n").Split('\n').ToList();

            Assert.AreEqual("Holder Cutting Moves: Collides", lines[0]);
            Assert.AreEqual("Holder Links: Collides", lines[1]);
            Assert.AreEqual("Holder Leads: Safe", lines[2]);
            Assert.AreEqual(ToolpathSafety.Collides, tpFail.HolderCuttingMovesSafety);
            Assert.AreEqual(ToolpathSafety.Collides, tpFail.HolderLinksSafety);
            Assert.AreEqual(ToolpathSafety.Safe, tpFail.HolderLeadsSafety);
        }

        [Test]
        public void CheckGougesTest()
        {
            // default project should not gouge.
            _powerMill.LoadProject(TestFiles.SimplePmProject1);
            var toolpath = _powerMill.ActiveProject.Toolpaths["brava"];
            var numberOfToolpathsBeforeRunningGougeCheck = _powerMill.ActiveProject.Toolpaths.Count;
            Assert.False(toolpath.DetectToolGouges(), "Should not have gouges in");
            var numberOfToolpathsAfterRunningGougeCheck = _powerMill.ActiveProject.Toolpaths.Count;

            Assert.AreEqual(numberOfToolpathsBeforeRunningGougeCheck, numberOfToolpathsAfterRunningGougeCheck);
            _powerMill.CloseProject();

            // 2nd project will fail.
            _powerMill.LoadProject(TestFiles.SimplePmProjectWithGouges1, true);
            var tpFail = _powerMill.ActiveProject.Toolpaths[0];
            Assert.True(tpFail.DetectToolGouges(), "Should have gouges in");
        }

        [Test]
        public void CheckToolHolderCollisions()
        {
            // default project should not gouge.
            _powerMill.LoadProject(TestFiles.SimplePmProject1);
            var toolpath = _powerMill.ActiveProject.Toolpaths["brava"];
            var numberOfToolpathsBeforeRunningHolderCollisions = _powerMill.ActiveProject.Toolpaths.Count;
            Assert.False(toolpath.DetectHolderCollisions(), "Should not have tool holder collisions in");
            var numberOfToolpathsAfterRunningHolderCollisions = _powerMill.ActiveProject.Toolpaths.Count;

            Assert.AreEqual(numberOfToolpathsBeforeRunningHolderCollisions,
                            numberOfToolpathsAfterRunningHolderCollisions);
            _powerMill.CloseProject();

            // 2nd project will fail.
            _powerMill.LoadProject(TestFiles.SimplePmProjectWithGouges1, true);
            Assert.Greater(_powerMill.ActiveProject.Toolpaths.Count, 0, "No toolpaths in project");
            var tpFail = _powerMill.ActiveProject.Toolpaths[0];
            Assert.True(tpFail.DetectHolderCollisions(), "Should have collision in it");
        }

        [Test]
        public void CuttingLength()
        {
            _powerMill.LoadProject(TestFiles.SimplePmProjectWithGouges1, true);
            var toolpath = _powerMill.ActiveProject.Toolpaths[0];
            Assert.IsTrue(toolpath.TotalCutLength == 160);
        }
        
        [Test]
        public void ToolPathStrategiesTests()
        {
            var toolpathStrategies = new List<Type>
            {
                typeof(PMToolpathCurveAreaClearance),
                typeof(PMToolpathCurveProfile),
                typeof(PMToolpathChamferMilling),
                typeof(PMToolpathFaceMilling),
                typeof(PMToolpathOffsetAreaClearance),
                typeof(PMToolpathProfileAreaClearance),
                typeof(PMToolpathRasterAreaClearance),
                typeof(PMToolpathCornerClearance),
                typeof(PMToolpathPlungeMilling),
                typeof(PMToolpathBladeFinishing),
                typeof(PMToolpathBliskAreaClearance),
                typeof(PMToolpathHubFinishing),
                typeof(PMToolpathMethod),
                typeof(PMToolpathDrilling),
                typeof(PMToolpath3DOffsetFinishing),
                typeof(PMToolpathConstantZFinishing),
                typeof(PMToolpathCornerFinishing),
                typeof(PMToolpathMultiPencilCornerFinishing),
                typeof(PMToolpathPencilCornerFinishing),
                typeof(PMToolpathDiscProfileFinishing),
                typeof(PMToolpathEmbeddedPatternFinishing),
                typeof(PMToolpathFlowlineFinishing),
                typeof(PMToolpathOffsetFlatFinishing),
                typeof(PMToolpathOptimisedConstantZFinishing),
                typeof(PMToolpathParametricOffsetFinishing),
                typeof(PMToolpathParametricSpiralFinishing),
                typeof(PMToolpathPatternFinishing),
                typeof(PMToolpathProfileFinishing),
                typeof(PMToolpathCurveProjectionFinishing),
                typeof(PMToolpathLineProjectionFinishing),
                typeof(PMToolpathPlaneProjectionFinishing),
                typeof(PMToolpathPointProjectionFinishing),
                typeof(PMToolpathSurfaceProjectionFinishing),
                typeof(PMToolpathRadialFinishing),
                typeof(PMToolpathRasterFinishing),
                typeof(PMToolpathRasterFlatFinishing),
                typeof(PMToolpathRotaryFinishing),
                typeof(PMToolpathSpiralFinishing),
                typeof(PMToolpathSteepAndShallowFinishing),
                typeof(PMToolpathSurfaceFinishing),
                typeof(PMToolpathSwarfMachining),
                typeof(PMToolpathWireframeProfileMachining),
                typeof(PMToolpathWireframeSwarfMachining),
                typeof(PMToolpathPortAreaClearance),
                typeof(PMToolpathPortPlungeFinishing),
                typeof(PMToolpathPortSpiralFinishing)
            };
            _powerMill.LoadProject(TestFiles.ToolPathStrategies);

            CollectionAssert.AllItemsAreNotNull(_powerMill.ActiveProject.Toolpaths);
            CollectionAssert.AreEquivalent(_powerMill.ActiveProject.Toolpaths.Select(t => t.GetType()), toolpathStrategies);
        }

        [Test]
        public void StartPointSetCoordinatesTest()
        {
            var rand = new Random();
            var toolpath = _powerMill.ActiveProject.Toolpaths.CreateAdaptiveAreaClearanceToolpath();
            var x = rand.NextDouble();
            var y = rand.NextDouble();
            var z = rand.NextDouble();

            toolpath.SetStartPointMethod(StartPointMethod.Absolute, x, y, z);

            Assert.That(toolpath.StartPointMethod, Is.EqualTo(StartPointMethod.Absolute));
            Assert.That(toolpath.StartPointPositionX, Is.EqualTo(x).Within(TOLERANCE));
            Assert.That(toolpath.StartPointPositionY, Is.EqualTo(y).Within(TOLERANCE));
            Assert.That(toolpath.StartPointPositionZ, Is.EqualTo(z).Within(TOLERANCE));
        }

        [Test]
        public void StartPointSetCoordinatesPointTest()
        {
            var rand = new Random();
            var toolpath = _powerMill.ActiveProject.Toolpaths.CreateAdaptiveAreaClearanceToolpath();
            var point = new Point(rand.NextDouble(), rand.NextDouble(), rand.NextDouble());

            toolpath.SetStartPointMethod(StartPointMethod.Absolute, point);

            Assert.That(toolpath.StartPointMethod, Is.EqualTo(StartPointMethod.Absolute));
            Assert.That(toolpath.StartPointPositionX, Is.EqualTo(point.X.Value).Within(TOLERANCE));
            Assert.That(toolpath.StartPointPositionY, Is.EqualTo(point.Y.Value).Within(TOLERANCE));
            Assert.That(toolpath.StartPointPositionZ, Is.EqualTo(point.Z.Value).Within(TOLERANCE));
        }

        [Test]
        public void StartPointSetCoordinatesPointWhileToolPathIsCalculatedTest()
        {
            var rand = new Random();
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var toolpath = _powerMill.ActiveProject.Toolpaths.First();
            var point = new Point(rand.NextDouble(), rand.NextDouble(), rand.NextDouble());

            toolpath.SetStartPointMethod(StartPointMethod.Absolute, point);

            Assert.That(toolpath.StartPointMethod, Is.EqualTo(StartPointMethod.Absolute));
            Assert.That(toolpath.StartPointPositionX, Is.EqualTo(point.X.Value).Within(TOLERANCE));
            Assert.That(toolpath.StartPointPositionY, Is.EqualTo(point.Y.Value).Within(TOLERANCE));
            Assert.That(toolpath.StartPointPositionZ, Is.EqualTo(point.Z.Value).Within(TOLERANCE));
        }

        [Test]
        public void EndPointSetCoordinatesTest()
        {
            var rand = new Random();
            var toolpath = _powerMill.ActiveProject.Toolpaths.CreateAdaptiveAreaClearanceToolpath();
            var x = rand.NextDouble();
            var y = rand.NextDouble();
            var z = rand.NextDouble();

            toolpath.SetEndPointMethod(EndPointMethod.Absolute, x, y, z);

            Assert.That(toolpath.EndPointMethod, Is.EqualTo(EndPointMethod.Absolute));
            Assert.That(toolpath.EndPointPositionX, Is.EqualTo(x).Within(TOLERANCE));
            Assert.That(toolpath.EndPointPositionY, Is.EqualTo(y).Within(TOLERANCE));
            Assert.That(toolpath.EndPointPositionZ, Is.EqualTo(z).Within(TOLERANCE));
        }

        [Test]
        public void EndPointSetCoordinatesPointTest()
        {
            var rand = new Random();
            var toolpath = _powerMill.ActiveProject.Toolpaths.CreateAdaptiveAreaClearanceToolpath();
            var point = new Point(rand.NextDouble(), rand.NextDouble(), rand.NextDouble());

            toolpath.SetEndPointMethod(EndPointMethod.Absolute, point);

            Assert.That(toolpath.EndPointMethod, Is.EqualTo(EndPointMethod.Absolute));
            Assert.That(toolpath.EndPointPositionX, Is.EqualTo(point.X.Value).Within(TOLERANCE));
            Assert.That(toolpath.EndPointPositionY, Is.EqualTo(point.Y.Value).Within(TOLERANCE));
            Assert.That(toolpath.EndPointPositionZ, Is.EqualTo(point.Z.Value).Within(TOLERANCE));
        }

        [Test]
        public void EndPointSetCoordinatesPointWhileToolPathIsCalculatedTest()
        {
            var rand = new Random();
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var toolpath = _powerMill.ActiveProject.Toolpaths.First();
            var point = new Point(rand.NextDouble(), rand.NextDouble(), rand.NextDouble());

            toolpath.SetEndPointMethod(EndPointMethod.Absolute, point);

            Assert.That(toolpath.EndPointMethod, Is.EqualTo(EndPointMethod.Absolute));
            Assert.That(toolpath.EndPointPositionX, Is.EqualTo(point.X.Value).Within(TOLERANCE));
            Assert.That(toolpath.EndPointPositionY, Is.EqualTo(point.Y.Value).Within(TOLERANCE));
            Assert.That(toolpath.EndPointPositionZ, Is.EqualTo(point.Z.Value).Within(TOLERANCE));
        }

        [Test]
        public void ConnectionTest()
        {
            // this test would be better if it was split into many small tests,
            // but it isn't testing doing any logic, it test concatenating the right strings
            var rand = new Random();
            var angle = rand.NextDouble();
            var distance = rand.Next();

            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var connections = _powerMill.ActiveProject.Toolpaths.First().Connections;
            connections.AddLeadsAtToolAxisDiscontinuities = true;
            connections.AllowStartPointToBeMoved = true;
            connections.AngularThreshold = angle;
            connections.ApproachDistance = distance;
            connections.ArcFitRadius = angle;
            connections.ArcFitRapidMove = true;
            connections.AutomaticallyExtend = true;
            connections.MaxMoveExtension = distance;
            connections.OverlapDistance = distance;
            connections.RetractDistance = distance;
            connections.GougeCheck = true;

            Assert.That(connections.AddLeadsAtToolAxisDiscontinuities, Is.True, "AddLeadsAtToolAxisDiscontinuities not been set");
            Assert.That(connections.AllowStartPointToBeMoved, Is.True, "AllowStartPointToBeMoved not been set");
            Assert.That(connections.AngularThreshold.Value, Is.EqualTo(angle).Within(TOLERANCE), "AngularThreshold not been set");
            Assert.That(connections.ApproachDistance.Value,
                        Is.EqualTo(distance).Within(TOLERANCE),
                        "ApproachDistance not been set");
            Assert.That(connections.ArcFitRadius.Value, Is.EqualTo(angle).Within(TOLERANCE), "ArcFitRadius not been set");
            Assert.That(connections.ArcFitRapidMove, Is.True, "ArcFitRapidMove not been set");
            Assert.That(connections.AutomaticallyExtend, Is.True, "AutomaticallyExtend not been set");
            Assert.That(connections.MaxMoveExtension.Value,
                        Is.EqualTo(distance).Within(TOLERANCE),
                        "MaxMoveExtension not been set");
            Assert.That(connections.OverlapDistance.Value,
                        Is.EqualTo(distance).Within(TOLERANCE),
                        "OverlapDistance not been set");
            Assert.That(connections.RetractDistance.Value,
                        Is.EqualTo(distance).Within(TOLERANCE),
                        "RetractDistance not been set");
            Assert.That(connections.GougeCheck, Is.True, "GougeCheck not been set");
        }

        [Test]
        public void ConnectionForPowerMill2016Test()
        {
            if (_powerMill.Version.Major > PMEntity.POWER_MILL_2016.Major)
            {
                Console.WriteLine(@"AddLeadsToShortLinks and ShortLinksToLongLinkThreshold is not support in 2017");
                return;
            }
            var distance = new Random().Next();
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var connections = _powerMill.ActiveProject.Toolpaths.First().Connections;

            connections.AddLeadsToShortLinks = true;
            connections.ShortLinkToLongLinkThreshold = distance;

            Assert.That(connections.AddLeadsToShortLinks, Is.True, "AddLeadsToShortLinks not been set");
            Assert.That(connections.ShortLinkToLongLinkThreshold.Value,
                        Is.EqualTo(distance).Within(TOLERANCE),
                        "ShortLinkToLongLinkThreshold not been set");
        }

        [Test]
        public void ConnectionForPowerMill2017Test()
        {
            if (_powerMill.Version.Major < PMEntity.POWER_MILL_2017.Major)
            {
                Console.WriteLine(@"Link1st to Link5th is not support in 2016");
                return;
            }
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var connections = _powerMill.ActiveProject.Toolpaths.First().Connections;
            foreach (var linkType in Enum.GetValues(typeof(LinkTypes)).Cast<LinkTypes>())
            {
                connections.Link1st = linkType;
                connections.Link2nd = linkType;
                connections.Link3rd = linkType;
                connections.Link4th = linkType;
                connections.Link5th = linkType;

                Assert.That(connections.Link1st, Is.EqualTo(linkType), "Link1st not been set");
                Assert.That(connections.Link2nd, Is.EqualTo(linkType), "Link2nd not been set");
                Assert.That(connections.Link3rd, Is.EqualTo(linkType), "Link3rd not been set");
                Assert.That(connections.Link4th, Is.EqualTo(linkType), "Link4th not been set");
                Assert.That(connections.Link5th, Is.EqualTo(linkType), "Link5th not been set");
            }
        }

        [Test]
        public void RetractMoveTypesTest()
        {
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var connections = _powerMill.ActiveProject.Toolpaths.First().Connections;
            foreach (var moveType in typeof(MoveDirection).GetEnumValues())
            {
                connections.RetractAndApproachMoves = (MoveDirection) moveType;
                Assert.That(connections.RetractAndApproachMoves, Is.EqualTo(moveType), "RetractAndApproachMoves not been set");
            }
        }

        [Test]
        public void DefaultLinkTypesTest()
        {
            if (_powerMill.Version.Major > PMEntity.POWER_MILL_2016.Major)
            {
                Console.WriteLine(@"DefaultLink and LongLink is not supportted in 2017");
                return;
            }

            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var connections = _powerMill.ActiveProject.Toolpaths.First().Connections;
            foreach (var linkType in typeof(DefaultLinkTypes).GetEnumValues())
            {
                connections.DefaultLink = (DefaultLinkTypes) linkType;
                connections.DefaultLinkSecond = (DefaultLinkTypes) linkType;
                connections.LongLink = (DefaultLinkTypes) linkType;
                connections.LongLinkSecond = (DefaultLinkTypes) linkType;
                Assert.That(connections.DefaultLink, Is.EqualTo(linkType), "DefaultLink not been set");
                Assert.That(connections.DefaultLink, Is.EqualTo(linkType), "DefaultLink not been set");
                Assert.That(connections.DefaultLinkSecond, Is.EqualTo(linkType), "DefaultLinkSecond not been set");
                Assert.That(connections.LongLink, Is.EqualTo(linkType), "LongLink not been set");
                Assert.That(connections.LongLinkSecond, Is.EqualTo(linkType), "LongLinkSecond not been set");
            }
        }

        [Test]
        public void ShortLinkTypesTest()
        {
            if (_powerMill.Version.Major > PMEntity.POWER_MILL_2016.Major)
            {
                Console.WriteLine(@"ShortLink is not supportted in 2017");
                return;
            }
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var connections = _powerMill.ActiveProject.Toolpaths.First().Connections;
            foreach (var linkType in typeof(LinkTypes).GetEnumValues().Cast<LinkTypes>())
            {
                connections.ShortLink = linkType;
                connections.ShortLinkSecond = linkType;
                Assert.That(connections.ShortLink, Is.EqualTo(linkType), "ShortLink not been set");
                Assert.That(connections.ShortLink, Is.EqualTo(linkType), "ShortLinkSecond not been set");
            }
        }

        [Test]
        public void NumberOfSegmentsTest()
        {
            _powerMill.LoadProject(TestFiles.SimplePmProject1);
            Assert.That(_powerMill.ActiveProject.Toolpaths[0].NumberOfSegments, Is.EqualTo(101));
        }

        [Test]
        public void SegmentLengthTest()
        {
            _powerMill.LoadProject(TestFiles.SimplePmProject1);
            Assert.That(_powerMill.ActiveProject.Toolpaths[0].SegmentLength(0), Is.EqualTo((MM) 215.627867));
        }

        [Test]
        public void SegmentPointsTest()
        {
            _powerMill.LoadProject(TestFiles.SimplePmProject1);
            Assert.That(_powerMill.ActiveProject.Toolpaths[0].NumberOfPointsInSegment(0), Is.EqualTo(19));
        }

        [Test]
        public void ToolpathPointPositionTest()
        {
            _powerMill.LoadProject(TestFiles.SimplePmProject1);
            Assert.That(_powerMill.ActiveProject.Toolpaths[0].ToolpathPointPosition(0, 0),
                        Is.EqualTo(new Point(-60.008766, -49.997471, 0)));
        }

        [Test]
        public void ToolpathPointToolAxisTest()
        {
            _powerMill.LoadProject(TestFiles.SimplePmProject1);
            Assert.That(_powerMill.ActiveProject.Toolpaths[0].ToolpathPointToolAxis(0, 0), Is.EqualTo(new Vector(0, 0, 1)));
        }

        [Test]
        public void ToolpathPointOrientationVectorTest()
        {
            _powerMill.LoadProject(TestFiles.SimplePmProject1);
            Assert.That(_powerMill.ActiveProject.Toolpaths[0].ToolpathPointOrientationVector(0, 0), Is.EqualTo(new Vector()));
        }

        [Test]
        public void NumberOfLeadsTest()
        {
            _powerMill.LoadProject(TestFiles.SimplePmProject1);
            Assert.That(_powerMill.ActiveProject.Toolpaths[0].NumberOfLeads, Is.EqualTo(0));
        }

        [Test]
        public void NumberOfLinksTest()
        {
            _powerMill.LoadProject(TestFiles.SimplePmProject1);
            Assert.That(_powerMill.ActiveProject.Toolpaths[0].NumberOfLinks, Is.EqualTo(100));
        }

        [Test]
        public void LeadAndLinkRapidTimeTest()
        {
            _powerMill.LoadProject(TestFiles.SimplePmProject1);
            Assert.That(_powerMill.ActiveProject.Toolpaths[0].LeadsAndLinksRapidTime, Is.EqualTo(TimeSpan.Parse("00:09:23.7610000")));
        }
        [Test]
        public void TotalCuttingPlungeTest()
        {
            _powerMill.LoadProject(TestFiles.SimplePmProject1);
            Assert.That(_powerMill.ActiveProject.Toolpaths[0].LeadsAndLinksPlungeTime, Is.EqualTo(TimeSpan.Parse("00:07:04.2000000")));
        }
        [Test]
        public void LeadsAndLinksRampTimeTest()
        {
            _powerMill.LoadProject(TestFiles.SimplePmProject1);
            Assert.That(_powerMill.ActiveProject.Toolpaths[0].LeadsAndLinksRampTime, Is.EqualTo(TimeSpan.Parse("00:00:00")));
        }
        [Test]
        public void LeadsAndLinksOthersTimeTest()
        {
            _powerMill.LoadProject(TestFiles.SimplePmProject1);
            Assert.That(_powerMill.ActiveProject.Toolpaths[0].LeadsAndLinksOthersTime, Is.EqualTo(TimeSpan.Parse("00:00:00")));
        }
        [Test]
        public void LinearCuttingTimeTest()
        {
            _powerMill.LoadProject(TestFiles.SimplePmProject1);
            Assert.That(_powerMill.ActiveProject.Toolpaths[0].LinearCuttingTime, Is.EqualTo(TimeSpan.Parse("00:23:27.9290000")));
        }
        [Test]
        public void ArcCuttingTimeTest()
        {
            _powerMill.LoadProject(TestFiles.SimplePmProject1);
            Assert.That(_powerMill.ActiveProject.Toolpaths[0].ArcCuttingTime, Is.EqualTo(TimeSpan.Parse("00:00:00")));
        }

        [Test]
        public void TotalCuttingTimeTest()
        {
            _powerMill.LoadProject(TestFiles.SimplePmProject1);
            Assert.That(_powerMill.ActiveProject.Toolpaths[0].TotalCuttingTime, Is.EqualTo(TimeSpan.Parse("00:39:55.8900000")));
        }

        [Test]
        public void BlockBoundingBoxTest()
        {
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var boundingBoxToFind = new BoundingBox(-20, 20, -20, 20, -10, 10);
            Assert.That(_powerMill.ActiveProject.Toolpaths[0].BlockBoundingBox, Is.EqualTo(boundingBoxToFind));
        }

        [Test]
        public void DrawAndUndrawAllToolpathsTest()
        {
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            _powerMill.ActiveProject.Toolpaths.DrawAll();
            _powerMill.ActiveProject.Toolpaths.UndrawAll();
            Assert.IsTrue(true);            
        }

        #endregion
    }
}