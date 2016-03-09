using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
namespace Isotope
{
    public class GameTime
    {
        #region "Variables & Properties"

        //Store the total amount of game time
        public double TotalTime;
        //Store the total amount of time since the last update called Delta Time

        public float DeltaTime;
        #endregion
        #region "Initializers"

        public GameTime(TimeSpan TimeSpanTotal, TimeSpan TimeSpanElapsed)
        {
            TotalTime = TimeSpanTotal.TotalSeconds / 1;
            DeltaTime = (float)TimeSpanElapsed.TotalSeconds / 1;
        }

        #endregion
    }
}
