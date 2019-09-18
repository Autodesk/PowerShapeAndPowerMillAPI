// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System.Collections.Generic;
using System.Linq;
using Autodesk.Geometry;

namespace Autodesk.ProductInterface.PowerMILL
{
    /// <summary>
    /// Collection of Pattern objects in the Active PowerMILL Project.
    /// </summary>
    public class PMPatternsCollection : PMEntitiesCollection<PMPattern>
    {
        #region Constructors

        /// <summary>
        /// Initialises the item.
        /// </summary>
        /// <param name="powerMILL">The base instance to interact with PowerMILL.</param>
        internal PMPatternsCollection(PMAutomation powerMILL) : base(powerMILL)
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
            foreach (string name in ReadPatterns())
            {
                Add(new PMPattern(_powerMILL, name));
            }
        }

        /// <summary>
        /// Gets a list of names of all Patterns in PowerMILL.
        /// </summary>
        internal List<string> ReadPatterns()
        {
            List<string> names = new List<string>();
            foreach (var pattern in _powerMILL.PowerMILLProject.Patterns)
            {
                names.Add(pattern.Name);
            }
            return names;
        }

        /// <summary>
        /// Creates an empty Pattern in PowerMill and adds it to the boundaries collection.
        /// </summary>
        /// <returns>The new PMPattern.</returns>
        public PMPattern CreateEmptyPattern()
        {
            _powerMILL.DoCommand("CREATE PATTERN ;");

            var newPattern = (PMPattern) _powerMILL.ActiveProject.CreatedItems(typeof(PMPattern)).Last();
            Add(newPattern);

            return newPattern;
        }

        /// <summary>
        /// Adds the specified Polyline to PowerMILL as a Pattern.
        /// </summary>
        /// <param name="polyline">The polyline to add.</param>
        public PMPattern CreatePattern(Polyline polyline)
        {
            try
            {
                PMPattern newPattern = null;

                // Write the Polyline to a file
                FileSystem.File tempFile = FileSystem.File.CreateTemporaryFile("pic");
                polyline.WriteToDUCTPictureFile(tempFile);

                // Then import it into PowerMILL (this includes adding it to the list)
                newPattern = CreatePattern(tempFile);

                // Delete the file
                tempFile.Delete();
                return newPattern;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Adds the specified Spline as a Pattern.
        /// </summary>
        /// <param name="spline"></param>
        /// <returns></returns>
        public PMPattern CreatePattern(Spline spline)
        {
            try
            {
                PMPattern newPattern = null;

                // Write the Spline to a file
                FileSystem.File tempFile = FileSystem.File.CreateTemporaryFile("pic");
                spline.WriteToDUCTPictureFile(tempFile);

                // Then import it into PowerMill (this includes adding to the list)
                newPattern = CreatePattern(tempFile);

                // Clean up
                tempFile.Delete();

                return newPattern;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Imports the DUCT Picture file into PowerMILL as a Pattern and adds it to the collection.
        /// </summary>
        /// <param name="file">the DUCT Picture file to import.</param>
        public PMPattern CreatePattern(FileSystem.File file)
        {
            PMPattern newPattern = null;
            if (file.Exists)
            {
                _powerMILL.DoCommand("CREATE PATTERN ;");

                // Get the name PowerMILL gave to the last pattern
                newPattern = (PMPattern) _powerMILL.ActiveProject.CreatedItems(typeof(PMPattern)).Last();
                Add(newPattern);
                _powerMILL.DoCommand("EDIT PATTERN '" + newPattern.Name + "' INSERT FILE '" + file.Path + "'");
            }
            return newPattern;
        }
        #endregion
    }
}