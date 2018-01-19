// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

namespace Autodesk.ProductInterface.PowerSHAPE
{
    /// <summary>
    /// Allows scaleable behaviour.
    /// </summary>
    /// <remarks></remarks>
    public interface IPSScalable
    {
        /// <summary>
        /// Scales entity by a single value. Axis can be locked to prevent scaling on that axis.
        /// </summary>
        /// <param name="scaleFactor">The scale factor.</param>
        /// <param name="lockX">Locks X axis to prevent scaling.</param>
        /// <param name="lockY">Locks Y axis to prevent scaling.</param>
        /// <param name="lockZ">Locks Z axis to prevent scaling.</param>
        /// <param name="scaleOrigin">Objects are scaled relative to the scale origin.</param>
        /// <remarks></remarks>
        void ScaleUniform(double scaleFactor, bool lockX, bool lockY, bool lockZ, Geometry.Point scaleOrigin = null);

        /// <summary>
        /// Resizes selected objects in a non-uniform manner, allowing you to choose the scaling to be applied for each axis.
        /// </summary>
        /// <param name="scaleFactorX">The x scale factor.</param>
        /// <param name="scaleFactorY">The y scale factor.</param>
        /// <param name="scaleFactorZ">The z scale factor.</param>
        /// <param name="lockX">Locked in x if true.</param>
        /// <param name="lockY">Locked in y if true.</param>
        /// <param name="lockZ">Locked in z if true.</param>
        /// <param name="scaleOrigin">Objects are scaled relative to the scale origin.</param>
        /// <remarks></remarks>
        void ScaleNonUniform(
            double scaleFactorX,
            double scaleFactorY,
            double scaleFactorZ,
            bool lockX,
            bool lockY,
            bool lockZ,
            Geometry.Point scaleOrigin = null);

        /// <summary>
        /// Resizes entities according to their volume as projected on to the active workplane.
        /// </summary>
        /// <param name="newVolume">The projected volume.</param>
        /// <param name="lockX">Locked in x if true.</param>
        /// <param name="lockY">Locked in y if true.</param>
        /// <param name="lockZ">Locked in z if true.</param>
        /// <param name="scaleOrigin">Objects are scaled relative to the scale origin.</param>
        /// <remarks></remarks>
        void ScaleProjectedVolume(double newVolume, bool lockX, bool lockY, bool lockZ, Geometry.Point scaleOrigin = null);
    }
}