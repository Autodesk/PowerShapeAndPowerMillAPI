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
using System.Text;
using Microsoft.VisualBasic;

namespace Autodesk.Geometry
{
    /// <summary>
    /// Encapsulates a point cloud, a collection of points that are not linked together.
    /// </summary>
    [Serializable]
    public class PointCloud : List<Point>
    {
        #region "Fields"

        #endregion

        #region "Constructors"

        /// <summary>
        /// Constructs an empty PointCloud.
        /// </summary>
        public PointCloud()
        {
        }

        /// <summary>
        /// Constructs a PointCloud and populates it with the specified points.
        /// </summary>
        /// <param name="points">List of points with which to populate the PointCloud.</param>
        public PointCloud(List<Point> points)
        {
            AddRange(points);
        }

        #endregion

        #region "Properties"

        /// <summary>
        /// Calculates and returns the bounding box of the point cloud.
        /// </summary>
        /// <returns>Bounding box of this PointCloud.</returns>
        public BoundingBox BoundingBox
        {
            get
            {
                MM minX = double.MaxValue;
                MM maxX = double.MinValue;
                MM minY = double.MaxValue;
                MM maxY = double.MinValue;
                MM minZ = double.MaxValue;
                MM maxZ = double.MinValue;

                if (Count == 0)
                {
                    return null;
                }

                foreach (Point cloudPoint in this)
                {
                    if (cloudPoint.X < minX)
                    {
                        minX = cloudPoint.X;
                    }
                    if (cloudPoint.X > maxX)
                    {
                        maxX = cloudPoint.X;
                    }

                    if (cloudPoint.Y < minY)
                    {
                        minY = cloudPoint.Y;
                    }
                    if (cloudPoint.Y > maxY)
                    {
                        maxY = cloudPoint.Y;
                    }

                    if (cloudPoint.Z < minZ)
                    {
                        minZ = cloudPoint.Z;
                    }
                    if (cloudPoint.Z > maxZ)
                    {
                        maxZ = cloudPoint.Z;
                    }
                }

                return new BoundingBox(minX, maxX, minY, maxY, minZ, maxZ);
            }
        }

        /// <summary>
        /// Point representing the centre of the point cloud.
        /// </summary>
        public Point Centre
        {
            get
            {
                int PointCount = Count;

                if (PointCount == 0)
                {
                    return null;
                }

                double xDouble = 0.0;
                double yDouble = 0.0;
                double zDouble = 0.0;
                foreach (Point pt in this)
                {
                    xDouble += pt.X;
                    yDouble += pt.Y;
                    zDouble += pt.Y;
                }

                return new Point(xDouble / Convert.ToDouble(PointCount),
                                 yDouble / Convert.ToDouble(PointCount),
                                 zDouble / Convert.ToDouble(PointCount));
            }
        }

        #endregion

        #region "Operations"

        /// <summary>
        /// Creates a DUCTPicture file at the specified path and using the specified units.
        /// </summary>
        /// <param name="file">Path to file.</param>
        /// <param name="enmUnits">Units to use.</param>
        public void WriteDUCTPictureFile(FileSystem.File file, LengthUnits enmUnits)
        {
            try
            {
                //Delete the file if it already exists
                file.Delete();

                StringBuilder strPictureData = new StringBuilder();

                //Write header
                string strShortFileName = file.Name;
                string strDateTime = null;
                var _with1 = DateAndTime.Now;
                strDateTime = _with1.Day + " " + DateAndTime.MonthName(_with1.Month, true).ToUpper() + " " + _with1.Year + " " +
                              _with1.Hour + "." + _with1.Minute + "." + _with1.Second;
                strPictureData.Append("   DuctPicture PICTURE FILE  " + strShortFileName + "  " + strDateTime +
                                      Constants.vbNewLine);

                //Write part line
                string strUnits = "";
                switch (enmUnits)
                {
                    case LengthUnits.MM:
                        strUnits = "MM";
                        break;
                    case LengthUnits.Inches:
                        strUnits = "INCHES";
                        break;
                }
                strPictureData.Append(" PART:                     PowerMILL MADE IN     " + strUnits + Constants.vbNewLine);

                //Write special marker
                strPictureData.Append(" *" + Constants.vbNewLine);

                //Write integer data
                string strIntLine = "";
                if (Count > 1000000)
                {
                    strIntLine += " " + GetField(4, 3) + GetField(3, 3) + GetField(-1, 11) + GetField(1, 11) +
                                  GetField(Count, 11) +
                                  GetField(-2, 6) + GetField(2, 6) + GetField(0, 6) + GetField(0, 6) + GetField(0, 6);
                }
                else
                {
                    strIntLine += " " + GetField(4, 6) + GetField(3, 6) + GetField(-1, 6) + GetField(1, 6) + GetField(Count, 6) +
                                  GetField(-2, 6) + GetField(2, 6) + GetField(0, 6) + GetField(0, 6) + GetField(0, 6);
                }
                strPictureData.Append(strIntLine + Constants.vbNewLine);

                //Write size data
                string strSizeLine = "";
                strSizeLine += " " + GetField(0, "0.0000", 10) + GetField(0, "0.0000", 10) + GetField(0, "0.0000", 10) +
                               GetField(0, "0.0000", 10) + GetField(0, "0.0000", 10) + GetField(0, "0.0000", 10) +
                               GetField(0, "0.0000", 10);
                strPictureData.Append(strSizeLine + Constants.vbNewLine);

                //Write instruction codes
                int intInstructionCode = GetInstructionCode(1, 0, 0, 0, 0, 0);
                string strInstructionCode = "";
                if (Count > 1000000)
                {
                    strInstructionCode = " " + GetField(intInstructionCode, 11) + " " + GetField(Count, 11) + Constants.vbNewLine;
                    strPictureData.Append(strInstructionCode);
                }
                else
                {
                    strInstructionCode = " " + GetField(intInstructionCode, 11) + " " + GetField(Count, 6) + Constants.vbNewLine;
                    strPictureData.Append(strInstructionCode);
                }

                //Write polyline data
                string strPointCode = null;
                for (int p = 0; p <= Count - 1; p++)
                {
                    //Use System.Globalization.CultureInfo.InvariantCulture
                    strPointCode = string.Format(System.Globalization.CultureInfo.InvariantCulture,
                                                 " {0,14:E} {1,14:E} {2,14:E}",
                                                 this[p].X,
                                                 this[p].Y,
                                                 this[p].Z);
                    strPictureData.Append(strPointCode + Constants.vbNewLine);
                }

                //Write the file
                file.WriteText(strPictureData.ToString(), false, Encoding.ASCII);
            }
            catch (Exception ex)
            {
                file.Delete();
                throw;
            }
        }

        private static int GetInstructionCode(int IPCOL, int IARC, int IIMRK, int IJRMK, int IUPDN, int IDASH)
        {
            return IPCOL * 2048 + IARC * 512 + IIMRK * 64 + IJRMK * 32 + IUPDN * 8 + IDASH * 1;
        }

        private static string GetField(int Value, int FieldLength)
        {
            string tmp = Value.ToString("0");
            tmp = Strings.Space(FieldLength - tmp.Length) + tmp;
            return tmp;
        }

        private static string GetField(double Value, string FormatCode, int FieldLength)
        {
            //Use System.Globalization.CultureInfo.InvariantCulture
            string tmp = Value.ToString(FormatCode, System.Globalization.CultureInfo.InvariantCulture);
            tmp = Strings.Space(FieldLength - tmp.Length) + tmp;
            return tmp;
        }

        /// <summary>
        /// Returns a clone of this PointCloud.
        /// </summary>
        public PointCloud Clone()
        {
            PointCloud pcClone = new PointCloud();
            foreach (Point p in this)
            {
                pcClone.Add(p.Clone());
            }
            return pcClone;
        }

        /// <summary>
        /// Returns the index of the point that is the minimum in the specified axis.
        /// </summary>
        /// <param name="Axis">X, Y or Z.</param>
        /// <returns>Index of the minimum point.</returns>
        public int IndexOfMinumumPoint(Axes Axis)
        {
            int functionReturnValue = 0;

            if (Count == 0)
            {
                return -1;
            }
            double dblMinimum = 0;
            switch (Axis)
            {
                case Axes.X:
                    dblMinimum = this[0].X;
                    break;
                case Axes.Y:
                    dblMinimum = this[0].Y;
                    break;
                case Axes.Z:
                    dblMinimum = this[0].Z;
                    break;
            }

            functionReturnValue = 0;
            int index = 0;
            foreach (Point pt in this)
            {
                switch (Axis)
                {
                    case Axes.X:
                        if (dblMinimum > pt.X.Value)
                        {
                            dblMinimum = pt.X;
                            functionReturnValue = index;
                        }
                        break;
                    case Axes.Y:
                        if (dblMinimum > pt.Y.Value)
                        {
                            dblMinimum = pt.Y;
                            functionReturnValue = index;
                        }
                        break;
                    case Axes.Z:
                        if (dblMinimum > pt.Z.Value)
                        {
                            dblMinimum = pt.Z;
                            functionReturnValue = index;
                        }
                        break;
                }
                index += 1;
            }
            return functionReturnValue;
        }

        #endregion
    }
}