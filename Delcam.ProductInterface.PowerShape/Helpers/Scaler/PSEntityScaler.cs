// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using DG = Autodesk.Geometry;

namespace Autodesk.ProductInterface.PowerSHAPE
{
    /// <summary>
    /// Helper to scale an entity.
    /// </summary>
    /// <remarks></remarks>
    public class PSEntityScaler
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
        public PSEntityScaler(PSAutomation powerSHAPE)
        {
            _powerSHAPE = powerSHAPE;
        }

        #endregion

        #region " Operations "

        /// <summary>
        /// Lets you resize selected objects in a uniform manner, making them larger or smaller equally in all directions. You can also lock the scaling factor in any of the directions.
        /// </summary>
        /// <param name="entityToScale">The entity to scale.</param>
        /// <param name="scaleFactor">The scale value.</param>
        /// <param name="lockX">Locked in x if true.</param>
        /// <param name="lockY">Locked in y if true.</param>
        /// <param name="lockZ">Locked in z if true.</param>
        /// <param name="scaleOrigin">Objects are scaled relative to the scale origin.</param>
        /// <remarks></remarks>
        public static void ScaleUniform(
            IPSScalable entityToScale,
            double scaleFactor,
            bool lockX,
            bool lockY,
            bool lockZ,
            Geometry.Point scaleOrigin = null)
        {
            _powerSHAPE = ((PSEntity) entityToScale).PowerSHAPE;

            ((PSEntity) entityToScale).AddToSelection(true);

            _powerSHAPE.DoCommand("EDIT SCALE");

            _powerSHAPE.DoCommand("UNIFORM");

            _powerSHAPE.DoCommand("VALUE " + scaleFactor);

            if (lockX)
            {
                _powerSHAPE.DoCommand("LOCK X ON");
            }
            else
            {
                _powerSHAPE.DoCommand("LOCK X OFF");
            }
            if (lockY)
            {
                _powerSHAPE.DoCommand("LOCK Y ON");
            }
            else
            {
                _powerSHAPE.DoCommand("LOCK Y OFF");
            }
            if (lockZ)
            {
                _powerSHAPE.DoCommand("LOCK Z ON");
            }
            else
            {
                _powerSHAPE.DoCommand("LOCK Z OFF");
            }

            if (scaleOrigin != null)
            {
                _powerSHAPE.DoCommand("SCALEORIGIN");
                _powerSHAPE.DoCommand(scaleOrigin.ToString());
            }

            if (_powerSHAPE.Version.Major > 11)
            {
                _powerSHAPE.DoCommand("APPLY");
                _powerSHAPE.DoCommand("DISMISS");
            }
            else
            {
                _powerSHAPE.DoCommand("CANCEL");
            }
        }

        /// <summary>
        /// Resizes selected objects in a non-uniform manner, allowing you to choose the scaling to be applied for each axis.
        /// </summary>
        /// <param name="entityToScale">The entity to scale.</param>
        /// <param name="scaleFactorX">The x scale factor.</param>
        /// <param name="scaleFactorY">The y scale factor.</param>
        /// <param name="scaleFactorZ">The z scale factor.</param>
        /// <param name="lockX">Locked in x if true.</param>
        /// <param name="lockY">Locked in y if true.</param>
        /// <param name="lockZ">Locked in z if true.</param>
        /// <param name="scaleOrigin">Objects are scaled relative to the scale origin.</param>
        /// <remarks></remarks>
        public static void ScaleNonUniform(
            IPSScalable entityToScale,
            double scaleFactorX,
            double scaleFactorY,
            double scaleFactorZ,
            bool lockX,
            bool lockY,
            bool lockZ,
            Geometry.Point scaleOrigin = null)
        {
            _powerSHAPE = ((PSEntity) entityToScale).PowerSHAPE;

            ((PSEntity) entityToScale).AddToSelection(true);

            _powerSHAPE.DoCommand("EDIT SCALE");

            _powerSHAPE.DoCommand("NONUNIFORM");

            _powerSHAPE.DoCommand("X " + scaleFactorX);
            _powerSHAPE.DoCommand("Y " + scaleFactorY);
            _powerSHAPE.DoCommand("Z " + scaleFactorZ);

            if (lockX)
            {
                _powerSHAPE.DoCommand("LOCK X ON");
            }
            else
            {
                _powerSHAPE.DoCommand("LOCK X OFF");
            }
            if (lockY)
            {
                _powerSHAPE.DoCommand("LOCK Y ON");
            }
            else
            {
                _powerSHAPE.DoCommand("LOCK Y OFF");
            }
            if (lockZ)
            {
                _powerSHAPE.DoCommand("LOCK Z ON");
            }
            else
            {
                _powerSHAPE.DoCommand("LOCK Z OFF");
            }

            if (scaleOrigin != null)
            {
                _powerSHAPE.DoCommand("SCALEORIGIN");
                _powerSHAPE.DoCommand(scaleOrigin.ToString());
            }

            if (_powerSHAPE.Version.Major > 11)
            {
                _powerSHAPE.DoCommand("APPLY");
                _powerSHAPE.DoCommand("DISMISS");
            }
            else
            {
                _powerSHAPE.DoCommand("ACCEPT");
                _powerSHAPE.DoCommand("CANCEL");
            }
        }

        /// <summary>
        /// Resizes surfaces and solids according to their volume as projected on to the active workplane.
        /// </summary>
        /// <param name="entityToScale">The entity to scale.</param>
        /// <param name="newVolume">The new projected volume.</param>
        /// <param name="lockX">Locked in x if true.</param>
        /// <param name="lockY">Locked in y if true.</param>
        /// <param name="lockZ">Locked in z if true.</param>
        /// <param name="scaleOrigin">Objects are scaled relative to the scale origin.</param>
        /// <remarks></remarks>
        public static void ScaleProjectedVolume(
            IPSScalable entityToScale,
            double newVolume,
            bool lockX,
            bool lockY,
            bool lockZ,
            Geometry.Point scaleOrigin = null)
        {
            _powerSHAPE = ((PSEntity) entityToScale).PowerSHAPE;

            ((PSEntity) entityToScale).AddToSelection(true);

            _powerSHAPE.DoCommand("EDIT SCALE");

            _powerSHAPE.DoCommand("UNIFORM");

            _powerSHAPE.DoCommand("VALUE " + newVolume);

            if (lockX)
            {
                _powerSHAPE.DoCommand("LOCK X ON");
            }
            else
            {
                _powerSHAPE.DoCommand("LOCK X OFF");
            }
            if (lockY)
            {
                _powerSHAPE.DoCommand("LOCK Y ON");
            }
            else
            {
                _powerSHAPE.DoCommand("LOCK Y OFF");
            }
            if (lockZ)
            {
                _powerSHAPE.DoCommand("LOCK Z ON");
            }
            else
            {
                _powerSHAPE.DoCommand("LOCK Z OFF");
            }

            if (scaleOrigin != null)
            {
                _powerSHAPE.DoCommand("SCALEORIGIN");
                _powerSHAPE.DoCommand(scaleOrigin.ToString());
            }

            _powerSHAPE.DoCommand("APPLY");
            _powerSHAPE.DoCommand("DISMISS");
        }

        #endregion
    }
}