using Org.LLRP.LTK.LLRPV1;
using Org.LLRP.LTK.LLRPV1.DataType;
using Org.LLRP.LTK.LLRPV1.Impinj;

public class RfidService
{
    private static LLRPClient reader = new LLRPClient(); // Instancia compartida
    private readonly string _hostname = "172.16.100.197"; // IP fija del lector

    public void ConnectLLRP()
    {
        if (!reader.IsConnected)
        {
            ENUM_ConnectionAttemptStatusType status;
            bool isConnected = reader.Open(_hostname, 2000, out status);

            if (!isConnected || status != ENUM_ConnectionAttemptStatusType.Success)
            {
                throw new Exception($"Failed to connect LLRP client: {status}");
            }

            Console.WriteLine("LLRP client connected successfully.");
            ConfigureKeepalives(); // Configurar los mensajes KEEPALIVE

        }
    }
    // se agregan keep alives
    public void ConfigureKeepalives()
    {
        var reader = GetLLRPClient();

        // Habilitar extensiones Impinj antes de configurar parámetros personalizados
        EnableImpinjExtensions();

        // Configurar KeepaliveSpec para enviar mensajes cada 10 segundos
        var keepaliveSpec = new PARAM_KeepaliveSpec
        {
            KeepaliveTriggerType = ENUM_KeepaliveTriggerType.Periodic,
            PeriodicTriggerValue = 10000 // Cada 10 segundos (recomendación del manual)
        };

        // Configurar ImpinjLinkMonitorConfiguration para la tolerancia de KEEPALIVE_ACK
        var impinjLinkMonitorConfig = new PARAM_ImpinjLinkMonitorConfiguration
        {
            LinkDownThreshold = 3 // Permitir hasta 3 mensajes faltantes
        };

        // Crear configuración para el lector
        var config = new MSG_SET_READER_CONFIG
        {
            KeepaliveSpec = keepaliveSpec,
            ResetToFactoryDefault = false,
            //Custom = new UNION_Custom()
        };

        // Agregar configuración personalizada de Impinj
        config.Custom.Add(impinjLinkMonitorConfig);

        // Aplicar configuración al lector
        MSG_ERROR_MESSAGE error;
        var response = reader.SET_READER_CONFIG(config, out error, 5000);

        if (response?.LLRPStatus.StatusCode != ENUM_StatusCode.M_Success)
        {
            throw new Exception($"Error configurando Keepalives: {response.LLRPStatus.ErrorDescription}");
        }

        Console.WriteLine("Keepalives y tolerancia configurados correctamente.");
    }

    public void EnableImpinjExtensions()
    {
        var reader = GetLLRPClient();

        // Crear el mensaje para habilitar las extensiones Impinj
        MSG_IMPINJ_ENABLE_EXTENSIONS enableExtensions = new MSG_IMPINJ_ENABLE_EXTENSIONS();
        //enableExtensions.MessageID = 1; // Asignar un ID único al mensaje

        // Enviar el mensaje al lector
        MSG_ERROR_MESSAGE errorMessage;
        var response = reader.CUSTOM_MESSAGE(enableExtensions, out errorMessage, 5000);

        // Validar la respuesta
        if (response is MSG_IMPINJ_ENABLE_EXTENSIONS_RESPONSE enableResponse &&
            enableResponse.LLRPStatus.StatusCode == ENUM_StatusCode.M_Success)
        {
            Console.WriteLine("Extensiones Impinj habilitadas correctamente.");
        }
        else
        {
            throw new Exception($"Error habilitando extensiones Impinj: {errorMessage?.ToString() ?? "Unknown error"}");
        }
    }



    public LLRPClient GetLLRPClient()
    {
        // Asegúrate de que la conexión esté activa antes de devolverla
        if (!reader.IsConnected)
        {
            ConnectLLRP();
        }

        return reader;
    }
    public void ConfigureAntennas()
    {
        var reader = GetLLRPClient();

        // Verifica que el lector esté conectado
        if (reader == null || !reader.IsConnected)
        {
            throw new Exception("LLRPClient is not connected. Please connect before configuring antennas.");
        }

        // Crear mensaje de configuración
        MSG_SET_READER_CONFIG config = new MSG_SET_READER_CONFIG
        {
            ResetToFactoryDefault = false
        };

        // Configurar antenas
        PARAM_AntennaConfiguration[] antennaConfigurations = new PARAM_AntennaConfiguration[13];
        for (ushort i = 1; i <= 13; i++)
        {
            antennaConfigurations[i - 1] = new PARAM_AntennaConfiguration
            {
                AntennaID = i,
                RFReceiver = new PARAM_RFReceiver
                {
                    ReceiverSensitivity = 25 // Índice para -47 dBm
                },
                RFTransmitter = new PARAM_RFTransmitter
                {
                    TransmitPower = 73,    // Índice para 28 dBm
                    HopTableID = 1,
                    ChannelIndex = 1
                }
            };
        }

        config.AntennaConfiguration = antennaConfigurations;

        // Validar antes de enviar
        if (config.AntennaConfiguration == null || config.AntennaConfiguration.Length == 0)
        {
            throw new Exception("AntennaConfiguration is null or empty. Please configure antennas properly.");
        }

        // Enviar configuración
        MSG_ERROR_MESSAGE error;
        MSG_SET_READER_CONFIG_RESPONSE response = reader.SET_READER_CONFIG(config, out error, 12000);

        if (response != null && response.LLRPStatus.StatusCode == ENUM_StatusCode.M_Success)
        {
            Console.WriteLine("Antennas configured successfully.");
        }
        else
        {
            string errorMsg = error?.ToString() ?? "Unknown error.";
            throw new Exception($"Error configuring antennas: {errorMsg}");
        }
    }


    public void DisconnectLLRP()
    {
        reader.Close();
        Console.WriteLine("LLRP client disconnected.");
    }
}
