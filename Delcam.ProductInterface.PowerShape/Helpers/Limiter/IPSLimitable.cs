// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System.Collections.Generic;

namespace Autodesk.ProductInterface.PowerSHAPE
{
    /// <summary>
    /// Allows an entity to be limited by another entity or entities.
    /// </summary>
    /// <remarks></remarks>
    public interface IPSLimitable
    {
        /// <summary>
        /// Limits an entity by other entity.
        /// </summary>
        /// <param name="limitingEntity">Entity with which to perform the limiting operation.</param>
        /// <param name="limitingMode">Mode in which to carry out the operation.</param>
        /// <param name="keepOption">Whether to keep one or both sides of the limit.</param>
        /// <param name="trimOption">Whether to trim one or all of the entities.</param>
        /// <param name="finishOperation">Whether the operation should be ended.</param>
        /// <returns>A list containing the new entities.</returns>
        /// <remarks></remarks>
        List<PSEntity> LimitToEntity(
            PSEntity limitingEntity,
            LimitingModes limitingMode = LimitingModes.LimitMode,
            LimitingKeepOptions keepOption = LimitingKeepOptions.KeepOne,
            LimitingTrimOptions trimOption = LimitingTrimOptions.LimitOne,
            bool finishOperation = true);

        /// <summary>
        /// Limits an entity by other entities.
        /// </summary>
        /// <param name="limitingEntities">Entities to limit with.</param>
        /// <param name="limitingMode">Mode in which to carry out the operation.</param>
        /// <param name="keepOption">Whether to keep one or both sides of the limit.</param>
        /// <param name="trimOption">Whether to trim one or all of the entities</param>
        /// <param name="finishOperation">Whether the operation should be ended.</param>
        /// <returns>A list containing the new entities.</returns>
        /// <remarks></remarks>
        List<PSEntity> LimitToEntities(
            List<PSEntity> limitingEntities,
            LimitingModes limitingMode = LimitingModes.LimitMode,
            LimitingKeepOptions keepOption = LimitingKeepOptions.KeepOne,
            LimitingTrimOptions trimOption = LimitingTrimOptions.LimitOne,
            bool finishOperation = true);

        /// <summary>
        /// Limits using the dynamic cutter option.
        /// </summary>
        /// <param name="keepOption">Whether to keep one or both sides of the limit.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        PSEntity LimitUsingDynamicCutter(LimitingKeepOptions keepOption = LimitingKeepOptions.KeepOne);
    }
}