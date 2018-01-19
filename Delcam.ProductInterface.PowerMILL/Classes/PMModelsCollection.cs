// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System.Collections.Generic;
using System.Threading;
using Autodesk.FileSystem;
using Autodesk.Geometry;

namespace Autodesk.ProductInterface.PowerMILL
{
    /// <summary>
    /// Collection of Model objects in the Active PowerMILL Project.
    /// </summary>
    public class PMModelsCollection : PMEntitiesCollection<PMModel>
    {
        #region Constructors

        /// <summary>
        /// Initialises the item.
        /// </summary>
        /// <param name="powerMILL">The base instance to interact with PowerMILL.</param>
        internal PMModelsCollection(PMAutomation powerMILL) : base(powerMILL)
        {
            Initialise();
        }

        #endregion

        #region Operations

        /// <summary>
        /// Initialises the collection with the models in PowerMILL.
        /// </summary>
        internal void Initialise()
        {
            foreach (string name in ReadModels())
            {
                Add(new PMModel(_powerMILL, name));
            }
        }

        /// <summary>
        /// Gets the list of the names of all the Models in PowerMILL.
        /// </summary>
        internal List<string> ReadModels()
        {
            List<string> names = new List<string>();
            foreach (var model in _powerMILL.PowerMILLProject.Models)
            {
                names.Add(model.Name);
            }
            return names;
        }

        /// <summary>
        /// Creates a reference surface model from a DMTModel.
        /// </summary>
        /// <param name="DMT">The DMTModel to set as a reference surface model.</param>
        public PMModel CreateReferenceModel(DMTModel DMT)
        {
            try
            {
                PMModel newModel = null;

                // Write the DMT Model to a file
                File tempFile = File.CreateTemporaryFile("dmt");
                DMTModelWriter.WriteFile(DMT, tempFile);

                // Then import it into PowerMILL (this includes adding it to the list)
                newModel = CreateReferenceModel(tempFile);

                // Delete the file
                tempFile.Delete();
                return newModel;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Creates a reference surface model from a file.
        /// </summary>
        /// <param name="file">The DMT file imported to create the reference surface model.</param>
        public PMModel CreateReferenceModel(File file)
        {
            if (file.Exists)
            {
                _powerMILL.DoCommand("IMPORT REFERENCE_SURFACES '" + file.Path + "'");

                // Add the model to the list of models.
                // When you import a model file it can also bring in other items so handles these also
                List<PMEntity> createdItems = _powerMILL.ActiveProject.CreatedItems();
                PMModel modelToReturn = null;
                foreach (PMEntity createdItem in createdItems)
                {
                    if (createdItem is PMBoundary)
                    {
                        _powerMILL.ActiveProject.Boundaries.Add((PMBoundary) createdItem);
                    }
                    else if (createdItem is PMLevelOrSet)
                    {
                        _powerMILL.ActiveProject.LevelsAndSets.Add((PMLevelOrSet) createdItem);
                    }
                    else if (createdItem is PMModel)
                    {
                        _powerMILL.ActiveProject.Models.Add((PMModel) createdItem);
                        modelToReturn = (PMModel) createdItem;
                    }
                    else if (createdItem is PMPattern)
                    {
                        _powerMILL.ActiveProject.Patterns.Add((PMPattern) createdItem);
                    }
                    else if (createdItem is PMWorkplane)
                    {
                        _powerMILL.ActiveProject.Workplanes.Add((PMWorkplane) createdItem);
                    }
                }
                return modelToReturn;
            }
            return null;
        }

        /// <summary>
        /// Adds the DMT Model into PowerMILL and adds the Model to the collection.
        /// </summary>
        /// <param name="DMT">The DMTModel to add.</param>
        public PMModel CreateModel(DMTModel DMT)
        {
            try
            {
                PMModel newModel;

                // Write the DMT Model to a file
                File tempFile = File.CreateTemporaryFile("dmt");
                DMTModelWriter.WriteFile(DMT, tempFile);

                // Then import it into PowerMILL (this includes adding it to the list)
                newModel = CreateModel(tempFile);

                // Delete the file
                tempFile.Delete();
                return newModel;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Imports the specified DMT file into PowerMILL and adds the model to the collection.
        /// </summary>
        /// <param name="file">The DMT file to import.</param>
        public PMModel CreateModel(File file)
        {
            if (file.Exists)
            {
                _powerMILL.DoCommand("IMPORT MODEL '" + file.Path + "'");

                // Add the model to the list of models.
                // When you import a model file it can also bring in other items so handles these also
                List<PMEntity> createdItems = _powerMILL.ActiveProject.CreatedItems();
                PMModel modelToReturn = null;
                foreach (PMEntity createdItem in createdItems)
                {
                    if (createdItem is PMBoundary)
                    {
                        _powerMILL.ActiveProject.Boundaries.Add((PMBoundary) createdItem);
                    }
                    else if (createdItem is PMLevelOrSet)
                    {
                        _powerMILL.ActiveProject.LevelsAndSets.Add((PMLevelOrSet) createdItem);
                    }
                    else if (createdItem is PMModel)
                    {
                        _powerMILL.ActiveProject.Models.Add((PMModel) createdItem);
                        modelToReturn = (PMModel) createdItem;
                    }
                    else if (createdItem is PMPattern)
                    {
                        _powerMILL.ActiveProject.Patterns.Add((PMPattern) createdItem);
                    }
                    else if (createdItem is PMWorkplane)
                    {
                        _powerMILL.ActiveProject.Workplanes.Add((PMWorkplane) createdItem);
                    }
                }
                return modelToReturn;
            }
            return null;
        }

        /// <summary>
        /// Deletes the selected surfaces.  Note that this does not delete the models that are now empty.
        /// To do this you should call DeleteEmptyModels.
        /// </summary>
        public void DeleteSelectedSurfaces()
        {
            _powerMILL.DoCommand("DELETE MODEL SELECTED");
        }

        /// <summary>
        /// Deletes any empty models within PowerMILL.
        /// </summary>
        public void DeleteEmptyModels()
        {
            _powerMILL.DoCommand("DELETE MODEL EMPTY");

            // Refresh the model list
            int i = 0;
            while (i < Count)
                if (this[i].Exists == false)
                {
                    RemoveAt(i);
                }
                else
                {
                    i += 1;
                }
        }

        /// <summary>
        /// Exports all the models to the selected file.  The files should typically be DGK, DDX, DDZ or DMT.
        /// </summary>
        /// <param name="exportFile"></param>
        public void ExportAllModels(File exportFile)
        {
            if (Count > 0)
            {
                _powerMILL.DoCommand($"EXPORT MODEL ALL FILESAVE \"{exportFile}\" YES");

                // Wait until PowerMill stops being busy before continuing
                while (_powerMILL.IsBusy)
                    Thread.Sleep(200);
            }
        }

        #endregion
    }
}