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
    /// Base class for PSLateralsCollection and PSLongitudinalsCollection.
    /// </summary>
    /// <typeparam name="T">The type of PSSurfaceCurve.</typeparam>
    /// <remarks></remarks>
    public abstract class PSSurfaceCurvesCollection<T> : PSEntitiesCollection<T> where T : PSSurfaceCurve
    {
        #region " Fields "

        /// <summary>
        /// The parent surface of a PSSurface.
        /// </summary>
        /// <remarks></remarks>
        protected PSSurface _parentSurface;

        #endregion

        #region " Constructors "

        /// <summary>
        /// Creates a new collection.
        /// </summary>
        /// <param name="powershape">The base instance to interact with PowerShape.</param>
        /// <param name="parentSurface">The parent surface for the lateral.</param>
        public PSSurfaceCurvesCollection(PSAutomation powershape, PSSurface parentSurface) : base(powershape)
        {
            _parentSurface = parentSurface;
        }

        #endregion

        #region " Operations "

        /// <summary>
        /// Removes surface curve from PowerShape and from surface curves collection.
        /// </summary>
        /// <param name="surfaceCurve">The surface curve to delete.</param>
        /// <returns>True if it removes curve from curves collection. False if it fails to remove curve from collection, but curve will be removed from PowerShape.</returns>
        public new bool Remove(T surfaceCurve)
        {
            // Delete the item from PowerSHAPE
            if (surfaceCurve.Exists)
            {
                // Add to selection
                surfaceCurve.AddToSelection(true);

                // Carry out operation
                _powerSHAPE.DoCommand("DELETE_CURVE");
            }

            // PowerSHAPE has updated names and ids of the remaining surface curves which lay higher than the
            // one that was deleted, so these need to be changed to reflect that
            for (int i = int.Parse(surfaceCurve.Name); i <= Count - 1; i++)
            {
                this[i].Name = (Convert.ToInt32(this[i].Name) - 1).ToString();
                this[i].Id = this[i].Id - 1;
            }

            // Check whether entity exists and its name matches that to be removed
            if (Contains(surfaceCurve))
            {
                //Return _list.Remove(item)
                RemoveAt(IndexOf(surfaceCurve));
                return true;
            }

            return false;
        }

        #endregion
    }
}