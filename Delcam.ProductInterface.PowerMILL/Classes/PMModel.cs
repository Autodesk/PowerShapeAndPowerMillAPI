// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using System.Linq;
using Autodesk.Geometry;

namespace Autodesk.ProductInterface.PowerMILL
{
    /// <summary>
    /// Captures a Model object in PowerMILL.
    /// </summary>
    public class PMModel : PMEntity
    {
        #region Constructors

        /// <summary>
        /// Initialises the item.
        /// </summary>
        /// <param name="powerMILL">The base instance to interact with PowerMILL.</param>
        internal PMModel(PMAutomation powerMILL) : base(powerMILL)
        {
        }

        /// <summary>
        /// Initialises the item.
        /// </summary>
        /// <param name="powerMILL">The base instance to interact with PowerMILL.</param>
        /// <param name="name">The new instance name.</param>
        internal PMModel(PMAutomation powerMILL, string name) : base(powerMILL, name)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets and sets the active state of the Model.
        /// </summary>
        public override bool IsActive
        {
            get { throw new Exception("This property is not valid on a model"); }
            set { throw new Exception("This property is not valid on a model"); }
        }

        /// <summary>
        /// Sets the transparency of the Model in PowerMill. 0 displays a solid model and 100 displays a transparent one.
        /// Values outside these ranges will be limited.
        /// </summary>
        public int Transparency
        {
            set
            {
                // Lower limit of value is 0
                if (value < 0)
                {
                    value = 0;
                }

                // Upper limit of value is 100
                if (value > 100)
                {
                    value = 100;
                }
                PowerMill.DoCommand("EDIT MODEL '" + Name + "' TRANSLUCENCY " + value);
            }
        }

        /// <summary>
        /// Sets the Level in PowerMill that this Model is assigned to.
        /// </summary>
        public PMLevelOrSet Level
        {
            set
            {
                AddToSelection(true);
                PowerMill.DoCommand("EDIT LEVEL \"" + value.Name + "\" ACQUIRE SELECTED");
            }
        }

        /// <summary>
        /// Gets the path of the model.
        /// </summary>
        public string Path
        {
            get
            {
                string output = PowerMill.DoCommandEx("PRINT ENTITY PARAMETERS MODEL '" + Name + "'").ToString();
                foreach (
                    string value in output.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (value.Trim().StartsWith("PATH"))
                    {
                        return value.Trim().Substring(9);
                    }
                }
                return "";
            }
        }

        internal static string MODEL_IDENTIFIER = "MODEL";

        /// <summary>
        /// Gets the identifier PowerMILL uses to identify this type of entity.
        /// </summary>
        internal override string Identifier
        {
            get { return MODEL_IDENTIFIER; }
        }

        #endregion

        #region Operations

        /// <summary>
        /// Deletes model from the active project. It updates PowerMill.
        /// </summary>
        public override void Delete()
        {
            PowerMill.ActiveProject.Models.Remove(this);
        }

        /// <summary>
        /// Exports the model to the specified file.
        /// </summary>
        /// <param name="file">The file to which the model will be exported.</param>
        public void ExportModel(FileSystem.File file,
                                bool exportInActiveWorkplane = true)
        {
            // Delete the file if it already exists
            file.Delete();
            if (exportInActiveWorkplane == false)
            {
                PowerMill.DoCommand("DEACTIVATE WORKPLANE");
            }
            PowerMill.DoCommand("EXPORT MODEL '" + Name + "' '" + file.Path + "'");
        }

        /// <summary>
        /// Exports the model into a DMT file and then reads the DMT model into a DMTModel object.
        /// </summary>
        public DMTModel ToDMTModel()
        {
            try
            {
                // Export the model to a temporary file
                FileSystem.File tempFile = FileSystem.File.CreateTemporaryFile("dmt");
                ExportModel(tempFile, false);

                // Read the file in as a DMT Object
                DMTModel objDMTModel = DMTModelReader.ReadFile(tempFile);

                // Delete the file
                tempFile.Delete();
                return objDMTModel;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Mirrors the model in the plane.
        /// </summary>
        /// <param name="plane">The plane used to mirror the model.</param>
        public void MirrorInPlane(Planes plane)
        {
            switch (plane)
            {
                case Planes.XY:
                    PowerMill.DoCommand("TRANSFORM TYPE SCALEZ TRANSFORM SCALEVALUE -1 TRANSFORM MODEL '" + Name + "'");
                    break;
                case Planes.ZX:
                    PowerMill.DoCommand("TRANSFORM TYPE SCALEY TRANSFORM SCALEVALUE -1 TRANSFORM MODEL '" + Name + "'");
                    break;
                case Planes.YZ:
                    PowerMill.DoCommand("TRANSFORM TYPE SCALEX TRANSFORM SCALEVALUE -1 TRANSFORM MODEL '" + Name + "'");
                    break;
            }
        }

        /// <summary>
        /// Moves the model by the specified vector.
        /// </summary>
        /// <param name="moveVector">The vector of how to move the model in X, Y, Z.</param>
        public void Move(Vector moveVector)
        {
            PowerMill.DoCommand("TRANSFORM TYPE MOVE TRANSFORM MOVEX \"" + moveVector.I + "\"",
                                "TRANSFORM TYPE MOVE TRANSFORM MOVEY \"" + moveVector.J + "\"",
                                "TRANSFORM TYPE MOVE TRANSFORM MOVEZ \"" + moveVector.K + "\"",
                                "TRANSFORM MODEL \"" + Name + "\"");
        }

        /// <summary>
        /// Rotate the model by the specified axis and angle.
        /// </summary>
        /// <param name="rotateAxis">The rotation axis</param>
        /// <param name="rotateAngle">The rotation angle</param>
        public void Rotate(Axes rotateAxis, double rotateAngle)
        {
            switch (rotateAxis)
            {
                case Axes.X:
                    PowerMill.DoCommand("TRANSFORM ANGLE \"" + rotateAngle + "\" TRANSFORM TYPE ROTATEX TRANSFORM MODEL \"" + Name + "\"");
                    break;
                case Axes.Y:
                    PowerMill.DoCommand("TRANSFORM ANGLE \"" + rotateAngle + "\" TRANSFORM TYPE ROTATEY TRANSFORM MODEL \"" + Name + "\"");
                    break;
                case Axes.Z:
                    PowerMill.DoCommand("TRANSFORM ANGLE \"" + rotateAngle + "\" TRANSFORM TYPE ROTATEZ TRANSFORM MODEL \"" + Name + "\"");
                    break;
            }
        }

        /// <summary>
        /// Selects the model, optionally removing everything else from the selection first.
        /// </summary>
        /// <param name="clearSelectionFirst">If true unselects everything else before selecting this model.</param>
        public void AddToSelection(bool clearSelectionFirst = false)
        {
            if (clearSelectionFirst)
            {
                PowerMill.DoCommand("EDIT MODEL ALL DESELECT ALL");
            }
            PowerMill.DoCommand("EDIT MODEL \"" + Name + "\" SELECT ALL");
        }

        /// <summary>
        /// Removes the model from the selection.
        /// </summary>
        public void RemoveFromSelection()
        {
            PowerMill.DoCommand("EDIT MODEL \"" + Name + "\" DESELECT ALL");
        }

        /// <summary>
        /// Inverts the selection of surfaces.  Those surfaces that were selected are no longer selected.  Those that were previously
        /// unselected are now selected.
        /// </summary>
        public void InvertSelection()
        {
            PowerMill.DoCommand("EDIT MODEL \"" + Name + "\" SELECT INVERT ALL");
        }

        /// <summary>
        /// Reimports a file into PowerMill.
        /// </summary>
        /// <param name="file">The file to import.</param>
        public void Reimport(FileSystem.File file)
        {
            PowerMill.DoCommand(string.Format("EDIT MODEL '{0}' REIMPORT '{1}'", Name, file.Path));
        }

        #endregion
    }
}