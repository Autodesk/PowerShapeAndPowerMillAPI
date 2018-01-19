// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using System.Linq;
using Autodesk.Geometry;
using Autodesk.ProductInterface.Properties;

namespace Autodesk.ProductInterface.PowerMILL
{
    /// <summary>
    /// Captures a Toolpath in PowerMILL.
    /// </summary>
    public partial class PMToolpath : PMEntity
    {
        #region Fields

        /// <summary>
        /// The safety of the tool during Cutting Moves.
        /// </summary>
        private ToolpathSafety _toolCuttingMovesSafety;

        /// <summary>
        /// The safety of the tool during Links.
        /// </summary>
        private ToolpathSafety _toolLinksSafety;

        /// <summary>
        /// The safety of the tool during Leads.
        /// </summary>
        private ToolpathSafety _toolLeadsSafety;

        /// <summary>
        /// The safety of the holder during Cutting Moves.
        /// </summary>
        private ToolpathSafety _holderCuttingMovesSafety;

        /// <summary>
        /// The safety of the holder during Links.
        /// </summary>
        private ToolpathSafety _holderLinksSafety;

        /// <summary>
        /// The safety of the holder during Leads.
        /// </summary>
        private ToolpathSafety _holderLeadsSafety;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialises the item.
        /// </summary>
        /// <param name="powerMill">The base instance to interact with PowerMILL.</param>
        internal PMToolpath(PMAutomation powerMill) : base(powerMill)
        {
            InitializeLeads();
        }

        /// <summary>
        /// Initialises the item.
        /// </summary>
        /// <param name="powerMill">The base instance to interact with PowerMILL.</param>
        /// <param name="name">The new instance name.</param>
        internal PMToolpath(PMAutomation powerMill, string name) : base(powerMill, name)
        {
            InitializeLeads();
        }

        #endregion

        #region Properties

        internal static string TOOLPATH_IDENTIFIER = "TOOLPATH";

        private PMConnections _connections;

        /// <summary>
        /// Gets the identifier PowerMILL uses to identify this type of entity.
        /// </summary>
        internal override string Identifier
        {
            get { return TOOLPATH_IDENTIFIER; }
        }

        /// <summary>
        /// Gets the total cut length of the Toolpath from PowerMill.
        /// </summary>
        public MM TotalCutLength
        {
            get
            {
                if (PowerMill.Version.Major > 12)
                {
                    //PowerMill 2012 + 
                    //uses different method of retrieving toolpath stats.
                    var arcLength =
                        Convert.ToDouble(
                            PowerMill.DoCommandEx("PRINT PAR terse \"entity('toolpath','" + Name +
                                                  "').Statistics.CuttingMoves.Lengths.arcs\""));
                    var linearLength =
                        Convert.ToDouble(
                            PowerMill.DoCommandEx("PRINT PAR terse \"entity('toolpath','" + Name +
                                                  "').Statistics.CuttingMoves.Lengths.Linear\""));
                    return arcLength + linearLength;

                    // PowerMill 2011 and under 
                }
                IsActive = true;
                PowerMill.DoCommand("FORM TOOLPATHSTATS", "TOOLPATHSTATS CANCEL");
                var length =
                    Convert.ToDouble(
                        PowerMill.DoCommandEx("print formvalue ToolpathStats.TotalCutLength").ToString().TrimEnd());
                IsActive = false;
                return length;
            }
        }

        /// <summary>
        /// <see cref="PMLead"/>s container, and base for common <see cref="PMLead"/> options
        /// </summary>
        public PMConnections Connections
        {
            get { return _connections; }
        }

        /// <summary>
        /// Gets the name of the tool associated with this toolpath from PowerMill.
        /// </summary>
        public string ToolName
        {
            get { return GetParameter(Resources.ToolName); }
        }

        /// <summary>
        /// Gets the number of the tool associated with this toolpath from PowerMill.
        /// </summary>
        public int ToolNumber
        {
            get { return Convert.ToInt32(GetParameter(Resources.ToolNumber)); }
        }

        /// <summary>
        /// Gets the type of the tool associated with this toolpath.
        /// </summary>
        public string ToolType
        {
            get { return GetParameter("tool.type"); }
        }

        /// <summary>
        /// Gets the diameter of the tool associated with this toolpath.
        /// </summary>
        public MM ToolDiameter
        {
            get { return GetParameterDoubleValue(Resources.ToolDiameter); }
        }

        /// <summary>
        /// Gets whether or not this toolpath has been calculated.
        /// </summary>
        public bool IsCalculated
        {
            get { return GetParameterBooleanValue(Resources.IsCalculated); }
        }

        /// <summary>
        /// Gets the tool length from PowerMill.
        /// </summary>
        public MM ToolLength
        {
            get { return GetParameterDoubleValue(Resources.ToolLength); }
        }

        /// <summary>
        /// Gets the tool tip radius from PowerMill.
        /// </summary>
        public MM ToolTipRadius
        {
            get { return GetParameterDoubleValue(Resources.ToolTipRadius); }
        }

        /// <summary>
        /// Gets the tool flutes from PowerMill.
        /// </summary>
        public int ToolFlutes
        {
            get { return Convert.ToInt32(GetParameter(Resources.ToolFlutes)); }
        }

        /// <summary>
        /// Gets the tool description from PowerMill.
        /// </summary>
        public string ToolDescription
        {
            get { return GetParameter(Resources.ToolDescription); }
        }

        /// <summary>
        /// Gets the tool identifier from PowerMill.
        /// </summary>
        public string ToolIndentifier
        {
            get { return GetParameter(Resources.ToolIndentifier); }
        }

        /// <summary>
        /// Gets Tool Z axis vector based on the workplane of the toolpath.
        /// Returns 0 0 1 if can not find workplane.
        /// </summary>
        public Vector ToolZAxisVector
        {
            get
            {
                var zAxisVector = new Vector(0, 0, 1);
                PMWorkplane wp = WorkplaneInformation;
                if (wp != null)
                {
                    zAxisVector = wp.ToWorkplane().ZAxis.Clone();

                    //assume world so points up
                }
                return zAxisVector;
            }
        }

        /// <summary>
        /// Gets workplane information.
        /// </summary>
        public PMWorkplane WorkplaneInformation
        {
            get
            {
                PMWorkplane wp = null;
                string wpName = GetParameter("workplane.name");
                if (wpName.Contains("#ERROR") == false)
                {
                    wp =
                        (from wpp in PowerMill.ActiveProject.Workplanes where wpp.Name == wpName select wpp)
                        .FirstOrDefault();
                }
                else
                {
                    //save for later use
                    PMWorkplane activeWp =
                        (from wpp in PowerMill.ActiveProject.Workplanes where wpp.IsActive select wpp).FirstOrDefault();

                    //will either de-activate all workplanes (i.e. using world) Z axis = 0 0 1
                    //create new workplane or use an existing workplane
                    PowerMill.DoCommand("ACTIVATE WORKPLANE FROMENTITY TOOLPATH '" + Name + "'");
                    PowerMill.ActiveProject.Initialise();

                    //redo look up
                    PMWorkplane nowActiveWp =
                        (from wpp in PowerMill.ActiveProject.Workplanes where wpp.IsActive select wpp).FirstOrDefault();
                    if (nowActiveWp != null)
                    {
                        wp = nowActiveWp;
                        if (activeWp != null)
                        {
                            activeWp.IsActive = true;
                        }
                        else
                        {
                            wp.IsActive = false;

                            //should now mean all workplanes are not active
                        }
                    }
                }
                return wp;
            }
        }

        /// <summary>
        /// Gets the workplane from PowerMill.
        /// </summary>
        public string WorkplaneName
        {
            get
            {
                string wp = "";
                string wpName = GetParameter("workplane.name");
                if (wpName.Contains("#ERROR") == false)
                {
                    wp = wpName;
                }
                return wp;
            }
        }

        /// <summary>
        /// Gets the safety of the tool during Cutting Moves.
        /// </summary>
        public ToolpathSafety ToolCuttingMovesSafety
        {
            get
            {
                DetectToolGouges();
                return _toolCuttingMovesSafety;
            }
        }

        /// <summary>
        /// Gets the safety of the tool during Links.
        /// </summary>
        public ToolpathSafety ToolLinksSafety
        {
            get
            {
                DetectToolGouges();
                return _toolLinksSafety;
            }
        }

        /// <summary>
        /// Gets the safety of the tool during Leads.
        /// </summary>
        public ToolpathSafety ToolLeadsSafety
        {
            get
            {
                DetectToolGouges();
                return _toolLeadsSafety;
            }
        }

        /// <summary>
        /// Gets safety of the holder during Cutting Moves.
        /// </summary>
        public ToolpathSafety HolderCuttingMovesSafety
        {
            get
            {
                DetectHolderCollisions();
                return _holderCuttingMovesSafety;
            }
        }

        /// <summary>
        /// Gets the safety of the holder during Links.
        /// </summary>
        public ToolpathSafety HolderLinksSafety
        {
            get
            {
                DetectHolderCollisions();
                return _holderLinksSafety;
            }
        }

        /// <summary>
        /// Gets the safety of the holder during Leads.
        /// </summary>
        public ToolpathSafety HolderLeadsSafety
        {
            get
            {
                DetectHolderCollisions();
                return _holderLeadsSafety;
            }
        }

        /// <summary>
        /// Gets the number of segments in the toolpath
        /// </summary>
        public int NumberOfSegments
        {
            get
            {
                return int.Parse(
                    PowerMill.DoCommandEx("print par terse ${toolpath_component_count('toolpath', '" + Name + "', 'segments')}")
                             .ToString());
            }
        }

        /// <summary>
        /// Gets the number of points in the specified segment of the toolpath
        /// </summary>
        public int NumberOfPointsInSegment(int segmentNumber)
        {
            var numberOfSegments = NumberOfSegments;
            if (segmentNumber < numberOfSegments)
            {
                return
                    (int) double.Parse(
                        PowerMill.DoCommandEx("print par terse ${segment_point_count(entity('toolpath', '" + Name + "'), " +
                                              segmentNumber + ")}").ToString());
            }
            throw new IndexOutOfRangeException(
                $"{segmentNumber} is greater than the {numberOfSegments} segments in this toolpath.");
        }

        public MM SegmentLength(int segmentNumber)
        {
            var numberOfSegments = NumberOfSegments;
            if (segmentNumber < numberOfSegments)
            {
                return
                    double.Parse(
                        PowerMill.DoCommandEx("print par terse ${segment_get_length(entity('toolpath', '" + Name + "'), " +
                                              segmentNumber + ")}").ToString());
            }
            throw new IndexOutOfRangeException(
                $"{segmentNumber} is greater than the {numberOfSegments} segments in this toolpath.");
        }

        /// <summary>
        /// Gets the position of the specified point in the specified segment
        /// </summary>
        /// <param name="segmentNumber">The segment number of the toolpath (indexed from zero)</param>
        /// <param name="pointNumber">The point number in the specified segment (indexed from zero)</param>
        /// <returns>The point position</returns>
        public Point ToolpathPointPosition(int segmentNumber, int pointNumber)
        {
            var numberOfPoints = NumberOfPointsInSegment(segmentNumber);
            if (pointNumber < numberOfPoints)
            {
                var x = double.Parse(PowerMill.DoCommandEx("print par terse ${segment_get_point(entity('toolpath', '" + Name +
                                                           "'), " +
                                                           segmentNumber + ", " + pointNumber + ").Position.X}").ToString());
                var y = double.Parse(PowerMill.DoCommandEx("print par terse ${segment_get_point(entity('toolpath', '" + Name +
                                                           "'), " +
                                                           segmentNumber + ", " + pointNumber + ").Position.Y}").ToString());
                var z = double.Parse(PowerMill.DoCommandEx("print par terse ${segment_get_point(entity('toolpath', '" + Name +
                                                           "'), " +
                                                           segmentNumber + ", " + pointNumber + ").Position.Z}").ToString());
                return new Point(x, y, z);
            }
            throw new IndexOutOfRangeException(
                $"{pointNumber} is greater than the {numberOfPoints} points in segment {segmentNumber} of this toolpath.");
        }

        /// <summary>
        /// Gets the tool axis vector for the specified point in the specified segment
        /// </summary>
        /// <param name="segmentNumber">The segment number of the toolpath (indexed from zero)</param>
        /// <param name="pointNumber">The point number in the specified segment (indexed from zero)</param>
        /// <returns>The tool axis vector</returns>
        public Vector ToolpathPointToolAxis(int segmentNumber, int pointNumber)
        {
            var numberOfPoints = NumberOfPointsInSegment(segmentNumber);
            if (pointNumber < numberOfPoints)
            {
                var i = double.Parse(PowerMill.DoCommandEx("print par terse ${segment_get_point(entity('toolpath', '" + Name +
                                                           "'), " +
                                                           segmentNumber + ", " + pointNumber + ").ToolAxis[0]}").ToString());
                var j = double.Parse(PowerMill.DoCommandEx("print par terse ${segment_get_point(entity('toolpath', '" + Name +
                                                           "'), " +
                                                           segmentNumber + ", " + pointNumber + ").ToolAxis[1]}").ToString());
                var k = double.Parse(PowerMill.DoCommandEx("print par terse ${segment_get_point(entity('toolpath', '" + Name +
                                                           "'), " +
                                                           segmentNumber + ", " + pointNumber + ").ToolAxis[2]}").ToString());
                return new Vector(i, j, k);
            }
            throw new IndexOutOfRangeException(
                $"{pointNumber} is greater than the {numberOfPoints} points in segment {segmentNumber} of this toolpath.");
        }

        /// <summary>
        /// Gets the orientation vector for the specified point in the specified segment
        /// </summary>
        /// <param name="segmentNumber">The segment number of the toolpath (indexed from zero)</param>
        /// <param name="pointNumber">The point number in the specified segment (indexed from zero)</param>
        /// <returns>The orientation vector</returns>
        public Vector ToolpathPointOrientationVector(int segmentNumber, int pointNumber)
        {
            var numberOfPoints = NumberOfPointsInSegment(segmentNumber);
            if (pointNumber < numberOfPoints)
            {
                var i = double.Parse(PowerMill.DoCommandEx("print par terse ${segment_get_point(entity('toolpath', '" + Name +
                                                           "'), " +
                                                           segmentNumber + ", " + pointNumber + ").OrientationVector[0]}")
                                              .ToString());
                var j = double.Parse(PowerMill.DoCommandEx("print par terse ${segment_get_point(entity('toolpath', '" + Name +
                                                           "'), " +
                                                           segmentNumber + ", " + pointNumber + ").OrientationVector[1]}")
                                              .ToString());
                var k = double.Parse(PowerMill.DoCommandEx("print par terse ${segment_get_point(entity('toolpath', '" + Name +
                                                           "'), " +
                                                           segmentNumber + ", " + pointNumber + ").OrientationVector[2]}")
                                              .ToString());
                return new Vector(i, j, k);
            }
            throw new IndexOutOfRangeException(
                $"{pointNumber} is greater than the {numberOfPoints} points in segment {segmentNumber} of this toolpath.");
        }

        /// <summary>
        /// Gets the number of leads in the toolpath
        /// </summary>
        public int NumberOfLeads
        {
            get
            {
                return int.Parse(
                    PowerMill.DoCommandEx("print par terse ${toolpath_component_count('toolpath', '" + Name + "', 'leads')}")
                             .ToString());
            }
        }

        /// <summary>
        /// Gets the number of links in the toolpath
        /// </summary>
        public int NumberOfLinks
        {
            get
            {
                return int.Parse(
                    PowerMill.DoCommandEx("print par terse ${toolpath_component_count('toolpath', '" + Name + "', 'links')}")
                             .ToString());
            }
        }

        #endregion

        #region Operations

        /// <summary>
        /// Set the tool's start position.
        /// </summary>
        /// <param name="method">
        ///     <see cref="StartPointMethod"/>
        /// </param>
        /// <param name="x">tool's x position axis </param>
        /// <param name="y">tool's y position axis</param>
        /// <param name="z">tool's z position axis</param>
        public void SetStartPointMethod(StartPointMethod method, double x = 0, double y = 0, double z = 0)
        {
            SetParameter(Resources.StartPointMethod, Resources.ResourceManager.GetString(method.ToString()));
            if (method == StartPointMethod.Absolute)
            {
                SetParameter(Resources.StartPointPositionX, x);
                SetParameter(Resources.StartPointPositionY, y);
                SetParameter(Resources.StartPointPositionZ, z);
            }
        }

        /// <summary>
        /// Set the tool's start position.
        /// </summary>
        /// <param name="method">
        ///     <see cref="StartPointMethod"/>
        /// </param>
        /// <param name="point">tool's point of x,y,z axis </param>
        public void SetStartPointMethod(StartPointMethod method, Point point)
        {
            SetStartPointMethod(method, point.X.Value, point.Y.Value, point.Z.Value);
        }

        /// <summary>
        /// Set the tool's end position.
        /// </summary>
        /// <param name="method">
        ///     <see cref="EndPointMethod"/>
        /// </param>
        /// <param name="x">tool's x position axis </param>
        /// <param name="y">tool's y position axis</param>
        /// <param name="z">tool's z position axis</param>
        public void SetEndPointMethod(EndPointMethod method, double x = 0, double y = 0, double z = 0)
        {
            SetParameter(Resources.EndPointMethod, Resources.ResourceManager.GetString(method.ToString()));
            if (method == EndPointMethod.Absolute)
            {
                SetParameter(Resources.EndPointPositionX, x);
                SetParameter(Resources.EndPointPositionY, y);
                SetParameter(Resources.EndPointPositionZ, z);
            }
        }

        /// <summary>
        /// Set the tool's end position.
        /// </summary>
        /// <param name="method">
        ///     <see cref="EndPointMethod"/>
        /// </param>
        /// <param name="point">tool's point of x,y,z axis </param>
        public void SetEndPointMethod(EndPointMethod method, Point point)
        {
            SetEndPointMethod(method, point.X.Value, point.Y.Value, point.Z.Value);
        }

        /// <summary>
        /// Deletes the toolpath from the active project and from PowerMill.
        /// </summary>
        public override void Delete()
        {
            PowerMill.ActiveProject.Toolpaths.Remove(this);
        }

        /// <summary>
        /// Calculates the toolpath.
        /// </summary>
        public void Calculate()
        {
            // Activate it then calculate it
            IsActive = true;
            PowerMill.DoCommand("EDIT TOOLPATH \"" + Name + "\" CALCULATE");
        }

        /// <summary>
        /// Invalidates the toolpath.
        /// </summary>
        public void MakeInvalid()
        {
            PowerMill.DoCommand("INVALIDATE TOOLPATH \"" + Name + "\"");
        }

        /// <summary>
        /// Reports the tool and holder safety of the Toolpath.
        /// </summary>
        public string SafetyReport()
        {
            return ToolSafetyReport() + Environment.NewLine + HolderSafetyReport();
        }

        /// <summary>
        /// Reports the tool safety for the Toolpath.
        /// </summary>
        public string ToolSafetyReport()
        {
            // Need to detect tool gouges first
            DetectToolGouges();
            return "Tool Cutting Moves: " + SafetyFromEnumeration(_toolCuttingMovesSafety) + Environment.NewLine +
                   "Tool Links: " + SafetyFromEnumeration(_toolLinksSafety) + Environment.NewLine + "Tool Leads: " +
                   SafetyFromEnumeration(_toolLeadsSafety);
        }

        /// <summary>
        /// Reports the holder safety for the Toolpath.
        /// </summary>
        public string HolderSafetyReport()
        {
            // Need to detect Holder collisions first
            DetectHolderCollisions();
            return "Holder Cutting Moves: " + SafetyFromEnumeration(_holderCuttingMovesSafety) + Environment.NewLine +
                   "Holder Links: " + SafetyFromEnumeration(_holderLinksSafety) + Environment.NewLine + "Holder Leads: " +
                   SafetyFromEnumeration(_holderLeadsSafety);
        }

        /// <summary>
        /// Detects if there are any gouges.
        /// If it returns true the three status indicators can then be inspected to determine the cause.
        /// </summary>
        public bool DetectToolGouges()
        {
            // See if gouges have already been detected
            if (GetParameterBooleanValue("Verification.GougeChecked") == false)
            {
                // Not yet detected so determine if there are any
                PowerMill.DoCommand("ACTIVATE TOOLPATH '" + Name + "'",
                                    "EDIT COLLISION TYPE Gouge",
                                    "EDIT COLLISION SCOPE ALL",
                                    "EDIT COLLISION SPLIT_TOOLPATH N",
                                    "EDIT COLLISION STOCKMODEL_CHECK N",
                                    "EDIT COLLISION APPLY");
            }

            // Now check the tool safety parameters
            var cuttingStatus = GetParameter("Safety.Tool.Cutting.Status");
            _toolCuttingMovesSafety = SafetyFromString(cuttingStatus);
            var linksStatus = GetParameter("Safety.Tool.Links.Status");
            _toolLinksSafety = SafetyFromString(linksStatus);
            var leadsStatus = GetParameter("Safety.Tool.Leads.Status");
            _toolLeadsSafety = SafetyFromString(leadsStatus);
            if (_toolCuttingMovesSafety == ToolpathSafety.Collides || _toolLinksSafety == ToolpathSafety.Collides ||
                _toolLeadsSafety == ToolpathSafety.Collides)
            {
                return true;
            }

            // No gouges detected
            return false;
        }

        /// <summary>
        /// Checks if there are any collisions.
        /// If it returns true the three status indicators can then be inspected to determine the cause
        /// </summary>
        public bool DetectHolderCollisions()
        {
            // See if collisions have already been detected
            if (GetParameterBooleanValue("Verification.CollisionChecked") == false)
            {
                // Not yet detected so determine if there are any
                PowerMill.DoCommand("ACTIVATE TOOLPATH '" + Name + "'",
                                    "EDIT COLLISION TYPE Collision",
                                    "EDIT COLLISION SCOPE ALL",
                                    "EDIT COLLISION SPLIT_TOOLPATH N",
                                    "EDIT COLLISION STOCKMODEL_CHECK N",
                                    "EDIT COLLISION DEPTH N",
                                    "EDIT COLLISION USE_TOOL \" \"",
                                    "EDIT COLLISION APPLY");
            }

            // Now check the tool safety parameters
            var cuttingStatus = GetParameter("Safety.Holder.Cutting.Status");
            _holderCuttingMovesSafety = SafetyFromString(cuttingStatus);
            var linksStatus = GetParameter("Safety.Holder.Links.Status");
            _holderLinksSafety = SafetyFromString(linksStatus);
            var leadsStatus = GetParameter("Safety.Holder.Leads.Status");
            _holderLeadsSafety = SafetyFromString(leadsStatus);
            if (_holderCuttingMovesSafety == ToolpathSafety.Collides ||
                _holderLinksSafety == ToolpathSafety.Collides || _holderLeadsSafety == ToolpathSafety.Collides)
            {
                return true;
            }

            // No collisions detected
            return false;
        }

        #endregion

        #region Private Operations

        private void InitializeLeads()
        {
            _connections = new PMConnections(this);
        }

        /// <summary>
        /// Turns a string into a ToolpathSafety enum
        /// </summary>
        private ToolpathSafety SafetyFromString(string strSafety)
        {
            if (strSafety == "safe")
            {
                return ToolpathSafety.Safe;
            }
            if (strSafety == "collides")
            {
                return ToolpathSafety.Collides;
            }
            return ToolpathSafety.NotChecked;
        }

        /// <summary>
        /// Gets a string version of the safety enumeration
        /// </summary>
        private string SafetyFromEnumeration(ToolpathSafety enmSafety)
        {
            switch (enmSafety)
            {
                case ToolpathSafety.Safe:
                    return "Safe";
                case ToolpathSafety.Collides:
                    return "Collides";
                default:
                    return "Not Checked";
            }
        }

        #endregion
    }
}