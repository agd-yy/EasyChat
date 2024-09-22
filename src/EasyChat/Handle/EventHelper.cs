using EasyChat.Utilities;

namespace EasyChat.Handle
{
    public class EventHelper : Singleton<EventHelper>
    {
        public event EventHandler? StartBlinkEvent;
        public event EventHandler? StopBlinkEvent;

        public void StartBlink()
        {
            StartBlinkEvent?.Invoke(this, EventArgs.Empty);
        }

        public void StopBlink()
        {
            StopBlinkEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}
