// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System.Collections.Generic;
using System.Linq;

namespace Autodesk.ProductInterface.PowerMILL
{
    /// <summary>
    /// Collection of Tool objects in the Active PowerMILL Project.
    /// </summary>
    public class PMToolsCollection : PMEntitiesCollection<PMTool>
    {
        #region Constructors

        /// <summary>
        /// Initialises the item.
        /// </summary>
        /// <param name="powerMILL">The base instance to interact with PowerMILL.</param>
        public PMToolsCollection(PMAutomation powerMILL) : base(powerMILL)
        {
            Initialise();
        }

        #endregion

        #region Operations

        /// <summary>
        /// Initialises the list of Tools
        /// </summary>
        internal void Initialise()
        {
            foreach (string name in ReadTools())
            {
                Add(PMToolEntityFactory.CreateEntity(_powerMILL, name));
            }
        }

        /// <summary>
        /// Gets the list of names of all tools in PowerMILL
        /// </summary>
        internal List<string> ReadTools()
        {
            List<string> names = new List<string>();
            foreach (var tool in _powerMILL.PowerMILLProject.Tools)
            {
                names.Add(tool.Name);
            }
            return names;
        }

        /// <summary>
        /// Creates a new ball nosed tool.
        /// </summary>
        public PMToolBallNosed CreateBallNosedTool()
        {
            _powerMILL.DoCommand("CREATE TOOL ; BALLNOSED");
            var newTool = _powerMILL.ActiveProject.CreatedItems(typeof(PMTool)).FirstOrDefault();
            _powerMILL.ActiveProject.AddEntityToCollection(newTool);
            return (PMToolBallNosed) newTool;
        }

        /// <summary>
        /// Creates a new barrel tool.
        /// </summary>
        public PMToolBarrel CreateBarrelTool()
        {
            _powerMILL.DoCommand("CREATE TOOL ; BARREL");
            var newTool = _powerMILL.ActiveProject.CreatedItems(typeof(PMTool)).FirstOrDefault();
            _powerMILL.ActiveProject.AddEntityToCollection(newTool);
            return (PMToolBarrel) newTool;
        }

        /// <summary>
        /// Creates a new dovetail tool.
        /// </summary>
        public PMToolDovetail CreateDovetailTool()
        {
            _powerMILL.DoCommand("CREATE TOOL ; DOVETAIL");
            var newTool = _powerMILL.ActiveProject.CreatedItems(typeof(PMTool)).FirstOrDefault();
            _powerMILL.ActiveProject.AddEntityToCollection(newTool);
            return (PMToolDovetail) newTool;
        }

        /// <summary>
        /// Creates a new drill tool.
        /// </summary>
        public PMToolDrill CreateDrillTool()
        {
            _powerMILL.DoCommand("CREATE TOOL ; DRILL");
            var newTool = _powerMILL.ActiveProject.CreatedItems(typeof(PMTool)).FirstOrDefault();
            _powerMILL.ActiveProject.AddEntityToCollection(newTool);
            return (PMToolDrill) newTool;
        }

        /// <summary>
        /// Creates a new end mill tool.
        /// </summary>
        public PMToolEndMill CreateEndMillTool()
        {
            _powerMILL.DoCommand("CREATE TOOL ; ENDMILL");
            var newTool = _powerMILL.ActiveProject.CreatedItems(typeof(PMTool)).FirstOrDefault();
            _powerMILL.ActiveProject.AddEntityToCollection(newTool);
            return (PMToolEndMill) newTool;
        }

        /// <summary>
        /// Creates a new form tool.
        /// </summary>
        public PMToolForm CreateFormTool()
        {
            _powerMILL.DoCommand("CREATE TOOL ; FORM");
            var newTool = _powerMILL.ActiveProject.CreatedItems(typeof(PMTool)).FirstOrDefault();
            _powerMILL.ActiveProject.AddEntityToCollection(newTool);
            return (PMToolForm) newTool;
        }

        /// <summary>
        /// Creates a new off centre tip radiused tool.
        /// </summary>
        public PMToolOffCentreTipRadiused CreateOffCentreTipRadiusedTool()
        {
            _powerMILL.DoCommand("CREATE TOOL ; OFFCENTRETIPRAD");
            var newTool = _powerMILL.ActiveProject.CreatedItems(typeof(PMTool)).FirstOrDefault();
            _powerMILL.ActiveProject.AddEntityToCollection(newTool);
            return (PMToolOffCentreTipRadiused) newTool;
        }

        /// <summary>
        /// Creates a new routing tool.
        /// </summary>
        public PMToolRouting CreateRoutingTool()
        {
            _powerMILL.DoCommand("CREATE TOOL ; ROUTING");
            var newTool = _powerMILL.ActiveProject.CreatedItems(typeof(PMTool)).FirstOrDefault();
            _powerMILL.ActiveProject.AddEntityToCollection(newTool);
            return (PMToolRouting) newTool;
        }

        /// <summary>
        /// Creates a new tap tool.
        /// </summary>
        public PMToolTap CreateTapTool()
        {
            _powerMILL.DoCommand("CREATE TOOL ; TAP");
            var newTool = _powerMILL.ActiveProject.CreatedItems(typeof(PMTool)).FirstOrDefault();
            _powerMILL.ActiveProject.AddEntityToCollection(newTool);
            return (PMToolTap) newTool;
        }

        /// <summary>
        /// Creates a new tapered spherical tool.
        /// </summary>
        public PMToolTaperedSpherical CreateTaperedSphericalTool()
        {
            _powerMILL.DoCommand("CREATE TOOL ; TAPERSPHERICAL");
            var newTool = _powerMILL.ActiveProject.CreatedItems(typeof(PMTool)).FirstOrDefault();
            _powerMILL.ActiveProject.AddEntityToCollection(newTool);
            return (PMToolTaperedSpherical) newTool;
        }

        /// <summary>
        /// Creates a new tapered tipped tool.
        /// </summary>
        public PMToolTaperedTipped CreateTaperedTippedTool()
        {
            _powerMILL.DoCommand("CREATE TOOL ; TAPERTIPPED");
            var newTool = _powerMILL.ActiveProject.CreatedItems(typeof(PMTool)).FirstOrDefault();
            _powerMILL.ActiveProject.AddEntityToCollection(newTool);
            return (PMToolTaperedTipped) newTool;
        }

        /// <summary>
        /// Creates a new thread mill tool.
        /// </summary>
        public PMToolThreadMill CreateThreadMillTool()
        {
            _powerMILL.DoCommand("CREATE TOOL ; THREADMILL");
            var newTool = _powerMILL.ActiveProject.CreatedItems(typeof(PMTool)).FirstOrDefault();
            _powerMILL.ActiveProject.AddEntityToCollection(newTool);
            return (PMToolThreadMill) newTool;
        }

        /// <summary>
        /// Creates a new tipped disc tool.
        /// </summary>
        public PMToolTippedDisc CreateTippedDiscTool()
        {
            _powerMILL.DoCommand("CREATE TOOL ; TIPPEDDISC");
            var newTool = _powerMILL.ActiveProject.CreatedItems(typeof(PMTool)).FirstOrDefault();
            _powerMILL.ActiveProject.AddEntityToCollection(newTool);
            return (PMToolTippedDisc) newTool;
        }

        /// <summary>
        /// Creates a new tip radiused tool.
        /// </summary>
        public PMToolTipRadiused CreateTipRadiusedTool()
        {
            _powerMILL.DoCommand("CREATE TOOL ; TIPRADIUSED");
            var newTool = _powerMILL.ActiveProject.CreatedItems(typeof(PMTool)).FirstOrDefault();
            _powerMILL.ActiveProject.AddEntityToCollection(newTool);
            return (PMToolTipRadiused) newTool;
        }
        #endregion
    }
}