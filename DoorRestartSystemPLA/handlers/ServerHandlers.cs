using MEC;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;

namespace DoorRestartSystemPLA.handlers
{
    internal class ServerHandlers
    {
        private readonly DoorRestartSystem _plugin;
        public ServerHandlers(DoorRestartSystem plugin) => _plugin = plugin;
        private CoroutineHandle Coroutine;
        
        [PluginEvent(ServerEventType.RoundStart)]
        public void OnRoundStart()
        {
            Timing.KillCoroutines(Coroutine);

            var y = _plugin.Random.Next(100);
            if (y < _plugin.Config.Spawnchance)
            {
                Coroutine = Timing.RunCoroutine(_plugin.RunDoorTimer());
            }
        }
        [PluginEvent(ServerEventType.RoundEnd)]
        public void OnRoundEnd(RoundSummary.LeadingTeam LeadingTeam)
        {
            Timing.KillCoroutines(Coroutine);
        }
        [PluginEvent(ServerEventType.WaitingForPlayers)]
        public void OnWaitingForPlayers()
        {
            Timing.KillCoroutines(Coroutine);
        }
    }
}