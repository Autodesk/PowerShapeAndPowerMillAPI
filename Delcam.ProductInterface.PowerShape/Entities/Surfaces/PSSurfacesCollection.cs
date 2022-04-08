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
using Autodesk.Extensions;

namespace Autodesk.ProductInterface.PowerSHAPE
{
    /// <summary>
    /// Captures the collection of Surfaces in a Project.
    /// </summary>
    public class PSSurfacesCollection : PSEntitiesCollection<PSSurface>
    {
        #region " Constructors "

        /// <summary>
        /// New surfaces collection.
        /// </summary>
        /// <param name="powerSHAPE">The base instance to interact with PowerShape.</param>
        internal PSSurfacesCollection(PSAutomation powerSHAPE) : base(powerSHAPE)
        {
        }

        #endregion

        #region " Properties "

        /// <summary>
        /// Gets the identifer used by PowerSHAPE when selecting a Surface.
        /// </summary>
        internal override string Identifier
        {
            get { return "SURFACE"; }
        }

        #endregion

        #region " Operations "

        #region " Creating Surfaces "

        /// <summary>
        /// Creates a new Surface using PowerSHAPE's automatic surfacing methods.  Any necessary wireframe must already be selected.
        /// </summary>
        /// <param name="automaticSurfacingMethod">The method to use to create the surface.</param>
        /// <param name="surfaceType">The type of the new surface NURBS or a PowerSurface.</param>
        /// <param name="advancedSurfaceOptions">The advanced surface option that can be used when creating the surface.</param>
        /// <returns>The new surface.</returns>
        public PSSurface CreateSurface(
            AutomaticSurfacingMethods automaticSurfacingMethod,
            AutomaticSurfaceTypes surfaceType,
            SurfaceAdvancedOptions advancedSurfaceOptions = null)
        {
            PSSurface newSurface =
                (PSSurface) PSSurface.CreateSmartSurface(_powerSHAPE,
                                                         automaticSurfacingMethod,
                                                         surfaceType,
                                                         advancedSurfaceOptions);
            Add(newSurface);
            return newSurface;
        }

        /// <summary>
        /// Creates a fill in Surface from the specified Curve or Composite Curve.
        /// </summary>
        /// <param name="genericCurve">The curve or composite curve from which to create the fill in surface.</param>
        /// <returns>The new surface.</returns>
        public PSSurface CreateFillInSurface(PSGenericCurve genericCurve)
        {
            genericCurve.AddToSelection(true);
            var newSurface = PSSurface.CreateSmartSurface(_powerSHAPE, AutomaticSurfacingMethods.Fill, AutomaticSurfaceTypes.PowerSurface);
            Add(newSurface);
            return newSurface;
        }

        /// <summary>
        /// Creates a surface from triangles using the specified parameters
        /// </summary>
        /// <param name="mesh">The mesh from which to create the surface</param>
        /// <param name="pointsProportion">The points proportion. Defaults to 25.0</param>
        /// <param name="orientationAngle">The orientation angle of the surface.  Defaults to 0.0</param>
        /// <param name="numberOfPatches">The number of surface patches.  Defaults to 10</param>
        /// <returns>The created surface.</returns>
        /// <exception cref="Exception">Failed to create surface from Mesh.</exception>
        public PSSurface CreateSurfaceFromTriangles(
            PSMesh mesh,
            List<PSGenericCurve> boundariesAndIslands = null,
            double pointsProportion = 25.0,
            double orientationAngle = 0.0,
            int numberOfPatches = 10)
        {
            mesh.AddToSelection(true);
            if (boundariesAndIslands != null)
            {
                boundariesAndIslands.ForEach(x => x.AddToSelection());
            }

            _powerSHAPE.DoCommand("CREATE SURFACE AUTOSURF",
                                  "METHOD FROMTRIANGLES",
                                  "ADVANCED",
                                  "SETPOINTS " + pointsProportion.ToString("0.######"),
                                  "SETPATCHES " + numberOfPatches,
                                  "SETANGLE " + orientationAngle.ToString("0.######"),
                                  "APPLY");
            var createdItems = _powerSHAPE.ActiveModel.CreatedItems;
            _powerSHAPE.DoCommand("ACCEPT", "ACCEPT");
            if (createdItems.Count != 1)
            {
                throw new Exception("Failed to create surface from Mesh");
            }

            return (PSSurface) createdItems[0];
        }

        /// <summary>
        /// Creates an extruded surface from wireframe.
        /// </summary>
        /// <param name="wireframe">The wireframe from which an extruded surface will be created.</param>
        /// <param name="length">The length of the extruded surface.</param>
        /// <param name="negLength">The negative length of the extruded surface.</param>
        /// <returns>The new surface.</returns>
        public PSSurface CreateExtrudedSurface(PSWireframe wireframe, Geometry.MM length, Geometry.MM negLength)
        {
            // Add wireframe to selection
            _powerSHAPE.ActiveModel.ClearCreatedItems();
            wireframe.AddToSelection(true);

            // Extrude to make solid
            _powerSHAPE.DoCommand("CREATE SURFACE EXTRUSION");

            // Get created surface
            PSSurface createdSurface = null;
            try
            {
                createdSurface = (PSSurface) _powerSHAPE.ActiveModel.CreatedItems[0];
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to create surface extrusion");
            }

            // Edit length/negative length to be as required. Set NEGLENGTH initially in case the desired LENGTH is 0, as can't have
            // two lengths of 0 within the form
            _powerSHAPE.DoCommand("MODIFY", "NEGLENGTH " + negLength.ToString("0.######"), "LENGTH " + length.ToString("0.######"), "ACCEPT");

            // Return created solid
            Add(createdSurface);
            return createdSurface;
        }

        /// <summary>
        /// Creates extruded surfaces from a list of wireframes.
        /// </summary>
        /// <param name="wireframes">The list of wireframes from which an extruded surface will be created.</param>
        /// <param name="length">The length of the extruded surface.</param>
        /// <param name="negLength">The negative length of the extruded surface.</param>
        /// <returns>The new surface.</returns>
        public List<PSSurface> CreateExtrudedSurfaces(
            IEnumerable<PSWireframe> wireframes,
            Geometry.MM length,
            Geometry.MM negLength)
        {
            // Add wireframes to selection
            _powerSHAPE.ActiveModel.ClearCreatedItems();
            _powerSHAPE.ActiveModel.ClearSelectedItems();
            foreach (PSWireframe wireframe in wireframes)
            {
                wireframe.AddToSelection();
            }

            // Extrude to make solid
            _powerSHAPE.DoCommand("CREATE SURFACE EXTRUSION",
                                  "LENGTH " + length.ToString("0.######"),
                                  "NEGLENGTH " + negLength.ToString("0.######"),
                                  "ACCEPT");

            // Get created solid
            List<PSSurface> createdSurfaces = new List<PSSurface>();
            try
            {
                foreach (PSSurface createdSurface in _powerSHAPE.ActiveModel.CreatedItems)
                {
                    createdSurfaces.Add(createdSurface);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to create surface extrusion");
            }

            // Return created surfaces
            foreach (PSSurface createdSurface in createdSurfaces)
            {
                Add(createdSurface);
            }
            return createdSurfaces;
        }

        /// <summary>
        /// Converts a solid into surfaces.
        /// </summary>
        /// <param name="solidToConvert">The solid to be converted into surfaces.</param>
        /// <returns>The list of the surfaces created from the solid.</returns>
        public List<PSSurface> CreateSurfacesFromSolid(PSSolid solidToConvert)
        {
            _powerSHAPE.ActiveModel.ClearCreatedItems();

            // Add the surface to the selection
            solidToConvert.AddToSelection(true);

            // Convert to surfaces
            _powerSHAPE.DoCommand("CONVERT SOLID");

            // Get new surfaces
            List<PSSurface> newSurfaces = new List<PSSurface>();

            //_powerSHAPE.ActiveModel.SelectedItems.ForEach(Sub(x) If (x.GetType() Is GetType(PSSurface)) Then newSurfaces.Add(x))
            //_powerSHAPE.ActiveModel.CreatedItems.ForEach(Sub(x) If (x.GetType() Is GetType(PSSurface)) Then newSurfaces.Add(x))

            newSurfaces.AddRange(_powerSHAPE.ActiveModel
                                            .SelectedItems
                                            .Where(i => i.GetType() == typeof(PSSurface))
                                            .Cast<PSSurface>());

            newSurfaces.AddRange(_powerSHAPE.ActiveModel
                                            .CreatedItems.Where(x => x.GetType() == typeof(PSSurface))
                                            .Cast<PSSurface>());

            // Check that surfaces have been created
            if (newSurfaces.Count == 0)
            {
                throw new ApplicationException("Solid not converted to surfaces correctly");
            }

            foreach (PSSurface newSurface in newSurfaces)
            {
                Add(newSurface);
            }

            // Remove now converted solid from PowerSHAPE collection
            _powerSHAPE.ActiveModel.Solids.Remove(solidToConvert);

            // Return new surfaces
            return newSurfaces;
        }

        /// <summary>
        /// Breaks a surface at the indicated surface curve, so creating a new surface.
        /// </summary>
        /// <param name="breakCurve">The surface curve at which to break the surface.</param>
        /// <returns>The surface created by the operation.</returns>
        /// <remarks>The original surface is derived from the breakCurve parameter.</remarks>
        public PSSurface CreateSurfaceFromBreak(PSSurfaceCurve breakCurve)
        {
            // Add the surface curve to the selection
            breakCurve.AddToSelection(true);

            // Break the surface
            _powerSHAPE.DoCommand("BREAKSURF");

            // Get the new surface
            breakCurve.ParentSurface.RemoveFromSelection();
            PSSurface newSurface = (PSSurface) _powerSHAPE.ActiveModel.SelectedItems[0];
            _powerSHAPE.ActiveModel.Surfaces.Add(newSurface);

            return newSurface;
        }

        /// <summary>
        /// Creates fillet surfaces from two individual surfaces.
        /// </summary>
        /// <param name="firstSurface">The primary surface to fillet.</param>
        /// <param name="secondSurface">The secondary surface to fillet.</param>
        /// <param name="filletRadius">The radius of the fillet.</param>
        /// <param name="filletType">Whether the fillet is concave or convex.</param>
        /// <param name="filletTrim">Whether the creation of the fillet trims surrounding surfaces.</param>
        /// <param name="filletRoute">whether the fillet follows all possible routes.</param>
        /// <param name="filletCrease">Whether the fillet is created along creases.</param>
        /// <param name="filletCorners">Whether any corners are Roll, Mitre or Round.</param>
        /// <returns>The list of created surfaces.</returns>
        public List<PSSurface> CreateFilletSurfaces(
            PSSurface firstSurface,
            PSSurface secondSurface,
            double filletRadius,
            FilletTypes filletType,
            FilletTrimOptions filletTrim,
            FilletRouteOptions filletRoute,
            FilletCreaseOptions filletCrease,
            FilletCornerTypes filletCorners)
        {
            // Do filleting operation
            List<PSSurface> firstListOfSurfaces = new List<PSSurface>();
            List<PSSurface> secondListOfSurfaces = new List<PSSurface>();
            firstListOfSurfaces.Add(firstSurface);
            secondListOfSurfaces.Add(secondSurface);
            return CreateFilletSurfaces(firstListOfSurfaces,
                                        secondListOfSurfaces,
                                        filletRadius,
                                        filletType,
                                        filletTrim,
                                        filletRoute,
                                        filletCrease,
                                        filletCorners);
        }

        /// <summary>
        /// Creates a fillet from the two lists of surfaces.
        /// </summary>
        /// <param name="firstSurfaces">The primary surfaces to fillet.</param>
        /// <param name="secondSurfaces">The secondary surfaces to fillet.</param>
        /// <param name="filletRadius">The radius of the fillet.</param>
        /// <param name="filletType">Whether the fillet is concave or convex.</param>
        /// <param name="filletTrim">Whether the creation of the fillet trims surrounding surfaces.</param>
        /// <param name="filletRoute">whether the fillet follows all possible routes.</param>
        /// <param name="filletCrease">Whether the fillet is created along creases.</param>
        /// <param name="filletCorners">Whether any corners are Roll, Mitre or Round.</param>
        /// <returns>The list of created surfaces.</returns>
        public List<PSSurface> CreateFilletSurfaces(
            List<PSSurface> firstSurfaces,
            List<PSSurface> secondSurfaces,
            double filletRadius,
            FilletTypes filletType,
            FilletTrimOptions filletTrim,
            FilletRouteOptions filletRoute,
            FilletCreaseOptions filletCrease,
            FilletCornerTypes filletCorners)
        {
            // Add first surfaces to the selection
            firstSurfaces[0].AddToSelection(true);
            foreach (PSSurface surface in firstSurfaces)
            {
                surface.AddToSelection(false);
            }

            // Setup fillet options
            _powerSHAPE.DoCommand("CREATE SURFACE FILLET");
            _powerSHAPE.DoCommand("RADIUS " + filletRadius);
            _powerSHAPE.DoCommand(filletType.ToString());
            _powerSHAPE.DoCommand(filletTrim.ToString());
            _powerSHAPE.DoCommand(filletRoute.ToString());
            _powerSHAPE.DoCommand(filletCrease.ToString());
            _powerSHAPE.DoCommand("CORNER " + filletCorners);

            // Add second surface(s)
            foreach (PSSurface surface in secondSurfaces)
            {
                surface.AddToSelection(false);
            }

            // Create fillet
            _powerSHAPE.DoCommand("ACCEPT", "ACCEPT");

            // Return any created surfaces
            List<PSSurface> surfacesToReturn = new List<PSSurface>();
            if (_powerSHAPE.ActiveModel.CreatedItems.Count != 0)
            {
                foreach (PSSurface createdSurface in _powerSHAPE.ActiveModel.SelectedItems)
                {
                    surfacesToReturn.Add(createdSurface);
                    Add(createdSurface);
                }
            }
            else
            {
                throw new ApplicationException("Filleting of surfaces failed");
            }

            // Return created surfaces
            return surfacesToReturn;
        }

        #region " Creating Primitives "

        /// <summary>
        /// Creates a plane at a specified origin.
        /// </summary>
        /// <param name="origin">The origin at which to create the plane.</param>
        /// <param name="principalPlane">Sets the principal plane. Defines which plane is the principal plane of the workspace.</param>
        /// <returns>The created plane.</returns>
        /// <remarks></remarks>
        public PSSurfacePlane CreatePlane(Geometry.Point origin, Planes principalPlane)
        {
            PSSurfacePlane newPlane = new PSSurfacePlane(_powerSHAPE, origin, principalPlane);
            _powerSHAPE.ActiveModel.Surfaces.Add(newPlane);
            return newPlane;
        }

        /// <summary>
        /// Creates a plane at a specified origin and sets its length and width.
        /// </summary>
        /// <param name="origin">The origin at which to create the plane.</param>
        /// <param name="principalPlane">Sets the principal plane. Defines which plane is the principal plane of the workspace.</param>
        /// <param name="length">The desired length of the plane.</param>
        /// <param name="width">The desired width of the plane.</param>
        /// <returns>The created plane.</returns>
        /// <remarks></remarks>
        public PSSurfacePlane CreatePlaneWithDimensions(
            Geometry.Point origin,
            Planes principalPlane,
            Geometry.MM length,
            Geometry.MM width)
        {
            PSSurfacePlane newPlane = new PSSurfacePlane(_powerSHAPE, origin, principalPlane, length, width);
            _powerSHAPE.ActiveModel.Surfaces.Add(newPlane);
            return newPlane;
        }

        /// <summary>
        /// Creates a revolved surface from a list of wireframe.
        /// </summary>
        /// <param name="rotationAxis">The axis around which to revolve the surface.</param>
        /// <param name="angleOfRevolution">The angle round which to revolve.</param>
        /// <param name="wireframe">The wireframe representing the profile of the surface.</param>
        /// <returns>A revolved surface.</returns>
        public PSSurfaceRevolution CreateRevolvedSurface(
            Axes rotationAxis,
            Geometry.Degree angleOfRevolution,
            PSWireframe wireframe)
        {
            // Set the principal plane
            _powerSHAPE.SetActivePlane(rotationAxis.AxisToPlane());

            // Select the wireframe
            wireframe.AddToSelection(true);

            // Create revolved surface
            _powerSHAPE.DoCommand("CREATE SURFACE REVOLUTION");

            // Check that surface has been created
            PSSurfaceRevolution createdSurface = null;
            if (_powerSHAPE.ActiveModel.SelectedItems.Count == 0)
            {
                throw new ApplicationException("No surface was created from the revolution");
            }
            createdSurface = (PSSurfaceRevolution) _powerSHAPE.ActiveModel.SelectedItems[0];

            // Change the revolution angle if required
            if (angleOfRevolution != 360)
            {
                createdSurface.Angle = angleOfRevolution;
            }

            return createdSurface;
        }

        #endregion

        #endregion

        /// <summary>
        /// Adds all Surfaces to the current selection.
        /// </summary>
        /// <param name="emptySelectionFirst">If true it will empty the PowerShape selection first.</param>
        public override void AddToSelection(bool emptySelectionFirst = false)
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
            _powerSHAPE.DoCommand("QUICKSELECTSURFONLY");

            // If required, the prevously selected items are restored to the selection
            foreach (PSEntity selectedEntity in previousSelection)
            {
                selectedEntity.AddToSelection();
            }
        }

        /// <summary>
        /// Creates a new surface sphere at the specified origin and radius.
        /// </summary>
        /// <param name="origin">Origin at which to create the sphere.</param>
        /// <param name="radius">Radius of the sphere.</param>
        /// <returns>The created sphere.</returns>
        public PSSurfaceSphere CreateSphere(Geometry.Point origin, Geometry.MM radius)
        {
            PSSurfaceSphere sphere = new PSSurfaceSphere(_powerSHAPE, origin);
            sphere.Radius = radius;
            Add(sphere);
            return sphere;
        }

        /// <summary>
        /// Creates a new surface cylinder at the specified origin, radius and length.
        /// </summary>
        /// <param name="origin">The origin at which to create the cylinder.</param>
        /// <param name="radius">The radius of the cylinder.</param>
        /// <param name="length">The length of the cylinder.</param>
        /// <returns>The created cylinder.</returns>
        public PSSurfaceCylinder CreateCylinder(Geometry.Point origin, Geometry.MM radius, Geometry.MM length)
        {
            PSSurfaceCylinder cylinder = new PSSurfaceCylinder(_powerSHAPE, origin);
            cylinder.Radius = radius;
            cylinder.Length = length;
            Add(cylinder);
            return cylinder;
        }

        /// <summary>
        /// Creates a new surface block at the specified origin, with the specified height, width, length
        /// and draft angle for all walls.
        /// </summary>
        /// <param name="origin">The origin at which to create the block.</param>
        /// <param name="width">The width of the block.</param>
        /// <param name="length">The length of the block.</param>
        /// <param name="height">The height of the block.</param>
        /// <param name="draftAngle">The draft angle to use for all walls.</param>
        /// <returns>The created block.</returns>
        public PSSurfaceBlock CreateBlock(
            Geometry.Point origin,
            Geometry.MM width,
            Geometry.MM length,
            Geometry.MM height,
            Geometry.Degree draftAngle)
        {
            PSSurfaceBlock block = new PSSurfaceBlock(_powerSHAPE, origin);
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
        /// Creates a cone at a specified origin.
        /// </summary>
        /// <param name="origin">The origin at which to create the cone.</param>
        /// <returns>The created cone.</returns>
        /// <remarks></remarks>
        public PSSurfaceCone CreateCone(Geometry.Point origin)
        {
            PSSurfaceCone newCone = new PSSurfaceCone(_powerSHAPE, origin);
            _powerSHAPE.ActiveModel.Surfaces.Add(newCone);
            return newCone;
        }

        /// <summary>
        /// Creates a Torus at a specified origin.
        /// </summary>
        /// <param name="origin">The origin at which to create the Torus.</param>
        /// <returns>The created Torus.</returns>
        /// <remarks></remarks>
        public PSSurfaceTorus CreateTorus(Geometry.Point origin)
        {
            PSSurfaceTorus newTorus = new PSSurfaceTorus(_powerSHAPE, origin);
            _powerSHAPE.ActiveModel.Surfaces.Add(newTorus);
            return newTorus;
        }

        /// <summary>
        /// Creates a spring at a specified origin.
        /// </summary>
        /// <param name="origin">The origin at which to create the spring.</param>
        /// <returns>The created spring.</returns>
        /// <remarks></remarks>
        public PSSurfaceSpring CreateSpring(Geometry.Point origin)
        {
            PSSurfaceSpring newSpring = new PSSurfaceSpring(_powerSHAPE, origin);
            _powerSHAPE.ActiveModel.Surfaces.Add(newSpring);
            return newSpring;
        }

        #endregion
    }
}