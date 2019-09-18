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

namespace Autodesk.ProductInterface.PowerMILL
{
    /// <summary>
    /// Collection of Workpath objects in the Active PowerMILL Project
    /// </summary>
    public class PMWorkplanesCollection : PMEntitiesCollection<PMWorkplane>
    {
        #region Constructors

        /// <summary>
        /// Initialises the item.
        /// </summary>
        /// <param name="powerMILL">The base instance to interact with PowerMILL.</param>
        internal PMWorkplanesCollection(PMAutomation powerMILL) : base(powerMILL)
        {
            Initialise();
        }

        #endregion

        #region Operations

        /// <summary>
        /// Initialises the collection with the items in PowerMILL.
        /// </summary>
        internal void Initialise()
        {
            foreach (string name in ReadWorkplanes())
            {
                Add(new PMWorkplane(_powerMILL, name));
            }
        }

        /// <summary>
        /// Gets the list of names of all Workplanes in PowerMILL.
        /// </summary>
        internal List<string> ReadWorkplanes()
        {
            List<string> names = new List<string>();
            foreach (var workplane in _powerMILL.PowerMILLProject.Workplanes)
            {
                names.Add(workplane.Name);
            }
            return names;
        }

        /// <summary>
        /// Creates a Workplane in PowerMILL based on the specified workplane.
        /// </summary>
        /// <param name="workplaneToCreate">The workplane to create in PowerMILL.</param>
        public PMWorkplane CreateWorkplane(Geometry.Workplane workplaneToCreate)
        {
            PMWorkplane newWorkplane = null;
            if (workplaneToCreate != null)
            {
                Geometry.Point xPoint = workplaneToCreate.Origin + workplaneToCreate.XAxis;
                Geometry.Point yPoint = workplaneToCreate.Origin + workplaneToCreate.YAxis;
                _powerMILL.DoCommand("MODE WORKPLANE_CREATE ; INTERACTIVE THREE_POINTS",
                                     "MODE WORKPLANE_CREATE THREE_POINTS DEFINE ORIGIN",
                                     "MODE WORKPLANE_CREATE THREE_POINTS COORD X \"" + workplaneToCreate.Origin.X + "\"",
                                     "MODE WORKPLANE_CREATE THREE_POINTS COORD Y \"" + workplaneToCreate.Origin.Y + "\"",
                                     "MODE WORKPLANE_CREATE THREE_POINTS COORD Z \"" + workplaneToCreate.Origin.Z + "\"",
                                     "MODE WORKPLANE_CREATE THREE_POINTS DEFINE X_AXIS",
                                     "MODE WORKPLANE_CREATE THREE_POINTS COORD X \"" + xPoint.X + "\"",
                                     "MODE WORKPLANE_CREATE THREE_POINTS COORD Y \"" + xPoint.Y + "\"",
                                     "MODE WORKPLANE_CREATE THREE_POINTS COORD Z \"" + xPoint.Z + "\"",
                                     "MODE WORKPLANE_CREATE THREE_POINTS DEFINE XY_PLANE",
                                     "MODE WORKPLANE_CREATE THREE_POINTS COORD X \"" + yPoint.X + "\"",
                                     "MODE WORKPLANE_CREATE THREE_POINTS COORD Y \"" + yPoint.Y + "\"",
                                     "MODE WORKPLANE_CREATE THREE_POINTS COORD Z \"" + yPoint.Z + "\"",
                                     "WPFROM3PTS ACCEPT");

                // Get the name PowerMILL gave to the last workplane
                //Initialise()
                Type type = typeof(PMWorkplane);
                List<PMEntity> wplist = _powerMILL.ActiveProject.CreatedItems(type);
                if (wplist == null || wplist.Count < 1)
                {
                    newWorkplane = null;
                }
                else
                {
                    foreach (PMWorkplane wp in wplist)
                    {
                        if (wp != null)
                        {
                            Add(wp);
                        }
                    }
                    newWorkplane = LastItem();
                }
            }
            return newWorkplane;
        }
        #endregion
    }
}