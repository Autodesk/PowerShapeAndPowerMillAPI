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
    /// Captures the collection of CompCurves in a Project
    /// </summary>
    public class PSCompCurvesCollection : PSGenericCurvesCollection<PSCompCurve>
    {
        #region " Constructors "

        /// <summary>
        /// This constructor is required as collection inherits from EntitiesCollection
        /// </summary>
        internal PSCompCurvesCollection(PSAutomation powerSHAPE) : base(powerSHAPE)
        {
        }

        #endregion

        #region " Properties "

        /// <summary>
        /// Gets the identifier used by PowerSHAPE when selecting a Composite Curve
        /// </summary>
        internal override string Identifier
        {
            get { return "COMPOSITECURVE"; }
        }

        #endregion

        #region " Operations "

        /// <summary>
        /// Creates a new compcurve through the specified points
        /// </summary>
        /// <param name="curveType">CurveTypes enum value for curev type</param>
        /// <param name="points">Optional array of points or singular point for curve to go through</param>
        /// <returns>A curve created by the operation</returns>
        public PSCompCurve CreateCompCurveThroughPoints(CurveTypes curveType, Geometry.Point[] points)
        {
            PSCurve curve = _powerSHAPE.ActiveModel.Curves.CreateCurveThroughPoints(curveType, points);
            PSCompCurve compCurve = CreateCompCurveFromWireframe(new PSWireframe[] {curve});
            curve.Delete();
            return compCurve;
        }

        /// <summary>
        /// Creates a new compcurve from Wireframe
        /// </summary>
        /// <param name="wireframe">The wireframe to turn into a CompCurve (arc, compcurve, curve or line)</param>
        /// <returns>The CompCurve created from the wireframe</returns>
        public PSCompCurve CreateCompCurveFromWireframe(PSWireframe[] wireframe)
        {
            // Loop through wireframe
            List<PSWireframe> duplicatedWireframe = new List<PSWireframe>();
            foreach (PSWireframe entity in wireframe)
            {
                // Duplicate the arc as it will delete the original
                PSWireframe newWireframe = (PSWireframe) entity.Duplicate();
                duplicatedWireframe.Add(newWireframe);
            }

            // Add all to wireframe selection
            _powerSHAPE.ActiveModel.ClearSelectedItems();
            foreach (PSWireframe entity in wireframe)
            {
                entity.AddToSelection(false);
            }

            _powerSHAPE.DoCommand("CONVERT COMPCURVE");

            //Add the new CompCurve to the collection of CompCurves
            PSCompCurve compCurve = null;
            compCurve = (PSCompCurve) _powerSHAPE.ActiveModel.CreatedItems[0];
            Add(compCurve);

            // Delete the duplicate arc from any lists
            foreach (PSWireframe entity in duplicatedWireframe)
            {
                entity.Delete();
            }

            return compCurve;
        }

        /// <summary>
        /// Creates CompCurves from an intersection between two sets of entities
        /// </summary>
        /// <param name="firstEntities">The first set of entities to intersect</param>
        /// <param name="secondEntities">The second set of entities to intersect</param>
        /// <returns>A list of the created CompCurves</returns>
        private List<PSCompCurve> CreateCompCurvesFromIntersectionOfTwoSetsOfEntities(
            List<PSEntity> firstEntities,
            List<PSEntity> secondEntities)
        {
            // Add first entities to selection
            _powerSHAPE.ActiveModel.ClearSelectedItems();
            foreach (PSEntity firstEntity in firstEntities)
            {
                firstEntity.AddToSelection(false);
            }

            // Start create intersection curves operation
            _powerSHAPE.DoCommand("CREATE CURVE INTERSECT");

            // Add second entities to selection
            foreach (PSEntity secondEntity in secondEntities)
            {
                secondEntity.AddToSelection(false);
            }

            // Finish create intersection curves operation
            _powerSHAPE.DoCommand("ACCEPT");

            // Add created curves to collection
            List<PSCompCurve> createdCurves = new List<PSCompCurve>();
            if (_powerSHAPE.ActiveModel.CreatedItems.Count == 0)
            {
                return createdCurves;
            }
            foreach (PSCompCurve createdCurve in _powerSHAPE.ActiveModel.CreatedItems)
            {
                createdCurves.Add(createdCurve);
                _powerSHAPE.ActiveModel.CompCurves.Add(createdCurve);
            }

            // Return created curves
            return createdCurves;
        }

        /// <summary>
        /// Creates CompCurves from an intersection between two sets of surfaces
        /// </summary>
        /// <param name="firstSurfaces">The first set of surfaces to intersect</param>
        /// <param name="secondSurfaces">The second set of surfaces to intersect</param>
        /// <returns>A list of the created CompCurves</returns>
        public List<PSCompCurve> CreateCompCurvesFromIntersectionOfTwoSetsOfSurfaces(
            List<PSSurface> firstSurfaces,
            List<PSSurface> secondSurfaces)
        {
            // Create lists of entites
            List<PSEntity> firstEntities = new List<PSEntity>();
            List<PSEntity> secondEntities = new List<PSEntity>();
            foreach (PSSurface firstSurface in firstSurfaces)
            {
                PSEntity firstEntity = firstSurface;
                firstEntities.Add(firstEntity);
            }
            foreach (PSSurface secondSurface in secondSurfaces)
            {
                PSEntity secondEntity = secondSurface;
                secondEntities.Add(secondEntity);
            }

            // Carry out operation
            return CreateCompCurvesFromIntersectionOfTwoSetsOfEntities(firstEntities, secondEntities);
        }

        /// <summary>
        /// Creates CompCurves from an intersection between two surfaces
        /// </summary>
        /// <param name="firstSurface">The first surface to intersect</param>
        /// <param name="secondSurface">The second surface to intersect</param>
        /// <returns>A list of the created CompCurves</returns>
        public List<PSCompCurve> CreateCompCurvesFromIntersectionOfTwoSurfaces(PSSurface firstSurface, PSSurface secondSurface)
        {
            // Create lists from individual surfaces
            List<PSEntity> firstSurfaceList = new List<PSEntity>();
            List<PSEntity> secondSurfaceList = new List<PSEntity>();
            firstSurfaceList.Add(firstSurface);
            secondSurfaceList.Add(secondSurface);

            // Carry out operation
            return CreateCompCurvesFromIntersectionOfTwoSetsOfEntities(firstSurfaceList, secondSurfaceList);
        }

        /// <summary>
        /// Creates CompCurves from an intersection between two solids
        /// </summary>
        /// <param name="firstSolid">The first solid to intersect</param>
        /// <param name="secondSolid">The second solid to intersect</param>
        /// <returns>A list of the created CompCurves</returns>
        public List<PSCompCurve> CreateCompCurvesFromIntersectionOfTwoSolids(PSSolid firstSolid, PSSolid secondSolid)
        {
            // Create lists from individual surfaces
            List<PSEntity> firstSolidList = new List<PSEntity>();
            List<PSEntity> secondSolidList = new List<PSEntity>();
            firstSolidList.Add(firstSolid);
            secondSolidList.Add(secondSolid);

            // Carry out operation
            return CreateCompCurvesFromIntersectionOfTwoSetsOfEntities(firstSolidList, secondSolidList);
        }

        /// <summary>
        /// Creates CompCurves from an intersection between a surface and a solid
        /// </summary>
        /// <param name="firstSurface">The surface to intersect</param>
        /// <param name="secondSolid">The solid to intersect</param>
        /// <returns>A list of the created CompCurves</returns>
        public List<PSCompCurve> CreateCompCurvesFromIntersectionOfSurfaceAndSolid(PSSurface firstSurface, PSSolid secondSolid)
        {
            // Create lists from individual surfaces
            List<PSEntity> firstList = new List<PSEntity>();
            List<PSEntity> secondList = new List<PSEntity>();
            firstList.Add(firstSurface);
            secondList.Add(secondSolid);

            // Carry out operation
            return CreateCompCurvesFromIntersectionOfTwoSetsOfEntities(firstList, secondList);
        }

        /// <summary>
        /// Creates CompCurves from an intersection between a surface and a mesh.
        /// </summary>
        /// <param name="surface">The surface to intersect.</param>
        /// <param name="mesh">The mesh to intersect.</param>
        /// <returns>The list of the created CompCurves.</returns>
        /// <remarks></remarks>
        public List<PSCompCurve> CreateCompCurvesFromIntersectionOfSurfaceAndMesh(PSSurface surface, PSMesh mesh)
        {
            // Create lists from individual surfaces
            List<PSEntity> firstList = new List<PSEntity>();
            List<PSEntity> secondList = new List<PSEntity>();
            firstList.Add(surface);
            secondList.Add(mesh);

            // Carry out operation
            return CreateCompCurvesFromIntersectionOfTwoSetsOfEntities(firstList, secondList);
        }

        /// <summary>
        /// Creates an Oblique curve against either a mesh, solid or surface.
        /// </summary>
        /// <param name="entityToOblique">The entity to be cut by the oblique curve.</param>
        /// <param name="UsePlane">Sets the principal plane.</param>
        /// <param name="position">Distance along plane to drop curve on.</param>
        /// <exception cref="ApplicationException">No curves were created from intersection operation.</exception>
        /// <exception cref="ApplicationException">Entity must be Mesh, Surface or Solid.</exception>
        /// <returns>List of CompCurves.</returns>
        public List<PSCompCurve> CreateObliqueCurve(PSEntity entityToOblique, Planes UsePlane, Geometry.MM position)
        {
            List<PSCompCurve> createdCurves = new List<PSCompCurve>();

            if (entityToOblique is PSMesh || entityToOblique is PSSolid || entityToOblique is PSSurface)
            {
                // Set the principal plane
                _powerSHAPE.SetActivePlane(UsePlane);
                entityToOblique.AddToSelection(true);
                _powerSHAPE.DoCommand("create curve Oblique", "DISTANCE " + position.Value, "ACCEPT");

                // Add created curves to collection
                if (_powerSHAPE.ActiveModel.CreatedItems.Count == 0)
                {
                    throw new ApplicationException("No curves were created from intersection operation");
                }
                foreach (PSCompCurve createdCurve in _powerSHAPE.ActiveModel.CreatedItems)
                {
                    createdCurves.Add(createdCurve);
                    _powerSHAPE.ActiveModel.CompCurves.Add(createdCurve);
                }
            }
            else
            {
                throw new ApplicationException("Entity must be Mesh, Surface or Solid");
            }

            // Return created curves
            return createdCurves;
        }

        /// <summary>
        /// Converts the specified PSAnnotation into a series of PSCompCurves
        /// </summary>
        /// <param name="annotationToConvert">The annotation to convert</param>
        /// <returns>A list of PSCompCurves</returns>
        /// <remarks>The original PSAnnotation will no longer exist after this operation</remarks>
        public List<PSCompCurve> CreateCompCurvesFromAnnotation(PSAnnotation annotationToConvert)
        {
            List<PSAnnotation> annotations = new List<PSAnnotation>();
            annotations.Add(annotationToConvert);
            return CreateCompCurvesFromAnnotations(annotations);
        }

        /// <summary>
        /// Converts the specified PSAnnotations into a series of PSCompCurves
        /// </summary>
        /// <param name="annotationsToConvert">The annotations to convert</param>
        /// <returns>A list of PSCompCurves</returns>
        /// <remarks>The original PSAnnotations will no longer exist after this operation</remarks>
        public List<PSCompCurve> CreateCompCurvesFromAnnotations(List<PSAnnotation> annotationsToConvert)
        {
            _powerSHAPE.ActiveModel.ClearSelectedItems();
            _powerSHAPE.ActiveModel.ClearCreatedItems();
            foreach (PSAnnotation annotation in annotationsToConvert)
            {
                annotation.AddToSelection();
            }

            _powerSHAPE.DoCommand("CONVERT WIREFRAME");

            List<PSCompCurve> createdCompCurves = new List<PSCompCurve>();
            foreach (PSCompCurve createdItem in _powerSHAPE.ActiveModel.CreatedItems)
            {
                _powerSHAPE.ActiveModel.Add(createdItem);
                createdCompCurves.Add(createdItem);
            }

            foreach (PSAnnotation annotation in annotationsToConvert)
            {
                _powerSHAPE.ActiveModel.Annotations.Remove(annotation);
            }
            return createdCompCurves;
        }

        #endregion
    }
}