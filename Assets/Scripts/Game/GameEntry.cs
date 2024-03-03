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

    private async void Awake()
    {
        _networkAgent = new NetworkAgent();
        
        _networkAgent.RegisterMessageHandler((ushort)MessageId.Login, (communicator, info) =>
        {
            var response = info.Message.ReadUInt16();
            Log.Info($"{response.ToString()}");
        });
        _networkAgent.RegisterMessageHandler((ushort)MessageId.Register, (communicator, info) =>
        {
            var response = info.Message.ReadUInt16();
            Log.Info($"{response.ToString()}");
        });
        
        await _networkAgent.Connect("127.0.0.1", 10001);
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