namespace Shared
{
    public enum StateCode : uint
    {
        Success = 0,

        Login_Failed_InfoWrong = 102,
        Register_Failed_UserExist = 103,
        LoginOrRegister_Failed_InfoEmpty = 104,
        Login_Success = 105,
        Register_Success = 106,
        Another_User_LoggedIn = 107,
        TimeOut = 108,
        JoinMatchQueue_Failed_AlreadyIn = 109,
        CancelJoinMatchQueue_Failed_NOtIn = 110,
    }
}