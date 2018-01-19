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

namespace Autodesk.ProductInterface.PowerSHAPE
{
    /// <summary>
    /// Helps to create surfaces from meshes.
    /// </summary>
    /// <remarks></remarks>
    public class AutomaticFitSurfacesToMeshTool
    {
        private PSMesh _mesh;
        private PSAutomation _powershape;
        private bool _isToolCurrentlyActive;
        private double _similarityAngle = 1.0;
        private double _tolerance = 0.2;
        private HashSet<FeatureTypes> _featureTypesToRecognise;
        private bool _generateFeaturesAsSolids;
        private bool _blankKeptFeatures = true;

        private bool _displayFeaturesAsTransparent = true;

        /// <summary>
        /// Creates a new instance. Initialises the mesh and the base instance to interact with PowerShape.
        /// </summary>
        /// <param name="mesh">Initialises with the mesh used to create the surfaces.</param>
        /// <remarks></remarks>
        public AutomaticFitSurfacesToMeshTool(PSMesh mesh)
        {
            _mesh = mesh;
            _powershape = mesh.PowerSHAPE;
        }

        #region "Properties"

        /// <summary>
        /// Gets the previous set value and sets the 'Similarity Sliderval' in PowerShape.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>Similarity Sliderval will only be updated in PowerShape if _isToolCurrentlyActive is true.</remarks>
        public double SimilarityAngle
        {
            get { return _similarityAngle; }
            set
            {
                if ((value < 0) | (value > 10))
                {
                    throw new ArgumentOutOfRangeException("value", "angle must be between 0 and 10.");
                }

                _similarityAngle = value;
                if (_isToolCurrentlyActive)
                {
                    _powershape.DoCommand(string.Format("Similarity Sliderval {0}", value));
                }
            }
        }

        /// <summary>
        /// Gets the previous set value and sets the 'Tolerance' in PowerShape.
        /// </summary>
        /// <value></value>
        /// <returns>Tolerance will only be updated in PowerShape if _isToolCurrentlyActive is true.</returns>
        /// <remarks>
        /// .
        /// </remarks>
        public double Tolerance
        {
            get { return _tolerance; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("value", "tolerance must be > 0.");
                }

                _tolerance = value;
                if (_isToolCurrentlyActive)
                {
                    _powershape.DoCommand(string.Format("Tolerance {0}", value));
                }
            }
        }

        /// <summary>
        /// Gets the previous set value and sets the feature types to recognise in PowerShape.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>Feature types to recognise will only be updated in PowerShape if _isToolCurrentlyActive is true.</remarks>
        public HashSet<FeatureTypes> FeatureTypesToRecognise
        {
            get { return _featureTypesToRecognise; }
            set
            {
                _featureTypesToRecognise = value;
                if (_isToolCurrentlyActive)
                {
                    RecogniseTypes(value);
                }
            }
        }

        /// <summary>
        /// Set to TRUE to generate features as solids; FALSE to generate surfaces.
        /// </summary>
        public bool GenerateFeaturesAsSolids
        {
            get { return _generateFeaturesAsSolids; }
            set
            {
                _generateFeaturesAsSolids = value;
                if (_isToolCurrentlyActive)
                {
                    _powershape.DoCommand("MakeSolids yes");
                }
                else
                {
                    _powershape.DoCommand("MakeSolids no");
                }
            }
        }

        /// <summary>
        /// If TRUE, features will be transparent, preventing them obscuring each other.
        /// However this can result in selection conflicts, when clicking in an area of multiple entities.
        /// Set to FALSE to prevent selection conflicts.
        /// </summary>
        public bool DisplayFeaturesAsTransparent
        {
            get { return _displayFeaturesAsTransparent; }
            set
            {
                _displayFeaturesAsTransparent = value;

                if (_isToolCurrentlyActive)
                {
                    if (value)
                    {
                        _powershape.DoCommand("Transparency 1");
                    }
                    else
                    {
                        _powershape.DoCommand("Transparency 0");
                    }
                }
            }
        }

        /// <summary>
        /// If TRUE, features are blanked when they are persisted.
        /// </summary>
        public bool BlankKeptFeatures
        {
            get { return _blankKeptFeatures; }
            set
            {
                _blankKeptFeatures = value;
                if (_isToolCurrentlyActive)
                {
                    if (value)
                    {
                        _powershape.DoCommand("Blanking on");
                    }
                    else
                    {
                        _powershape.DoCommand("Blanking off");
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// Start operation.
        /// </summary>
        /// <remarks></remarks>
        public void Start()
        {
            _mesh.AddToSelection(true);
            _powershape.DoCommand("Segmenter");
            _isToolCurrentlyActive = true;

            _powershape.DoCommand(string.Format("Similarity Sliderval {0}", _similarityAngle));
            _powershape.DoCommand(string.Format("Tolerance {0}", _tolerance));
            RecogniseTypes(_featureTypesToRecognise);
            if (_blankKeptFeatures)
            {
                _powershape.DoCommand("Blanking on");
            }
            else
            {
                _powershape.DoCommand("Blanking off");
            }
            if (_displayFeaturesAsTransparent)
            {
                _powershape.DoCommand("Transparency 1");
            }
            else
            {
                _powershape.DoCommand("Transparency 0");
            }
            if (_generateFeaturesAsSolids)
            {
                _powershape.DoCommand("MakeSolids yes");
            }
            else
            {
                _powershape.DoCommand("MakeSolids no");
            }
        }

        /// <summary>
        /// Updates PowerShape with the feature types to recognise.
        /// </summary>
        /// <param name="types">Feature types to recognise.</param>
        /// <remarks></remarks>
        private void RecogniseTypes(HashSet<FeatureTypes> types)
        {
            if ((types == null) | (types.Count == 0))
            {
                return;
            }

            string command = "Primtypes Allow";
            foreach (FeatureTypes type_loopVariable in types)
            {
                var t = type_loopVariable;
                switch (t)
                {
                    case FeatureTypes.Cone:
                        command += " Cone";
                        break;
                    case FeatureTypes.Cylinder:
                        command += " Cylinder";
                        break;
                    case FeatureTypes.Extrusion:
                        command += " Extrusion";
                        break;
                    case FeatureTypes.Plane:
                        command += " Plane";
                        break;
                    case FeatureTypes.RevolvedSurface:
                        command += " RevolvedSurface";
                        break;
                    case FeatureTypes.ShrinkWrap:
                        throw new Exception("This tool does not support shrink wrap features.");
                    case FeatureTypes.Sphere:
                        command += " Sphere";
                        break;
                    case FeatureTypes.Tori:
                        command += " Tori";
                        break;
                }
            }

            // Command syntax:
            // primtypes [allow|disallow] [all|plane|cylinder|cone|sphere|torus|extrusion|revolution]*
            _powershape.DoCommand("Primtypes Disallow all");
            _powershape.DoCommand(command);
        }

        /// <summary>
        /// Generate and display the features.
        /// </summary>
        public void Preview()
        {
            if (!_isToolCurrentlyActive)
            {
                throw new Exception("Tool has not been started.");
            }

            _powershape.DoCommand("Preview");
        }

        /// <summary>
        /// Persist the selected features.
        /// </summary>
        /// <remarks></remarks>
        public List<PSSurface> ExtractSelectedFeatures()
        {
            if (!_isToolCurrentlyActive)
            {
                throw new Exception("Tool has not been started.");
            }

            _powershape.DoCommand("Apply");

            // Process any new entities
            List<PSSurface> newEntities = new List<PSSurface>();
            if (_powershape.ActiveModel.CreatedItems.Count > 0)
            {
                foreach (PSSurface newEntity in _powershape.ActiveModel.CreatedItems)
                {
                    newEntity.Id = _powershape.ReadIntValue(newEntity.Identifier + "['" + newEntity.Name + "'].ID");
                    _powershape.ActiveModel.Add(newEntity);
                    newEntities.Add(newEntity);
                }
                _powershape.ActiveModel.ClearCreatedItems();
            }

            return newEntities;
        }

        /// <summary>
        /// Close the tool.
        /// </summary>
        /// <returns>The features that were extracted from the mesh.</returns>
        public List<PSSurface> Finish()
        {
            if (!_isToolCurrentlyActive)
            {
                throw new Exception("Tool has not been started.");
            }

            _powershape.DoCommand("Cancel");
            _isToolCurrentlyActive = false;

            // Process any new entities
            List<PSSurface> newEntities = new List<PSSurface>();
            if (_powershape.ActiveModel.CreatedItems.Count > 0)
            {
                foreach (PSSurface newEntity in _powershape.ActiveModel.CreatedItems)
                {
                    newEntity.Id = _powershape.ReadIntValue(newEntity.Identifier + "['" + newEntity.Name + "'].ID");
                    _powershape.ActiveModel.Add(newEntity);
                    newEntities.Add(newEntity);
                }
                _powershape.ActiveModel.ClearCreatedItems();
            }

            return newEntities;
        }

        /// <summary>
        /// Stop the tool.
        /// </summary>
        public void StopTool()
        {
            if (!_isToolCurrentlyActive)
            {
                throw new Exception("Tool has not been started.");
            }

            _powershape.DoCommand("Cancel");

            _isToolCurrentlyActive = false;
        }
    }
}