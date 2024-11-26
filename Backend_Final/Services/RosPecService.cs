using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.LLRP.LTK.LLRPV1;
using Org.LLRP.LTK.LLRPV1.DataType;
using System.Reflection.PortableExecutable;

namespace Backend_Final.Services
{
    public class RosPecService
    {
        private readonly RfidService _rfidService;

        public RosPecService(RfidService rfidService)
        {
            _rfidService = rfidService ?? throw new ArgumentNullException(nameof(rfidService));
        }


        public void Delete_RoSpec()
        {
            var reader = _rfidService.GetLLRPClient();

            MSG_DELETE_ROSPEC msg = new MSG_DELETE_ROSPEC
            {
                ROSpecID = 0 // ID 0 elimina todos los ROSpecs
            };
            MSG_ERROR_MESSAGE msgErr;

            MSG_DELETE_ROSPEC_RESPONSE rsp = reader.DELETE_ROSPEC(msg, out msgErr, 20000);

            if (rsp != null)
            {
                Console.WriteLine("ROSpec deleted successfully:");
                Console.WriteLine(rsp.ToString());

                // Verificar si la conexión sigue activa
                if (!reader.IsConnected)
                {
                    Console.WriteLine("Connection lost. Reconnecting...");
                    ReconnectLLRP();
                }
            }
            else if (msgErr != null)
            {
                Console.WriteLine("Error deleting ROSpec:");
                Console.WriteLine(msgErr.ToString());
                throw new Exception($"Error deleting ROSpec: {msgErr.ToString()}");
            }
            else
            {
                Console.WriteLine("Timeout while deleting ROSpec.");
                throw new TimeoutException("Timeout while deleting ROSpec.");
            }
        }

        private void ReconnectLLRP()
        {
            var reader = _rfidService.GetLLRPClient();
            ENUM_ConnectionAttemptStatusType status;
            bool isConnected = reader.Open("172.16.100.197", 2000, out status);

            if (!isConnected || status != ENUM_ConnectionAttemptStatusType.Success)
            {
                throw new Exception($"Failed to reconnect LLRP client: {status}");
            }

            Console.WriteLine("LLRP client reconnected successfully.");
        }

        //agregar rospec
        public void Add_RoSpec()
        {
            var reader = _rfidService.GetLLRPClient();
            MSG_ERROR_MESSAGE msgErr;
            MSG_ADD_ROSPEC msg = new MSG_ADD_ROSPEC();

            // Crear un nuevo ROSpec
            msg.ROSpec = new PARAM_ROSpec
            {
                // El ROSpec debe estar deshabilitado por defecto
                CurrentState = ENUM_ROSpecState.Disabled,
                ROSpecID = 123 // ID único para el ROSpec
            };

            // Especificar las condiciones de inicio y parada del ROSpec
            msg.ROSpec.ROBoundarySpec = new PARAM_ROBoundarySpec
            {
                ROSpecStartTrigger = new PARAM_ROSpecStartTrigger
                {
                    ROSpecStartTriggerType = ENUM_ROSpecStartTriggerType.Immediate // Inicio inmediato
                },
                ROSpecStopTrigger = new PARAM_ROSpecStopTrigger
                {
                    ROSpecStopTriggerType = ENUM_ROSpecStopTriggerType.Null // Sin trigger de parada
                }
            };
            // Especificar qué antenas usar y con qué protocolo
            PARAM_AISpec aiSpec = new PARAM_AISpec
            {
                AntennaIDs = new UInt16Array(), // Crear el arreglo de antenas
                AISpecStopTrigger = new PARAM_AISpecStopTrigger
                {
                    AISpecStopTriggerType = ENUM_AISpecStopTriggerType.Null // Sin trigger de parada
                },
                InventoryParameterSpec = new PARAM_InventoryParameterSpec[1]
    {
        new PARAM_InventoryParameterSpec
        {
            InventoryParameterSpecID = 1234, // ID único
            ProtocolID = ENUM_AirProtocols.EPCGlobalClass1Gen2 // Protocolo RFID
        }
    }
            };

            // Agregar IDs de antenas (1 al 13)
            for (ushort i = 1; i <= 13; i++)
            {
                aiSpec.AntennaIDs.Add(i);
            }


            msg.ROSpec.SpecParameter = new UNION_SpecParameter();
            msg.ROSpec.SpecParameter.Add(aiSpec);

            // Configurar el reporte de etiquetas
            msg.ROSpec.ROReportSpec = new PARAM_ROReportSpec
            {
                ROReportTrigger = ENUM_ROReportTriggerType.Upon_N_Tags_Or_End_Of_ROSpec, // Reportar por etiqueta
                N = 1,
                TagReportContentSelector = new PARAM_TagReportContentSelector()
            };

            // Enviar el mensaje al lector
            MSG_ADD_ROSPEC_RESPONSE rsp = reader.ADD_ROSPEC(msg, out msgErr, 10000);

            if (rsp != null)
            {
                Console.WriteLine("ROSpec added successfully:");
                Console.WriteLine(rsp.ToString());
            }
            else if (msgErr != null)
            {
                Console.WriteLine("Error adding ROSpec:");
                Console.WriteLine(msgErr.ToString());
                throw new Exception($"Error adding ROSpec: {msgErr.ToString()}");
            }
            else
            {
                Console.WriteLine("Timeout while adding ROSpec.");
                throw new TimeoutException("Timeout while adding ROSpec.");
            }
        }

        public void Enable_RoSpec()
        {
            var reader = _rfidService.GetLLRPClient();
            MSG_ERROR_MESSAGE msgErr;
            MSG_ENABLE_ROSPEC msg = new MSG_ENABLE_ROSPEC
            {
                ROSpecID = 123 // Debe coincidir con el ID del ROSpec configurado
            };

            // Enviar el mensaje al lector
            MSG_ENABLE_ROSPEC_RESPONSE rsp = reader.ENABLE_ROSPEC(msg, out msgErr, 10000);

            if (rsp != null)
            {
                Console.WriteLine("ROSpec enabled successfully:");
                Console.WriteLine(rsp.ToString());
            }
            else if (msgErr != null)
            {
                Console.WriteLine("Error enabling ROSpec:");
                Console.WriteLine(msgErr.ToString());
                throw new Exception($"Error enabling ROSpec: {msgErr.ToString()}");
            }
            else
            {
                Console.WriteLine("Timeout while enabling ROSpec.");
                throw new TimeoutException("Timeout while enabling ROSpec.");
            }
        }
        public void Get_RoSpecs()
        {
            var reader = _rfidService.GetLLRPClient();
            // Crear el mensaje GET_ROSPECS
            MSG_GET_ROSPECS msg = new MSG_GET_ROSPECS();
            MSG_ERROR_MESSAGE msgErr;

            // Enviar el mensaje al lector
            MSG_GET_ROSPECS_RESPONSE rsp = reader.GET_ROSPECS(msg, out msgErr, 5000); // Tiempo de espera 5000 ms

            if (rsp != null && rsp.ROSpec != null)
            {
                Console.WriteLine("ROSpecs found on the reader:");

                // Iterar sobre los ROSpecs
                foreach (var rospec in rsp.ROSpec)
                {
                    Console.WriteLine($"ROSpec ID: {rospec.ROSpecID}, State: {rospec.CurrentState}");
                }
            }
            else if (msgErr != null)
            {
                Console.WriteLine("Error retrieving ROSpecs:");
                Console.WriteLine(msgErr.ToString());
            }
            else
            {
                Console.WriteLine("Timeout while retrieving ROSpecs.");
            }
        }
    }
}
