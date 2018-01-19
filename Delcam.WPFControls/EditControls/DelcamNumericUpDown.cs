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
using System.Windows.Input;
using Microsoft.VisualBasic;

namespace Autodesk.WPFControls
{
    [TemplatePart(Name = "PART_Value", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_Up", Type = typeof(RepeatButton))]
    [TemplatePart(Name = "PART_Down", Type = typeof(RepeatButton))]
    public class DelcamNumericUpDown : Control
    {
        #region "Template Parts"

        public readonly string PART_Value = "PART_Value";
        private readonly string PART_Up = "PART_Up";

        private readonly string PART_Down = "PART_Down";

        #endregion

        #region "Constructors"

        static DelcamNumericUpDown()
        {
            InitialiseCommands();

            //This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
            //This style is defined in themes\generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DelcamNumericUpDown),
                                                     new FrameworkPropertyMetadata(typeof(DelcamNumericUpDown)));
        }

        #endregion

        #region "Operations"

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            //Bind the value TextBox's Text property to the Value property of the control
            TextBox objValue = (TextBox) GetTemplateChild(PART_Value);
            if (objValue != null)
            {
                objValue.PreviewKeyDown += ValidateKeyDown;
            }
        }

        #endregion

        #region "Properties"

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value",
                                                                                              typeof(decimal),
                                                                                              typeof(DelcamNumericUpDown),
                                                                                              new PropertyMetadata(0m,
                                                                                                                   ValuePropertyChanged,
                                                                                                                   ValuePropertyCoerce))
            ;

        public decimal Value
        {
            get { return (decimal) GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent("ValueChanged",
                                                                                       RoutingStrategy.Bubble,
                                                                                       typeof(
                                                                                           RoutedPropertyChangedEventArgs<decimal>
                                                                                       ),
                                                                                       typeof(DelcamNumericUpDown));

        public event RoutedEventHandler ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }

            remove { RemoveHandler(ValueChangedEvent, value); }
        }

        private static void ValuePropertyChanged(DependencyObject g, DependencyPropertyChangedEventArgs e)
        {
            DelcamNumericUpDown numericUpDown = (DelcamNumericUpDown) g;

            decimal oldValue = (decimal) e.OldValue;
            decimal newValue = (decimal) e.NewValue;

            RoutedPropertyChangedEventArgs<decimal> args = new RoutedPropertyChangedEventArgs<decimal>(oldValue,
                                                                                                       newValue,
                                                                                                       ValueChangedEvent);
            numericUpDown.OnValueChanged(args);
        }

        protected virtual void OnValueChanged(RoutedPropertyChangedEventArgs<decimal> args)
        {
            // Raise the ValueChanged Event
            RaiseEvent(args);
        }

        private static object ValuePropertyCoerce(DependencyObject g, object value)
        {
            decimal decMax = ((DelcamNumericUpDown) g).MaximumValue;
            decimal decMin = ((DelcamNumericUpDown) g).MinimumValue;

            if (Information.IsNumeric(value) == false)
            {
                value = decMin;
            }

            if ((decimal) value > decMax)
            {
                return decMax;
            }
            if ((decimal) value < decMin)
            {
                return decMin;
            }
            return value;
        }

        public static readonly DependencyProperty IncrementProperty = DependencyProperty.Register("Increment",
                                                                                                  typeof(decimal),
                                                                                                  typeof(DelcamNumericUpDown),
                                                                                                  new PropertyMetadata(0.1m));

        public decimal Increment
        {
            get { return (decimal) GetValue(IncrementProperty); }
            set { SetValue(IncrementProperty, value); }
        }

        public static readonly DependencyProperty MinimumValueProperty = DependencyProperty.Register("MinimumValue",
                                                                                                     typeof(decimal),
                                                                                                     typeof(DelcamNumericUpDown),
                                                                                                     new PropertyMetadata(
                                                                                                         decimal.MinValue));

        public decimal MinimumValue
        {
            get { return (decimal) GetValue(MinimumValueProperty); }
            set { SetValue(MinimumValueProperty, value); }
        }

        public static readonly DependencyProperty MaximumValueProperty = DependencyProperty.Register("MaximumValue",
                                                                                                     typeof(decimal),
                                                                                                     typeof(DelcamNumericUpDown),
                                                                                                     new PropertyMetadata(
                                                                                                         decimal.MaxValue));

        public decimal MaximumValue
        {
            get { return (decimal) GetValue(MaximumValueProperty); }
            set { SetValue(MaximumValueProperty, value); }
        }

        #endregion

        #region "Commands"

        public static readonly RoutedCommand IncreaseCommand = new RoutedCommand("IncreaseCommand", typeof(DelcamNumericUpDown));

        public static readonly RoutedCommand DecreaseCommand = new RoutedCommand("DecreaseCommand", typeof(DelcamNumericUpDown));

        private static void InitialiseCommands()
        {
            CommandManager.RegisterClassCommandBinding(typeof(DelcamNumericUpDown),
                                                       new CommandBinding(IncreaseCommand, IncreaseCommand_Execute));
            CommandManager.RegisterClassCommandBinding(typeof(DelcamNumericUpDown),
                                                       new CommandBinding(DecreaseCommand, DecreaseCommand_Execute));
        }

        public static void IncreaseCommand_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            DelcamNumericUpDown objControl = (DelcamNumericUpDown) sender;

            if (objControl != null)
            {
                objControl.Increase();
            }
        }

        public void Increase()
        {
            Value += Increment;
        }

        public static void DecreaseCommand_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            DelcamNumericUpDown objControl = (DelcamNumericUpDown) sender;

            if (objControl != null)
            {
                objControl.Decrease();
            }
        }

        public void Decrease()
        {
            Value -= Increment;
        }

        #endregion

        #region "Events"

        private void ValidateKeyDown(Object sender, KeyEventArgs e)
        {
            if (((e.Key >= Key.D0) & (e.Key <= Key.D9)) | ((e.Key >= Key.NumPad0) & (e.Key <= Key.NumPad9)))
            {
                if ((((TextBox) e.Source).CaretIndex == 0) & (e.Key == Key.OemMinus))
                {
                    e.Handled = true;
                }
            }
            else if ((e.Key == Key.OemPeriod) | (e.Key == Key.Decimal))
            {
                if (e.Key == Key.OemPeriod)
                {
                    e.Handled = true;
                }
                else if ((((TextBox) e.Source).CaretIndex == 0) & (e.Key == Key.OemMinus))
                {
                    e.Handled = true;
                }
            }
            else if ((e.Key == Key.Delete) | (e.Key == Key.Back))
            {
                //ok
            }
            else if ((e.Key == Key.Subtract) | (e.Key == Key.OemMinus))
            {
                if (e.Key == Key.OemMinus)
                {
                    e.Handled = true;
                }
                else if (((TextBox) e.Source).CaretIndex == 0)
                {
                    if (MinimumValue < 0)
                    {
                        //ok
                    }
                    else
                    {
                        e.Handled = true;
                    }
                }
                else
                {
                    e.Handled = true;
                }
            }
            else if ((e.Key == Key.Left) | (e.Key == Key.Right))
            {
                //ok
            }
            else if (e.Key == Key.Up)
            {
                Increase();
            }
            else if (e.Key == Key.Down)
            {
                Decrease();
            }
            else if (e.Key == Key.Tab)
            {
                //ok
            }
            else
            {
                e.Handled = true;
            }
        }

        #endregion
    }
}