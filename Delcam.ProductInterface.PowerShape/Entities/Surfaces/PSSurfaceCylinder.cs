// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using Autodesk.Geometry;

namespace Autodesk.ProductInterface.PowerSHAPE
{
    /// <summary>
    /// Captures a Surface Cylinder object in PowerSHAPE
    /// </summary>
    public class PSSurfaceCylinder : PSPrimitive
    {
        #region " Fields "

        #endregion

        #region " Properties "

        /// <summary>
        /// Gets and Sets the radius of the cylinder
        /// </summary>
        public MM Radius
        {
            get { return _powerSHAPE.ReadDoubleValue(Identifier + "[ID " + _id + "].RADIUS"); }

            set
            {
                // Add the cylinder to the selection
                AddToSelection();

                // Change the radius
                _powerSHAPE.DoCommand("SURFEDITS");
                _powerSHAPE.DoCommand("RADIUS " + value.ToString("0.######"), "ACCEPT");
            }
        }

        /// <summary>
        /// Gets and Sets the length of the cylinder
        /// </summary>
        public MM Length
        {
            get { return _powerSHAPE.ReadDoubleValue(Identifier + "[ID " + _id + "].LENGTH"); }
            set
            {
                // Add the cylinder to the selection
                AddToSelection();

                // Make the length adjustment
                _powerSHAPE.DoCommand("SURFEDITS");
                _powerSHAPE.DoCommand("LENGTH " + value.ToString("0.######"), "ACCEPT");
            }
        }

        /// <summary>
        /// Gets the volume of the cylinder, which is the area of the circular face,
        /// multiplied by its length, not 0 as returned by PowerSHAPE.
        /// </summary>
        public override double Volume
        {
            get
            {
                double area = Math.PI * Math.Pow(Radius, 2);
                return area * Length;
            }
        }

        #endregion

        #region " Constructors "

        internal PSSurfaceCylinder(PSAutomation powerSHAPE) : base(powerSHAPE)
        {
        }

        internal PSSurfaceCylinder(PSAutomation powerSHAPE, int id, string name) : base(powerSHAPE, id, name)
        {
        }

        internal PSSurfaceCylinder(PSAutomation powershape, Point origin) : base(powershape)
        {
            // Clear CreatedItems
            _powerSHAPE.ActiveModel.ClearCreatedItems();

            // Create a plane at the point specified
            _powerSHAPE.DoCommand("CREATE SURFACE CYLINDER");
            _powerSHAPE.DoCommand(origin.ToString());

            // Get created plane id
            PSSurfaceCylinder newCylinder = (PSSurfaceCylinder) _powerSHAPE.ActiveModel.CreatedItems[0];
            _id = newCylinder.Id;
        }

        #endregion
    }
}