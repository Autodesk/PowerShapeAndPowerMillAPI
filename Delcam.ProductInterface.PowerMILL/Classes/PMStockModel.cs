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
    /// Captures a Stock Model object in PowerMILL.
    /// </summary>
    public class PMStockModel : PMEntity
    {
        #region Constructors

        /// <summary>
        /// Initialises the item.
        /// </summary>
        /// <param name="powerMILL">The base instance to interact with PowerMILL.</param>
        internal PMStockModel(PMAutomation powerMILL) : base(powerMILL)
        {
        }

        /// <summary>
        /// Initialises the item.
        /// </summary>
        /// <param name="powerMILL">The base instance to interact with PowerMILL.</param>
        /// <param name="name">The new instance name.</param>
        internal PMStockModel(PMAutomation powerMILL, string name) : base(powerMILL, name)
        {
        }

        #endregion

        #region Properties

        internal static string STOCKMODEL_IDENTIFIER = "STOCKMODEL";

        /// <summary>
        /// Gets the identifier PowerMILL uses to identify this type of entity.
        /// </summary>
        internal override string Identifier
        {
            get { return STOCKMODEL_IDENTIFIER; }
        }

        #endregion

        #region Operations

        /// <summary>
        /// Applys the block to the StockModel.
        /// </summary>
        /// <remarks></remarks>
        public void ApplyBlock()
        {            
            PowerMill.DoCommand(string.Format("EDIT STOCKMODEL \"{0}\" BLOCK ;", Name));
        }

        /// <summary>
        /// Applys the toolpath first to the StockModel.
        /// </summary>
        /// <remarks></remarks>
        public void ApplyToolpathFirst()
        {            
            PowerMill.DoCommand(string.Format("EDIT STOCKMODEL \"{0}\" INSERT_INPUT TOOLPATH ; FIRST", Name));
        }

        /// <summary>
        /// Applys the toolpath last to the StockModel.
        /// </summary>
        /// <remarks></remarks>
        public void ApplyToolpathLast()
        {
            PowerMill.DoCommand(string.Format("EDIT STOCKMODEL \"{0}\" INSERT_INPUT TOOLPATH ; LAST", Name));
        }

        /// <summary>
        /// Applys the tool first to the StockModel.
        /// </summary>
        /// <remarks></remarks>
        public void ApplyToolFirst()
        {            
            PowerMill.DoCommand(string.Format("EDIT STOCKMODEL \"{0}\" INSERT_INPUT TOOL ; FIRST", Name));
        }

        /// <summary>
        /// Applys the tool last to the StockModel.
        /// </summary>
        /// <remarks></remarks>
        public void ApplyToolLast()
        {
            PowerMill.DoCommand(string.Format("EDIT STOCKMODEL \"{0}\" INSERT_INPUT TOOL ; LAST", Name));
        }

        /// <summary>
        /// Gets a list with the names of all the StockModel states.
        /// </summary>
        public List<string> States
        {
            get
            {
                // Get the list of states of the StockModel                
                string stateList = PowerMill.DoCommandEx(string.Format("PRINT PAR \"entity('stockmodel', '{0}').States\"", Name)).ToString();
                stateList = stateList.Replace(((char)13).ToString(), string.Empty);
                string[] splitList = stateList.Split((char)10);
                List<string> returnStates = new List<string>();
               
                var startIndex = 0;
                for (int i = startIndex; i <= splitList.Length - 1; i++)
                {
                    if (splitList[i].Trim().Contains("Name:"))
                    {
                        returnStates.Add(splitList[i].Replace("Name: (STRING)", "").Trim());
                    }
                }
                return returnStates;
            }
        }

        /// <summary>
        /// Deletes the stock model from the active project and from PowerMill.
        /// </summary>
        /// <remarks></remarks>
        public override void Delete()
        {
            PowerMill.ActiveProject.StockModels.Remove(this);
        }

        #endregion
    }
}