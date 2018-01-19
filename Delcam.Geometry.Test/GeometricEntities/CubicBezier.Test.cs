// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using NUnit.Framework;

namespace Autodesk.Geometry.Test.GeometricEntities
{
    [TestFixture]
    public class CubicBezierTest
    {
        [Test]
        public void PositionTest()
        {
            Point startPoint = new Point(0, 0, 0);
            Point startControlPoint = new Point(1.3, 3, 0);
            Point endControlPoint = new Point(6, 4, 0);
            Point endPoint = new Point(8, 1.6, 0);

            CubicBezier bezier = new CubicBezier(startPoint, endPoint, startControlPoint, endControlPoint);

            Assert.AreEqual(new Point(2.8016, 2.5504, 0), bezier.Position(0.4));
        }

        [Test]
        public void TangentTest()
        {
            Point startPoint = new Point(0, 0, 0);
            Point startControlPoint = new Point(1.3, 3, 0);
            Point endControlPoint = new Point(6, 4, 0);
            Point endPoint = new Point(8, 1.6, 0);

            CubicBezier bezier = new CubicBezier(startPoint, endPoint, startControlPoint, endControlPoint);

            Assert.AreEqual(new Vector(0.93280749948951, 0.360375039224594, 0), bezier.Tangent(0.4));
        }
    }
}