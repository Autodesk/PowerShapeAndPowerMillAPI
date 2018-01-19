// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using Autodesk.Matrix;
using NUnit.Framework;

namespace Autodesk.Geometry.Test
{
    [TestFixture]
    public class MatrixTests
    {
        [Test]
        public void WhenDoingAHadamardProduct_GivenDifferentDimensions_ThenThrowException()
        {
            var a = Matrix.Matrix.Create(3, 3, 0);
            var b = Matrix.Matrix.Create(2, 3, 0);

            Assert.Throws(typeof(ArgumentException),
                          () => a.HadamardProduct(b),
                          "Hadamard product cannot be calculated if dimensions differ.");
        }

        [Test]
        public void WhenDoingAHadamardProduct_ThenCheckExpected()
        {
            var a = Matrix.Matrix.Identity(3);
            var b = Matrix.Matrix.Identity(3);
            a = a.Multiply(3);
            b = b.Multiply(2);

            var c = a.HadamardProduct(b);
            Assert.AreEqual(6, c[0, 0]);
            Assert.AreEqual(0, c[0, 1]);
            Assert.AreEqual(0, c[0, 2]);
            Assert.AreEqual(6, c[1, 1]);
            Assert.AreEqual(0, c[1, 0]);
            Assert.AreEqual(0, c[1, 2]);
            Assert.AreEqual(6, c[2, 2]);
            Assert.AreEqual(0, c[2, 0]);
            Assert.AreEqual(0, c[2, 1]);
        }
    }
}