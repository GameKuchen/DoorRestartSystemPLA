using System.Collections.Generic;
using System.Linq;
using DoorRestartSystemPLA.handlers;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using Interactables.Interobjects.DoorUtils;
using MEC;
using UnityEngine;
using Random = System.Random;
using Log = PluginAPI.Core.Log;

namespace DoorRestartSystemPLA
{
    public class DoorRestartSystem
    {
        private static DoorRestartSystem Instance { get; set; }
        
        public Random Random { get; } = new Random();
        private static bool _timerOn = true;
        [PluginPriority(LoadPriority.High)]
        [PluginEntryPoint("DoorRestartSystem", "1.0.0", "Event for doors.", "GameKuchen")]
        void LoadPlugin()
        {
            Log.Info("DoorRestartSystem Starting...");
            Log.Info("Spilling Coffee on Door System...");
            Instance = this;
            EventManager.RegisterEvents(this, new ServerHandlers(this));
            
            var handler = PluginHandler.Get(this);
            handler.SaveConfig(this, nameof(Config));
        }
        public static void SoftLockDoors()
        {
            foreach (var door in Facility.Doors)
            {
                door.Lock(DoorLockReason.Warhead, true);
            }
        }

        public IEnumerator<float> RunDoorTimer()
        {
            yield return Timing.WaitForSeconds(Config.InitialDelay);
            yield return Timing.WaitForSeconds((float) Random.NextDouble() * (Config.DelayMax - Config.DelayMin) + Config.DelayMin);
            for (;;)
            {
                yield return Timing.WaitUntilTrue(() => !(Warhead.IsDetonated || Warhead.IsDetonationInProgress));
                Cassie.Message(Config.DoorSentence);

                _timerOn = true;
                yield return Timing.WaitForSeconds(Config.TimeBetweenSentenceAndStart);
                if (Config.Countdown)
                {
                    Cassie.Message("3 . 2 . 1");
                    yield return Timing.WaitForSeconds(3f);
                }

                var doorOutDur = (float) (Random.NextDouble() * (Config.DurationMax - Config.DurationMin) + Config.DurationMin);
                foreach (var door in DoorVariant.AllDoors.Where(door => !IsNuke(door)))
                {
                    if (Config.CloseDoors)
                    {
                        Log.Debug("Closing Door");
                        door.NetworkTargetState = false;
                    }
                    door.ServerChangeLock(DoorLockReason.SpecialDoorFeature, true);
                }
                yield return Timing.WaitForSeconds(doorOutDur);
                foreach (var door in DoorVariant.AllDoors.Where(door => !IsNuke(door)))
                {
                    door.ServerChangeLock(DoorLockReason.SpecialDoorFeature, false);
                }
                Cassie.Message(Config.DoorAfterSentence);
                yield return Timing.WaitForSeconds((float)Random.NextDouble() * (Config.DelayMax - Config.DelayMin) + Config.DelayMin);
                _timerOn = false;
                Log.Debug("Done");
            }
        }

        private static bool IsNuke(DoorVariant doorVariant)
        {
            var nameTag = doorVariant.TryGetComponent(out DoorNametagExtension name) ? name.GetName : null;
            if (nameTag == null) return false;
            var bracketStart = nameTag.IndexOf('(') - 1;
            if (bracketStart > 0)
                nameTag = nameTag.Remove(bracketStart, nameTag.Length - bracketStart);
            return nameTag == "SURFACE_NUKE";
        }
        [PluginConfig]
        public Config Config;
    }
}