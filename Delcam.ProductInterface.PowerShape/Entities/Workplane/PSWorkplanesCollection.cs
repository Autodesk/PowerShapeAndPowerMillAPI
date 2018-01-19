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
    /// Captures the collection of Workplanes in a Project.
    /// </summary>
    public class PSWorkplanesCollection : PSEntitiesCollection<PSWorkplane>
    {
        #region " Constructors "

        /// <summary>
        /// Creates a new collection.
        /// </summary>
        /// <param name="powerSHAPE">The base instance to interact with PowerShape.</param>
        internal PSWorkplanesCollection(PSAutomation powerSHAPE) : base(powerSHAPE)
        {
        }

        #endregion

        #region " Properties "

        /// <summary>
        /// Gets the identifier used by PowerSHAPE when selecting a Workplane.
        /// </summary>
        internal override string Identifier
        {
            get { return "WORKPLANE"; }
        }

        #endregion

        #region " Operations "

        /// <summary>
        /// This operation creates a new Workplane from a Geometry.Workplane and adds it to the collection.
        /// </summary>
        /// <param name="workplane">The workplane from which to create a workplane in PowerSHAPE.</param>
        /// <returns>The created Workplane in PowerSHAPE.</returns>
        public PSWorkplane CreateWorkplane(Geometry.Workplane workplane)
        {
            PSWorkplane newWorkplane = new PSWorkplane(_powerSHAPE, workplane);
            Add(newWorkplane);
            return newWorkplane;
        }

        /// <summary>
        /// This operation creates a new Workplane from just an origin and a Z axis.  The X and Y axes are
        /// created off the Z axis orientation.
        /// </summary>
        /// <param name="origin">The origin at which to create the new workplane.</param>
        /// <param name="zAxis">The Z axis of the new workplane.</param>
        /// <returns>The created workplane.</returns>
        public PSWorkplane CreateWorkplaneFromZAxis(Geometry.Point origin, Geometry.Vector zAxis)
        {
            // Ensure that the z axis is normalised so as not to skew the X and Y axes when they get created
            Geometry.Vector localZAxis = zAxis.Clone();
            localZAxis.Normalize();

            Geometry.Vector xAxis = null;
            Geometry.Vector yAxis = null;
            localZAxis.GetXYVectors(ref xAxis, ref yAxis);

            PSWorkplane newWorkplane = new PSWorkplane(_powerSHAPE, new Geometry.Workplane(origin, xAxis, yAxis, localZAxis));
            Add(newWorkplane);
            return newWorkplane;
        }

        /// <summary>
        /// This operation creates a new Workplane from just an origin and an X and Y axis.  The Z axis is
        /// created off the X and Y axes orientation.
        /// </summary>
        /// <param name="origin">The origin at which to create the new workplane.</param>
        /// <param name="xAxis">The X axis of the new workplane.</param>
        /// <param name="yAxis">The Y axis of the new workplane.</param>
        /// <returns>The created workplane.</returns>
        public PSWorkplane CreateWorkplaneFromXYAxes(Geometry.Point origin, Geometry.Vector xAxis, Geometry.Vector yAxis)
        {
            // Ensure that the x and y axes are normalised so as not to skew the Z axis when it gets created
            Geometry.Vector localXAxis = xAxis.Clone();
            localXAxis.Normalize();
            Geometry.Vector localYAxis = yAxis.Clone();
            localYAxis.Normalize();

            Geometry.Vector zAxis = Geometry.Vector.CrossProduct(localXAxis, localYAxis);

            PSWorkplane newWorkplane =
                new PSWorkplane(_powerSHAPE, new Geometry.Workplane(origin, localXAxis, localYAxis, zAxis));
            Add(newWorkplane);
            return newWorkplane;
        }

        /// <summary>
        /// Creates a new Workplane from three points.
        /// </summary>
        /// <param name="origin">The origin of the workplane to be created.</param>
        /// <param name="xAxisPoint">The point on the intended X axis of the workplane to be created.</param>
        /// <param name="yAxisPoint">The point on the intended Y axis of the workplane to be created.</param>
        /// <returns>The created workplane in PowerSHAPE.</returns>
        public PSWorkplane CreateWorkplaneFromThreePoints(
            Geometry.Point origin,
            Geometry.Point xAxisPoint,
            Geometry.Point yAxisPoint)
        {
            PSWorkplane newWorkplane = new PSWorkplane(_powerSHAPE, origin, xAxisPoint, yAxisPoint);
            Add(newWorkplane);
            return newWorkplane;
        }

        /// <summary>
        /// Creates a new Workplane relative to an entity and adds it to the collection.
        /// </summary>
        /// <param name="entity">The entity on which to create the new Workplane.</param>
        /// <param name="positionAtEntity">The position on the entity at which to create the workplane: Centre, Top, Bottom.</param>
        /// <returns>The created workplane.</returns>
        public PSWorkplane CreateWorkplaneAtEntity(PSEntity entity, NewWorkplaneInEntityPositions positionAtEntity)
        {
            PSWorkplane newWorkplane = new PSWorkplane(_powerSHAPE, entity, positionAtEntity);
            Add(newWorkplane);
            return newWorkplane;
        }

        /// <summary>
        /// Creates a new Workplane relative to an entity and adds it to the collection.
        /// </summary>
        /// <param name="entity">The entity on which to create the new Workplane.</param>
        /// <param name="origin">The intended origin for the workplane.</param>
        /// <returns>The created workplane.</returns>
        public PSWorkplane CreateWorkplaneAlignedToEntity(PSEntity entity, Geometry.Point origin)
        {
            PSWorkplane newWorkplane = new PSWorkplane(_powerSHAPE, entity, origin);
            Add(newWorkplane);
            return newWorkplane;
        }

        /// <summary>
        /// Creates a new Workplane oriented to the current view at the specified origin and adds it to the collection.
        /// </summary>
        /// <param name="origin">The intended origin for the workplane.</param>
        public PSWorkplane CreateWorkplaneAlignedToView(Geometry.Point origin)
        {
            PSWorkplane newWorkplane = new PSWorkplane(_powerSHAPE, origin);
            Add(newWorkplane);
            return newWorkplane;
        }

        /// <summary>
        /// Overrides the AddToSelection in EntitiesCollection.
        /// </summary>
        /// <param name="emptySelectionFirst">The selection CANNOT be cleared first as workplanes cannot be selected along with other Entity types.</param>
        public override void AddToSelection(bool emptySelectionFirst = false)
        {
            // Add all workplanes
            _powerSHAPE.DoCommand("SELECT WORKPLANES");
        }

        #endregion
    }
}