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

namespace Autodesk.ProductInterface.PowerSHAPE
{
    /// <summary>
    /// Captures the list of open models in PowerSHAPE.
    /// </summary>
    public class PSModelsCollection : PSDatabaseEntitiesCollection<PSModel>
    {
        #region " Constructors "

        /// <summary>
        /// Creates a new collection.
        /// </summary>
        /// <param name="powerSHAPE">The base instance to interact with PowerShape.</param>
        internal PSModelsCollection(PSAutomation powerSHAPE) : base(powerSHAPE)
        {
        }

        #endregion

        #region " Properties "

        /// <summary>
        /// Gets the model identifier in PowerSHAPE.
        /// </summary>
        internal override string Identifier
        {
            get { return "MODEL"; }
        }

        #endregion

        #region " Operations "

        /// <summary>
        /// Gets the desired named model from the the collection.
        /// </summary>
        /// <returns>The desired model.</returns>
        /// <value>The name of the desired model.</value>
        public PSModel this[string name]
        {
            // Use List property
            get
            {
                foreach (PSModel model in this)
                {
                    if (model.Name == name)
                    {
                        return model;
                    }
                }

                // Model is not in collection
                return null;
            }
            set { this[Convert.ToInt32(name)] = value; }
        }

        /// <summary>
        /// Creates a new empty model in PowerSHAPE.
        /// </summary>
        /// <returns>The created empty model.</returns>
        public PSModel CreateEmptyModel()
        {
            PSModel newModel = new PSModel(_powerSHAPE);
            Add(newModel);
            newModel.Initialise();
            _powerSHAPE.DoCommand("FORM DELETION OFF");
            return newModel;
        }

        /// <summary>
        /// Creates a new Model from a PowerSHAPE model file.
        /// </summary>
        /// <param name="file">The PowerSHAPE model file to create the model from.</param>
        /// <param name="openReadOnly">Open the model read only. Defaults to false.</param>
        /// <returns>The created model.</returns>
        public PSModel CreateModelFromFile(File file, bool openReadOnly = false)
        {
            PSModel newModel = new PSModel(_powerSHAPE, file, openReadOnly);
            Add(newModel);
            newModel.Initialise();
            _powerSHAPE.DoCommand("FORM DELETION OFF");
            return newModel;
        }

        /// <summary>
        /// Removes the model at the specified index from the collection. Closes all windows associated with the selected model.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        public new void RemoveAt(int index)
        {
            if (index >= Count)
            {
                throw new IndexOutOfRangeException("Current number of items is " + Count);
            }

            //Close all windows associated with the selected model
            PSModel model = this[index];
            Remove(model);
        }

        /// <summary>
        /// Removes the model with the specified name from the collection. Closes all windows associated with the selected model.
        /// </summary>
        /// <param name="name">The name of the model to remove.</param>
        /// <returns>True if succeeds to remove model.</returns>
        public bool Remove(string name)
        {
            // Get model with the specified name
            PSModel modelToRemove = null;
            modelToRemove = this[name];

            // Remove model
            try
            {
                return Remove(modelToRemove);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Removes the specified model. Closes all windows associated with the selected model.
        /// </summary>
        /// <param name="model">The model to remove.</param>
        /// <returns>True if succeeds to remove model.</returns>
        public bool Remove(PSModel model)
        {
            if (model.Exists)
            {
                // Close all windows associated with the selected model
                _powerSHAPE.Windows.Remove(model.Window);
            }

            if (Contains(model))
            {
                return base.Remove(model);
            }

            return false;
        }

        /// <summary>
        /// Closes and removes all models in the list. Closes all windows.
        /// </summary>
        /// <remarks></remarks>
        public override void Clear()
        {
            // Close all open windows
            _powerSHAPE.Windows.Clear();

            // Clear list
            base.Clear();
        }

        /// <summary>
        /// Checks whether the models collection contains a model of the specified name.
        /// </summary>
        /// <param name="file">A Delcam file indicating the model to be found.</param>
        /// <returns>A boolean indicating whether the model has been found.</returns>
        public bool Contains(File file)
        {
            // Cycle through all models to see whether any have that filename
            foreach (PSModel openModel in this)
            {
                if (openModel.Name == file.NameWithoutExtension)
                {
                    return true;
                }
            }

            // Model has not been found, so return false
            return false;
        }

        #endregion
    }
}