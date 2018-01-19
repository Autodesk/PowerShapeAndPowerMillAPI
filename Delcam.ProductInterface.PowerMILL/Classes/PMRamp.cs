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
    /// <see cref="PMRamp"/> encapsulate the option that enables you to specify how the tool ramps into the stock.
    /// </summary>
    public class PMRamp
    {
        private readonly LeadTypes _leadType;
        private readonly PMToolpath _toolpath;

        /// <summary>
        /// Get property name prefixed with the instance <see cref="LeadTypes"/> name
        /// </summary>
        /// <param name="property">property name</param>
        /// <returns>Full qualified powermill parameter name/></returns>
        private string GetPropertyFullName(string property)
        {
            return string.Format("{0}.{1}.{2}.{3}",
                                 Resources.Connections,
                                 Resources.ResourceManager.GetString(_leadType.ToString()),
                                 Resources.Ramp,
                                 property);
        }

        /// <summary>
        /// <see cref="PMRamp"/> encapsulate the option that enables you to specify how the tool ramps into the stock.
        /// </summary>
        /// <param name="leadType"><see cref="LeadTypes"/> that <see cref="PMRamp"/> is based on</param>
        /// <param name="toolpath"><see cref="PMToolpath"/> to allow access to powermill macro</param>
        public PMRamp(LeadTypes leadType, PMToolpath toolpath)
        {
            _leadType = leadType;
            _toolpath = toolpath;
        }

        /// <summary>
        /// The angle of descent formed as the tool ramps into the block.
        /// </summary>
        public Degree MaxZigAngle
        {
            get { return new Degree(Convert.ToDouble(_toolpath.GetParameter(GetPropertyFullName(Resources.MaxZigAngle)))); }
            set { _toolpath.SetParameter(GetPropertyFullName(Resources.MaxZigAngle), value.Value); }
        }

        /// <summary>
        /// Select how to control the direction of the <see cref="PMRamp"/>.
        /// </summary>
        public RampFollow Follow
        {
            get
            {
                var output = _toolpath.GetParameter(GetPropertyFullName(Resources.Follow));
                var resourceset = Resources.ResourceManager.GetResourceSet(System.Globalization.CultureInfo.CurrentCulture,
                                                                           false,
                                                                           true);
                return Extensions.EnumExtensions.GetEnumByResourceValue<RampFollow>(output, resourceset);
            }
            set
            {
                _toolpath.SetParameter(GetPropertyFullName(Resources.Follow),
                                       Resources.ResourceManager.GetString(value.ToString()));
            }
        }

        /// <summary>
        /// The diameter of the circle using tool diameter units (TDUs).
        /// Tool diameter units is the distance relative to the tool diameter.
        /// So, a 10 mm tool and a TDU of 2, gives an actual value of 20 mm.
        /// </summary>
        public MM CircleDiameter
        {
            get
            {
                return new MM {Value = Convert.ToDouble(_toolpath.GetParameter(GetPropertyFullName(Resources.CircleDiameter)))};
            }
            set { _toolpath.SetParameter(GetPropertyFullName(Resources.CircleDiameter), value.Value); }
        }

        /// <summary>
        /// When selected, the ramp moves are inserted only into closed <see cref="PMToolpath"/> segments.
        /// </summary>
        public bool ClosedSegments
        {
            get
            {
                return Convert.ToBoolean(Convert.ToInt16(_toolpath.GetParameter(GetPropertyFullName(Resources.ClosedSegments))));
            }
            set { _toolpath.SetParameter(GetPropertyFullName(Resources.ClosedSegments), Convert.ToDouble(value)); }
        }

        /// <summary>
        /// This is measured with respect to the tool axis.
        /// For 3-axis machining, the tool axis is the Z axis. For multi-axis machining, Ramp hight height
        /// the tool axis is specified on the Tool Axis dialog.
        /// </summary>
        public RampHeight RampHeight
        {
            get
            {
                var output = _toolpath.GetParameter(GetPropertyFullName(Resources.RampHeight));
                var resourceset = Resources.ResourceManager.GetResourceSet(System.Globalization.CultureInfo.CurrentCulture,
                                                                           false,
                                                                           true);
                return Extensions.EnumExtensions.GetEnumByResourceValue<RampHeight>(output, resourceset);
            }
            set
            {
                _toolpath.SetParameter(GetPropertyFullName(Resources.RampHeight),
                                       Resources.ResourceManager.GetString(value.ToString()));
            }
        }

        /// <summary>
        /// the height of the start of the ramp above the option selected in the Type field
        /// </summary>
        public MM RampHeightValue
        {
            get
            {
                return new MM {Value = Convert.ToDouble(_toolpath.GetParameter(GetPropertyFullName(Resources.RampHeightValue)))};
            }
            set { _toolpath.SetParameter(GetPropertyFullName(Resources.RampHeightValue), value.Value); }
        }

        /// <summary>
        /// When selected, enter a maximum ramp length.When deselected, the tool ramps down in a single pass
        /// </summary>
        public bool FiniteRamp
        {
            get { return Convert.ToBoolean(Convert.ToInt16(_toolpath.GetParameter(GetPropertyFullName(Resources.FiniteRamp)))); }
            set { _toolpath.SetParameter(GetPropertyFullName(Resources.FiniteRamp), Convert.ToDouble(value)); }
        }

        /// <summary>
        /// Enter the maximum ramp length. This determines the number of zig and zag moves required in the ramp.
        /// This value is represented in TDUs.
        /// Tool diameter units is the distance relative to the tool diameter.
        /// So, a 10 mm tool and a TDU of 2, gives an actual value of 20 mm.
        /// </summary>
        public MM FiniteRampLenght
        {
            get
            {
                return new MM {Value = Convert.ToDouble(_toolpath.GetParameter(GetPropertyFullName(Resources.FiniteRampLength)))};
            }
            set { _toolpath.SetParameter(GetPropertyFullName(Resources.FiniteRampLength), value.Value); }
        }

        /// <summary>
        /// When selected, you can enter a different Zag Angle from the Zig Angle. When deselected, the Zag Angle is the same as the Zig Angle.
        /// The second angle of descent is used if the tool doesn't reach the start of the toolpath segment in a single pass.
        /// </summary>
        public bool ZagAngle
        {
            get { return Convert.ToBoolean(Convert.ToInt16(_toolpath.GetParameter(GetPropertyFullName(Resources.ZagAngle)))); }
            set { _toolpath.SetParameter(GetPropertyFullName(Resources.ZagAngle), Convert.ToDouble(value)); }
        }

        /// <summary>
        /// Enter the angle of descent formed as the tool ramps into the block. This is specified in the same way as the Zig Angle.
        /// </summary>
        public Degree ZagAngleMaximum
        {
            get { return new Degree(Convert.ToDouble(_toolpath.GetParameter(GetPropertyFullName(Resources.ZagAngleMaximum)))); }
            set { _toolpath.SetParameter(GetPropertyFullName(Resources.ZagAngleMaximum), value.ToString()); }
        }

        /// <summary>
        /// Select to extend the ramp moves at the start or end of a toolpath. If you select a <see cref="RampFollow"/> of <see cref="PMToolpath"/>,
        /// this option extends a Ramp Lead In (or Lead Out) to the end (or start) of its associated toolpath segment.
        /// </summary>
        public bool Extend
        {
            get { return Convert.ToBoolean(Convert.ToInt16(_toolpath.GetParameter(GetPropertyFullName(Resources.Extend)))); }
            set { _toolpath.SetParameter(GetPropertyFullName(Resources.Extend), Convert.ToDouble(value)); }
        }
    }
}