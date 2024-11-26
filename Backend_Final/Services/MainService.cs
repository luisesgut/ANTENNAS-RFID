﻿using Org.LLRP.LTK.LLRPV1;
using Org.LLRP.LTK.LLRPV1.DataType;
using Org.LLRP.LTK.LLRPV1.Impinj;

namespace Backend_Final.Services
{
    public class MainService
    {
        private readonly RfidService _rfidService;
        private readonly RosPecService _rosPecService;

        public MainService(RfidService rfidService, RosPecService rosPecService)
        {
            _rfidService = rfidService;
            _rosPecService = rosPecService;
        }

        public void Start()
        {
            try
            {
                // Paso 1: Conectar al lector
                _rfidService.ConnectLLRP();

                // Paso 2: Configurar evento para recibir reportes de etiquetas
                var reader = _rfidService.GetLLRPClient();
                reader.OnRoAccessReportReceived += new delegateRoAccessReport(OnReportEvent);

                // Paso 3: Eliminar ROSpecs existentes
                _rosPecService.Delete_RoSpec();

                // Paso 4: Agregar y habilitar un nuevo ROSpec
                _rosPecService.Add_RoSpec();
                _rosPecService.Enable_RoSpec();

                Console.WriteLine("Lector configurado y listo para leer etiquetas.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en el proceso: {ex.Message}");
                throw;
            }
        }

        public void Stop()
        {
            try
            {
                // Paso 5: Eliminar el ROSpec activo
                _rosPecService.Delete_RoSpec();

                // Paso 6: Desconectar del lector
                _rfidService.DisconnectLLRP();

                Console.WriteLine("Proceso detenido y lector desconectado.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al detener el proceso: {ex.Message}");
                throw;
            }
        }

        // Evento para manejar reportes de etiquetas
        private void OnReportEvent(MSG_RO_ACCESS_REPORT msg)
        {
            if (msg?.TagReportData == null || msg.TagReportData.Length == 0)
            {
                Console.WriteLine("No se detectaron etiquetas en el reporte.");
                return;
            }

            // Iterar sobre las etiquetas en el reporte
            foreach (var tagData in msg.TagReportData)
            {
                if (tagData.EPCParameter.Count > 0)
                {
                    string epc;

                    // Verificar el tipo de EPC (96-bit o 128-bit)
                    if (tagData.EPCParameter[0] is PARAM_EPC_96 epc96)
                    {
                        epc = epc96.EPC.ToHexString();
                    }
                    else if (tagData.EPCParameter[0] is PARAM_EPCData epcData)
                    {
                        epc = epcData.EPC.ToHexString();
                    }
                    else
                    {
                        epc = "Formato de EPC desconocido";
                    }

                    Console.WriteLine($"Etiqueta detectada: EPC = {epc}");
                }
            }
        }
    
}
}