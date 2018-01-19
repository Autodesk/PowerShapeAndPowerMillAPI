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
    /// Captures the collection of Levels in a Project.
    /// </summary>
    public class PSLevelsCollection : PSCollection<PSLevel>
    {
        #region " Fields "

        /// <summary>
        /// The PowerSHAPE Automation object.
        /// </summary>
        private PSAutomation _powerSHAPE;

        #endregion

        #region " Constructors "

        /// <summary>
        /// Creates a new collection.
        /// </summary>
        /// <param name="powerSHAPE">The base instance to interact with PowerShape.</param>
        internal PSLevelsCollection(PSAutomation powerSHAPE)
        {
            _powerSHAPE = powerSHAPE;
        }

        #endregion

        #region " Properties "

        /// <summary>
        /// Gets the level with the specified number.  If the specified level does not exist in the collection, then Nothing is returned.
        /// </summary>
        /// <param name="number">The level number.</param>
        /// <returns>The found level.</returns>
        /// <remarks>If none is found, it returns Nothing.</remarks>
        public PSLevel Item(int number)
        {
            // See if we can find a level with the specified number
            foreach (PSLevel level in this)
            {
                if (level.Number == number)
                {
                    return level;
                }
            }

            // None found, so return Nothing
            return null;
        }

        /// <summary>
        /// Gets the level with the specified name.  If the specified level does not exist in the collection, then Nothing is returned.
        /// </summary>
        /// <param name="name">The level name.</param>
        /// <returns>The found level.</returns>
        /// <remarks>If none is found, it returns Nothing.</remarks>
        public PSLevel Item(string name)
        {
            // See if we can find a level with the specified name
            foreach (PSLevel level in this)
            {
                if (level.Name == name)
                {
                    return level;
                }
            }

            // None found, so return Nothing
            return null;
        }

        #endregion

        #region " Operations "

        /// <summary>
        /// Reads all the levels in the model.  This operation takes a while to execute.
        /// </summary>
        internal void ReadLevels()
        {
            Clear();

            //Check all the levels.  If it exists then add it to the collection
            for (int number = 0; number <= 999; number++)
            {
                PSLevel level = new PSLevel(_powerSHAPE, number);
                Add(level);
            }
        }

        /// <summary>
        /// Activates all Levels.
        /// </summary>
        public void ActivateAllLevels()
        {
            _powerSHAPE.DoCommand("LEVEL OPTIONS ACTIVATE_ALL");
        }

        /// <summary>
        /// Deactivates all Levels.
        /// </summary>
        public void DeactivateAllLevels()
        {
            _powerSHAPE.DoCommand("LEVEL OPTIONS DEACTIVATE_ALL");
        }

        #endregion
    }
}