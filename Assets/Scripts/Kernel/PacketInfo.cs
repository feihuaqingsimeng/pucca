using Common.Packet;

public class PacketInfo
{
    PACKET_BASE m_PacketBase;

    public PACKET_BASE packetBase
    {
        get
        {
            return m_PacketBase;
        }
    }

    bool m_Indication;

    public bool indication
    {
        get
        {
            return m_Indication;
        }
    }

    public PacketInfo(PACKET_BASE packetBase, bool indication)
    {
        m_PacketBase = packetBase;
        m_Indication = indication;
    }
}
