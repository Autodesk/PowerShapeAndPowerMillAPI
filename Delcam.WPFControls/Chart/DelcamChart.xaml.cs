// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.VisualBasic;

namespace Autodesk.WPFControls
{
    public partial class DelcamChart
    {
        #region "Attributes"

        private int intTopMargin;
        private int intLeftMargin;
        private int intRightMargin;
        private int intBottomMargin;

        private string strFormat;

        #endregion

        #region "Events"

        public event ChartItemClickEventHandler ChartItemClick;

        public delegate void ChartItemClickEventHandler(object sender, ChartItemClickEventArgs e);

        #endregion

        #region "Constructors"

        public DelcamChart()
        {
            myDelegate = ConstructChart;

            // This call is required by the Windows Form Designer.
            InitializeComponent();

            // Add any initialization after the InitializeComponent() call.

            ctlChart.DataContext = this;
        }

        #endregion

        #region "Dependency Properties"

        #region "Header"

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header",
                                                                                               typeof(string),
                                                                                               typeof(DelcamChart),
                                                                                               new PropertyMetadata("",
                                                                                                                    HeaderChanged))
            ;

        public string Header
        {
            get { return (string) GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        private static void HeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        #endregion

        #region "ItemLabel"

        public static readonly DependencyProperty ItemLabelProperty = DependencyProperty.Register("ItemLabel",
                                                                                                  typeof(string),
                                                                                                  typeof(DelcamChart),
                                                                                                  new PropertyMetadata(
                                                                                                      ItemLabelChanged));

        public string ItemLabel
        {
            get { return (string) GetValue(ItemLabelProperty); }
            set { SetValue(ItemLabelProperty, value); }
        }

        private static void ItemLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DelcamChart chart = (DelcamChart) d;
            chart.ConstructChart();
        }

        #endregion

        #region "ItemsSource"

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource",
                                                                                                    typeof(IEnumerable),
                                                                                                    typeof(DelcamChart),
                                                                                                    new PropertyMetadata(
                                                                                                        ItemsSourceChanged));

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable) GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        private static void ItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DelcamChart chart = (DelcamChart) d;
            CollectionView myView = (CollectionView) CollectionViewSource.GetDefaultView(chart.ItemsSource);

            if (e.NewValue != null && e.OldValue != null)
            {
                if (!ReferenceEquals(e.OldValue, e.NewValue))
                {
                    INotifyCollectionChanged oldValues = (INotifyCollectionChanged) e.OldValue;
                    oldValues.CollectionChanged -= chart.CollectionChanged;

                    INotifyCollectionChanged newValues = (INotifyCollectionChanged) e.OldValue;
                    newValues.CollectionChanged += chart.CollectionChanged;
                }
                else
                {
                    INotifyCollectionChanged newValues = (INotifyCollectionChanged) e.NewValue;
                    newValues.CollectionChanged += chart.CollectionChanged;
                }
            }
            else
            {
                INotifyCollectionChanged newValues = (INotifyCollectionChanged) e.NewValue;
                newValues.CollectionChanged += chart.CollectionChanged;
            }

            if (myView.Count == 0)
            {
                return;
            }

            chart.ConstructChart();

            foreach (object objItem in myView)
            {
                INotifyPropertyChanged itemObserver = null;
                itemObserver = (INotifyPropertyChanged) objItem;
                itemObserver.PropertyChanged += chart.PlottedValueUpdated;
            }
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ConstructChart();
        }

        #endregion

        #region "PlottedValue"

        public static readonly DependencyProperty PlottedValueProperty = DependencyProperty.Register("PlottedValue",
                                                                                                     typeof(string),
                                                                                                     typeof(DelcamChart),
                                                                                                     new PropertyMetadata("",
                                                                                                                          PlottedValueChanged))
            ;

        public string PlottedValue
        {
            get { return (string) GetValue(PlottedValueProperty); }
            set { SetValue(PlottedValueProperty, value); }
        }

        private static void PlottedValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DelcamChart chart = (DelcamChart) d;
            chart.ConstructChart();
        }

        private void PlottedValueUpdated(object sender, PropertyChangedEventArgs e)
        {
            ConstructChart();
        }

        #endregion

        #region "MaxGood"

        public static readonly DependencyProperty MaxGoodProperty = DependencyProperty.Register("MaxGood",
                                                                                                typeof(double),
                                                                                                typeof(DelcamChart),
                                                                                                new PropertyMetadata(
                                                                                                    double.MaxValue));

        public double MaxGood
        {
            get { return (double) GetValue(MaxGoodProperty); }
            set { SetValue(MaxGoodProperty, value); }
        }

        #endregion

        #region "MinGood"

        public static readonly DependencyProperty MinGoodProperty = DependencyProperty.Register("MinGood",
                                                                                                typeof(double),
                                                                                                typeof(DelcamChart),
                                                                                                new PropertyMetadata(
                                                                                                    double.MinValue));

        public double MinGood
        {
            get { return (double) GetValue(MinGoodProperty); }
            set { SetValue(MinGoodProperty, value); }
        }

        #endregion

        #region "GraphType"

        public enum E_GraphType
        {
            LineGraph,
            BarGraph,
            PieChart,
            ParetoGraph
        }

        public static readonly DependencyProperty GraphTypeProperty = DependencyProperty.Register("GraphType",
                                                                                                  typeof(E_GraphType),
                                                                                                  typeof(DelcamChart),
                                                                                                  new PropertyMetadata(
                                                                                                      E_GraphType.LineGraph,
                                                                                                      GraphTypeChanged));

        public E_GraphType GraphType
        {
            get { return (E_GraphType) GetValue(GraphTypeProperty); }
            set { SetValue(GraphTypeProperty, value); }
        }

        private static void GraphTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DelcamChart chart = (DelcamChart) d;
            chart.ConstructChart();
        }

        #endregion

        #region "ToolTipDataTemplate"

        public static readonly DependencyProperty ToolTipDataTemplateProperty = DependencyProperty.Register(
            "ToolTipDataTemplate",
            typeof(DataTemplate),
            typeof(DelcamChart),
            new PropertyMetadata(null, ToolTipDataTemplateChanged));

        private static void ToolTipDataTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        public DataTemplate ToolTipDataTemplate
        {
            get { return (DataTemplate) GetValue(ToolTipDataTemplateProperty); }
            set { SetValue(ToolTipDataTemplateProperty, value); }
        }

        private void ToolTipDataTemplateChanged()
        {
            //ConstructChart()
        }

        #endregion

        #region "Orientation"

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation",
                                                                                                    typeof(Orientation),
                                                                                                    typeof(DelcamChart),
                                                                                                    new PropertyMetadata(
                                                                                                        Orientation.Vertical,
                                                                                                        OrientationChanged));

        public Orientation Orientation
        {
            get { return (Orientation) GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        private static void OrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DelcamChart chart = (DelcamChart) d;
            chart.ConstructChart();
        }

        #endregion

        #region "ColourSelector"

        public static readonly DependencyProperty ColourSelectorProperty = DependencyProperty.Register("ColourSelector",
                                                                                                       typeof(
                                                                                                           IndexedColourSelector),
                                                                                                       typeof(DelcamChart),
                                                                                                       new PropertyMetadata(null,
                                                                                                                            ColourSelectorChanged))
            ;

        public IndexedColourSelector ColourSelector
        {
            get { return (IndexedColourSelector) GetValue(ColourSelectorProperty); }
            set { SetValue(ColourSelectorProperty, value); }
        }

        private static void ColourSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DelcamChart chart = (DelcamChart) d;
            chart.ConstructChart();
        }

        #endregion

        #endregion

        #region "Chart Drawing"

        #region "General"

        private void ConstructChart()
        {
            if (ReferenceEquals(Dispatcher.Thread, System.Threading.Thread.CurrentThread))
            {
                CollectionView myView = (CollectionView) CollectionViewSource.GetDefaultView(ItemsSource);

                if (string.IsNullOrEmpty(PlottedValue) || myView == null || myView.Count == 0)
                {
                    return;
                }

                double dblMaxValue = double.MinValue;
                double dblMinValue = double.MaxValue;
                double dblCumulativeTotal = 0;

                foreach (object objItem in myView)
                {
                    if (GetPlottedPropertyValue(objItem) > dblMaxValue)
                    {
                        dblMaxValue = GetPlottedPropertyValue(objItem);
                    }
                    if (GetPlottedPropertyValue(objItem) < dblMinValue)
                    {
                        dblMinValue = GetPlottedPropertyValue(objItem);
                    }

                    dblCumulativeTotal += GetPlottedPropertyValue(objItem);
                }
                if (dblMinValue > 0)
                {
                    dblMinValue = 0;
                }
                if (dblMaxValue < 0)
                {
                    dblMaxValue = 0;
                }

                if (dblMinValue == dblMaxValue)
                {
                    dblMaxValue += 1;
                }

                switch (GraphType)
                {
                    case E_GraphType.LineGraph:
                        if (Orientation == Orientation.Horizontal)
                        {
                            ctlTopAxis.Text = "";
                            ctlLeftAxis.Text = ItemLabel;
                            ctlRightAxis.Text = "";
                            ctlBottomAxis.Text = PlottedValue;
                        }
                        else
                        {
                            ctlTopAxis.Text = "";
                            ctlLeftAxis.Text = PlottedValue;
                            ctlRightAxis.Text = "";
                            ctlBottomAxis.Text = ItemLabel;
                        }
                        SetSize(dblMinValue, dblMaxValue);
                        DrawGoodRegion(dblMinValue, dblMaxValue);
                        DrawXAxis(dblMinValue, dblMaxValue);
                        DrawYAxis(dblMinValue, dblMaxValue);
                        DrawLineData(dblMinValue, dblMaxValue);
                        ExpandForXLabels(dblMinValue, dblMaxValue);
                        break;
                    case E_GraphType.BarGraph:
                        if (Orientation == Orientation.Horizontal)
                        {
                            ctlTopAxis.Text = "";
                            ctlLeftAxis.Text = ItemLabel;
                            ctlRightAxis.Text = "";
                            ctlBottomAxis.Text = PlottedValue;
                        }
                        else
                        {
                            ctlTopAxis.Text = "";
                            ctlLeftAxis.Text = PlottedValue;
                            ctlRightAxis.Text = "";
                            ctlBottomAxis.Text = ItemLabel;
                        }
                        SetSize(dblMinValue, dblMaxValue);
                        DrawGoodRegion(dblMinValue, dblMaxValue);
                        DrawBarData(dblMinValue, dblMaxValue);
                        DrawXAxis(dblMinValue, dblMaxValue);
                        DrawYAxis(dblMinValue, dblMaxValue);
                        ExpandForXLabels(dblMinValue, dblMaxValue);
                        break;
                    case E_GraphType.PieChart:
                        ctlTopAxis.Text = "";
                        ctlLeftAxis.Text = "";
                        ctlRightAxis.Text = "";
                        ctlBottomAxis.Text = "";
                        SetPieChartSize();
                        DrawPieChartData(dblCumulativeTotal);
                        break;
                    case E_GraphType.ParetoGraph:
                        if (Orientation == Orientation.Horizontal)
                        {
                            ctlTopAxis.Text = "Percentage";
                            ctlLeftAxis.Text = ItemLabel;
                            ctlRightAxis.Text = "";
                            ctlBottomAxis.Text = PlottedValue;
                        }
                        else
                        {
                            ctlTopAxis.Text = "";
                            ctlLeftAxis.Text = PlottedValue;
                            ctlRightAxis.Text = "Percentage";
                            ctlBottomAxis.Text = ItemLabel;
                        }
                        SetParetoChartSize(dblMinValue, dblMaxValue);
                        if (myView.CanSort)
                        {
                            myView.SortDescriptions.Add(new SortDescription(PlottedValue, ListSortDirection.Descending));
                        }
                        DrawGoodRegion(dblMinValue, dblMaxValue);
                        DrawBarData(dblMinValue, dblMaxValue);
                        DrawXAxis(dblMinValue, dblMaxValue);
                        DrawYAxis(dblMinValue, dblMaxValue);
                        DrawParetoPercentageAxis();
                        DrawParetoLineData(dblMinValue, dblMaxValue, dblCumulativeTotal);
                        ExpandForXLabels(dblMinValue, dblMaxValue);
                        break;
                }
            }
            else
            {
                Dispatcher.Invoke(myDelegate);
            }
        }

        private delegate void ConstructChartDelegate();

        private ConstructChartDelegate myDelegate;

        private void SetSize(double dblMinValue, double dblMaxValue)
        {
            CollectionView myView = (CollectionView) CollectionViewSource.GetDefaultView(ItemsSource);

            if (Orientation == Orientation.Vertical)
            {
                double dblMax = dblMaxValue;
                if (Math.Abs(dblMinValue) > dblMaxValue)
                {
                    dblMax = dblMinValue;
                }

                ctlChartArea.Height = 300;

                ctlChartArea.Children.Clear();

                double dblStep = (dblMaxValue - dblMinValue) / (ctlChartArea.Height / 30);
                if (dblStep > 1)
                {
                    strFormat = "0";
                }
                else
                {
                    strFormat = "0.";
                    for (var i = dblStep.ToString().IndexOf(".") + 1; i <= dblStep.ToString().Length; i++)
                    {
                        strFormat += 0;
                        if ((dblStep.ToString().ToCharArray()[i] != '0') & (dblStep.ToString().ToCharArray()[i] != '.'))
                        {
                            break; // TODO: might not be correct. Was : Exit For
                        }
                    }
                }

                if (dblMinValue < 0)
                {
                    //Leave room for the minus sign
                    intLeftMargin = StringLength(Strings.Format(dblMax, strFormat)) + 10 + 7;
                }
                else
                {
                    intLeftMargin = StringLength(Strings.Format(dblMax, strFormat)) + 7;
                }

                ctlChartArea.Width = 20 * myView.Count + intLeftMargin + 10;
            }
            else
            {
                double dblMax = dblMaxValue;
                if (Math.Abs(dblMinValue) > dblMaxValue)
                {
                    dblMax = dblMinValue;
                }

                ctlChartArea.Width = 300;

                ctlChartArea.Children.Clear();

                double dblStep = (dblMaxValue - dblMinValue) / (ctlChartArea.Width / 30);
                if (dblStep > 1)
                {
                    strFormat = "0";
                }
                else
                {
                    strFormat = "0.";
                    for (var i = dblStep.ToString().IndexOf(".") + 1; i <= dblStep.ToString().Length; i++)
                    {
                        strFormat += 0;
                        if ((dblStep.ToString().ToCharArray()[i] != '0') & (dblStep.ToString().ToCharArray()[i] != '.'))
                        {
                            break; // TODO: might not be correct. Was : Exit For
                        }
                    }
                }

                if (dblMinValue < 0)
                {
                    //Leave room for the minus sign
                    intBottomMargin = StringLength(Strings.Format(dblMax, strFormat)) + 10;
                }
                else
                {
                    intBottomMargin = StringLength(Strings.Format(dblMax, strFormat));
                }

                ctlChartArea.Height = 20 * myView.Count + intBottomMargin + 10;

                double dblMaxLabelLength = 0;
                foreach (object objItem in myView)
                {
                    double dblLength = StringLength(GetItemLabel(objItem));
                    if (dblLength > dblMaxLabelLength)
                    {
                        dblMaxLabelLength = dblLength;
                    }
                }
                if (dblMaxLabelLength > Math.Abs(ctlChartArea.Width * (dblMinValue / dblMaxValue - dblMinValue)))
                {
                    intLeftMargin = (int) (dblMaxLabelLength -
                                           Math.Abs(ctlChartArea.Width * (dblMinValue / dblMaxValue - dblMinValue)) + 7);
                    ctlChartArea.Width += intLeftMargin;
                }
            }
        }

        private int StringLength(string strText)
        {
            int intWidth = 0;

            TextBlock myTextBlock = new TextBlock();
            var _with1 = myTextBlock;
            _with1.Text = strText;
            _with1.HorizontalAlignment = HorizontalAlignment.Left;
            LayoutRoot.Children.Add(myTextBlock);
            UpdateLayout();
            intWidth = (int) _with1.ActualWidth;
            LayoutRoot.Children.Remove(myTextBlock);

            if (intWidth == 0)
            {
                intWidth = 1;
            }

            return intWidth;
        }

        private void ExpandForXLabels(double dblMinValue, double dblMaxValue)
        {
            CollectionView myView = (CollectionView) CollectionViewSource.GetDefaultView(ItemsSource);

            if (Orientation == Orientation.Vertical)
            {
                //Allow for extra room for X Axis labels too
                double dblMaxLabelLength = 0;
                foreach (object objItem in myView)
                {
                    double dblLength = StringLength(GetItemLabel(objItem));
                    if (dblLength > dblMaxLabelLength)
                    {
                        dblMaxLabelLength = dblLength;
                    }
                }
                if (dblMaxLabelLength > Math.Abs(ctlChartArea.Height * (dblMinValue / dblMaxValue - dblMinValue)))
                {
                    ctlChartArea.Height += dblMaxLabelLength -
                                           Math.Abs(ctlChartArea.Height * (dblMinValue / dblMaxValue - dblMinValue)) + 7;
                }
            }
        }

        private void DrawGoodRegion(double dblMinValue, double dblMaxValue)
        {
            //Only draw if the values have been defined
            if ((MaxGood == double.MaxValue) | (MinGood == double.MinValue))
            {
                return;
            }

            Rectangle objRect = new Rectangle();
            double dblMaxGood = MaxGood;
            double dblMinGood = MinGood;

            if (dblMaxGood > dblMaxValue)
            {
                dblMaxGood = dblMaxValue;
            }
            if (dblMinGood < dblMinValue)
            {
                dblMinGood = dblMinValue;
            }

            if (Orientation == Orientation.Vertical)
            {
                var _with2 = objRect;
                _with2.SetValue(Canvas.LeftProperty, Convert.ToDouble(intLeftMargin));
                _with2.SetValue(Canvas.TopProperty,
                                ctlChartArea.Height * ((dblMaxValue - dblMaxGood) / (dblMaxValue - dblMinValue)));
                _with2.Width = ctlChartArea.Width - intLeftMargin;
                _with2.Height = ctlChartArea.Height * ((dblMaxGood - dblMinGood) / (dblMaxValue - dblMinValue));
                LinearGradientBrush brush = new LinearGradientBrush();
                var _with3 = brush;
                _with3.StartPoint = new Point(0, 0);
                _with3.EndPoint = new Point(0, 1);
                _with3.GradientStops.Add(new GradientStop(Colors.Transparent, 0));
                _with3.GradientStops.Add(new GradientStop(Colors.LightGreen, 0.1));
                _with3.GradientStops.Add(new GradientStop(Colors.LightGreen, 0.9));
                _with3.GradientStops.Add(new GradientStop(Colors.Transparent, 1));
                _with2.Fill = brush;
            }
            else
            {
                var _with4 = objRect;
                _with4.SetValue(Canvas.TopProperty, Convert.ToDouble(0));
                _with4.SetValue(Canvas.LeftProperty,
                                ctlChartArea.Width -
                                (ctlChartArea.Width - intLeftMargin) *
                                ((dblMaxValue - dblMinGood) / (dblMaxValue - dblMinValue)));
                _with4.Height = ctlChartArea.Height - intBottomMargin;
                _with4.Width = (ctlChartArea.Width - intLeftMargin) * ((dblMaxGood - dblMinGood) / (dblMaxValue - dblMinValue));
                LinearGradientBrush brush = new LinearGradientBrush();
                var _with5 = brush;
                _with5.StartPoint = new Point(0, 0);
                _with5.EndPoint = new Point(1, 0);
                _with5.GradientStops.Add(new GradientStop(Colors.Transparent, 0));
                _with5.GradientStops.Add(new GradientStop(Colors.LightGreen, 0.1));
                _with5.GradientStops.Add(new GradientStop(Colors.LightGreen, 0.9));
                _with5.GradientStops.Add(new GradientStop(Colors.Transparent, 1));
                _with4.Fill = brush;
            }

            ctlChartArea.Children.Add(objRect);
        }

        private void DrawXAxis(double dblMinValue, double dblMaxValue)
        {
            CollectionView myView = (CollectionView) CollectionViewSource.GetDefaultView(ItemsSource);

            Line xAxis = new Line();
            var _with6 = xAxis;
            if (Orientation == Orientation.Vertical)
            {
                _with6.X1 = intLeftMargin;
                _with6.X2 = ctlChartArea.Width - intRightMargin;
                _with6.Y1 = ctlChartArea.Height * (dblMaxValue / (dblMaxValue - dblMinValue));
                _with6.Y2 = _with6.Y1;
            }
            else
            {
                _with6.Y1 = intTopMargin;
                _with6.Y2 = ctlChartArea.Height - intBottomMargin;
                _with6.X1 = ctlChartArea.Width -
                            (ctlChartArea.Width - intLeftMargin) * (dblMaxValue / (dblMaxValue - dblMinValue));
                _with6.X2 = _with6.X1;
            }
            _with6.Stroke = Brushes.Gray;
            ctlChartArea.Children.Add(xAxis);

            //Now add labels
            int i = 0;
            foreach (object objItem in myView)
            {
                Line marker = new Line();
                var _with7 = marker;
                if (Orientation == Orientation.Vertical)
                {
                    _with7.X1 = intLeftMargin + 10 + i * 20;
                    _with7.X2 = _with7.X1;
                    _with7.Y1 = ctlChartArea.Height * (dblMaxValue / (dblMaxValue - dblMinValue));
                    _with7.Y2 = _with7.Y1 + 5;
                }
                else
                {
                    _with7.Y1 = ctlChartArea.Height - intBottomMargin - 10 - i * 20;
                    _with7.Y2 = _with7.Y1;
                    _with7.X1 = ctlChartArea.Width -
                                (ctlChartArea.Width - intLeftMargin) * (dblMaxValue / (dblMaxValue - dblMinValue));
                    _with7.X2 = _with7.X1 - 5;
                }
                _with7.Stroke = Brushes.Gray;
                ctlChartArea.Children.Add(marker);
                TextBlock label = new TextBlock();
                var _with8 = label;
                _with8.Text = GetItemLabel(objItem);
                if (Orientation == Orientation.Vertical)
                {
                    _with8.SetValue(Canvas.LeftProperty, intLeftMargin + Convert.ToDouble(i * 20));
                    _with8.SetValue(Canvas.TopProperty, ctlChartArea.Height * (dblMaxValue / (dblMaxValue - dblMinValue)) + 6);
                    _with8.LayoutTransform = new RotateTransform(270);
                }
                else
                {
                    _with8.SetValue(Canvas.TopProperty, ctlChartArea.Height - intBottomMargin - Convert.ToDouble(i * 20) - 20);
                    _with8.SetValue(Canvas.LeftProperty, Convert.ToDouble(0));
                    _with8.Width = ctlChartArea.Width -
                                   (ctlChartArea.Width - intLeftMargin) * (dblMaxValue / (dblMaxValue - dblMinValue)) - 7;
                    _with8.TextAlignment = TextAlignment.Right;
                }
                _with8.Foreground = Brushes.Gray;
                ctlChartArea.Children.Add(label);
                i += 1;
            }
        }

        private void DrawYAxis(double dblMinValue, double dblMaxValue)
        {
            Line yAxis = new Line();
            var _with9 = yAxis;
            if (Orientation == Orientation.Vertical)
            {
                _with9.X1 = intLeftMargin;
                _with9.X2 = _with9.X1;
                _with9.Y1 = 0;
                _with9.Y2 = ctlChartArea.Height;
            }
            else
            {
                _with9.Y1 = ctlChartArea.Height - intBottomMargin;
                _with9.Y2 = _with9.Y1;
                _with9.X1 = intLeftMargin;
                _with9.X2 = ctlChartArea.Width;
            }
            _with9.Stroke = Brushes.Gray;
            ctlChartArea.Children.Add(yAxis);

            //Mark zero
            Line marker = new Line();
            var _with10 = marker;
            if (Orientation == Orientation.Vertical)
            {
                _with10.X1 = intLeftMargin - 5;
                _with10.X2 = intLeftMargin;
                _with10.Y1 = ctlChartArea.Height * (dblMaxValue / (dblMaxValue - dblMinValue));
                _with10.Y2 = _with10.Y1;
            }
            else
            {
                _with10.Y1 = ctlChartArea.Height - intBottomMargin;
                _with10.Y2 = _with10.Y1 + 5;
                _with10.X1 = ctlChartArea.Width -
                             (ctlChartArea.Width - intLeftMargin) * (dblMaxValue / (dblMaxValue - dblMinValue));
                _with10.X2 = _with10.X1;
            }
            _with10.Stroke = Brushes.Gray;
            TextBlock label = new TextBlock();
            var _with11 = label;
            _with11.Text = "0";
            if (Orientation == Orientation.Vertical)
            {
                _with11.Width = intLeftMargin - 6;
                _with11.TextAlignment = TextAlignment.Right;
                _with11.SetValue(Canvas.LeftProperty, Convert.ToDouble(0));
                _with11.SetValue(Canvas.TopProperty, ctlChartArea.Height * (dblMaxValue / (dblMaxValue - dblMinValue)) - 10);
            }
            else
            {
                _with11.TextAlignment = TextAlignment.Left;
                _with11.SetValue(Canvas.TopProperty, Convert.ToDouble(ctlChartArea.Height - intBottomMargin + 7));
                _with11.SetValue(Canvas.LeftProperty,
                                 ctlChartArea.Width -
                                 (ctlChartArea.Width - intLeftMargin) * (dblMaxValue / (dblMaxValue - dblMinValue)) - 10);
                _with11.LayoutTransform = new RotateTransform(270);
            }
            _with11.Foreground = Brushes.Gray;
            ctlChartArea.Children.Add(label);
            ctlChartArea.Children.Add(marker);

            if (Orientation == Orientation.Vertical)
            {
                //Mark above zero
                for (double i = 30 / ctlChartArea.Height * (dblMaxValue - dblMinValue);
                    i <= dblMaxValue;
                    i += 30 / ctlChartArea.Height * (dblMaxValue - dblMinValue))
                {
                    marker = new Line();
                    var _with12 = marker;
                    _with12.X1 = intLeftMargin - 5;
                    _with12.X2 = intLeftMargin;
                    _with12.Y1 = ctlChartArea.Height * ((dblMaxValue - i) / (dblMaxValue - dblMinValue));
                    _with12.Y2 = _with12.Y1;
                    _with12.Stroke = Brushes.Gray;
                    ctlChartArea.Children.Add(marker);
                    label = new TextBlock();
                    var _with13 = label;
                    _with13.Text = Strings.Format(i, strFormat);
                    _with13.Width = intLeftMargin - 6;
                    _with13.TextAlignment = TextAlignment.Right;
                    _with13.SetValue(Canvas.LeftProperty, Convert.ToDouble(0));
                    _with13.SetValue(Canvas.TopProperty,
                                     ctlChartArea.Height * ((dblMaxValue - i) / (dblMaxValue - dblMinValue)) - 10);
                    _with13.Foreground = Brushes.Gray;
                    ctlChartArea.Children.Add(label);
                }

                //Mark below zero
                for (double i = -(30 / ctlChartArea.Height * (dblMaxValue - dblMinValue));
                    i >= dblMinValue;
                    i += -(30 / ctlChartArea.Height * (dblMaxValue - dblMinValue)))
                {
                    marker = new Line();
                    var _with14 = marker;
                    _with14.X1 = intLeftMargin - 5;
                    _with14.X2 = intLeftMargin;
                    _with14.Y1 = ctlChartArea.Height * ((dblMaxValue - i) / (dblMaxValue - dblMinValue));
                    _with14.Y2 = _with14.Y1;
                    _with14.Stroke = Brushes.Gray;
                    ctlChartArea.Children.Add(marker);
                    label = new TextBlock();
                    var _with15 = label;
                    _with15.Text = Strings.Format(i, strFormat);
                    _with15.Width = intLeftMargin - 6;
                    _with15.TextAlignment = TextAlignment.Right;
                    _with15.SetValue(Canvas.LeftProperty, Convert.ToDouble(0));
                    _with15.SetValue(Canvas.TopProperty,
                                     ctlChartArea.Height * ((dblMaxValue - i) / (dblMaxValue - dblMinValue)) - 10);
                    _with15.Foreground = Brushes.Gray;
                    ctlChartArea.Children.Add(label);
                }
            }
            else
            {
                //Mark above zero
                for (double i = 30 / ctlChartArea.Width * (dblMaxValue - dblMinValue);
                    i <= dblMaxValue;
                    i += 30 / ctlChartArea.Width * (dblMaxValue - dblMinValue))
                {
                    marker = new Line();
                    var _with16 = marker;
                    _with16.Y1 = ctlChartArea.Height - intBottomMargin;
                    _with16.Y2 = _with16.Y1 + 5;
                    _with16.X1 = ctlChartArea.Width -
                                 (ctlChartArea.Width - intLeftMargin) * ((dblMaxValue - i) / (dblMaxValue - dblMinValue));
                    _with16.X2 = _with16.X1;
                    _with16.Stroke = Brushes.Gray;
                    ctlChartArea.Children.Add(marker);
                    label = new TextBlock();
                    var _with17 = label;
                    _with17.Text = Strings.Format(i, strFormat);
                    _with17.TextAlignment = TextAlignment.Left;
                    _with17.SetValue(Canvas.TopProperty, Convert.ToDouble(ctlChartArea.Height - intBottomMargin + 7));
                    _with17.SetValue(Canvas.LeftProperty,
                                     ctlChartArea.Width -
                                     (ctlChartArea.Width - intLeftMargin) * ((dblMaxValue - i) / (dblMaxValue - dblMinValue)) -
                                     10);
                    _with17.LayoutTransform = new RotateTransform(270);
                    _with17.Foreground = Brushes.Gray;
                    ctlChartArea.Children.Add(label);
                }

                //Mark below zero
                for (double i = -(30 / ctlChartArea.Width * (dblMaxValue - dblMinValue));
                    i >= dblMinValue;
                    i += -(30 / ctlChartArea.Width * (dblMaxValue - dblMinValue)))
                {
                    marker = new Line();
                    var _with18 = marker;
                    _with18.Y1 = ctlChartArea.Height - intBottomMargin;
                    _with18.Y2 = _with18.Y1 + 5;
                    _with18.X1 = ctlChartArea.Width -
                                 (ctlChartArea.Width - intLeftMargin) * ((dblMaxValue - i) / (dblMaxValue - dblMinValue));
                    _with18.X2 = _with18.X1;
                    _with18.Stroke = Brushes.Gray;
                    ctlChartArea.Children.Add(marker);
                    label = new TextBlock();
                    var _with19 = label;
                    _with19.Text = Strings.Format(i, strFormat);
                    _with19.TextAlignment = TextAlignment.Left;
                    _with19.SetValue(Canvas.TopProperty, Convert.ToDouble(ctlChartArea.Height - intBottomMargin + 7));
                    _with19.SetValue(Canvas.LeftProperty,
                                     ctlChartArea.Width -
                                     (ctlChartArea.Width - intLeftMargin) * ((dblMaxValue - i) / (dblMaxValue - dblMinValue)) -
                                     10);
                    _with19.LayoutTransform = new RotateTransform(270);
                    _with19.Foreground = Brushes.Gray;
                    ctlChartArea.Children.Add(label);
                }
            }
        }

        private double GetPlottedPropertyValue(object objItem)
        {
            PropertyDescriptorCollection propDesc = TypeDescriptor.GetProperties(objItem);
            return Convert.ToDouble(propDesc[PlottedValue].GetValue(objItem));
        }

        private string GetItemLabel(object objItem)
        {
            PropertyDescriptorCollection propDesc = TypeDescriptor.GetProperties(objItem);
            return Convert.ToString(propDesc[ItemLabel].GetValue(objItem));
        }

        private void DisplayToolTip(object sender, ToolTipEventArgs e)
        {
            CollectionView myView = (CollectionView) CollectionViewSource.GetDefaultView(ItemsSource);

            if (ToolTipDataTemplate == null)
            {
                e.Handled = true;
                return;
            }

            ToolTip tip = ((dynamic) sender).ToolTip;
            tip.DataContext = myView.GetItemAt(((dynamic) sender).Tag);
        }

        private void ChartItemClicked(object sender, EventArgs e)
        {
            CollectionView myView = (CollectionView) CollectionViewSource.GetDefaultView(ItemsSource);

            if (ChartItemClick != null)
            {
                ChartItemClick(this, new ChartItemClickEventArgs(myView.GetItemAt(((dynamic) sender).Tag)));
            }
        }

        #endregion

        #region "LineChart"

        private void DrawLineData(double dblMinValue, double dblMaxValue)
        {
            CollectionView myView = (CollectionView) CollectionViewSource.GetDefaultView(ItemsSource);

            Polyline objPolyline = new Polyline();
            ctlChartArea.Children.Add(objPolyline);

            var _with20 = objPolyline;
            _with20.Stroke = Brushes.Black;

            int i = 0;
            foreach (object objItem in myView)
            {
                Ellipse objDot = new Ellipse();
                var _with21 = objDot;
                if (Orientation == Orientation.Vertical)
                {
                    _with21.SetValue(Canvas.LeftProperty, intLeftMargin + 10 + Convert.ToDouble(i * 20 - 3));
                    _with21.SetValue(Canvas.TopProperty,
                                     ctlChartArea.Height *
                                     ((dblMaxValue - GetPlottedPropertyValue(objItem)) / (dblMaxValue - dblMinValue)) - 3);
                }
                else
                {
                    _with21.SetValue(Canvas.TopProperty, ctlChartArea.Height - intBottomMargin - 10 - i * 20 - 3);
                    _with21.SetValue(Canvas.LeftProperty,
                                     ctlChartArea.Width -
                                     (ctlChartArea.Width - intLeftMargin) *
                                     ((dblMaxValue - GetPlottedPropertyValue(objItem)) / (dblMaxValue - dblMinValue)) - 3);
                }
                _with21.Width = 6;
                _with21.Height = 6;
                if ((MinGood == double.MinValue) | (MaxGood == double.MaxValue))
                {
                    _with21.Fill = new SolidColorBrush(ColourSelector.SelectColor(objDot, i));
                }
                else
                {
                    if (GetPlottedPropertyValue(objItem) >= MinGood && GetPlottedPropertyValue(objItem) <= MaxGood)
                    {
                        _with21.Fill = Brushes.Green;
                    }
                    else
                    {
                        _with21.Fill = Brushes.Red;
                    }
                }
                _with21.ToolTip = new ToolTip();
                _with21.Tag = i;
                objDot.ToolTipOpening += DisplayToolTip;
                objDot.MouseUp += ChartItemClicked;
                ctlChartArea.Children.Add(objDot);
                if (Orientation == Orientation.Vertical)
                {
                    objPolyline.Points.Add(new Point(intLeftMargin + 10 + i * 20,
                                                     ctlChartArea.Height *
                                                     ((dblMaxValue - GetPlottedPropertyValue(objItem)) /
                                                      (dblMaxValue - dblMinValue))));
                }
                else
                {
                    objPolyline.Points.Add(
                        new Point(
                            ctlChartArea.Width -
                            (ctlChartArea.Width - intLeftMargin) *
                            ((dblMaxValue - GetPlottedPropertyValue(objItem)) / (dblMaxValue - dblMinValue)),
                            ctlChartArea.Height - intBottomMargin - 10 - i * 20));
                }
                i += 1;
            }
        }

        #endregion

        #region "Bar Chart"

        private void DrawBarData(double dblMinValue, double dblMaxValue)
        {
            CollectionView myView = (CollectionView) CollectionViewSource.GetDefaultView(ItemsSource);

            int i = 0;
            foreach (object objItem in myView)
            {
                Border objBar = new Border();
                var _with22 = objBar;
                if (Orientation == Orientation.Vertical)
                {
                    _with22.Width = 14;
                    _with22.Height =
                        Math.Abs(ctlChartArea.Height * (GetPlottedPropertyValue(objItem) / (dblMaxValue - dblMinValue)));
                    _with22.SetValue(Canvas.LeftProperty, intLeftMargin + 10 + Convert.ToDouble(i * 20 - 7));
                    if (GetPlottedPropertyValue(objItem) >= 0)
                    {
                        _with22.SetValue(Canvas.TopProperty,
                                         ctlChartArea.Height * (dblMaxValue / (dblMaxValue - dblMinValue)) - _with22.Height);
                    }
                    else
                    {
                        _with22.SetValue(Canvas.TopProperty, ctlChartArea.Height * (dblMaxValue / (dblMaxValue - dblMinValue)));
                        _with22.LayoutTransform = new RotateTransform(180);
                    }
                }
                else
                {
                    _with22.Width = 14;
                    _with22.Height =
                        Math.Abs((ctlChartArea.Width - intLeftMargin) *
                                 (GetPlottedPropertyValue(objItem) / (dblMaxValue - dblMinValue)));
                    _with22.SetValue(Canvas.TopProperty, ctlChartArea.Height - intBottomMargin - Convert.ToDouble(i * 20) - 17);
                    if (GetPlottedPropertyValue(objItem) >= 0)
                    {
                        _with22.SetValue(Canvas.LeftProperty,
                                         ctlChartArea.Width -
                                         (ctlChartArea.Width - intLeftMargin) * (dblMaxValue / (dblMaxValue - dblMinValue)));
                        _with22.LayoutTransform = new RotateTransform(90);
                    }
                    else
                    {
                        _with22.SetValue(Canvas.LeftProperty,
                                         ctlChartArea.Width -
                                         (ctlChartArea.Width - intLeftMargin) *
                                         ((dblMaxValue - GetPlottedPropertyValue(objItem)) / (dblMaxValue - dblMinValue)));
                        _with22.LayoutTransform = new RotateTransform(270);
                    }
                }
                LinearGradientBrush objBrush = new LinearGradientBrush();
                var _with23 = objBrush;
                if (Orientation == Orientation.Vertical)
                {
                    _with23.StartPoint = new Point(0, 1);
                    _with23.EndPoint = new Point(0, 0);
                }
                else
                {
                    _with23.StartPoint = new Point(0, 1);
                    _with23.EndPoint = new Point(0, 0);
                }
                if ((MinGood == double.MinValue) | (MaxGood == double.MaxValue))
                {
                    _with23.GradientStops.Add(new GradientStop(Colors.White, 0));
                    _with23.GradientStops.Add(new GradientStop(ColourSelector.SelectColor(objBar, i), 1));
                }
                else
                {
                    if (GetPlottedPropertyValue(objItem) >= MinGood && GetPlottedPropertyValue(objItem) <= MaxGood)
                    {
                        _with23.GradientStops.Add(new GradientStop(Colors.LightGreen, 0));
                        _with23.GradientStops.Add(new GradientStop(Colors.Green, 1));
                    }
                    else
                    {
                        _with23.GradientStops.Add(new GradientStop(Colors.Pink, 0));
                        _with23.GradientStops.Add(new GradientStop(Colors.Red, 1));
                    }
                }
                _with22.Background = objBrush;
                _with22.CornerRadius = new CornerRadius(5, 5, 0, 0);
                _with22.BorderBrush = Brushes.Black;
                _with22.BorderThickness = new Thickness(1, 1, 1, 0);
                _with22.ToolTip = new ToolTip();
                _with22.Tag = i;
                objBar.ToolTipOpening += DisplayToolTip;
                objBar.MouseUp += ChartItemClicked;
                ctlChartArea.Children.Add(objBar);
                i += 1;
            }
        }

        #endregion

        #region "Pie Chart"

        private void SetPieChartSize()
        {
            ctlChartArea.Height = 300;
            ctlChartArea.Width = 300;
        }

        private void DrawPieChartData(double dblCumulativeTotal)
        {
            CollectionView myView = (CollectionView) CollectionViewSource.GetDefaultView(ItemsSource);

            double dblCumulativeAngle = 0.0;

            double CentreX = ctlChartArea.Width / 2;
            double CentreY = ctlChartArea.Height / 2;

            int i = 0;
            foreach (object objItem in myView)
            {
                double dblAngle = 360 * (GetPlottedPropertyValue(objItem) / dblCumulativeTotal);
                if (dblAngle == 360)
                {
                    dblAngle = 359.9;
                }

                PiePiece objPiePiece = new PiePiece();
                var _with24 = objPiePiece;
                _with24.CentreX = CentreX;
                _with24.CentreY = CentreY;
                _with24.WedgeAngle = dblAngle;
                _with24.Radius = CentreX;
                _with24.InnerRadius = _with24.Radius * 0.25;
                _with24.RotationAngle = dblCumulativeAngle;
                RadialGradientBrush objBrush = new RadialGradientBrush();
                var _with25 = objBrush;
                _with25.Center = new Point(0, 0);
                _with25.GradientOrigin = new Point(0, 0);
                _with25.RadiusX = 1;
                _with25.RadiusY = 1;

                //.StartPoint = New Point(0, 0)
                //.EndPoint = New Point(1, 1)
                _with25.GradientStops.Add(new GradientStop(Colors.White, 0));
                if ((MinGood == double.MinValue) | (MaxGood == double.MaxValue))
                {
                    _with25.GradientStops.Add(new GradientStop(ColourSelector.SelectColor(objPiePiece, i), 1));
                }
                else
                {
                    if (GetPlottedPropertyValue(objItem) >= MinGood && GetPlottedPropertyValue(objItem) <= MaxGood)
                    {
                        _with25.GradientStops.Add(new GradientStop(Colors.Green, 1));
                    }
                    else
                    {
                        _with25.GradientStops.Add(new GradientStop(Colors.Red, 1));
                    }
                }
                _with24.Fill = objBrush;
                _with24.Stroke = Brushes.White;
                _with24.ToolTip = new ToolTip();
                _with24.Tag = i;
                objPiePiece.ToolTipOpening += DisplayToolTip;
                objPiePiece.MouseUp += ChartItemClicked;

                ctlChartArea.Children.Add(objPiePiece);

                dblCumulativeAngle += dblAngle;
                i += 1;
            }
        }

        #endregion

        #region "Pareto Chart"

        private void SetParetoChartSize(double dblMinValue, double dblMaxValue)
        {
            SetSize(dblMinValue, dblMaxValue);

            if (Orientation == Orientation.Horizontal)
            {
                intTopMargin = StringLength("100") + 7;
                ctlChartArea.Height += intTopMargin;
            }
            else
            {
                intRightMargin = StringLength("100") + 7;
                ctlChartArea.Width += intRightMargin;
            }
        }

        private void DrawParetoPercentageAxis()
        {
            if (Orientation == Orientation.Vertical)
            {
                Line objAxis = new Line();
                var _with26 = objAxis;
                _with26.X1 = ctlChartArea.Width - intRightMargin;
                _with26.X2 = _with26.X1;
                _with26.Y1 = 0;
                _with26.Y2 = ctlChartArea.Height;
                _with26.Stroke = Brushes.Gray;
                ctlChartArea.Children.Add(objAxis);

                //Add markers
                for (int i = 0; i <= 100; i += 20)
                {
                    Line objMarker = new Line();
                    var _with27 = objMarker;
                    _with27.X1 = ctlChartArea.Width - intRightMargin;
                    _with27.X2 = _with27.X1 + 5;
                    _with27.Y1 = ctlChartArea.Height * (i / 100);
                    _with27.Y2 = _with27.Y1;
                    _with27.Stroke = Brushes.Gray;
                    ctlChartArea.Children.Add(objMarker);
                    TextBlock objLabel = new TextBlock();
                    var _with28 = objLabel;
                    _with28.SetValue(Canvas.TopProperty, ctlChartArea.Height - ctlChartArea.Height * (i / 100) - 10);
                    _with28.SetValue(Canvas.LeftProperty, Convert.ToDouble(ctlChartArea.Width - intRightMargin + 7));
                    _with28.Text = i.ToString();
                    _with28.Foreground = Brushes.Gray;
                    ctlChartArea.Children.Add(objLabel);
                }
            }
            else
            {
                Line objAxis = new Line();
                var _with29 = objAxis;
                _with29.X1 = intLeftMargin;
                _with29.X2 = ctlChartArea.Width;
                _with29.Y1 = intTopMargin;
                _with29.Y2 = _with29.Y1;
                _with29.Stroke = Brushes.Gray;
                ctlChartArea.Children.Add(objAxis);

                //Add markers
                for (int i = 0; i <= 100; i += 20)
                {
                    Line objMarker = new Line();
                    var _with30 = objMarker;
                    _with30.X1 = intLeftMargin + (ctlChartArea.Width - intLeftMargin) * (i / 100);
                    _with30.X2 = _with30.X1;
                    _with30.Y1 = intTopMargin - 5;
                    _with30.Y2 = intTopMargin;
                    _with30.Stroke = Brushes.Gray;
                    ctlChartArea.Children.Add(objMarker);
                    TextBlock objLabel = new TextBlock();
                    var _with31 = objLabel;
                    _with31.SetValue(Canvas.LeftProperty, intLeftMargin + (ctlChartArea.Width - intLeftMargin) * (i / 100) - 10);
                    _with31.SetValue(Canvas.TopProperty, Convert.ToDouble(0));
                    _with31.Text = i.ToString();
                    _with31.LayoutTransform = new RotateTransform(270);
                    _with31.Foreground = Brushes.Gray;
                    ctlChartArea.Children.Add(objLabel);
                }
            }
        }

        private void DrawParetoLineData(double dblMinValue, double dblMaxValue, double dblTotalValue)
        {
            CollectionView myView = (CollectionView) CollectionViewSource.GetDefaultView(ItemsSource);

            Polyline objPolyline = new Polyline();
            ctlChartArea.Children.Add(objPolyline);

            var _with32 = objPolyline;
            _with32.Stroke = Brushes.Black;

            int i = 0;
            double dblCumulative = 0.0;
            foreach (object objItem in myView)
            {
                dblCumulative += GetPlottedPropertyValue(objItem) / dblTotalValue * 100.0;
                Ellipse objDot = new Ellipse();
                var _with33 = objDot;
                if (Orientation == Orientation.Vertical)
                {
                    _with33.SetValue(Canvas.LeftProperty, intLeftMargin + 10 + Convert.ToDouble(i * 20 - 3));
                    _with33.SetValue(Canvas.TopProperty, ctlChartArea.Height * ((100 - dblCumulative) / 100) - 3);
                }
                else
                {
                    _with33.SetValue(Canvas.TopProperty, ctlChartArea.Height - intBottomMargin - 10 - i * 20 - 3);
                    _with33.SetValue(Canvas.LeftProperty,
                                     ctlChartArea.Width - (ctlChartArea.Width - intLeftMargin) * ((100 - dblCumulative) / 100) -
                                     3);
                }
                _with33.Width = 6;
                _with33.Height = 6;
                if ((MinGood == double.MinValue) | (MaxGood == double.MaxValue))
                {
                    _with33.Stroke = new SolidColorBrush(ColourSelector.SelectColor(objDot, i));
                }
                else
                {
                    if (GetPlottedPropertyValue(objItem) >= MinGood && GetPlottedPropertyValue(objItem) <= MaxGood)
                    {
                        _with33.Stroke = Brushes.Green;
                    }
                    else
                    {
                        _with33.Stroke = Brushes.Red;
                    }
                }
                _with33.Fill = Brushes.White;
                _with33.ToolTip = new ToolTip();
                _with33.Tag = i;
                objDot.ToolTipOpening += DisplayToolTip;
                objDot.MouseUp += ChartItemClicked;
                ctlChartArea.Children.Add(objDot);
                if (Orientation == Orientation.Vertical)
                {
                    objPolyline.Points.Add(new Point(intLeftMargin + 10 + i * 20,
                                                     ctlChartArea.Height * ((100 - dblCumulative) / 100)));
                }
                else
                {
                    objPolyline.Points.Add(
                        new Point(ctlChartArea.Width -
                                  (ctlChartArea.Width - intLeftMargin) * ((100 - dblCumulative) / 100),
                                  ctlChartArea.Height - intBottomMargin - 10 - i * 20));
                }
                i += 1;
            }
        }

        #endregion

        #endregion
    }
}