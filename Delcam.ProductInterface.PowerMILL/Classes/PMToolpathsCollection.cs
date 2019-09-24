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

namespace Autodesk.ProductInterface.PowerMILL
{
    /// <summary>
    /// Collection of Toolpaths objects in the Active PowerMILL Project
    /// </summary>
    public class PMToolpathsCollection : PMEntitiesCollection<PMToolpath>
    {
        #region Constructors

        /// <summary>
        /// Initialises the item.
        /// </summary>
        /// <param name="powerMILL">The base instance to interact with PowerMILL.</param>
        internal PMToolpathsCollection(PMAutomation powerMILL) : base(powerMILL)
        {
            Initialise();
        }

        #endregion

        #region Operations

        /// <summary>
        /// Initialises the collection with the items in PowerMILL.
        /// </summary>
        internal void Initialise()
        {
            foreach (string name in ReadToolpaths())
            {
                Add(PMToolpathEntityFactory.CreateEntity(_powerMILL, name));
            }
        }

        /// <summary>
        /// Gets a list of names of all Toolpaths in PowerMILL.
        /// </summary>
        internal List<string> ReadToolpaths()
        {
            List<string> names = new List<string>();
            foreach (var toolpath in _powerMILL.PowerMILLProject.Toolpaths)
            {
                names.Add(toolpath.Name);
            }
            return names;
        }

        /// <summary>
        /// Creates a 3DOffsetFinishing Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpath3DOffsetFinishing Create3DOffsetFinishingToolpath()
        {
            _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"Finishing\\3D-Offset-Finishing.ptf\"");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpath3DOffsetFinishing) toolpath;
        }

        /// <summary>
        /// Creates an AlongCornerFinishing Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathAlongCornerFinishing CreateAlongCornerFinishingToolpath()
        {
            _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"Finishing\\Corner-Finishing.ptf\"",
                                 "EDIT PAR 'Strategy' 'along_corner'",
                                 "EDIT TOOLPATH ; REAPPLYFROMGUI");
            var toolpath = (PMToolpath) _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add(toolpath);
            return (PMToolpathAlongCornerFinishing) toolpath;
        }

        /// <summary>
        /// Creates a BladeFinishing Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathBladeFinishing CreateBladeFinishingToolpath()
        {
            _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"Blisks\\Blade-Finishing.ptf\"");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathBladeFinishing) toolpath;
        }

        /// <summary>
        /// Creates a BliskAreaClearance Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathBliskAreaClearance CreateBliskAreaClearanceToolpath()
        {
            _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"Blisks\\Blisk-Area-Clearance.ptf\"");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathBliskAreaClearance) toolpath;
        }

        /// <summary>
        /// Creates a ChamferMilling Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathChamferMilling CreateChamferMillingToolpath()
        {
            _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"2.5D-Area-Clearance\\Chamfer-Milling.ptf\"");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathChamferMilling) toolpath;
        }

        /// <summary>
        /// Creates a ConstantZFinishing Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathConstantZFinishing CreateConstantZFinishingToolpath()
        {
            if (_powerMILL.Version.Major <= 17)
            {
                _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"Finishing\\Constant-Z-Finishing.ptf\"");
            }
            else
            {
                _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"Finishing\\Constant-Z-Finishing.002.ptf\"");
            }
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathConstantZFinishing) toolpath;
        }

        /// <summary>
        /// Creates a CornerClearance Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathCornerClearance CreateCornerClearanceToolpath()
        {
            _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"3D-Area-Clearance\\Corner-Clearance.ptf\"");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathCornerClearance) toolpath;
        }

        /// <summary>
        /// Creates a CornerFinishing Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathCornerFinishing CreateCornerFinishingToolpath()
        {
            _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"Finishing\\Corner-Finishing.ptf\"");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathCornerFinishing) toolpath;
        }

        /// <summary>
        /// Creates a CurveAreaClearance Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathCurveAreaClearance CreateCurveAreaClearanceToolpath()
        {
            _powerMILL.DoCommand(
                "IMPORT TEMPLATE ENTITY TOOLPATH \"2.5D-Area-Clearance\\2D-Curve-Area-Clearance.002.ptf\"");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathCurveAreaClearance) toolpath;
        }

        /// <summary>
        /// Creates a CurveProfile Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathCurveProfile CreateCurveProfileToolpath()
        {
            _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"2.5D-Area-Clearance\\2D-Curve-Profile.002.ptf\"");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathCurveProfile) toolpath;
        }

        /// <summary>
        /// Creates a CurveProjectionFinishing Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathCurveProjectionFinishing CreateCurveProjectionFinishingToolpath()
        {
            _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"Finishing\\Projection-Curve-Finishing.ptf\"");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathCurveProjectionFinishing) toolpath;
        }

        /// <summary>
        /// Creates a DiscProfileFinishing Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathDiscProfileFinishing CreateDiscProfileFinishingToolpath()
        {
            _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"Finishing\\Disc-Profile-Finishing.ptf\"");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathDiscProfileFinishing) toolpath;
        }

        /// <summary>
        /// Creates a BreakChipDrilling Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathDrilling CreateBreakChipDrillingToolpath()
        {
            _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"Drilling\\Break-Chip.ptf\"");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathDrilling) toolpath;
        }

        /// <summary>
        /// Creates a CounterBoreDrilling Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathDrilling CreateCounterBoreDrillingToolpath()
        {
            _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"Drilling\\Counter-Bore.ptf\"");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathDrilling) toolpath;
        }

        /// <summary>
        /// Creates a DeepDrillDrilling Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathDrilling CreateDeepDrillDrillingToolpath()
        {
            _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"Drilling\\Deep-Drill.ptf\"");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathDrilling) toolpath;
        }

        /// <summary>
        /// Creates a Drilling Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathDrilling CreateDrillingToolpath()
        {
            _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"Drilling\\Drilling.ptf\"");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathDrilling) toolpath;
        }

        /// <summary>
        /// Creates a ExternalThreadDrilling Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathDrilling CreateExternalThreadDrillingToolpath()
        {
            _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"Drilling\\External-Thread.ptf\"");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathDrilling) toolpath;
        }

        /// <summary>
        /// Creates a FineBoringDrilling Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathDrilling CreateFineBoringDrillingToolpath()
        {
            _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"Drilling\\Fine-Boring.ptf\"");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathDrilling) toolpath;
        }

        /// <summary>
        /// Creates a HelicalDrilling Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathDrilling CreateHelicalDrillingToolpath()
        {
            _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"Drilling\\Helical.ptf\"");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathDrilling) toolpath;
        }

        /// <summary>
        /// Creates a ProfileDrilling Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathDrilling CreateProfileDrillingToolpath()
        {
            _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"Drilling\\Profile.ptf\"");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathDrilling) toolpath;
        }

        /// <summary>
        /// Creates a ReamDrilling Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathDrilling CreateReamDrillingToolpath()
        {
            _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"Drilling\\Ream.ptf\"");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathDrilling) toolpath;
        }

        /// <summary>
        /// Creates a RigidTappingDrilling Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathDrilling CreateRigidTappingDrillingToolpath()
        {
            _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"Drilling\\Rigid-Tapping.ptf\"");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathDrilling) toolpath;
        }

        /// <summary>
        /// Creates a SinglePeckDrilling Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathDrilling CreateSinglePeckDrillingToolpath()
        {
            _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"Drilling\\Single-Peck.ptf\"");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathDrilling) toolpath;
        }

        /// <summary>
        /// Creates a TapDrilling Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathDrilling CreateTapDrillingToolpath()
        {
            _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"Drilling\\Tap.ptf\"");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathDrilling) toolpath;
        }

        /// <summary>
        /// Creates a ThreadMillDrilling Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathDrilling CreateThreadMillDrillingToolpath()
        {
            _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"Drilling\\Thread-Mill.ptf\"");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathDrilling) toolpath;
        }

        /// <summary>
        /// Creates a EmbeddedPatternFinishing Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathEmbeddedPatternFinishing CreateEmbeddedPatternFinishingToolpath()
        {
            _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"Finishing\\Embedded-Pattern-Finishing.ptf\"");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathEmbeddedPatternFinishing) toolpath;
        }

        /// <summary>
        /// Creates a FaceMilling Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathFaceMilling CreateFaceMillingToolpath()
        {
            _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"2.5D-Area-Clearance\\Face-Milling.ptf\"");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathFaceMilling) toolpath;
        }

        /// <summary>
        /// Creates a FlowlineFinishing Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathFlowlineFinishing CreateFlowlineFinishingToolpath()
        {
            _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"Finishing\\Flowline-Finishing.ptf\"");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathFlowlineFinishing) toolpath;
        }

        /// <summary>
        /// Creates a HubFinishing Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathHubFinishing CreateHubFinishingToolpath()
        {
            _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"Blisks\\Hub-Finishing.ptf\"");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathHubFinishing) toolpath;
        }

        /// <summary>
        /// Creates a LineProjectionFinishing Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathLineProjectionFinishing CreateLineProjectionFinishingToolpath()
        {
            _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"Finishing\\Projection-Line-Finishing.ptf\"");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathLineProjectionFinishing) toolpath;
        }

        /// <summary>
        /// Creates a MultiPencilCornerFinishing Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathMultiPencilCornerFinishing CreateMultiPencilCornerFinishingToolpath()
        {
            _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"Finishing\\Corner-Multi--pencil-Finishing.ptf\"");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathMultiPencilCornerFinishing) toolpath;
        }

        /// <summary>
        /// Creates an OffsetAreaClearance Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathOffsetAreaClearance CreateOffsetAreaClearanceToolpath()
        {
            _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"3D-Area-Clearance\\Model-Area-Clearance.003.ptf\"",
                                 "EDIT PAR 'Strategy' 'offset_area_clear'",
                                 "EDIT TOOLPATH ; REAPPLYFROMGUI");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathOffsetAreaClearance) toolpath;
        }

        /// <summary>
        /// Creates an OffsetFlatFinishing Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathOffsetFlatFinishing CreateOffsetFlatFinishingToolpath()
        {
            _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"Finishing\\Offset-Flat-Finishing.ptf\"");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathOffsetFlatFinishing) toolpath;
        }

        /// <summary>
        /// Creates an OptimisedConstantZFinishing Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathOptimisedConstantZFinishing CreateOptimisedConstantZFinishingToolpath()
        {
            if (_powerMILL.Version.Major <= 17)
            {
                _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"Finishing\\Optimised-Constant-Z-Finishing.ptf\"");
            }
            else
            {
                _powerMILL.DoCommand(
                    "IMPORT TEMPLATE ENTITY TOOLPATH \"Finishing\\Optimised-Constant-Z-Finishing.002.ptf\"");
            }
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathOptimisedConstantZFinishing) toolpath;
        }

        /// <summary>
        /// Creates a ParametricOffsetFinishing Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathParametricOffsetFinishing CreateParametricOffsetFinishingToolpath()
        {
            _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"Finishing\\Parametric-Offset-Finishing.ptf\"");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathParametricOffsetFinishing) toolpath;
        }

        /// <summary>
        /// Creates a ParametricSpiralFinishing Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathParametricSpiralFinishing CreateParametricSpiralFinishingToolpath()
        {
            _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"Finishing\\Parametric-Spiral-Finishing.ptf\"");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathParametricSpiralFinishing) toolpath;
        }

        /// <summary>
        /// Creates a PatternFinishing Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathPatternFinishing CreatePatternFinishingToolpath()
        {
            _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"Finishing\\Pattern-Finishing.ptf\"");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathPatternFinishing) toolpath;
        }

        /// <summary>
        /// Creates a PencilCornerFinishing Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathPencilCornerFinishing CreatePencilCornerFinishingToolpath()
        {
            _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"Finishing\\Corner-Pencil-Finishing.ptf\"");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathPencilCornerFinishing) toolpath;
        }

        /// <summary>
        /// Creates a PlaneProjectionFinishing Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathPlaneProjectionFinishing CreatePlaneProjectionFinishingToolpath()
        {
            _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"Finishing/Projection-Plane-Finishing.ptf\"");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathPlaneProjectionFinishing) toolpath;
        }

        /// <summary>
        /// Creates a PlungeMilling Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathPlungeMilling CreatePlungeMillingToolpath()
        {
            _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"3D-Area-Clearance\\Plunge-Milling.ptf\"");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathPlungeMilling) toolpath;
        }

        /// <summary>
        /// Creates a PointProjectionFinishing Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathPointProjectionFinishing CreatePointProjectionFinishingToolpath()
        {
            _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"Finishing\\Projection-Point-Finishing.ptf\"");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathPointProjectionFinishing) toolpath;
        }

        /// <summary>
        /// Creates a PortAreaClearance Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathPortAreaClearance CreatePortAreaClearanceToolpath()
        {
            _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"Ports\\Port-Area-Clearance.ptf\"");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathPortAreaClearance) toolpath;
        }

        /// <summary>
        /// Creates a PortPlungeFinishing Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathPortPlungeFinishing CreatePortPlungeFinishingToolpath()
        {
            _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"Ports\\Port-Plunge-Finishing.ptf\"");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathPortPlungeFinishing) toolpath;
        }

        /// <summary>
        /// Creates a PortSpiralFinishing Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathPortSpiralFinishing CreatePortSpiralFinishingToolpath()
        {
            _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"Ports\\Port-Spiral-Finishing.ptf\"");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathPortSpiralFinishing) toolpath;
        }

        /// <summary>
        /// Creates a ProfileAreaClearance Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathProfileAreaClearance CreateProfileAreaClearanceToolpath()
        {
            _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"3D-Area-Clearance\\Model-Profile.002.ptf\"");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathProfileAreaClearance) toolpath;
        }

        /// <summary>
        /// Creates a ProfileFinishing Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathProfileFinishing CreateProfileFinishingToolpath()
        {
            _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"Finishing\\Profile-Finishing.ptf\"");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathProfileFinishing) toolpath;
        }

        /// <summary>
        /// Creates a RadialFinishing Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathRadialFinishing CreateRadialFinishingToolpath()
        {
            _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"Finishing\\Radial-Finishing.ptf\"");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathRadialFinishing) toolpath;
        }

        /// <summary>
        /// Creates a RasterAreaClearance Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathRasterAreaClearance CreateRasterAreaClearanceToolpath()
        {
            _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"3D-Area-Clearance\\Model-Area-Clearance.003.ptf\"",
                                 "EDIT PAR 'Strategy' 'raster_area_clear'",
                                 "EDIT TOOLPATH ; REAPPLYFROMGUI");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathRasterAreaClearance) toolpath;
        }

        /// <summary>
        /// Creates an AdaptiveAreaClearance Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathAdaptiveAreaClearance CreateAdaptiveAreaClearanceToolpath()
        {
            _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"3D-Area-Clearance\\Model-Area-Clearance.003.ptf\"",
                                 "EDIT PAR 'Strategy' 'adaptive_area_clear'",
                                 "EDIT TOOLPATH ; REAPPLYFROMGUI");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathAdaptiveAreaClearance) toolpath;
        }

        /// <summary>
        /// Creates a RasterFinishing Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathRasterFinishing CreateRasterFinishingToolpath()
        {
            _powerMILL.DoCommand(_powerMILL.Version.Major <= 17
                                     ? "IMPORT TEMPLATE ENTITY TOOLPATH \"Finishing\\Raster-Finishing.ptf\""
                                     : "IMPORT TEMPLATE ENTITY TOOLPATH \"Finishing\\Raster-Finishing.002.ptf\"");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathRasterFinishing) toolpath;
        }

        /// <summary>
        /// Creates a RasterFlatFinishing Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathRasterFlatFinishing CreateRasterFlatFinishingToolpath()
        {
            _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"Finishing\\Raster-Flat-Finishing.ptf\"");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathRasterFlatFinishing) toolpath;
        }

        /// <summary>
        /// Creates a RotaryFinishing Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathRotaryFinishing CreateRotaryFinishingToolpath()
        {
            _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"Finishing\\Rotary-Finishing.ptf\"");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathRotaryFinishing) toolpath;
        }

        /// <summary>
        /// Creates a SpiralFinishing Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathSpiralFinishing CreateSpiralFinishingToolpath()
        {
            _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"Finishing\\Spiral-Finishing.ptf\"");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathSpiralFinishing) toolpath;
        }

        /// <summary>
        /// Creates a SteepAndShallowFinishingToolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathSteepAndShallowFinishing CreateSteepAndShallowFinishingToolpath()
        {
            _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"Finishing\\Steep-and-Shallow-Finishing.002.ptf\"");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathSteepAndShallowFinishing) toolpath;
        }

        /// <summary>
        /// Creates a StitchCornerFinishing Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathStitchCornerFinishing CreateStitchCornerFinishingToolpath()
        {
            _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"Finishing\\Corner-Finishing.ptf\"",
                                 "EDIT PAR 'Strategy' 'stitch_corner'",
                                 "EDIT TOOLPATH ; REAPPLYFROMGUI");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathStitchCornerFinishing) toolpath;
        }

        /// <summary>
        /// Creates a SurfaceFinishing Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathSurfaceFinishing CreateSurfaceFinishingToolpath()
        {
            _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"Finishing\\Surface-Finishing.ptf\"");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathSurfaceFinishing) toolpath;
        }

        /// <summary>
        /// Creates a SurfaceProjectionFinishingToolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathSurfaceProjectionFinishing CreateSurfaceProjectionFinishingToolpath()
        {
            _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"Finishing\\Projection-Surface-Finishing.ptf\"");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathSurfaceProjectionFinishing) toolpath;
        }

        /// <summary>
        /// Creates a SwarfMachining Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathSwarfMachining CreateSwarfMachiningToolpath()
        {
            _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"Finishing/Swarf-Finishing.ptf\"");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathSwarfMachining) toolpath;
        }

        /// <summary>
        /// Creates a WireframeProfileMachining Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathWireframeProfileMachining CreateWireframeProfileMachiningToolpath()
        {
            _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"Finishing\\Wireframe-Profile-Machining.ptf\"");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathWireframeProfileMachining) toolpath;
        }

        /// <summary>
        /// Creates a WireframeSwarfMachining Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathWireframeSwarfMachining CreateWireframeSwarfMachiningToolpath()
        {
            _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"Finishing\\Wireframe-Swarf-Finishing.ptf\"");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathWireframeSwarfMachining) toolpath;
        }

        /// <summary>
        /// Creates a Rib Machining Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathRibMachining CreateRibMachiningToolpath()
        {
            if (_powerMILL.Version.Major < PMEntity.POWER_MILL_2016.Major)
            {
                throw new Exception("Rib machining toolpaths are not supported in this version of PowerMILL");
            }
            _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"Ribs\\Rib-Machining.ptf\"");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathRibMachining) toolpath;
        }

        /// <summary>
        /// Creates a Blade Machining Toolpath
        /// </summary>
        /// <returns>The created toolpath</returns>
        public PMToolpathBladeMachining CreateBladeMachiningToolpath()
        {
            if (_powerMILL.Version.Major < PMEntity.POWER_MILL_2016.Major)
            {
                throw new Exception("Blade finishing toolpaths are not supported in this version of PowerMILL");
            }
            _powerMILL.DoCommand("IMPORT TEMPLATE ENTITY TOOLPATH \"Blisks\\Single-Blade-Finishing.ptf\"");
            var toolpath = _powerMILL.ActiveProject.CreatedItems(typeof(PMToolpath)).FirstOrDefault();
            if (toolpath == null)
            {
                throw new Exception("Failed to create toolpath");
            }
            Add((PMToolpath) toolpath);
            return (PMToolpathBladeMachining) toolpath;
        }

        /// <summary>
        /// Deletes the toolpath from active project.
        /// </summary>
        /// <param name="toolpathName">The toolpath name.</param>
        /// <param name="useNoQuibble">Force deletion</param>
        /// <remarks></remarks>
        public void DeleteToolpath(string toolpathName, bool useNoQuibble = false)
        {
            PMToolpath tp = this[toolpathName];
            if (tp != null)
            {
                Remove(tp, useNoQuibble);
            }
        }
        #endregion
    }
}