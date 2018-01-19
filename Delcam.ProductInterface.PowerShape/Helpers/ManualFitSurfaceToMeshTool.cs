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
    /// Helper to manual fit a surface to a mesh.
    /// </summary>
    /// <remarks></remarks>
    public class ManualFitSurfaceToMeshTool
    {
        private PSMesh _mesh;
        private PSAutomation _powershape;
        private PSMeshTrianglePicker _trianglePicker;
        private bool _isToolCurrentlyActive;
        private FeatureTypes _featureTypeToRecognise;
        private bool _generateFeaturesAsSolids;
        private bool _blankKeptFeatures = true;

        private bool _displayFeaturesAsTransparent = true;

        /// <summary>
        /// Initialises the new instance with the mesh to work with and the base instance to interact with PowerShape.
        /// The feature type to recognise is set to plane.
        /// </summary>
        /// <param name="mesh">The mesh to fit a surface to.</param>
        /// <remarks></remarks>
        public ManualFitSurfaceToMeshTool(PSMesh mesh)
        {
            _mesh = mesh;
            _powershape = mesh.PowerSHAPE;
            _trianglePicker = new PSMeshTrianglePicker(mesh);
            _featureTypeToRecognise = FeatureTypes.Plane;

            // Only chose this because Powershape defaults to Plane
        }

        #region "Properties"

        /// <summary>
        /// Gets the mesh triangle picker instance.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public PSMeshTrianglePicker TrianglePicker
        {
            get { return _trianglePicker; }
        }

        /// <summary>
        /// Gets the previous set value and sets the feature type to recognise in PowerShape.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>Feature type to recognise will only be updated in PowerShape if _isToolCurrentlyActive is true.</remarks>
        public FeatureTypes FeatureTypeToRecognise
        {
            get { return _featureTypeToRecognise; }
            set
            {
                _featureTypeToRecognise = value;
                if (_isToolCurrentlyActive)
                {
                    RecogniseType(value);
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
        /// Starts manual segment mesh operation in PowerShape.
        /// </summary>
        /// <remarks></remarks>
        public void StartTool()
        {
            _mesh.AddToSelection(true);
            _powershape.DoCommand("Segmenter ManualSegment");
            _trianglePicker.StartPicking();
            _isToolCurrentlyActive = true;

            RecogniseType(_featureTypeToRecognise);
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
        /// Updates PowerShape with the feature type to recognise.
        /// </summary>
        /// <param name="type">The feature type to recognise.</param>
        /// <remarks></remarks>
        private void RecogniseType(FeatureTypes type)
        {
            switch (type)
            {
                case FeatureTypes.Cone:
                    _powershape.DoCommand("Primtype Cone");
                    break;
                case FeatureTypes.Cylinder:
                    _powershape.DoCommand("Primtype Cylinder");
                    break;
                case FeatureTypes.Extrusion:
                    _powershape.DoCommand("Primtype Extrusion");
                    break;
                case FeatureTypes.Plane:
                    _powershape.DoCommand("Primtype Plane");
                    break;
                case FeatureTypes.RevolvedSurface:
                    _powershape.DoCommand("Primtype Revolution");
                    break;
                case FeatureTypes.ShrinkWrap:
                    _powershape.DoCommand("Primtype ShrinkWrap");
                    break;
                case FeatureTypes.Sphere:
                    _powershape.DoCommand("Primtype Sphere");
                    break;
                case FeatureTypes.Tori:
                    _powershape.DoCommand("Primtype Torus");
                    break;
            }
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
        /// Return the last generated surface.
        /// </summary>
        /// <returns>The last surface that was generated from the mesh.</returns>
        public PSSurface Extract()
        {
            if (!_isToolCurrentlyActive)
            {
                throw new Exception("Tool has not been started.");
            }

            _powershape.DoCommand("Apply");

            // Process any new entities
            PSSurface newEntity = null;
            if (_powershape.ActiveModel.CreatedItems.Count == 1)
            {
                newEntity = (PSSurface) _powershape.ActiveModel.CreatedItems[0];
                newEntity.Id = _powershape.ReadIntValue(newEntity.Identifier + "['" + newEntity.Name + "'].ID");
            }

            return newEntity;
        }

        /// <summary>
        /// Finishes the manual fit operation.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public List<PSSurface> Finish()
        {
            if (!_isToolCurrentlyActive)
            {
                throw new Exception("Tool has not been started.");
            }

            _powershape.DoCommand("Apply");
            StopTool();

            List<PSSurface> newSurfaces = new List<PSSurface>();
            foreach (PSSurface surface in _powershape.ActiveModel.CreatedItems)
            {
                surface.Id = _powershape.ReadIntValue(surface.Identifier + "['" + surface.Name + "'].ID");

                //If _powershape.ActiveModel.Surfaces.Count(Function(x) x.Id = surface.Id) = 0 Then
                // Only add surface to model if not already present (a previous call to Extract() may have already put surface in model).
                _powershape.ActiveModel.Add(surface);

                //End If
                newSurfaces.Add(surface);
            }
            _powershape.ActiveModel.ClearCreatedItems();
            return newSurfaces;
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

            _trianglePicker.StopPicking();

            _isToolCurrentlyActive = false;
        }
    }
}