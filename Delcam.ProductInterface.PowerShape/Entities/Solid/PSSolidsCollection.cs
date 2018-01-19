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

namespace Autodesk.ProductInterface.PowerSHAPE
{
    /// <summary>
    /// Captures the collection of Solids in a Project.
    /// </summary>
    public class PSSolidsCollection : PSEntitiesCollection<PSSolid>
    {
        #region " Constructors "

        /// <summary>
        /// Creates a new collection.
        /// </summary>
        /// <param name="powerSHAPE">The base instance to interact with PowerShape.</param>
        internal PSSolidsCollection(PSAutomation powerSHAPE) : base(powerSHAPE)
        {
        }

        #endregion

        #region " Properties "

        /// <summary>
        /// Gets the identifier used by PowerSHAPE when selecting a Solid.
        /// </summary>
        internal override string Identifier
        {
            get { return "SOLID"; }
        }

        #endregion

        #region " Operations "

        /// <summary>
        /// Selects all solids in the collection.
        /// </summary>
        /// <param name="emptySelectionFirst">If true it will empty the current PowerShape selection.</param>
        public void AddToSelection(bool emptySelectionFirst = false)
        {
            // The current selection is recorded, for restoration later if necessary
            List<PSEntity> previousSelection = new List<PSEntity>();
            if (emptySelectionFirst == false)
            {
                foreach (PSEntity selectedEntity in _powerSHAPE.ActiveModel.SelectedItems)
                {
                    previousSelection.Add(selectedEntity);
                }
            }

            // Add all surfaces
            _powerSHAPE.DoCommand("QUICKSELECTSOLIDONLY");

            // If required, the prevously selected items are restored to the selection
            foreach (PSEntity selectedEntity in previousSelection)
            {
                selectedEntity.AddToSelection();
            }
        }

        /// <summary>
        /// Creates an extruded solid from wireframe.
        /// </summary>
        /// <param name="wireframe">The wireframe from which an extruded solid will be created.</param>
        /// <param name="length">The length of the extruded solid.</param>
        /// <param name="negLength">The negative length of the extruded solid.</param>
        /// <returns>The created solid.</returns>
        public PSSolidExtrusion CreateSolidExtrusionFromWireframe(PSWireframe wireframe, MM length, MM negLength)
        {
            // Add wireframe to selection
            _powerSHAPE.ActiveModel.ClearCreatedItems();
            wireframe.AddToSelection(true);

            // Extrude to make solid
            _powerSHAPE.DoCommand("CREATE SOLID EXTRUSION");

            // Get created solid
            PSSolidExtrusion createdSolid = null;
            try
            {
                createdSolid = (PSSolidExtrusion) _powerSHAPE.ActiveModel.CreatedItems[0];
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to create solid extrusion");
            }

            // Edit length/negative length to be as required
            createdSolid.NegativeLength = negLength;
            createdSolid.Length = length;

            // Return created solid
            Add(createdSolid);
            return createdSolid;
        }

        /// <summary>
        /// Creates extruded solids from a list of wireframes.
        /// </summary>
        /// <param name="wireframes">The list of wireframes from which an extruded solid will be created.</param>
        /// <param name="length">The length of the extruded solid.</param>
        /// <param name="negLength">The negative length of the extruded solid.</param>
        /// <returns>The created solids.</returns>
        public List<PSSolidExtrusion> CreateSolidExtrusionsFromWireframes(List<PSWireframe> wireframes, MM length, MM negLength)
        {
            // Duplicate the wireframes as they get lost during this process
            List<PSWireframe> duplicateWires = new List<PSWireframe>();
            foreach (PSWireframe wire in wireframes)
            {
                duplicateWires.Add((PSWireframe) wire.Duplicate());
            }

            // Add wireframes to selection
            _powerSHAPE.ActiveModel.ClearCreatedItems();
            _powerSHAPE.ActiveModel.ClearSelectedItems();
            foreach (PSWireframe wireframe in duplicateWires)
            {
                wireframe.AddToSelection();
            }

            // Extrude to make solid
            _powerSHAPE.DoCommand("CREATE SOLID EXTRUSION");

            // Get created items (not all solids) and add the solids to the list and delete the others
            List<PSSolidExtrusion> createdSolids = new List<PSSolidExtrusion>();
            try
            {
                foreach (PSEntity createdItem in _powerSHAPE.ActiveModel.CreatedItems)
                {
                    if (createdItem is PSSolid)
                    {
                        createdSolids.Add((PSSolidExtrusion) createdItem);
                    }
                    else
                    {
                        createdItem.Delete();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to create solid extrusion");
            }

            // Return created solid
            foreach (PSSolidExtrusion createdSolid in createdSolids)
            {
                Add(createdSolid);

                // Avoid setting both lengths to 0 at the same time by setting the negative length first
                createdSolid.NegativeLength = negLength;
                createdSolid.Length = length;
            }
            return createdSolids;
        }

        /// <summary>
        /// Creates a solid from a list of surfaces.
        /// </summary>
        /// <param name="surfacesToConvert">Surfaces to be converted to a solid.</param>
        /// <returns>The solid created from the surfaces.</returns>
        public List<PSSolid> CreateSolidsFromSurfaces(List<PSSurface> surfacesToConvert)
        {
            // Add all surfaces
            _powerSHAPE.ActiveModel.ClearSelectedItems();
            foreach (PSSurface surfaceToConvert in surfacesToConvert)
            {
                surfaceToConvert.AddToSelection(false);
            }

            // Convert surfaces to solid
            _powerSHAPE.ActiveModel.ClearCreatedItems();
            _powerSHAPE.DoCommand("CREATE SOLID SURFACES", "NO");

            // If only one solid was created then it will be in the selected items else they will be in the created items
            List<PSSolid> createdSolids = new List<PSSolid>();

            // _powerSHAPE.ActiveModel.SelectedItems.ForEach(Sub(x) If (x.GetType() Is GetType(PSSolid)) Then createdSolids.Add(x))
            // _powerSHAPE.ActiveModel.CreatedItems.ForEach(Sub(x) If (x.GetType() Is GetType(PSSolid)) Then createdSolids.Add(x))
            _powerSHAPE.ActiveModel.SelectedItems.Where(x => x.GetType() == typeof(PSSolid)).ToList()
                       .ForEach(s => createdSolids.Add((PSSolid) s));
            _powerSHAPE.ActiveModel.CreatedItems.Where(x => x.GetType() == typeof(PSSolid)).ToList()
                       .ForEach(s => createdSolids.Add((PSSolid) s));

            // Check at least one solid was created
            if (createdSolids.Count == 0)
            {
                throw new ApplicationException("No solid created");
            }

            foreach (PSSolid createdSolid in createdSolids)
            {
                Add(createdSolid);
            }

            foreach (PSSurface convertedSurface in surfacesToConvert)
            {
                _powerSHAPE.ActiveModel.Surfaces.Remove(convertedSurface);
            }

            // If the solid was a version 8 solid then the act of removing the surface from the collection above causes
            // the solid to be unselected so add it back to the selection
            foreach (PSSolid createdSolid in createdSolids)
            {
                createdSolid.AddToSelection();
            }

            // Return new solid
            return createdSolids;
        }

        /// <summary>
        /// Creates a solid from a mesh.
        /// </summary>
        /// <param name="meshToConvert">The mesh to be converted to a solid.</param>
        /// <returns>The solid created from the mesh.</returns>
        public PSSolid CreateSolidFromMesh(PSMesh meshToConvert)
        {
            PSSolid createdSolid = new PSSolid(_powerSHAPE, meshToConvert);
            Add(createdSolid);

            // Return new solid
            return createdSolid;
        }

        /// <summary>
        /// Creates a new solid sphere at the specified origin and radius.
        /// </summary>
        /// <param name="origin">Origin at which to create the sphere.</param>
        /// <param name="radius">Radius of the sphere.</param>
        /// <returns>The created sphere.</returns>
        public PSSolidSphere CreateSphere(Point origin, MM radius)
        {
            PSSolidSphere sphere = new PSSolidSphere(_powerSHAPE, origin);
            sphere.Radius = radius;
            Add(sphere);
            return sphere;
        }

        /// <summary>
        /// Creates a new solid cylinder at the specified origin, radius and length.
        /// </summary>
        /// <param name="origin">Origin at which to create the cylinder.</param>
        /// <param name="radius">Radius of the cylinder.</param>
        /// <param name="length">Length of the cylinder.</param>
        /// <returns>The created cylinder.</returns>
        public PSSolidCylinder CreateCylinder(Point origin, MM radius, MM length)
        {
            PSSolidCylinder cylinder = new PSSolidCylinder(_powerSHAPE, origin);
            cylinder.Radius = radius;
            cylinder.Length = length;
            Add(cylinder);
            return cylinder;
        }

        /// <summary>
        /// Creates a new solid block at the specified origin, with the specified height, width, length
        /// and draft angle for all walls.
        /// </summary>
        /// <param name="origin">Origin at which to create the block.</param>
        /// <param name="width">Width of the block.</param>
        /// <param name="length">Length of the block.</param>
        /// <param name="height">Height of the block.</param>
        /// <param name="draftAngle">Draft angle to use for all walls.</param>
        /// <returns>The created block.</returns>
        public PSSolidBlock CreateBlock(Point origin, MM width, MM length, MM height, Degree draftAngle)
        {
            PSSolidBlock block = new PSSolidBlock(_powerSHAPE, origin);
            block.Width = width;
            block.Length = length;
            block.Height = height;
            block.DraftAngle1 = draftAngle;
            block.DraftAngle2 = draftAngle;
            block.DraftAngle3 = draftAngle;
            block.DraftAngle4 = draftAngle;
            Add(block);
            return block;
        }

        /// <summary>
        /// Creates a solid from a drive curve and one or more sections.
        /// </summary>
        /// <param name="driveCurve">The drive curve for the solid.</param>
        /// <param name="sections">The sections through which to drive the solid.</param>
        public PSSolid CreateSolidFromDriveAndSections(PSGenericCurve driveCurve, List<PSGenericCurve> sections)
        {
            // Create the solid
            PSSolid newSolid = new PSSolid(_powerSHAPE, driveCurve, sections);
            Add(newSolid);
            return newSolid;
        }

        /// <summary>
        /// Creates a new solid cone at the specified origin.
        /// </summary>
        /// <param name="origin">Origin at which to create the cone.</param>
        /// <returns>The created cone.</returns>
        public PSSolidCone CreateCone(Point origin)
        {
            PSSolidCone cone = new PSSolidCone(_powerSHAPE, origin);
            Add(cone);
            return cone;
        }

        /// <summary>
        /// Creates a new solid Torus at the specified origin.
        /// </summary>
        /// <param name="origin">Origin at which to create the Torus.</param>
        /// <returns>The created Torus.</returns>
        public PSSolidTorus CreateTorus(Point origin)
        {
            PSSolidTorus torus = new PSSolidTorus(_powerSHAPE, origin);
            Add(torus);
            return torus;
        }

        /// <summary>
        /// Creates a new solid spring at the specified origin.
        /// </summary>
        /// <param name="origin">Origin at which to create the spring.</param>
        /// <returns>The created spring.</returns>
        public PSSolidSpring CreateSpring(Point origin)
        {
            PSSolidSpring spring = new PSSolidSpring(_powerSHAPE, origin);
            Add(spring);
            return spring;
        }

        #endregion
    }
}