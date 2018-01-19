// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using Autodesk.Geometry;
using Autodesk.ProductInterface.Properties;

namespace Autodesk.ProductInterface.PowerMILL
{
    /// <summary>
    /// Home for <see cref="PMLead"/> and options that affect all leads on <see cref="PMToolpath"/>
    /// </summary>
    public class PMConnections
    {
        private readonly PMToolpath _toolpath;

        /// <summary>
        /// Home for <see cref="PMLead"/> and options that affect all leads on <see cref="PMToolpath"/>
        /// </summary>
        /// <param name="toolpath"><see cref="PMEntity"/> to allow access to powermill macro</param>
        internal PMConnections(PMToolpath toolpath)
        {
            _toolpath = toolpath;

            LeadIn = new PMLead(LeadTypes.LeadIn, _toolpath);
            LeadInSecond = new PMLead(LeadTypes.LeadInSecond, _toolpath);
            LeadOut = new PMLead(LeadTypes.LeadOut, _toolpath);
            LeadOutSecond = new PMLead(LeadTypes.LeadOutSecond, _toolpath);
            FirstLeadIn = new PMLead(LeadTypes.FirstLeadIn, _toolpath);
            LastLeadOut = new PMLead(LeadTypes.LastLeadOut, _toolpath);
            LeadInExtension = new PMLeadExtension(LeadTypes.LeadInExtension, _toolpath);
            LeadOutExtension = new PMLeadExtension(LeadTypes.LeadOutExtension, _toolpath);
        }

        /// <summary>
        /// Lead in first choice, Settings to control the tool's movement as it approaches the stock,
        /// before beginning a cutting move.
        /// </summary>
        public PMLead LeadIn { get; }

        /// <summary>
        /// Lead in second choice, Settings to control the tool's movement as it approaches the stock,
        /// before beginning a cutting move.
        /// </summary>
        public PMLead LeadInSecond { get; }

        /// <summary>
        /// Lead out first choice, Settings to control the tool's movement after it leaves the stock,
        /// at the end of a cutting move.
        /// </summary>
        public PMLead LeadOut { get; }

        /// <summary>
        /// Lead out second choice, Settings to control the tool's movement after it leaves the stock,
        /// at the end of a cutting move.
        /// </summary>
        public PMLead LeadOutSecond { get; }

        /// <summary>
        /// Settings to specify a First Lead In move that is different from the subsequent lead in moves.
        /// </summary>
        public PMLead FirstLeadIn { get; }

        /// <summary>
        /// Settings to specify a Last Lead Out move that is different from the previous lead out moves.
        /// </summary>
        public PMLead LastLeadOut { get; }

        /// <summary>
        /// These options add an additional Lead In before the existing one.
        /// </summary>
        public PMLeadExtension LeadInExtension { get; }

        /// <summary>
        /// These options add an additional Lead Out after the existing one.
        /// </summary>
        public PMLeadExtension LeadOutExtension { get; }

        /// <summary>
        /// Enter the distance by which a Lead in or Lead out can overlap a closed toolpath segment.
        /// </summary>
        public MM OverlapDistance
        {
            get { return _toolpath.GetParameterDoubleValue(GetPropertyFullName(Resources.OverlapDistance)); }
            set { _toolpath.SetParameter(GetPropertyFullName(Resources.OverlapDistance), value); }
        }

        /// <summary>
        /// When selected, this option moves the start points on closed toolpath segments to try and find a non-gouging position.
        /// When deselected, PowerMILL will not move the start points - this means that if you carefully position a start point,
        /// you can prevent PowerMILL from moving it.
        /// </summary>
        public bool AllowStartPointToBeMoved
        {
            get { return _toolpath.GetParameterBooleanValue(GetPropertyFullName(Resources.AllowStartPointToBeMoved)); }
            set { _toolpath.SetParameter(GetPropertyFullName(Resources.AllowStartPointToBeMoved), value); }
        }

        /// <summary>
        /// When selected, this option adds leads to short links. By default, leads are applied to all link moves.
        /// However, for reasons of efficiency and/or quality,  there can be occasions where you want leads on long
        /// link moves and on the moves at the start and end of the toolpath, but don't want leads on short link moves.
        /// <exception cref="InvalidPowerMillVersionException">
        /// This feature available on PowerMILL 2016 and lower versions,
        /// otherwise exception will be thrown
        /// </exception>
        /// </summary>
        public bool AddLeadsToShortLinks
        {
            get
            {
                CheckMaximumVersion(PMEntity.POWER_MILL_2016,
                                    _toolpath.Version,
                                    GetPropertyFullName(Resources.AddLeadsToShortLinks));
                return _toolpath.GetParameterBooleanValue(GetPropertyFullName(Resources.AddLeadsToShortLinks));
            }
            set
            {
                CheckMaximumVersion(PMEntity.POWER_MILL_2016,
                                    _toolpath.Version,
                                    GetPropertyFullName(Resources.AddLeadsToShortLinks));
                _toolpath.SetParameter(GetPropertyFullName(Resources.AddLeadsToShortLinks), value);
            }
        }

        /// <summary>
        /// Inserts leads and links between continuous segments where there is an angular change in tool axis between neighboring
        /// segments. This ensures that any reorientation of the tool axis at a specific point takes place away from the part.
        /// </summary>
        public bool AddLeadsAtToolAxisDiscontinuities
        {
            get { return _toolpath.GetParameterBooleanValue(GetPropertyFullName(Resources.AddLeadsAtToolAxisDiscontinuities)); }
            set { _toolpath.SetParameter(GetPropertyFullName(Resources.AddLeadsAtToolAxisDiscontinuities), value); }
        }

        /// <summary>
        /// Enter the minimum angle required before leads and links are inserted. The angle is the change in the tool axis between neighbouring segments.
        /// </summary>
        public Degree AngularThreshold
        {
            get { return _toolpath.GetParameterDoubleValue(GetPropertyFullName(Resources.AngularThreshold)); }
            set { _toolpath.SetParameter(GetPropertyFullName(Resources.AngularThreshold), value); }
        }

        /// <summary>
        /// Select this option for Powermill to gouge-check all leads and links.
        /// </summary>
        public bool GougeCheck
        {
            get { return _toolpath.GetParameterBooleanValue(GetPropertyFullName(Resources.GougeCheck)); }
            set { _toolpath.SetParameter(GetPropertyFullName(Resources.GougeCheck), value); }
        }

        /// <summary>
        /// The distance at which a short link becomes a long link.
        /// </summary>
        /// threshold
        public MM ShortLinkToLongLinkThreshold
        {
            get { return _toolpath.GetParameterDoubleValue(GetPropertyFullName(Resources.ShortLinkThersholdToLong)); }
            set { _toolpath.SetParameter(GetPropertyFullName(Resources.ShortLinkThersholdToLong), value); }
        }

        /// <summary>
        /// The length and orientation of the link moves.
        /// </summary>
        public MoveDirection RetractAndApproachMoves
        {
            get { return GetEnumByResourceValue<MoveDirection>(Resources.MoveDirection); }

            set
            {
                _toolpath.SetParameter(GetPropertyFullName(Resources.MoveDirection),
                                       Resources.ResourceManager.GetString(value.ToString()));
            }
        }

        /// <summary>
        /// Select to lengthen the retract and approach moves made within rapid links.
        /// This enables the moves to reach the surface, over which the rapid or skim moves are made.
        /// This also lengthens the initial move at the start, and the final move at the end.
        /// </summary>
        public bool AutomaticallyExtend
        {
            get { return _toolpath.GetParameterBooleanValue(GetPropertyFullName(Resources.AutomaticallyExtend)); }
            set { _toolpath.SetParameter(GetPropertyFullName(Resources.AutomaticallyExtend), value); }
        }

        /// <summary>
        /// The maximum length of the extension.
        /// </summary>
        public MM MaxMoveExtension
        {
            get { return _toolpath.GetParameterDoubleValue(GetPropertyFullName(Resources.MaxMoveExtension)); }
            set { _toolpath.SetParameter(GetPropertyFullName(Resources.MaxMoveExtension), value); }
        }

        /// <summary>
        /// The length of the Retract move.
        /// </summary>
        public MM RetractDistance
        {
            get { return _toolpath.GetParameterDoubleValue(GetPropertyFullName(Resources.RetractDistance)); }
            set { _toolpath.SetParameter(GetPropertyFullName(Resources.RetractDistance), value); }
        }

        /// <summary>
        /// The length of the Approach move.
        /// </summary>
        public MM ApproachDistance
        {
            get { return _toolpath.GetParameterDoubleValue(GetPropertyFullName(Resources.ApproachDistance)); }
            set { _toolpath.SetParameter(GetPropertyFullName(Resources.ApproachDistance), value); }
        }

        /// <summary>
        /// Use these options to place arcs on the corners of link moves as the tool rapids across the part.
        /// This avoids sudden changes in direction and is useful when high speed machining.
        /// <remarks>
        /// When the option is deselected, rapid moves are linear and form right-angles with the link moves
        /// When the option is selected, rapid moves are curved and form arcs with the link moves.
        /// The arc radius is controlled by the <see cref="ArcFitRadius"/>TDU
        /// </remarks>
        /// </summary>
        public bool ArcFitRapidMove
        {
            get { return _toolpath.GetParameterBooleanValue(GetPropertyFullName(Resources.ArcFitRapidMove)); }
            set { _toolpath.SetParameter(GetPropertyFullName(Resources.ArcFitRapidMove), value); }
        }

        /// <summary>
        /// The TDU radius to control the <see cref="ArcFitRapidMove"/>.
        /// </summary>
        public Degree ArcFitRadius
        {
            get { return _toolpath.GetParameterDoubleValue(GetPropertyFullName(Resources.ArcFitRadius)); }
            set { _toolpath.SetParameter(GetPropertyFullName(Resources.ArcFitRadius), value); }
        }

        /// <summary>
        /// First choice
        /// The type of link moves to connect consecutive cutting passes for links longer than the Short/Long threshold value.
        /// This feature maximum version is PowerMill 2016
        /// <exception cref="InvalidPowerMillVersionException">
        /// This feature available on PowerMILL 2016 and lower versions,
        /// otherwise exception will be thrown
        /// </exception>
        /// </summary>
        public DefaultLinkTypes LongLink
        {
            get
            {
                CheckMaximumVersion(PMEntity.POWER_MILL_2016, _toolpath.Version, Resources.LongLink);
                return GetEnumByResourceValue<DefaultLinkTypes>(Resources.LongLink);
            }
            set
            {
                CheckMaximumVersion(PMEntity.POWER_MILL_2016, _toolpath.Version, Resources.LongLink);
                _toolpath.SetParameter(GetPropertyFullName(Resources.LongLink),
                                       Resources.ResourceManager.GetString(value.ToString()));
            }
        }

        /// <summary>
        /// Second choice
        /// The type of link moves to connect consecutive cutting passes for links longer than the Short/Long threshold value.
        /// This feature maximum version is PowerMill 2016
        /// <exception cref="InvalidPowerMillVersionException">
        /// This feature available on PowerMILL 2016 and lower versions,
        /// otherwise exception will be thrown
        /// </exception>
        /// </summary>
        public DefaultLinkTypes LongLinkSecond
        {
            get
            {
                CheckMaximumVersion(PMEntity.POWER_MILL_2016, _toolpath.Version, Resources.LongLinkSecond);
                return GetEnumByResourceValue<DefaultLinkTypes>(Resources.LongLinkSecond);
            }
            set
            {
                CheckMaximumVersion(PMEntity.POWER_MILL_2016, _toolpath.Version, Resources.LongLinkSecond);
                _toolpath.SetParameter(GetPropertyFullName(Resources.LongLinkSecond),
                                       Resources.ResourceManager.GetString(value.ToString()));
            }
        }

        /// <summary>
        /// First choice
        /// The type of link moves to be used if <see cref="ShortLink"/> or <see cref="LongLink"/> types gouge,
        /// or in version 2017 the type of link moves from the list to be used if the constraint criteria for both
        /// the <see cref="Link1st"/> choice and <see cref="Link2nd"/> choice links are not met, or if they gouge
        /// </summary>
        public DefaultLinkTypes DefaultLink
        {
            get { return GetEnumByResourceValue<DefaultLinkTypes>(Resources.DefaultLink); }
            set
            {
                _toolpath.SetParameter(GetPropertyFullName(Resources.DefaultLink),
                                       Resources.ResourceManager.GetString(value.ToString()));
            }
        }

        /// <summary>
        /// Second choice
        /// The type of link moves to be used if <see cref="ShortLink"/> or <see cref="LongLink"/> types gouge.
        /// </summary>
        public DefaultLinkTypes DefaultLinkSecond
        {
            get { return GetEnumByResourceValue<DefaultLinkTypes>(Resources.DefaultLinkSecond); }
            set
            {
                _toolpath.SetParameter(GetPropertyFullName(Resources.DefaultLinkSecond),
                                       Resources.ResourceManager.GetString(value.ToString()));
            }
        }

        /// <summary>
        /// First choice
        /// The type of link moves to connect consecutive cutting passes for links shorter than the Short/Long threshold value.
        /// This feature maximum version is PowerMill 2016
        /// <exception cref="InvalidPowerMillVersionException">
        /// This feature available on PowerMILL 2016 and lower versions,
        /// otherwise exception will be thrown
        /// </exception>
        /// </summary>
        public LinkTypes ShortLink
        {
            get
            {
                CheckMaximumVersion(PMEntity.POWER_MILL_2016, _toolpath.Version, GetPropertyFullName(Resources.ShortLinkType));
                return GetEnumByResourceValue<LinkTypes>(Resources.ShortLinkType);
            }
            set
            {
                CheckMaximumVersion(PMEntity.POWER_MILL_2016, _toolpath.Version, GetPropertyFullName(Resources.ShortLinkType));
                _toolpath.SetParameter(GetPropertyFullName(Resources.ShortLinkType),
                                       Resources.ResourceManager.GetString(value.ToString()));
            }
        }

        /// <summary>
        /// Second choice
        /// The type of link moves to connect consecutive cutting passes for links shorter than the Short/Long threshold value.
        /// This feature maximum version is PowerMill 2016
        /// <exception cref="InvalidPowerMillVersionException">
        /// This feature available on PowerMILL 2016 and lower versions,
        /// otherwise exception will be thrown
        /// </exception>
        /// </summary>
        public LinkTypes ShortLinkSecond
        {
            get
            {
                CheckMaximumVersion(PMEntity.POWER_MILL_2016, _toolpath.Version, GetPropertyFullName(Resources.ShortLinkSecond));
                return GetEnumByResourceValue<LinkTypes>(Resources.ShortLinkSecond);
            }
            set
            {
                CheckMaximumVersion(PMEntity.POWER_MILL_2016, _toolpath.Version, GetPropertyFullName(Resources.ShortLinkSecond));
                _toolpath.SetParameter(GetPropertyFullName(Resources.ShortLinkSecond),
                                       Resources.ResourceManager.GetString(value.ToString()));
            }
        }

        /// <summary>
        /// First choice
        /// the type of link moves to connect consecutive cutting passes for your first choice.
        /// PowerMill applies this link where its constraint criteria are met.
        /// This feature minimum version is PowerMill 2017
        /// <exception cref="InvalidPowerMillVersionException">
        /// This feature become available on PowerMILL 2017,
        /// therefore older versions will throw exception
        /// </exception>
        /// </summary>
        public LinkTypes Link1st
        {
            //TODO: 2017 Links has new concept called Contstrains which make links as separate entity
            get
            {
                CheckMinimumVersion(PMEntity.POWER_MILL_2017, _toolpath.Version, GetPropertyFullName(Resources.Link1st));
                return GetEnumByResourceValue<LinkTypes>(Resources.Link1st);
            }
            set
            {
                CheckMinimumVersion(PMEntity.POWER_MILL_2017, _toolpath.Version, GetPropertyFullName(Resources.Link1st));
                _toolpath.SetParameter(GetPropertyFullName(Resources.Link1st),
                                       Resources.ResourceManager.GetString(value.ToString()));
            }
        }

        /// <summary>
        /// Second choice
        /// The type of link moves to connect consecutive cutting passes for your second choice.
        /// PowerMill applies this link where its constraint criteria are met.
        /// This feature minimum version is PowerMill 2017
        /// <exception cref="InvalidPowerMillVersionException">
        /// This feature become available on PowerMILL 2017,
        /// therefore older versions will throw exception
        /// </exception>
        /// </summary>
        public LinkTypes Link2nd
        {
            get
            {
                CheckMinimumVersion(PMEntity.POWER_MILL_2017, _toolpath.Version, GetPropertyFullName(Resources.Link2nd));
                return GetEnumByResourceValue<LinkTypes>(Resources.Link2nd);
            }
            set
            {
                CheckMinimumVersion(PMEntity.POWER_MILL_2017, _toolpath.Version, GetPropertyFullName(Resources.Link2nd));
                _toolpath.SetParameter(GetPropertyFullName(Resources.Link2nd),
                                       Resources.ResourceManager.GetString(value.ToString()));
            }
        }

        /// <summary>
        /// Third choice
        /// The type of link moves to connect consecutive cutting passes for your second choice.
        /// PowerMill applies this link where its constraint criteria are met.
        /// This feature minimum version is PowerMill 2017
        /// <exception cref="InvalidPowerMillVersionException">
        /// This feature become available on PowerMILL 2017,
        /// therefore older versions will throw exception
        /// </exception>
        /// </summary>
        public LinkTypes Link3rd
        {
            get
            {
                CheckMinimumVersion(PMEntity.POWER_MILL_2017, _toolpath.Version, GetPropertyFullName(Resources.Link3rd));
                return GetEnumByResourceValue<LinkTypes>(Resources.Link3rd);
            }
            set
            {
                CheckMinimumVersion(PMEntity.POWER_MILL_2017, _toolpath.Version, GetPropertyFullName(Resources.Link3rd));
                _toolpath.SetParameter(GetPropertyFullName(Resources.Link3rd),
                                       Resources.ResourceManager.GetString(value.ToString()));
            }
        }

        /// <summary>
        /// Forth choice
        /// The type of link moves to connect consecutive cutting passes for your second choice.
        /// PowerMill applies this link where its constraint criteria are met.
        /// This feature minimum version is PowerMill 2017
        /// <exception cref="InvalidPowerMillVersionException">
        /// This feature become available on PowerMILL 2017,
        /// therefore older versions will throw exception
        /// </exception>
        /// </summary>
        public LinkTypes Link4th
        {
            get
            {
                CheckMinimumVersion(PMEntity.POWER_MILL_2017, _toolpath.Version, GetPropertyFullName(Resources.Link4th));
                return GetEnumByResourceValue<LinkTypes>(Resources.Link4th);
            }
            set
            {
                CheckMinimumVersion(PMEntity.POWER_MILL_2017, _toolpath.Version, GetPropertyFullName(Resources.Link4th));
                _toolpath.SetParameter(GetPropertyFullName(Resources.Link4th),
                                       Resources.ResourceManager.GetString(value.ToString()));
            }
        }

        /// <summary>
        /// Fifth choice
        /// The type of link moves to connect consecutive cutting passes for your second choice.
        /// PowerMill applies this link where its constraint criteria are met.
        /// This feature minimum version is PowerMill 2017
        /// <exception cref="InvalidPowerMillVersionException">
        /// This feature become available on PowerMILL 2017,
        /// therefore older versions will throw exception
        /// </exception>
        /// </summary>
        public LinkTypes Link5th
        {
            get
            {
                CheckMinimumVersion(PMEntity.POWER_MILL_2017, _toolpath.Version, GetPropertyFullName(Resources.Link5th));
                return GetEnumByResourceValue<LinkTypes>(Resources.Link5th);
            }
            set
            {
                CheckMinimumVersion(PMEntity.POWER_MILL_2017, _toolpath.Version, GetPropertyFullName(Resources.Link5th));
                _toolpath.SetParameter(GetPropertyFullName(Resources.Link5th),
                                       Resources.ResourceManager.GetString(value.ToString()));
            }
        }

        /// <summary>
        /// Get property name prefixed with the instance <see cref="LeadTypes"/> nam
        /// e
        /// </summary>
        /// <param name="property">property name</param>
        /// <returns>Full qualified powermill parameter name/></returns>
        private string GetPropertyFullName(string property)
        {
            return string.Format("{0}.{1}", Resources.Connections, property);
        }

        /// <summary>
        /// encapsulate the code to retrieve values by resources name and enum type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameterName">parameter name</param>
        /// <returns></returns>
        private T GetEnumByResourceValue<T>(string parameterName) where T : struct
        {
            var output = _toolpath.GetParameter(GetPropertyFullName(parameterName));
            var resourceset =
                Resources.ResourceManager.GetResourceSet(System.Globalization.CultureInfo.CurrentCulture, false, true);
            return Extensions.EnumExtensions.GetEnumByResourceValue<T>(output, resourceset);
        }

        /// <summary>
        /// Check that the current version is not less than the required one to run new features
        /// </summary>
        private void CheckMinimumVersion(Version requiredVersion, Version runningVersion, string option)
        {
            if (runningVersion.Major < requiredVersion.Major)
            {
                throw new InvalidPowerMillVersionException(runningVersion, option);
            }
        }

        /// <summary>
        /// Check that the current version is not greater than the required one to run old features
        /// </summary>
        private void CheckMaximumVersion(Version requiredVersion, Version runningVersion, string option)
        {
            if (runningVersion.Major > requiredVersion.Major)
            {
                throw new InvalidPowerMillVersionException(runningVersion, option);
            }
        }
    }
}