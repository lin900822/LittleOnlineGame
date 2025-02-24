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
    
        // Client To Main
        Debug = 100,
        Register = 101,
        Login = 102,
        Match = 103,
        C2M_PlayerLoginOrRegister = 104,
        C2M_JoinMatchQueue = 105,
        C2M_CancelJoinMatchQueue = 106,
    
        // Main To Client
        M2C_StateCode = 16384,
        M2C_LoginSync = 16385,
        M2C_RoomMatched = 16386,
        
        // Client To Battle
        C2B_JoinRoom = 20001,
        C2B_Input = 20002,
        
        // Battle To Client
        B2C_BattleStart = 21001,
        B2C_SyncState = 21002,

        ClientMax = 32767,
        
#if !UNITY_CLIENT
        // Server 內部
        ServerInfo = 32768,
        
        // Main To Battle
        M2B_HandShake = 33000,
        M2B_CreateRoom = 33001,
    
        // Battle To Main
        B2M_RoomCreated = 34001,
        
        ServerMax = 65535, 
#endif
    }
}

