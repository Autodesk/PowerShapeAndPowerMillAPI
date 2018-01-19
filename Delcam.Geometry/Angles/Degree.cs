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
    /// Structure encapsulating all operations for manipulating angles in degrees.
    /// </summary>
    [DebuggerDisplay("{Value}")]
    [Serializable]
    public struct Degree : IFormattable, IComparable
    {
        #region "Fields"

        /// <summary>
        /// This is the actual value of the angle
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private double _angle;

        #endregion

        #region "Constructors"

        /// <summary>
        /// Constructs a Degree structure and initialises the angle to the specified number of degrees
        /// </summary>
        /// <param name="angle">Angle in degrees.</param>
        public Degree(double angle)
        {
            _angle = angle;
        }

        #endregion

        #region "Properties"

        /// <summary>
        /// Value of the angle in degrees.
        /// </summary>
        /// <value>The angle in degrees.</value>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public double Value
        {
            get { return _angle; }
            set { _angle = value; }
        }

        #endregion

        #region "Operators"

        /// <summary>
        /// Negates the current value of the angle.
        /// </summary>
        /// <param name="right">Degree object to negate.</param>
        /// <returns>New Degree object containing the result of the operation.</returns>
        public static Degree operator -(Degree right)
        {
            return new Degree(-right.Value);
        }

        /// <summary>
        /// Subtract one Degree object from another.
        /// </summary>
        /// <param name="left">Degree object from which to subtract (minuend).</param>
        /// <param name="right">Degree object to subtract (subtrahend).</param>
        /// <returns>New Degree object containing the result of the operation.</returns>
        public static Degree operator -(Degree left, Degree right)
        {
            return new Degree(left.Value - right.Value);
        }

        /// <summary>
        /// Add one Degree object to another.
        /// </summary>
        /// <param name="left">First of two Degree objects to be summed.</param>
        /// <param name="right">Second of two Degree objects to be summed.</param>
        /// <returns>New Degree object containing the result of the operation.</returns>
        public static Degree operator +(Degree left, Degree right)
        {
            return new Degree(left.Value + right.Value);
        }

        /// <summary>
        /// Division of one Degree object by another.
        /// </summary>
        /// <param name="left">Degree object to be divided (dividend).</param>
        /// <param name="right">Degree object to divide with (divisor).</param>
        /// <returns>New Degree object containing the result of the operation.</returns>
        public static Degree operator /(Degree left, Degree right)
        {
            return new Degree(left.Value / right.Value);
        }

        /// <summary>
        /// Division of one Degree object by a double.
        /// </summary>
        /// <param name="left">Degree object to be divided (dividend).</param>
        /// <param name="right">Double to divide by (divisor).</param>
        /// <returns>New Degree object containing the result of the operation.</returns>
        public static Degree operator /(Degree left, double right)
        {
            return new Degree(left.Value / right);
        }

        /// <summary>
        /// Multiplication of two degrees objects.
        /// </summary>
        /// <param name="left">First of two Degree objects to be multiplied (multiplicand).</param>
        /// <param name="right">Second of two Degree objects to be multiplied (multiplier).</param>
        /// <returns>New Degree object containing the result of the operation.</returns>
        public static Degree operator *(Degree left, Degree right)
        {
            return new Degree(left.Value * right.Value);
        }

        /// <summary>
        /// Multiplication of a double by a Degree object.
        /// </summary>
        /// <param name="left">First of two parameters to multiply (multiplicand).</param>
        /// <param name="right">Second of two parameters to multiply (multiplier).</param>
        /// <returns>New Degree object containing the result of the operation.</returns>
        public static Degree operator *(double left, Degree right)
        {
            return new Degree(left * right.Value);
        }

        /// <summary>
        /// Multiplication of a Degree object by a double.
        /// </summary>
        /// <param name="left">First of two parameters to multiply (multiplicand).</param>
        /// <param name="right">Second of two parameters to multiply (multiplier).</param>
        /// <returns>New Degree object containing the result of the operation.</returns>
        public static Degree operator *(Degree left, double right)
        {
            return new Degree(left.Value * right);
        }

        /// <summary>
        /// Determine whether the left-hand Degree object is less than the right hand double.
        /// </summary>
        /// <param name="left">First of two parameters to compare.</param>
        /// <param name="right">Second of two parameters to compare.</param>
        /// <returns>Returns True if the Degree objects is less than the double; false otherwise.</returns>
        public static bool operator <(Degree left, double right)
        {
            return left.Value < right;
        }

        /// <summary>
        /// Determine whether the left-hand Degree object is greater than the right hand double.
        /// </summary>
        /// <param name="left">First of two parameters to compare.</param>
        /// <param name="right">Second of two parameters to compare.</param>
        /// <returns>Returns True if the Degree objects is greater than the double; false otherwise.</returns>
        public static bool operator >(Degree left, double right)
        {
            return left.Value > right;
        }

        /// <summary>
        /// Determine whether the left-hand Degree object is less than or equal to the right hand double.
        /// </summary>
        /// <param name="left">First of two parameters to compare.</param>
        /// <param name="right">Second of two parameters to compare.</param>
        /// <returns>Returns True if the Degree objects is less than or equal to the double; false otherwise.</returns>
        public static bool operator <=(Degree left, double right)
        {
            return left.Value <= right;
        }

        /// <summary>
        /// Determine whether the left-hand Degree object is greater than or equal to the right hand double.
        /// </summary>
        /// <param name="left">First of two parameters to compare.</param>
        /// <param name="right">Second of two parameters to compare.</param>
        /// <returns>Returns True if the Degree objects is greater than or equal to the double; false otherwise.</returns>
        public static bool operator >=(Degree left, double right)
        {
            return left.Value >= right;
        }

        /// <summary>
        /// Determine whether the left-hand Degree object is less than the right.
        /// </summary>
        /// <param name="left">First of two Degree objects to compare.</param>
        /// <param name="right">Second of two Degree objects to compare.</param>
        /// <returns>Returns True if the left-hand Degree object is less than the right; false otherwise.</returns>
        public static bool operator <(Degree left, Degree right)
        {
            return left.Value < right.Value;
        }

        /// <summary>
        /// Determine whether the left-hand Degree object is greater than the right.
        /// </summary>
        /// <param name="left">First of two Degree objects to compare.</param>
        /// <param name="right">Second of two Degree objects to compare.</param>
        /// <returns>Returns True if the left-hand Degree object is greater than the right; false otherwise.</returns>
        public static bool operator >(Degree left, Degree right)
        {
            return left.Value > right.Value;
        }

        /// <summary>
        /// Determine whether the left-hand Degree object is less than or equal to the right.
        /// </summary>
        /// <param name="left">First of two Degree objects to compare.</param>
        /// <param name="right">Second of two Degree objects to compare.</param>
        /// <returns>Returns True if the left-hand Degree object is less than or equal to the right; false otherwise.</returns>
        public static bool operator <=(Degree left, Degree right)
        {
            return left.Value <= right.Value || left == right;
        }

        /// <summary>
        /// Determine whether the left-hand Degree object is greater than or equal to the right.
        /// </summary>
        /// <param name="left">First of two Degree objects to compare.</param>
        /// <param name="right">Second of two Degree objects to compare.</param>
        /// <returns>Returns True if the left-hand Degree object is greater than or equal to the right; false otherwise.</returns>
        public static bool operator >=(Degree left, Degree right)
        {
            return left.Value >= right.Value || left == right;
        }

        /// <summary>
        /// Determine whether two Degree objects are equal.
        /// </summary>
        /// <param name="left">First of two Degree objects to compare.</param>
        /// <param name="right">Second of two Degree objects to compare.</param>
        /// <returns>Returns True if the two Degree objects are equivalent; false otherwise.</returns>
        public static bool operator ==(Degree left, Degree right)
        {
            return Math.Abs(left.Value - right.Value) < 1E-06;
        }

        /// <summary>
        /// Determine whether two Degree objects are not equal.
        /// </summary>
        /// <param name="left">First of two Degree objects to compare.</param>
        /// <param name="right">Second of two Degree objects to compare.</param>
        /// <returns>Returns True if the two Degree objects are unquivalent; false otherwise.</returns>
        public static bool operator !=(Degree left, Degree right)
        {
            return left.Value != right.Value;
        }

        #endregion

        #region "CType Operators"

        /// <summary>
        /// Cast a Degree object to double.
        /// </summary>
        /// <param name="left">Degree object to cast.</param>
        /// <returns>Double representation of the Degree object.</returns>
        public static implicit operator double(Degree left)
        {
            return left.Value;
        }

        /// <summary>
        /// Cast a Degree object to int. Note: There is an implicit loss of precision with this operation.
        /// </summary>
        /// <param name="left">Degree object to cast.</param>
        /// <returns>Integer representation of the Degree object.</returns>
        public static explicit operator float(Degree left)
        {
            return (float) left.Value;
        }

        /// <summary>
        /// Cast a Degree object to decimal.
        /// </summary>
        /// <param name="left">Degree object to cast.</param>
        /// <returns>Decimal representation of the Degree object.</returns>
        public static explicit operator decimal(Degree left)
        {
            return (decimal) left.Value;
        }

        /// <summary>
        /// Cast a Radian object to a Degree object.
        /// </summary>
        /// <param name="left">Radian object to cast.</param>
        /// <returns>New Degree object containing the result of the operation.</returns>
        public static implicit operator Degree(Radian left)
        {
            return new Degree(left.Value / Math.PI * 180.0);
        }

        /// <summary>
        /// Cast a double to a Degree object.
        /// </summary>
        /// <param name="left">Double to cast.</param>
        /// <returns>New Degree object containing the result of the operation.</returns>
        public static implicit operator Degree(double left)
        {
            return new Degree(left);
        }

        #endregion

        #region "Operations"

        /// <summary>
        /// Returns a string representation of the value in degrees.
        /// </summary>
        /// <returns>String representation of the value in degrees.</returns>
        public override string ToString()
        {
            // This is the most accuracy that PowerShape and PowerMill accept
            return ToString("0.######", null);
        }

        /// <summary>
        /// Returns a formatted string representation of the value in degrees.
        /// </summary>
        /// <param name="format">Standard numeric format specifier.</param>
        /// <returns>String representation of the value in degrees.</returns>
        public string ToString(string format)
        {
            return Value.ToString(format, null);
        }

        /// <summary>
        /// Returns the numeric value of this instance in its equivalent string representation using the specified culture-specific format information.
        /// </summary>
        /// <param name="fp">Custom object implementing the IFormatProvider interface.</param>
        /// <returns>String representation of the value in degrees.</returns>
        public string ToString(IFormatProvider fp)
        {
            return Value.ToString(fp);
        }

        /// <summary>
        /// Returns the numeric value of this instance in its equivalent string representation using the specified format and culture-specific format information.
        /// </summary>
        /// <param name="format">Standard numeric format specifier.</param>
        /// <param name="fp">Custom object implementing the IFormatProvider interface.</param>
        /// <returns>String representation of the value in degrees.</returns>
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
            if (obj is Degree)
            {
                return this == (Degree) obj;
            }
            if (obj is Radian)
            {
                Degree objAsMM = (Radian) obj;
                return this == objAsMM;
            }
            return Value == (Degree) obj;
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
                Degree objAsDegree = (Degree) obj;
                double roundedObj = Math.Round(objAsDegree.Value, nDecPts);
                double roundedMe = Math.Round(Value, nDecPts);
                return roundedObj == roundedMe;
            }
            if (obj is Radian)
            {
                Degree objAsDegree = (Radian) obj;
                double roundedObj = Math.Round(objAsDegree.Value, nDecPts);
                double roundedMe = Math.Round(Value, nDecPts);
                return roundedObj == roundedMe;
            }
            return false;
        }

        /// <summary>
        /// Determines whether the passed argument is before or after this one in logical sort order.
        /// </summary>
        /// <param name="obj">Passed object to compare. Must be of type Degree.</param>
        /// <returns>
        /// Integer detailing the relative ordering of the two objects in the sort order: If zero, both
        /// occur in the same position; negative and this object preceeds the passed argument and, if positive,
        /// this object comes after the passed argument.
        /// </returns>
        /// <exception cref="ArgumentException">Thrown if the object to compare is not of type Degree.</exception>
        public int CompareTo(object obj)
        {
            if (!(obj is Degree))
            {
                throw new ArgumentException("Object to compare is not a Degree.");
            }

            return Value.CompareTo(((Degree) obj).Value);
        }

        #endregion
    }
}