using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAAgent.Core
{
    public delegate void ExitedEventHandler(object sender, EventArgs e);
    public class ProcessStartEventProcessor : EventProcessor
    {
        private Process _Process;

        
        public event ExitedEventHandler Exited;

        public override void DoProcess(EventData eventData)
        {
            if (_Process == null)
            {
                string fileName = eventData.Data["fileName"].ToObject<string>();
                _Process = Process.Start(fileName);

                // Exited Event Enabled
                _Process.EnableRaisingEvents = true;
                _Process.Exited += Process_Exited;
            }
            else
            {

            }
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            Exited?.Invoke(sender, e);
            _Process.Dispose();
            _Process = null;
        }
    }
}
