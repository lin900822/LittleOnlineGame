namespace Shared
{
    public enum MessageId : ushort
    {
        HeartBeat = 0,
    
        // 測試
        Echo      = 10,
        EchoAsync = 11,
        Move      = 12,
        Hello     = 13,
        RawByte   = 14,
        Broadcast = 15,
    
        // C2M
        Debug = 100,
        Register = 101,
        Login = 102,
        Match = 103,
        C2M_PlayerLoginOrRegister = 104,
    
        ClientMax = 32767,
    
        // M2C
        ServerInfo = 32768,
        M2C_StateCode = 32769,
        M2C_LoginSync = 32770,
    }
}

