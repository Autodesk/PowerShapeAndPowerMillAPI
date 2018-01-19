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
using DG = Autodesk.Geometry;

namespace Autodesk.ProductInterface.PowerSHAPE
{
    /// <summary>
    /// Helps to mirror entities.
    /// </summary>
    /// <remarks></remarks>
    public class PSEntityMirrorer
    {
        #region " Fields "

        private static PSAutomation _powerSHAPE;

        #endregion

        #region " Constructors "

        /// <summary>
        /// Initialises new instance.
        /// </summary>
        /// <param name="powerSHAPE">The base instance to interact with PowerShape.</param>
        /// <remarks></remarks>
        public PSEntityMirrorer(PSAutomation powerSHAPE)
        {
            _powerSHAPE = powerSHAPE;
        }

        #endregion

        #region " Operations "

        /// <summary>
        /// Mirrors entities about the specifed plane.
        /// </summary>
        /// <param name="entitiesToMirror">The entities that are to be mirrored.</param>
        /// <param name="mirrorPlane">The plane about which to mirror the entities.</param>
        /// <param name="mirrorPoint">The origin of the mirror plane.</param>
        public static void MirrorEntities(
            List<IPSMirrorable> entitiesToMirror,
            Planes mirrorPlane,
            Geometry.Point mirrorPoint)
        {
            // Get PowerSHAPE instance
            _powerSHAPE = ((PSEntity) entitiesToMirror[0]).PowerSHAPE;

            // Carry out operation
            // Create a temporary workplane (store the current workplane, to restore after mirror)
            PSWorkplane activeWP = _powerSHAPE.ActiveModel.ActiveWorkplane;
            _powerSHAPE.SetActivePlane(Planes.XY);

            // Ensures that the orientation of tempWP matches activeWP
            _powerSHAPE.ActiveModel.CreateTemporaryWorkplane(mirrorPoint);

            // Add all IPSMirrorables to selection
            ((PSEntity) entitiesToMirror[0]).AddToSelection(true);
            int numberOfEntities = entitiesToMirror.Count();
            for (int i = 0; i <= numberOfEntities - 1; i++)
            {
                ((PSEntity) entitiesToMirror[i]).AddToSelection(false);
            }

            // Clear updated items
            _powerSHAPE.ActiveModel.ClearUpdatedItems();

            // Mirror in this temporary workplane
            _powerSHAPE.DoCommand("EDIT MIRROR");
            _powerSHAPE.DoCommand("NOKEEP");

            _powerSHAPE.DoCommand(mirrorPlane.ToString());

            // This command is needed only for build 11 upward
            if (_powerSHAPE.Version >= new Version("11.2"))
            {
                _powerSHAPE.DoCommand("APPLY");
            }
            _powerSHAPE.DoCommand("CANCEL");

            int numberOfNonSolids = 0;
            for (int i = 0; i <= numberOfEntities - 1; i++)
            {
                if (!(entitiesToMirror[i] is PSSolid))
                {
                    numberOfNonSolids += 1;
                }
            }

            // Check that entities were mirrored, but only if non-solids were passed in, as powershape does not add altered solids to the updated set
            if (numberOfNonSolids > 0)
            {
                if (_powerSHAPE.ActiveModel.UpdatedItems.Count == 0)
                {
                    throw new ApplicationException("No entities were mirrored");
                }
                if (_powerSHAPE.ActiveModel.UpdatedItems.Count != numberOfNonSolids)
                {
                    throw new ApplicationException("Not all entities were mirrored");
                }
            }

            // Delete the temporary workplane that was created for the operation
            _powerSHAPE.ActiveModel.DeleteTemporaryWorkplane();

            // Restore the active workplane
            _powerSHAPE.ActiveModel.ActiveWorkplane = activeWP;
        }

        /// <summary>
        /// Mirrors a single entity about the specifed plane.
        /// </summary>
        /// <param name="entityToMirror">The single entity that is to be mirrored.</param>
        /// <param name="mirrorPlane">The plane about which to mirror the entity.</param>
        /// <param name="mirrorPoint">The origin of the mirror plane.</param>
        public static void MirrorEntity(IPSMirrorable entityToMirror, Planes mirrorPlane, Geometry.Point mirrorPoint)
        {
            // Create a list of the single entity
            List<IPSMirrorable> entityToMirrorList = new List<IPSMirrorable>();
            entityToMirrorList.Add(entityToMirror);

            // Carry out move operation
            MirrorEntities(entityToMirrorList, mirrorPlane, mirrorPoint);
        }

        #endregion
    }
}