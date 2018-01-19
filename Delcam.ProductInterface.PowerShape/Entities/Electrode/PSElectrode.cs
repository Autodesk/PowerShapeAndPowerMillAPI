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
using Autodesk.Geometry;

namespace Autodesk.ProductInterface.PowerSHAPE
{
    /// <summary>
    /// Captures an Electrode object in PowerSHAPE
    /// </summary>
    public class PSElectrode : PSEntity, IPSMoveable, IPSRotateable, IPSMirrorable, IPSScalable
    {
        #region " Fields "

        /// <summary>
        /// Material that is currently applied to the Electrode
        /// </summary>
        private PSMaterial _material;

        #endregion

        #region " Constructors "

        /// <summary>
        /// Initialises the Electrode.
        /// </summary>
        /// <param name="powerSHAPE">This is the PowerSHAPE Automation object.</param>
        /// <param name="id">The ID of the new model.</param>
        /// <param name="name">The name of the new model.</param>
        /// <remarks></remarks>
        internal PSElectrode(PSAutomation powerSHAPE, int id, string name) : base(powerSHAPE, id, name)
        {
        }

        #endregion

        #region " Properties "

        internal const string ELECTRODE_IDENTIFIER = "ELECTRODE";

        /// <summary>
        /// Gets the identifier PowerSHAPE uses to identify this type of entity
        /// </summary>
        internal override string Identifier
        {
            get { return ELECTRODE_IDENTIFIER; }
        }

        /// <summary>
        /// Gets the composite ID for use in communication with the ActiveDocument object
        /// </summary>
        internal override int CompositeID
        {
            get { return 60 * 20000000 + _id; }
        }

        internal override string RenameCommand
        {
            get { return "MODIFY"; }
        }

        /// <summary>
        /// Gets and Sets the material of the Electrode in PowerSHAPE
        /// </summary>
        public PSMaterial Material
        {
            get
            {
                // Check first that the material still exists
                if (_powerSHAPE.ActiveModel.Materials.Contains(_material))
                {
                    return _material;
                }
                return null;
            }

            set
            {
                _material = value;
                AddToSelection(true);
                _powerSHAPE.DoCommand("FORMAT MATERIAL", "CUSTOM", "SELECT " + _material.Name, "APPLY", "ACCEPT", "ACCEPT");
            }
        }

        /// <summary>
        /// Gets the datum of the Electrode in PowerSHAPE based on the active Workplane
        /// </summary>
        public Point Datum
        {
            get
            {
                double[] datumPosition = _powerSHAPE.DoCommandEx(Identifier + "[ID " + _id + "].DATUM") as double[];
                return new Point(datumPosition[0], datumPosition[1], datumPosition[2]);
            }
        }

        /// <summary>
        /// Gets and sets the undersize rough value of the electrode
        /// </summary>
        /// <value>The undersize rough value</value>
        public double UndersizeRough
        {
            get { return _powerSHAPE.ReadDoubleValue(Identifier + "[ID " + Id + "].UNDERSIZE.ROUGH"); }
            set
            {
                int originalNumber = _powerSHAPE.ReadIntValue("WINDOW.NUMBER");
                AddToSelection(true);
                _powerSHAPE.DoCommand("MODIFY", "ROUGH UNDERSIZE " + value, "ACCEPT");
                while (_powerSHAPE.ReadIntValue("WINDOW.NUMBER") > originalNumber) _powerSHAPE.DoCommand("WINDOW CLOSE");
            }
        }

        /// <summary>
        /// Gets and sets the undersize semi value of the electrode
        /// </summary>
        /// <value>The undersize semi value</value>
        public double UndersizeSemi
        {
            get { return _powerSHAPE.ReadDoubleValue(Identifier + "[ID " + Id + "].UNDERSIZE.SEMI"); }
            set
            {
                int originalNumber = _powerSHAPE.ReadIntValue("WINDOW.NUMBER");
                AddToSelection(true);
                _powerSHAPE.DoCommand("MODIFY", "SEMI UNDERSIZE " + value, "ACCEPT");
                while (_powerSHAPE.ReadIntValue("WINDOW.NUMBER") > originalNumber) _powerSHAPE.DoCommand("WINDOW CLOSE");
            }
        }

        /// <summary>
        /// Gets and sets the undersize finish value of the electrode
        /// </summary>
        /// <value>The undersize finish value</value>
        public double UndersizeFinish
        {
            get { return _powerSHAPE.ReadDoubleValue(Identifier + "[ID " + Id + "].UNDERSIZE.FINISH"); }
            set
            {
                int originalNumber = _powerSHAPE.ReadIntValue("WINDOW.NUMBER");
                AddToSelection(true);
                _powerSHAPE.DoCommand("MODIFY", "FINISHER UNDERSIZE " + value, "ACCEPT");
                while (_powerSHAPE.ReadIntValue("WINDOW.NUMBER") > originalNumber) _powerSHAPE.DoCommand("WINDOW CLOSE");
            }
        }

        /// <summary>
        /// Gets and sets the quantity rough value of the electrode
        /// </summary>
        /// <value>The undersize rough value</value>
        public int QuantityRough
        {
            get { return _powerSHAPE.ReadIntValue(Identifier + "[ID " + Id + "].QUANTITY.ROUGH"); }
            set
            {
                int originalNumber = _powerSHAPE.ReadIntValue("WINDOW.NUMBER");
                AddToSelection(true);
                _powerSHAPE.DoCommand("MODIFY", "ROUGH QUANTITY " + value, "ACCEPT");
                while (_powerSHAPE.ReadIntValue("WINDOW.NUMBER") > originalNumber) _powerSHAPE.DoCommand("WINDOW CLOSE");
            }
        }

        /// <summary>
        /// Gets and sets the quantity semi value of the electrode
        /// </summary>
        /// <value>The quantity semi value</value>
        public int QuantitySemi
        {
            get { return _powerSHAPE.ReadIntValue(Identifier + "[ID " + Id + "].QUANTITY.SEMI"); }
            set
            {
                int originalNumber = _powerSHAPE.ReadIntValue("WINDOW.NUMBER");
                AddToSelection(true);
                _powerSHAPE.DoCommand("MODIFY", "SEMI QUANTITY " + value, "ACCEPT");
                while (_powerSHAPE.ReadIntValue("WINDOW.NUMBER") > originalNumber) _powerSHAPE.DoCommand("WINDOW CLOSE");
            }
        }

        /// <summary>
        /// Gets and sets the quantity finish value of the electrode
        /// </summary>
        /// <value>The quantity finish value</value>
        public int QuantityFinish
        {
            get { return _powerSHAPE.ReadIntValue(Identifier + "[ID " + Id + "].QUANTITY.FINISH"); }
            set
            {
                int originalNumber = _powerSHAPE.ReadIntValue("WINDOW.NUMBER");
                AddToSelection(true);
                _powerSHAPE.DoCommand("MODIFY", "FINISHER QUANTITY " + value, "ACCEPT");
                while (_powerSHAPE.ReadIntValue("WINDOW.NUMBER") > originalNumber) _powerSHAPE.DoCommand("WINDOW CLOSE");
            }
        }

        /// <summary>
        /// Gets and sets the spark gap of the electrode
        /// </summary>
        /// <value>The spark gap value</value>
        public double SparkGap
        {
            get { return _powerSHAPE.ReadDoubleValue(Identifier + "[ID " + Id + "].SPARKGAP"); }
            set
            {
                int originalNumber = _powerSHAPE.ReadIntValue("WINDOW.NUMBER");
                AddToSelection(true);
                _powerSHAPE.DoCommand("MODIFY", "SPARKGAP " + value, "ACCEPT");
                while (_powerSHAPE.ReadIntValue("WINDOW.NUMBER") > originalNumber) _powerSHAPE.DoCommand("WINDOW CLOSE");
            }
        }

        /// <summary>
        /// Gets and sets the surface finish of the electrode
        /// </summary>
        /// <value>The surface finish value</value>
        public string SurfaceFinish
        {
            get { return _powerSHAPE.ReadStringValue(Identifier + "[ID " + Id + "].SURFACE_FINISH"); }
            set
            {
                int originalNumber = _powerSHAPE.ReadIntValue("WINDOW.NUMBER");
                AddToSelection(true);
                _powerSHAPE.DoCommand("MODIFY", "QUALITY " + value, "ACCEPT");
                while (_powerSHAPE.ReadIntValue("WINDOW.NUMBER") > originalNumber) _powerSHAPE.DoCommand("WINDOW CLOSE");
            }
        }

        /// <summary>
        /// Gets the projected area of the Electrode
        /// </summary>
        public double ProjectedArea
        {
            get { return _powerSHAPE.ReadDoubleValue(Identifier + "[ID " + Id + "].PROJECTED_AREA"); }
        }

        /// <summary>
        /// Gets the direction of the electrode burn
        /// </summary>
        public Vector BurnVector
        {
            get
            {
                double[] details = _powerSHAPE.DoCommandEx(Identifier + "[ID " + _id + "].BURN_VECTOR") as double[];
                return new Vector(details[0], details[1], details[2]);
            }
        }

        /// <summary>
        /// Gets the clearance distance from beginning of vector motion to start of burn
        /// </summary>
        public MM VectorClearance
        {
            get
            {
                // Can return as "-" so do a try cast
                double value = 0.0;
                double.TryParse(_powerSHAPE.ReadStringValue(Identifier + "[ID " + Id + "].VECTOR_CLEARANCE"), out value);
                return value;
            }
        }

        /// <summary>
        /// Gets the A angle. Angle between projected burn vector in XY plane and X axis
        /// </summary>
        public Degree AngleA
        {
            get { return Convert.ToDouble(_powerSHAPE.DoCommandEx(Identifier + "[ID " + Id + "].ANGLE.A")); }
        }

        /// <summary>
        /// Gets the B angle. Angle between projected burn vector and Z axis. Normal Z sinking: B=0. Side sparking: B=90
        /// </summary>
        public Degree AngleB
        {
            get { return Convert.ToDouble(_powerSHAPE.DoCommandEx(Identifier + "[ID " + Id + "].ANGLE.B")); }
        }

        /// <summary>
        /// Gets the C angle. Rotation around burn vector. This is same as "Rotation" if ANGLE.B is 0
        /// </summary>
        public Degree AngleC
        {
            get { return Convert.ToDouble(_powerSHAPE.DoCommandEx(Identifier + "[ID " + Id + "].ANGLE.C")); }
        }

        /// <summary>
        /// Gets the name of the blank
        /// </summary>
        public string BlankName
        {
            get { return _powerSHAPE.ReadStringValue(Identifier + "[ID " + Id + "].BLANK.NAME"); }
        }

        /// <summary>
        /// Gets whether or not the electrode is rectangular (or cylindrical)
        /// </summary>
        public bool IsBlankRectangular
        {
            get { return _powerSHAPE.ReadBoolValue(Identifier + "[ID " + Id + "].BLANK.RECTANGULAR"); }
        }

        /// <summary>
        /// Gets the length of the electrode blank if it is rectangular
        /// </summary>
        public MM BlankLength
        {
            get { return Convert.ToDouble(_powerSHAPE.DoCommandEx(Identifier + "[ID " + Id + "].BLANK.LENGTH")); }
        }

        /// <summary>
        /// Gets the width of the electrode blank if it is rectangular
        /// </summary>
        public MM BlankWidth
        {
            get { return Convert.ToDouble(_powerSHAPE.DoCommandEx(Identifier + "[ID " + Id + "].BLANK.WIDTH")); }
        }

        /// <summary>
        /// Gets the diameter of the electrode blank if it is cylindrical
        /// </summary>
        public MM BlankDiameter
        {
            get { return Convert.ToDouble(_powerSHAPE.DoCommandEx(Identifier + "[ID " + Id + "].BLANK.DIAMETER")); }
        }

        /// <summary>
        /// Gets the height of the electrode blank
        /// </summary>
        public MM BlankHeight
        {
            get { return Convert.ToDouble(_powerSHAPE.DoCommandEx(Identifier + "[ID " + Id + "].BLANK.HEIGHT")); }
        }

        /// <summary>
        /// Gets the material of the electrode blank in PowerSHAPE
        /// </summary>
        public PSMaterial BlankMaterial
        {
            get
            {
                string name = _powerSHAPE.ReadStringValue(Identifier + "[ID " + Id + "].BLANK.MATERIAL");

                return _powerSHAPE.ActiveModel.Materials.FirstOrDefault(x => x.Name == name);
            }
        }

        /// <summary>
        /// Gets the height of the electrode base
        /// </summary>
        public MM BaseHeight
        {
            get { return Convert.ToDouble(_powerSHAPE.DoCommandEx(Identifier + "[ID " + Id + "].BASE_HEIGHT")); }
        }

        /// <summary>
        /// Gets the rotation of the electrode
        /// </summary>
        public Degree Rotation
        {
            get
            {
                // Can return as "-" so do a try cast
                double value = 0.0;
                double.TryParse(_powerSHAPE.ReadStringValue(Identifier + "[ID " + Id + "].ROTATION"), out value);
                return value;
            }
        }

        /// <summary>
        /// Gets whether or not this electrode is a copy of another
        /// </summary>
        public bool IsCopy
        {
            get
            {
                List<string> copies = new List<string>(Convert.ToString(_powerSHAPE.DoCommandEx("ELECTRODE.LIST.COPIES"))
                                                              .Split(NewLine,
                                                                     StringSplitOptions.RemoveEmptyEntries));
                return copies.Contains(Name);
            }
        }

        /// <summary>
        /// Gets the parent of this electrode.  If this electrode is not a copy then null is returned
        /// </summary>
        public PSElectrode Parent
        {
            get
            {
                string parentName = _powerSHAPE.ReadStringValue("ELECTRODE['" + Name + "'].PARENT");
                return _powerSHAPE.ActiveModel.Electrodes.GetByName(parentName);
            }
        }

        /// <summary>
        /// Gets the copies of this electrode
        /// </summary>
        public List<PSElectrode> Copies
        {
            get
            {
                List<PSElectrode> returnList = new List<PSElectrode>();
                List<string> copyList = new List<string>(
                    Convert.ToString(_powerSHAPE.DoCommandEx("ELECTRODE['" + Name + "'].COPIES.LIST"))
                           .Split(NewLine,
                                  StringSplitOptions.None));

                foreach (string copy in copyList)
                {
                    PSElectrode thisCopy = _powerSHAPE.ActiveModel.Electrodes.GetByName(copy);
                    if (thisCopy != null)
                    {
                        returnList.Add(thisCopy);
                    }
                }
                return returnList;
            }
        }

        /// <summary>
        /// Gets the active workplane for the Electrode.  Returns null if none is selected.
        /// </summary>
        public PSWorkplane ActiveWorkplane
        {
            get
            {
                string wp = _powerSHAPE.ReadStringValue(Identifier + "[\"" + Name + "\"].ACTIVE_WORKPLANE");
                return _powerSHAPE.ActiveModel.Workplanes.FirstOrDefault(x => x.Name == wp);
            }
        }

        /// <summary>
        /// Gets the machining workplane for the Electrode.  Returns null if none is selected.
        /// </summary>
        public PSWorkplane MachiningWorkplane
        {
            get
            {
                string wp = _powerSHAPE.ReadStringValue(Identifier + "[\"" + Name + "\"].MACHINING_WORKPLANE");
                return _powerSHAPE.ActiveModel.Workplanes.FirstOrDefault(x => x.Name == wp);
            }
        }

        /// <summary>
        /// Gets the active solid for the Electrode.  Returns null if none is selected.
        /// </summary>
        public PSSolid ActiveSolid
        {
            get
            {
                string solid = _powerSHAPE.ReadStringValue(Identifier + "[\"" + Name + "\"].ACTIVE_SOLID");
                return _powerSHAPE.ActiveModel.Solids.FirstOrDefault(x => x.Name == solid);
            }
        }

        #endregion

        #region " Operations "

        /// <summary>
        /// Deletes the Electrode from PowerSHAPE and removes it from the Electrodes collection
        /// </summary>
        public override void Delete()
        {
            _powerSHAPE.ActiveModel.Electrodes.Remove(this);
        }

        #region " Edit Operations "

        /// <summary>
        /// This operation mirrors a single Electrode in a specified plane
        /// </summary>
        /// <param name="mirrorPlane">The plane about which to mirror the Electrode</param>
        /// <param name="mirrorPoint">The origin of the mirror plane</param>
        public void Mirror(Planes mirrorPlane, Point mirrorPoint)
        {
            PSEntityMirrorer.MirrorEntity(this, mirrorPlane, mirrorPoint);
        }

        /// <summary>
        /// This operation moves a single Electrode by the relative distance between two absolute positions
        /// </summary>
        /// <param name="moveOriginCoordinates">The first of the two absolute positions</param>
        /// <param name="pointToMoveTo">The second of the two absolute positions</param>
        /// <param name="copiesToCreate">The number of copies that should be created by the operation</param>
        /// <returns>A list containing the entity moved</returns>
        public List<PSEntity> MoveBetweenPoints(Point moveOriginCoordinates, Point pointToMoveTo, int copiesToCreate)
        {
            return PSEntityMover.MoveEntityBetweenPoints(this, moveOriginCoordinates, pointToMoveTo, copiesToCreate);
        }

        /// <summary>
        /// This operation moves a single Electrode by a specified relative amount
        /// </summary>
        /// <param name="moveVector">The relative amount by which the Electrode will be moved</param>
        /// <param name="copiesToCreate">The number of copies that should be created by the operation</param>
        /// <returns>A list containing the entity moved</returns>
        public List<PSEntity> MoveByVector(Vector moveVector, int copiesToCreate)
        {
            return PSEntityMover.MoveEntityByVector(this, moveVector, copiesToCreate);
        }

        /// <summary>
        /// This operation rotates a single Electrode by a specified angle around a specified axis
        /// </summary>
        /// <param name="rotationAxis">The axis around which the Electrode is are to be rotated</param>
        /// <param name="rotationAngle">The angle by which the Electrode is to be rotated</param>
        /// <param name="copiesToCreate">The number of copies to create of the original Electrode</param>
        /// <param name="rotationOrigin">The origin of the rotation axis</param>
        /// <returns>A list of Electrodes created by the operation.  If numberOfCopies is 0, an empty list is returned</returns>
        public List<PSEntity> Rotate(Axes rotationAxis, Degree rotationAngle, int copiesToCreate, Point rotationOrigin = null)
        {
            return PSEntityRotater.RotateEntity(this, rotationAxis, rotationAngle, copiesToCreate, rotationOrigin);
        }

        /// <summary>
        /// Scales the Electrode a different amount in each axis
        /// </summary>
        /// <param name="scaleFactorX">Scale factor for the X axis</param>
        /// <param name="scaleFactorY">Scale factor for the Y axis</param>
        /// <param name="scaleFactorZ">Scale factor for the Z axis</param>
        /// <param name="lockX">Whether to use the X scale or not</param>
        /// <param name="lockY">Whether to use the Y scale or not</param>
        /// <param name="lockZ">Whether to use the Z scale or not</param>
        /// <param name="scaleOrigin">Origin for the scale operation</param>
        public void ScaleNonUniform(
            double scaleFactorX,
            double scaleFactorY,
            double scaleFactorZ,
            bool lockX,
            bool lockY,
            bool lockZ,
            Point scaleOrigin = null)
        {
            PSEntityScaler.ScaleNonUniform(this, scaleFactorX, scaleFactorY, scaleFactorZ, lockX, lockY, lockZ, scaleOrigin);
        }

        /// <summary>
        /// Scales the Electrode to a projected volume
        /// </summary>
        /// <param name="newVolume">Volume to scale to</param>
        /// <param name="lockX">Whether to use the X scale or not</param>
        /// <param name="lockY">Whether to use the Y scale or not</param>
        /// <param name="lockZ">Whether to use the Z scale or not</param>
        /// <param name="scaleOrigin">Origin for the scale operation</param>
        public void ScaleProjectedVolume(double newVolume, bool lockX, bool lockY, bool lockZ, Point scaleOrigin = null)
        {
            PSEntityScaler.ScaleProjectedVolume(this, newVolume, lockX, lockY, lockZ, scaleOrigin);
        }

        /// <summary>
        /// Scales the Electrode the same amount in each axis
        /// </summary>
        /// <param name="scaleFactor">Scale factor for the axes</param>
        /// <param name="lockX">Whether to use the X scale or not</param>
        /// <param name="lockY">Whether to use the Y scale or not</param>
        /// <param name="lockZ">Whether to use the Z scale or not</param>
        /// <param name="scaleOrigin">Origin for the scale operation</param>
        public void ScaleUniform(double scaleFactor, bool lockX, bool lockY, bool lockZ, Point scaleOrigin = null)
        {
            PSEntityScaler.ScaleUniform(this, scaleFactor, lockX, lockY, lockZ, scaleOrigin);
        }

        #endregion

        #endregion
    }
}