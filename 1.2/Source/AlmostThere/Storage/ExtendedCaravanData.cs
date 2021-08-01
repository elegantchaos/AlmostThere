using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace AlmostThere.Storage
{
    public enum CacheState {
        NotCalculated,
        NotResting,
        Resting
    }

    public enum RestMode
    {
        DontRest,
        AlmostThere,
        ForceRest,
    }

    public class ExtendedCaravanData : IExposable
    {
        public CacheState cache = CacheState.NotCalculated;
        public RestMode mode = RestMode.AlmostThere;
        public void ExposeData()
        {
            Scribe_Values.Look(ref mode, "mode", RestMode.AlmostThere);
        }
    }
}
