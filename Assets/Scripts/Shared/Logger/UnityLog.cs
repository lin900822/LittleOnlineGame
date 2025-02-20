namespace Shared.Logger
{
    public class UnityLog : ILog
    {
        public void Debug(string message)
        {
#if UNITY_STANDALONE
            UnityEngine.Debug.Log($"[Debug] {message}");
#endif
        }

        public void Info(string message)
        {
#if UNITY_STANDALONE
            UnityEngine.Debug.Log($"[Info] {message}");
#endif
        }

        public void Warn(string message)
        {
#if UNITY_STANDALONE
            UnityEngine.Debug.LogWarning($"[Warn] {message}");
#endif
        }

        public void Error(string message)
        {
#if UNITY_STANDALONE
            UnityEngine.Debug.LogError($"[Error] {message}");
#endif
        }
    }
}