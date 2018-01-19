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
    /// Encapsulates length measured in millimetres and the operations thereon.
    /// </summary>
    [DebuggerDisplay("{Value}")]
    [Serializable]
    public struct MM : IFormattable, IComparable
    {
        #region "Fields"

        /// <summary>
        /// This is the actual length in MM
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private double _length;

        #endregion

        #region "Constructors"

        /// <summary>
        /// Constructs an MM object and initialises it to the specified length.
        /// </summary>
        /// <param name="length">Initial value of the structure.</param>
        internal MM(double length)
        {
            _length = length;
        }

        #endregion

        #region "Properties"

        /// <summary>
        /// Length in MM.
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
        /// Returns the sum of two MM objects.
        /// </summary>
        /// <param name="left">Left value to sum.</param>
        /// <param name="right">Right value to sum.</param>
        /// <returns>Sum of the two MM operands.</returns>
        public static MM operator +(MM left, MM right)
        {
            return new MM(left.Value + right.Value);
        }

        /// <summary>
        /// Adds an <see cref="MM"/> and a <see cref="int"/>.
        /// </summary>
        /// <param name="left">Left value to sum.</param>
        /// <param name="right">Right value to sum.</param>
        /// <returns>Sum of the add operation.</returns>
        public static MM operator +(MM left, int right)
        {
            return new MM(left.Value + right);
        }

        /// <summary>
        /// Adds a <see cref="int"/> and an <see cref="MM"/>.
        /// </summary>
        /// <param name="left">Left value to sum.</param>
        /// <param name="right">Right value to sum.</param>
        /// <returns>Sum of the add operation.</returns>
        public static MM operator +(int left, MM right)
        {
            return new MM(left + right.Value);
        }

        /// <summary>
        /// Subtracts one MM object from another.
        /// </summary>
        /// <param name="left">Object from which to subtract (minuend).</param>
        /// <param name="right">Object to subtract (subtrahend).</param>
        /// <returns>MM object containing the result.</returns>
        public static MM operator -(MM left, MM right)
        {
            return new MM(left.Value - right.Value);
        }

        /// <summary>
        /// Subtracts a <see cref="int"/> from one <see cref="MM"/> object.
        /// </summary>
        /// <param name="left">Object from which to subtract (minuend).</param>
        /// <param name="right">Object to subtract (subtrahend).</param>
        /// <returns>MM object containing the result.</returns>
        public static MM operator -(MM left, int right)
        {
            return new MM(left.Value - right);
        }

        /// <summary>
        /// Subtracts one <see cref="MM"/> object from a <see cref="int"/>.
        /// </summary>
        /// <param name="left">Object from which to subtract (minuend).</param>
        /// <param name="right">Object to subtract (subtrahend).</param>
        /// <returns>MM object containing the result.</returns>
        public static MM operator -(int left, MM right)
        {
            return new MM(left - right.Value);
        }

        /// <summary>
        /// Multiplies two MM objects together.
        /// </summary>
        /// <param name="left">First of two MM objects.</param>
        /// <param name="right">Second of two MM objects.</param>
        /// <returns>Product of the multiplication.</returns>
        public static double operator *(MM left, MM right)
        {
            return left.Value * right.Value;
        }

        /// <summary>
        /// Multiplies a MM by a double.
        /// </summary>
        /// <param name="left">A dobule scalar.</param>
        /// <param name="right">The MM object.</param>
        /// <returns>Product of the multiplication.</returns>
        public static MM operator *(double left, MM right)
        {
            return new MM(left * right.Value);
        }

        /// <summary>
        /// Multiplies a MM by a double.
        /// </summary>
        /// <param name="left">The MM object.</param>
        /// <param name="right">A dobule scalar.</param>
        /// <returns>Product of the multiplication.</returns>
        public static MM operator *(MM left, double right)
        {
            return new MM(left.Value * right);
        }

        /// <summary>
        /// Multiplies a <see cref="MM"/> by a <see cref="double"/>.
        /// </summary>
        /// <param name="left">A int scalar.</param>
        /// <param name="right">The MM object.</param>
        /// <returns>Product of the multiplication.</returns>
        public static MM operator *(int left, MM right)
        {
            return new MM(left * right.Value);
        }

        /// <summary>
        /// Multiplies a <see cref="MM"/> by a <see cref="int"/>.
        /// </summary>
        /// <param name="left">The MM object.</param>
        /// <param name="right">A int scalar.</param>
        /// <returns>Product of the multiplication.</returns>
        public static MM operator *(MM left, int right)
        {
            return new MM(left.Value * right);
        }

        /// <summary>
        /// Divide the left-hand MM object by the right.
        /// </summary>
        /// <param name="left">MM object to divide (dividend).</param>
        /// <param name="right">MM object to divide with (divisor).</param>
        /// <returns>Result of the division.</returns>
        public static double operator /(MM left, MM right)
        {
            return left.Value / right.Value;
        }

        /// <summary>
        /// Divide the left-hand MM object by a double.
        /// </summary>
        /// <param name="left">MM object to divide (dividend).</param>
        /// <param name="right">Double to divide by (divisor).</param>
        /// <returns>Result of the division.</returns>
        public static MM operator /(MM left, double right)
        {
            return left.Value / right;
        }

        /// <summary>
        /// Divide the left-hand <see cref="MM"/> object by a <see cref="int"/>.
        /// </summary>
        /// <param name="left">MM object to divide (dividend).</param>
        /// <param name="right">Int to divide by (divisor).</param>
        /// <returns>Result of the division.</returns>
        public static MM operator /(MM left, int right)
        {
            return left.Value / right;
        }

        /// <summary>
        /// Divide the left-hand <see cref="int"/> object by a <see cref="MM"/>.
        /// </summary>
        /// <param name="left">Int to divide (dividend).</param>
        /// <param name="right">MM object to divide by (divisor).</param>
        /// <returns>Result of the division.</returns>
        public static double operator /(int left, MM right)
        {
            return left / right.Value;
        }

        /// <summary>
        /// Negates the MM object to the right of the operator.
        /// </summary>
        /// <param name="right">MM object to negate.</param>
        /// <returns>MM object containing the result of the operation.</returns>
        public static MM operator -(MM right)
        {
            return new MM(-right.Value);
        }

        /// <summary>
        /// Determines whether one MM object is less than another.
        /// </summary>
        /// <param name="left">First MM object.</param>
        /// <param name="right">Second MM object.</param>
        /// <returns>True if the left-hand MM object is less than the right; false otherwise.</returns>
        public static bool operator <(MM left, MM right)
        {
            return left.Value < right.Value;
        }

        /// <summary>
        /// Determines whether one MM object is greater than another.
        /// </summary>
        /// <param name="left">First MM object.</param>
        /// <param name="right">Second MM object.</param>
        /// <returns>True if the left-hand MM object is greater than the right; false otherwise.</returns>
        public static bool operator >(MM left, MM right)
        {
            return left.Value > right.Value;
        }

        /// <summary>
        /// Determines whether one MM object is less than or equal to another.
        /// </summary>
        /// <param name="left">First MM object.</param>
        /// <param name="right">Second MM object.</param>
        /// <returns>True if the left-hand MM object is less than or equal to the right; false otherwise.</returns>
        public static bool operator <=(MM left, MM right)
        {
            return left.Value < right.Value || left == right;
        }

        /// <summary>
        /// Determines whether one MM object is greater than or equal to another.
        /// </summary>
        /// <param name="left">First MM object.</param>
        /// <param name="right">Second MM object.</param>
        /// <returns>True if the left-hand MM object is greater than or equal to the right; false otherwise.</returns>
        public static bool operator >=(MM left, MM right)
        {
            return left.Value > right.Value || left == right;
        }

        /// <summary>
        /// Returns True if the two specified MM objects are equal.
        /// </summary>
        /// <param name="left">First of two MM objects.</param>
        /// <param name="right">Second of two MM objects.</param>
        /// <returns>True if the operands are equivalent in magnitude; false otherwise.</returns>
        public static bool operator ==(MM left, MM right)
        {
            return Math.Abs(left.Value - right.Value) < 1E-06;
        }

        /// <summary>
        /// Compares two MM objects and returns True if they are unequal.
        /// </summary>
        /// <param name="left">First of two MM objects.</param>
        /// <param name="right">Second of two MM objects.</param>
        /// <returns>True if the operands are unequal in magnitude; false otherwise.</returns>
        public static bool operator !=(MM left, MM right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Returns the string result of concatenating the string representation of the specified MM object with the specified string.
        /// The MM object is added to the result first.
        /// </summary>
        /// <param name="left">First of two operands.</param>
        /// <param name="right">Second of two operands.</param>
        /// <returns>Result of the concatenation.</returns>
        public static string operator +(MM left, string right)
        {
            return left.ToString() + right;
        }

        /// <summary>
        /// Returns the result of concatenating the specified string with the string representation of the specified MM object.
        /// The specified string is added to the result first.
        /// </summary>
        /// <param name="left">First of two operands.</param>
        /// <param name="right">Second of two operands.</param>
        /// <returns>Result of the concatenation.</returns>
        public static string operator +(string left, MM right)
        {
            return left + right.ToString();
        }

        #endregion

        #region "CType Operators"

        /// <summary>
        /// Casts an MM object to a Double.
        /// </summary>
        /// <param name="left">MM object to cast.</param>
        /// <returns>Double representation of the MM object.</returns>
        public static implicit operator double(MM left)
        {
            return left.Value;
        }

        /// <summary>
        /// Casts an MM object to Single.
        /// </summary>
        /// <param name="left">MM object to cast.</param>
        /// <returns>Single representation of the MM object.</returns>
        public static explicit operator float(MM left)
        {
            return (float) left.Value;
        }

        /// <summary>
        /// Casts an MM object to Decimal.
        /// </summary>
        /// <param name="left">MM object to cast.</param>
        /// <returns>Decimal representation of the MM object.</returns>
        public static explicit operator decimal(MM left)
        {
            return (decimal) left.Value;
        }

        /// <summary>
        /// Casts an MM object to int.
        /// </summary>
        /// <param name="left">MM object to cast.</param>
        /// <returns>Decimal representation of the MM object.</returns>
        public static explicit operator int(MM left)
        {
            return (int) left.Value;
        }

        /// <summary>
        /// Casts an Inch object to a MM object.
        /// </summary>
        /// <param name="left">Inch object to cast.</param>
        /// <returns>New MM object containing the result of the operation.</returns>
        public static implicit operator MM(Inch left)
        {
            return new MM(left.Value * 25.4);
        }

        /// <summary>
        /// Casts a Double to an MM object.
        /// </summary>
        /// <param name="left">Double to be cast.</param>
        /// <returns>New MM object containing the result of the operation.</returns>
        public static implicit operator MM(double left)
        {
            return new MM(left);
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
            if (obj is MM)
            {
                return this == (MM) obj;
            }
            if (obj is Inch)
            {
                MM objAsMM = (Inch) obj;
                return this == objAsMM;
            }
            return Value == Convert.ToDouble(obj);
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
                MM objAsMM = (MM) obj;
                double roundedObj = Math.Round(objAsMM.Value, nDecPts);
                double roundedMe = Math.Round(Value, nDecPts);
                return roundedObj == roundedMe;
            }
            if (obj is Inch)
            {
                MM objAsMM = (Inch) obj;
                double roundedObj = Math.Round(objAsMM.Value, nDecPts);
                double roundedMe = Math.Round(Value, nDecPts);
                return roundedObj == roundedMe;
            }
            return false;
        }

        /// <summary>
        /// Required for MM to implement IComparable.
        /// </summary>
        /// <param name="obj">Object to compare.</param>
        /// <exception cref="ArgumentException">Thrown if the object to compare is not of type MM.</exception>
        public int CompareTo(object obj)
        {
            if (!ReferenceEquals(obj.GetType(), GetType()))
            {
                throw new ArgumentException("Object to compare is not a MM");
            }

            return Value.CompareTo(((MM) obj).Value);
        }

        #endregion
    }
}