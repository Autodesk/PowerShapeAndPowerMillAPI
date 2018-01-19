// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

namespace Autodesk.ProductInterface.PowerMILL
{
    public enum DrillCycleTypes
    {
        /// <summary>
        /// Drills the holes in several stages (multiple peck) only retracting a small amount after each peck.
        /// </summary>
        BreakChip,

        /// <summary>
        /// The second of the boring cycles (G86).
        /// </summary>
        CounterBore,

        /// <summary>
        /// Drills the holes in several stages (multiple peck).
        /// </summary>
        DeepDrill,

        /// <summary>
        /// Creates an external thread on a boss.
        /// </summary>
        ExternalThread,

        /// <summary>
        /// An alternative deep drilling cycle, so may be used for machines with a variety of deep drilling cycles.
        /// </summary>
        FineBoring,

        /// <summary>
        /// Bores out a large hole with a small tool. The concept is similar to trochoidal milling.
        /// The difference between the two is that trochoidal milling machines a slot (no variation in Z)
        /// and helical milling drills a hole (machines down the Z axis).
        /// </summary>
        Helical,

        /// <summary>
        /// Drills the hole using circular moves. A tool smaller than the size of the hole is required. At each depth,
        /// the tool moves on to the edge of the hole using a circular arc lead with a straight extension, cuts a circle,
        /// and then moves back to the centre with a circular arc lead and straight extension.
        /// You may use a thickness value to create a smaller hole.
        /// </summary>
        Profile,

        /// <summary>
        /// The first of the boring cycles (G85).
        /// </summary>
        Ream,

        /// <summary>
        /// Enables you to enter a Peck depth as well as a Pitch.
        /// </summary>
        RigidTapping,

        /// <summary>
        /// Drills the holes in one operation.
        /// </summary>
        SinglePeck,

        /// <summary>
        /// Drills the holes in one direction and reverses out.
        /// </summary>
        Tap,

        /// <summary>
        /// A cycle similar to reverse helical but with improved leads in and out for thread creation.
        /// </summary>
        ThreadMill,

        /// <summary>
        /// These correspond to the third, fourth, and fifth type of boring cycle (G87-G89) normally found on a machine tool.
        /// The difference between the boring operations varies from machine tool to machine tool.
        /// </summary>
        Bore,

        /// <summary>
        /// A second helical drill cycle. This bores out a large hole with a small tool.
        /// It performs a helical movement from the bottom of the hole.
        /// Use this for finishing a hole as you can use it only with a pre-drilled hole.
        /// </summary>
        ReverseHelical,

        HelicalClockwise,

        ReverseHelicalClockwise,

        ProfileClockwise,

        TapClockwise
    }
}