using EasyChat.Utilities;

namespace EasyChat.Handle
{
    public class EventHelper : Singleton<EventHelper>
    {
        public event Action? StartBlinkEvent;
        public event Action? StopBlinkEvent;
        public event Action? ClearNewMessage;

        public void StartBlink()
        {
            StartBlinkEvent?.Invoke();
        }

        public void StopBlink()
        {
            StopBlinkEvent?.Invoke();
        }

        public void ClearNewMessageCount()
        {
            ClearNewMessage?.Invoke();
        }
    }
}
