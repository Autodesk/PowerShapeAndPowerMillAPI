// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using System.Diagnostics;

namespace Autodesk.Geometry
{
    /// <summary>
    /// Encapsulates length measured in inches and the operations thereon.
    /// </summary>
    [DebuggerDisplay("{Value}")]
    [Serializable]
    public struct Inch : IFormattable
    {
        #region "Fields"

        /// <summary>
        /// This is the actual value of the length
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private double _length;

        #endregion

        #region "Constructors"

        /// <summary>
        /// Constructs an Inch object and initialises it to the specified length.
        /// </summary>
        /// <param name="length">Initial value of the structure.</param>
        public Inch(double length)
        {
            _length = length;
        }

        #endregion

        #region "Properties"

        /// <summary>
        /// Length in Inches.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public double Value
        {
            get { return _length; }

            set { _length = value; }
        }

        #endregion

        #region "Operators"

        /// <summary>
        /// Returns the sum ot two Inch objects.
        /// </summary>
        /// <param name="left">Left value to sum.</param>
        /// <param name="right">Right value to sum.</param>
        /// <returns>Sum of the two Inch operands.</returns>
        public static Inch operator +(Inch left, Inch right)
        {
            return new Inch(left.Value + right.Value);
        }

        /// <summary>
        /// Subtracts one Inch object from another.
        /// </summary>
        /// <param name="left">Object from which to subtract (minuend).</param>
        /// <param name="right">Object to subtract (subtrahend).</param>
        /// <returns>Inch object containing the result.</returns>
        public static Inch operator -(Inch left, Inch right)
        {
            return new Inch(left.Value - right.Value);
        }

        /// <summary>
        /// Multiplies two Inch objects together.
        /// </summary>
        /// <param name="left">First of two Inch objects.</param>
        /// <param name="right">Second of two Inch objects.</param>
        /// <returns>Product of the multiplication.</returns>
        public static double operator *(Inch left, Inch right)
        {
            return left.Value * right.Value;
        }

        /// <suInchary>
        /// Multiplies a Inch by a double.
        /// </suInchary>
        /// <param name="left">A dobule scalar.</param>
        /// <param name="right">The Inch object.</param>
        /// <returns>Product of the multiplication.</returns>
        public static Inch operator *(double left, Inch right)
        {
            return new Inch(left * right.Value);
        }

        /// <suInchary>
        /// Multiplies a Inch by a double.
        /// </suInchary>
        /// <param name="left">The Inch object.</param>
        /// <param name="right">A dobule scalar.</param>
        /// <returns>Product of the multiplication.</returns>
        public static Inch operator *(Inch left, double right)
        {
            return new Inch(left.Value * right);
        }

        /// <summary>
        /// Divides one Inch object by another.
        /// </summary>
        /// <param name="left">Inch object to divide (dividend).</param>
        /// <param name="right">Inch object to divide with (divisor).</param>
        /// <returns>Quotient resulting from the operation.</returns>
        public static double operator /(Inch left, Inch right)
        {
            return left.Value / right.Value;
        }

        /// <suInchary>
        /// Divide the left-hand Inch object by a double.
        /// </suInchary>
        /// <param name="left">Inch object to divide (dividend).</param>
        /// <param name="right">Double to divide by (divisor).</param>
        /// <returns>Result of the division.</returns>
        public static Inch operator /(Inch left, double right)
        {
            return new Inch(left.Value / right);
        }

        /// <summary>
        /// Negates the Inch object to the right of the operator.
        /// </summary>
        /// <param name="right">Inch object to negate.</param>
        /// <returns>Inch object containing the result of the operation.</returns>
        public static Inch operator -(Inch right)
        {
            return new Inch(-right.Value);
        }

        /// <summary>
        /// Determines whether one Inch object is less than another.
        /// </summary>
        /// <param name="left">First Inch object.</param>
        /// <param name="right">Second Inch object.</param>
        /// <returns>True if the left-hand Inch object is less than the right; false otherwise.</returns>
        public static bool operator <(Inch left, Inch right)
        {
            return left.Value < right.Value;
        }

        /// <summary>
        /// Determines whether one Inch object is greater than another.
        /// </summary>
        /// <param name="left">First Inch object.</param>
        /// <param name="right">Second Inch object.</param>
        /// <returns>True if the left-hand Inch object is greater than the right; false otherwise.</returns>
        public static bool operator >(Inch left, Inch right)
        {
            return left.Value > right.Value;
        }

        /// <summary>
        /// Determines whether one Inch object is less than or equal to another.
        /// </summary>
        /// <param name="left">First Inch object.</param>
        /// <param name="right">Second Inch object.</param>
        /// <returns>True if the left-hand Inch object is less than or equal to the right; false otherwise.</returns>
        public static bool operator <=(Inch left, Inch right)
        {
            return left.Value <= right.Value || left == right;
        }

        /// <summary>
        /// Determines whether one Inch object is greater than or equal to another.
        /// </summary>
        /// <param name="left">First Inch object.</param>
        /// <param name="right">Second Inch object.</param>
        /// <returns>True if the left-hand Inch object is greater than or equal to the right; false otherwise.</returns>
        public static bool operator >=(Inch left, Inch right)
        {
            return left.Value >= right.Value || left == right;
        }

        /// <summary>
        /// Compares two Inch objects for equivalence and returns true if they are within
        /// one 100,000th of an inch.
        /// </summary>
        /// <param name="left">First of two Inch objects.</param>
        /// <param name="right">Second of two Inch objects.</param>
        /// <returns>True if the operands are considered equivalent; false otherwise.</returns>
        public static bool operator ==(Inch left, Inch right)
        {
            return Math.Abs(left.Value - right.Value) < 1E-06;
        }

        /// <summary>
        /// Compares two Inch objects and returns True if the difference between them
        /// is greater than one 100,000th of an inch. I.e. they are unequal.
        /// </summary>
        /// <param name="left">First of two Inch objects.</param>
        /// <param name="right">Second of two Inch objects.</param>
        /// <returns>True if the operands are considered unequal; false otherwise.</returns>
        public static bool operator !=(Inch left, Inch right)
        {
            return Math.Abs(left.Value - right.Value) >= 1E-05;
        }

        #endregion

        #region "CType Operators"

        /// <summary>
        /// Casts an Inch object to Double.
        /// </summary>
        /// <param name="left">Inch object to cast.</param>
        /// <returns>Double representation of the Inch object.</returns>
        public static explicit operator double(Inch left)
        {
            return left.Value;
        }

        /// <summary>
        /// Casts an Inch object to Single.
        /// </summary>
        /// <param name="left">Inch object to cast.</param>
        /// <returns>Single representation of the Inch object.</returns>
        public static explicit operator float(Inch left)
        {
            return (float) left.Value;
        }

        /// <summary>
        /// Casts an Inch object to Decimal.
        /// </summary>
        /// <param name="left">Inch object to cast.</param>
        /// <returns>Decimal representation of the Inch object.</returns>
        public static explicit operator decimal(Inch left)
        {
            return (decimal) left.Value;
        }

        /// <summary>
        /// Casts a MM (millimetre) object to an Inch object.
        /// </summary>
        /// <param name="left">MM object to cast.</param>
        /// <returns>New Inch object containing the result of the operation.</returns>
        public static implicit operator Inch(MM left)
        {
            return new Inch(left / 25.4);
        }

        /// <summary>
        /// Casts a Double to an Inch object.
        /// </summary>
        /// <param name="left">Double to be cast.</param>
        /// <returns>New Inch object containing the result of the operation.</returns>
        public static implicit operator Inch(double left)
        {
            return new Inch(left);
        }

        #endregion

        #region "Operations"

        /// <summary>
        /// Returns the value of this object as a string.
        /// </summary>
        public override string ToString()
        {
            // This is the most accuracy that PowerShape and PowerMill accept
            return ToString("0.######", null);
        }

        /// <summary>
        /// Returns the value of this object as a formatted string.
        /// </summary>
        /// <param name="format">Standard format specifier with which to format the string.</param>
        public string ToString(string format)
        {
            return ToString(format, null);
        }

        /// <summary>
        /// Returns the value of this object in its equivalent string representation using the specified format and culture-specific format information.
        /// </summary>
        /// <param name="format">Standard numeric format specifier.</param>
        /// <param name="fp">Custom object implementing the IFormatProvider interface.</param>
        public string ToString(string format, IFormatProvider fp)
        {
            return Value.ToString(format, fp);
        }

        /// <summary>
        /// Returns True if the magnitude of this object is equivalent to that of the specified object.
        /// If the specified object is neither of type MM or Inch, false will be returned.
        /// </summary>
        /// <param name="obj">Object with which to compare this.</param>
        /// <returns>True if the magnitudes of this and the specified object are equivalent; false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Inch)
            {
                return this == (Inch) obj;
            }
            if (obj is MM)
            {
                Inch objAsMM = (MM) obj;
                return this == objAsMM;
            }
            return Value == (double) obj;
        }

        /// <summary>
        /// Returns True if the magnitude of this object is equivalent to that of the specified object to the specified number of decimal places.
        /// If the specified object is neither of type MM or Inch, false will be returned.
        /// </summary>
        /// <param name="obj">Object with which to compare this.</param>
        /// <param name="nDecPts">Number of decimal places to compare.</param>
        /// <returns>True if the magnitudes of this and the specified object are considered equivalent; false otherwise.</returns>
        public bool Equals(object obj, int nDecPts)
        {
            if (obj is MM)
            {
                Inch objAsInch = (MM) obj;
                double roundedObj = Math.Round(objAsInch.Value, nDecPts);
                double roundedMe = Math.Round(Value, nDecPts);
                return roundedObj == roundedMe;
            }
            if (obj is Inch)
            {
                Inch objAsInch = (Inch) obj;
                double roundedObj = Math.Round(objAsInch.Value, nDecPts);
                double roundedMe = Math.Round(Value, nDecPts);
                return roundedObj == roundedMe;
            }
            return false;
        }

        #endregion
    }
}