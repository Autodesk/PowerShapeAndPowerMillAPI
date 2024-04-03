// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using Autodesk.Geometry;
using Autodesk.ProductInterface.Properties;

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
                var returnStates = new List<string>();
                var stateListXml = PowerMill.GetPowerMillParameterXML("extract(entity('stockmodel', '" + Name + "').States,'name')").GetElementsByTagName("Name");
                foreach (XmlNode state in stateListXml)
                {
                    returnStates.Add(state.InnerText.Trim());
                }
                return returnStates;
            }
        }

        /// <summary>
        /// Gets or sets the tolerance of the stockmodel.
        /// </summary>        
        /// <returns>The tolerance of the stockmodel.</returns>
        public MM Tolerance
        {
            get { return GetParameterDoubleValue(Resources.Tolerance); }
            set { SetParameter("Tolerance", value); }
        }

        /// <summary>
        /// Gets or sets the Stepover of the stockmodel.
        /// </summary>        
        /// <returns>The Stepover of the stockmodel.</returns>
        public MM Stepover
        {
            get { return GetParameterDoubleValue("Stepover"); }
            set { SetParameter("Stepover", value); }
        }

        /// <summary>
        /// Gets or sets the Rest thickness of the stockmodel.
        /// </summary>        
        /// <returns>The Rest thickness of the stockmodel.</returns>
        public MM RestThickness
        {
            get { return GetParameterDoubleValue("ThresholdThickness"); }
            set { SetParameter("ThresholdThickness", value); }
        }

        /// <summary>
        /// Gets or sets if Overhang is detected.
        /// </summary>        
        /// <returns>Whether Overhang detection is on or off.</returns>
        public bool DetectOverhang
        {
            get { return GetParameterBooleanValue("DetectOverhang"); }
            set { SetParameter("DetectOverhang", value); }
        }

        /// <summary>
        /// Gets or sets the Workplane.
        /// </summary>
        /// <returns>The Workplane</returns>
        public PMWorkplane Workplane
        {
            get
            {
                var workplane = PowerMill.GetPowerMillEntityParameter("stockmodel", Name, "Workplane.Name");
                return PowerMill.ActiveProject.Workplanes.GetByName(workplane);
            }
            set
            {
                SetParameter("Workplane", value.Name);
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