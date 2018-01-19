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
    /// Captures a substitution token. When running PowerMILLMacros occurrences of the token will
    /// be replaced with the specified value.
    /// </summary>
    public class PMSubstitutionToken
    {
        #region Fields

        /// <summary>
        /// The token to be replaced.
        /// </summary>
        protected string _token;

        /// <summary>
        /// The value to replace the token with.
        /// </summary>
        protected string _value;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialises the token and replacement value to the specified values.
        /// </summary>
        /// <param name="token">The Token to search for.</param>
        /// <param name="value">The value the Token will be replaced by.</param>
        /// <remarks></remarks>
        public PMSubstitutionToken(string token, string value)
        {
            _token = token;
            _value = value;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets and sets the Token to search for.
        /// </summary>
        public string Token
        {
            get { return _token; }
            set { _token = value; }
        }

        /// <summary>
        /// Gets and sets the Value the Token will be replaced by.
        /// </summary>
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        #endregion
    }
}