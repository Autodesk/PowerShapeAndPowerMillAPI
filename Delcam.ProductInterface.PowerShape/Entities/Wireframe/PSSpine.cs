// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;

namespace Autodesk.ProductInterface.PowerSHAPE
{
    /// <summary>
    /// Captures a Spine object in PowerSHAPE.
    /// The spine is a reference curve that may be used to define the overall shape of a surface.
    /// It was widely used in DUCT, but PowerSHAPE users need not normally be concerned with it.
    /// </summary>
    public class PSSpine : PSWireframe
    {
        #region " Fields "

        /// <summary>
        /// The parent surface of a spine.
        /// </summary>
        /// <remarks></remarks>
        protected PSSurface _surface;

        #endregion

        #region " Properties "

        internal override string Identifier
        {
            get { return "SURFACE['" + _surface.Name + "'].SPINE"; }
        }

        /// <summary>
        /// Gets/Sets the level of the surface curve by getting/setting the level
        /// of its parent surface
        /// </summary>
        public override PSLevel Level
        {
            get { return _surface.Level; }
            set { _surface.Level = value; }
        }

        /// <summary>
        /// Indicates that the user cannot currently obtain the spine centre of gravity in
        /// PowerSHAPE
        /// </summary>
        public new object CentreOfGravity
        {
            get { return new NotSupportedException("This feature is not available in PowerSHAPE"); }
        }

        #endregion

        #region " Constructors "

        /// <summary>
        /// Connects to PowerSHAPE
        /// </summary>
        internal PSSpine(PSAutomation powerSHAPE, PSSurface surface) : base(powerSHAPE)
        {
            _powerSHAPE = powerSHAPE;
            _surface = surface;
        }

        /// <summary>
        /// Connects to PowerSHAPE and a specific entity using its name
        /// </summary>
        internal PSSpine(PSAutomation powerSHAPE, int id, string name, PSSurface surface) : base(powerSHAPE, id, name)
        {
            _powerSHAPE = powerSHAPE;
            _id = id;
            _name = name;
            _surface = surface;
        }

        #endregion

        #region " Operations "

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <exception cref="NotImplementedException">Not implemented.</exception>
        /// <remarks></remarks>
        public override void Delete()
        {
            throw new NotImplementedException();
        }

        #region " Tangency Interrogations "

        /// <summary>
        /// Gets the tangent through the desired point
        /// </summary>
        /// <param name="pointNumber">The point at which tangency is required</param>
        /// <returns>Tangent vector</returns>
        public Geometry.Vector GetPointTangent(int pointNumber)
        {
            double[] tangent = _powerSHAPE.DoCommandEx(Identifier + "[ID " + "].POINT[" + pointNumber + "].TANGENT") as double[];
            return new Geometry.Vector(tangent[0], tangent[1], tangent[2]);
        }

        /// <summary>
        /// Gets the azimuth angle of the tangent of the desired point
        /// </summary>
        /// <param name="pointNumber">The point at which the tangency azimuth angle is required</param>
        /// <returns>Azimuth vector</returns>
        public Geometry.Vector GetPointAzimuthAngle(int pointNumber)
        {
            double[] azimuth =
                _powerSHAPE.DoCommandEx(Identifier + "[ID " + "].POINT[" + pointNumber + "].TANGENT.AZIMUTH") as double[];
            return new Geometry.Vector(azimuth[0], azimuth[1], azimuth[2]);
        }

        /// <summary>
        /// Gets the elevation angle of the tangent of the desired point
        /// </summary>
        /// <param name="pointNumber">The point at which the tangency elevation angle is required</param>
        /// <returns>Elevation vector</returns>
        public Geometry.Vector GetPointElevationAngle(int pointNumber)
        {
            double[] elevation =
                _powerSHAPE.DoCommandEx(Identifier + "[ID " + "].POINT[" + pointNumber + "].TANGENT.ELEVATION") as double[];
            return new Geometry.Vector(elevation[0], elevation[1], elevation[2]);
        }

        #endregion

        #endregion
    }
}