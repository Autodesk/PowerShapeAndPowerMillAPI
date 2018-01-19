// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using Autodesk.Geometry;

namespace Autodesk.ProductInterface.PowerSHAPE
{
    /// <summary>
    /// Captures the collection of Annotations in a Project
    /// </summary>
    public class PSAnnotationsCollection : PSEntitiesCollection<PSAnnotation>
    {
        #region " Fields "

        #endregion

        #region " Constructors "

        /// <summary>
        /// This constructor is required as collection inherits from EntitiesCollection
        /// </summary>
        internal PSAnnotationsCollection(PSAutomation powerSHAPE) : base(powerSHAPE)
        {
        }

        #endregion

        #region " Properties "

        /// <summary>
        /// Gets the identifier used by PowerSHAPE when selecting an Annotation
        /// </summary>
        internal override string Identifier
        {
            get { return "TEXT"; }
        }

        #endregion

        #region " Operations "

        /// <summary>
        /// Creates a new annotation in PowwerSHAPE and adds it to the annotations collection.
        /// </summary>
        /// <param name="text">The text to introduce in PowerShape.</param>
        /// <param name="fontName">
        /// The font type for the text (e.g. Arial, Times New Roman). The font Type must exist in
        /// PowerShape.
        /// </param>
        /// <param name="height">The font height.</param>
        /// <param name="position">The position of the text in the active workplane.</param>
        /// <remarks>The created text will have pitch of 0, text origin will be 'CENTRE' and text justification will be 'LEFT'.</remarks>
        public PSAnnotation CreateAnnotation(string text, string fontName, double height, Point position)
        {
            PSAnnotation annotation = new PSAnnotation(_powerSHAPE, text, fontName, height, position);
            Add(annotation);
            return annotation;
        }

        /// <summary>
        /// Creates a new annotation in PowwerSHAPE.
        /// </summary>
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
        public PSAnnotation CreateAnnotation(
            string text,
            string fontName,
            double height,
            Point position,
            double pitch,
            TextJustifications textJustification,
            TextOrigins textOrigin)
        {
            PSAnnotation annotation =
                new PSAnnotation(_powerSHAPE, text, fontName, height, position, pitch, textJustification, textOrigin);
            Add(annotation);
            return annotation;
        }

        #endregion
    }
}