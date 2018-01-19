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
using Autodesk.Extensions;
using DG = Autodesk.Geometry;

namespace Autodesk.ProductInterface.PowerSHAPE
{
    /// <summary>
    /// Helper to rotate entities.
    /// </summary>
    /// <remarks></remarks>
    public class PSEntityRotater
    {
        #region " Fields "

        private static PSAutomation _powerSHAPE;

        #endregion

        #region " Constructors "

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="powerSHAPE">The base instance to interact with PowerShape.</param>
        /// <remarks></remarks>
        public PSEntityRotater(PSAutomation powerSHAPE)
        {
            _powerSHAPE = powerSHAPE;
        }

        #endregion

        #region " Operations "

        /// <summary>
        /// Rotates a group of entities by a specified angle around a specified axis.
        /// </summary>
        /// <param name="entitiesToRotate">The group of entities that are to be rotated.</param>
        /// <param name="rotationAxis">The axis around which the entities are to be rotated.</param>
        /// <param name="rotationAngle">The angle by which the entities are to be rotated.</param>
        /// <param name="copiesToCreate">The number of copies to create of the original entities.</param>
        /// <param name="rotationOrigin">The origin of the rotation axis.</param>
        /// <returns>A list of entities created by the operation.  If numberOfCopies is 0, an .empty list is returned.</returns>
        public static List<PSEntity> RotateEntities(
            List<IPSRotateable> entitiesToRotate,
            Axes rotationAxis,
            DG.Degree rotationAngle,
            int copiesToCreate,
            DG.Point rotationOrigin = null)
        {
            // Get PowerSHAPE instance
            _powerSHAPE = ((PSEntity) entitiesToRotate.First()).PowerSHAPE;

            // Add all IRotatables
            ((PSEntity) entitiesToRotate.First()).AddToSelection(true);
            int numberOfEntities = entitiesToRotate.Count();
            for (int i = 1; i <= numberOfEntities - 1; i++)
            {
                ((PSEntity) entitiesToRotate[i]).AddToSelection(false);
            }

            // Carry out operation
            _powerSHAPE.DoCommand("EDIT ROTATE");

            // Determine whether a copy is to be created
            if (copiesToCreate == 0)
            {
                _powerSHAPE.DoCommand("NOKEEP");
            }
            else
            {
                _powerSHAPE.DoCommand("KEEP");
                _powerSHAPE.DoCommand("COPIES " + copiesToCreate);
            }

            // If a different rotation origin has been defined, set it within PowerSHAPE
            if (rotationOrigin != null)
            {
                _powerSHAPE.DoCommand("AXIS");
                _powerSHAPE.DoCommand(rotationOrigin.ToString());
            }

            // Set the active plane, which determines the axis of rotation
            _powerSHAPE.SetActivePlane(rotationAxis.AxisToPlane());

            // Enter rotation angle
            _powerSHAPE.DoCommand("ANGLE " + rotationAngle);
            if (_powerSHAPE.Version >= new Version("11.2"))
            {
                _powerSHAPE.DoCommand("APPLY", "DISMISS");
            }
            else
            {
                _powerSHAPE.DoCommand("CANCEL");
            }

            // If no copies were made, return an empty list
            if (copiesToCreate == 0)
            {
                return new List<PSEntity>();
            }

            // If copies were made, add them to their appropriate collections and return new entities
            List<PSEntity> copiedEntities = new List<PSEntity>();
            foreach (PSEntity copiedEntity in _powerSHAPE.ActiveModel.CreatedItems)
            {
                _powerSHAPE.ActiveModel.Add(copiedEntity);
                copiedEntities.Add(copiedEntity);
            }

            return copiedEntities;
        }

        /// <summary>
        /// Rotates a single entity by a specified angle around a specified axis.
        /// </summary>
        /// <param name="entityToRotate">The single entity that is to be rotated.</param>
        /// <param name="rotationAxis">The axis around which the entity is are to be rotated.</param>
        /// <param name="rotationAngle">The angle by which the entity is to be rotated.</param>
        /// <param name="copiesToCreate">The number of copies to create of the original entity.</param>
        /// <param name="rotationOrigin">The origin of the rotation axis.</param>
        /// <returns>A list of entities created by the operation.  If numberOfCopies is 0, an empty list is returned.</returns>
        public static List<PSEntity> RotateEntity(
            IPSRotateable entityToRotate,
            Axes rotationAxis,
            DG.Degree rotationAngle,
            int copiesToCreate,
            DG.Point rotationOrigin = null)
        {
            // Create a list of the single entity
            List<IPSRotateable> entityToRotateList = new List<IPSRotateable>();
            entityToRotateList.Add(entityToRotate);

            // Carry out move operation
            return RotateEntities(entityToRotateList, rotationAxis, rotationAngle, copiesToCreate, rotationOrigin);
        }

        #endregion
    }
}