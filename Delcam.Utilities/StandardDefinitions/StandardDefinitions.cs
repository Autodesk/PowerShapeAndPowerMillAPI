/// <summary>
/// Definition of length units.  Possible values are MM and Inches
/// </summary>
/// 
namespace Delcam
{
    public enum LengthUnits {

        MM,
        Inches
    }

/// <summary>
/// Definition of view angles.  Possible values are directions and ISO
/// </summary>
public enum ViewAngles {
    ViewFromTop,
    ViewFromBottom,
    ViewFromLeft,
    ViewFromRight,
    ViewFromFront,
    ViewFromBack,
    ISO1,
    ISO2,
    ISO3,
    ISO4
}

/// <summary>
/// Definition of Axes.  Possible values are X, Y, Z
/// </summary>
public enum Axes {
    X,
    Y,
    Z,
}

/// <summary>
/// Definition of Planes.  Possible values are XY, YZ, ZX
/// </summary>
public enum Planes {
    XY,
    YZ,
    ZX,
}
 }