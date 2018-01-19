// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using NUnit.Framework;

namespace Autodesk.Geometry.Test.Lengths
{
    [TestFixture]
    public class MMTest
    {
        #region "Operators"

        [Test]
        public void WhenAddMMWithInt_ThenResultShouldBeMM()
        {
            // When
            MM left = 3;
            int right = 6;
            var result = left + right;

            // Then
            Assert.That(result.GetType(), Is.EqualTo(typeof(MM)));
            Assert.That(result.Value, Is.EqualTo(9));
        }

        [Test]
        public void WhenAddIntWithMM_ThenResultShouldBeMM()
        {
            // When
            int left = 6;
            MM right = 3;
            var result = left + right;

            // Then
            Assert.That(result.GetType(), Is.EqualTo(typeof(MM)));
            Assert.That(result.Value, Is.EqualTo(9));
        }

        [Test]
        public void WhenSubtractMMFromInt_ThenResultShouldBeMM()
        {
            // When
            MM left = 3;
            int right = 6;
            var result = left - right;

            // Then
            Assert.That(result.GetType(), Is.EqualTo(typeof(MM)));
            Assert.That(result.Value, Is.EqualTo(-3));
        }

        [Test]
        public void WhenSubtractIntFromMM_ThenResultShouldBeMM()
        {
            // When
            int left = 6;
            MM right = 3;
            var result = left - right;

            // Then
            Assert.That(result.GetType(), Is.EqualTo(typeof(MM)));
            Assert.That(result.Value, Is.EqualTo(3));
        }

        [Test]
        public void WhenMultiplyMMWithInt_ThenResultShouldBeMM()
        {
            // When
            MM left = 3;
            int right = 6;
            var result = left * right;

            // Then
            Assert.That(result.GetType(), Is.EqualTo(typeof(MM)));
            Assert.That(result.Value, Is.EqualTo(18));
        }

        [Test]
        public void WhenMultiplyIntWithMM_ThenResultShouldBeMM()
        {
            // When
            int left = 6;
            MM right = 3;
            var result = left * right;

            // Then
            Assert.That(result.GetType(), Is.EqualTo(typeof(MM)));
            Assert.That(result.Value, Is.EqualTo(18));
        }

        [Test]
        public void WhenDivideMMByInt_ThenResultShouldBeMM()
        {
            // When
            MM left = 3;
            int right = 6;
            var result = left / right;

            // Then
            Assert.That(result.GetType(), Is.EqualTo(typeof(MM)));
            Assert.That(result, Is.EqualTo((MM) 0.5));
        }

        [Test]
        public void WhenDivideIntByMM_ThenResultShouldBeDouble()
        {
            // When
            int left = 6;
            MM right = 3;
            var result = left / right;

            // Then
            Assert.That(result.GetType(), Is.EqualTo(typeof(double)));
            Assert.That(result, Is.EqualTo(2));
        }

        #endregion
    }
}