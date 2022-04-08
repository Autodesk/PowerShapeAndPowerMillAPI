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
    /// Helps to move entities.
    /// </summary>
    /// <remarks></remarks>
    public class PSEntityMover
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
        public PSEntityMover(PSAutomation powerSHAPE)
        {
            _powerSHAPE = powerSHAPE;
        }

        #endregion

        #region " Operations "

        /// <summary>
        /// Starts the Move operation.
        /// </summary>
        /// <param name="entitiesToMove">The group of entities that are to be moved.</param>
        /// <param name="copiesToCreate">The number of copies to create.</param>
        private static void SetupMove(List<IPSMoveable> entitiesToMove, int copiesToCreate)
        {
            // Get PowerSHAPE instance
            _powerSHAPE = ((PSEntity) entitiesToMove[0]).PowerSHAPE;

            // Add all IMovables
            ((PSEntity) entitiesToMove[0]).AddToSelection(true);
            int numberOfEntities = entitiesToMove.Count();
            for (int i = 1; i <= numberOfEntities - 1; i++)
            {
                ((PSEntity) entitiesToMove[i]).AddToSelection(false);
            }

            // Clear created items
            _powerSHAPE.ActiveModel.ClearCreatedItems();

            // Start move operation
            _powerSHAPE.DoCommand("EDIT MOVE");

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
        }

        /// <summary>
        /// Finishes the Move operation.
        /// </summary>
        /// <returns>A list of the copied entities that have been created.  If numberOfCopies was 0, an empty list is returned</returns>
        private static List<PSEntity> FinishMove()
        {
            // Clear selection
            _powerSHAPE.ActiveModel.ClearSelectedItems();

            // If no copies were made, return an empty list
            if (_powerSHAPE.ActiveModel.CreatedItems.Count == 0)
            {
                return new List<PSEntity>();
            }

            // Check copies were made
            if (_powerSHAPE.ActiveModel.CreatedItems.Count == 0)
            {
                throw new ApplicationException("No entities were moved");
            }

            // Return copied entities and add them to their appropriate collections
            List<PSEntity> copiedEntities = new List<PSEntity>();
            foreach (PSEntity copiedEntity in _powerSHAPE.ActiveModel.CreatedItems)
            {
                _powerSHAPE.ActiveModel.Add(copiedEntity);
                copiedEntities.Add(copiedEntity);
            }

            return copiedEntities;
        }

        /// <summary>
        /// Moves a group of entities by a specified relative amount.
        /// </summary>
        /// <param name="entitiesToMove">The group of entities that are to be moved.</param>
        /// <param name="moveVector">The relative amount by which each entity will be moved.</param>
        /// <param name="copiesToCreate">The number of copies that should be created by the operation.</param>
        /// <returns>A list containing any created copies of the original entities.  If copiesToCreate is 0, the operation returns an empty list.</returns>
        public static List<PSEntity> MoveEntitiesByVector(
            List<IPSMoveable> entitiesToMove,
            Geometry.Vector moveVector,
            int copiesToCreate)
        {
            SetupMove(entitiesToMove, copiesToCreate);

            // Carry out the process
            if (_powerSHAPE.Version >= new Version("11.2"))
            {
                _powerSHAPE.DoCommand("X " + moveVector.I.ToString("0.######"),
                                      "Y " + moveVector.J.ToString("0.######"),
                                      "Z " + moveVector.K.ToString("0.######"));
                _powerSHAPE.DoCommand("APPLY", "DISMISS");
            }
            else
            {
                _powerSHAPE.DoCommand(moveVector.I.ToString("0.######") + " " + moveVector.J.ToString("0.######") + " " + moveVector.K.ToString("0.######"));
                _powerSHAPE.DoCommand("CANCEL");
            }

            return FinishMove();
        }

        /// <summary>
        /// .Moves a single entity by a specified relative amount.
        /// </summary>
        /// <param name="entityToMove">The single entity that is to be moved.</param>
        /// <param name="moveVector">The relative amount by which the entity will be moved.</param>
        /// <param name="copiesToCreate">The number of copies that should be created by the operation.</param>
        public static List<PSEntity> MoveEntityByVector(IPSMoveable entityToMove, Geometry.Vector moveVector, int copiesToCreate)
        {
            // Create a list of the single entity
            List<IPSMoveable> entityToMoveList = new List<IPSMoveable>();
            entityToMoveList.Add(entityToMove);

            // Carry out move operation
            return MoveEntitiesByVector(entityToMoveList, moveVector, copiesToCreate);
        }

        /// <summary>
        /// Moves a group of entities by the relative distance between two absolute positions..
        /// </summary>
        /// <param name="entitiesToMove">The group of entities that are to be moved.</param>
        /// <param name="moveOriginCoordinates">The first of the two absolute positions.</param>
        /// <param name="pointToMoveTo">The second of the two absolute positions.</param>
        /// <param name="copiesToCreate">The number of copies that should be created by the operation.</param>
        /// <returns>A list containing any created copies of the original entities.  If copiesToCreate is 0, the operation returns an empty list.</returns>
        public static List<PSEntity> MoveEntitiesBetweenPoints(
            List<IPSMoveable> entitiesToMove,
            Geometry.Point moveOriginCoordinates,
            Geometry.Point pointToMoveTo,
            int copiesToCreate)
        {
            SetupMove(entitiesToMove, copiesToCreate);

            // Move the origin
            _powerSHAPE.DoCommand("MOVEORIGIN");
            _powerSHAPE.DoCommand("ABS " + moveOriginCoordinates);

            // Carry out the process
            _powerSHAPE.DoCommand("ABS " + pointToMoveTo.X.ToString("0.######") + " " + pointToMoveTo.Y.ToString("0.######") + " " +
                                  pointToMoveTo.Z.ToString("0.######"));

            if (_powerSHAPE.Version >= new Version("11.2"))
            {
                _powerSHAPE.DoCommand("APPLY", "DISMISS");
            }
            else
            {
                _powerSHAPE.DoCommand("CANCEL");
            }
            _powerSHAPE.DoCommand("CANCEL");

            return FinishMove();
        }

        /// <summary>
        /// Moves a single entity by the relative distance between two absolute positions.
        /// </summary>
        /// <param name="entityToMove">The single entity that is to be moved.</param>
        /// <param name="moveOriginCoordinates">The first of the two absolute positions.</param>
        /// <param name="pointToMoveTo">The second of the two absolute positions.</param>
        /// <param name="copiesToCreate">The number of copies that should be created by the operation.</param>
        public static List<PSEntity> MoveEntityBetweenPoints(
            IPSMoveable entityToMove,
            Geometry.Point moveOriginCoordinates,
            Geometry.Point pointToMoveTo,
            int copiesToCreate)
        {
            // Create a list of the single entity
            List<IPSMoveable> entityToMoveList = new List<IPSMoveable>();
            entityToMoveList.Add(entityToMove);

            // Carry out move operation
            return MoveEntitiesBetweenPoints(entityToMoveList, moveOriginCoordinates, pointToMoveTo, copiesToCreate);
        }

        #endregion
    }
}