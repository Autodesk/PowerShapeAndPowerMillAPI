// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

namespace Autodesk.ProductInterface.PowerMILL
{
    /// <summary>
    /// Type of leads to configure <see cref="PMLead"/> settings.
    /// </summary>
    public enum LeadTypes
    {
        /// <summary>
        /// Lead in first choice, Settings to control the movement of the tool as it approaches the stock, before beginning a cutting move.
        /// </summary>
        LeadIn,

        /// <summary>
        /// Lead in second choice,Settings to control the movement of the tool as it approaches the stock, before beginning a cutting move.
        /// which is used if 1st choice gouges.
        /// </summary>
        LeadInSecond,

        /// <summary>
        /// Settings to control the movement of the tool after it leaves the stock, at the end of a cutting move.
        /// </summary>
        LeadOut,

        /// <summary>
        /// Settings to control the movement of the tool after it leaves the stock, at the end of a cutting move.
        /// which is used if 1st choice gouges.
        /// </summary>
        LeadOutSecond,

        /// <summary>
        /// Settings to specify a First Lead In move that is different from the subsequent lead in moves.
        /// </summary>
        FirstLeadIn,

        /// <summary>
        /// Settings to specify a Last Lead Out move that is different from the previous lead out moves.
        /// </summary>
        LastLeadOut,

        /// <summary>
        /// These options add an additional Lead In before the existing one.
        /// </summary>
        LeadInExtension,

        /// <summary>
        /// These options add an additional Lead Out after the existing one.
        /// </summary>
        LeadOutExtension
    }
}