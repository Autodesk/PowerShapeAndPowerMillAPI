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
using Autodesk.FileSystem;
using Autodesk.Geometry;
using Autodesk.ProductInterface.PowerSHAPE;
using NUnit.Framework;

namespace Autodesk.ProductInterface.PowerSHAPETest
{
    [TestFixture]
    public class PSMacroTest
    {
        private PSAutomation _powerShape;
        private List<Point> _macroPoints;

        [TestFixtureSetUp]
        public void TestSetup()
        {
            _macroPoints = new List<Point>
            {
                new Point(0, 0, 0),
                new Point(0, 0, 10),
                new Point(0, 0, 20)
            };

            // Initialise PowerSHAPE
            StartPowerShape();
        }

        private void StartPowerShape()
        {
            if (_powerShape == null)
            {
                _powerShape = new PSAutomation(InstanceReuse.UseExistingInstance);
            }

            _powerShape.Reset();

            _powerShape.FormUpdateOff();
            _powerShape.RefreshOff();
            _powerShape.DialogsOff();
        }

        [SetUp]
        public void UnitTestSetup()
        {
            _powerShape.Reset();
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            try
            {
                _powerShape.Reset();
            }
            catch (Exception)
            {
            }
        }

        [Test]
        public void ShouldRunMacrosFromAListOfStrings_UsingRunMacro()
        {
            var macroText = new File(TestFiles.CreateThreePoints).ReadTextLines();
            var macro = _powerShape.LoadMacro(macroText.ToArray());

            _powerShape.RunMacro(macro);
            _powerShape.ActiveModel.Refresh();

            AssertMacroPointsAreCreated(_powerShape.ActiveModel.Points);
        }

        private void AssertMacroPointsAreCreated(PSPointsCollection pointCollection)
        {
            Assert.That(pointCollection.Count, Is.EqualTo(_macroPoints.Count));

            var points = pointCollection.Select(point => new Point(point.X, point.Y, point.Z)).ToList();
            CollectionAssert.AreEquivalent(points, _macroPoints);
        }

        [Test]
        public void ShouldRunMacrosFromAString_UsingLoadMacro()
        {
            var macroText = new File(TestFiles.CreateThreePoints).ReadTextLines();
            var macro = _powerShape.LoadMacro(string.Join(Environment.NewLine, macroText));

            macro.Run();
            _powerShape.ActiveModel.Refresh();

            AssertMacroPointsAreCreated(_powerShape.ActiveModel.Points);
        }
    }
}