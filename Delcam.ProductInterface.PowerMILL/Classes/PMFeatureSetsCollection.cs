// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System.Collections.Generic;

namespace Autodesk.ProductInterface.PowerMILL
{
    /// <summary>
    /// Represents the collection of FeatureSet objects in the Active PowerMILL Project.
    /// </summary>
    public class PMFeatureSetsCollection : PMEntitiesCollection<PMFeatureSet>
    {
        #region Constructors

        /// <summary>
        /// Initialises the item.
        /// </summary>
        /// <param name="powerMILL">The base instance to interact with PowerMILL.</param>
        internal PMFeatureSetsCollection(PMAutomation powerMILL) : base(powerMILL)
        {
            Initialise();
        }

        #endregion

        #region Operations

        /// <summary>
        /// Initialises the collection with the items in PowerMILL.
        /// </summary>
        internal void Initialise()
        {
            foreach (string name in ReadFeatureSets())
            {
                Add(new PMFeatureSet(_powerMILL, name));
            }
        }

        /// <summary>
        /// Gets the list of the names of all the FeatureSets in PowerMILL.
        /// </summary>
        /// <returns>The list of the names of all the FeatureSets in PowerMILL.</returns>
        internal List<string> ReadFeatureSets()
        {
            List<string> names = new List<string>();
            foreach (var featureSet in _powerMILL.PowerMILLProject.FeatureSets)
            {
                names.Add(featureSet.Name);
            }
            return names;
        }

        /// <summary>
        /// Undraw all FeatureSets.
        /// </summary>        
        public void UndrawAll()
        {
            _powerMILL.DoCommand("UNDRAW FEATURESET ALL");
        }

        #endregion
    }
}