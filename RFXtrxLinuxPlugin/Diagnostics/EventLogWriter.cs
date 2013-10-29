using System.Diagnostics;

namespace SwitchKing.Server.Plugins.RFXtrx.Diagnostics
{
    public static class EventLogWriter
    {
        private const string EVENT_LOG_SOURCE = "SwitchKingRFXtrxPlugin";

        public static void WriteToLog(EventLogEntryType type, string msg)
        {
            try
            {
                if (!EventLog.SourceExists(EVENT_LOG_SOURCE))
                    EventLog.CreateEventSource(EVENT_LOG_SOURCE, "Application");

                System.Diagnostics.EventLog.WriteEntry(
                    EVENT_LOG_SOURCE,
                    msg,
                    type);
            }
            catch
            {
            }
        }
    }
}
