// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System.Collections.Generic;
using Autodesk.Extensions;

namespace Autodesk.ProductInterface.PowerSHAPE
{
    public enum DecayDefinitionOptions
    {
        None,
        Distance,
        Curve,
        Surface
    }

    public enum DecayDefinitionBlend
    {
        Linear,
        Parabolic,
        Cubic,
        Quartic
    }

    public class PSEntityMorpher
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
        public PSEntityMorpher(PSAutomation powerSHAPE)
        {
            _powerSHAPE = powerSHAPE;
        }

        #endregion

        #region " Operations "

        /// <summary>
        /// Morphs surfaces between a control and reference surface.  Note that you can morph one surface if it is a PowerSurface but you
        /// must morph more than one surface if it is not a PowerSurface.  Also, the control and reference surfaces must have the same number
        /// of laterals and longitudinals
        /// </summary>
        /// <param name="surfacesToMorph">The surfaces to be morphed</param>
        /// <param name="referenceSurface">The reference surface</param>
        /// <param name="controlSurface"></param>
        /// <param name="decayDefinitionOption"></param>
        /// <param name="decayDefinitionBlend"></param>
        /// <param name="keepReferenceAndControlSurfaces"></param>
        /// <param name="normalOffsetting"></param>
        /// <param name="decaySelection"></param>
        /// <param name="decayLimit"></param>
        /// <param name="decayValue"></param>
        public static void MorphSurfacesBetweenTwoSurfaces(
            List<PSSurface> surfacesToMorph,
            PSSurface referenceSurface,
            PSSurface controlSurface,
            DecayDefinitionOptions decayDefinitionOption,
            DecayDefinitionBlend decayDefinitionBlend = DecayDefinitionBlend.Quartic,
            bool keepReferenceAndControlSurfaces = false,
            bool normalOffsetting = false,
            PSSurface decaySurface = null,
            PSGenericCurve decayCurve = null,
            int decayLimit = 100,
            double decayValue = 0.5)
        {
            _powerSHAPE = referenceSurface.PowerSHAPE;
            var activeModel = _powerSHAPE.ActiveModel;
            activeModel.ClearSelectedItems();
            surfacesToMorph.ForEach(x => x.AddToSelection());

            _powerSHAPE.DoCommand("MORPH", "TWOSURFS", "REFERENCE ON");
            referenceSurface.AddToSelection();
            _powerSHAPE.DoCommand("CONTROL ON");
            controlSurface.AddToSelection();

            // Always keep the surfaces as they are only actually hidden from view but kept in the database. If
            // the user wants to lose them then we can delete them afterwards.
            _powerSHAPE.DoCommand("KEEP ON", "WRAP " + normalOffsetting.ToOnOff(), "DECAY " + decayDefinitionOption);

            if (decayDefinitionOption != DecayDefinitionOptions.None)
            {
                _powerSHAPE.DoCommand("BLEND " + decayDefinitionBlend);
                if (decayDefinitionOption == DecayDefinitionOptions.Distance)
                {
                    if (decayValue < 0.05)
                    {
                        decayValue = 0.05;
                    }
                    if (decayValue > 0.5)
                    {
                        decayValue = 0.5;
                    }
                    _powerSHAPE.DoCommand("LIMIT " + decayLimit, "HALFLIFE " + decayValue);
                }
                else
                {
                    _powerSHAPE.DoCommand("DECAYSELECT ON");
                    if (decaySurface != null)
                    {
                        decaySurface.AddToSelection();
                    }
                    if (decayCurve != null)
                    {
                        decayCurve.AddToSelection();
                    }
                }
            }

            _powerSHAPE.DoCommand("ACCEPT");

            // Delete the reference and control surfaces if chosen to
            if (keepReferenceAndControlSurfaces == false)
            {
                referenceSurface.Delete();
                controlSurface.Delete();
            }
        }

        /// <summary>
        /// Morphs solid between a control and reference surface. The control and reference surfaces must have the same number
        /// of laterals and longitudinals
        /// </summary>
        /// <param name="solidToMorph">The solid to be morphed</param>
        /// <param name="referenceSurface">The reference surface</param>
        /// <param name="controlSurface"></param>
        /// <param name="normalOffsetting"></param>
        public static void MorphSolidBetweenTwoSurfaces(
            PSSolid solidToMorph,
            PSSurface referenceSurface,
            PSSurface controlSurface,
            bool normalOffsetting = false)
        {
            _powerSHAPE = referenceSurface.PowerSHAPE;
            solidToMorph.AddToSelection(true);

            _powerSHAPE.DoCommand("MORPH", "CANCEL SURFACEMORPH", "REFERENCE");
            referenceSurface.AddToSelection();
            _powerSHAPE.DoCommand("CONTROL");
            controlSurface.AddToSelection();

            if (normalOffsetting)
            {
                _powerSHAPE.DoCommand("OFFSETNORMAL ON");
            }

            _powerSHAPE.DoCommand("ACCEPT CANCEL");
        }

        #endregion
    }
}