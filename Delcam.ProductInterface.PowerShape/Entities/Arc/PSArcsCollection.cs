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
    /// Captures the collection of Arcs in a Project
    /// </summary>
    public class PSArcsCollection : PSEntitiesCollection<PSArc>
    {
        #region " Fields "

        #endregion

        #region " Constructors "

        /// <summary>
        /// This constructor is required as collection inherits from EntitiesCollection
        /// </summary>
        internal PSArcsCollection(PSAutomation powerSHAPE) : base(powerSHAPE)
        {
        }

        #endregion

        #region " Properties "

        /// <summary>
        /// Gets the identifier used by PowerSHAPE when selecting an Arc
        /// </summary>
        internal override string Identifier
        {
            get { return "ARC"; }
        }

        #endregion

        #region " Operations "

        /// <summary>
        /// Creates an arc with the specified start point and radius.
        /// </summary>
        /// <param name="centre">Centre point for arc</param>
        /// <param name="start">Start point for arc</param>
        /// <param name="radius">Radius for arc</param>
        /// <returns>Arc created by the operation</returns>
        public PSArc CreateArcCircle(Geometry.Point centre, Geometry.Point start, double radius)
        {
            PSArc arc = new PSArc(_powerSHAPE, centre, start, radius);
            Add(arc);
            return arc;
        }

        /// <summary>
        /// Creates an arc between the start and end points,
        /// inferring the radius from the start point.
        /// </summary>
        /// <param name="centre">Centre point for arc</param>
        /// <param name="startPoint">Start point for arc</param>
        /// <param name="endPoint">End point for arc</param>
        /// <returns>Arc created by the operation</returns>
        public PSArc CreateArcSpanImplicit(Geometry.Point centre, Geometry.Point startPoint, Geometry.Point endPoint)
        {
            PSArc arc = new PSArc(_powerSHAPE, centre, startPoint, endPoint);
            Add(arc);
            return arc;
        }

        /// <summary>
        /// Creates an arc from the start point, with the specified radius,
        /// and with the specified span.
        /// </summary>
        /// <param name="centre">Centre point for arc</param>
        /// <param name="start">Start point for arc</param>
        /// <param name="radius">Radius for arc</param>
        /// <param name="span">Span for arc</param>
        /// <returns>Arc created by the operation</returns>
        public PSArc CreateArcSpanExplicit(Geometry.Point centre, Geometry.Point start, double radius, Geometry.Degree span)
        {
            PSArc arc = new PSArc(_powerSHAPE, centre, start, radius, span);
            Add(arc);
            return arc;
        }

        /// <summary>
        /// Creates an arc from the start point to the end point with a specified intermediate point.
        /// To subsequently change the radius to mimic PowerShape functionality, the Radius property should be used.
        /// </summary>
        /// <param name="startPoint">The start point of the arc.</param>
        /// <param name="endPoint">The end point of the arc.</param>
        /// <param name="intermediatePoint">An intermediate point on the span of the arc between the start and end point.</param>
        /// <returns></returns>
        public PSArc CreateArcThroughThreePoints(
            Geometry.Point startPoint,
            Geometry.Point endPoint,
            Geometry.Point intermediatePoint)
        {
            //Clear the list of CreatedItems
            _powerSHAPE.ActiveModel.ClearCreatedItems();

            //Create the line
            _powerSHAPE.DoCommand("CREATE ARC FITTED",
                                  startPoint.X.ToString("0.######") + " " + startPoint.Y.ToString("0.######") + " " + startPoint.Z.ToString("0.######"),
                                  "ABS " + endPoint.X.ToString("0.######") + " " + endPoint.Y.ToString("0.######") + " " + endPoint.Z.ToString("0.######"),
                                  "ABS " + intermediatePoint.X.ToString("0.######") + " " + intermediatePoint.Y.ToString("0.######") + " " +
                                  intermediatePoint.Z.ToString("0.######"),
                                  "OK");

            // Get its id
            PSArc newArc = (PSArc) _powerSHAPE.ActiveModel.CreatedItems[0];
            Add(newArc);
            return newArc;
        }

        /// <summary>
        /// Creates an Arc by allowing the user to sketch through three points
        /// </summary>
        /// <param name="interval">The time in milliseconds between each check to see if the curve has been created yet</param>
        /// <returns>Arc created by the operation</returns>
        public PSArc SketchArcThroughThreePoints(int interval = 500)
        {
            // Start the sketch
            _powerSHAPE.DoCommand("CREATE ARC FITTED");

            // Clear the created items list
            _powerSHAPE.ActiveModel.ClearCreatedItems();

            PSArc arc = null;
            PSModel model = _powerSHAPE.ActiveModel;

            // Keep looping and picking points
            while (arc == null)
            {
                // Wait for half a second to see if a arc has been created yet
                System.Threading.Thread.Sleep(interval);

                // Fire in "OK" every time because if the user has finished sketching they have to confirm the radius before the creation completes
                _powerSHAPE.DoCommand("OK");

                // See if the user finished creating the arc yet
                if (model.CreatedItems.Count == 1)
                {
                    arc = (PSArc) model.CreatedItems[0];

                    // Call select to end the arc creation
                    _powerSHAPE.DoCommand("SELECT");

                    // Add the new arc to the collection
                    model.Arcs.Add(arc);
                }
            }

            return arc;
        }

        #endregion
    }
}