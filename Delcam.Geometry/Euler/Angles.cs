// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using Autodesk.Matrix;

namespace Autodesk.Geometry.Euler
{
    /// <summary>
    /// Euler angle conventions supported by Autodesk.Geometry.
    /// </summary>
    public enum Conventions
    {
        XZX,
        XYX,
        YXY,
        YZY,
        ZYZ,
        ZXZ,
        XZY,
        YXZ,
        XYZ,
        YZX,
        ZYX,
        ZXY
    }

    /// <summary>
    /// Constructs Euler rotational matrices.
    /// </summary>
    [Serializable]
    public class Angles
    {
        #region "Fields"

        private double[,] _matrix = new double[3, 3];

        #endregion

        #region "Constructors"

        /// <summary>
        /// Constructs a Euler rotation matrix based on the specified angles and convention.
        /// </summary>
        /// <param name="alpha">First angle of rotation.</param>
        /// <param name="beta"> Second angle of rotation.</param>
        /// <param name="gamma">Third angle of rotation.</param>
        /// <param name="convention">Specifies the axes of rotation and the order in which rotations are performed about the axes.</param>
        public Angles(Radian alpha, Radian beta, Radian gamma, Conventions convention)
        {
            switch (convention)
            {
                case Conventions.XZX:
                    MatrixXZX(alpha, beta, gamma);
                    break;
                case Conventions.XYX:
                    MatrixXYX(alpha, beta, gamma);
                    break;
                case Conventions.YXY:
                    MatrixYXY(alpha, beta, gamma);
                    break;
                case Conventions.YZY:
                    MatrixYZY(alpha, beta, gamma);
                    break;
                case Conventions.ZYZ:
                    MatrixZYZ(alpha, beta, gamma);
                    break;
                case Conventions.ZXZ:
                    MatrixZXZ(alpha, beta, gamma);
                    break;
                case Conventions.XZY:
                    MatrixXZY(alpha, beta, gamma);
                    break;
                case Conventions.XYZ:
                    MatrixXYZ(alpha, beta, gamma);
                    break;
                case Conventions.YXZ:
                    MatrixYXZ(alpha, beta, gamma);
                    break;
                case Conventions.YZX:
                    MatrixYZX(alpha, beta, gamma);
                    break;
                case Conventions.ZYX:
                    MatrixZYX(alpha, beta, gamma);
                    break;
                case Conventions.ZXY:
                    MatrixZXY(alpha, beta, gamma);
                    break;
            }
        }

        #endregion

        #region "Properties"

        /// <summary>
        /// Euler rotational matrix.
        /// </summary>
        public double[,] Matrix
        {
            get { return _matrix; }
        }

        /// <summary>
        /// The inverse of Euler rotational matrix.
        /// </summary>
        public double[,] MatrixInverse
        {
//The inverse of a rotation matrix is its transpose
            get { return _matrix.Transpose(); }
        }

        #endregion

        #region "Public Methods"

        /// <summary>
        /// Computes Euler angles from a MatrixXYZ.
        /// </summary>
        /// <returns>The rotation around z, the rotation around y and the rotation around x, by this order.</returns>
        public static Degree[] GetRotationsFromMatrixXYZ(double[,] rotationMatrix)
        {
            // Get the rotation around z using line x
            double zDegrees = 180 / Math.PI * Math.Atan2(rotationMatrix[1, 0], rotationMatrix[0, 0]);
            ApplyZRotation(ref rotationMatrix, -zDegrees, true);

            // Get the rotation around y using line x
            double yDegrees = -180 / Math.PI * Math.Atan2(rotationMatrix[2, 0], rotationMatrix[0, 0]);
            ApplyYRotation(ref rotationMatrix, -yDegrees, true);

            // Get the rotation around x using line z
            double xDegrees = 180 / Math.PI * Math.Atan2(rotationMatrix[2, 2], rotationMatrix[1, 2]) - 90;
            ApplyXRotation(ref rotationMatrix, -xDegrees, true);

            return new[]
            {
                new Degree(zDegrees),
                new Degree(yDegrees),
                new Degree(xDegrees)
            };
        }

        #endregion

        #region "Private Operations"

        /// <summary>
        /// This operation sets the Matrix for XZX
        /// </summary>
        private void MatrixXZX(Radian alpha, Radian beta, Radian gamma)
        {
            double cos1 = Math.Cos(alpha);
            double sin1 = Math.Sin(alpha);
            double cos2 = Math.Cos(beta);
            double sin2 = Math.Sin(beta);
            double cos3 = Math.Cos(gamma);
            double sin3 = Math.Sin(gamma);

            _matrix = new double[3, 3]
            {
                {
                    cos2,
                    -(cos1 * sin2),
                    sin1 * sin2
                },
                {
                    cos3 * sin2,
                    cos1 * cos2 * cos3 - sin2 * sin3,
                    -(cos2 * cos3 * sin1) - cos1 * sin3
                },
                {
                    sin2 * sin3,
                    cos3 * sin1 + cos1 * cos2 * sin3,
                    cos1 * cos3 - cos2 * sin1 * sin3
                }
            };
        }

        /// <summary>
        /// This operation sets the Matrix for XYX
        /// </summary>
        private void MatrixXYX(Radian alpha, Radian beta, Radian gamma)
        {
            double cos1 = Math.Cos(alpha);
            double sin1 = Math.Sin(alpha);
            double cos2 = Math.Cos(beta);
            double sin2 = Math.Sin(beta);
            double cos3 = Math.Cos(gamma);
            double sin3 = Math.Sin(gamma);

            _matrix = new double[3, 3]
            {
                {
                    cos2,
                    sin1 * sin2,
                    cos1 * sin2
                },
                {
                    sin2 * sin3,
                    cos1 * cos3 - cos2 * sin1 * sin3,
                    -(cos3 * sin1) - cos1 * cos2 * sin3
                },
                {
                    -(cos3 * sin2),
                    cos2 * cos3 * sin1 + cos1 * sin3,
                    cos1 * cos2 * cos3 - sin1 * sin3
                }
            };
        }

        /// <summary>
        /// This operation sets the Matrix for YXY
        /// </summary>
        private void MatrixYXY(Radian alpha, Radian beta, Radian gamma)
        {
            double cos1 = Math.Cos(alpha);
            double sin1 = Math.Sin(alpha);
            double cos2 = Math.Cos(beta);
            double sin2 = Math.Sin(beta);
            double cos3 = Math.Cos(gamma);
            double sin3 = Math.Sin(gamma);

            _matrix = new double[3, 3]
            {
                {
                    cos1 * cos3 - cos2 * sin1 * sin3,
                    sin2 * sin3,
                    cos3 * sin1 + cos1 * cos2 * sin3
                },
                {
                    sin1 * sin2,
                    cos2,
                    -(cos1 * sin2)
                },
                {
                    -(cos2 * cos3 * sin1) - cos1 * sin3,
                    cos3 * sin2,
                    cos1 * cos2 * cos3 - sin1 * sin3
                }
            };
        }

        /// <summary>
        /// This operation sets the Matrix for YZY
        /// </summary>
        private void MatrixYZY(Radian alpha, Radian beta, Radian gamma)
        {
            double cos1 = Math.Cos(alpha);
            double sin1 = Math.Sin(alpha);
            double cos2 = Math.Cos(beta);
            double sin2 = Math.Sin(beta);
            double cos3 = Math.Cos(gamma);
            double sin3 = Math.Sin(gamma);

            _matrix = new double[3, 3]
            {
                {
                    cos1 * cos2 * cos3 - sin1 * sin3,
                    -(cos3 * sin2),
                    cos2 * cos3 * sin1 + cos1 * sin3
                },
                {
                    cos1 * sin2,
                    cos2,
                    sin1 * sin2
                },
                {
                    -(cos3 * sin1) - cos1 * cos2 * sin3,
                    sin2 * sin3,
                    cos1 * cos3 - cos2 * sin1 * sin3
                }
            };
        }

        /// <summary>
        /// This operation sets the Matrix for ZYZ
        /// </summary>
        private void MatrixZYZ(Radian alpha, Radian beta, Radian gamma)
        {
            double cos1 = Math.Cos(alpha);
            double sin1 = Math.Sin(alpha);
            double cos2 = Math.Cos(beta);
            double sin2 = Math.Sin(beta);
            double cos3 = Math.Cos(gamma);
            double sin3 = Math.Sin(gamma);
            _matrix = new double[3, 3]
            {
                {
                    cos1 * cos2 * cos3 - sin1 * sin3,
                    -(cos2 * cos3 * sin1) - cos1 * sin3,
                    cos3 * sin2
                },
                {
                    cos3 * sin1 + cos1 * cos2 * sin3,
                    cos1 * cos3 - cos2 * sin1 * sin3,
                    sin2 * sin3
                },
                {
                    -(cos1 * sin2),
                    sin1 * sin2,
                    cos2
                }
            };
        }

        /// <summary>
        /// This operation sets the Matrix for ZXZ
        /// </summary>
        private void MatrixZXZ(Radian alpha, Radian beta, Radian gamma)
        {
            double cos1 = Math.Cos(alpha);
            double sin1 = Math.Sin(alpha);
            double cos2 = Math.Cos(beta);
            double sin2 = Math.Sin(beta);
            double cos3 = Math.Cos(gamma);
            double sin3 = Math.Sin(gamma);

            _matrix = new double[3, 3]
            {
                {
                    cos1 * cos3 - cos2 * sin1 * sin3,
                    -(cos3 * sin1) - cos1 * cos2 * sin3,
                    sin2 * sin3
                },
                {
                    cos2 * cos3 * sin1 + cos1 * sin3,
                    cos1 * cos2 * cos3 - sin1 * sin3,
                    -(cos3 * sin2)
                },
                {
                    sin1 * sin2,
                    cos1 * sin2,
                    cos2
                }
            };
        }

        /// <summary>
        /// This operation sets the Matrix for XZY
        /// </summary>
        private void MatrixXZY(Radian alpha, Radian beta, Radian gamma)
        {
            double cos1 = Math.Cos(alpha);
            double sin1 = Math.Sin(alpha);
            double cos2 = Math.Cos(beta);
            double sin2 = Math.Sin(beta);
            double cos3 = Math.Cos(gamma);
            double sin3 = Math.Sin(gamma);

            _matrix = new double[3, 3]
            {
                {
                    cos2 * cos3,
                    sin1 * sin3 - cos1 * cos3 * sin2,
                    cos2 * sin1 * sin2 + cos1 * sin3
                },
                {
                    sin2,
                    cos1 * cos2,
                    -(cos2 * sin1)
                },
                {
                    -(cos2 * sin3),
                    cos3 * sin1 + cos1 * sin2 * sin3,
                    cos1 * cos3 - sin1 * sin2 * sin3
                }
            };
        }

        /// <summary>
        /// This operation sets the Matrix for XYZ
        /// </summary>
        private void MatrixXYZ(Radian alpha, Radian beta, Radian gamma)
        {
            double cos1 = Math.Cos(alpha);
            double sin1 = Math.Sin(alpha);
            double cos2 = Math.Cos(beta);
            double sin2 = Math.Sin(beta);
            double cos3 = Math.Cos(gamma);
            double sin3 = Math.Sin(gamma);

            _matrix = new double[3, 3]
            {
                {
                    cos2 * cos3,
                    cos3 * sin1 * sin2 - cos1 * sin3,
                    cos1 * cos3 * sin2 + sin1 * sin3
                },
                {
                    cos2 * sin3,
                    cos1 * cos3 + sin1 * sin2 * sin3,
                    cos1 * sin2 * sin3 - cos3 * sin1
                },
                {
                    -sin2,
                    cos2 * sin1,
                    cos1 * cos2
                }
            };
        }

        /// <summary>
        /// This operation sets the Matrix for YXZ
        /// </summary>
        private void MatrixYXZ(Radian alpha, Radian beta, Radian gamma)
        {
            double cos1 = Math.Cos(alpha);
            double sin1 = Math.Sin(alpha);
            double cos2 = Math.Cos(beta);
            double sin2 = Math.Sin(beta);
            double cos3 = Math.Cos(gamma);
            double sin3 = Math.Sin(gamma);

            _matrix = new double[3, 3]
            {
                {
                    cos1 * cos3 - sin1 * sin2 * sin3,
                    -(cos2 * sin3),
                    cos2 * sin1 + cos1 * sin2 * sin3
                },
                {
                    cos3 * sin1 * sin2 + cos1 * sin3,
                    cos2 * cos3,
                    sin1 * sin3 - cos1 * cos3 * sin2
                },
                {
                    -(cos2 * sin1),
                    sin2,
                    cos1 * cos2
                }
            };
        }

        /// <summary>
        /// This operation sets the Matrix for YZX
        /// </summary>
        private void MatrixYZX(Radian alpha, Radian beta, Radian gamma)
        {
            double cos1 = Math.Cos(alpha);
            double sin1 = Math.Sin(alpha);
            double cos2 = Math.Cos(beta);
            double sin2 = Math.Sin(beta);
            double cos3 = Math.Cos(gamma);
            double sin3 = Math.Sin(gamma);

            _matrix = new double[3, 3]
            {
                {
                    cos1 * cos2,
                    -sin2,
                    cos2 * sin1
                },
                {
                    cos1 * cos3 * sin2 + sin1 * sin3,
                    cos2 * cos3,
                    cos3 * sin1 * sin2 - cos1 * sin3
                },
                {
                    cos1 * sin2 * sin3 - cos3 * sin1,
                    cos2 * sin3,
                    cos1 * cos3 + sin1 * sin2 * sin3
                }
            };
        }

        /// <summary>
        /// This operation sets the Matrix for ZYX
        /// </summary>
        /// <history>
        /// Who  When        Why
        /// ---  ----        ---
        /// lae  12/01/2009  Initial version
        /// </history>
        private void MatrixZYX(Radian alpha, Radian beta, Radian gamma)
        {
            double cos1 = Math.Cos(alpha);
            double sin1 = Math.Sin(alpha);
            double cos2 = Math.Cos(beta);
            double sin2 = Math.Sin(beta);
            double cos3 = Math.Cos(gamma);
            double sin3 = Math.Sin(gamma);

            _matrix = new double[3, 3]
            {
                {
                    cos1 * cos2,
                    -(cos2 * sin1),
                    sin2
                },
                {
                    cos3 * sin1 + cos1 * sin2 * sin3,
                    cos1 * cos3 - sin1 * sin2 * sin3,
                    -(cos2 * sin3)
                },
                {
                    sin1 * sin3 - cos1 * cos3 * sin2,
                    cos3 * sin1 * sin2 + cos1 * sin3,
                    cos2 * cos3
                }
            };
        }

        /// <summary>
        /// This operation sets the Matrix for ZXY
        /// </summary>
        /// <history>
        /// Who  When        Why
        /// ---  ----        ---
        /// lae  12/01/2009  Initial version
        /// </history>
        private void MatrixZXY(Radian alpha, Radian beta, Radian gamma)
        {
            double cos1 = Math.Cos(alpha);
            double sin1 = Math.Sin(alpha);
            double cos2 = Math.Cos(beta);
            double sin2 = Math.Sin(beta);
            double cos3 = Math.Cos(gamma);
            double sin3 = Math.Sin(gamma);

            _matrix = new double[3, 3]
            {
                {
                    cos1 * cos3 + sin1 * sin2 * sin3,
                    cos1 * sin2 * sin3 - cos3 * sin1,
                    cos2 * sin3
                },
                {
                    cos2 * sin1,
                    cos1 * cos2,
                    -sin2
                },
                {
                    cos3 * sin1 * sin2 - cos1 * sin3,
                    cos1 * cos3 * sin2 + sin1 * sin3,
                    cos2 * cos3
                }
            };
        }

        /// <summary>
        /// Applies a rotation around the z-axis.
        /// </summary>
        /// <param name="rotationMatrix">The rotation matrix.</param>
        /// <param name="rotation">The rotation to apply around z-axis.</param>
        /// <param name="isInDegrees">True if the rotation is in degrees.</param>
        private static void ApplyZRotation(ref double[,] rotationMatrix, double rotation, bool isInDegrees = false)
        {
            if (isInDegrees)
            {
                rotation = rotation * Math.PI / 180;
            }

            double[,] r = Autodesk.Matrix.Matrix.Identity(3);

            double sinRot = Math.Sin(rotation);
            double cosRot = Math.Cos(rotation);

            r[0, 0] = cosRot;
            r[0, 1] = -sinRot;
            r[1, 0] = sinRot;
            r[1, 1] = cosRot;

            double[,] mTemp = rotationMatrix.Copy();
            rotationMatrix = r.Multiply(mTemp);
        }

        /// <summary>
        /// Applies a rotation around the y-axis.
        /// </summary>
        /// <param name="rotationMatrix">The transformation matrix that will contain the result.</param>
        /// <param name="rotation">The rotation to apply around y-axis.</param>
        /// <param name="isInDegrees">True if the rotation is in degrees.</param>
        private static void ApplyYRotation(ref double[,] rotationMatrix, double rotation, bool isInDegrees = false)
        {
            if (isInDegrees)
            {
                rotation = rotation * Math.PI / 180;
            }

            double[,] r = Autodesk.Matrix.Matrix.Identity(3);

            double sinRot = Math.Sin(rotation);
            double cosRot = Math.Cos(rotation);

            r[0, 0] = cosRot;
            r[0, 2] = sinRot;
            r[2, 0] = -sinRot;
            r[2, 2] = cosRot;

            double[,] mTemp = rotationMatrix.Copy();
            rotationMatrix = r.Multiply(mTemp);
        }

        /// <summary>
        /// Applies a rotation around the x-axis.
        /// </summary>
        /// <param name="rotationMatrix">The transformation matrix that will contain the result.</param>
        /// <param name="rotation">The rotation to apply around x-axis.</param>
        /// <param name="isInDegrees">True if the rotation is in degrees.</param>
        private static void ApplyXRotation(ref double[,] rotationMatrix, double rotation, bool isInDegrees = false)
        {
            if (isInDegrees)
            {
                rotation = rotation * Math.PI / 180;
            }

            double[,] r = Autodesk.Matrix.Matrix.Identity(3);

            double sinRot = Math.Sin(rotation);
            double cosRot = Math.Cos(rotation);

            r[1, 1] = cosRot;
            r[1, 2] = -sinRot;
            r[2, 1] = sinRot;
            r[2, 2] = cosRot;

            double[,] mTemp = rotationMatrix.Copy();
            rotationMatrix = r.Multiply(mTemp);
        }

        #endregion
    }
}