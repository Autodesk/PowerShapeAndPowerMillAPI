// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

namespace Autodesk.ProductInterface.PowerSHAPE
{
    /// <summary>
    /// Captures the collection of Points in a Project.
    /// </summary>
    public class PSPointsCollection : PSEntitiesCollection<PSPoint>
    {
        #region " Constructors "

        /// <summary>
        /// Creates a new collection.
        /// </summary>
        /// <param name="powerSHAPE">The base instance to interact with PowerShape.</param>
        internal PSPointsCollection(PSAutomation powerSHAPE) : base(powerSHAPE)
        {
        }

        #endregion

        #region " Properties "

        /// <summary>
        /// Gets the identifier used by PowerSHAPE when selecting a Point
        /// </summary>
        internal override string Identifier
        {
            get { return "POINT"; }
        }

        #endregion

        #region " Operations "

        /// <summary>
        /// Creates a single point in PowerShape.
        /// </summary>
        /// <param name="point">The point to create.</param>
        /// <returns>The created point.</returns>
        public PSPoint CreatePoint(Geometry.Point point)
        {
            PSPoint newPoint = new PSPoint(_powerSHAPE, point);
            Add(newPoint);
            return newPoint;
        }

        /// <summary>
        /// This operation creates a new Point using the position dialog.
        /// </summary>
        /// <param name="firstPoint">The first point to be used.</param>
        /// <param name="secondPoint">The last point to be used.</param>
        /// <returns>The point between the first and the second points.</returns>
        public PSPoint CreatePointBetweenPoints(Geometry.Point firstPoint, Geometry.Point secondPoint)
        {
            // Create line between the two points
            Geometry.Line lineBetweenPoints = new Geometry.Line(firstPoint, secondPoint);

            // Start create point
            return _powerSHAPE.ActiveModel.Points.CreatePoint(lineBetweenPoints.MidPoint);
        }

        /// <summary>
        /// This operation creates a new PSPoint at the point of intersection of a surface and a wireframe.
        /// Note that if the two do not intersect then a point will be created at the origin of the active workplane.
        /// The user should be sure that the surface and wireframe intersect before calling this function
        /// </summary>
        /// <param name="surface">The surface to create a point on where the wireframe intersects</param>
        /// <param name="wireframe">The wireframe that intersects the surface</param>
        /// <returns>The created PSPoint at the point of intersection</returns>
        public PSPoint CreatePointAtIntersectionOfSurfaceAndWireframe(PSSurface surface, PSWireframe wireframe)
        {
            _powerSHAPE.ActiveModel.ClearSelectedItems();
            _powerSHAPE.DoCommand("CREATE POINT SINGLE", "POSITION", "INTERSECT");
            wireframe.AddToSelection();
            surface.AddToSelection();
            _powerSHAPE.DoCommand("ACCEPT");

            PSPoint point = (PSPoint) _powerSHAPE.ActiveModel.SelectedItems[0];
            AddNoChecks(point);

            return point;
        }

        #endregion
    }
}