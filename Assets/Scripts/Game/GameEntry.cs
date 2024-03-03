using Common;
using Core.Network;
using Network;
using Protocols;
using UnityEngine;

public class GameEntry : MonoBehaviour
{
    [SerializeField] private string _username;
    [SerializeField] private string _password;
    
    private NetworkAgent _networkAgent;

    private byte[] _loginData;

    private async void Awake()
    {
        _networkAgent = new NetworkAgent();
        
        await _networkAgent.Connect("127.0.0.1", 10001);

        ByteBuffer byteBuffer = ByteBufferPool.Shared.Rent(10);
        byteBuffer.WriteUInt16(123);
        _loginData = new byte[byteBuffer.Length];
        byteBuffer.Read(_loginData, 0, byteBuffer.Length);
        
        _networkAgent.RegisterMessageHandler((ushort)MessageId.Login, (communicator, info) =>
        {
            var response = info.Message.ReadUInt16();
            Log.Info($"{response.ToString()}");
        });
    }

    private void Update()
    {
        _networkAgent.Update();
        
        if (Input.GetKeyDown(KeyCode.A))
        {
            var user = new User()
            {
                Username = _username,
                Password = _password
            };
            var userData = ProtoUtils.Encode(user);
            _networkAgent.SendMessage((ushort)MessageId.Login, userData);
        }
        else if(Input.GetKeyDown(KeyCode.B))
        {
            var user = new User()
            {
                Username = _username,
                Password = _password
            };
            var userData = ProtoUtils.Encode(user);
            _networkAgent.SendMessage((ushort)MessageId.Register, userData);
        }
    }
}