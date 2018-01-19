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

namespace Autodesk.ProductInterface.PowerSHAPE
{
    /// <summary>
    /// This class allows the user to select triangles in a mesh using a variety of methods,
    /// and perform 3 separate actions on the selection.
    /// </summary>
    /// <remarks></remarks>
    public class PSMeshTrianglePicker
    {
        private PSMesh _mesh;
        private PSAutomation _powershape;

        private TrianglePickMode _pickMode;

        // 20 degrees is the default value in Powershape
        private Degree _angle = 20;

        private bool _isToolCurrentlyPicking;

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="mesh">Initialises the mesh.</param>
        /// <param name="pickMode">Initialises the pick mode.</param>
        /// <remarks></remarks>
        public PSMeshTrianglePicker(PSMesh mesh, TrianglePickMode pickMode = TrianglePickMode.ContinuousLasso)
        {
            _mesh = mesh;
            _powershape = mesh.PowerSHAPE;
            _pickMode = pickMode;
        }

        #region "Properties"

        /// <summary>
        /// Gets the previous set value and Sets the triangle pick mode in PowerShape.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public TrianglePickMode Mode
        {
            get { return _pickMode; }
            set
            {
                // Close any existing dialogs
                if (_isToolCurrentlyPicking)
                {
                    if ((_pickMode == TrianglePickMode.ToDiscontinuityAngle) | (_pickMode == TrianglePickMode.ToHorizonAngle))
                    {
                        _powershape.DoCommand("AREA_OFF");
                    }
                }

                _pickMode = value;
                if (_isToolCurrentlyPicking)
                {
                    switch (_pickMode)
                    {
                        case TrianglePickMode.Box:
                            _powershape.DoCommand("BOX_SELECT_TRIS");
                            break;
                        case TrianglePickMode.ContinuousLasso:
                            _powershape.DoCommand("LASSO_CONTINUOUS");
                            break;
                        case TrianglePickMode.DiscreteLasso:
                            _powershape.DoCommand("LASSO_DISCRETE");
                            break;
                        case TrianglePickMode.ToDiscontinuityAngle:
                            _powershape.DoCommand("AREA_TO_DISCONTINUITY");
                            _powershape.DoCommand(string.Format("MSHVALUE SCALEVALUE {0}", _angle * (100.0 / 180.0)));
                            break;
                        case TrianglePickMode.ToHorizonAngle:
                            _powershape.DoCommand("AREA_TO_LOCAL_HORIZON");
                            _powershape.DoCommand(string.Format("MSHVALUE SCALEVALUE {0}", _angle * (100.0 / 180.0)));
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the previous set value and Sets the angle for horizon or discontinuity mode in PowerShape.
        /// </summary>
        public Degree AngleForHorizonOrDiscontinuity
        {
            get { return _angle; }
            set
            {
                if ((value < 0.0) | (value > 180.0))
                {
                    throw new Exception("Angle must be between 0 and 180.");
                }

                _angle = value;
                if (_isToolCurrentlyPicking)
                {
                    _powershape.DoCommand(string.Format("MSHVALUE SCALEVALUE {0}", value * (100.0 / 180.0)));
                }
            }
        }

        #endregion

        /// <summary>
        /// Starts the picking operation.
        /// </summary>
        /// <remarks></remarks>
        public void StartPicking()
        {
            _mesh.AddToSelection(true);
            switch (_pickMode)
            {
                case TrianglePickMode.Box:
                    _powershape.DoCommand("BOX_SELECT_TRIS");
                    break;
                case TrianglePickMode.ContinuousLasso:
                    _powershape.DoCommand("LASSO_CONTINUOUS");
                    break;
                case TrianglePickMode.DiscreteLasso:
                    _powershape.DoCommand("LASSO_DISCRETE");
                    break;
                case TrianglePickMode.ToDiscontinuityAngle:
                    _powershape.DoCommand("AREA_TO_DISCONTINUITY");
                    _powershape.DoCommand(string.Format("MSHVALUE SCALEVALUE {0}", _angle * (100.0 / 180.0)));
                    break;
                case TrianglePickMode.ToHorizonAngle:
                    _powershape.DoCommand("AREA_TO_LOCAL_HORIZON");
                    _powershape.DoCommand(string.Format("MSHVALUE SCALEVALUE {0}", _angle * (100.0 / 180.0)));
                    break;
            }

            _isToolCurrentlyPicking = true;
        }

        /// <summary>
        /// Deletes selected triangles.
        /// </summary>
        /// <remarks></remarks>
        public void DeleteSelectedTriangles()
        {
            _powershape.Execute("DeleteTriangles");
        }

        /// <summary>
        /// Masks selected triangles.
        /// </summary>
        /// <remarks></remarks>
        public void MaskSelectedTriangles()
        {
            _powershape.Execute("MaskMesh");
        }

        /// <summary>
        /// Unmask selected triangles.
        /// </summary>
        /// <remarks></remarks>
        public void UnmaskSelectedTriangles()
        {
            _powershape.Execute("UnmaskMesh");
        }

        /// <summary>
        /// Stops picking operation.
        /// </summary>
        /// <remarks></remarks>
        public void StopPicking()
        {
            //_powershape.Execute("ViewEscape")
            switch (_pickMode)
            {
                case TrianglePickMode.Box:
                    _powershape.DoCommand("BOX_SELECT_TRIS");
                    break;
                case TrianglePickMode.ContinuousLasso:
                    _powershape.DoCommand("LASSO_CONTINUOUS");
                    break;
                case TrianglePickMode.DiscreteLasso:
                    _powershape.DoCommand("LASSO_DISCRETE");
                    break;
                case TrianglePickMode.ToDiscontinuityAngle:
                    _powershape.DoCommand("AREA_TO_DISCONTINUITY");
                    break;
                case TrianglePickMode.ToHorizonAngle:
                    _powershape.DoCommand("AREA_TO_LOCAL_HORIZON");
                    break;
            }

            if (_mesh.Exists)
            {
                _mesh.RemoveFromSelection();
            }
            _isToolCurrentlyPicking = false;
        }
    }
}