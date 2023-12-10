using System;
using System.Runtime.InteropServices;
using GameEvent;

namespace TSEspionage
{
    public class GameEventHandler
    {
        public void HandleEvent(ref IntPtr eventBuffer)
        {
            var eventType = (EventType)Marshal.ReadIntPtr(eventBuffer).ToInt32();
            var eventPointer = eventBuffer + Marshal.SizeOf(typeof(int));

            switch (eventType)
            {
                case EventType.GameOver:
                    var gameOver = Marshal.PtrToStructure<GameOver>(eventPointer);
                    break;
            }
        }
    }
}
