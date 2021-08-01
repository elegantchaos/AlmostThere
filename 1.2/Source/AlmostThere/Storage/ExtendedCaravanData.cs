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

    public class ExtendedCaravanData : IExposable
    {
        public CacheState cache = CacheState.NotCalculated;
        public bool fullyIgnoreRest = false;
        public bool forceRest = false;
        public bool almostThere = true;
        public void ExposeData()
        {
            Scribe_Values.Look(ref almostThere, "almostThere", false);            
            Scribe_Values.Look(ref fullyIgnoreRest, "fullyIgnoreRest", false);            
            Scribe_Values.Look(ref forceRest, "forceRest", false);            
        }
    }
}
