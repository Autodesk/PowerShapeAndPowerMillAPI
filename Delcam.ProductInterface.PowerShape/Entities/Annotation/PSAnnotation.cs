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
    /// Captures an Annotation object in PowerSHAPE
    /// </summary>
    public class PSAnnotation : PSEntity
    {
        #region " Fields "

        #endregion

        #region " Constructors "

        /// <summary>
        /// Initialises the Annotation
        /// </summary>
        internal PSAnnotation(PSAutomation powerSHAPE, int id, string name) : base(powerSHAPE, id, name)
        {
        }

        /// <summary>
        /// Creates a new annotation in PowwerSHAPE.
        /// </summary>
        /// <param name="powerSHAPE">The base instance to interact with PowerShape.</param>
        /// <param name="text">The text to introduce in PowerShape.</param>
        /// <param name="fontName">
        /// The font type for the text (e.g. Arial, Times New Roman). The font Type must exist in
        /// PowerShape.
        /// </param>
        /// <param name="height">The font height.</param>
        /// <param name="position">The position of the text in the active workplane.</param>
        /// <remarks>The created text will have pitch of 0, text origin will be 'CENTRE' and text justification will be 'LEFT'.</remarks>
        internal PSAnnotation(
            PSAutomation powerSHAPE,
            string text,
            string fontName,
            double height,
            Point position) : base(powerSHAPE)
        {
            //Clear the list of CreatedItems
            _powerSHAPE.ActiveModel.ClearCreatedItems();

            // Create text in PowerSHape
            CreateText(powerSHAPE, height, fontName, 0, TextOrigins.Centre, TextJustifications.Left, position, text);

            //Now get its Id
            PSAnnotation annotation = (PSAnnotation) _powerSHAPE.ActiveModel.CreatedItems[0];
            _id = annotation.Id;
            _name = annotation.Name;
        }

        /// <summary>
        /// Creates a new annotation in PowwerSHAPE.
        /// </summary>
        /// <param name="powerSHAPE">The base instance to interact with PowerShape.</param>
        /// <param name="text">The text to introduce in PowerShape.</param>
        /// <param name="fontName">
        /// The font type for the text (e.g. Arial, Times New Roman). The font Type must exist in
        /// PowerShape.
        /// </param>
        /// <param name="height">The font height.</param>
        /// <param name="pitch">The distance between each character in a line.</param>
        /// <param name="textJustification">
        /// The wanted justify option. This is how the text in the block aligns with the left and
        /// right margins.
        /// </param>
        /// <param name="textOrigin">
        /// The origin of the text block. The origin is the 'anchor point' used to position the text and
        /// can be set at any of the four corners of the text box, the mid points of the sides of the text box or the centre of the
        /// text box. Use the option menu to change the origin of the text.
        /// </param>
        /// <param name="position">The position of the text in the active workplane.</param>
        /// <remarks></remarks>
        internal PSAnnotation(
            PSAutomation powerSHAPE,
            string text,
            string fontName,
            double height,
            Point position,
            double pitch,
            TextJustifications textJustification,
            TextOrigins textOrigin) : base(powerSHAPE)
        {
            // Create text in PowerSHape
            CreateText(powerSHAPE, height, fontName, pitch, textOrigin, textJustification, position, text);

            //Now get its Id
            PSAnnotation annotation = (PSAnnotation) _powerSHAPE.ActiveModel.CreatedItems[0];
            _id = annotation.Id;
            _name = annotation.Name;
        }

        #endregion

        #region " Properties "

        internal const string ANNOTATION_IDENTIFIER = "TEXT";

        /// <summary>
        /// Gets the identifier PowerSHAPE uses to identify this type of entity.
        /// </summary>
        internal override string Identifier
        {
            get { return ANNOTATION_IDENTIFIER; }
        }

        /// <summary>
        /// Gets the composite ID for use in communication with the ActiveDocument object.
        /// </summary>
        internal override int CompositeID
        {
            get { return 61 * 20000000 + _id; }
        }

        /// <summary>
        /// BoundingBox property is not valid for this type
        /// </summary>
        public override BoundingBox BoundingBox
        {
            get { throw new Exception("Property not valid for " + GetType()); }
        }

        /// <summary>
        /// Gets and sets the text of the Annotation.
        /// </summary>
        public string Text
        {
            get { return Convert.ToString(_powerSHAPE.DoCommandEx(Identifier + "['" + Name + "'].string")); }
            set
            {
                AddToSelection(true);
                _powerSHAPE.DoCommand(string.Format("ScrolledText {0}", value), "ACCEPT");
            }
        }

        /// <summary>
        /// Gets and sets the position of the Annotation. Point with the position at which the text was placed.
        /// </summary>
        public Point Position
        {
            get
            {
                // The returned string should be in the following format [-20.036979, 326.789583, -0.000000]
                double[] positions = _powerSHAPE.DoCommandEx(Identifier + "['" + Name + "'].position") as double[];

                return new Point(positions[0], positions[1], positions[2]);
            }
        }

        /// <summary>
        /// Gets and sets the font type name of the Annotation.
        /// </summary>
        public string Font
        {
            get { return Convert.ToString(_powerSHAPE.DoCommandEx(Identifier + "['" + Name + "'].font")); }
            set
            {
                AddToSelection(true);
                _powerSHAPE.DoCommand(string.Format("{0} FONT {1}", Identifier, value));
            }
        }

        /// <summary>
        /// Gets and sets the height of the characters.
        /// </summary>
        public double Height
        {
            get { return Convert.ToDouble(_powerSHAPE.DoCommandEx(Identifier + "['" + Name + "'].char_height")); }

            set
            {
                AddToSelection(true);
                _powerSHAPE.DoCommand(string.Format("{0} HEIGHT {1}", Identifier, value));
            }
        }

        /// <summary>
        /// Sets whether the text is Bold or not. A True value will set it to Bold.
        /// </summary>
        public bool Bold
        {
            set
            {
                AddToSelection(true);
                _powerSHAPE.DoCommand(string.Format("{0} BOLD {1}", Identifier, value ? "ON" : "OFF"));
            }
        }

        /// <summary>
        /// Gets and sets whether the Annotation is italic.
        /// </summary>
        public bool Italic
        {
            get { return Convert.ToInt32(_powerSHAPE.DoCommandEx(Identifier + "['" + Name + "'].italic")) == 1; }

            set
            {
                AddToSelection(true);
                _powerSHAPE.DoCommand(string.Format("{0} ITALIC {1}", Identifier, value ? "ON" : "OFF"));
            }
        }

        /// <summary>
        /// Sets whether the Annotation is underlined.
        /// </summary>
        public bool Underline
        {
            set
            {
                AddToSelection(true);
                _powerSHAPE.DoCommand(string.Format("{0} UNDERLINE {1}", Identifier, value ? "ON" : "OFF"));
            }
        }

        /// <summary>
        /// Sets the Pitch of the Annotation.
        /// </summary>
        public double Pitch
        {
            set
            {
                AddToSelection(true);
                _powerSHAPE.DoCommand(string.Format("{0} PITCH {1}", Identifier, value));
            }
        }

        /// <summary>
        /// Sets the Spacing of the Annotation. Spacing between lines of text.
        /// </summary>
        public double Spacing
        {
            get { return Convert.ToDouble(_powerSHAPE.DoCommandEx(Identifier + "['" + Name + "'].line_spacing")); }

            set
            {
                if ((value < 0) | (value > 5))
                {
                    throw new ArgumentException("Spacing value as to be between 0 and 5.");
                }

                AddToSelection(true);
                _powerSHAPE.DoCommand(string.Format("{0} SPACING {1}", Identifier, value));
            }
        }

        /// <summary>
        /// Gets and sets the text Angle.
        /// </summary>
        public Degree Angle
        {
            get { return Convert.ToDouble(_powerSHAPE.DoCommandEx(Identifier + "['" + Name + "'].angle")); }
            set
            {
                AddToSelection(true);
                _powerSHAPE.DoCommand(string.Format("{0} ANGLE {1}", Identifier, value));
            }
        }

        /// <summary>
        /// Gets and sets if text characters are Horizontal.
        /// </summary>
        public bool Horizontal
        {
            get { return Convert.ToInt32(_powerSHAPE.DoCommandEx(Identifier + "['" + Name + "'].horizontal")) == 1; }
        }

        /// <summary>
        /// Gets and sets the Justification of the Annotation. This is how the text in the block aligns with the left and
        /// right margins.
        /// </summary>
        public TextJustifications Justification
        {
            get
            {
                var textJustification = Convert.ToString(_powerSHAPE.DoCommandEx(Identifier + "['" + Name + "'].justification"));
                return ConvertToTextJustification(textJustification);
            }
            set
            {
                AddToSelection(true);
                _powerSHAPE.DoCommand(string.Format("{0} JUSTIFICATION {1}", Identifier, ConvertToString(value)));
            }
        }

        /// <summary>
        /// Gets and sets the Origin of the Annotation. The origin is the 'anchor point' used to position the text and
        /// can be set at any of the four corners of the text box, the mid points of the sides of the text box or the centre of the
        /// text box.
        /// </summary>
        public TextOrigins Origin
        {
            get
            {
                var textOrigin = Convert.ToString(_powerSHAPE.DoCommandEx(Identifier + "['" + Name + "'].origin"));
                return ConvertToTextOrigins(textOrigin);
            }

            set
            {
                AddToSelection(true);
                _powerSHAPE.DoCommand(string.Format("{0} ORIGIN {1}", Identifier, ConvertToString(value)));
            }
        }

        /// <summary>
        /// Gets the number of the colour used by the text.
        /// </summary>
        public int Colour
        {
            get { return Convert.ToInt32(_powerSHAPE.DoCommandEx(Identifier + "['" + Name + "'].colour")); }
        }

        #endregion

        #region " Operations "

        /// <summary>
        /// This operation deletes the Annotation from PowerSHAPE and removes it from the Annotations collection
        /// </summary>
        public override void Delete()
        {
            _powerSHAPE.ActiveModel.Annotations.Remove(this);
        }

        #endregion

        #region " Implementation "

        private void CreateText(
            PSAutomation powerSHAPE,
            double height,
            string fontName,
            double pitch,
            TextOrigins textOrigin,
            TextJustifications textJustification,
            Point position,
            string text)
        {
            //Clear the list of CreatedItems
            _powerSHAPE.ActiveModel.ClearCreatedItems();

            //Set preferences to be able to introduce text by commands
            powerSHAPE.DoCommand("TOOLS PREFERENCES");
            powerSHAPE.DoCommand("UNITPREFS");
            powerSHAPE.DoCommand("TEXTPREFS");
            powerSHAPE.DoCommand("TEXT LIVETEXT OFF");
            powerSHAPE.DoCommand("ACCEPT");

            //Create the text
            powerSHAPE.SetActivePlane(Planes.XY);
            powerSHAPE.DoCommand("CREATE TEXT TEXT HORIZONTAL YES");
            powerSHAPE.DoCommand(string.Format("TEXT HEIGHT {0}", height));
            powerSHAPE.DoCommand(string.Format("TEXT FONT {0}", fontName));
            powerSHAPE.DoCommand(string.Format("TEXT PITCH {0}", pitch));
            powerSHAPE.DoCommand(string.Format("TEXT ORIGIN {0}", ConvertToString(textOrigin)));
            powerSHAPE.DoCommand(string.Format("TEXT JUSTIFICATION {0}", ConvertToString(textJustification)));
            powerSHAPE.DoCommand(string.Format("{0} {1} {2}", position.X, position.Y, position.Z));
            powerSHAPE.DoCommand(string.Format("ScrolledText {0}", text));
            powerSHAPE.DoCommand("ACCEPT");
        }

        private string ConvertToString(TextJustifications textJustification)
        {
            switch (textJustification)
            {
                case TextJustifications.Centre:
                    return "CENTRE";
                case TextJustifications.Left:
                    return "LEFT";
                case TextJustifications.Right:
                    return "RIGHT";
                default:
                    throw new ArgumentOutOfRangeException("Argument outside the range of values supported.");
            }
        }

        private TextJustifications ConvertToTextJustification(string textOrigin)
        {
            switch (textOrigin.ToUpperInvariant())
            {
                case "CENTRE":
                    return TextJustifications.Centre;
                case "LEFT":
                    return TextJustifications.Left;
                case "RIGHT":
                    return TextJustifications.Right;
                default:
                    throw new ArgumentOutOfRangeException("Argument outside the range of values supported.");
            }
        }

        private TextOrigins ConvertToTextOrigins(string textOrigin)
        {
            switch (textOrigin.ToUpperInvariant())
            {
                case "BOTTOM CENTRE":
                    return TextOrigins.BottomCentre;
                case "BOTTOM LEFT":
                    return TextOrigins.BottomLeft;
                case "BOTTOM RIGHT":
                    return TextOrigins.BottomRight;
                case "CENTRE":
                    return TextOrigins.Centre;
                case "CENTRE LEFT":
                    return TextOrigins.LeftCentre;
                case "CENTRE RIGHT":
                    return TextOrigins.RightCentre;
                case "TOP CENTRE":
                    return TextOrigins.TopCentre;
                case "TOP LEFT":
                    return TextOrigins.TopLeft;
                case "TOP RIGHT":
                    return TextOrigins.TopRight;
                default:
                    throw new ArgumentOutOfRangeException("Argument outside the range of values supported.");
            }
        }

        private string ConvertToString(TextOrigins textOrigin)
        {
            switch (textOrigin)
            {
                case TextOrigins.BottomCentre:
                    return "BOTTOMCENTRE";
                case TextOrigins.BottomLeft:
                    return "BOTTOMLEFT";
                case TextOrigins.BottomRight:
                    return "BOTTOMRIGHT";
                case TextOrigins.Centre:
                    return "CENTRE";
                case TextOrigins.LeftCentre:
                    return "CENTRELEFT";
                case TextOrigins.RightCentre:
                    return "CENTRERIGHT";
                case TextOrigins.TopCentre:
                    return "TOPCENTRE";
                case TextOrigins.TopLeft:
                    return "TOPLEFT";
                case TextOrigins.TopRight:
                    return "TOPRIGHT";
                default:
                    throw new ArgumentOutOfRangeException("Argument outside the range of values supported.");
            }
        }

        #endregion
    }
}