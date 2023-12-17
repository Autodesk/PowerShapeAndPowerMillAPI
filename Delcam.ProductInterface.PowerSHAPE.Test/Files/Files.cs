// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Directory = Autodesk.FileSystem.Directory;

namespace Autodesk.ProductInterface.PowerSHAPETest
{
    static class TestFiles
    {
        private static readonly string FOLDER_NAME = "Files";

        public static readonly string DIRECTORY = SetSolutionFolder();

        // All files are DGK files unless otherwise stated

        #region Geometry Files

        #region Single Objects

        //public static readonly string SINGLE_ANNOTATION = FetchTestFile("Annotation.igs");
        public static readonly string SINGLE_ARC = FetchTestFile("Arc.dgk");
        public static readonly string SINGLE_COMPCURVE = FetchTestFile("CompCurve.dgk");
        public static readonly string SINGLE_CURVE = FetchTestFile("Curve.dgk");
        public static readonly string SINGLE_CURVE_G2 = FetchTestFile("CurveG2.dgk");
        public static readonly string SINGLE_CURVE_BSPLINE = FetchTestFile("CurveBSpline.dgk");
        public static readonly string SINGLE_MESH1 = FetchTestFile("Mesh1.dmt");
        public static readonly string SINGLE_MESH2 = FetchTestFile("Mesh2.dmt");
        public static readonly string SINGLE_MESH3 = FetchTestFile("Mesh3.dmt");
        public static readonly string SINGLE_LINE = FetchTestFile("Line.dgk");
        public static readonly string SINGLE_POINT = FetchTestFile("Point.dgk");
        public static readonly string SINGLE_SOLID = FetchTestFile("Solid.x_b");
        public static readonly string SINGLE_SOLID_DGK = FetchTestFile("Solid.dgk");
        public static readonly string SINGLE_SURFACE = FetchTestFile("Surface.dgk");
        public static readonly string SINGLE_SURFACE_WITH_BULGE = FetchTestFile("SurfaceWithBulge.dgk");
        public static readonly string SINGLE_SURFACE_DMT = FetchTestFile("Surface.dmt");
        public static readonly string SINGLE_SURFACE_SPINE = FetchTestFile("SurfaceSpine.dgk");
        public static readonly string SINGLE_SURFACEBLOCK = FetchTestFile("SurfaceBlock.dgk");
        public static readonly string SINGLE_SURFACECONE = FetchTestFile("SurfaceCone.dgk");
        public static readonly string SINGLE_SURFACECYLINDER = FetchTestFile("SurfaceCylinder.dgk");
        public static readonly string SINGLE_SURFACEEXTRUSION = FetchTestFile("SurfaceExtrusion.dgk");
        public static readonly string SINGLE_SURFACEPLANE = FetchTestFile("SurfacePlane.dgk");
        public static readonly string SINGLE_SURFACEREVOLUTION = FetchTestFile("SurfaceRevolution.dgk");
        public static readonly string SINGLE_SURFACESPHERE = FetchTestFile("SurfaceSphere.dgk");
        public static readonly string SINGLE_SURFACESPRING = FetchTestFile("SurfaceSpring.dgk");
        public static readonly string SINGLE_SURFACETORUS = FetchTestFile("SurfaceTorus.dgk");
        public static readonly string SINGLE_WORKPLANE1 = FetchTestFile("Workplane1.dgk");
        public static readonly string SINGLE_WORKPLANE2 = FetchTestFile("Workplane2.dgk");
        public static readonly string SINGLE_OBLIQUE_MESH1 = FetchTestFile("MeshOblique.dgk");
        public static readonly string SINGLE_MESH = FetchTestFile("SingleMesh.dmt");

        public static readonly string ARC_TO_OFFSET = FetchTestFile("ArcToOffset.dgk");
        public static readonly string CURVE_TO_OFFSET = FetchTestFile("CurveToOffset.dgk");
        public static readonly string COMPCURVE_TO_OFFSET = FetchTestFile("CompCurveToOffset.dgk");

        #endregion

        #region Multiple Objects

        public static readonly string TWO_ANNOTATIONS = FetchTestFile("TwoAnnotations.igs");
        public static readonly string THREE_ARCS = FetchTestFile("ThreeArcs.dgk");
        public static readonly string THREE_COMPCURVES = FetchTestFile("ThreeCompCurves.dgk");
        public static readonly string THREE_CURVES = FetchTestFile("ThreeCurves.dgk");
        public static readonly string THREE_MESHES_DMT = FetchTestFile("ThreeMeshes.dmt");
        public static readonly string THREE_MESHES_COMBINED = FetchTestFile("ThreeMeshesCombined.dmt");
        public static readonly string THREE_LINES = FetchTestFile("ThreeLines.dgk");
        public static readonly string THREE_POINTS = FetchTestFile("ThreePoints.dgk");
        public static readonly string THREE_SOLIDS = FetchTestFile("ThreeSolids.dgk");
        public static readonly string THREE_SURFACES = FetchTestFile("ThreeSurfaces.dgk");
        public static readonly string THREE_WORKPLANES = FetchTestFile("ThreeWorkplanes.dgk");
        public static readonly string TOUCHING_SURFACES = FetchTestFile("MultipleSurfacesForAppending.dgk");

        #endregion

        #region Objects for Operations

        public static readonly string FILLET_SURFACES = FetchTestFile("FilletSurfaces.dgk");
        public static readonly string COMPCURVE_TO_PROJECT = FetchTestFile("CompCurveToProject.dgk");
        public static readonly string COMPCURVES_TO_WRAP = FetchTestFile("CompCurvesToWrap.dgk");
        public static readonly string COMPCURVE_MULTIPLE_ITEMS = FetchTestFile("CompCurveMultipleItems.dgk");
        public static readonly string COMPCURVE_TO_FREE = FetchTestFile("CompCurveToFree.dgk");
        public static readonly string CURVE_TO_FREE = FetchTestFile("CurveToFree.dgk");
        public static readonly string CURVE_TO_PROJECT = FetchTestFile("CurveToProject.dgk");
        public static readonly string CURVES_TO_WRAP = FetchTestFile("CurvesToWrap.dgk");
        public static readonly string SURFACE_TO_WRAP_ONTO = FetchTestFile("SurfaceToWrapOnto.dgk");
        public static readonly string SOLID_TO_WRAP_ONTO = FetchTestFile("SolidToWrapOnto.dgk");
        public static readonly string ARC_LIMITERS = FetchTestFile("ArcLimiters.dgk");
        public static readonly string COMPCURVE_LIMITERS = FetchTestFile("CompCurveLimiters.dgk");
        public static readonly string CURVE_LIMITERS = FetchTestFile("CurveLimiters.dgk");
        public static readonly string SURFACE_LIMITERS = FetchTestFile("SurfaceLimiters.dgk");
        public static readonly string TWO_SURFACES_FOR_INTERSECTION = FetchTestFile("TwoSurfacesForIntersection.dgk");
        public static readonly string MULT_SURFACES_FOR_INTERSECTION = FetchTestFile("MultipleSurfacesForIntersection.dgk");
        public static readonly string SOLID_OPEN = FetchTestFile("SolidOpen.dgk");
        public static readonly string SOLID_TO_REMOVE = FetchTestFile("SolidToRemove.dgk");
        public static readonly string SURFACE_DRIVECURVE = FetchTestFile("SurfaceDriveCurve.dgk");
        public static readonly string SURFACE_FILLIN = FetchTestFile("SurfaceFillIn.dgk");
        public static readonly string SURFACE_FILLIN_MULTIPLE = FetchTestFile("SurfaceFillInMultiple.dgk");
        public static readonly string SURFACE_FROM_NETWORK = FetchTestFile("SurfaceFromNetwork.dgk");
        public static readonly string SURFACE_FROM_SEPARATE = FetchTestFile("SurfaceFromSeparate.dgk");
        public static readonly string SURFACE_PLANE_OF_BEST_FIT = FetchTestFile("SurfacePlaneOfBestFit.dgk");
        public static readonly string SURFACE_TWO_RAILS = FetchTestFile("SurfaceTwoRails.dgk");
        public static readonly string SURFACE_LATSOPENCLOSED = FetchTestFile("SurfaceLatsOpenClosed.dgk");
        public static readonly string SURFACE_LONSOPENCLOSED = FetchTestFile("SurfaceLonsOpenClosed.dgk");
        public static readonly string SURFACE_BADLYTRIMMED = FetchTestFile("SurfaceBadlyTrimmed.dgk");
        public static readonly string SURFACE_TRIM = FetchTestFile("SurfaceTrim.dgk");
        public static readonly string TWO_SURFACE_MORPH = FetchTestFile("TwoSurfaceMorph.dgk");
        public static readonly string TWO_SURFACE_MORPH_SOLIDS = FetchTestFile("TwoSurfaceMorphSolids.dgk");
        public static readonly string WIREFRAMES_FOR_DRIVE_SURFACE = FetchTestFile("WireframesForDriveSurface.dgk");
        public static readonly string VIEWS_CUBE = FetchTestFile("ViewsCube.dgk");
        public static readonly string COMPCURVE_DUCT = FetchTestFile("TwoCompCurves.pic");
        public static readonly string PIC_FILE_FOR_ARCFIT = FetchTestFile("NotArcFitted.pic");
        public static readonly string STITCH_FILE = FetchTestFile("Stitch.dgk");
        public static readonly string TRIMMED_SURFACE = FetchTestFile("TrimmedSurface.dgk");

        #endregion

        #endregion

        #region PowerSHAPE Models

        public static readonly string LEVELS_MODEL = FetchTestFile("LevelTests.psmodel");
        public static readonly string ADD_INTERSECT_SUBTRACT_MODEL = FetchTestFile("AddIntersectSubtractTests.psmodel");
        public static readonly string MINIMUM_DISTANCE_TESTS = FetchTestFile("MinimumDistanceTests.psmodel");
        public static readonly string SURFACE_FROM_TRIANGLES = FetchTestFile("SurfaceFromTriangles.psmodel");
        public static readonly string SINGLE_ELECTRODE_MODEL = FetchTestFile("SingleElectrode.psmodel");
        public static readonly string THREE_ELECTRODES_MODEL = FetchTestFile("ThreeElectrodes.psmodel");
        public static readonly string ELECTRODES_MODEL = FetchTestFile("Electrodes.psmodel");
        public static readonly string MANY_ELECTRODES_MODEL = FetchTestFile("ManyElectrodes.psmodel");
        public static readonly string CREATE_SOLIDS_FROM_SURFACES_MODEL = FetchTestFile("CreateSolidsFromSurfaces.psmodel");
        public static readonly string NUMERIC_NAMED_MODEL = FetchTestFile("123.psmodel");
        public static readonly string CreateThreePoints = FetchTestFile("CreateThreePoints.mac");
        public static readonly string PUMPKIN_MODEL = FetchTestFile("pumpkin.psmodel");

        #endregion

        private static string SetSolutionFolder()
        {
            var currentLocation = TestContext.CurrentContext.TestDirectory;
            var solutionDirector = currentLocation.Split(new[] {"\\bin\\"}, StringSplitOptions.RemoveEmptyEntries).First();
            return Path.Combine(solutionDirector, FOLDER_NAME);
        }

        public static string FetchTestFile(string fullName)
        {
            var directoryInfo = new DirectoryInfo(DIRECTORY);
            var results = directoryInfo.GetFiles(fullName, SearchOption.AllDirectories);
            return results.FirstOrDefault()?.FullName;
        }

        public static Directory FetchDirectory(string fullName)
        {
            var dir = new DirectoryInfo(DIRECTORY);
            var result = dir.GetDirectories(fullName, SearchOption.AllDirectories);
            return new Directory(result.FirstOrDefault()?.FullName);
        }
    }
}