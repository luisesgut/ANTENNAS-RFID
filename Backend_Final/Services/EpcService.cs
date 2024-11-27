using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;


namespace Backend_Final.Services
{
    public class EpcService
    {
        private readonly List<string> _epcList = new List<string>();
        private readonly System.Timers.Timer _timer;


        public event Action<List<string>> OnBatchProcessed;

        public EpcService(double aggregationInterval)
        {
            _timer = new System.Timers.Timer(aggregationInterval);
            _timer.Elapsed += (sender, e) => ProcessBatch();
            _timer.AutoReset = false; // Solo dispara una vez por intervalo
        }

        public void AddEpc(string epc)
        {
            lock (_epcList)
            {
                if (!_epcList.Contains(epc)) // Evitar duplicados
                {
                    _epcList.Add(epc);
                }
            }
            // Reinicia el temporizador cada vez que se agrega un EPC
            _timer.Stop();
            _timer.Start();
        }

        private void ProcessBatch()
        {
            lock (_epcList)
            {
                if (_epcList.Count > 0)
                {
                    // Dispara el evento con los EPCs agrupados
                    OnBatchProcessed?.Invoke(new List<string>(_epcList));
                    _epcList.Clear(); // Limpia la lista después del procesamiento
                }
            }


        }
    }
}
