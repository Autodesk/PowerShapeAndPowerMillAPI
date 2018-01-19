// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;

namespace Autodesk.ProductInterface.PowerSHAPE
{
    /// <summary>
    /// Abstract class captures a database entity in PowerSHAPE.
    /// </summary>
    public abstract class PSDatabaseEntity
    {
        #region " Fields "

        /// <summary>
        /// The PowerSHAPE Automation object.
        /// </summary>
        protected PSAutomation _powerSHAPE;

        /// <summary>
        /// The database ID of the object.
        /// </summary>
        protected int _id;

        /// <summary>
        /// The name of the object.
        /// </summary>
        protected string _name;

        #endregion

        #region " Constructors "

        /// <summary>
        /// Creates a new base entity.
        /// </summary>
        /// <param name="powerSHAPE">The base instance to interact with PowerShape.</param>
        internal PSDatabaseEntity(PSAutomation powerSHAPE)
        {
            _powerSHAPE = powerSHAPE;
        }

        #endregion

        #region " Properties "

        /// <summary>
        /// Gets the parent PSAutomation.
        /// </summary>
        public PSAutomation PSAutomation
        {
            get { return _powerSHAPE; }
        }

        /// <summary>
        /// Gets the Id of the entity in PowerSHAPE.
        /// </summary>
        public int Id
        {
            get { return _id; }
            internal set { _id = value; }
        }

        /// <summary>
        /// Gets the identifier PowerSHAPE uses to identify this type of entity.
        /// </summary>
        internal abstract string Identifier { get; }

        /// <summary>
        /// Gets or Sets the name of the entity.
        /// </summary>
        public abstract string Name { get; set; }

        /// <summary>
        /// Checks to see if the entity exists in PowerSHAPE.
        /// </summary>
        public virtual bool Exists
        {
            get { return _powerSHAPE.ReadIntValue(Identifier + "[ID " + _id + "].EXISTS") == 1; }
        }

        /// <summary>
        /// Gets the current PowerSHAPE instance.
        /// </summary>
        internal PSAutomation PowerSHAPE
        {
            get { return _powerSHAPE; }
        }

        #endregion

        #region " Operations "

        /// <summary>
        /// This operation throws an error if the item does not exist in PowerSHAPE.
        /// </summary>
        protected virtual void AbortIfDoesNotExist()
        {
            if (!Exists)
            {
                throw new NullReferenceException("Item does not exist in PowerSHAPE");
            }
        }

        /// <summary>
        /// Compares the DatabaseEntity to another DatabaseEntity by checking their IDs.
        /// </summary>
        /// <param name="obj">The other DatabaseEntity to compare to.</param>
        /// <returns>Boolean determining whether the IDs match.</returns>
        /// <remarks></remarks>
        public override bool Equals(object obj)
        {
            if (obj is PSDatabaseEntity)
            {
                return (_id == ((PSDatabaseEntity) obj).Id) & (Identifier == ((PSDatabaseEntity) obj).Identifier);
            }
            return false;
        }

        #endregion
    }
}