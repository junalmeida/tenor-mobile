//
// Copyright (c) Microsoft Corporation.  All rights reserved.
//
//
// Use of this sample source code is subject to the terms of the Microsoft
// license agreement under which you licensed this sample source code. If
// you did not accept the terms of the license agreement, you are not
// authorized to use this sample source code. For the terms of the license,
// please see the license agreement between you and Microsoft or, if applicable,
// see the LICENSE.RTF on your install media or the root of your tools installation.
// THE SAMPLE SOURCE CODE IS PROVIDED "AS IS", WITH NO WARRANTIES OR INDEMNITIES.
//
#region Using directives

using System;

#endregion

namespace Tenor.Mobile.Location.Gps
{
    /// <summary>
    /// Event args used for LocationChanged events.
    /// </summary>
    internal class LocationChangedEventArgs: EventArgs
    {
        public LocationChangedEventArgs(GpsPosition position)
        {
            this.position = position;
        }

        /// <summary>
        /// Gets the new position when the GPS reports a new position.
        /// </summary>
        public GpsPosition Position
        {
            get 
            {
                return position;
            }
        }

        private GpsPosition position;

    }
}
