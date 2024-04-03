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
        /// Gets the strategy value of the toolpath
        /// </summary>
        public string Strategy
        {
            get { return GetParameter(Resources.Strategy); }
        }
      
      public MM Tolerance
        {
            get { return GetParameterDoubleValue(Resources.Tolerance); }
            set { SetParameter("Tolerance", value); }
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
                            PowerMill.GetPowerMillEntityParameter("toolpath",Name,"Statistics.CuttingMoves.Lengths.arcs"));
                    var linearLength =
                        Convert.ToDouble(
                            PowerMill.GetPowerMillEntityParameter("toolpath", Name, "Statistics.CuttingMoves.Lengths.Linear"));
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
            get
            {
                var toolName = string.Empty;

                try
                {
                    toolName = GetParameter(Resources.ToolName);
                }
                catch (InvalidPowerMillParameterException)
                {
                }

                return toolName;
            }
        }

        /// <summary>
        /// Gets the tool associated with this toolpath from PowerMill.
        /// </summary>
        public PMTool Tool
        {
            get
            {
                var name = ToolName;

                if (string.IsNullOrWhiteSpace(name) || PowerMill.ActiveProject.Tools[name] == null)
                {
                    return null;
                }

                return PowerMill.ActiveProject.Tools[name];
            }
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
      /// Gets the Toolpath type value of the toolpath
      /// </summary>
      public string ToolpathType
      {
         get { return GetParameter("toolpath.ToolpathType"); }
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
        /// Gets the Block BoundingBox from PowerMill.
        /// </summary>
        public BoundingBox BlockBoundingBox
        {
            get
            {
                BoundingBox boundingBox = null;

                var tpBlockXmin = GetParameter("toolpath.block.limits.xmin");
                var tpBlockXmax = GetParameter("toolpath.block.limits.xmax");
                var tpBlockYmin = GetParameter("toolpath.block.limits.ymin");
                var tpBlockYmax = GetParameter("toolpath.block.limits.ymax");
                var tpBlockZmin = GetParameter("toolpath.block.limits.zmin");
                var tpBlockZmax = GetParameter("toolpath.block.limits.zmax");

                if (!tpBlockXmin.Contains("#ERROR") && !tpBlockXmax.Contains("#ERROR") &&
                    !tpBlockYmin.Contains("#ERROR") && !tpBlockYmax.Contains("#ERROR") &&
                    !tpBlockZmin.Contains("#ERROR") && !tpBlockZmax.Contains("#ERROR"))
                {
                    boundingBox = new BoundingBox(Convert.ToDouble(tpBlockXmin), Convert.ToDouble(tpBlockXmax),
                                                  Convert.ToDouble(tpBlockYmin), Convert.ToDouble(tpBlockYmax),
                                                  Convert.ToDouble(tpBlockZmin), Convert.ToDouble(tpBlockZmax));
                }
                return boundingBox;
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
                    PowerMill.GetPowerMillParameter("toolpath_component_count('toolpath', '" + Name + "', 'segments')"));
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
                        PowerMill.GetPowerMillParameter("segment_point_count(entity('toolpath', '" + Name + "'), " + segmentNumber + ")"));
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
                        PowerMill.GetPowerMillParameter("segment_get_length(entity('toolpath', '" + Name + "'), " + segmentNumber + ")"));
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
                var x = double.Parse(
                        PowerMill.GetPowerMillParameter("segment_get_point(entity('toolpath', '" + Name + "'), " + segmentNumber + ", " + pointNumber + ").Position.X"));
                var y = double.Parse(
                        PowerMill.GetPowerMillParameter("segment_get_point(entity('toolpath', '" + Name + "'), " + segmentNumber + ", " + pointNumber + ").Position.Y"));
                var z = double.Parse(
                        PowerMill.GetPowerMillParameter("segment_get_point(entity('toolpath', '" + Name + "'), " + segmentNumber + ", " + pointNumber + ").Position.Z"));
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
                var i = double.Parse(
                        PowerMill.GetPowerMillParameter("segment_get_point(entity('toolpath', '" + Name + "'), " + segmentNumber + ", " + pointNumber + ").ToolAxis[0]"));
                var j = double.Parse(
                        PowerMill.GetPowerMillParameter("segment_get_point(entity('toolpath', '" + Name + "'), " + segmentNumber + ", " + pointNumber + ").ToolAxis[1]"));
                var k = double.Parse(
                        PowerMill.GetPowerMillParameter("segment_get_point(entity('toolpath', '" + Name + "'), " + segmentNumber + ", " + pointNumber + ").ToolAxis[2]"));
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
                var i = double.Parse(
                        PowerMill.GetPowerMillParameter("segment_get_point(entity('toolpath', '" + Name + "'), " + segmentNumber + ", " + pointNumber + ").OrientationVector[0]"));
                var j = double.Parse(
                        PowerMill.GetPowerMillParameter("segment_get_point(entity('toolpath', '" + Name + "'), " + segmentNumber + ", " + pointNumber + ").OrientationVector[1]"));
                var k = double.Parse(
                        PowerMill.GetPowerMillParameter("segment_get_point(entity('toolpath', '" + Name + "'), " + segmentNumber + ", " + pointNumber + ").OrientationVector[2]"));
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
                    PowerMill.GetPowerMillParameter("toolpath_component_count('toolpath', '" + Name + "', 'leads')"));
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
                    PowerMill.GetPowerMillParameter("toolpath_component_count('toolpath', '" + Name + "', 'links')"));
            }
        }

        /// <summary>
        /// Speed of the tool during material removal, <see cref="SurfaceSpeed"/> represents the rate at which the cutting edges of the tool can be driven through the material.
        /// <remarks>Any changes made to <see cref="SurfaceSpeed"/> value recalculates <see cref="SpindleSpeed"/>, <see cref="CuttingFeed"/> Rate, and <see cref="PlungingFeed"/> Rate.</remarks>
        /// </summary>
        public double SurfaceSpeed
        {
            get { return double.Parse(GetParameter(Resources.SurfaceSpeed)); }
            set { SetParameter(Resources.SurfaceSpeed, value); }
        }

        /// <summary>
        /// Cutting feed rate.
        /// <remarks>Any changes made to <see cref="CuttingFeed"/> value updates <see cref="FeedPerTooth"/> </remarks>
        /// </summary>
        public double CuttingFeed
        {
            get { return double.Parse(GetParameter(Resources.CuttingFeed)); }
            set { SetParameter(Resources.CuttingFeed, value); }
        }

        /// <summary>
        /// Cutting feed per tooth, <see cref="FeedPerTooth"/> is determined by the construction of the tool, and may be limited by the strength of the cutting edges or the capacity of the tool to remove swarf.
        /// <remarks> Any changes made to <see cref="FeedPerTooth"/> value recalculates  <see cref="CuttingFeed"/> Rate and <see cref="PlungingFeed"/> Rate </remarks>
        /// </summary>
        public MM FeedPerTooth
        {
            get { return double.Parse(GetParameter(Resources.FeedPerTooth)); }
            set { SetParameter(Resources.FeedPerTooth, (double)value); }
        }

        /// <summary>
        /// Controls the speed of the tool as it approaches the stock, before beginning a cutting move.
        /// </summary>
        public double LeadInFactor
        {
            get { return double.Parse(GetParameter(Resources.LeadInFactor)); }
            set { SetParameter(Resources.LeadInFactor, value); }
        }

        /// <summary>
        /// Controls the speed of the tool after it leaves the stock, at the end of a cutting move.
        /// </summary>
        public double LeadOutFactor
        {
            get { return double.Parse(GetParameter(Resources.LeadOutFactor)); }
            set { SetParameter(Resources.LeadOutFactor, value); }
        }

        /// <summary>
        /// Speed of the tool when entering the material ready for its cutting moves. When 3-axis machining, these are vertical moves.
        /// </summary>
        public double PlungingFeed
        {
            get { return double.Parse(GetParameter(Resources.PlungingFeed)); }
            set { SetParameter(Resources.PlungingFeed, value); }
        }

        /// <summary>
        /// Feed rate for straight line moves from point A to point B.
        /// <remarks> If everything is totally safe, the machine moves at rapid which usually appears as G0 in the output file. For skim moves, the machine performs a linear move (which G0 moves do not guarantee), normally G1 in the output file, at a very high feed rate. </remarks>
        /// </summary>
        public double SkimFeed
        {
            get { return double.Parse(GetParameter(Resources.SkimFeed)); }
            set { SetParameter(Resources.SkimFeed, value); }
        }

        /// <summary>
        /// Rotation of the spindle, measured in revolutions per minute.
        /// <remarks> If you edit this value then <see cref="SurfaceSpeed"/> value automatically updates to reflect your change. </remarks>
        /// </summary>
        public double SpindleSpeed
        {
            get { return double.Parse(GetParameter(Resources.SpindleSpeed)); }
            set { SetParameter(Resources.SpindleSpeed, value); }
        }

        /// <summary>
        /// Enter the length of the approach move at the start of the toolpath.
        /// </summary>
        public MM StartPointApproachDistance
        {
            get { return double.Parse(GetParameter(Resources.StartPointApproachDistance)); }
            set { SetParameter(Resources.StartPointApproachDistance, (double)value); }
        }

        /// <summary>
        /// These options determine the orientation of the First approach. This is equivalent to the Links functionality
        /// </summary>
        /// <returns></returns>
        public PointApproach StartPointApproachAlong
        {
            get
            {
                var output = GetParameter(Resources.StartPointApproachAlong);
                var resourceset =
                    Resources.ResourceManager.GetResourceSet(System.Globalization.CultureInfo.CurrentCulture, false, true);
                return Extensions.EnumExtensions.GetEnumByResourceValue<PointApproach>(output, resourceset);
            }
            set { SetParameter(Resources.StartPointApproachAlong, Resources.ResourceManager.GetString(value.ToString())); }
        }

        /// <summary>
        /// Select the location of the start point.
        /// </summary>
        /// <returns></returns>
        public StartPointMethod StartPointMethod
        {
            get
            {
                var output = GetParameter(Resources.StartPointMethod);
                var resourceset =
                    Resources.ResourceManager.GetResourceSet(System.Globalization.CultureInfo.CurrentCulture, false, true);
                return Extensions.EnumExtensions.GetEnumByResourceValue<StartPointMethod>(output, resourceset);
            }
            set { SetParameter(Resources.StartPointMethod, Resources.ResourceManager.GetString(value.ToString())); }
        }

        /// <summary>
        /// X Coordinate of the tool at the start
        /// </summary>
        public double StartPointPositionX
        {
            get { return double.Parse(GetParameter(Resources.StartPointPositionX)); }
        }

        /// <summary>
        /// Y Coordinate of the tool at the start
        /// </summary>
        public double StartPointPositionY
        {
            get { return double.Parse(GetParameter(Resources.StartPointPositionY)); }
        }

        /// <summary>
        /// X Coordinate of the tool at the start
        /// </summary>
        public double StartPointPositionZ
        {
            get { return double.Parse(GetParameter(Resources.StartPointPositionZ)); }
        }

        /// <summary>
        /// Enter the length of the approach move at the  retract move at the end of the toolpath.
        /// </summary>
        public MM EndPointApproachDistance
        {
            get { return double.Parse(GetParameter(Resources.EndPointApproachDistance)); }
            set { SetParameter(Resources.EndPointApproachDistance, (double)value); }
        }

        /// <summary>
        /// These options determine the orientation of the Final retract moves. This is equivalent to the Links functionality
        /// </summary>
        /// <returns></returns>
        public PointApproach EndPointApproachAlong
        {
            get
            {
                var output = GetParameter(Resources.EndPointApproachAlong);
                var resourceset =
                    Resources.ResourceManager.GetResourceSet(System.Globalization.CultureInfo.CurrentCulture, false, true);
                return Extensions.EnumExtensions.GetEnumByResourceValue<PointApproach>(output, resourceset);
            }
            set { SetParameter(Resources.EndPointApproachAlong, Resources.ResourceManager.GetString(value.ToString())); }
        }

        /// <summary>
        /// Select the location of the end point.
        /// </summary>
        /// <returns></returns>
        public EndPointMethod EndPointMethod
        {
            get
            {
                var output = GetParameter(Resources.EndPointMethod);
                var resourceset =
                    Resources.ResourceManager.GetResourceSet(System.Globalization.CultureInfo.CurrentCulture, false, true);
                return Extensions.EnumExtensions.GetEnumByResourceValue<EndPointMethod>(output, resourceset);
            }
            set { SetParameter(Resources.EndPointMethod, Resources.ResourceManager.GetString(value.ToString())); }
        }

        /// <summary>
        /// X Coordinate of the tool at the end
        /// </summary>
        public double EndPointPositionX
        {
            get { return double.Parse(GetParameter(Resources.EndPointPositionX)); }
        }

        /// <summary>
        /// Y Coordinate of the tool at the end
        /// </summary>
        public double EndPointPositionY
        {
            get { return double.Parse(GetParameter(Resources.EndPointPositionY)); }
        }

        /// <summary>
        /// Z Coordinate of the tool at the end
        /// </summary>
        public double EndPointPositionZ
        {
            get { return double.Parse(GetParameter(Resources.EndPointPositionZ)); }
        }

        /// <summary>
        /// The leads and links rapid time
        /// </summary>
        public TimeSpan LeadsAndLinksRapidTime
        {
            get
            {
                var rapid = double.Parse(GetParameter("Statistics.LeadsandLinks.Times.Rapid"));
                return TimeSpan.FromMinutes(rapid);
            }
        }

        /// <summary>
        /// The leads and links plunge time
        /// </summary>
        public TimeSpan LeadsAndLinksPlungeTime
        {
            get
            {
                var plunge = double.Parse(GetParameter("Statistics.LeadsandLinks.Times.Plunge"));
                return TimeSpan.FromMinutes(plunge);
            }
        }

        /// <summary>
        /// The leads and links ramp time
        /// </summary>
        public TimeSpan LeadsAndLinksRampTime
        {
            get
            {
                var ramp = double.Parse(GetParameter("Statistics.LeadsandLinks.Times.Ramp"));
                return TimeSpan.FromMinutes(ramp);
            }
        }

        /// <summary>
        /// The leads and links others time
        /// </summary>
        public TimeSpan LeadsAndLinksOthersTime
        {
            get
            {
                var others = double.Parse(GetParameter("Statistics.LeadsandLinks.Times.Others"));
                return TimeSpan.FromMinutes(others);
            }
        }

        /// <summary>
        /// The linear cutting move time
        /// </summary>
        public TimeSpan LinearCuttingTime
        {
            get
            {
                var linear = double.Parse(GetParameter("Statistics.CuttingMoves.Times.Linear"));
                return TimeSpan.FromMinutes(linear);
            }
        }

        /// <summary>
        /// The arc cutting move time
        /// </summary>
        public TimeSpan ArcCuttingTime
        {
            get
            {
                var arcs = double.Parse(GetParameter("Statistics.CuttingMoves.Times.Arcs"));
                return TimeSpan.FromMinutes(arcs);
            }
        }

        /// <summary>
        /// The total cutting time for the toolpath including leads and links
        /// </summary>
        public TimeSpan TotalCuttingTime
        {
            get
            {
                var rapid = double.Parse(GetParameter("Statistics.LeadsandLinks.Times.Rapid"));
                var plunge = double.Parse(GetParameter("Statistics.LeadsandLinks.Times.Plunge"));
                var ramp = double.Parse(GetParameter("Statistics.LeadsandLinks.Times.Ramp"));
                var others = double.Parse(GetParameter("Statistics.LeadsandLinks.Times.Others"));
                var linear = double.Parse(GetParameter("Statistics.CuttingMoves.Times.Linear"));
                var arcs = double.Parse(GetParameter("Statistics.CuttingMoves.Times.Arcs"));
                var total = rapid + plunge + ramp + others + linear + arcs;
                return TimeSpan.FromMinutes(total);
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