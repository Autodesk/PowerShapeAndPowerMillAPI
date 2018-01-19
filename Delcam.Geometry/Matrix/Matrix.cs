// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;

namespace Autodesk.Matrix
{
    /// <summary>
    /// Contains Matrix helper methods.
    /// </summary>
    public static class Matrix
    {
        /// <summary>
        /// Creates a matrix of a given size with a default value.
        /// </summary>
        /// <param name="numberOfRows">The number of rows.</param>
        /// <param name="numberOfColumns">The number of columns.</param>
        /// <param name="value">The default value for each matrix element.</param>
        /// <returns>Returns the matrix.</returns>
        public static double[,] Create(int numberOfRows, int numberOfColumns, double value)
        {
            double[,] m = new double[numberOfRows, numberOfColumns];

            for (int i = 0; i <= numberOfColumns - 1; i++)
            {
                for (int j = 0; j <= numberOfRows - 1; j++)
                {
                    m[j, i] = value;
                }
            }

            return m;
        }

        /// <summary>
        /// Creates an Identity matrix.
        /// </summary>
        /// <param name="size">The size of the Identity matrix.</param>
        /// <returns>The Identity matrix.</returns>
        public static double[,] Identity(int size)
        {
            double[,] m = new double[size, size];

            for (int i = 0; i <= size - 1; i++)
            {
                for (int j = 0; j <= size - 1; j++)
                {
                    if (i == j)
                    {
                        m[j, i] = 1;
                    }
                }
            }

            return m;
        }

        /// <summary>
        /// Adds the current matrix (a) to matrix b.
        /// </summary>
        /// <param name="a">The current matrix.</param>
        /// <param name="b">Matrix to add.</param>
        /// <returns>The sum of matrix the current matrix and matrix b.</returns>
        public static double[,] Add(this double[,] a, double[,] b)
        {
            var rows = a.GetLength(0);
            var columns = b.GetLength(0);

            if (rows != b.GetLength(0))
            {
                throw new ArgumentException("The rows of matrix a must equal the rows of matrix b.");
            }

            if (columns != b.GetLength(1))
            {
                throw new ArgumentException("The columns of matrix a must equal the columns of matrix b.");
            }

            double[,] addition = new double[rows, columns];
            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < columns; ++j)
                {
                    addition[i, j] = a[i, j] + b[i, j];
                }
            }
            return addition;
        }

        /// <summary>
        /// Multiplies the current matrix (a) by b.
        /// </summary>
        /// <param name="a">The current matrix.</param>
        /// <param name="b">The matrix to multiply by.</param>
        /// <returns>The multiplication of matrix the current matrix by matrix b.</returns>
        public static double[,] Multiply(this double[,] a, double[,] b)
        {
            int aRows = a.GetLength(0);
            int aColumns = a.GetLength(1);
            int bRows = b.GetLength(0);
            int bColumns = b.GetLength(1);

            if (aColumns != bRows)
            {
                throw new ArgumentException(
                    "Multiplication is invalid. The number of columns in the current matrix must much the number of rows in matrix b.");
            }

            double[,] multiplication = new double[aRows, bColumns];

            for (int j = 0; j < aRows; j++)
            {
                for (int i = 0; i < bColumns; i++)
                {
                    double sum = 0;
                    for (int k = 0; k < aColumns; k++)
                    {
                        sum = sum + a[j, k] * b[k, i];
                    }
                    multiplication[j, i] = sum;
                }
            }

            return multiplication;
        }

        /// <summary>
        /// Multiplies the current matrix (a) by b.
        /// </summary>
        /// <param name="a">The current matrix.</param>
        /// <param name="b">The number to multiply by.</param>
        /// <returns>The multiplication of matrix the current matrix by b.</returns>
        public static double[,] Multiply(this double[,] a, double b)
        {
            int aRows = a.GetLength(0);
            int aColumns = a.GetLength(1);

            double[,] multiplication = new double[aRows, aColumns];

            for (int j = 0; j < aRows; j++)
            {
                for (int i = 0; i < aColumns; i++)
                {
                    multiplication[j, i] = a[j, i] * b;
                }
            }

            return multiplication;
        }

        /// <summary>
        /// Multiplies the current matrix (a) by column vector.
        /// </summary>
        /// <param name="a">The current matrix.</param>
        /// <param name="columnVector">The column vector to multiply by.</param>
        /// <returns>The multiplication of matrix the current matrix by b.</returns>
        public static double[] Multiply(this double[,] a, double[] columnVector)
        {
            int aRows = a.GetLength(0);
            if (a.GetLength(1) != columnVector.Length)
            {
                throw new ArgumentException(
                    "Multiplication is invalid. The number of columns in the current matrix (a) must much the number of rows in the vector.");
            }

            double[] numArray = new double[aRows];
            for (int i = 0; i < aRows; ++i)
            {
                for (int j = 0; j < columnVector.Length; ++j)
                {
                    numArray[i] += a[i, j] * columnVector[j];
                }
            }
            return numArray;
        }

        /// <summary>
        /// Hadamard product of the current matrix by b.
        /// </summary>
        /// <param name="a">The current matrix.</param>
        /// <param name="b">The matrix to multiply by.</param>
        /// <returns>The Hadamard product of the current matrix by b.</returns>
        /// <exception cref="System.ArgumentException">Thrown when matrices don't have the same dimensions.</exception>
        public static double[,] HadamardProduct(this double[,] a, double[,] b)
        {
            int aRows = a.GetLength(0);
            int aColumns = a.GetLength(1);

            if (aRows != b.GetLength(0)
                || aColumns != b.GetLength(1))
            {
                throw new ArgumentException(
                    "Unable to calculate Hadamard Product. The two matrices must have the same dimensions.");
            }

            double[,] hadamardProduct = new double[aRows, aColumns];

            for (int j = 0; j < aRows; j++)
            {
                for (int i = 0; i < aColumns; i++)
                {
                    hadamardProduct[j, i] = a[j, i] * b[j, i];
                }
            }

            return hadamardProduct;
        }

        /// <summary>
        /// Copies matrix a.
        /// </summary>
        /// <param name="a">The matrix to copy from.</param>
        /// <returns>The new matrix copy.</returns>
        public static double[,] Copy(this double[,] a)
        {
            int rows = a.GetLength(0);
            int colums = a.GetLength(1);

            double[,] copy = new double[rows, colums];

            for (int i = 0; i < colums; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    copy[j, i] = a[j, i];
                }
            }

            return copy;
        }

        /// <summary>
        /// Subtracts matrix b to the current matrix (a).
        /// </summary>
        /// <param name="a">The current matrix.</param>
        /// <param name="b">The matrix to subtract.</param>
        /// <returns>The result of the subtraction.</returns>
        public static double[,] Subtract(this double[,] a, double[,] b)
        {
            if (a.GetLength(0) != b.GetLength(0))
            {
                throw new ArgumentException("Subtraction is invalid. The number of rows of the two matrices must be equal.");
            }

            if (a.GetLength(1) != b.GetLength(1))
            {
                throw new ArgumentException("Subtraction is invalid. The number of columns of the two matrices must be equal.");
            }

            int rows = a.GetLength(0);
            int cols = a.GetLength(1);

            double[,] subtraction = new double[rows, cols];

            for (int j = 0; j <= rows - 1; j++)
            {
                for (int i = 0; i <= cols - 1; i++)
                {
                    subtraction[j, i] = a[j, i] - b[j, i];
                }
            }

            return subtraction;
        }

        /// <summary>
        /// Gets the transpose of the matrix (a).
        /// </summary>
        /// <param name="a">The matrix to transpose from.</param>
        /// <returns>The transpose of the given matrix.</returns>
        public static double[,] Transpose(this double[,] a)
        {
            int rows = a.GetLength(0);
            int columns = a.GetLength(1);

            double[,] objArray = new double[columns, rows];
            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < columns; ++j)
                {
                    objArray[j, i] = a[i, j];
                }
            }
            return objArray;
        }
    }
}