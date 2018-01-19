// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Autodesk.WPFControls
{
    [TemplatePart(Name = "PART_InputTextBox", Type = typeof(TextBox))]
    public class DelcamEditBox : Control, INotifyPropertyChanged
    {
        public static readonly DependencyProperty TextProperty;

        public static readonly DependencyProperty InputMaskProperty;

        private static readonly string PART_InputTextBox = "PART_InputTextBox";

        static DelcamEditBox()
        {
            //This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
            //This style is defined in themes\generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DelcamEditBox), new FrameworkPropertyMetadata(typeof(DelcamEditBox)));
            TextProperty = DependencyProperty.Register("TextProperty",
                                                       typeof(string),
                                                       typeof(DelcamEditBox),
                                                       new PropertyMetadata(""));
            InputMaskProperty = DependencyProperty.Register("InputMaskProperty",
                                                            typeof(string),
                                                            typeof(DelcamEditBox),
                                                            new PropertyMetadata("", InputMask_Changed));
        }

        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set
            {
                SetValue(TextProperty, value);
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Text"));
                }
            }
        }

        private static void InputMask_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DelcamEditBox objBox = (DelcamEditBox) d;

            if (objBox != null)
            {
                objBox.OnInputMaskChanged(e.OldValue.ToString(), e.NewValue.ToString());
            }
        }

        public string InputMask
        {
            get { return (string) GetValue(InputMaskProperty); }
            set { SetValue(InputMaskProperty, value); }
        }

        private List<EditPart> lstParts = new List<EditPart>();

        private int intCurrentPartIndex;

        private class EditPart
        {
            public enum E_CharacterClass
            {
                Alpha,
                Numeric,
                AlphaNumeric
            }

            public enum E_Multiplicity
            {
                One,
                Many,
                Defined
            }

            private E_CharacterClass enmCharacterClass;
            private E_Multiplicity enmMultiplicity;
            private int intCount;
            private string strCurrentText;

            private string strPartEndString;

            public EditPart(E_CharacterClass enmCharacterClass, E_Multiplicity enmMultiplicty, int intCount = 0)
            {
                this.enmCharacterClass = enmCharacterClass;
                enmMultiplicity = enmMultiplicty;
                this.intCount = intCount;
                strCurrentText = "";
            }

            public E_CharacterClass CharacterClass
            {
                get { return enmCharacterClass; }
            }

            public E_Multiplicity Multiplicity
            {
                get { return enmMultiplicity; }
            }

            public int Count
            {
                get { return intCount; }
            }

            public string PartEndString
            {
                get { return strPartEndString; }
                set { strPartEndString = value; }
            }

            public string CurrentText
            {
                get
                {
                    if (strCurrentText == null)
                    {
                        return "";
                    }
                    return strCurrentText;
                }
                set { strCurrentText = value; }
            }
        }

        public virtual void OnInputMaskChanged(string strOldValue, string strNewValue)
        {
            //Clear the text
            Text = "";

            //Need to section up the Input Mask
            lstParts = new List<EditPart>();

            string strPart = "";
            string strDisplayTextCalc = "";
            string strMaskTextCalc = "";
            for (int i = 0; i <= strNewValue.Length; i++)
            {
                if (i == strNewValue.Length || (strNewValue[i] == '.') | (strNewValue[i] == '-') | (strNewValue[i] == '@'))
                {
                    EditPart.E_CharacterClass enmCharacterClass = default(EditPart.E_CharacterClass);
                    EditPart.E_Multiplicity enmMultiplicity = default(EditPart.E_Multiplicity);
                    int intCount = 0;
                    if (strPart.StartsWith("X"))
                    {
                        enmCharacterClass = EditPart.E_CharacterClass.AlphaNumeric;
                    }
                    else if (strPart.StartsWith("A"))
                    {
                        enmCharacterClass = EditPart.E_CharacterClass.Alpha;
                    }
                    else if (strPart.StartsWith("N"))
                    {
                        enmCharacterClass = EditPart.E_CharacterClass.Numeric;
                    }
                    if (strPart.Length == 1)
                    {
                        enmMultiplicity = EditPart.E_Multiplicity.One;
                    }
                    else if (strPart.EndsWith("*"))
                    {
                        enmMultiplicity = EditPart.E_Multiplicity.Many;
                    }
                    else if (strPart.Contains("+"))
                    {
                        enmMultiplicity = EditPart.E_Multiplicity.Defined;
                        intCount = Convert.ToInt32(strPart.Substring(strPart.IndexOf("+") + 1));
                    }
                    EditPart objEditPart = new EditPart(enmCharacterClass, enmMultiplicity, intCount);
                    if (i != strNewValue.Length)
                    {
                        objEditPart.PartEndString = strNewValue[i].ToString();
                    }
                    lstParts.Add(objEditPart);
                    strPart = "";

                    if ((objEditPart.Multiplicity == EditPart.E_Multiplicity.One) |
                        (objEditPart.Multiplicity == EditPart.E_Multiplicity.Many))
                    {
                        strDisplayTextCalc += " " + objEditPart.PartEndString;
                        strMaskTextCalc += "X" + objEditPart.PartEndString;
                    }
                    else
                    {
                        for (int j = 0; j <= intCount - 1; j++)
                        {
                            strDisplayTextCalc += " " + objEditPart.PartEndString;
                            strMaskTextCalc += "X" + objEditPart.PartEndString;
                        }
                    }
                }
                else
                {
                    strPart += strNewValue[i];
                }
            }

            strDisplayText = strDisplayTextCalc;
            Text = strDisplayTextCalc;
            strMaskText = strMaskTextCalc;

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("InputMask"));
            }
        }

        private string strMaskText = "";

        public string MaskText
        {
            get { return strMaskText; }
            internal set { strMaskText = value; }
        }

        private string strDisplayText = "";

        public string DisplayText
        {
            get { return strDisplayText; }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            intCurrentPartIndex = 0;
            for (int i = 0; i <= ((TextBox) GetTemplateChild(PART_InputTextBox)).CaretIndex - 1; i++)
            {
                switch (Text[i])
                {
                    case '.':
                    case '-':
                    case '@':
                        intCurrentPartIndex += 1;
                        break;
                    default:
                        break;
                }
            }

            if (e.Key == Key.Back)
            {
                if (((TextBox) GetTemplateChild(PART_InputTextBox)).CaretIndex > 0)
                {
                    ChangeText(((TextBox) GetTemplateChild(PART_InputTextBox)).CaretIndex, strDisplayText, e.Key);
                }
                e.Handled = true;
            }
            else if (e.Key == Key.Delete)
            {
                if (((TextBox) GetTemplateChild(PART_InputTextBox)).CaretIndex < Text.Length - 1)
                {
                    ChangeText(((TextBox) GetTemplateChild(PART_InputTextBox)).CaretIndex, strDisplayText, e.Key);
                }
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            //First chance to ban any key presses we dont want
            if ((e.Key >= Key.A) & (e.Key <= Key.Z) || (e.Key >= Key.D0) & (e.Key <= Key.D9) ||
                (e.Key >= Key.NumPad0) & (e.Key <= Key.NumPad9))
            {
                //Alpha numeric key press
                bool blnValid = IsEntryValid(((TextBox) GetTemplateChild(PART_InputTextBox)).CaretIndex, Text, e.Key);
                if (blnValid)
                {
                    ChangeText(((TextBox) GetTemplateChild(PART_InputTextBox)).CaretIndex, strDisplayText, e.Key);
                }
            }

            //We want to handle everything
            e.Handled = true;
        }

        protected void ChangeText(int intIndex, string strText, Key enmKey)
        {
            //Work out the actual text by removing the spaces
            string strNewValue = strText;
            if (enmKey == Key.Back)
            {
                switch (strNewValue[intIndex - 1])
                {
                    case '.':
                    case '-':
                    case '@':
                        break;
                    default:
                        strNewValue = strNewValue.Remove(intIndex - 1, 1);
                        break;
                }
            }
            else if (enmKey == Key.Delete)
            {
                switch (strNewValue[intIndex])
                {
                    case '.':
                    case '-':
                    case '@':
                        break;
                    default:
                        strNewValue = strNewValue.Remove(intIndex, 1);
                        break;
                }
            }
            else
            {
                switch (strNewValue[intIndex])
                {
                    case '.':
                    case '-':
                    case '@':
                        break;
                    case ' ':
                        strNewValue = strNewValue.Remove(intIndex, 1);
                        break;
                    default:
                        break;
                }
                string strCharacter = null;
                strCharacter = enmKey.ToString().Last().ToString();
                strNewValue = strNewValue.Insert(intIndex, strCharacter);
            }

            string strActualText = strNewValue.Replace(" ", "");

            //Split it up and update the parts
            string[] strParts = strActualText.Split('.', '-', '@');

            int i = 0;
            foreach (string strPart in strParts)
            {
                lstParts[i].CurrentText = strPart;
                i += 1;
            }

            string strMaskTextCalc = "";
            string strDisplayTextCalc = "";

            //Update the mask, display text, and input text boxes
            //Also determine current part and move the caret
            int intCountPartIndex = 0;
            for (i = 0; i <= strNewValue.Length - 1; i++)
            {
                switch (strNewValue[i])
                {
                    case '.':
                    case '-':
                    case '@':

                        //Add any necessary spaces/prompts
                        switch (lstParts[intCountPartIndex].Multiplicity)
                        {
                            case EditPart.E_Multiplicity.One:
                                if (lstParts[intCountPartIndex].CurrentText.Length < 1)
                                {
                                    strMaskTextCalc += "X";
                                    strDisplayTextCalc += " ";
                                }
                                break;
                            case EditPart.E_Multiplicity.Many:
                                if (lstParts[intCountPartIndex].CurrentText.Length < 1)
                                {
                                    strMaskTextCalc += "X";
                                    strDisplayTextCalc += " ";
                                }
                                break;
                            case EditPart.E_Multiplicity.Defined:
                                for (int j = 0;
                                    j <= lstParts[intCountPartIndex].Count - lstParts[intCountPartIndex].CurrentText.Length - 1;
                                    j++)
                                {
                                    strMaskTextCalc += "X";
                                    strDisplayTextCalc += " ";
                                }

                                break;
                        }

                        //Add the character
                        strMaskTextCalc += strNewValue[i];
                        strDisplayTextCalc += strNewValue[i];

                        //Move to next part
                        intCountPartIndex += 1;
                        break;
                    case ' ':
                        break;

                    //ignore
                    default:

                        //Add the character
                        strMaskTextCalc += strNewValue[i];
                        strDisplayTextCalc += strNewValue[i];
                        break;
                }
            }

            //Do the last section
            switch (lstParts[intCountPartIndex].Multiplicity)
            {
                case EditPart.E_Multiplicity.One:
                    if (lstParts[intCountPartIndex].CurrentText.Length < 1)
                    {
                        strMaskTextCalc += "X";
                        strDisplayTextCalc += " ";
                    }
                    break;
                case EditPart.E_Multiplicity.Many:
                    if (lstParts[intCountPartIndex].CurrentText.Length < 1)
                    {
                        strMaskTextCalc += "X";
                        strDisplayTextCalc += " ";
                    }
                    break;
                case EditPart.E_Multiplicity.Defined:
                    for (int j = 0;
                        j <= lstParts[intCountPartIndex].Count - lstParts[intCountPartIndex].CurrentText.Length - 1;
                        j++)
                    {
                        strMaskTextCalc += "X";
                        strDisplayTextCalc += " ";
                    }

                    break;
            }

            strMaskText = strMaskTextCalc;
            strDisplayText = strDisplayTextCalc;
            Text = strDisplayTextCalc;

            //Adjust the caret index
            if (enmKey == Key.Back)
            {
                ((TextBox) GetTemplateChild(PART_InputTextBox)).CaretIndex = intIndex - 1;
            }
            else if (enmKey == Key.Delete)
            {
                //no need to move it
            }
            else
            {
                if (intIndex + 1 < strNewValue.Length)
                {
                    switch (strNewValue[intIndex + 1])
                    {
                        case '.':
                        case '-':
                        case '@':

                            //Move the caret on two places
                            if (lstParts[intCurrentPartIndex].Multiplicity == EditPart.E_Multiplicity.Many)
                            {
                                ((TextBox) GetTemplateChild(PART_InputTextBox)).CaretIndex = intIndex + 1;
                            }
                            else
                            {
                                ((TextBox) GetTemplateChild(PART_InputTextBox)).CaretIndex = intIndex + 2;
                            }
                            break;
                        default:

                            //Move the caret on one place
                            ((TextBox) GetTemplateChild(PART_InputTextBox)).CaretIndex = intIndex + 1;
                            break;
                    }
                }
                else
                {
                    ((TextBox) GetTemplateChild(PART_InputTextBox)).CaretIndex = strNewValue.Length;
                }
            }

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Text"));
            }
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("MaskText"));
            }
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("DisplayText"));
            }
        }

        private bool IsEntryValid(int intIndex, string strText, Key enmKey)
        {
            EditPart objCurrentPart = lstParts[intCurrentPartIndex];
            if ((enmKey >= Key.A) & (enmKey <= Key.Z))
            {
                if (objCurrentPart.CharacterClass == EditPart.E_CharacterClass.Numeric)
                {
                    return false;
                }
                switch (objCurrentPart.Multiplicity)
                {
                    case EditPart.E_Multiplicity.One:
                        if (objCurrentPart.CurrentText.Length >= 1)
                        {
                            return false;
                        }
                        break;
                    case EditPart.E_Multiplicity.Many:

                        break;
                    case EditPart.E_Multiplicity.Defined:
                        if (objCurrentPart.CurrentText.Length >= objCurrentPart.Count)
                        {
                            return false;
                        }
                        break;
                }
            }
            else if ((enmKey >= Key.D0) & (enmKey <= Key.D9) || (enmKey >= Key.NumPad0) & (enmKey <= Key.NumPad9))
            {
                if (objCurrentPart.CharacterClass == EditPart.E_CharacterClass.Alpha)
                {
                    return false;
                }
                switch (objCurrentPart.Multiplicity)
                {
                    case EditPart.E_Multiplicity.One:
                        if (objCurrentPart.CurrentText.Length >= 1)
                        {
                            return false;
                        }
                        break;
                    case EditPart.E_Multiplicity.Many:

                        break;
                    case EditPart.E_Multiplicity.Defined:
                        if (objCurrentPart.CurrentText.Length >= objCurrentPart.Count)
                        {
                            return false;
                        }
                        break;
                }
            }
            else
            {
                //Invalid key return false
                return false;
            }

            //No problems so retrun true
            return true;
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        public delegate void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e);
    }
}