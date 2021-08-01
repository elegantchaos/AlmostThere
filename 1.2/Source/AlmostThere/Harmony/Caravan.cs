using AlmostThere;
using AlmostThere.Storage;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AlmostThere.Harmony
{
    [HarmonyPatch(typeof(Caravan), "get_NightResting")]
    class Caravan_get_NightResting
    {
        static void Postfix(Caravan __instance, ref bool __result)
        {
            if(Base.Instance.GetExtendedDataStorage() is ExtendedDataStorage store)
            {
                if (store.GetExtendedDataFor(__instance) is ExtendedCaravanData data)
                {
                    // return cached value if we have one, unless it's time to recalculate
                    if ((data.cache != CacheState.NotCalculated) && !__instance.IsHashIntervalTick(60))
                    {
                        __result = data.cache == CacheState.Resting;
                        return;
                    }

                    switch (data.mode) {
                        case RestMode.ForceRest:
                            __result = true;
                            break;

                        case RestMode.DontRest:
                            __result = false;
                            break;

                        case RestMode.AlmostThere:
                            if (__result) {
                                var estimatedTicks = (float)CaravanArrivalTimeEstimator.EstimatedTicksToArrive(__instance, allowCaching: true);
                                var restTicksLeft = CaravanNightRestUtility.LeftRestTicksAt(__instance.Tile, Find.TickManager.TicksAbs);
                                estimatedTicks -= restTicksLeft;
                                if (estimatedTicks / GenDate.TicksPerHour < Base.almostThereHours.Value)
                                {
                                    __result = false;
                                }
                            }
                            break;
                    }

                    data.cache = __result ? CacheState.Resting : CacheState.NotResting;
                }
            }
        }
    }


    [HarmonyPatch(typeof(Caravan), "GetGizmos")]
    class Caravan_GetGizmos
    {
        static void Postfix(Caravan __instance, ref IEnumerable<Gizmo> __result)
        {
            if(__instance.Faction == Faction.OfPlayer)
            {
                var gizmoList = __result.ToList();
                var store = Base.Instance.GetExtendedDataStorage();
                var caravanData = store.GetExtendedDataFor(__instance);
                if (caravanData != null)
                {
                    gizmoList.Add(CreateAlmostThereCommand(__instance, caravanData));
                    gizmoList.Add(CreateIgnoreRestCommand(__instance, caravanData));
                    gizmoList.Add(CreateForceRestCommand(__instance, caravanData));
                }
                __result = gizmoList;
            }
        }

        private static Command_Toggle CreateIgnoreRestCommand(Caravan __instance, ExtendedCaravanData caravanData)
        {
            Command_Toggle command_Toggle = new Command_Toggle();
            command_Toggle.isActive = (() => caravanData.mode == RestMode.DontRest);
            command_Toggle.toggleAction = delegate
            {
                caravanData.mode = RestMode.DontRest;
            };
            command_Toggle.defaultDesc = "AT_Command_DontRest_Description".Translate();
            command_Toggle.icon = ContentFinder<Texture2D>.Get(("UI/" + "DontRest"), true);
            command_Toggle.defaultLabel = "AT_Command_DontRest_Label".Translate();
            return command_Toggle;
        }
        private static Command_Toggle CreateAlmostThereCommand(Caravan __instance, ExtendedCaravanData caravanData)
        {
            Command_Toggle command_Toggle = new Command_Toggle();
            command_Toggle.isActive = (() => caravanData.mode == RestMode.AlmostThere);
            command_Toggle.toggleAction = delegate
            {
                caravanData.mode = RestMode.AlmostThere;
            };
            command_Toggle.defaultDesc = "AT_Command_AlmostThere_Description".Translate(Base.almostThereHours.Value);
            command_Toggle.icon = ContentFinder<Texture2D>.Get(("UI/" + "AlmostThere"), true);
            command_Toggle.defaultLabel = "AT_Command_AlmostThere_Label".Translate();
            return command_Toggle;
        }

        private static Command_Toggle CreateForceRestCommand(Caravan __instance, ExtendedCaravanData caravanData)
        {
            Command_Toggle command_Toggle = new Command_Toggle();
            command_Toggle.isActive = (() => caravanData.mode == RestMode.ForceRest);
            command_Toggle.toggleAction = delegate
            {
                caravanData.mode = RestMode.ForceRest;
            };
            command_Toggle.defaultDesc = "AT_Command_ForceRest_Description".Translate();
            command_Toggle.icon = ContentFinder<Texture2D>.Get(("UI/" + "ForceRest"), true);
            command_Toggle.defaultLabel = "AT_Command_ForceRest_Title".Translate();
            return command_Toggle;
        }
    }

}
