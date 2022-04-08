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

namespace Autodesk.ProductInterface.PowerSHAPE
{
    /// <summary>
    /// Captures a Curve object in PowerSHAPE
    /// </summary>
    public class PSCurve : PSGenericCurve, IPSMoveable, IPSRotateable, IPSMirrorable, IPSOffsetable, IPSScalable
    {
        #region " Fields "

        #endregion

        #region " Constructors "

        /// <summary>
        /// Initialises the Curve
        /// </summary>
        internal PSCurve(PSAutomation powerSHAPE, int id, string name) : base(powerSHAPE, id, name)
        {
        }

        /// <summary>
        /// Creates a new curve through the specified points
        /// </summary>
        internal PSCurve(PSAutomation powerSHAPE, CurveTypes curveType, Geometry.Point[] points) : base(powerSHAPE)
        {
            // Clear created list
            _powerSHAPE.ActiveModel.ClearCreatedItems();

            // Create the curve
            switch (curveType)
            {
                case CurveTypes.Bezier:
                    _powerSHAPE.DoCommand("CREATE CURVE THROUGH");
                    break;
                case CurveTypes.bSpline:
                case CurveTypes.g2:
                    _powerSHAPE.DoCommand("CREATE CURVE " + curveType);
                    break;
            }

            foreach (Geometry.Point point in points)
            {
                var _with1 = point;
                _powerSHAPE.DoCommand("ABS " + _with1.X.ToString("0.######") + " " + _with1.Y.ToString("0.######") + " " + _with1.Z.ToString("0.######"));
            }
            _powerSHAPE.DoCommand("QUIT");

            // Get the id of the Curve
            PSCurve curve = (PSCurve) _powerSHAPE.ActiveModel.SelectedItems[0];
            _name = curve.Name;
            _id = curve.Id;
        }

        /// <summary>
        /// Creates a new Curve from a CompCurve
        /// </summary>
        internal PSCurve(PSAutomation powerSHAPE, PSCompCurve compCurveToConvert) : base(powerSHAPE)
        {
            // Selects the CompCurve
            compCurveToConvert.AddToSelection(true);

            // Converts CompCurve into a curve
            _powerSHAPE.DoCommand("CONVERT WIREFRAME");
            compCurveToConvert.Delete();

            // Get the id of the Curve
            PSCurve curve = (PSCurve) _powerSHAPE.ActiveModel.UpdatedItems[0];
            _name = curve.Name;
            _id = curve.Id;
        }

        #endregion

        #region " Properties "

        internal const string CURVE_IDENTIFIER = "CURVE";

        /// <summary>
        /// Gets the identifier PowerSHAPE uses to identify this type of entity
        /// </summary>
        internal override string Identifier
        {
            get { return CURVE_IDENTIFIER; }
        }

        /// <summary>
        /// Gets the composite ID for use in communication with the ActiveDocument object
        /// </summary>
        internal override int CompositeID
        {
            get { return 53 * 20000000 + _id; }
        }

        /// <summary>
        /// Gets the type of a Curve
        /// </summary>
        public CurveTypes Type
        {
            get
            {
                string curveType = _powerSHAPE.ReadStringValue(Identifier + "[ID " + _id + "].TYPE");
                return (CurveTypes) Enum.Parse(typeof(CurveTypes), curveType, true);
            }
        }

        #endregion

        #region " Operations "

        /// <summary>
        /// Deletes the Curve from PowerSHAPE and removes it from the Curves collection
        /// </summary>
        public override void Delete()
        {
            _powerSHAPE.ActiveModel.Curves.Remove(this);
        }

        #region " Edit Operations "

        /// <summary>
        /// Mirrors a single curve in a specified plane
        /// </summary>
        /// <param name="mirrorPlane">Plane about which to mirror the curve</param>
        /// <param name="mirrorPoint">Origin of the mirror plane</param>
        public void Mirror(Planes mirrorPlane, Geometry.Point mirrorPoint)
        {
            PSEntityMirrorer.MirrorEntity(this, mirrorPlane, mirrorPoint);
        }

        /// <summary>
        /// Moves a single curve by the relative distance between two absolute positions
        /// </summary>
        /// <param name="moveOriginCoordinates">First of the two absolute positions</param>
        /// <param name="pointToMoveTo">Second of the two absolute positions</param>
        /// <param name="copiesToCreate">Number of copies that should be created by the operation</param>
        public List<PSEntity> MoveBetweenPoints(
            Geometry.Point moveOriginCoordinates,
            Geometry.Point pointToMoveTo,
            int copiesToCreate)
        {
            return PSEntityMover.MoveEntityBetweenPoints(this, moveOriginCoordinates, pointToMoveTo, copiesToCreate);
        }

        /// <summary>
        /// Moves a single curve by a specified relative amount
        /// </summary>
        /// <param name="moveVector">Relative amount by which the curve will be moved</param>
        /// <param name="copiesToCreate">Number of copies that should be created by the operation</param>
        public List<PSEntity> MoveByVector(Geometry.Vector moveVector, int copiesToCreate)
        {
            return PSEntityMover.MoveEntityByVector(this, moveVector, copiesToCreate);
        }

        /// <summary>
        /// Rotates a single curve by a specified angle around a specified axis
        /// </summary>
        /// <param name="rotationAxis">Axis around which the curve is are to be rotated</param>
        /// <param name="rotationAngle">Angle by which the curve is to be rotated</param>
        /// <param name="copiesToCreate">Number of copies to create of the original curve</param>
        /// <param name="rotationOrigin">Origin of the rotation axis</param>
        /// <returns>List of curves created by the operation.  If numberOfCopies is 0, an empty list is returned</returns>
        public List<PSEntity> Rotate(
            Axes rotationAxis,
            Geometry.Degree rotationAngle,
            int copiesToCreate,
            Geometry.Point rotationOrigin = null)
        {
            return PSEntityRotater.RotateEntity(this, rotationAxis, rotationAngle, copiesToCreate, rotationOrigin);
        }

        /// <summary>
        /// Offsets a single curve by a specified distance
        /// </summary>
        /// <param name="offsetDistance">Distance by which to offset the curve</param>
        /// <param name="copiesToCreate">Number of copies to be created from the operation</param>
        /// <returns>List of offset curves</returns>
        public List<PSEntity> Offset(Geometry.MM offsetDistance, int copiesToCreate)
        {
            return PSEntityOffseter.OffsetEntity(this, offsetDistance, copiesToCreate);
        }

        /// <summary>
        /// Scales curve by a single value. Axis can be locked to prevent scaling on that axis
        /// </summary>
        /// <param name="scaleFactor">Factor to scale by</param>
        /// <param name="lockX">Optionally locks X axis to prevent scaling</param>
        /// <param name="lockY">Optionally locks Y axis to prevent scaling</param>
        /// <param name="lockZ">Optionally locks Z axis to prevent scaling</param>
        /// <param name="scaleOrigin">Origin to scale round</param>
        /// <remarks></remarks>
        public void ScaleUniform(double scaleFactor, bool lockX, bool lockY, bool lockZ, Geometry.Point scaleOrigin = null)
        {
            PSEntityScaler.ScaleUniform(this, scaleFactor, lockX, lockY, lockZ, scaleOrigin);
        }

        /// <summary>
        /// Resizes selected objects in a non-uniform manner, allowing you to choose the scaling to be applied for each axis.
        /// </summary>
        /// <param name="scaleFactorX">Factor to scale by along X</param>
        /// <param name="scaleFactorY">Factor to scale by along Y</param>
        /// <param name="scaleFactorZ">Factor to scale by along Z</param>
        /// <param name="lockX">Optionally locks X axis to prevent scaling</param>
        /// <param name="lockY">Optionally locks Y axis to prevent scaling</param>
        /// <param name="lockZ">Optionally locks Z axis to prevent scaling</param>
        /// <param name="scaleOrigin">Origin to scale round</param>
        /// <remarks></remarks>
        public void ScaleNonUniform(
            double scaleFactorX,
            double scaleFactorY,
            double scaleFactorZ,
            bool lockX,
            bool lockY,
            bool lockZ,
            Geometry.Point scaleOrigin = null)
        {
            PSEntityScaler.ScaleNonUniform(this, scaleFactorX, scaleFactorY, scaleFactorZ, lockX, lockY, lockZ, scaleOrigin);
        }

        /// <summary>
        /// Scales curve by projected volume. Axis can be locked to prevent scaling on that axis.
        /// </summary>
        /// <param name="newVolume">New projected volume</param>
        /// <param name="lockX">Optionally locks X axis to prevent scaling</param>
        /// <param name="lockY">Optionally locks Y axis to prevent scaling</param>
        /// <param name="lockZ">Optionally locks Z axis to prevent scaling</param>
        /// <param name="scaleOrigin">Objects are scaled relative to the scale origin.</param>
        /// <remarks></remarks>
        public void ScaleProjectedVolume(double newVolume, bool lockX, bool lockY, bool lockZ, Geometry.Point scaleOrigin = null)
        {
            PSEntityScaler.ScaleProjectedVolume(this, newVolume, lockX, lockY, lockZ, scaleOrigin);
        }

        #endregion

        #endregion
    }
}