using MEC;

namespace DoorRestartSystemPLA.handlers
{
    internal sealed class Server
    {
        private readonly DoorRestartSystem _plugin;
        public Server(DoorRestartSystem plugin) => _plugin = plugin;
        public CoroutineHandle Coroutine;

        public void OnRoundStart()
        {
            Timing.KillCoroutines(Coroutine);
            
            var y = _plugin
        }
    }
}