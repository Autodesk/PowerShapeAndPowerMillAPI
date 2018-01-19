// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Autodesk.WPFControls
{
    internal class PiePiece : Shape
    {
        protected override Geometry DefiningGeometry
        {
            get
            {
                StreamGeometry geometry = new StreamGeometry();
                geometry.FillRule = FillRule.EvenOdd;

                using (StreamGeometryContext context = geometry.Open())
                {
                    DrawGeometry(context);
                }

                geometry.Freeze();

                return geometry;
            }
        }

        private void DrawGeometry(StreamGeometryContext context)
        {
            Point startPoint = new Point(CentreX, CentreY);

            Point innerArcStartPoint = ComputeCartesianCoordinate(RotationAngle, InnerRadius);
            innerArcStartPoint.Offset(CentreX, CentreY);

            var innerArcEndPoint = ComputeCartesianCoordinate(RotationAngle + WedgeAngle, InnerRadius);
            innerArcEndPoint.Offset(CentreX, CentreY);

            Point outerArcStartPoint = ComputeCartesianCoordinate(RotationAngle, Radius);
            outerArcStartPoint.Offset(CentreX, CentreY);

            Point outerArcEndPoint = ComputeCartesianCoordinate(RotationAngle + WedgeAngle, Radius);
            outerArcEndPoint.Offset(CentreX, CentreY);

            bool largeArc = WedgeAngle > 180.0;

            Size outerArcSize = new Size(Radius, Radius);
            Size innerArcSize = new Size(InnerRadius, InnerRadius);

            context.BeginFigure(innerArcStartPoint, true, true);
            context.LineTo(outerArcStartPoint, true, true);
            context.ArcTo(outerArcEndPoint, outerArcSize, 0, largeArc, SweepDirection.Clockwise, true, true);
            context.LineTo(innerArcEndPoint, true, true);
            context.ArcTo(innerArcStartPoint, innerArcSize, 0, largeArc, SweepDirection.Counterclockwise, true, true);
        }

        public static readonly DependencyProperty CentreXProperty = DependencyProperty.Register("CentreX",
                                                                                                typeof(double),
                                                                                                typeof(PiePiece),
                                                                                                new FrameworkPropertyMetadata(
                                                                                                    0.0,
                                                                                                    FrameworkPropertyMetadataOptions
                                                                                                        .AffectsRender |
                                                                                                    FrameworkPropertyMetadataOptions
                                                                                                        .AffectsMeasure));

        public double CentreX
        {
            get { return (double) GetValue(CentreXProperty); }
            set { SetValue(CentreXProperty, value); }
        }

        public static readonly DependencyProperty CentreYProperty = DependencyProperty.Register("CentreY",
                                                                                                typeof(double),
                                                                                                typeof(PiePiece),
                                                                                                new FrameworkPropertyMetadata(
                                                                                                    0.0,
                                                                                                    FrameworkPropertyMetadataOptions
                                                                                                        .AffectsRender |
                                                                                                    FrameworkPropertyMetadataOptions
                                                                                                        .AffectsMeasure));

        public double CentreY
        {
            get { return (double) GetValue(CentreYProperty); }
            set { SetValue(CentreYProperty, value); }
        }

        public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register("Radius",
                                                                                               typeof(double),
                                                                                               typeof(PiePiece),
                                                                                               new FrameworkPropertyMetadata(0.0,
                                                                                                                             FrameworkPropertyMetadataOptions
                                                                                                                                 .AffectsRender |
                                                                                                                             FrameworkPropertyMetadataOptions
                                                                                                                                 .AffectsMeasure))
            ;

        public double Radius
        {
            get { return (double) GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }

        public static readonly DependencyProperty InnerRadiusProperty = DependencyProperty.Register("InnerRadius",
                                                                                                    typeof(double),
                                                                                                    typeof(PiePiece),
                                                                                                    new FrameworkPropertyMetadata(
                                                                                                        0.0,
                                                                                                        FrameworkPropertyMetadataOptions
                                                                                                            .AffectsRender |
                                                                                                        FrameworkPropertyMetadataOptions
                                                                                                            .AffectsMeasure));

        public double InnerRadius
        {
            get { return (double) GetValue(InnerRadiusProperty); }
            set { SetValue(InnerRadiusProperty, value); }
        }

        public static readonly DependencyProperty WedgeAngleProperty = DependencyProperty.Register("WedgeAngle",
                                                                                                   typeof(double),
                                                                                                   typeof(PiePiece),
                                                                                                   new FrameworkPropertyMetadata(
                                                                                                       0.0,
                                                                                                       FrameworkPropertyMetadataOptions
                                                                                                           .AffectsRender |
                                                                                                       FrameworkPropertyMetadataOptions
                                                                                                           .AffectsMeasure));

        public double WedgeAngle
        {
            get { return (double) GetValue(WedgeAngleProperty); }
            set { SetValue(WedgeAngleProperty, value); }
        }

        public static readonly DependencyProperty RotationAngleProperty = DependencyProperty.Register("RotationAngle",
                                                                                                      typeof(double),
                                                                                                      typeof(PiePiece),
                                                                                                      new
                                                                                                          FrameworkPropertyMetadata(
                                                                                                              0.0,
                                                                                                              FrameworkPropertyMetadataOptions
                                                                                                                  .AffectsRender |
                                                                                                              FrameworkPropertyMetadataOptions
                                                                                                                  .AffectsMeasure))
            ;

        public double RotationAngle
        {
            get { return (double) GetValue(RotationAngleProperty); }
            set { SetValue(RotationAngleProperty, value); }
        }

        private static Point ComputeCartesianCoordinate(double angle, double radius)
        {
            double angleRad = Math.PI / 180.0 * (angle - 90);

            double x = radius * Math.Cos(angleRad);
            double y = radius * Math.Sin(angleRad);

            return new Point(x, y);
        }
    }
}