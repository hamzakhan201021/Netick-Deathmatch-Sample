public class ClientNetworkProtocol
{
    public enum ClientNetworkProtocolOptions
    {
        None = 0,
        UDP = 1,
        Web_Socket = 2,
        Relay_UDP = 3,
        Relay_Web_Socket = 4,
        Auto = 5,
    }

    public const string Tooltip = "Client Protocol\nNone: No Protocol\nUDP: User Datagram Protocol\nWeb Socket: For WebGL and browsers\nRelay UDP: UDP + Unity Relay\nRelay Web Socket: WebSocket + Unity Relay\nAuto: Select Protocol based on platform";
}