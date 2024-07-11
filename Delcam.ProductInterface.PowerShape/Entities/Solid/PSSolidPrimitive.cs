// **********************************************************************
// *         © COPYRIGHT 2024 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

namespace Autodesk.ProductInterface.PowerSHAPE
{
    /// <summary>
    /// Base class for PowerSHAPE primitives
    /// </summary>
    public abstract class PSSolidPrimitive : PSSolid, IPSPrimitive
    {
        #region " Fields "

        #endregion

        #region " Constructors "

        internal PSSolidPrimitive(PSAutomation powerSHAPE) : base(powerSHAPE)
        {
        }

        internal PSSolidPrimitive(PSAutomation powerSHAPE, int id, string name) : base(powerSHAPE, id, name)
        {
        }

        #endregion

        #region " Properties "

        /// <summary>
        /// Gets the origin of the primitive
        /// </summary>
        public Geometry.Point Origin
        {
            get
            {
                double[] psOrigin = _powerSHAPE.DoCommandEx(Identifier + "[ID " + _id + "].ORIGIN") as double[];
                return new Geometry.Point(psOrigin[0], psOrigin[1], psOrigin[2]);
            }
        }

        /// <summary>
        /// Gets the X axis of the primitive
        /// </summary>
        public Geometry.Vector XAxis
        {
            get
            {
                double[] psOrigin = _powerSHAPE.DoCommandEx(Identifier + "[ID " + _id + "].XAXIS") as double[];
                return new Geometry.Vector(psOrigin[0], psOrigin[1], psOrigin[2]);
            }
        }

        /// <summary>
        /// Gets the Y axis of the primitive
        /// </summary>
        public Geometry.Vector YAxis
        {
            get
            {
                double[] psOrigin = _powerSHAPE.DoCommandEx(Identifier + "[ID " + _id + "].YAXIS") as double[];
                return new Geometry.Vector(psOrigin[0], psOrigin[1], psOrigin[2]);
            }
        }

        /// <summary>
        /// Gets the Z axis of the primitive
        /// </summary>
        public Geometry.Vector ZAxis
        {
            get
            {
                double[] psOrigin = _powerSHAPE.DoCommandEx(Identifier + "[ID " + _id + "].ZAXIS") as double[];
                return new Geometry.Vector(psOrigin[0], psOrigin[1], psOrigin[2]);
            }
        }

        #endregion

        #region " Operations "

        #endregion
    }
}
