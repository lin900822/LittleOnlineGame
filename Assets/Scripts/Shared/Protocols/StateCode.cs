namespace Shared
{
    public enum StateCode : uint
    {
        Success = 0,

        Register_Failed_UserExist = 103,
        LoginOrRegister_Failed_InfoEmpty = 104,
        Login_Success = 105,
        Register_Success = 106
    }
}