// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using Autodesk.Geometry;

namespace Autodesk.ProductInterface.PowerSHAPE
{
    /// <summary>
    /// Abstract class to capture an entity in PowerSHAPE
    /// </summary>
    public abstract class PSEntity : PSDatabaseEntity
    {
        #region " Fields "

        protected internal int _levelNumber = -1;
        internal readonly string[] NewLine = {"\n", "\r\n"};

        #endregion

        #region " Constructors "

        /// <summary>
        /// Connects to PowerSHAPE
        /// </summary>
        internal PSEntity(PSAutomation powerSHAPE) : base(powerSHAPE)
        {
        }

        /// <summary>
        /// Connects to PowerSHAPE and a specific entity using its name
        /// </summary>
        internal PSEntity(PSAutomation powerSHAPE, int id, string name) : base(powerSHAPE)
        {
            _id = id;
            _name = name;
        }

        #endregion

        #region " Properties "

        /// <summary>
        /// Command used to rename the entity.  It can vary between entity types
        /// </summary>
        /// <returns>Command used to rename the entity</returns>
        internal virtual string RenameCommand
        {
            get { return "RENAME"; }
        }

        /// <summary>
        /// Gets the Name of the entity in PowerSHAPE
        /// </summary>
        public override string Name
        {
            get
            {
                if (_name == null)
                {
                    AbortIfDoesNotExist();
                    _name = _powerSHAPE.ReadStringValue(Identifier + "[ID " + _id + "].NAME");
                }

                return _name;
            }

            set
            {
                // Do the rename
                AddToSelection(true);
                _powerSHAPE.DoCommand(RenameCommand, "NAME " + value, "ACCEPT");

                // Set the name
                _name = value;
            }
        }

        /// <summary>
        /// Gets and Sets the Level the Entity is on in PowerSHAPE
        /// </summary>
        public virtual PSLevel Level
        {
            get { return _powerSHAPE.ActiveModel.Levels[(int) LevelNumber]; }

            set { LevelNumber = (uint) value.Number; }
        }

        /// <summary>
        /// Gets and sets the Level Number the Entity is on in PowerSHAPE
        /// </summary>
        public virtual uint LevelNumber
        {
            get
            {
                if (_levelNumber == -1)
                {
                    string levelNumberString = _powerSHAPE.ReadStringValue(Identifier + "[ID " + _id + "].LEVEL");
                    uint levelNumberValue = 0;
                    if (uint.TryParse(levelNumberString, out levelNumberValue) == false)
                    {
                        throw new NullReferenceException("Item does not exist in PowerSHAPE");
                    }
                    _levelNumber = (int) levelNumberValue;
                }
                return (uint) _levelNumber;
            }

            set
            {
                //Add to the selection and move to the defined level number
                AddToSelection(true);
                _powerSHAPE.DoCommand("LEVELSELECTOR INPUTLEVEL " + value);
                _levelNumber = (int) value;
            }
        }

        /// <summary>
        /// Gets the bounding box of the Entity in PowerSHAPE
        /// </summary>
        /// <returns>A BoundingBox object representing the bounding box in PowerSHAPE</returns>
        public virtual BoundingBox BoundingBox
        {
            get
            {
                AbortIfDoesNotExist();
                AddToSelection(true);
                double[] minSize = _powerSHAPE.DoCommandEx("SELECTION.MIN_RANGE_EXACT") as double[];
                double[] maxSize = _powerSHAPE.DoCommandEx("SELECTION.MAX_RANGE_EXACT") as double[];

                return new BoundingBox(minSize[0], maxSize[0], minSize[1], maxSize[1], minSize[2], maxSize[2]);
            }
        }

        /// <summary>
        /// Gets and Sets the transparency of the Entity in PowerSHAPE
        /// </summary>
        public virtual double Transparency
        {
            get
            {
                AbortIfDoesNotExist();
                return _powerSHAPE.ReadDoubleValue(Identifier + "[ID " + _id + "].MATERIAL.TRANSPARENCY");
            }

            set
            {
                AddToSelection(true);
                _powerSHAPE.DoCommand("FORMAT OBJECT_TRANSPARENCY VALUE " + value);
            }
        }

        /// <summary>
        /// Gets the PowerSHAPE name of the object
        /// </summary>
        public virtual string PowerSHAPEType
        {
            get { return Identifier; }
        }

        /// <summary>
        /// Gets the composite ID for use in communication with the ActiveDocument object
        /// </summary>
        internal virtual int CompositeID
        {
            get { throw new NotImplementedException("Not available on the base type"); }
        }

        #endregion

        #region " Operations "

        /// <summary>
        /// Deletes the item from PowerSHAPE and removes it from the correct collection
        /// if it is a member of one
        /// </summary>
        public abstract void Delete();

        /// <summary>
        /// Overrides the GetHashCode function to return the ID as an integer
        /// </summary>
        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }

        /// <summary>
        /// Adds the entity to the selection and optionally clears the current selection
        /// </summary>
        /// <param name="emptySelectionFirst">Optionally clears the current selection</param>
        public virtual void AddToSelection(bool emptySelectionFirst = false)
        {
            // Ensure the items level is active first
            if (Level.IsActive == false)
            {
                Level.IsActive = true;
            }

            string command = "";
            if (emptySelectionFirst)
            {
                command = "SELECT CLEARLIST ";
            }
            command += string.Format("ADD " + Identifier + " '{0}'", Name);

            _powerSHAPE.DoCommand(command);
        }

        /// <summary>
        /// Removes the Entity from the current selection
        /// </summary>
        public virtual void RemoveFromSelection()
        {
            AbortIfDoesNotExist();
            _powerSHAPE.DoCommand("REMOVE " + Identifier + " '" + Name + "'");
        }

        /// <summary>
        /// Adds the item to the clipboard
        /// </summary>
        internal virtual void Copy()
        {
            AddToSelection(true);
            _powerSHAPE.DoCommand("COPY");
        }

        /// <summary>
        /// Copies and pastes the Entity.
        /// NB:Any dependencies are also copied and cause an exception to be thrown.
        /// </summary>
        /// <exception cref="NotImplementedException">Doesn't yet support multiple pasted objects</exception>
        /// <exception cref="Exception">Item in clipboard is not a PowerSHAPE, or the expected object</exception>
        public virtual PSEntity Duplicate()
        {
            Copy();
            _powerSHAPE.ActiveModel.ClearSelectedItems();
            _powerSHAPE.DoCommand("PASTE");

            int nSelectedItemsCount = _powerSHAPE.ActiveModel.SelectedItems.Count;

            //only need count once

            // No object has been pasted within PowerSHAPE
            if (nSelectedItemsCount == 0)
            {
                throw new Exception("Item in clipboard is not a PowerSHAPE object");

                // One object has been pasted in PowerSHAPE
            }
            if (nSelectedItemsCount == 1)
            {
                PSEntity newEntity = _powerSHAPE.ActiveModel.SelectedItems[0];
                if (newEntity.Identifier != Identifier)
                {
                    // The pasted object is not of the required type
                    // Delete the incorrect object
                    newEntity.AddToSelection(true);
                    _powerSHAPE.DoCommand("DELETE");
                    throw new Exception("Item in clipboard is not a " + Identifier);
                }
                _powerSHAPE.ActiveModel.Add(newEntity);
                return newEntity;

                // More than one item was pasted in PowerSHAPE
            }
            throw new NotImplementedException("Doesn't yet support multiple pasted objects!");
        }

        /// <summary>
        /// Shifts the entity from being relative to one workplane to being relative to another
        /// </summary>
        /// <param name="toWorkplane">The workplane to rebase to</param>
        /// <param name="fromWorkplane">Optional parameter.  The workplane to rebase from.  Default of Nothing is world.</param>
        /// <exception cref="NotImplementedException">Doesn't yet support multiple pasted objects</exception>
        /// <exception cref="Exception">Item in clipboard is not a PowerSHAPE, or the expected object</exception>
        public virtual void RebaseToWorkplane(PSWorkplane toWorkplane, PSWorkplane fromWorkplane = null)
        {
            // Activate the from workplane (default is world)
            _powerSHAPE.ActiveModel.ActiveWorkplane = fromWorkplane;

            Copy();
            _powerSHAPE.ActiveModel.ClearSelectedItems();

            // Activate the to workplane
            _powerSHAPE.ActiveModel.ActiveWorkplane = toWorkplane;
            _powerSHAPE.DoCommand("PASTE");

            // No object has been pasted within PowerSHAPE
            if (_powerSHAPE.ActiveModel.SelectedItems.Count == 0)
            {
                throw new Exception("Item in clipboard is not a PowerSHAPE object");

                // One object has been pasted in PowerSHAPE
            }
            if (_powerSHAPE.ActiveModel.SelectedItems.Count == 1)
            {
                PSEntity newEntity = _powerSHAPE.ActiveModel.SelectedItems[0];
                if (newEntity.Identifier != Identifier)
                {
                    // The pasted object is not of the required type
                    // Delete the incorrect object
                    newEntity.AddToSelection(true);
                    _powerSHAPE.DoCommand("DELETE");
                    throw new Exception("Item in clipboard is not a " + Identifier);
                }

                // Delete the original item and link this entity to the new one
                AddToSelection(true);
                _powerSHAPE.DoCommand("DELETE");
                _id = newEntity.Id;
                _name = newEntity.Name;

                // More than one item was pasted in PowerSHAPE
            }
            else
            {
                throw new NotImplementedException("Doesn't yet support multiple pasted objects!");
            }
        }

        /// <summary>
        /// Blanks the Entity in PowerSHAPE
        /// </summary>
        public virtual void Blank()
        {
            AbortIfDoesNotExist();

            // Blank the Entity
            AddToSelection(true);
            _powerSHAPE.DoCommand("DISPLAY BLANKSELECTED");
        }

        /// <summary>
        /// Blanks everything apart from this Entity in PowerSHAPE
        /// </summary>
        public virtual void BlankExcept()
        {
            AbortIfDoesNotExist();

            // Blank the Entity
            AddToSelection(true);
            _powerSHAPE.DoCommand("DISPLAY BLANKEXCEPT");
        }

        /// <summary>
        /// Gets the distance between this entity and another
        /// </summary>
        /// <param name="otherEntity">The function returns the distance between this entity and the otherEntity</param>
        /// <exception cref="Exception">Failed to determine distance between objects</exception>
        public MM DistanceTo(PSEntity otherEntity)
        {
            object[] values;
            try
            {
                values = _powerSHAPE.ActiveDocument.MinDist(CompositeID, otherEntity.CompositeID);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to determine distance between objects", ex);
            }

            if (values.Length != 2 || int.Parse(values[0].ToString()) == -1)
            {
                throw new Exception("Failed to determine distance between objects");
            }
            return double.Parse(values[1].ToString());
        }

        /// <summary>
        /// Sets the level number to the specified value - used during cache refreshes
        /// </summary>
        internal void SetLevelNumber(int value)
        {
            _levelNumber = value;
        }

        #endregion
    }
}