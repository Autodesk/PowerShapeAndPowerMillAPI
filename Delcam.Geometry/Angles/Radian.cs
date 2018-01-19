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
    /// Structure encapsulating all operations for manipulating angles in radians.
    /// </summary>
    [DebuggerDisplay("{Value}")]
    [Serializable]
    public struct Radian : IFormattable, IComparable
    {
        #region "Fields"

        /// <summary>
        /// This is the actual angle in radians
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private double _angle;

        #endregion

        #region "Constructors"

        /// <summary>
        /// Constructs a Radian structure and initialises the angle to the specified number of radians.
        /// </summary>
        /// <param name="angle">Initial angle.</param>
        public Radian(double angle)
        {
            _angle = angle;
        }

        #endregion

        #region "Properties"

        /// <summary>
        /// Value of the angle in radians.
        /// </summary>
        /// <value>The angle in radians.</value>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public double Value
        {
            get { return _angle; }

            set { _angle = value; }
        }

        #endregion

        #region "Operators"

        /// <summary>
        /// Add one Radian object to another.
        /// </summary>
        /// <param name="left">First of two Radian objects to be summed.</param>
        /// <param name="right">Second of two Radian objects to be summed.</param>
        /// <returns>New Radian object containing the result of the operation.</returns>
        public static Radian operator +(Radian left, Radian right)
        {
            return new Radian(left.Value + right.Value);
        }

        /// <summary>
        /// Subtract one Radian object from another.
        /// </summary>
        /// <param name="left">Radian object from which to subtract (minuend).</param>
        /// <param name="right">Radian object to subtract (subtrahend).</param>
        /// <returns>New Radian object containing the result of the operation.</returns>
        public static Radian operator -(Radian left, Radian right)
        {
            return new Radian(left.Value - right.Value);
        }

        /// <summary>
        /// Multiplication of two Radian objects.
        /// </summary>
        /// <param name="left">First of two Radian objects to be multiplied (multiplicand).</param>
        /// <param name="right">Second of two Radian objects to be multiplied (multiplier).</param>
        /// <returns>New Radian object containing the result of the operation.</returns>
        public static Radian operator *(Radian left, Radian right)
        {
            return new Radian(left.Value * right.Value);
        }

        /// <summary>
        /// Multiplication of a double by a Radian object.
        /// </summary>
        /// <param name="left">First of two parameters to multiply (multiplicand).</param>
        /// <param name="right">Second of two parameters to multiply (multiplier).</param>
        /// <returns>New Radian object containing the result of the operation.</returns>
        public static Radian operator *(double left, Radian right)
        {
            return new Radian(left * right.Value);
        }

        /// <summary>
        /// Multiplication of a Radian object by a double.
        /// </summary>
        /// <param name="left">First of two parameters to multiply (multiplicand).</param>
        /// <param name="right">Second of two parameters to multiply (multiplier).</param>
        /// <returns>New Radian object containing the result of the operation.</returns>
        public static Radian operator *(Radian left, double right)
        {
            return new Radian(left.Value * right);
        }

        /// <summary>
        /// Division of one Radian object by another.
        /// </summary>
        /// <param name="left">Radian object to be divided (dividend).</param>
        /// <param name="right">Radian object to divide with (divisor).</param>
        /// <returns>New Radian object containing the result of the operation.</returns>
        public static Radian operator /(Radian left, Radian right)
        {
            return new Radian(left.Value / right.Value);
        }

        /// <summary>
        /// Division of one Radian object by a double.
        /// </summary>
        /// <param name="left">Radian object to be divided (dividend).</param>
        /// <param name="right">Double to divide by (divisor).</param>
        /// <returns>New Radian object containing the result of the operation.</returns>
        public static Radian operator /(Radian left, double right)
        {
            return new Radian(left.Value / right);
        }

        /// <summary>
        /// Negates the current value of the angle.
        /// </summary>
        /// <param name="right">Radian object to negate.</param>
        /// <returns>New Radian object containing the result of the operation.</returns>
        public static Radian operator -(Radian right)
        {
            return new Radian(-right.Value);
        }

        /// <summary>
        /// Determine whether the left-hand Radian object is less than the right.
        /// </summary>
        /// <param name="left">First of two Radian objects to compare.</param>
        /// <param name="right">Second of two Radian objects to compare.</param>
        /// <returns>Returns True if the left-hand Radian object is less than the right; false otherwise.</returns>
        public static bool operator <(Radian left, Radian right)
        {
            return left.Value < right.Value;
        }

        /// <summary>
        /// Determine whether the left-hand Radian object is greater than the right.
        /// </summary>
        /// <param name="left">First of two Radian objects to compare.</param>
        /// <param name="right">Second of two Radian objects to compare.</param>
        /// <returns>Returns True if the left-hand Radian object is greater than the right; false otherwise.</returns>
        public static bool operator >(Radian left, Radian right)
        {
            return left.Value > right.Value;
        }

        /// <summary>
        /// Determine whether the left-hand Radian object is less than or equal to the right.
        /// </summary>
        /// <param name="left">First of two Radian objects to compare.</param>
        /// <param name="right">Second of two Radian objects to compare.</param>
        /// <returns>Returns True if the left-hand Radian object is less than or equal to the right; false otherwise.</returns>
        public static bool operator <=(Radian left, Radian right)
        {
            return left.Value < right.Value || left == right;
        }

        /// <summary>
        /// Determine whether the left-hand Radian object is greater than or equal to the right.
        /// </summary>
        /// <param name="left">First of two Radian objects to compare.</param>
        /// <param name="right">Second of two Radian objects to compare.</param>
        /// <returns>Returns True if the left-hand Radian object is greater than or equal to the right; false otherwise.</returns>
        public static bool operator >=(Radian left, Radian right)
        {
            return left.Value > right.Value || left == right;
        }

        /// <summary>
        /// Determine whether two Radian objects are equal.
        /// </summary>
        /// <param name="left">First of two Radian objects to compare.</param>
        /// <param name="right">Second of two Radian objects to compare.</param>
        /// <returns>Returns True if the two Radian objects are equivalent; false otherwise.</returns>
        public static bool operator ==(Radian left, Radian right)
        {
            return Math.Abs(left.Value - right.Value) < 1E-06;
        }

        /// <summary>
        /// Determine whether two Radian objects are not equal.
        /// </summary>
        /// <param name="left">First of two Radian objects to compare.</param>
        /// <param name="right">Second of two Radian objects to compare.</param>
        /// <returns>Returns True if the two Radian objects are unquivalent; false otherwise.</returns>
        public static bool operator !=(Radian left, Radian right)
        {
            return !(left.Value == right.Value);
        }

        /// <summary>
        /// Concatenates the string representation of a Radian object with a string.
        /// </summary>
        /// <param name="left">Radian object with which to concatenate a string.</param>
        /// <param name="right">String to concatenate .</param>
        /// <returns>Returns a string containing the result of the concatenation.</returns>
        public static string operator +(Radian left, string right)
        {
            return left.ToString() + right;
        }

        /// <summary>
        /// Concatenates a string with the string representation of a Radian object.
        /// </summary>
        /// <param name="left">String with which to concatenate a Radian object.</param>
        /// <param name="right">Radian object to concatenate .</param>
        /// <returns>Returns a string containing the result of the concatenation.</returns>
        public static string operator +(string left, Radian right)
        {
            return left + right.ToString();
        }

        #endregion

        #region "CType Operators"

        /// <summary>
        /// Cast a Radian object to double.
        /// </summary>
        /// <param name="left">Radian object to cast.</param>
        /// <returns>Double representation of the Radian object.</returns>
        public static implicit operator double(Radian left)
        {
            return left.Value;
        }

        /// <summary>
        /// Cast a Radian object to Single.
        /// </summary>
        /// <param name="left">Radian object to cast.</param>
        /// <returns>Single representation of the Radian object.</returns>
        public static explicit operator float(Radian left)
        {
            return (float) left.Value;
        }

        /// <summary>
        /// Cast a Radian object to Decimal.
        /// </summary>
        /// <param name="left">Radian object to cast.</param>
        /// <returns>Decimal representation of the Radian object.</returns>
        public static explicit operator decimal(Radian left)
        {
            return (decimal) left.Value;
        }

        /// <summary>
        /// Cast a Degree object to a Radian object.
        /// </summary>
        /// <param name="left">Degree object to cast.</param>
        /// <returns>Radian representation of the Degree object.</returns>
        public static implicit operator Radian(Degree left)
        {
            return new Radian(left.Value / 180.0 * Math.PI);
        }

        /// <summary>
        /// Cast a double to a Radian object.
        /// </summary>
        /// <param name="left">Double to cast.</param>
        /// <returns>New Radian object containing the result of the operation.</returns>
        public static implicit operator Radian(double left)
        {
            return new Radian(left);
        }

        #endregion

        #region "Operations"

        /// <summary>
        /// Returns a string representation of the value in radians.
        /// </summary>
        /// <returns>String representation of the value in radians.</returns>
        public override string ToString()
        {
            // This is the most accuracy that PowerShape and PowerMill accept
            return ToString("0.######", null);
        }

        /// <summary>
        /// Returns a formatted string representation of the value in radians.
        /// </summary>
        /// <param name="format">Standard numeric fornat specifier.</param>
        /// <returns>String representation of the value in radians.</returns>
        public string ToString(string format)
        {
            return ToString(format, null);
        }

        /// <summary>
        /// Returns the numeric value of this instance in its equivalent string representation using the specified culture-specific format information.
        /// </summary>
        /// <param name="fp">Custom object implementing the IFormatProvider interface.</param>
        /// <returns>String representation of the value in radians.</returns>
        public string ToString(IFormatProvider fp)
        {
            return Value.ToString(fp);
        }

        /// <summary>
        /// Returns the numeric value of this instance in its equivalent string representation using the specified format and culture-specific format information.
        /// </summary>
        /// <param name="format">Standard numeric fornat specifier.</param>
        /// <param name="fp">Custom object implementing the IFormatProvider interface.</param>
        /// <returns>String representation of the value in radians.</returns>
        public string ToString(string format, IFormatProvider fp)
        {
            return Value.ToString(format, fp);
        }

        /// <summary>
        /// Returns True if the magnitude of this object is equivalent to that of the specified object.
        /// If the specified object is neither of type Degree or Radian, false will be returned.
        /// </summary>
        /// <param name="obj">Object with which to compare this.</param>
        /// <returns>True if the magnitudes of this and the specified object are equivalent; false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Radian)
            {
                return this == (Radian) obj;
            }
            if (obj is Degree)
            {
                Radian objAsRadian = (Degree) obj;
                return this == objAsRadian;
            }
            return Value == (double) obj;
        }

        /// <summary>
        /// Returns True if the magnitude of this object is equivalent to that of the specified object to the specified number of decimal places.
        /// If the specified object is neither of type Degree or Radian, false will be returned.
        /// </summary>
        /// <param name="obj">Object with which to compare this.</param>
        /// <param name="nDecPts">Number of decimal places to compare.</param>
        /// <returns>True if the magnitudes of this and the specified object are considered equivalent; false otherwise.</returns>
        public bool Equals(object obj, int nDecPts)
        {
            if (obj is Degree)
            {
                Radian objAsRadian = (Degree) obj;
                double roundedObj = Math.Round(objAsRadian.Value, nDecPts);
                double roundedMe = Math.Round(Value, nDecPts);
                return roundedObj == roundedMe;
            }
            if (obj is Radian)
            {
                Radian objAsRadian = (Radian) obj;
                double roundedObj = Math.Round(objAsRadian.Value, nDecPts);
                double roundedMe = Math.Round(Value, nDecPts);
                return roundedObj == roundedMe;
            }
            return false;
        }

        /// <summary>
        /// Determines whether the passed argument is before or after this one in logical sort order.
        /// </summary>
        /// <param name="obj">Passed object to compare. Must be of type Radian.</param>
        /// <returns>
        /// Integer detailing the relative ordering of the two objects in the sort order: If zero, both
        /// occur in the same position; negative and this object preceeds the passed argument and, if positive,
        /// this object comes after the passed argument.
        /// </returns>
        /// <exception cref="ArgumentException">Thrown if the object to compare is not of type Radian.</exception>
        public int CompareTo(object obj)
        {
            if (!ReferenceEquals(obj.GetType(), GetType()))
            {
                throw new ArgumentException("Object to compare is not a Radian");
            }

            return Value.CompareTo(((Radian) obj).Value);
        }

        #endregion
    }
}