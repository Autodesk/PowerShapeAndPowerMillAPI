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
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using Point2D = System.Windows.Point;
using MediaColor = System.Windows.Media.Color;

namespace Autodesk.WPFControls
{
    [TemplatePart(Name = "PART_Items", Type = typeof(ModelVisual3D))]
    [TemplatePart(Name = "PART_Camera", Type = typeof(PerspectiveCamera))]
    [TemplatePart(Name = "PART_Viewport", Type = typeof(Viewport3D))]
    [TemplatePart(Name = "PART_Slider", Type = typeof(Slider))]
    public class DelcamFlowControl : Selector
    {
        #region "Template Parts"

        public static readonly string PART_Items = "PART_Items";
        public static readonly string PART_Camera = "PART_Camera";
        public static readonly string PART_Viewport = "PART_Viewport";

        public static readonly string PART_Slider = "PART_Slider";

        #endregion

        #region "Constructors"

        static DelcamFlowControl()
        {
            //This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
            //This style is defined in themes\generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DelcamFlowControl),
                                                     new FrameworkPropertyMetadata(typeof(DelcamFlowControl)));
        }

        #endregion

        #region "Event Handlers"

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            UpdateDisplay();

            Slider objSlider = (Slider) GetTemplateChild(PART_Slider);
            if (objSlider != null)
            {
                objSlider.Minimum = 0;
                Binding objMaxBinding = new Binding("Items.Count");
                objMaxBinding.Mode = BindingMode.OneWay;
                objMaxBinding.Source = this;
                objMaxBinding.Converter = new ItemsMaxConverter();
                objSlider.SetBinding(Slider.MaximumProperty, objMaxBinding);
                Binding objValueBinding = new Binding("SelectedIndex");
                objValueBinding.Source = this;
                objSlider.SetBinding(Slider.ValueProperty, objValueBinding);
            }
        }

        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            UpdateDisplay();
        }

        #endregion

        #region "Operations"

        private readonly TimeSpan AnimationDuration = TimeSpan.FromMilliseconds(400);

        public void Animate(int index)
        {
            try
            {
                DoubleAnimation rotateAnimation = new DoubleAnimation(RotationAngle(index), AnimationDuration);
                DoubleAnimation xAnimation = new DoubleAnimation(TranslationX(index), AnimationDuration);
                DoubleAnimation zAnimation = new DoubleAnimation(TranslationZ(index), AnimationDuration);
                var _with1 =
                    (Transform3DGroup)
                    ((Viewport2DVisual3D)
                        ((ModelVisual3D)
                            GetTemplateChild(PART_Items))
                        .Children[index])
                    .Transform;
                ((RotateTransform3D) _with1.Children[0]).Rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty,
                                                                                 rotateAnimation);
                _with1.Children[1].BeginAnimation(TranslateTransform3D.OffsetXProperty, xAnimation);
                _with1.Children[1].BeginAnimation(TranslateTransform3D.OffsetZProperty, zAnimation);
            }
            catch
            {
            }
        }

        private double RotationAngle(int index)
        {
            return Math.Sign(index - SelectedIndex) * -90;
        }

        private double TranslationX(int index)
        {
            return index * 0.2 + Math.Sign(index - SelectedIndex) * 1.5;
        }

        private double TranslationZ(int index)
        {
            return index == SelectedIndex ? 1 : 0;
        }

        private int oldIndex;

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);

            if (GetTemplateChild(PART_Items) == null ||
                ((ModelVisual3D) GetTemplateChild(PART_Items)).Children.Count != Items.Count)
            {
                return;
            }

            if (SelectedIndex > oldIndex)
            {
                for (int i = oldIndex; i <= SelectedIndex; i++)
                {
                    Animate(i);
                }
            }
            else
            {
                for (int i = oldIndex; i >= SelectedIndex; i += -1)
                {
                    Animate(i);
                }
            }

            PerspectiveCamera objCamera = (PerspectiveCamera) GetTemplateChild(PART_Camera);

            objCamera.Position = new Point3D(0.2 * SelectedIndex, objCamera.Position.Y, objCamera.Position.Z);

            oldIndex = SelectedIndex;
        }

        private void Control_MouseDown(object sender, MouseButtonEventArgs e)
        {
            int i = 0;
            foreach (Viewport2DVisual3D objItem in ((ModelVisual3D) GetTemplateChild(PART_Items)).Children)
            {
                if (ReferenceEquals(objItem.Visual, sender))
                {
                    SelectedIndex = i;
                }
                i += 1;
            }
        }

        private void UpdateDisplay()
        {
            if (GetTemplateChild(PART_Items) != null)
            {
                ((ModelVisual3D) GetTemplateChild(PART_Items)).Children.Clear();
                oldIndex = -1;

                int i = 0;

                foreach (object objItem in Items)
                {
                    Viewport2DVisual3D objDisplayItem = new Viewport2DVisual3D();
                    var _with2 = objDisplayItem;
                    MeshGeometry3D objGeometry = new MeshGeometry3D();
                    var _with3 = objGeometry;
                    _with3.TriangleIndices = new Int32Collection(new[]
                    {
                        0,
                        1,
                        2,
                        2,
                        3,
                        0
                    });
                    _with3.TextureCoordinates = new PointCollection(new[]
                    {
                        new Point(0, 1),
                        new Point(1, 1),
                        new Point(1, 0),
                        new Point(0, 0)
                    });
                    _with3.Positions = new Point3DCollection(new[]
                    {
                        new Point3D(-1, -1, 0),
                        new Point3D(+1, -1, 0),
                        new Point3D(+1, +1, 0),
                        new Point3D(-1, +1, 0)
                    });

                    _with2.Geometry = objGeometry;

                    _with2.Material = new DiffuseMaterial();

                    Transform3DGroup objTransform = new Transform3DGroup();
                    objTransform.Children.Add(
                        new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), RotationAngle(i))));
                    objTransform.Children.Add(new TranslateTransform3D(TranslationX(i), 0, TranslationZ(i)));
                    _with2.Transform = objTransform;

                    //Dim objStackPanel As New StackPanel
                    //objStackPanel.Orientation = Orientation.Vertical
                    ContentControl objControl = new ContentControl();
                    objControl.ClipToBounds = true;
                    objControl.ContentTemplate = ItemTemplate;
                    objControl.DataContext = objItem;

                    //objStackPanel.Children.Add(objControl)

                    //Dim objMirror As New ContentControl
                    //objMirror.ClipToBounds = True
                    //objMirror.ContentTemplate = Me.ItemTemplate
                    //objMirror.DataContext = objItem
                    //Dim objTransformGroup As New TransformGroup
                    //objTransformGroup.Children.Add(New ScaleTransform(1, -1))
                    //objTransformGroup.Children.Add(New TranslateTransform(0, objMirror.DesiredSize.Height / 2))
                    //objMirror.RenderTransform = objTransformGroup
                    //objStackPanel.Children.Add(objMirror)

                    _with2.Visual = objControl;
                    objControl.MouseDown += Control_MouseDown;

                    Viewport2DVisual3D.SetIsVisualHostMaterial(objDisplayItem.Material, true);

                    ((ModelVisual3D) GetTemplateChild(PART_Items)).Children.Add(objDisplayItem);
                    i += 1;
                }
            }
        }

        #endregion

        #region "Converters"

        private class ItemsMaxConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                if (value == null)
                {
                    return null;
                }
                if ((int) value == 0)
                {
                    return 0;
                }

                return (int) value - 1;
            }

            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}