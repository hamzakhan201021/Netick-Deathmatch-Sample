using System;

public class ServerNetworkProtocol
{
    [Flags]
    public enum ServerNetworkProtocolOptions
    {
        None = 0,
        UDP = 1 << 0,
        Wed_Socket = 1 << 2,
        Relay_UDP = 1 << 3,
        Relay_Web_Socket = 1 << 4,
    }

    public const string Tooltip = "Server Protocols\nNone: No Protocol\nUDP: User Datagram Protocol\nWeb Socket: For WebGL and browsers\nRelay UDP: UDP + Unity Relay\nRelay Web Socket: WebSocket + Unity Relay";
}