using System;

[Serializable]
public class NetworkConnectionData
{
    private const string DEFAULT_ADRESS = "127.0.0.1";
    private const int DEFAULT_PORT = 27000;

    public string connectAddress = default;
    public int connectPort = default;

    public NetworkConnectionData()
    {
        this.connectAddress = DEFAULT_ADRESS;
        this.connectPort = DEFAULT_PORT;
    }

    public NetworkConnectionData(string connectAddress, int connectPort)
    {
        this.connectAddress = connectAddress;
        this.connectPort = connectPort;
    }
}
