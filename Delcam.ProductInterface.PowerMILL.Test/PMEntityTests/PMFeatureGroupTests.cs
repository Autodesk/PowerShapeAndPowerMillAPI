﻿// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using System.Linq;
using Autodesk.ProductInterface.PowerMILL;
using NUnit.Framework;

namespace Autodesk.ProductInterface.PowerMILLTest.PMEntityTests
{
    [TestFixture]
    public class PMFeatureGroupTests
    {
        private PMAutomation _powerMill;

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

        #endregion


        #region Test operations

        [Test]
        public void UndrawAllFeatureGroupsTest()
        {
            _powerMill.Execute("CREATE FEATUREGROUP ;");
            _powerMill.Execute("CREATE FEATUREGROUP ;");            
            _powerMill.Execute("DRAW FEATUREGROUP ALL");
            _powerMill.ActiveProject.FeatureGroups.UndrawAll();
            Assert.IsTrue(true);
        }

        #endregion

    }
}
