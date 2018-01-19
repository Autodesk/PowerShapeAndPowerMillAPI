// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using Autodesk.FileSystem;

namespace Autodesk.ProductInterface.PowerSHAPE
{
    /// <summary>
    /// Captures a collection of Meshes in a Project
    /// </summary>
    public class PSMeshesCollection : PSEntitiesCollection<PSMesh>
    {
        #region " Fields "

        #endregion

        #region " Constructors "

        /// <summary>
        /// This constructor is required as collection inherits from EntitiesCollection
        /// </summary>
        internal PSMeshesCollection(PSAutomation powerSHAPE) : base(powerSHAPE)
        {
        }

        #endregion

        #region " Properties "

        /// <summary>
        /// Gets the identifier used by PowerSHAPE when selecting a Mesh
        /// </summary>
        internal override string Identifier
        {
            get { return "MESH"; }
        }

        #endregion

        #region " Operations "

        /// <summary>
        /// Creates a new Mesh from a DMT Model
        /// </summary>
        /// <param name="model">The DMT Model from which to create the mesh</param>
        /// <returns>Mesh created by operation</returns>
        public PSMesh CreateMeshFromDMT(Geometry.DMTModel model)
        {
            PSMesh mesh = new PSMesh(_powerSHAPE, model);
            Add(mesh);
            return mesh;
        }

        /// <summary>
        /// Creates a new Mesh from a Surface
        /// </summary>
        /// <param name="surface">The Surface from which to create the mesh</param>
        /// <returns>Mesh created by operation</returns>
        public PSMesh CreateMeshFromSurface(PSSurface surface)
        {
            PSSurface newSurface = (PSSurface) surface.Duplicate();

            newSurface.AddToSelection(true);

            // Carry out operation
            _powerSHAPE.ActiveModel.ClearCreatedItems();
            _powerSHAPE.DoCommand("CONVERT MESH");

            // Add new Mesh to collection of Meshes
            PSMesh newMesh = (PSMesh) _powerSHAPE.ActiveModel.CreatedItems[0];
            _powerSHAPE.ActiveModel.Meshes.Add(newMesh);

            // Remove this Surface from the collection of surfaces
            newSurface.Delete();

            return newMesh;
        }

        /// <summary>
        /// Creates a new Mesh from a Solid
        /// </summary>
        /// <param name="solid">The Solid from which to create the mesh</param>
        /// <returns>Mesh created by operation</returns>
        public PSMesh CreateMeshFromSolid(PSSolid solid)
        {
            PSSolid newSolid = (PSSolid) solid.Duplicate();

            newSolid.AddToSelection(true);

            // Carry out operation
            _powerSHAPE.ActiveModel.ClearCreatedItems();
            _powerSHAPE.DoCommand("CONVERT MESH");

            // Add new Mesh to collection of Meshes
            PSMesh newMesh = (PSMesh) _powerSHAPE.ActiveModel.CreatedItems[0];
            _powerSHAPE.ActiveModel.Meshes.Add(newMesh);

            // Remove this Surface from the collection of surfaces
            newSolid.Delete();

            return newMesh;
        }

        /// <summary>
        /// Creates a new Mesh from a specified file
        /// </summary>
        /// <param name="file">The file from which to create the mesh</param>
        /// <returns>Mesh created by operation</returns>
        public PSMesh CreateMeshFromFile(File file)
        {
            // Clear the created items
            _powerSHAPE.ActiveModel.ClearCreatedItems();

            // Import the file
            _powerSHAPE.ActiveModel.Import(file);

            // Get the one created item (if nothing was created then return Nothing
            if (_powerSHAPE.ActiveModel.CreatedItems.Count == 1)
            {
                PSMesh newMesh = (PSMesh) _powerSHAPE.ActiveModel.CreatedItems[0];
                _powerSHAPE.ActiveModel.Meshes.Add(newMesh);
                return newMesh;
            }
            return null;
        }

        #endregion
    }
}