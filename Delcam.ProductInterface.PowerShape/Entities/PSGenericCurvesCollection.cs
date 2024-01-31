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
using System.Linq;
using Autodesk.Geometry;

namespace Autodesk.ProductInterface.PowerSHAPE
{
    /// <summary>
    /// Base class for curve collections
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks></remarks>
    public abstract class PSGenericCurvesCollection<T> : PSEntitiesCollection<T> where T : PSGenericCurve
    {
        #region " Fields "

        #endregion

        #region " Constructors "

        internal PSGenericCurvesCollection(PSAutomation powershape) : base(powershape)
        {
        }

        #endregion

        #region " Properties "

        #endregion

        #region " Operations "

        /// <summary>
        /// Breaks a curve at the specified point.  The first section of
        /// the curve is retained, while the second section becomes a new curve.
        /// </summary>
        /// <param name="initialCurve">The curve to be split.</param>
        /// <param name="breakPoint">The point at which to break the initial curve. Index starts from zero.</param>
        /// <returns>A new curve that is the second portion of the initial.</returns>
        /// <remarks></remarks>
        public T CreateCurveFromBreakPoint(T initialCurve, int breakPoint)
        {
            int numberOfPoints = initialCurve.NumberPoints;
            int maximumIndex = numberOfPoints - 1;

            //call to Powershape

            // Throw exception if breakpoint is not valid
            if (breakPoint < 0)
            {
                throw new ApplicationException("Break point must be a positive integer");
            }
            if (breakPoint > maximumIndex)
            {
                throw new ApplicationException("Break point is greater than the number of curve points");
            }
            if (breakPoint == 0 || breakPoint == maximumIndex)
            {
                throw new ApplicationException("Break point cannot be the end of a curve");
            }

            // Create a copy of the initial curve
            T initialCopy = (T) initialCurve.Duplicate();

            // Remove the relevant points from the initial curve
            int[] pointsToDelete = Enumerable.Range(breakPoint + 1, maximumIndex - breakPoint).ToArray();
            initialCurve.DeleteCurvePoints(pointsToDelete);

            // Remove the relevant points from the copied curve
            pointsToDelete = Enumerable.Range(0, breakPoint).ToArray();
            initialCopy.DeleteCurvePoints(pointsToDelete);

            // Add the copied curve to this collection
            Add(initialCopy);

            // Return the copied curve
            return initialCopy;
        }

        /// <summary>
        /// Creates new curves and compcurves by projecting wireframe onto a surface
        /// </summary>
        /// <param name="surfaceToWrapTo">The surface that will have wireframe wrapped onto it</param>
        /// <param name="wireframeToWrap">The curves and compcurves that will be wrapped</param>
        /// <param name="rotation">The desired rotation or the wireframe before wrapping</param>
        /// <returns>A list of the created curves and compcurves</returns>
        public List<PSGenericCurve> CreateCurvesFromWrap(
            PSSurface surfaceToWrapTo,
            IEnumerable<PSGenericCurve> wireframeToWrap,
            Degree rotation)
        {
            // First page is to select the object to wrap to
            surfaceToWrapTo.AddToSelection(true);

            // Raise the dialog (but not really)
            _powerSHAPE.DoCommand("CREATE CURVE WRAPWIRE");

            // Next page
            _powerSHAPE.DoCommand("NEXT");

            // Select the items to wrap
            foreach (PSGenericCurve wireframe in wireframeToWrap)
            {
                wireframe.AddToSelection(false);
            }

            // Next page
            _powerSHAPE.DoCommand("NEXT");

            // Select workplane
            _powerSHAPE.DoCommand("SELECTWORKPLANE REFERENCE");

            // Next page
            _powerSHAPE.DoCommand("NEXT");

            // Select method
            _powerSHAPE.DoCommand("METHOD CHORD");

            // Next page
            _powerSHAPE.DoCommand("NEXT");

            // Select distortion
            _powerSHAPE.DoCommand("MINDISTORTION X_AXIS");

            // Next page
            _powerSHAPE.DoCommand("NEXT");

            // Set rotation angle
            _powerSHAPE.DoCommand("ROTATE " + rotation);

            // Next page
            _powerSHAPE.DoCommand("NEXT");

            // Next page
            _powerSHAPE.DoCommand("FINISH");

            // Get the list of everything selected
            List<PSGenericCurve> wrappedCurves = new List<PSGenericCurve>();
            foreach (PSGenericCurve wrappedCurve in _powerSHAPE.ActiveModel.SelectedItems)
            {
                wrappedCurves.Add(wrappedCurve);
            }

            // Need to find out if any workplanes got created and add them to the workplanes collection
            _powerSHAPE.DoCommand("SELECT WORKPLANES");
            int numberOfWorkplanes = _powerSHAPE.ActiveModel.SelectedItems.Count;
            for (int i = 0; i <= numberOfWorkplanes - 1; i++)
            {
                // Get new workplane
                PSWorkplane workplane = (PSWorkplane) _powerSHAPE.ActiveModel.SelectedItems[i];

                // The workplane is added to the active collection only if it is not already in there
                _powerSHAPE.ActiveModel.Workplanes.Add(workplane);
            }

            // Return the curves
            return wrappedCurves;
        }

        #endregion
    }
}