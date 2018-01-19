// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using System.Diagnostics;
using Microsoft.VisualBasic.ApplicationServices;
using Microsoft.VisualBasic.Devices;

// This file was created by the VB to C# converter (SharpDevelop 4.4.2.9749).
// It contains classes for supporting the VB "My" namespace in C#.
// If the VB application does not use the "My" namespace, or if you removed the usage
// after the conversion to C#, you can delete this file.

namespace Autodesk.WPFControls.My
{
    sealed class MyProject
    {
        [ThreadStatic] static MyApplication application;

        public static MyApplication Application
        {
            [DebuggerStepThrough]
            get
            {
                if (application == null)
                {
                    application = new MyApplication();
                }
                return application;
            }
        }

        [ThreadStatic] static MyComputer computer;

        public static MyComputer Computer
        {
            [DebuggerStepThrough]
            get
            {
                if (computer == null)
                {
                    computer = new MyComputer();
                }
                return computer;
            }
        }

        [ThreadStatic] static User user;

        public static User User
        {
            [DebuggerStepThrough]
            get
            {
                if (user == null)
                {
                    user = new User();
                }
                return user;
            }
        }
    }

    class MyApplication : ApplicationBase
    {
    }

    class MyComputer : Computer
    {
    }
}