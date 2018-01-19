// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using DG = Autodesk.Geometry;

namespace Autodesk.ProductInterface.PowerSHAPE
{
    /// <summary>
    /// Entity cutter helper.
    /// </summary>
    /// <remarks></remarks>
    public class PSEntityCutter
    {
        #region " Fields "

        private static PSAutomation _powerSHAPE;

        #endregion

        #region " Constructors "

        /// <summary>
        /// Creates a new entity cutter instance.
        /// </summary>
        /// <param name="powerSHAPE">The base instance to interact with PowerShape.</param>
        /// <remarks></remarks>
        public PSEntityCutter(PSAutomation powerSHAPE)
        {
            _powerSHAPE = powerSHAPE;
        }

        #endregion

        #region " Operations "

        /// <summary>
        /// Cuts a single entity at a defined point.
        /// </summary>
        /// <param name="entityToCut">The entity to cut.</param>
        /// <param name="pointAtWhichToCut">The point at which to cut the entity, which must lie on the entity.</param>
        /// <returns>A piece of the original entity.</returns>
        public static PSEntity CutEntity(PSEntity entityToCut, Geometry.Point pointAtWhichToCut)
        {
            // Get PowerSHAPE instance
            _powerSHAPE = entityToCut.PowerSHAPE;

            // Cut entity
            entityToCut.AddToSelection(true);
            _powerSHAPE.DoCommand("EDIT LIMIT CUTS");
            _powerSHAPE.DoCommand(pointAtWhichToCut.ToString());
            _powerSHAPE.DoCommand("EDIT LIMIT CUTS OFF");

            // Check that operation worked
            if (_powerSHAPE.ActiveModel.CreatedItems.Count == 0)
            {
                throw new ApplicationException("Cut operation did not function correctly");
            }

            // Get created entity
            PSEntity newEntity = _powerSHAPE.ActiveModel.CreatedItems[0];
            newEntity.Id = _powerSHAPE.ReadIntValue(newEntity.Identifier + "['" + newEntity.Name + "'].ID");
            _powerSHAPE.ActiveModel.Add(newEntity);
            return newEntity;
        }

        #endregion
    }
}