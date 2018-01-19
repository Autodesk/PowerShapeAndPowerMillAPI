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
using Autodesk.Geometry;

namespace Autodesk.ProductInterface.PowerSHAPE
{
    /// <summary>
    /// Captures a collection of Curves in a Project
    /// </summary>
    public class PSCurvesCollection : PSGenericCurvesCollection<PSCurve>
    {
        #region " Constructors "

        /// <summary>
        /// This constructor is required as collection inherits from EntitiesCollection
        /// </summary>
        internal PSCurvesCollection(PSAutomation powerSHAPE) : base(powerSHAPE)
        {
        }

        #endregion

        #region " Properties "

        /// <summary>
        /// Gets the identifier used by PowerSHAPE when selecting a Curve
        /// </summary>
        /// <returns></returns>
        /// <value></value>
        internal override string Identifier
        {
            get { return "CURVE"; }
        }

        #endregion

        #region " Operations "

        /// <summary>
        /// Creates a new curve through the specified points
        /// </summary>
        /// <param name="curveType">CurveTypes enum value for curev type</param>
        /// <param name="points">Optional array of points or singular point for curve to go through</param>
        /// <returns>Curve created by the operation</returns>
        public PSCurve CreateCurveThroughPoints(CurveTypes curveType, Point[] points)
        {
            PSCurve curve = new PSCurve(_powerSHAPE, curveType, points);
            _powerSHAPE.ActiveModel.Curves.Add(curve);
            return curve;
        }

        /// <summary>
        /// Creates a Curve from a CompCurve
        /// </summary>
        /// <param name="compCurve">CompCurve to create a curev from</param>
        /// <returns>Curve created by the operation</returns>
        public PSCurve CreateCurveFromCompCurve(PSCompCurve compCurve)
        {
            PSCompCurve newCompCurve = (PSCompCurve) compCurve.Duplicate();

            newCompCurve.AddToSelection(true);
            _powerSHAPE.DoCommand("CONVERT WIREFRAME");

            //Add the new Curve to the collection of Curves
            PSCurve newCurve = null;
            newCurve = (PSCurve) _powerSHAPE.ActiveModel.UpdatedItems[0];
            _powerSHAPE.ActiveModel.Curves.Add(newCurve);

            //Remove duplicate CompCurve from the collection of compcurves
            newCompCurve.Delete();

            return newCurve;
        }

        /// <summary>
        /// Allows the user to sketch a curve.
        /// </summary>
        /// <param name="closeCurve">Forces the user to create a curve that is closed</param>
        /// <param name="interval">The number of milliseconds to wait before each check for a new curve.  Defaults to 0.5seconds</param>
        /// <returns>Curve created by the operation</returns>
        public PSCurve CreateCurveFromSketch(bool closeCurve, int interval = 500)
        {
            // Clear the created items list
            _powerSHAPE.ActiveModel.ClearCreatedItems();

            // Start the sketch
            _powerSHAPE.DoCommand("CREATE CURVE THROUGH");

            PSCurve curve = null;
            PSModel model = _powerSHAPE.ActiveModel;

            // Keep looping and picking points
            while (curve == null)
            {
                // Wait for half a second to see if a Curve has been created yet
                System.Threading.Thread.Sleep(interval);

                // See if the user finished creating the curve yet
                if (model.CreatedItems.Count == 1 && model.CreatedItems[0].GetType() == typeof(PSCurve))
                {
                    curve = (PSCurve) model.CreatedItems[0];

                    // Yes, so do we need to check that it is closed?
                    if (closeCurve && curve.IsClosed == false)
                    {
                        curve.Delete();

                        // Cancel to exit curve creation
                        _powerSHAPE.DoCommand("CANCEL");

                        throw new Exception("User created an open curve");
                    }

                    // Cancel to exit curve creation
                    _powerSHAPE.DoCommand("CANCEL");

                    // Add the new curve to the collection
                    _powerSHAPE.ActiveModel.Curves.Add(curve);
                }
            }

            return curve;
        }

        // TODO: Work out oddities of wrapping onto a solid (reference workplanes being created at strange angles etc)
        /// '
        /// <summary>
        /// ' Thie operation creates new curves by projecting wireframe onto a solid
        /// '
        /// </summary>
        /// '
        /// <param name="solidToWrapTo">The solid that will have wireframe wrapped onto it</param>
        /// '
        /// <param name="wireframeToWrap">The curves or compcurves that will be wrapped</param>
        /// '
        /// <param name="rotation">The desired rotation or the wireframe before wrapping</param>
        /// '
        /// <returns>A list of the created curves</returns>
        /// <summary>
        /// Creates new curves from MSR data
        /// </summary>
        /// <param name="msrFile">The file from which to get the point data</param>
        /// <param name="pointsPerCurve">The number of points in each curve</param>
        /// <returns>A list of created curves</returns>
        public List<PSCurve> CreateCurvesFromMSRFile(MSRFile msrFile, int pointsPerCurve)
        {
            // Create list of created curves
            List<PSCurve> createdCurves = new List<PSCurve>();

            // Create curves from each group of points
            int counter = pointsPerCurve - 1;

            while (counter <= msrFile.Points.Count)
            {
                // Create a list of the curve points
                List<Point> curvePoints = new List<Point>();
                for (int i = counter - pointsPerCurve + 1; i <= counter; i++)
                {
                    curvePoints.Add(msrFile.Points[i].MeasuredSurfacePoint);
                }

                // Create the curve
                createdCurves.Add(CreateCurveThroughPoints(CurveTypes.Bezier, curvePoints.ToArray()));

                counter += pointsPerCurve;
            }

            return createdCurves;
        }

        #endregion
    }
}