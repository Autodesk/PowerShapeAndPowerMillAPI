// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System.Threading;

namespace Autodesk.ProductInterface.PowerSHAPETest.HelperClassesTests
{
    class PointPickerCancelTestHarness
    {
        ICancelPointPickCheck _pointCancelStrategy;
        PowerSHAPE.PSAutomation _powerShape;
        Thread _myThread;

        public PointPickerCancelTestHarness(ICancelPointPickCheck pointCancelStrategy, PowerSHAPE.PSAutomation powerShape)
        {
            _pointCancelStrategy = pointCancelStrategy;
            _powerShape = powerShape;
        }

        public void Run()
        {
            PickedPoint = new Geometry.Point();
            _myThread = new Thread(RunThread);
            _myThread.Start();
        }

        private void RunThread()
        {
            //Start point picker which then sits and waits until it has been cancelled
            Geometry.Point point = _powerShape.PickPoint(_pointCancelStrategy);
            PickedPoint = point;
        }

        public void StopPointPicker()
        {
            _pointCancelStrategy.CanelPointPicking();
        }

        public bool IsMyThreadAlive()
        {
            return _myThread.IsAlive;
        }

        public Geometry.Point PickedPoint { get; set; }
    }
}