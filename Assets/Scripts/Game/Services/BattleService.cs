using Game.MonoBehaviours;
using Game.UI;
using Shared;
using Shared.Common;
using Shared.Logger;
using Shared.Network;
using UnityEngine;

namespace Game
{
    public class BattleService : ServiceBase
    {
        private NetworkAgent _battleAgent;

        private GameObject _battleGO;
        private SnakeUnit _snakeUnitPrefab;
        
        private SnakeUnit _player1SnakeUnit;
        private SnakeUnit _player2SnakeUnit;

        private ByteBuffer _inputByteBuffer = new ByteBuffer();

        private bool _isBattleStart;

        public override void Init()
        {
            _networkSystem.RegisterMessageHandler(MessageId.M2C_RoomMatched, M2C_RoomMatched);
            
            _snakeUnitPrefab = Resources.Load<SnakeUnit>($"Prefabs/SnakeUnit");

            Log.Info("BattleService Init");
        }

        public override void Update()
        {
            if (_battleAgent != null)
                _battleAgent.Update();

            if (!_isBattleStart)
                return;

            _inputByteBuffer.SetReadIndex(0);
            _inputByteBuffer.SetWriteIndex(0);

            if (Input.GetKey(KeyCode.W))
            {
                _inputByteBuffer.WriteInt32(90);
                _battleAgent.SendMessage((ushort)MessageId.C2B_Input, _inputByteBuffer);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                _inputByteBuffer.WriteInt32(270);
                _battleAgent.SendMessage((ushort)MessageId.C2B_Input, _inputByteBuffer);
            }
            else if (Input.GetKey(KeyCode.A))
            {
                _inputByteBuffer.WriteInt32(180);
                _battleAgent.SendMessage((ushort)MessageId.C2B_Input, _inputByteBuffer);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                _inputByteBuffer.WriteInt32(0);
                _battleAgent.SendMessage((ushort)MessageId.C2B_Input, _inputByteBuffer);
            }
        }

        private void M2C_RoomMatched(NetworkCommunicator communicator, ByteBuffer byteBuffer)
        {
            if (!byteBuffer.TryDecode(out M2C_RoomMatched m2CRoomMatched))
                return;

            _uIManager.HideWindow<Window_Matching>();

            var uiMatchedData = new UIMatchedData()
            {
                Player1Name = m2CRoomMatched.Player1Name,
                Player2Name = m2CRoomMatched.Player2Name,
            };

            _uIManager.PopUpWindow<Window_Matched>(uiMatchedData);

            _battleAgent = new NetworkAgent();
            _battleAgent.Connect(m2CRoomMatched.Ip, m2CRoomMatched.Port).Await(() =>
            {
                OnConnectedToBattleServer(m2CRoomMatched.KeyToEnterRoom);
            });
        }

        private void OnConnectedToBattleServer(string keyToEnterRoom)
        {
            _battleAgent.RegisterMessageHandler((ushort)MessageId.B2C_BattleStart, B2C_BattleStart);
            _battleAgent.RegisterMessageHandler((ushort)MessageId.B2C_SyncState, B2C_SyncState);
            
            var c2BJoinRoom = new C2B_JoinRoom()
            {
                KeyToEnterRoom = keyToEnterRoom,
                PlayerId = ServiceSystem.Instance.PlayerService.PlayerId,
            };
            var data = ProtoUtils.Encode(c2BJoinRoom);
            _battleAgent.SendMessage((ushort)MessageId.C2B_JoinRoom, data);
        }

        private void B2C_BattleStart(NetworkCommunicator communicator, ByteBuffer byteBuffer)
        {
            Log.Info($"B2C_BattleStart");
            _isBattleStart = true;

            _uIManager.HideAllWindows();

            var player1X = byteBuffer.ReadInt32();
            var player1Y = byteBuffer.ReadInt32(); 
            var player2X = byteBuffer.ReadInt32();
            var player2Y = byteBuffer.ReadInt32();

            _battleGO = GamePlayApp.Instance.BattleGO;
            _battleGO.SetActive(true);
            _player1SnakeUnit = Object.Instantiate(_snakeUnitPrefab, _battleGO.transform);
            _player1SnakeUnit.UpdatePos(player1X, player1Y);
            _player2SnakeUnit = Object.Instantiate(_snakeUnitPrefab, _battleGO.transform);
            _player2SnakeUnit.UpdatePos(player2X, player2Y);
        }

        private void B2C_SyncState(NetworkCommunicator communicator, ByteBuffer byteBuffer)
        {
            var player1X = byteBuffer.ReadInt32();
            var player1Y = byteBuffer.ReadInt32(); 
            var player2X = byteBuffer.ReadInt32();
            var player2Y = byteBuffer.ReadInt32();

            _player1SnakeUnit.UpdatePos(player1X, player1Y);
            _player2SnakeUnit.UpdatePos(player2X, player2Y);
        }
    }
}