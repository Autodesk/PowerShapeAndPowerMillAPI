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

namespace Autodesk.Geometry
{
    internal class ArcFunctions
    {
        /// <summary>
        /// Returns two possible arc centres given two points on the arc, the plane of the arc and the arc radius.
        /// </summary>
        /// <param name="startPoint">First point on the arc.</param>
        /// <param name="endPoint">Second point on the arc.</param>
        /// <param name="radius">Radius of the arc.</param>
        /// <param name="plane">Plane of the arc.</param>
        /// <returns>List containing two possible arc centres.</returns>
        internal static List<Point> GetCentreFromPointsAndRadius(Point startPoint, Point endPoint, MM radius, Planes plane)
        {
            // Determine which point coordinates to use, dependent upon the plane
            Point firstCentre = new Point();
            Point secondCentre = new Point();
            float startPointCoordinate1 = 0;
            float startPointCoordinate2 = 0;
            float endPointCoordinate1 = 0;
            float endPointCoordinate2 = 0;
            startPointCoordinate1 =
                Convert.ToSingle(
                    (MM)
                    startPoint.GetType()
                              .GetProperty(Enum.GetName(plane.GetType(), plane)[0].ToString())
                              .GetValue(startPoint, null));
            startPointCoordinate2 =
                Convert.ToSingle(
                    (MM)
                    startPoint.GetType()
                              .GetProperty(Enum.GetName(plane.GetType(), plane)[1].ToString())
                              .GetValue(startPoint, null));
            endPointCoordinate1 =
                Convert.ToSingle(
                    (MM)
                    startPoint.GetType()
                              .GetProperty(Enum.GetName(plane.GetType(), plane)[0].ToString())
                              .GetValue(endPoint, null));
            endPointCoordinate2 =
                Convert.ToSingle(
                    (MM)
                    startPoint.GetType()
                              .GetProperty(Enum.GetName(plane.GetType(), plane)[1].ToString())
                              .GetValue(endPoint, null));

            // Find distance between two points
            double distance =
                Math.Sqrt(Math.Pow(endPointCoordinate1 - startPointCoordinate1, 2) +
                          Math.Pow(endPointCoordinate2 - startPointCoordinate2, 2));

            // Find midpoint
            float midpointCoordinate1 = 0;
            float midpointCoordinate2 = 0;
            midpointCoordinate1 = (startPointCoordinate1 + endPointCoordinate1) / 2;
            midpointCoordinate2 = (startPointCoordinate2 + endPointCoordinate2) / 2;

            // Find centre coordinates
            double firstCentreCoordinate1 = 0;
            double firstCentreCoordinate2 = 0;
            double secondCentreCoordinate1 = 0;
            double secondCentreCoordinate2 = 0;
            firstCentreCoordinate1 = midpointCoordinate1 +
                                     Math.Sqrt(Math.Pow(radius, 2) - Math.Pow(distance / 2, 2)) *
                                     (startPointCoordinate2 - endPointCoordinate2) / distance;
            firstCentreCoordinate2 = midpointCoordinate2 +
                                     Math.Sqrt(Math.Pow(radius, 2) - Math.Pow(distance / 2, 2)) *
                                     (endPointCoordinate1 - startPointCoordinate1) / distance;
            secondCentreCoordinate1 = midpointCoordinate1 -
                                      Math.Sqrt(Math.Pow(radius, 2) - Math.Pow(distance / 2, 2)) *
                                      (startPointCoordinate2 - endPointCoordinate2) / distance;
            secondCentreCoordinate2 = midpointCoordinate2 -
                                      Math.Sqrt(Math.Pow(radius, 2) - Math.Pow(distance / 2, 2)) *
                                      (endPointCoordinate1 - startPointCoordinate1) / distance;

            // Create points to return
            switch (plane)
            {
                case Planes.YZ:
                    firstCentre.X = startPoint.X;
                    firstCentre.Y = firstCentreCoordinate1;
                    firstCentre.Z = firstCentreCoordinate2;
                    secondCentre.X = startPoint.X;
                    secondCentre.Y = secondCentreCoordinate1;
                    secondCentre.Z = secondCentreCoordinate2;
                    break;
                case Planes.ZX:
                    firstCentre.X = firstCentreCoordinate1;
                    firstCentre.Y = startPoint.Y;
                    firstCentre.Z = firstCentreCoordinate2;
                    secondCentre.X = secondCentreCoordinate1;
                    secondCentre.Y = startPoint.Y;
                    secondCentre.Z = secondCentreCoordinate2;
                    break;
                case Planes.XY:
                    firstCentre.X = firstCentreCoordinate1;
                    firstCentre.Y = firstCentreCoordinate2;
                    firstCentre.Z = startPoint.Z;
                    secondCentre.X = secondCentreCoordinate1;
                    secondCentre.Y = secondCentreCoordinate2;
                    secondCentre.Z = startPoint.Z;
                    break;
            }

            // Return points
            List<Point> centrePoints = new List<Point>();
            centrePoints.Add(firstCentre);
            centrePoints.Add(secondCentre);
            return centrePoints;
        }
    }
}