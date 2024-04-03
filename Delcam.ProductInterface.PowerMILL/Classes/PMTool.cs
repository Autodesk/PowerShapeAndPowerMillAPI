// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using Autodesk.FileSystem;
using Autodesk.Geometry;

namespace Autodesk.ProductInterface.PowerMILL
{
    /// <summary>
    /// Captures a Tool object in PowerMILL.
    /// </summary>
    public class PMTool : PMEntity
    {
        #region Constructors

        /// <summary>
        /// Initialises the item.
        /// </summary>
        /// <param name="powerMILL">The base instance to interact with PowerMILL.</param>
        internal PMTool(PMAutomation powerMILL) : base(powerMILL)
        {
        }

        /// <summary>
        /// Initialises the item.
        /// </summary>
        /// <param name="powerMILL">The base instance to interact with PowerMILL.</param>
        /// <param name="name">The new instance name.</param>
        internal PMTool(PMAutomation powerMILL, string name) : base(powerMILL, name)
        {
        }

        #endregion

        #region Properties

        internal static string TOOL_IDENTIFIER = "TOOL";

        /// <summary>
        /// Gets the identifier PowerMILL uses to identify this type of entity.
        /// </summary>
        internal override string Identifier
        {
            get { return TOOL_IDENTIFIER; }
        }

        /// <summary>
        /// BoundingBox property is not valid for this type.
        /// </summary>
        public override BoundingBox BoundingBox
        {
            get { throw new Exception("Property not valid for " + GetType()); }
        }

        /// <summary>
        /// Gets and sets the tool length in PowerMill.
        /// </summary>
        public MM Length
        {
            get
            {
                return
                    Convert.ToDouble(
                        PowerMill.GetPowerMillEntityParameter("tool", Name, "length").Trim());
            }
            set { PowerMill.DoCommand("EDIT TOOL \"" + Name + "\" LENGTH \"" + value + "\"", "TOOL ACCEPT"); }
        }

        /// <summary>
        /// Gets and sets the tool number in PowerMill.
        /// </summary>
        public int ToolNumber
        {
            get
            {
                return
                    Convert.ToInt32(
                        PowerMill.GetPowerMillEntityParameter("tool", Name, "number").Trim());
            }
            set { PowerMill.DoCommand("EDIT TOOL \"" + Name + "\" NUMBER " + value, "TOOL ACCEPT"); }
        }

        /// <summary>
        /// Gets and sets the number of flutes in PowerMill.
        /// </summary>
        public int NumberOfFlutes
        {
            get
            {
                return
                    Convert.ToInt32(
                        PowerMill.GetPowerMillEntityParameter("tool", Name, "flutes").Trim());
            }
            set { PowerMill.DoCommand("EDIT TOOL \"" + Name + "\" NUM_FLUTES \"" + value + "\"", "TOOL ACCEPT"); }
        }

        /// <summary>
        /// Gets and sets the tool diameter.
        /// </summary>
        public MM Diameter
        {
            get
            {
                return
                    Convert.ToDouble(
                        PowerMill.GetPowerMillEntityParameter("tool", Name, "diameter").Trim());
            }
            set { PowerMill.DoCommand("EDIT TOOL \"" + Name + "\" DIAMETER \"" + value + "\"", "TOOL ACCEPT"); }
        }

        /// <summary>
        /// Gets and sets the holder overhang.
        /// </summary>
        public MM Overhang
        {
            get
            {
                return
                    Convert.ToDouble(
                        PowerMill.GetPowerMillEntityParameter("tool", Name, "overhang").Trim());
            }
            set { PowerMill.DoCommand("EDIT TOOL \"" + Name + "\" OVERHANG " + value, "TOOL ACCEPT"); }
        }

        /// <summary>
        /// Gets and sets the tool description.
        /// </summary>
        public string Description
        {
            get
            {
                return
                        PowerMill.GetPowerMillEntityParameter("tool", Name, "description").Trim();
            }
            set { PowerMill.DoCommand("EDIT TOOL \"" + Name + "\" DESCRIPTION \"" + value + "\"", "TOOL ACCEPT"); }
        }

        /// <summary>
        /// Gets and sets the tool holder name.
        /// </summary>
        public string HolderName
        {
            get
            {
                return
                    PowerMill.GetPowerMillEntityParameter("tool", Name, "holdername").Trim();
            }
            set { PowerMill.DoCommand("EDIT TOOL \"" + Name + "\" HOLDER_NAME \"" + value + "\"", "TOOL ACCEPT"); }
        }

        /// <summary>
        /// Gets and sets the tool coolant type.
        /// </summary>
        public CoolantOptions Coolant
        {
            get
            {
                switch (PowerMill.GetPowerMillEntityParameter("tool", Name, "coolant").Trim()
                )
                {
                    case "none":
                        return CoolantOptions.None;
                    case "standard":
                        return CoolantOptions.Standard;
                    case "flood":
                        return CoolantOptions.Flood;
                    case "mist":
                        return CoolantOptions.Mist;
                    case "tap":
                        return CoolantOptions.Tap;
                    case "air":
                        return CoolantOptions.Air;
                    case "thru":
                        return CoolantOptions.Through;
                    case "both":
                        return CoolantOptions.Double;
                }
                throw new Exception("Failed to determine coolant type");
            }
            set
            {
                string coolantValue = "";
                switch (value)
                {
                    case CoolantOptions.None:
                        coolantValue = "NONE";
                        break;
                    case CoolantOptions.Standard:
                        coolantValue = "ON";
                        break;
                    case CoolantOptions.Flood:
                        coolantValue = "FLOOD";
                        break;
                    case CoolantOptions.Mist:
                        coolantValue = "MIST";
                        break;
                    case CoolantOptions.Tap:
                        coolantValue = "TAP";
                        break;
                    case CoolantOptions.Air:
                        coolantValue = "AIR";
                        break;
                    case CoolantOptions.Through:
                        coolantValue = "THRU";
                        break;
                    case CoolantOptions.Double:
                        coolantValue = "BOTH";
                        break;
                }
                PowerMill.DoCommand("EDIT TOOL \"" + Name + "\" COOLANT " + coolantValue, "TOOL ACCEPT");
            }
        }

        /// <summary>
        /// Gets and sets the number of elements in the shank.
        /// </summary>
        public int NumberOfShankElements
        {
            get
            {
                if (PowerMill.Version < new Version("15.0"))
                {
                    throw new Exception(
                        "Shank elementes are not available for this version of PowerMILL.  PowerMILL 15 or greater is required");
                }
                return
                    Convert.ToInt32(
                        PowerMill.GetPowerMillParameter("size(entity('tool','" + Name + "').shanksetvalues)"));
            }
        }

        /// <summary>
        /// Gets and sets the upper diameter of the requested shank component.
        /// </summary>
        public MM ShankElementUpperDiameter(int index)
        {
            if (PowerMill.Version < new Version("15.0"))
            {
                throw new Exception(
                    "Shank elementes are not available for this version of PowerMILL.  PowerMILL 15 or greater is required");
            }
            string result =
                PowerMill.GetPowerMillEntityParameter("tool", Name, "shanksetvalues[" + index + "].upperdiameter");
            if (string.IsNullOrWhiteSpace(result) || result.Contains("#ERROR"))
            {
                throw new IndexOutOfRangeException(
                    "The specified index is greater than the number of elements in the shank");
            }
            return Convert.ToDouble(result);
        }

        /// <summary>
        /// Gets and sets the lower diameter of the requested shank component.
        /// </summary>
        public MM ShankElementLowerDiameter(int index)
        {
            if (PowerMill.Version < new Version("15.0"))
            {
                throw new Exception(
                    "Shank elementes are not available for this version of PowerMILL.  PowerMILL 15 or greater is required");
            }
            string result =
                PowerMill.GetPowerMillEntityParameter("tool", Name, "shanksetvalues[" + index + "].lowerdiameter");
            if (string.IsNullOrWhiteSpace(result) || result.Contains("#ERROR"))
            {
                throw new IndexOutOfRangeException(
                    "The specified index is greater than the number of elements in the shank");
            }
            return Convert.ToDouble(result);
        }

        /// <summary>
        /// Gets and sets the length of the requested shank component.
        /// </summary>
        public MM ShankElementLength(int index)
        {
            if (PowerMill.Version < new Version("15.0"))
            {
                throw new Exception(
                    "Shank elementes are not available for this version of PowerMILL.  PowerMILL 15 or greater is required");
            }
            string result =
                PowerMill.GetPowerMillEntityParameter("tool", Name, "shanksetvalues[" + index + "].length");
            if (string.IsNullOrWhiteSpace(result) || result.Contains("#ERROR"))
            {
                throw new IndexOutOfRangeException(
                    "The specified index is greater than the number of elements in the shank");
            }
            return Convert.ToDouble(result);
        }

        /// <summary>
        /// Gets and sets the number of elements in the holder.
        /// </summary>
        public int NumberOfHolderElements
        {
            get
            {
                if (PowerMill.Version < new Version("15.0"))
                {
                    throw new Exception(
                        "Holder elementes are not available for this version of PowerMILL.  PowerMILL 15 or greater is required");
                }
                return
                    Convert.ToInt32(
                        PowerMill.GetPowerMillParameter("size(entity('tool','" + Name + "').holdersetvalues)"));
            }
        }

        /// <summary>
        /// Gets and sets the upper diameter of the requested holder component.
        /// </summary>
        public MM HolderElementUpperDiameter(int index)
        {
            if (PowerMill.Version < new Version("15.0"))
            {
                throw new Exception(
                    "Holder elements are not available for this version of PowerMILL.  PowerMILL 15 or greater is required");
            }
            string result =
                PowerMill.GetPowerMillEntityParameter("tool", Name, "holdersetvalues[" + index + "].upperdiameter");
            if (string.IsNullOrWhiteSpace(result) || result.Contains("#ERROR"))
            {
                throw new IndexOutOfRangeException(
                    "The specified index is greater than the number of elements in the holder");
            }
            return Convert.ToDouble(result);
        }

        /// <summary>
        /// Gets and sets the lower diameter of the requested holder component.
        /// </summary>
        public MM HolderElementLowerDiameter(int index)
        {
            if (PowerMill.Version < new Version("15.0"))
            {
                throw new Exception(
                    "Holder elementes are not available for this version of PowerMILL.  PowerMILL 15 or greater is required");
            }
            string result =
                PowerMill.GetPowerMillEntityParameter("tool", Name, "holdersetvalues[" + index + "].lowerdiameter");
            if (string.IsNullOrWhiteSpace(result) || result.Contains("#ERROR"))
            {
                throw new IndexOutOfRangeException(
                    "The specified index is greater than the number of elements in the holder");
            }
            return Convert.ToDouble(result);
        }

        /// <summary>
        /// Gets and sets the length of the requested holder component.
        /// </summary>
        public MM HolderElementLength(int index)
        {
            if (PowerMill.Version < new Version("15.0"))
            {
                throw new Exception(
                    "Holder elementes are not available for this version of PowerMILL.  PowerMILL 15 or greater is required");
            }
            string result =
                PowerMill.GetPowerMillEntityParameter("tool", Name, "holdersetvalues[" + index + "].length");
            if (string.IsNullOrWhiteSpace(result) || result.Contains("#ERROR"))
            {
                throw new IndexOutOfRangeException(
                    "The specified index is greater than the number of elements in the holder");
            }
            return Convert.ToDouble(result);
        }

        /// <summary>
        /// Gets the length of the complete Holder
        /// </summary>
        public MM HolderLength
        {
            get
            {
                if (PowerMill.Version < new Version("15.0"))
                {
                    throw new Exception(
                        "Holder elementes are not available for this version of PowerMILL.  PowerMILL 15 or greater is required");
                }
                MM holderLength = 0;
                for (int i = 0; i < NumberOfHolderElements; i++)
                {
                    holderLength += HolderElementLength(i);
                }
                return holderLength;
            }           
        }

        /// <summary>
        /// Gets the gauge length of the tool
        /// </summary>
        public MM GaugeLength
        {
            get
            {
                if (PowerMill.Version < new Version("15.0"))
                {
                    throw new Exception(
                        "Holder elementes are not available for this version of PowerMILL.  PowerMILL 15 or greater is required");
                }
                return HolderLength + Overhang;
            }
        }
        #endregion

        #region Operations

        /// <summary>
        /// Deletes the tool from the active project and from PowerMill.
        /// </summary>
        /// <remarks></remarks>
        public override void Delete()
        {
            PowerMill.ActiveProject.Tools.Remove(this);
        }

    /// <summary>
    /// Export holder
    /// </summary>
    public void ExportHolder(File filePath)
    {
      PowerMill.DoCommand("EDIT TOOL \"" + Name + "\" EXPORT_STL HOLDER NO_OFFSET  \"" + filePath.Path + "\"");
    }

    /// <summary>
    /// Export Shank
    /// </summary>
    public void ExportShank(File filePath)
    {
      PowerMill.DoCommand("EDIT TOOL \"" + Name + "\" EXPORT_STL SHANK NO_OFFSET  \"" + filePath.Path + "\"");
    }

    /// <summary>
    /// Export holder
    /// </summary>
    public void ExportTip(File filePath)
    {
      PowerMill.DoCommand("EDIT TOOL \"" + Name + "\" EXPORT_STL TIP NO_OFFSET  \"" + filePath.Path + "\"");
    }

    #endregion
  }
}