// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DG = Autodesk.Geometry;

[assembly:
    InternalsVisibleTo(
        "Autodesk.ProductInterface.PowerSHAPE.Test,PublicKey=0024000004800000940000000602000000240000525341310004000001000100bf63e2c23194ffb896e84c5d483c8c87de8761893bb3302cfaab0cef2949259964ef260ee1c12475d7446a584d7797a83c31ae05b72975c3130cc4c1ebf8f53fb90332d3b691591856110c9c9198a9137ff575b7921ac19d323df6e1bd96005373a2d2daff2d25e128433b149c91c2dfa4987b7fc196f0e431409b509e027db3")]

namespace Autodesk.ProductInterface.PowerSHAPE
{
    /// <summary>
    /// Contains the necessary code to limit entities.
    /// </summary>
    /// <remarks>Must be public for the enumerations to be accessible.</remarks>
    public class PSEntityLimiter
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
        internal PSEntityLimiter(PSAutomation powerSHAPE)
        {
            _powerSHAPE = powerSHAPE;
        }

        #endregion

        #region " Operations "

        /// <summary>
        /// Limits entities using a list of entities.
        /// </summary>
        /// <param name="entitiesToLimit">The entities on which to perform the limiting operation.</param>
        /// <param name="limitingEntities">The entities with which to perform the limiting operation.</param>
        /// <param name="limitingMode">The mode in which to carry out the operation.</param>
        /// <param name="keepOption">Whether to keep one or both sides of the limit.</param>
        /// <param name="trimOption">Whether to trim one or all of the entities.</param>
        internal static List<PSEntity> LimitEntities(
            List<IPSLimitable> entitiesToLimit,
            List<PSEntity> limitingEntities,
            LimitingModes limitingMode = LimitingModes.LimitMode,
            LimitingKeepOptions keepOption = LimitingKeepOptions.KeepOne,
            LimitingTrimOptions trimOption = LimitingTrimOptions.LimitOne,
            bool finishOperation = true)
        {
            // Get PowerSHAPE instance
            _powerSHAPE = ((PSEntity) entitiesToLimit[0]).PowerSHAPE;

            // Clear the selection
            _powerSHAPE.ActiveModel.ClearSelectedItems();

            // Select all the entities with which to do the limit
            foreach (PSEntity entity in limitingEntities)
            {
                entity.AddToSelection(false);
            }

            // Do the limit
            _powerSHAPE.DoCommand("EDIT SELECTION");
            _powerSHAPE.DoCommand(keepOption.ToString());
            if (trimOption == LimitingTrimOptions.LimitOne)
            {
                _powerSHAPE.DoCommand("TRIM ONE");
            }
            else
            {
                _powerSHAPE.DoCommand("TRIM BOTH");
            }
            switch (limitingMode)
            {
                case LimitingModes.LimitMode:
                    _powerSHAPE.DoCommand("PROFILING LIMIT");
                    break;
                case LimitingModes.ProjectMode:
                    _powerSHAPE.DoCommand("PROFILING PROJECT");
                    break;
                case LimitingModes.ProjectViewMode:
                    _powerSHAPE.DoCommand("PROFILING PROJECT_VIEW");
                    break;
                case LimitingModes.ProjectSurfaceNormalMode:
                    _powerSHAPE.DoCommand("PROFILING MAKEPCU");
                    break;
                case LimitingModes.IntersectCurveMode:
                    _powerSHAPE.DoCommand("PROFILING SURFCURVEINT");
                    break;
            }

            // Select all the entities on which to perform the limit
            foreach (PSEntity entity in entitiesToLimit)
            {
                entity.AddToSelection(false);
            }

            // Add any new entities to the list of entities
            List<PSEntity> newEntities = new List<PSEntity>();
            if (_powerSHAPE.ActiveModel.CreatedItems.Count == 0)
            {
                return newEntities;
            }
            foreach (PSEntity newEntity in _powerSHAPE.ActiveModel.CreatedItems)
            {
                newEntity.Id = _powerSHAPE.ReadIntValue(newEntity.Identifier + "['" + newEntity.Name + "'].ID");
                _powerSHAPE.ActiveModel.Add(newEntity);
                newEntities.Add(newEntity);
            }

            //Ensure also updated entities are retrieved too when keep both is selected
            if (keepOption == LimitingKeepOptions.KeepBoth)
            {
                foreach (PSEntity itme in _powerSHAPE.ActiveModel.UpdatedItems)
                {
                    var updatedEntity = itme;
                    updatedEntity.Id = _powerSHAPE.ReadIntValue(updatedEntity.Identifier + "['" + updatedEntity.Name + "'].ID");
                    if (!newEntities.Exists(x => x.Id == updatedEntity.Id))
                    {
                        newEntities.Add(updatedEntity);
                    }
                }
            }

            // Finish operation
            if (finishOperation)
            {
                _powerSHAPE.DoCommand("EDIT SELECTION OFF");
            }

            // Doing this changes the ID of the new entities but the names stay the same so get the new IDs
            foreach (PSEntity limitedEntity in entitiesToLimit)
            {
                limitedEntity.Id = _powerSHAPE.ReadIntValue(limitedEntity.Identifier + "['" + limitedEntity.Name + "'].ID");
            }

            return newEntities;
        }

        /// <summary>
        /// Limits a single entity using a list of entities.
        /// </summary>
        /// <param name="entityToLimit">The entity on which to perform the limiting operation.</param>
        /// <param name="limitingEntity">The entity with which to perform the limiting operation.</param>
        /// <param name="limitingMode">The mode in which to carry out the operation.</param>
        /// <param name="keepOption">Whether to keep one or both sides of the limit.</param>
        /// <param name="trimOption">Whether to trim one or all of the entities.</param>
        internal static List<PSEntity> LimitEntity(
            IPSLimitable entityToLimit,
            PSEntity limitingEntity,
            LimitingModes limitingMode = LimitingModes.LimitMode,
            LimitingKeepOptions keepOption = LimitingKeepOptions.KeepOne,
            LimitingTrimOptions trimOption = LimitingTrimOptions.LimitOne,
            bool finishOperation = true)
        {
            // Create a list of the single entity
            List<PSEntity> limitingEntities = new List<PSEntity>();
            limitingEntities.Add(limitingEntity);

            // Carry out limit operation
            return LimitEntity(entityToLimit, limitingEntities, limitingMode, keepOption, trimOption, finishOperation);
        }

        /// <summary>
        /// Limits a single entity using a list of entities.
        /// </summary>
        /// <param name="entityToLimit">The entity on which to perform the limiting operation.</param>
        /// <param name="limitingEntities">The entities with which to perform the limiting operation.</param>
        /// <param name="limitingMode">The mode in which to carry out the operation.</param>
        /// <param name="keepOption">Whether to keep one or both sides of the limit.</param>
        /// <param name="trimOption">Whether to trim one or all of the entities.</param>
        internal static List<PSEntity> LimitEntity(
            IPSLimitable entityToLimit,
            List<PSEntity> limitingEntities,
            LimitingModes limitingMode = LimitingModes.LimitMode,
            LimitingKeepOptions keepOption = LimitingKeepOptions.KeepOne,
            LimitingTrimOptions trimOption = LimitingTrimOptions.LimitOne,
            bool finishOperation = true)
        {
            // Create a list of the single entity
            List<IPSLimitable> entitiesToLimit = new List<IPSLimitable>();
            entitiesToLimit.Add(entityToLimit);

            // Carry out limit operation
            return LimitEntities(entitiesToLimit, limitingEntities, limitingMode, keepOption, trimOption, finishOperation);
        }

        /// <summary>
        /// This operation will cycle to the next solution for the last limit function. To accept call AcceptLimit.
        /// </summary>
        /// <param name="entityToLimit">The entity to limit.</param>
        /// <remarks></remarks>
        public static void NextSolution(IPSLimitable entityToLimit)
        {
            _powerSHAPE.DoCommand("NEXT SOLUTION");

            // Doing this changes the id but the name stays the same so get the new ID
            ((PSEntity) entityToLimit).Id =
                _powerSHAPE.ReadIntValue(((PSEntity) entityToLimit).Identifier + "['" + ((PSEntity) entityToLimit).Name +
                                         "'].ID");
        }

        /// <summary>
        /// This operation will cycle to the next solution for the last limit function. To accept call AcceptLimit.
        /// </summary>
        /// <param name="entitiesToLimit">The entity to limit.</param>
        /// <remarks></remarks>
        public void NextSolution(IEnumerable<IPSLimitable> entitiesToLimit)
        {
            _powerSHAPE.DoCommand("NEXT SOLUTION");

            // Doing this changes the id but the name stays the same so get the new ID
            foreach (PSEntity limitedEntity in entitiesToLimit)
            {
                limitedEntity.Id = _powerSHAPE.ReadIntValue(limitedEntity.Identifier + "['" + limitedEntity.Name + "'].ID");
            }
        }

        /// <summary>
        /// This operation will close the limit operation
        /// </summary>
        public static void AcceptLimit()
        {
            _powerSHAPE.DoCommand("CANCEL");
        }

        /// <summary>
        /// Does not work as created.number is not updated.
        /// </summary>
        /// <param name="entityToLimit">The entity to limit.</param>
        /// <param name="keepOption">Keep option, by default KeepOne.</param>
        /// <returns>The limited entity.</returns>
        /// <remarks></remarks>
        internal static PSEntity LimitEntityUsingDynamicCutter(
            IPSLimitable entityToLimit,
            LimitingKeepOptions keepOption = LimitingKeepOptions.KeepOne)
        {
            // Get PowerSHAPE instance
            _powerSHAPE = ((PSEntity) entityToLimit).PowerSHAPE;

            // Create a list of the single entity
            PSEntity entity = (PSEntity) entityToLimit;

            _powerSHAPE.ActiveModel.ClearCreatedItems();

            entity.AddToSelection(true);

            _powerSHAPE.DoCommand("EDIT SELECTION");
            _powerSHAPE.DoCommand(keepOption.ToString());

            _powerSHAPE.DoCommand("Cutter_Dynamic On");

            bool limitingHappened = false;
            PSModel model = _powerSHAPE.ActiveModel;
            var interval = 1000;

            // Keep looping and picking points
            while (limitingHappened == false)
            {
                // Wait for a second to see if a limit has happened yet.  The item will no longer be selected when it has
                System.Threading.Thread.Sleep(interval);

                // See if the user finished creating the curve yet
                int selectedCount = 1;
                try
                {
                    selectedCount = _powerSHAPE.ReadIntValue("SELECTION.NUMBER");
                }
                catch
                {
                    selectedCount = 1;
                }
                if (selectedCount == 0)
                {
                    foreach (PSEntity newEntity in _powerSHAPE.ActiveModel.CreatedItems)
                    {
                        newEntity.Id = _powerSHAPE.ReadIntValue(newEntity.Identifier + "['" + newEntity.Name + "'].ID");
                        _powerSHAPE.ActiveModel.Add(newEntity);
                    }
                    limitingHappened = true;
                }
            }

            // When the limit happens we get a new entity with a new id but it has the same name as the original
            // So change the id of the entity we wanted to limit so things work as normal
            entity.Id = _powerSHAPE.ReadIntValue(entity.Identifier + "['" + entity.Name + "'].ID");

            return entity;
        }

        #endregion
    }
}