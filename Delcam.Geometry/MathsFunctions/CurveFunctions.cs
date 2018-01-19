// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using System.Collections;

namespace Autodesk.Geometry
{
    internal class CurveFunctions
    {
        /// <summary>
        /// Polygonises a 2D cubic curve returning the points for the polyline.
        /// The cubic curve control points must be supplied in order, e.g.
        /// Anchor 1, Control 1a, Control 1b, Anchor 2, Control 2a, Control 2b, Anchor 3
        /// </summary>
        /// <history>
        /// Who     When      What
        /// ----- ---------- ---------------------------------------------------
        /// DAL  23/11/2006  Written
        /// </history>
        internal static Point[] PolygoniseCubicCurve(Point[] cp, double Tolerance)
        {
            double SqTol = Tolerance * Tolerance;
            Point[] p = null;
            Point[] firstpoint = SplicePointArray(ref cp, 0, 1);
            Point last = null;
            last = firstpoint[0];
            Point[] lp = {last};
            p = SplicePointArray(ref cp, 0, 3);
            while (p.Length == 3)
            {
                Point[] np = GetPolyPointsForCubicSpan(last, p[0], p[1], p[2], SqTol);
                int Offset = lp.Length;
                Array.Resize(ref lp, lp.Length + np.Length + 1);
                for (int i = 0; i <= np.Length - 1; i++)
                {
                    lp[Offset + i] = np[i];
                }
                lp[lp.Length - 1] = p[2];
                last = p[2];
                p = SplicePointArray(ref cp, 0, 3);
            }
            if (cp.Length != 0)
            {
                throw new Exception("Invalid cubic curve specified.");
            }
            return lp;
        }

        /// <summary>
        /// Returns 2D points from an array by the specified Offset, and Length.
        /// The original array has the extracted points removed from it.
        /// </summary>
        /// <history>
        /// Who     When      What
        /// ----- ---------- ---------------------------------------------------
        /// DAL  23/11/2006  Written
        /// </history>
        private static Point[] SplicePointArray(ref Point[] CurrentArray, int Offset, int Length)
        {
            //Make safe copy of array
            Point[] SafeArray = null;
            SafeArray = new Point[CurrentArray.Length];
            for (int i = 0; i <= SafeArray.Length - 1; i++)
            {
                SafeArray[i] = CurrentArray[i];
            }

            try
            {
                //Store indeces of original array
                int[] Id = null;
                Id = new int[CurrentArray.Length];
                for (int i = 0; i <= Id.Length - 1; i++)
                {
                    Id[i] = i;
                }
                ArrayList OriginalArray = new ArrayList(Id);

                //Check if length is valid
                if (Offset + Length > OriginalArray.Count - 1)
                {
                    Length = OriginalArray.Count - Offset;
                }

                //Get indeces of points to extract
                ArrayList ReturnArray = OriginalArray.GetRange(Offset, Length);

                //Setup return array from indeces extracted
                Point[] ReturnValues = new Point[-1 + 1];
                ReturnValues = new Point[ReturnArray.Count];
                for (int i = 0; i <= ReturnArray.Count - 1; i++)
                {
                    ReturnValues[i] = SafeArray[Convert.ToInt32(ReturnArray[i])];
                }

                //Remove extracted values from original array
                OriginalArray.RemoveRange(Offset, Length);
                CurrentArray = new Point[OriginalArray.Count];
                for (int i = 0; i <= OriginalArray.Count - 1; i++)
                {
                    CurrentArray[i] = SafeArray[Convert.ToInt32(OriginalArray[i])];
                }

                return ReturnValues;
            }
            catch
            {
                //Return nothing and restore original array if failure occurs
                CurrentArray = new Point[SafeArray.Length];
                for (int i = 0; i <= SafeArray.Length - 1; i++)
                {
                    CurrentArray[i] = SafeArray[i];
                }
                return null;
            }
        }

        /// <summary>
        /// Returns the points that polygonise a single cubic curve span to the specified squared tolerance.
        /// </summary>
        /// <history>
        /// Who     When      What
        /// ----- ---------- ---------------------------------------------------
        /// DAL  23/11/2006  Written
        /// </history>
        private static Point[] GetPolyPointsForCubicSpan(Point AP0, Point CP0, Point CP1, Point AP1, double SquareTolerance)
        {
            Point p0 = null;
            Point p10 = null;
            Point p20 = null;
            Point p30 = null;
            Point p21 = null;
            Point p12 = null;
            Point p3 = null;
            Point[] ret = new Point[-1 + 1];
            Point[] Res = DivideCubicSpan(AP0, CP0, CP1, AP1, 0.5);
            p0 = Res[0];
            p10 = Res[1];
            p20 = Res[2];
            p30 = Res[3];
            p21 = Res[4];
            p12 = Res[5];
            p3 = Res[6];
            Point c = new Point((p0.X + p3.X) / 2.0, (p0.Y + p3.Y) / 2.0, (p0.Z + p3.Z) / 2.0);
            if ((p30.X - c.X) * (p30.X - c.X) + (p30.Y - c.Y) * (p30.Y - c.Y) + (p30.Z - c.Z) * (p30.Z - c.Z) < SquareTolerance)
            {
                Point c0 = new Point((p0.X + p30.X) / 2.0, (p0.Y + p30.Y) / 2.0, (p0.Z + p30.Z) / 2.0);
                Point[] Res2 = DivideCubicSpan(AP0, CP0, CP1, AP1, 0.25);
                Point pp30 = Res2[3];

                if ((pp30.X - c0.X) * (pp30.X - c0.X) + (pp30.Y - c0.Y) * (pp30.Y - c0.Y) + (pp30.Z - c0.Z) * (pp30.Z - c0.Z) <
                    SquareTolerance)
                {
                    ret = new Point[-1 + 1];
                    return ret;
                }
            }

            int Offset = 0;
            Point[] SubRet1 = GetPolyPointsForCubicSpan(p0, p10, p20, p30, SquareTolerance);
            Offset = ret.Length;
            Array.Resize(ref ret, ret.Length + SubRet1.Length);
            for (int i = 0; i <= SubRet1.Length - 1; i++)
            {
                ret[Offset + i] = SubRet1[i];
            }

            Array.Resize(ref ret, ret.Length + 1);
            ret[ret.Length - 1] = p30;

            Point[] SubRet2 = GetPolyPointsForCubicSpan(p30, p21, p12, p3, SquareTolerance);
            Offset = ret.Length;
            Array.Resize(ref ret, ret.Length + SubRet2.Length);
            for (int i = 0; i <= SubRet2.Length - 1; i++)
            {
                ret[Offset + i] = SubRet2[i];
            }

            return ret;
        }

        /// <summary>
        /// divides one segment of the cubic bezier curve at ratio "sep", and returns
        /// the new cubic bezier which has two segment (7 points).
        /// </summary>
        private static Point[] DivideCubicSpan(Point p0, Point p1, Point p2, Point p3, double sep)
        {
            Point[] ret = new Point[7];
            var p10 = new Point();
            var p11 = new Point();
            var p12 = new Point();
            var p20 = new Point();
            var p21 = new Point();
            Point p30 = new Point();
            p10.X = p0.X + sep * (p1.X - p0.X);
            p10.Y = p0.Y + sep * (p1.Y - p0.Y);
            p10.Z = p0.Z + sep * (p1.Z - p0.Z);
            p11.X = p1.X + sep * (p2.X - p1.X);
            p11.Y = p1.Y + sep * (p2.Y - p1.Y);
            p11.Z = p1.Z + sep * (p2.Z - p1.Z);
            p12.X = p2.X + sep * (p3.X - p2.X);
            p12.Y = p2.Y + sep * (p3.Y - p2.Y);
            p12.Z = p2.Z + sep * (p3.Z - p2.Z);
            p20.X = p10.X + sep * (p11.X - p10.X);
            p20.Y = p10.Y + sep * (p11.Y - p10.Y);
            p20.Z = p10.Z + sep * (p11.Z - p10.Z);
            p21.X = p11.X + sep * (p12.X - p11.X);
            p21.Y = p11.Y + sep * (p12.Y - p11.Y);
            p21.Z = p11.Z + sep * (p12.Z - p11.Z);
            p30.X = p20.X + sep * (p21.X - p20.X);
            p30.Y = p20.Y + sep * (p21.Y - p20.Y);
            p30.Z = p20.Z + sep * (p21.Z - p20.Z);
            ret[0] = p0;
            ret[1] = p10;
            ret[2] = p20;
            ret[3] = p30;
            ret[4] = p21;
            ret[5] = p12;
            ret[6] = p3;
            return ret;
        }
    }
}