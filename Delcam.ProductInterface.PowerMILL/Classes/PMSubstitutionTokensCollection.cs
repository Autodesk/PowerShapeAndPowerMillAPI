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
    /// Collection of SubstitutionTokens that can be used by a PowerMILLMacro
    /// </summary>
    public class PMSubstitutionTokensCollection : PMCollection<PMSubstitutionToken>
    {
        #region Constructors

        #endregion

        #region Operations

        /// <summary>
        /// Adds the SubstitutionToken to the collection.
        /// If one with the same token already exists then change its replacement value, else it will add the token.
        /// </summary>
        /// <param name="token">The token to add.</param>
        public override void Add(PMSubstitutionToken token)
        {
            // If one with the same token already exists then change its replacement value
            foreach (PMSubstitutionToken existingToken in this)
            {
                if (existingToken.Token == token.Token)
                {
                    existingToken.Value = token.Value;
                    return;
                }
            }

            // Doesn't already exist so add the token
            base.Add(token);
        }

        #endregion
    }
}