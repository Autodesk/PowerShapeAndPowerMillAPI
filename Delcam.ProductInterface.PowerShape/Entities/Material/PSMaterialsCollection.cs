// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;

namespace Autodesk.ProductInterface.PowerSHAPE
{
    /// <summary>
    /// Captures the collection of Materials in PowerSHAPE.
    /// </summary>
    public class PSMaterialsCollection : PSCollection<PSMaterial>
    {
        #region " Fields "

        /// <summary>
        /// The base instance to interact with PowerShape.
        /// </summary>
        private PSAutomation _powerSHAPE;

        #endregion

        #region " Constructors "

        /// <summary>
        /// This constructor is required as collection inherits from EntitiesCollection.
        /// </summary>
        /// <param name="powerSHAPE">The base instance to interact with PowerShape.</param>
        internal PSMaterialsCollection(PSAutomation powerSHAPE)
        {
            _powerSHAPE = powerSHAPE;
        }

        #endregion

        #region " Properties "

        /// <summary>
        /// Gets the material with the specified name.  If the specified material does not exist
        /// in the collection, then Nothing is returned.
        /// </summary>
        /// <param name="name">The material name.</param>
        /// <returns>The found material.</returns>
        /// <remarks>None found, so returns Nothing.</remarks>
        public PSMaterial Item(string name)
        {
            // See if we can find a level with the specified name
            foreach (PSMaterial mat in this)
            {
                if (mat.Name == name)
                {
                    return mat;
                }
            }

            // None found, so return Nothing
            return null;
        }

        #endregion

        #region " Operations "

        /// <summary>
        /// Removes a material from the collection and from PowerShape.
        /// </summary>
        /// <param name="material">The material to remove.</param>
        public new bool Remove(PSMaterial material)
        {
            _powerSHAPE.DoCommand("FORMAT MATERIAL", "CUSTOM", "SELECT " + material.Name, "DELETE", "APPLY", "ACCEPT");

            return base.Remove(material);
        }

        /// <summary>
        /// Creates a new material in PowerSHAPE with the specified values.
        /// </summary>
        /// <param name="name">The name of the material.</param>
        /// <param name="polish">The polish of the material.</param>
        /// <param name="emission">The emission of the material.</param>
        /// <param name="transparency">The transparency of the material.</param>
        /// <param name="reflectance">The reflectance of the material.</param>
        /// <param name="colourR">The red colour of the material. Between 0 and 255.</param>
        /// <param name="colourG">The green colour of the material. Between 0 and 255.</param>
        /// <param name="colourB">The blue colour of the material.Between 0 and 255.</param>
        public PSMaterial CreateMaterial(
            string name,
            double polish,
            double emission,
            double transparency,
            double reflectance,
            byte colourR,
            byte colourG,
            byte colourB)
        {
            PSMaterial material = new PSMaterial(_powerSHAPE,
                                                 name,
                                                 polish,
                                                 emission,
                                                 transparency,
                                                 reflectance,
                                                 colourR,
                                                 colourG,
                                                 colourB);
            Add(material);
            return material;
        }

        /// <summary>
        /// Create the defined materials.
        /// </summary>
        /// <param name="materialDefinitions">Each definition should be name, polish, emission, transparency, reflectance, byte[R, G, B].</param>
        public List<PSMaterial> CreateMaterials(Tuple<string, double, double, double, double, byte[]>[] materialDefinitions)
        {
            List<PSMaterial> materials = new List<PSMaterial>();

            if (materialDefinitions.Count() == 0)
            {
                return materials;
            }

            _powerSHAPE.DoCommand("FORMAT MATERIALSTABLE", "MODE WRITE");

            foreach (Tuple<string, double, double, double, double, byte[]> materialDefinition in materialDefinitions)
            {
                _powerSHAPE.DoCommand("//-----------------------------------------------------------------------------",
                                      "PRINT '" + materialDefinition.Item1 + "'",
                                      "\"" + materialDefinition.Item1 + "\" \"Custom\" Create",
                                      "POLISH " + materialDefinition.Item2 / 100.0,
                                      "EMISSION " + materialDefinition.Item3 / 100.0,
                                      "TRANSPARENCY " + materialDefinition.Item4 / 100.0,
                                      "REFLECTANCE " + materialDefinition.Item5 / 100.0,
                                      "RED " + materialDefinition.Item6[0] / 255.0,
                                      "GREEN " + materialDefinition.Item6[1] / 255.0,
                                      "BLUE " + materialDefinition.Item6[2] / 255.0,
                                      "SCALE 1.0",
                                      "ACCEPT");
                PSMaterial material = new PSMaterial(_powerSHAPE, materialDefinition.Item1);
                Add(material);
                materials.Add(material);
            }
            _powerSHAPE.DoCommand("MODE READ", "RELOAD", "ACCEPT");

            return materials;
        }

        #endregion
    }
}