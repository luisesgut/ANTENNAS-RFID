using Org.LLRP.LTK.LLRPV1;

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

    public void DisconnectLLRP()
    {
        reader.Close();
        Console.WriteLine("LLRP client disconnected.");
    }
}
