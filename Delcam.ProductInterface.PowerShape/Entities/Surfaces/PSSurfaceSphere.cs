// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

namespace Autodesk.ProductInterface.PowerSHAPE
{
    /// <summary>
    /// Captures a Surface Sphere object in PowerSHAPE
    /// </summary>
    public class PSSurfaceSphere : PSPrimitive
    {
        #region " Fields "

        #endregion

        #region " Properties "

        /// <summary>
        /// Gets and sets the radius of the sphere in MM
        /// </summary>
        /// <value>The new radius of the sphere</value>
        /// <returns>The radius of the sphere</returns>
        public Geometry.MM Radius
        {
            get { return _powerSHAPE.ReadDoubleValue("SURFACE['" + Name + "'].RADIUS"); }

            set
            {
                // Add to selection
                AddToSelection(true);

                _powerSHAPE.DoCommand("MODIFY", "CHANGE_DIMENSION RADIUS", "DIMENSION " + value.ToString("0.######"), "ACCEPT");
            }
        }

        #endregion

        #region " Constructors "

        internal PSSurfaceSphere(PSAutomation powerSHAPE) : base(powerSHAPE)
        {
        }

        internal PSSurfaceSphere(PSAutomation powerSHAPE, int id, string name) : base(powerSHAPE, id, name)
        {
        }

        internal PSSurfaceSphere(PSAutomation powershape, Geometry.Point origin) : base(powershape)
        {
            // Clear CreatedItems
            _powerSHAPE.ActiveModel.ClearCreatedItems();

            // Create a plane at the point specified
            _powerSHAPE.DoCommand("CREATE SURFACE SPHERE");
            _powerSHAPE.DoCommand(origin.ToString());

            // Get created plane id
            PSSurfaceSphere newSphere = (PSSurfaceSphere) _powerSHAPE.ActiveModel.CreatedItems[0];
            _id = newSphere.Id;
        }

        #endregion
    }
}