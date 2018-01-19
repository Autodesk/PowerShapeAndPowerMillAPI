// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using Autodesk.Geometry;
using Autodesk.ProductInterface.Properties;

namespace Autodesk.ProductInterface.PowerMILL
{
    /// <summary>
    /// Represent a Lead in <see cref="PMToolpath"/>.
    /// </summary>
    public class PMLead
    {
        #region Fields

        /// <summary>
        /// <see cref="PMToolpath"/> that own this <see cref="PMLead"/>
        /// </summary>
        protected readonly PMToolpath Toolpath;

        #endregion

        #region Constructors

        /// <summary>
        /// Represent a Lead in <see cref="PMToolpath"/>.
        /// </summary>
        /// <param name="leadType"></param>
        /// <param name="toolpath"></param>
        internal PMLead(LeadTypes leadType, PMToolpath toolpath)
        {
            LeadType = leadType;
            Toolpath = toolpath;
            Ramp = new PMRamp(leadType, toolpath);
        }

        #endregion

        /// <summary>
        /// <see cref="PMToolpath"/> that own this <see cref="PMLead"/>
        /// </summary>
        /// <param name="property">property name</param>
        /// <returns>Full qualified powermill parameter name/></returns>
        protected string GetPropertyFullName(string property)
        {
            return string.Format("{0}.{1}.{2}",
                                 Resources.Connections,
                                 Resources.ResourceManager.GetString(LeadType.ToString()),
                                 property);
        }

        /// <summary>
        /// The type of move to use after each cutting move
        /// </summary>
        public MoveType MoveType
        {
            get
            {
                var result = Toolpath.GetParameter(GetPropertyFullName(Resources.MoveType));
                var resourceset = Resources.ResourceManager.GetResourceSet(System.Globalization.CultureInfo.CurrentCulture,
                                                                           false,
                                                                           true);
                return Extensions.EnumExtensions.GetEnumByResourceValue<MoveType>(result, resourceset);
            }
            set
            {
                Toolpath.SetParameter(GetPropertyFullName(Resources.MoveType),
                                      Resources.ResourceManager.GetString(value.ToString()));
            }
        }

        /// <summary>
        /// Define <see cref="LeadType"/> of this lead.
        /// </summary>
        public readonly LeadTypes LeadType;

        /// <summary>
        /// The length to extend the Lead move before the cutting move starts.
        /// If this is an arc move,
        /// then the distance is the length of the linear tangential move between the arc and the cutting portion of the toolpath.
        /// </summary>
        public MM Distance
        {
            get { return Toolpath.GetParameterDoubleValue(GetPropertyFullName(Resources.Distance)); }
            set { Toolpath.SetParameter(GetPropertyFullName(Resources.Distance), value); }
        }

        /// <summary>
        /// The angle of the Lead move relative to the toolpath segment.
        /// For arcs, this is the angle spanned by the arc.
        /// For lines, it is the orientation relative to the cutting move.
        /// </summary>
        public Degree Angle
        {
            get { return Toolpath.GetParameterDoubleValue(GetPropertyFullName(Resources.Angle)); }
            set { Toolpath.SetParameter(GetPropertyFullName(Resources.Angle), value); }
        }

        /// <summary>
        /// The radius of the Lead arc move
        /// </summary>
        public MM Radius
        {
            get { return Toolpath.GetParameterDoubleValue(GetPropertyFullName(Resources.Radius)); }
            set { Toolpath.SetParameter(GetPropertyFullName(Resources.Radius), value); }
        }

        /// <summary>
        /// enables you to specify how the tool ramps into the stock.
        /// </summary>
        public readonly PMRamp Ramp;
    }
}