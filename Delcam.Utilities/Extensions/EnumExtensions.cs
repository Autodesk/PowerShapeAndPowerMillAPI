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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Resources;

namespace Autodesk.Extensions
{
    /// <summary>
    /// This module contains all enum extensions
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Returns plane normal to the input axis.
        /// </summary>
        /// <param name="inputAxis">Axis against which to return the plane.</param>
        /// <returns>Plane normal to input axis.</returns>
        public static Planes AxisToPlane(this Axes inputAxis)
        {
            // Returns associated plane
            switch (inputAxis)
            {
                case Axes.X:
                    return Planes.YZ;
                case Axes.Y:
                    return Planes.ZX;
                case Axes.Z:
                    return Planes.XY;
            }
            throw new Exception();
        }

        /// <summary>
        /// Returns axis normal to the input plane.
        /// </summary>
        /// <param name="inputPlane">Plane against which to return the axis.</param>
        /// <returns>Axis normal to input plane.</returns>
        public static Axes PlaneToAxis(this Planes inputPlane)
        {
            // Returns associated axis
            switch (inputPlane)
            {
                case Planes.YZ:
                    return Axes.X;
                case Planes.ZX:
                    return Axes.Y;
                case Planes.XY:
                    return Axes.Z;
            }
            throw new Exception();
        }

        /// <summary>
        /// Get Enum description value
        /// </summary>
        /// <param name="value">enum value</param>
        /// <returns>description value</returns>
        public static string ToDescription<T>(T value) where T : struct
        {
            Type type = value.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException("value must be of Enum type", "value");
            }

            //Tries to find a DescriptionAttribute for a potential friendly name
            //for the enum

            MemberInfo[] memberInfo = type.GetMember(value.ToString());
            if (memberInfo != null && memberInfo.Length > 0)
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs.Any(attr => attr.GetType() == typeof(DescriptionAttribute)))
                {
                    return ((DescriptionAttribute) attrs[0]).Description;
                }
            }

            //return empty if there is no description attribute 
            return string.Empty;
        }

        /// <summary>
        /// Get enum full name
        /// </summary>
        /// <example>
        /// enum Colors{ Red, Yellow }
        /// GetFullName(Colors.Red) --> "ColorsRed";
        /// </example>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetFullName<T>(T value) where T : struct
        {
            if (!value.GetType().IsEnum)
            {
                throw new ArgumentException("value must be of Enum type", "value");
            }

            return typeof(T).Name + value;
        }

        /// <summary>
        /// Get enum value by string value representing value withing a  <see cref="ResourceSet"/> lookup
        /// </summary>
        /// <example>
        /// Giving
        /// enumOfItems {
        /// FirstItem,
        /// SecondItem,
        /// LastItme
        /// }
        /// mapped to keys of
        /// resourceSet {
        /// ("FirstItem","First value"),
        /// ("SecondItem","Second value"),
        /// ("LastItem","Last value")
        /// }
        /// and if you need to find an enum of value =  "Second value"
        /// this will return enumOfItems.SecondItem
        /// </example>
        /// <typeparam name="T">type of Enum</typeparam>
        /// <param name="value">a string value of Enum</param>
        /// <param name="resourceSet">lookup resources</param>
        /// <returns>value of enum</returns>
        public static T GetEnumByResourceValue<T>(string value, ResourceSet resourceSet) where T : struct
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("value must be of Enum type", value);
            }

            var resourceKeys = resourceSet.Cast<DictionaryEntry>()
                                          .Where(item => item.Value.ToString().ToLower() == value.ToLower()).ToList();
            var entries = resourceKeys.Where(k => Enum.GetNames(typeof(T)).Contains(k.Key));
            if (entries.Any())
            {
                return (T) Enum.Parse(typeof(T), (string) entries.First().Key);
            }
            throw new KeyNotFoundException(string.Format("value {0} for Enum {1}, not exists in resources ", value, typeof(T)));
        }
    }
}