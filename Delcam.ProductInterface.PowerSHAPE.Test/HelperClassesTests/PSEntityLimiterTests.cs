// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System.Collections.Generic;
using System.IO;
using Autodesk.ProductInterface.PowerSHAPE;
using NUnit.Framework;

namespace Autodesk.ProductInterface.PowerSHAPETest.HelperClassesTests
{
    [TestFixture]
    public class PSEntityLimiterTests
    {
        [Test]
        public void WhenLimitEntities_GivenKeepBoth_ThenReturnExpectedNumberOfEntities()
        {
            var testDirectory = Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\Files\");

            var powerShape = new PSAutomation(InstanceReuse.UseExistingInstance);
            powerShape.Reset();

            var testFilepath = Path.Combine(testDirectory, "KeepBoth.dgk");
            powerShape.ActiveModel.Import(new FileSystem.File(testFilepath));

            var entityToBeCut = powerShape.ActiveModel.Lines[0];
            var entityUsedToCut = powerShape.ActiveModel.CompCurves[0];

            var result = PSEntityLimiter.LimitEntities(new List<IPSLimitable> {entityToBeCut},
                                                       new List<PSEntity> {entityUsedToCut},
                                                       LimitingModes.LimitMode,
                                                       LimitingKeepOptions.KeepBoth);

            Assert.AreEqual(3, result.Count, "It should return 3 entities when KeepBoth option is used.");
        }
    }
}