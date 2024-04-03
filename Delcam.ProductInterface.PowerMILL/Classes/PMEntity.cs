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
using System.Linq;
using Autodesk.Geometry;
using Autodesk.ProductInterface.Properties;

namespace Autodesk.ProductInterface.PowerMILL
{
    /// <summary>
    /// Abstract base class that represents an item in PowerMILL.
    /// </summary>
    public abstract class PMEntity
    {
        #region Fields

        /// <summary>
        /// The PowerMill Automation object.
        /// </summary>
        protected PMAutomation PowerMill;

        /// <summary>
        /// The name of the Entity.  It is the only way of accessing it in PowerMILL.
        /// If the name changes without us knowing we can no longer access the entity.
        /// </summary>
        private string _name;

        /// <summary>
        /// The GUID of the Entity.
        /// </summary>
        private readonly Guid _id;

        /// <summary>
        /// Powermill true value represented in 1 means true and 0 means false;
        /// </summary>
        private string TrueValue = "1";

        /// <summary>
        /// Powermill error output start's with #ERROR
        /// </summary>
        private string ErrorMessage = "ERROR";

        /// <summary>
        /// Powermill 2016 version
        /// </summary>
        public static readonly Version POWER_MILL_2016 = new Version(20, 0, 0);

        /// <summary>
        /// Powermill 2017 version
        /// </summary>
        public static readonly Version POWER_MILL_2017 = new Version(21, 0, 0);

        #endregion

        #region Constructors

        /// <summary>
        /// Initialises the item.
        /// </summary>
        /// <param name="powerMill">The base instance to interact with PowerMILL.</param>
        internal PMEntity(PMAutomation powerMill)
        {
            PowerMill = powerMill;
            _name = "";
            _id = new Guid(GetParameter("ID"));
        }

        /// <summary>
        /// Initialises the item.
        /// </summary>
        /// <param name="powerMill">The base instance to interact with PowerMILL.</param>
        /// <param name="name">The new instance name.</param>
        internal PMEntity(PMAutomation powerMill, string name)
        {
            PowerMill = powerMill;
            _name = name;
            _id = new Guid(GetParameter("ID"));
        }

        #endregion

        #region Properties

        /// <summary>
        /// Version of current Power mill.
        /// </summary>
        public Version Version
        {
            get { return PowerMill.Version; }
        }

        /// <summary>
        /// Gets and sets the name of the entity. It also updates PowerMill.
        /// </summary>
        public virtual string Name
        {
            get { return _name; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new Exception("Name cannot be an empty string or null.");
                }
                PowerMill.DoCommand("RENAME " + Identifier + " '" + _name + "' '" + value + "'");
                _name = value;
            }
        }

        /// <summary>
        /// Gets the GUID of the entity.
        /// </summary>
        public Guid ID
        {
            get { return _id; }
        }
        
        /// <summary>
        /// Gets and sets whether the entity is active. It also updates PowerMill.
        /// </summary>
        public virtual bool IsActive
        {
            get
            {
                string activeName =
                    PowerMill.GetPowerMillEntityParameter(Identifier, "", "Name");
                return activeName == Name;
            }
            set
            {
                if (value == false)
                {
                    //Only one can be active at any time so check that your deactivating the correct one.
                    if (IsActive)
                    {
                        PowerMill.DoCommand("DEACTIVATE " + Identifier);
                    }
                }
                else
                {
                    PowerMill.DoCommand("ACTIVATE " + Identifier + " '" + _name + "'");
                }
            }
        }

        /// <summary>
        /// Gets the identifier PowerMILL uses to identify this type of entity
        /// </summary>
        internal abstract string Identifier { get; }

        /// <summary>
        /// Get the bounding box of the entity
        /// </summary>
        public virtual BoundingBox BoundingBox
        {
            get
            {
                try
                {
                    // Ensure dialogs are off before calling
                    PowerMill.DialogsOff();
                    string result = PowerMill.DoCommandEx("SIZE " + Identifier + " '" + Name + "'").ToString();

                    // Remove the first line so the model name isn't detected as containing size values
                    result = result.Substring(result.IndexOf((char) 10));
                    result = result.Replace(Environment.NewLine, " ");
                    result = result.Replace("\t", " ");
                    string[] spl = result.Split(' ');
                    double[] Sizes;
                    int counter = 0;
                    Sizes = new double[result.Length];
                    for (int i = 0; i <= spl.Length - 1; i++)
                    {
                        double splDouble = 0.0;
                        if (double.TryParse(spl[i], out splDouble))
                        {
                            Sizes[counter] = splDouble;
                            counter += 1;
                        }
                    }
                    Array.Resize(ref Sizes, counter);
                    if (counter < 6)
                    {
                        return null;
                    }
                    var tmp = new BoundingBox(Sizes[0], Sizes[3], Sizes[1], Sizes[4], Sizes[2], Sizes[5]);
                    return tmp;
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets whether or not the entity exists in PowerMILL
        /// </summary>
        /// <returns></returns>
        public bool Exists
        {
            get
            {
                return
                    PowerMill.GetPowerMillEntityParameter(Identifier ,  Name ,"Name")
                             .ToString()
                             .Trim() == Name;
            }
        }

        #endregion

        #region Functions

        /// <summary>
        /// Get a parameter value
        /// </summary>
        /// <param name="paramter">Parameter qualified name</param>
        /// <returns>String raw output of the application</returns>
        internal string GetParameter(string paramter)
        {
            var result = PowerMill.GetPowerMillEntityParameter( Identifier, Name, paramter);
            ValidateOutput(result, paramter);
            return result;
        }

        /// <summary>
        /// Get a double value
        /// </summary>
        /// <param name="paramter">Parameter qualified name</param>
        /// <returns>output will be converted to <see cref="double"/></returns>
        internal double GetParameterDoubleValue(string paramter)
        {
            return Convert.ToDouble(GetParameter(paramter));
        }

        /// <summary>
        /// Get a <see cref="bool"/> value
        /// </summary>
        /// <param name="paramter">Parameter qualified name</param>
        /// <returns>output will be converted to <see cref="bool"/></returns>
        internal bool GetParameterBooleanValue(string paramter)
        {
            return GetParameter(paramter) == TrueValue;
        }

        /// <summary>
        /// Set a parameter value
        /// </summary>
        /// <param name="paramter">Parameter qualified name</param>
        /// <param name="value">a string value for the parameter</param>
        /// <returns>Not all parameters return a value after edit</returns>
        internal string SetParameter(string paramter, string value)
        {
            var result = PowerMill.DoCommandEx(
                                      string.Format(Resources.SetParameterStringValue,
                                                    Identifier,
                                                    Name,
                                                    paramter,
                                                    value.ToLowerInvariant()))
                                  .ToString();
            ValidateOutput(result, paramter);
            return result;
        }

        /// <summary>
        /// Set a parameter value
        /// </summary>
        /// <param name="paramter">Parameter qualified name</param>
        /// <param name="value">a double value for the parameter</param>
        /// <returns>Not all parameters return a value after edit</returns>
        internal string SetParameter(string paramter, double value)
        {
            var result =
                PowerMill.DoCommandEx(string.Format(Resources.SetParameterNumaricValue, Identifier, Name, paramter, value))
                         .ToString();
            ValidateOutput(result, paramter);
            return result;
        }

        /// <summary>
        /// Set a parameter value
        /// </summary>
        /// <param name="paramter">Parameter qualified name</param>
        /// <param name="value">a boolean value for the parameter</param>
        /// <returns>Not all parameters return a value after edit</returns>
        internal string SetParameter(string paramter, bool value)
        {
            return SetParameter(paramter, Convert.ToDouble(value));
        }

        /// <summary>
        /// Set a parameter value
        /// </summary>
        /// <param name="paramter">Parameter qualified name</param>
        /// <param name="value">a <see cref="MM"/> value for the parameter</param>
        /// <returns>Not all parameters return a value after edit</returns>
        internal string SetParameter(string paramter, MM value)
        {
            return SetParameter(paramter, value.Value);
        }

        /// <summary>
        /// Set a parameter value
        /// </summary>
        /// <param name="paramter">Parameter qualified name</param>
        /// <param name="value">a <see cref="Degree"/> value for the parameter</param>
        /// <returns>Not all parameters return a value after edit</returns>
        internal string SetParameter(string paramter, Degree value)
        {
            return SetParameter(paramter, value.Value);
        }

        /// <summary>
        /// Duplicates an entity. It also updates PowerMill.
        /// </summary>
        public virtual PMEntity Duplicate()
        {
            PMEntity copiedItem = null;

            // Use Clipboard copy for a model to avoid empty model
            if (Identifier == "MODEL")
            {
                PowerMill.DoCommand("EDIT MODEL '" + _name + "' CLIPBOARD COPY");
                PowerMill.DoCommand("CREATE MODEL CLIPBOARD");
            }
            else
            {
                PowerMill.DoCommand("Copy " + Identifier + " '" + _name + "'");
            }

            List<PMEntity> createdItems = PowerMill.ActiveProject.CreatedItems();
            if (createdItems.Count == 1)
            {
                if (createdItems[0].Identifier == Identifier)
                {
                    copiedItem = createdItems[0];
                }
                else
                {
                    throw new ApplicationException("Item not duplicated");
                }
            }
            else
            {
                List<PMEntity> entLst =
                    (from ent in createdItems where ent.Identifier == Identifier select ent).ToList();
                if ((entLst.Count > 1) | (entLst.Count == 0))
                {
                    throw new ApplicationException("Too many items created. Do not know which is the one duplicated!");
                }
                copiedItem = entLst[0];

                //return item
            }
            if (copiedItem != null)
            {
                PowerMill.ActiveProject.AddEntityToCollection(copiedItem);
            }
            return copiedItem;
        }

        /// <summary>
        /// Deletes an entity.
        /// </summary>
        public abstract void Delete();

        #endregion

        #region Overrides

        public override bool Equals(object obj)
        {
            return (obj is PMEntity) ? _id.Equals(((PMEntity)obj)._id) : obj.Equals(this);
        }

        #endregion

        private void ValidateOutput(string result, string parameter)
        {
            if (!string.IsNullOrEmpty(result) && result.ToLower().Contains(ErrorMessage.ToLower()))
            {
                throw new InvalidPowerMillParameterException(PowerMill.Version, parameter);
            }
        }
    }
}