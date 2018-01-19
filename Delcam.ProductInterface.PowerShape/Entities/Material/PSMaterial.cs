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
    /// Captures a Material in PowerSHAPE.  A Material is used to give colour and texture
    /// to a surface.
    /// </summary>
    public class PSMaterial
    {
        #region " Fields "

        /// <summary>
        /// The PowerSHAPE Automation object.
        /// </summary>
        private PSAutomation _powerSHAPE;

        /// <summary>
        /// The name of the Material.
        /// </summary>
        private string _name;

        #endregion

        #region " Constructors "

        /// <summary>
        /// This operation initialises a Material.
        /// </summary>
        /// <param name="powerSHAPE">The base instance to interact with PowerShape.</param>
        /// <param name="name">The material name.</param>
        internal PSMaterial(PSAutomation powerSHAPE, string name)
        {
            _powerSHAPE = powerSHAPE;
            _name = name;
        }

        /// <summary>
        /// This constructor creates a new material in PowerSHAPE with the specified values.
        /// </summary>
        /// <param name="powerSHAPE">The PowerSHAPE Automation object.</param>
        /// <param name="name">The name of the material.</param>
        /// <param name="polish">The polish of the material.</param>
        /// <param name="emission">The emission of the material.</param>
        /// <param name="transparency">The transparency of the material.</param>
        /// <param name="reflectance">The reflectance of the material.</param>
        /// <param name="colourR">The red colour of the material.  Between 0 and 255.</param>
        /// <param name="colourG">The green colour of the material.  Between 0 and 255.</param>
        /// <param name="colourB">The blue colour of the material.  Between 0 and 255.</param>
        internal PSMaterial(
            PSAutomation powerSHAPE,
            string name,
            double polish,
            double emission,
            double transparency,
            double reflectance,
            byte colourR,
            byte colourG,
            byte colourB) : this(powerSHAPE, name)
        {
            _powerSHAPE.DoCommand("FORMAT MATERIALSTABLE",
                                  "MODE WRITE",
                                  "//-----------------------------------------------------------------------------",
                                  "PRINT '" + name + "'",
                                  "\"" + name + "\" \"Custom\" Create",
                                  "POLISH " + polish / 100.0,
                                  "EMISSION " + emission / 100.0,
                                  "TRANSPARENCY " + transparency / 100.0,
                                  "REFLECTANCE " + reflectance / 100.0,
                                  "RED " + colourR / 255.0,
                                  "GREEN " + colourG / 255.0,
                                  "BLUE " + colourB / 255.0,
                                  "SCALE 1.0",
                                  "ACCEPT",
                                  "MODE READ",
                                  "RELOAD",
                                  "ACCEPT");
        }

        #endregion

        #region " Properties "

        /// <summary>
        /// Gets the name of the Material.
        /// </summary>
        /// <returns>Name of the Material.</returns>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Gets and sets the polish value of the material.
        /// </summary>
        /// <returns>The polish of the material.</returns>
        /// <value>The polish of the material.</value>
        /// .
        public double Polish
        {
            get { return _powerSHAPE.ReadDoubleValue("MATERIAL['" + Name + "'].POLISH"); }

            set { _powerSHAPE.DoCommand("FORMAT MATERIAL", "CUSTOM", "SELECT " + _name, "POLISH " + value, "APPLY", "ACCEPT"); }
        }

        /// <summary>
        /// Gets and sets the emission of the material.
        /// </summary>
        /// <returns>The emission of the material.</returns>
        /// <value>The emission of the material.</value>
        public double Emission
        {
            get { return _powerSHAPE.ReadDoubleValue("MATERIAL['" + Name + "'].EMISSION"); }

            set { _powerSHAPE.DoCommand("FORMAT MATERIAL", "CUSTOM", "SELECT " + _name, "EMISSION " + value, "APPLY", "ACCEPT"); }
        }

        /// <summary>
        /// Gets and sets the transparency of the material.
        /// </summary>
        /// <returns>The transparency of the material.</returns>
        /// <value>The transparency of the material.</value>
        public double Transparency
        {
            get { return _powerSHAPE.ReadDoubleValue("MATERIAL['" + Name + "'].TRANSPARENCY"); }

            set
            {
                _powerSHAPE.DoCommand("FORMAT MATERIAL", "CUSTOM", "SELECT " + _name, "TRANSPARENCY " + value, "APPLY", "ACCEPT");
            }
        }

        /// <summary>
        /// Gets and sets the reflectance of the material.
        /// </summary>
        /// <returns>The reflectance of the material.</returns>
        /// <value>The reflectance of the material.</value>
        public double Reflectance
        {
            get { return _powerSHAPE.ReadDoubleValue("MATERIAL['" + Name + "'].REFLECTANCE"); }

            set { _powerSHAPE.DoCommand("FORMAT MATERIAL", "CUSTOM", "SELECT " + _name, "Reflect " + value, "APPLY", "ACCEPT"); }
        }

        /// <summary>
        /// Gets and sets the red colour of the material...
        /// </summary>
        /// <returns>The red colour of the material.</returns>
        /// <value>The red colour of the material.</value>
        public byte ColourR
        {
            get { return (byte) _powerSHAPE.DoCommandEx("MATERIAL['" + Name + "'].COLOR.R"); }

            set { _powerSHAPE.DoCommand("FORMAT MATERIAL", "CUSTOM", "SELECT " + _name, "Red255 " + value, "APPLY", "ACCEPT"); }
        }

        /// <summary>
        /// Gets and sets the green colour of the material..
        /// </summary>
        /// <returns>The green colour of the material..</returns>
        /// <value>The green colour of the material..</value>
        public byte ColourG
        {
            get { return (byte) _powerSHAPE.DoCommandEx("MATERIAL['" + Name + "'].COLOR.G"); }

            set { _powerSHAPE.DoCommand("FORMAT MATERIAL", "CUSTOM", "SELECT " + _name, "Green255 " + value, "APPLY", "ACCEPT"); }
        }

        /// <summary>
        /// Gets and sets the blue colour of the material.
        /// </summary>
        /// <returns>The blue colour of the material.</returns>
        /// <value>The blue colour of the material.</value>
        public byte ColourB
        {
            get { return (byte) _powerSHAPE.DoCommandEx("MATERIAL['" + Name + "'].COLOR.B"); }

            set { _powerSHAPE.DoCommand("FORMAT MATERIAL", "CUSTOM", "SELECT " + _name, "Blue255 " + value, "APPLY", "ACCEPT"); }
        }

        #endregion

        #region " Operations "

        /// <summary>
        /// This operation checks to see if a material exists.
        /// </summary>
        internal bool Exists()
        {
            if (ColourB == null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// This operation deletes the Material from PowerSHAPE and removes it from the Materials collection.
        /// </summary>
        public void Delete()
        {
            _powerSHAPE.ActiveModel.Materials.Remove(this);
        }

        #endregion
    }
}