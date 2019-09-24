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
    /// Collection of Stock Model objects in the Active PowerMILL Project.
    /// </summary>
    public class PMStockModelsCollection : PMEntitiesCollection<PMStockModel>
    {
        #region Constructors

        /// <summary>
        /// Initialises the item.
        /// </summary>
        /// <param name="powerMILL">The base instance to interact with PowerMILL.</param>
        internal PMStockModelsCollection(PMAutomation powerMILL) : base(powerMILL)
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
            foreach (string name in ReadStockModels())
            {
                Add(new PMStockModel(_powerMILL, name));
            }
        }

        /// <summary>
        /// Gets a list of names of all Stock Models in PowerMILL.
        /// </summary>
        internal List<string> ReadStockModels()
        {
            List<string> names = new List<string>();
            foreach (var stockModel in _powerMILL.PowerMILLProject.StockModels)
            {
                names.Add(stockModel.Name);
            }
            return names;
        }

        /// <summary>
        /// Creates a new Stockmodel with the specified name.
        /// </summary>
        /// <param name="name">The name of the Stockmodel.</param>
        /// <returns></returns>
        public PMStockModel CreateStockmodel(string name)
        {
            // Check to make sure a stockmodel by this name does not already exists
            if (this[name] != null)
            {
                return null;
            }
            
            // Create the stockmodel
            _powerMILL.DoCommand("CREATE STOCKMODEL ;");

            // Get the new stockmodel
            var newStockmodel = (PMStockModel)_powerMILL.ActiveProject.CreatedItems(typeof(PMStockModel))[0];
            if (!string.IsNullOrEmpty(name))
            {
                newStockmodel.Name = name;
            }
            Add(newStockmodel);
            return newStockmodel;
        }

        #endregion
    }
}