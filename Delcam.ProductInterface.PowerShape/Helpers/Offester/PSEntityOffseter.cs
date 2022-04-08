// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System.Collections.Generic;
using System.Linq;
using DG = Autodesk.Geometry;

namespace Autodesk.ProductInterface.PowerSHAPE
{
    /// <summary>
    /// Offseter helper for entities.
    /// </summary>
    /// <remarks></remarks>
    public class PSEntityOffseter
    {
        #region " Fields "

        private static PSAutomation _powerSHAPE;

        #endregion

        #region " Constructors "

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="powerSHAPE">The base instance to interact with PowerShape.</param>
        /// <remarks></remarks>
        public PSEntityOffseter(PSAutomation powerSHAPE)
        {
            _powerSHAPE = powerSHAPE;
        }

        #endregion

        #region " Operations "

        /// <summary>
        /// Offsets a group of entities by a specified distance.
        /// </summary>
        /// <param name="entitiesToOffset">The group of entities that are to be offset.</param>
        /// <param name="offsetDistance">The distance the entities are to be offset by.</param>
        /// <param name="copiesToCreate">The number of copies to create of the origina.l entities.</param>
        /// <returns>A list of entities created by the operation.  If numberOfCopies is 0, an empty list is returned.</returns>
        public static List<PSEntity> OffsetEntities(
            List<IPSOffsetable> entitiesToOffset,
            Geometry.MM offsetDistance,
            int copiesToCreate)
        {
            // Get PowerSHAPE instance
            _powerSHAPE = ((PSEntity) entitiesToOffset[0]).PowerSHAPE;

            // Add all IPSOffsetables
            ((PSEntity) entitiesToOffset[0]).AddToSelection(true);
            int numberOfEntities = entitiesToOffset.Count();
            for (int i = 1; i <= numberOfEntities - 1; i++)
            {
                ((PSEntity) entitiesToOffset[i]).AddToSelection(false);
            }

            // Carry out operation
            _powerSHAPE.DoCommand("EDIT OFFSET");

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

            // Enter offset distance
            _powerSHAPE.DoCommand("DISTANCE " + offsetDistance.ToString("0.######"));
            _powerSHAPE.DoCommand("CANCEL");

            // If no copies were made, return an empty list
            if (copiesToCreate == 0)
            {
                // If any of the entities were meshes, curve, compcurve or arcs then we need to reassign the Ids
                var resultEntities = _powerSHAPE.ActiveModel.SelectedItems;
                foreach (PSEntity entity in entitiesToOffset)
                {
                    if (entity is PSMesh)
                    {
                        foreach (PSEntity newEntity in resultEntities)
                        {
                            if (newEntity is PSMesh && newEntity.Name == entity.Name)
                            {
                                entity.Id = newEntity.Id;
                                break; // TODO: might not be correct. Was : Exit For
                            }
                        }
                    }
                    if (entity is PSArc)
                    {
                        foreach (PSEntity newEntity in resultEntities)
                        {
                            if (newEntity is PSArc && newEntity.Name == entity.Name)
                            {
                                entity.Id = newEntity.Id;
                                break; // TODO: might not be correct. Was : Exit For
                            }
                        }
                    }
                    if (entity is PSCompCurve)
                    {
                        foreach (PSEntity newEntity in resultEntities)
                        {
                            if (newEntity is PSCompCurve && newEntity.Name == entity.Name)
                            {
                                entity.Id = newEntity.Id;
                                break; // TODO: might not be correct. Was : Exit For
                            }
                        }
                    }
                    if (entity is PSCurve)
                    {
                        foreach (PSEntity newEntity in resultEntities)
                        {
                            if (newEntity is PSCurve && newEntity.Name == entity.Name)
                            {
                                entity.Id = newEntity.Id;
                                break; // TODO: might not be correct. Was : Exit For
                            }
                        }
                    }
                }
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
        /// Offsets a single entity by a specified distance.
        /// </summary>
        /// <param name="entityToOffset">The single entity that is to be offset.</param>
        /// <param name="offsetDistance">The offset value to be applied.</param>
        /// <param name="copiesToCreate">The number of copies to create of the original entity.</param>
        /// <returns>A list of entities created by the operation.  If numberOfCopies is 0, an empty list is returned.</returns>
        /// <remarks></remarks>
        public static List<PSEntity> OffsetEntity(IPSOffsetable entityToOffset, Geometry.MM offsetDistance, int copiesToCreate)
        {
            // Create a list of the single entity
            List<IPSOffsetable> entityToOffsetList = new List<IPSOffsetable>();
            entityToOffsetList.Add(entityToOffset);

            // Carry out move operation
            return OffsetEntities(entityToOffsetList, offsetDistance, copiesToCreate);
        }

        #endregion
    }
}