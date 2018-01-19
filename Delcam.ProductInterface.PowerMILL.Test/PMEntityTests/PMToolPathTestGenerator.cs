// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
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
        private readonly Random _random = new Random();

        [Test]
        public void EditSurfaceSpeedWhileToolPathIsCalculatedTest()
        {
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var toolpath = _powerMill.ActiveProject.Toolpaths.First();

            var value = _random.Next(1, 1000);
            toolpath.SurfaceSpeed = value;

            var result = toolpath.SurfaceSpeed;
            Assert.That(result, Is.EqualTo(value));
        }

        [Test]
        public void EditCuttingFeedWhileToolPathIsCalculatedTest()
        {
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var toolpath = _powerMill.ActiveProject.Toolpaths.First();

            var value = _random.Next(1, 1000);
            toolpath.CuttingFeed = value;

            var result = toolpath.CuttingFeed;
            Assert.That(result, Is.EqualTo(value));
        }

        [Test]
        public void EditFeedPerToothWhileToolPathIsCalculatedTest()
        {
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var toolpath = _powerMill.ActiveProject.Toolpaths.First();

            MM value = _random.Next(1, 1000);
            toolpath.FeedPerTooth = value;

            var result = toolpath.FeedPerTooth;
            Assert.That(result, Is.EqualTo(value));
        }

        [Test]
        public void EditLeadInFactorWhileToolPathIsCalculatedTest()
        {
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var toolpath = _powerMill.ActiveProject.Toolpaths.First();

            var value = _random.Next(1, 1000);
            toolpath.LeadInFactor = value;

            var result = toolpath.LeadInFactor;
            Assert.That(result, Is.EqualTo(value));
        }

        [Test]
        public void EditLeadOutFactorWhileToolPathIsCalculatedTest()
        {
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var toolpath = _powerMill.ActiveProject.Toolpaths.First();

            var value = _random.Next(1, 1000);
            toolpath.LeadOutFactor = value;

            var result = toolpath.LeadOutFactor;
            Assert.That(result, Is.EqualTo(value));
        }

        [Test]
        public void EditPlungingFeedWhileToolPathIsCalculatedTest()
        {
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var toolpath = _powerMill.ActiveProject.Toolpaths.First();

            var value = _random.Next(1, 1000);
            toolpath.PlungingFeed = value;

            var result = toolpath.PlungingFeed;
            Assert.That(result, Is.EqualTo(value));
        }

        [Test]
        public void EditSkimFeedWhileToolPathIsCalculatedTest()
        {
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var toolpath = _powerMill.ActiveProject.Toolpaths.First();

            var value = _random.Next(1, 1000);
            toolpath.SkimFeed = value;

            var result = toolpath.SkimFeed;
            Assert.That(result, Is.EqualTo(value));
        }

        [Test]
        public void EditSpindleSpeedWhileToolPathIsCalculatedTest()
        {
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var toolpath = _powerMill.ActiveProject.Toolpaths.First();

            var value = _random.Next(1, 1000);
            toolpath.SpindleSpeed = value;

            var result = toolpath.SpindleSpeed;
            Assert.That(result, Is.EqualTo(value));
        }

        [Test]
        public void EditStartPointApproachDistanceWhileToolPathIsCalculatedTest()
        {
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var toolpath = _powerMill.ActiveProject.Toolpaths.First();

            MM value = _random.Next(1, 1000);
            toolpath.StartPointApproachDistance = value;

            var result = toolpath.StartPointApproachDistance;
            Assert.That(result, Is.EqualTo(value));
        }

        [Test]
        public void StartPointApproachAlongWhileToolPathIsCalculatedTest()
        {
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var toolpath = _powerMill.ActiveProject.Toolpaths.First();
            foreach (var item in typeof(PointApproach).GetEnumValues())
            {
                toolpath.StartPointApproachAlong = (PointApproach) item;
                Assert.That(toolpath.StartPointApproachAlong, Is.EqualTo(item));
            }
        }

        [Test]
        public void StartPointMethodWhileToolPathIsCalculatedTest()
        {
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var toolpath = _powerMill.ActiveProject.Toolpaths.First();
            foreach (var item in typeof(StartPointMethod).GetEnumValues())
            {
                toolpath.StartPointMethod = (StartPointMethod) item;
                Assert.That(toolpath.StartPointMethod, Is.EqualTo(item));
            }
        }

        [Test]
        public void StartPointPositionXReadOnlyTest()
        {
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var toolpath = _powerMill.ActiveProject.Toolpaths.First();

            var result = toolpath.StartPointPositionX;
            Assert.That(result, Is.GreaterThan(0));
        }

        [Test]
        public void StartPointPositionYReadOnlyTest()
        {
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var toolpath = _powerMill.ActiveProject.Toolpaths.First();

            var result = toolpath.StartPointPositionY;
            Assert.That(result, Is.GreaterThan(0));
        }

        [Test]
        public void StartPointPositionZReadOnlyTest()
        {
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var toolpath = _powerMill.ActiveProject.Toolpaths.First();

            var result = toolpath.StartPointPositionZ;
            Assert.That(result, Is.GreaterThan(0));
        }

        [Test]
        public void EditEndPointApproachDistanceWhileToolPathIsCalculatedTest()
        {
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var toolpath = _powerMill.ActiveProject.Toolpaths.First();

            MM value = _random.Next(1, 1000);
            toolpath.EndPointApproachDistance = value;

            var result = toolpath.EndPointApproachDistance;
            Assert.That(result, Is.EqualTo(value));
        }

        [Test]
        public void EndPointApproachAlongWhileToolPathIsCalculatedTest()
        {
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var toolpath = _powerMill.ActiveProject.Toolpaths.First();
            foreach (var item in typeof(PointApproach).GetEnumValues())
            {
                toolpath.EndPointApproachAlong = (PointApproach) item;
                Assert.That(toolpath.EndPointApproachAlong, Is.EqualTo(item));
            }
        }

        [Test]
        public void EndPointMethodWhileToolPathIsCalculatedTest()
        {
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var toolpath = _powerMill.ActiveProject.Toolpaths.First();
            foreach (var item in typeof(EndPointMethod).GetEnumValues())
            {
                toolpath.EndPointMethod = (EndPointMethod) item;
                Assert.That(toolpath.EndPointMethod, Is.EqualTo(item));
            }
        }

        [Test]
        public void EndPointPositionXReadOnlyTest()
        {
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var toolpath = _powerMill.ActiveProject.Toolpaths.First();

            var result = toolpath.EndPointPositionX;
            Assert.That(result, Is.GreaterThan(0));
        }

        [Test]
        public void EndPointPositionYReadOnlyTest()
        {
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var toolpath = _powerMill.ActiveProject.Toolpaths.First();

            var result = toolpath.EndPointPositionY;
            Assert.That(result, Is.GreaterThan(0));
        }

        [Test]
        public void EndPointPositionZReadOnlyTest()
        {
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var toolpath = _powerMill.ActiveProject.Toolpaths.First();

            var result = toolpath.EndPointPositionZ;
            Assert.That(result, Is.GreaterThan(0));
        }

        [Test]
        public void LeadInTest()
        {
            var angle = _random.NextDouble();
            var distance = _random.Next();
            var radius = _random.NextDouble();
            var diameter = _random.Next();
            var rampFollow = (RampFollow) _random.Next(typeof(RampFollow).GetEnumValues().Length);
            var rampHeight = (RampHeight) _random.Next(typeof(RampHeight).GetEnumValues().Length);
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var lead = _powerMill.ActiveProject.Toolpaths.First().Connections.LeadIn;

            lead.Angle = angle;
            lead.Distance = distance;
            lead.Radius = radius;
            lead.Ramp.CircleDiameter = diameter;
            lead.Ramp.ClosedSegments = true;
            lead.Ramp.Extend = true;
            lead.Ramp.FiniteRamp = true;
            lead.Ramp.FiniteRampLenght = distance;
            lead.Ramp.Follow = rampFollow;
            lead.Ramp.MaxZigAngle = angle;
            lead.Ramp.RampHeight = rampHeight;
            lead.Ramp.RampHeightValue = distance;
            lead.Ramp.MaxZigAngle = angle;
            lead.Ramp.ZagAngle = true;
            lead.Ramp.ZagAngleMaximum = angle;

            Assert.That(lead.LeadType, Is.EqualTo(LeadTypes.LeadIn));
            Assert.That(lead.Angle.Value, Is.EqualTo(angle).Within(TOLERANCE));
            Assert.That(lead.Distance.Value, Is.EqualTo(distance).Within(TOLERANCE));
            Assert.That(lead.Radius.Value, Is.EqualTo(radius).Within(TOLERANCE));
            Assert.That(lead.Ramp.CircleDiameter.Value, Is.EqualTo(diameter).Within(TOLERANCE));
            Assert.That(lead.Ramp.ClosedSegments, Is.True);
            Assert.That(lead.Ramp.Extend, Is.True);
            Assert.That(lead.Ramp.FiniteRamp, Is.True);
            Assert.That(lead.Ramp.FiniteRampLenght.Value, Is.EqualTo(distance).Within(TOLERANCE));
            Assert.That(lead.Ramp.Follow, Is.EqualTo(rampFollow));
            Assert.That(lead.Ramp.MaxZigAngle.Value, Is.EqualTo(angle).Within(TOLERANCE));
            Assert.That(lead.Ramp.RampHeight, Is.EqualTo(rampHeight));
            Assert.That(lead.Ramp.RampHeightValue.Value, Is.EqualTo(distance).Within(TOLERANCE));
            Assert.That(lead.Ramp.MaxZigAngle.Value, Is.EqualTo(angle).Within(TOLERANCE));
            Assert.That(lead.Ramp.ZagAngle, Is.True);
            Assert.That(lead.Ramp.ZagAngleMaximum.Value, Is.EqualTo(angle).Within(TOLERANCE));
        }

        [Test]
        public void MoveTypeOnLeadInTest()
        {
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var lead = _powerMill.ActiveProject.Toolpaths.First().Connections.LeadIn;
            foreach (var moveType in typeof(MoveType).GetEnumValues())
            {
                lead.MoveType = (MoveType) moveType;
                Assert.That(lead.MoveType, Is.EqualTo(moveType));
            }
        }

        public void RampFollowOnLeadInTest()
        {
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var lead = _powerMill.ActiveProject.Toolpaths.First().Connections.LeadIn;
            foreach (var rampFollow in typeof(RampFollow).GetEnumValues())
            {
                lead.Ramp.Follow = (RampFollow) rampFollow;
                Assert.That(lead.Ramp.Follow, Is.EqualTo(rampFollow));
            }
        }

        [Test]
        public void RampHeightOnLeadInTest()
        {
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var lead = _powerMill.ActiveProject.Toolpaths.First().Connections.LeadIn;
            foreach (var rampHeight in typeof(RampHeight).GetEnumValues())
            {
                lead.Ramp.RampHeight = (RampHeight) rampHeight;
                Assert.That(lead.Ramp.RampHeight, Is.EqualTo(rampHeight));
            }
        }

        [Test]
        public void LeadInSecondTest()
        {
            var angle = _random.NextDouble();
            var distance = _random.Next();
            var radius = _random.NextDouble();
            var diameter = _random.Next();
            var rampFollow = (RampFollow) _random.Next(typeof(RampFollow).GetEnumValues().Length);
            var rampHeight = (RampHeight) _random.Next(typeof(RampHeight).GetEnumValues().Length);
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var lead = _powerMill.ActiveProject.Toolpaths.First().Connections.LeadInSecond;

            lead.Angle = angle;
            lead.Distance = distance;
            lead.Radius = radius;
            lead.Ramp.CircleDiameter = diameter;
            lead.Ramp.ClosedSegments = true;
            lead.Ramp.Extend = true;
            lead.Ramp.FiniteRamp = true;
            lead.Ramp.FiniteRampLenght = distance;
            lead.Ramp.Follow = rampFollow;
            lead.Ramp.MaxZigAngle = angle;
            lead.Ramp.RampHeight = rampHeight;
            lead.Ramp.RampHeightValue = distance;
            lead.Ramp.MaxZigAngle = angle;
            lead.Ramp.ZagAngle = true;
            lead.Ramp.ZagAngleMaximum = angle;

            Assert.That(lead.LeadType, Is.EqualTo(LeadTypes.LeadInSecond));
            Assert.That(lead.Angle.Value, Is.EqualTo(angle).Within(TOLERANCE));
            Assert.That(lead.Distance.Value, Is.EqualTo(distance).Within(TOLERANCE));
            Assert.That(lead.Radius.Value, Is.EqualTo(radius).Within(TOLERANCE));
            Assert.That(lead.Ramp.CircleDiameter.Value, Is.EqualTo(diameter).Within(TOLERANCE));
            Assert.That(lead.Ramp.ClosedSegments, Is.True);
            Assert.That(lead.Ramp.Extend, Is.True);
            Assert.That(lead.Ramp.FiniteRamp, Is.True);
            Assert.That(lead.Ramp.FiniteRampLenght.Value, Is.EqualTo(distance).Within(TOLERANCE));
            Assert.That(lead.Ramp.Follow, Is.EqualTo(rampFollow));
            Assert.That(lead.Ramp.MaxZigAngle.Value, Is.EqualTo(angle).Within(TOLERANCE));
            Assert.That(lead.Ramp.RampHeight, Is.EqualTo(rampHeight));
            Assert.That(lead.Ramp.RampHeightValue.Value, Is.EqualTo(distance).Within(TOLERANCE));
            Assert.That(lead.Ramp.MaxZigAngle.Value, Is.EqualTo(angle).Within(TOLERANCE));
            Assert.That(lead.Ramp.ZagAngle, Is.True);
            Assert.That(lead.Ramp.ZagAngleMaximum.Value, Is.EqualTo(angle).Within(TOLERANCE));
        }

        [Test]
        public void MoveTypeOnLeadInSecondTest()
        {
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var lead = _powerMill.ActiveProject.Toolpaths.First().Connections.LeadInSecond;
            foreach (var moveType in typeof(MoveType).GetEnumValues())
            {
                lead.MoveType = (MoveType) moveType;
                Assert.That(lead.MoveType, Is.EqualTo(moveType));
            }
        }

        public void RampFollowOnLeadInSecondTest()
        {
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var lead = _powerMill.ActiveProject.Toolpaths.First().Connections.LeadIn;
            foreach (var rampFollow in typeof(RampFollow).GetEnumValues())
            {
                lead.Ramp.Follow = (RampFollow) rampFollow;
                Assert.That(lead.Ramp.Follow, Is.EqualTo(rampFollow));
            }
        }

        [Test]
        public void RampHeightOnLeadInSecondTest()
        {
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var lead = _powerMill.ActiveProject.Toolpaths.First().Connections.LeadInSecond;
            foreach (var rampHeight in typeof(RampHeight).GetEnumValues())
            {
                lead.Ramp.RampHeight = (RampHeight) rampHeight;
                Assert.That(lead.Ramp.RampHeight, Is.EqualTo(rampHeight));
            }
        }

        [Test]
        public void LeadOutTest()
        {
            var angle = _random.NextDouble();
            var distance = _random.Next();
            var radius = _random.NextDouble();
            var diameter = _random.Next();
            var rampFollow = (RampFollow) _random.Next(typeof(RampFollow).GetEnumValues().Length);
            var rampHeight = (RampHeight) _random.Next(typeof(RampHeight).GetEnumValues().Length);
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var lead = _powerMill.ActiveProject.Toolpaths.First().Connections.LeadOut;

            lead.Angle = angle;
            lead.Distance = distance;
            lead.Radius = radius;
            lead.Ramp.CircleDiameter = diameter;
            lead.Ramp.ClosedSegments = true;
            lead.Ramp.Extend = true;
            lead.Ramp.FiniteRamp = true;
            lead.Ramp.FiniteRampLenght = distance;
            lead.Ramp.Follow = rampFollow;
            lead.Ramp.MaxZigAngle = angle;
            lead.Ramp.RampHeight = rampHeight;
            lead.Ramp.RampHeightValue = distance;
            lead.Ramp.MaxZigAngle = angle;
            lead.Ramp.ZagAngle = true;
            lead.Ramp.ZagAngleMaximum = angle;

            Assert.That(lead.LeadType, Is.EqualTo(LeadTypes.LeadOut));
            Assert.That(lead.Angle.Value, Is.EqualTo(angle).Within(TOLERANCE));
            Assert.That(lead.Distance.Value, Is.EqualTo(distance).Within(TOLERANCE));
            Assert.That(lead.Radius.Value, Is.EqualTo(radius).Within(TOLERANCE));
            Assert.That(lead.Ramp.CircleDiameter.Value, Is.EqualTo(diameter).Within(TOLERANCE));
            Assert.That(lead.Ramp.ClosedSegments, Is.True);
            Assert.That(lead.Ramp.Extend, Is.True);
            Assert.That(lead.Ramp.FiniteRamp, Is.True);
            Assert.That(lead.Ramp.FiniteRampLenght.Value, Is.EqualTo(distance).Within(TOLERANCE));
            Assert.That(lead.Ramp.Follow, Is.EqualTo(rampFollow));
            Assert.That(lead.Ramp.MaxZigAngle.Value, Is.EqualTo(angle).Within(TOLERANCE));
            Assert.That(lead.Ramp.RampHeight, Is.EqualTo(rampHeight));
            Assert.That(lead.Ramp.RampHeightValue.Value, Is.EqualTo(distance).Within(TOLERANCE));
            Assert.That(lead.Ramp.MaxZigAngle.Value, Is.EqualTo(angle).Within(TOLERANCE));
            Assert.That(lead.Ramp.ZagAngle, Is.True);
            Assert.That(lead.Ramp.ZagAngleMaximum.Value, Is.EqualTo(angle).Within(TOLERANCE));
        }

        [Test]
        public void MoveTypeOnLeadOutTest()
        {
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var lead = _powerMill.ActiveProject.Toolpaths.First().Connections.LeadOut;
            foreach (var moveType in typeof(MoveType).GetEnumValues())
            {
                lead.MoveType = (MoveType) moveType;
                Assert.That(lead.MoveType, Is.EqualTo(moveType));
            }
        }

        public void RampFollowOnLeadOutTest()
        {
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var lead = _powerMill.ActiveProject.Toolpaths.First().Connections.LeadIn;
            foreach (var rampFollow in typeof(RampFollow).GetEnumValues())
            {
                lead.Ramp.Follow = (RampFollow) rampFollow;
                Assert.That(lead.Ramp.Follow, Is.EqualTo(rampFollow));
            }
        }

        [Test]
        public void RampHeightOnLeadOutTest()
        {
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var lead = _powerMill.ActiveProject.Toolpaths.First().Connections.LeadOut;
            foreach (var rampHeight in typeof(RampHeight).GetEnumValues())
            {
                lead.Ramp.RampHeight = (RampHeight) rampHeight;
                Assert.That(lead.Ramp.RampHeight, Is.EqualTo(rampHeight));
            }
        }

        [Test]
        public void LeadOutSecondTest()
        {
            var angle = _random.NextDouble();
            var distance = _random.Next();
            var radius = _random.NextDouble();
            var diameter = _random.Next();
            var rampFollow = (RampFollow) _random.Next(typeof(RampFollow).GetEnumValues().Length);
            var rampHeight = (RampHeight) _random.Next(typeof(RampHeight).GetEnumValues().Length);
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var lead = _powerMill.ActiveProject.Toolpaths.First().Connections.LeadOutSecond;

            lead.Angle = angle;
            lead.Distance = distance;
            lead.Radius = radius;
            lead.Ramp.CircleDiameter = diameter;
            lead.Ramp.ClosedSegments = true;
            lead.Ramp.Extend = true;
            lead.Ramp.FiniteRamp = true;
            lead.Ramp.FiniteRampLenght = distance;
            lead.Ramp.Follow = rampFollow;
            lead.Ramp.MaxZigAngle = angle;
            lead.Ramp.RampHeight = rampHeight;
            lead.Ramp.RampHeightValue = distance;
            lead.Ramp.MaxZigAngle = angle;
            lead.Ramp.ZagAngle = true;
            lead.Ramp.ZagAngleMaximum = angle;

            Assert.That(lead.LeadType, Is.EqualTo(LeadTypes.LeadOutSecond));
            Assert.That(lead.Angle.Value, Is.EqualTo(angle).Within(TOLERANCE));
            Assert.That(lead.Distance.Value, Is.EqualTo(distance).Within(TOLERANCE));
            Assert.That(lead.Radius.Value, Is.EqualTo(radius).Within(TOLERANCE));
            Assert.That(lead.Ramp.CircleDiameter.Value, Is.EqualTo(diameter).Within(TOLERANCE));
            Assert.That(lead.Ramp.ClosedSegments, Is.True);
            Assert.That(lead.Ramp.Extend, Is.True);
            Assert.That(lead.Ramp.FiniteRamp, Is.True);
            Assert.That(lead.Ramp.FiniteRampLenght.Value, Is.EqualTo(distance).Within(TOLERANCE));
            Assert.That(lead.Ramp.Follow, Is.EqualTo(rampFollow));
            Assert.That(lead.Ramp.MaxZigAngle.Value, Is.EqualTo(angle).Within(TOLERANCE));
            Assert.That(lead.Ramp.RampHeight, Is.EqualTo(rampHeight));
            Assert.That(lead.Ramp.RampHeightValue.Value, Is.EqualTo(distance).Within(TOLERANCE));
            Assert.That(lead.Ramp.MaxZigAngle.Value, Is.EqualTo(angle).Within(TOLERANCE));
            Assert.That(lead.Ramp.ZagAngle, Is.True);
            Assert.That(lead.Ramp.ZagAngleMaximum.Value, Is.EqualTo(angle).Within(TOLERANCE));
        }

        [Test]
        public void MoveTypeOnLeadOutSecondTest()
        {
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var lead = _powerMill.ActiveProject.Toolpaths.First().Connections.LeadOutSecond;
            foreach (var moveType in typeof(MoveType).GetEnumValues())
            {
                lead.MoveType = (MoveType) moveType;
                Assert.That(lead.MoveType, Is.EqualTo(moveType));
            }
        }

        public void RampFollowOnLeadOutSecondTest()
        {
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var lead = _powerMill.ActiveProject.Toolpaths.First().Connections.LeadIn;
            foreach (var rampFollow in typeof(RampFollow).GetEnumValues())
            {
                lead.Ramp.Follow = (RampFollow) rampFollow;
                Assert.That(lead.Ramp.Follow, Is.EqualTo(rampFollow));
            }
        }

        [Test]
        public void RampHeightOnLeadOutSecondTest()
        {
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var lead = _powerMill.ActiveProject.Toolpaths.First().Connections.LeadOutSecond;
            foreach (var rampHeight in typeof(RampHeight).GetEnumValues())
            {
                lead.Ramp.RampHeight = (RampHeight) rampHeight;
                Assert.That(lead.Ramp.RampHeight, Is.EqualTo(rampHeight));
            }
        }

        [Test]
        public void FirstLeadInTest()
        {
            var angle = _random.NextDouble();
            var distance = _random.Next();
            var radius = _random.NextDouble();
            var diameter = _random.Next();
            var rampFollow = (RampFollow) _random.Next(typeof(RampFollow).GetEnumValues().Length);
            var rampHeight = (RampHeight) _random.Next(typeof(RampHeight).GetEnumValues().Length);
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var lead = _powerMill.ActiveProject.Toolpaths.First().Connections.FirstLeadIn;

            lead.Angle = angle;
            lead.Distance = distance;
            lead.Radius = radius;
            lead.Ramp.CircleDiameter = diameter;
            lead.Ramp.ClosedSegments = true;
            lead.Ramp.Extend = true;
            lead.Ramp.FiniteRamp = true;
            lead.Ramp.FiniteRampLenght = distance;
            lead.Ramp.Follow = rampFollow;
            lead.Ramp.MaxZigAngle = angle;
            lead.Ramp.RampHeight = rampHeight;
            lead.Ramp.RampHeightValue = distance;
            lead.Ramp.MaxZigAngle = angle;
            lead.Ramp.ZagAngle = true;
            lead.Ramp.ZagAngleMaximum = angle;

            Assert.That(lead.LeadType, Is.EqualTo(LeadTypes.FirstLeadIn));
            Assert.That(lead.Angle.Value, Is.EqualTo(angle).Within(TOLERANCE));
            Assert.That(lead.Distance.Value, Is.EqualTo(distance).Within(TOLERANCE));
            Assert.That(lead.Radius.Value, Is.EqualTo(radius).Within(TOLERANCE));
            Assert.That(lead.Ramp.CircleDiameter.Value, Is.EqualTo(diameter).Within(TOLERANCE));
            Assert.That(lead.Ramp.ClosedSegments, Is.True);
            Assert.That(lead.Ramp.Extend, Is.True);
            Assert.That(lead.Ramp.FiniteRamp, Is.True);
            Assert.That(lead.Ramp.FiniteRampLenght.Value, Is.EqualTo(distance).Within(TOLERANCE));
            Assert.That(lead.Ramp.Follow, Is.EqualTo(rampFollow));
            Assert.That(lead.Ramp.MaxZigAngle.Value, Is.EqualTo(angle).Within(TOLERANCE));
            Assert.That(lead.Ramp.RampHeight, Is.EqualTo(rampHeight));
            Assert.That(lead.Ramp.RampHeightValue.Value, Is.EqualTo(distance).Within(TOLERANCE));
            Assert.That(lead.Ramp.MaxZigAngle.Value, Is.EqualTo(angle).Within(TOLERANCE));
            Assert.That(lead.Ramp.ZagAngle, Is.True);
            Assert.That(lead.Ramp.ZagAngleMaximum.Value, Is.EqualTo(angle).Within(TOLERANCE));
        }

        [Test]
        public void MoveTypeOnFirstLeadInTest()
        {
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var lead = _powerMill.ActiveProject.Toolpaths.First().Connections.FirstLeadIn;
            foreach (var moveType in typeof(MoveType).GetEnumValues())
            {
                lead.MoveType = (MoveType) moveType;
                Assert.That(lead.MoveType, Is.EqualTo(moveType));
            }
        }

        public void RampFollowOnFirstLeadInTest()
        {
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var lead = _powerMill.ActiveProject.Toolpaths.First().Connections.LeadIn;
            foreach (var rampFollow in typeof(RampFollow).GetEnumValues())
            {
                lead.Ramp.Follow = (RampFollow) rampFollow;
                Assert.That(lead.Ramp.Follow, Is.EqualTo(rampFollow));
            }
        }

        [Test]
        public void RampHeightOnFirstLeadInTest()
        {
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var lead = _powerMill.ActiveProject.Toolpaths.First().Connections.FirstLeadIn;
            foreach (var rampHeight in typeof(RampHeight).GetEnumValues())
            {
                lead.Ramp.RampHeight = (RampHeight) rampHeight;
                Assert.That(lead.Ramp.RampHeight, Is.EqualTo(rampHeight));
            }
        }

        [Test]
        public void LastLeadOutTest()
        {
            var angle = _random.NextDouble();
            var distance = _random.Next();
            var radius = _random.NextDouble();
            var diameter = _random.Next();
            var rampFollow = (RampFollow) _random.Next(typeof(RampFollow).GetEnumValues().Length);
            var rampHeight = (RampHeight) _random.Next(typeof(RampHeight).GetEnumValues().Length);
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var lead = _powerMill.ActiveProject.Toolpaths.First().Connections.LastLeadOut;

            lead.Angle = angle;
            lead.Distance = distance;
            lead.Radius = radius;
            lead.Ramp.CircleDiameter = diameter;
            lead.Ramp.ClosedSegments = true;
            lead.Ramp.Extend = true;
            lead.Ramp.FiniteRamp = true;
            lead.Ramp.FiniteRampLenght = distance;
            lead.Ramp.Follow = rampFollow;
            lead.Ramp.MaxZigAngle = angle;
            lead.Ramp.RampHeight = rampHeight;
            lead.Ramp.RampHeightValue = distance;
            lead.Ramp.MaxZigAngle = angle;
            lead.Ramp.ZagAngle = true;
            lead.Ramp.ZagAngleMaximum = angle;

            Assert.That(lead.LeadType, Is.EqualTo(LeadTypes.LastLeadOut));
            Assert.That(lead.Angle.Value, Is.EqualTo(angle).Within(TOLERANCE));
            Assert.That(lead.Distance.Value, Is.EqualTo(distance).Within(TOLERANCE));
            Assert.That(lead.Radius.Value, Is.EqualTo(radius).Within(TOLERANCE));
            Assert.That(lead.Ramp.CircleDiameter.Value, Is.EqualTo(diameter).Within(TOLERANCE));
            Assert.That(lead.Ramp.ClosedSegments, Is.True);
            Assert.That(lead.Ramp.Extend, Is.True);
            Assert.That(lead.Ramp.FiniteRamp, Is.True);
            Assert.That(lead.Ramp.FiniteRampLenght.Value, Is.EqualTo(distance).Within(TOLERANCE));
            Assert.That(lead.Ramp.Follow, Is.EqualTo(rampFollow));
            Assert.That(lead.Ramp.MaxZigAngle.Value, Is.EqualTo(angle).Within(TOLERANCE));
            Assert.That(lead.Ramp.RampHeight, Is.EqualTo(rampHeight));
            Assert.That(lead.Ramp.RampHeightValue.Value, Is.EqualTo(distance).Within(TOLERANCE));
            Assert.That(lead.Ramp.MaxZigAngle.Value, Is.EqualTo(angle).Within(TOLERANCE));
            Assert.That(lead.Ramp.ZagAngle, Is.True);
            Assert.That(lead.Ramp.ZagAngleMaximum.Value, Is.EqualTo(angle).Within(TOLERANCE));
        }

        [Test]
        public void MoveTypeOnLastLeadOutTest()
        {
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var lead = _powerMill.ActiveProject.Toolpaths.First().Connections.LastLeadOut;
            foreach (var moveType in typeof(MoveType).GetEnumValues())
            {
                lead.MoveType = (MoveType) moveType;
                Assert.That(lead.MoveType, Is.EqualTo(moveType));
            }
        }

        public void RampFollowOnLastLeadOutTest()
        {
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var lead = _powerMill.ActiveProject.Toolpaths.First().Connections.LeadIn;
            foreach (var rampFollow in typeof(RampFollow).GetEnumValues())
            {
                lead.Ramp.Follow = (RampFollow) rampFollow;
                Assert.That(lead.Ramp.Follow, Is.EqualTo(rampFollow));
            }
        }

        [Test]
        public void RampHeightOnLastLeadOutTest()
        {
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var lead = _powerMill.ActiveProject.Toolpaths.First().Connections.LastLeadOut;
            foreach (var rampHeight in typeof(RampHeight).GetEnumValues())
            {
                lead.Ramp.RampHeight = (RampHeight) rampHeight;
                Assert.That(lead.Ramp.RampHeight, Is.EqualTo(rampHeight));
            }
        }

        [Test]
        public void LeadInExtensionTest()
        {
            var angle = _random.NextDouble();
            var distance = _random.Next();
            var radius = _random.NextDouble();
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var lead = _powerMill.ActiveProject.Toolpaths.First().Connections.LeadInExtension;

            lead.Angle = angle;
            lead.Distance = distance;
            lead.Radius = radius;

            Assert.That(lead.LeadType, Is.EqualTo(LeadTypes.LeadInExtension));
            Assert.That(lead.Angle.Value, Is.EqualTo(angle).Within(TOLERANCE));
            Assert.That(lead.Distance.Value, Is.EqualTo(distance).Within(TOLERANCE));
            Assert.That(lead.Radius.Value, Is.EqualTo(radius).Within(TOLERANCE));
        }

        [Test]
        public void MoveTypeOnLeadInExtensionTest()
        {
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var lead = _powerMill.ActiveProject.Toolpaths.First().Connections.LeadInExtension;
            foreach (var moveType in typeof(ExtensionLeadMoveTypes).GetEnumValues())
            {
                lead.MoveType = (ExtensionLeadMoveTypes) moveType;
                Assert.That(lead.MoveType, Is.EqualTo(moveType));
            }
        }

        [Test]
        public void LeadOutExtensionTest()
        {
            var angle = _random.NextDouble();
            var distance = _random.Next();
            var radius = _random.NextDouble();
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var lead = _powerMill.ActiveProject.Toolpaths.First().Connections.LeadOutExtension;

            lead.Angle = angle;
            lead.Distance = distance;
            lead.Radius = radius;

            Assert.That(lead.LeadType, Is.EqualTo(LeadTypes.LeadOutExtension));
            Assert.That(lead.Angle.Value, Is.EqualTo(angle).Within(TOLERANCE));
            Assert.That(lead.Distance.Value, Is.EqualTo(distance).Within(TOLERANCE));
            Assert.That(lead.Radius.Value, Is.EqualTo(radius).Within(TOLERANCE));
        }

        [Test]
        public void MoveTypeOnLeadOutExtensionTest()
        {
            _powerMill.LoadProject(TestFiles.BasicToolpath);
            var lead = _powerMill.ActiveProject.Toolpaths.First().Connections.LeadOutExtension;
            foreach (var moveType in typeof(ExtensionLeadMoveTypes).GetEnumValues())
            {
                lead.MoveType = (ExtensionLeadMoveTypes) moveType;
                Assert.That(lead.MoveType, Is.EqualTo(moveType));
            }
        }
    }
}