using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using System.ServiceProcess;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SPOT_Monitor_Service
{
    public partial class SPOTMonitorService : ServiceBase
    {
        private int eventId = 1;

        public enum ServiceState
        {
            SERVICE_STOPPED = 0x00000001,
            SERVICE_START_PENDING = 0x00000002,
            SERVICE_STOP_PENDING = 0x00000003,
            SERVICE_RUNNING = 0x00000004,
            SERVICE_CONTINUE_PENDING = 0x00000005,
            SERVICE_PAUSE_PENDING = 0x00000006,
            SERVICE_PAUSED = 0x00000007,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ServiceStatus
        {
            public int dwServiceType;
            public ServiceState dwCurrentState;
            public int dwControlsAccepted;
            public int dwWin32ExitCode;
            public int dwServiceSpecificExitCode;
            public int dwCheckPoint;
            public int dwWaitHint;
        };

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(System.IntPtr handle, ref ServiceStatus serviceStatus);

        public SPOTMonitorService()
        {
            InitializeComponent();
            eventLog1 = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists("SPOTMonitor"))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    "SPOTMonitor", "SPOTMonitorLog");
            }
            eventLog1.Source = "SPOTMonitor";
            eventLog1.Log = "SPOTMonitorLog";
        }

        protected override void OnStart(string[] args)
        {
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            eventLog1.WriteEntry("Der Dienst wurde gestartet.");
            Timer timer = new Timer();
            timer.Interval = 300000; // 300 seconds
            timer.Elapsed += new ElapsedEventHandler(this.OnTimer);
            timer.Start();

            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }

        public void OnTimer(object sender, ElapsedEventArgs args)
        {
            SQL sql = new SQL();
            string anzahl_offene_Messages = "--";
            string anzahl_Message_Gesamt = "--";
            string anzahl_Aktive_Personen = "--";
            try
            {
                anzahl_offene_Messages = sql.get_anzahl_messages_offen();
                anzahl_Message_Gesamt = sql.get_anzahl_messages_today();
                anzahl_Aktive_Personen = sql.get_Anzahl_Personen_aktiv();
            }
            catch
            { }

            // Logging
            string logging = sql.loggingToDB(System.Net.Dns.GetHostName(), anzahl_offene_Messages.ToString(), anzahl_Aktive_Personen, anzahl_Message_Gesamt);
            eventLog1.WriteEntry("Eintrag in der DB: " + logging, EventLogEntryType.Information, eventId++);
        }

        protected override void OnStop()
        {
            eventLog1.WriteEntry("Der Dienst wurde gestoppt.");
        }
    }
}