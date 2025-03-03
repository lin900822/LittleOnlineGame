using Game.MonoBehaviours;
using Game.UI;
using Shared;
using Shared.Common;
using Shared.Logger;
using Shared.Network;
using Unity.VisualScripting;
using UnityEngine;

namespace Game
{
    public class BattleService : ServiceBase
    {
        private NetworkAgent _battleAgent;

        private GameObject _battleGO;
        private SnakeUnit _snakeUnitPrefab;

        private GameObject _food;
        private GameObject _foodPrefab;

        private SnakeUnit _player1SnakeUnit;
        private SnakeUnit _player2SnakeUnit;

        private ByteBuffer _inputByteBuffer = new ByteBuffer();

        private bool _isBattleStart;

        public override void Init()
        {
            _networkSystem.RegisterMessageHandler(MessageId.M2C_RoomMatched, M2C_RoomMatched);

            _snakeUnitPrefab = Resources.Load<SnakeUnit>($"Prefabs/SnakeUnit");
            _foodPrefab = Resources.Load<GameObject>($"Prefabs/Food");

            Log.Info("BattleService Init");
        }

        public override void Update()
        {
            if (_battleAgent != null)
                _battleAgent.Update();

            if (!_isBattleStart)
                return;


            if (Input.GetKey(KeyCode.W))
            {
                SetInput(90);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                SetInput(270);
            }
            else if (Input.GetKey(KeyCode.A))
            {
                SetInput(180);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                SetInput(0);
            }
        }

        public void SetInput(float angle)
        {
            _inputByteBuffer.SetReadIndex(0);
            _inputByteBuffer.SetWriteIndex(0);

            _inputByteBuffer.WriteInt32((int)angle);
            _battleAgent.SendMessage((ushort)MessageId.C2B_Input, _inputByteBuffer);
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
            _battleAgent.RegisterMessageHandler((ushort)MessageId.B2C_BattleEnd, B2C_BattleEnd);

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
            _uIManager.PopUpWindow<Window_GamePlay>();

            _battleGO = GamePlayApp.Instance.BattleGO;
            _battleGO.SetActive(true);

            _food = Object.Instantiate(_foodPrefab);

            var player1X = byteBuffer.ReadInt32();
            var player1Y = byteBuffer.ReadInt32();
            var player1Length = byteBuffer.ReadInt32();
            var player2X = byteBuffer.ReadInt32();
            var player2Y = byteBuffer.ReadInt32();
            var player2Length = byteBuffer.ReadInt32();
            var foodX = byteBuffer.ReadInt32();
            var foodY = byteBuffer.ReadInt32();
            _player1SnakeUnit = Object.Instantiate(_snakeUnitPrefab, _battleGO.transform);
            _player1SnakeUnit.UpdateStatus(player1X, player1Y, player1Length);
            _player2SnakeUnit = Object.Instantiate(_snakeUnitPrefab, _battleGO.transform);
            _player2SnakeUnit.UpdateStatus(player2X, player2Y, player2Length);
            _food.transform.position = new Vector2(foodX / 1000f, foodY / 1000f);
        }

        private void B2C_SyncState(NetworkCommunicator communicator, ByteBuffer byteBuffer)
        {
            var player1X = byteBuffer.ReadInt32();
            var player1Y = byteBuffer.ReadInt32();
            var player1Length = byteBuffer.ReadInt32();
            var player2X = byteBuffer.ReadInt32();
            var player2Y = byteBuffer.ReadInt32();
            var player2Length = byteBuffer.ReadInt32();
            var foodX = byteBuffer.ReadInt32();
            var foodY = byteBuffer.ReadInt32();
            _player1SnakeUnit.UpdateStatus(player1X, player1Y, player1Length);
            _player2SnakeUnit.UpdateStatus(player2X, player2Y, player2Length);

            _food.transform.position = new Vector2(foodX / 1000f, foodY / 1000f);
        }

        private void B2C_BattleEnd(NetworkCommunicator communicator, ByteBuffer byteBuffer)
        {
            var result = (BattleEndResult)byteBuffer.ReadInt32();
            var winnerPlayerId = byteBuffer.ReadUInt32();

            string message;

            if (result == BattleEndResult.OtherDisconnected)
            {
                message = "對手中斷連線 遊戲結束";
            }
            else
            {
                if (winnerPlayerId == ServiceSystem.Instance.PlayerService.PlayerId)
                {
                    message = "你贏了";
                }
                else
                {
                    message = "你輸了";
                }
            }

            _uIManager.PopUpWindow<Window_SystemInfo>(new UIData_SystemInfo()
            {
                Message = message,
                OnConfirm = () =>
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Object.Destroy(_player1SnakeUnit.gameObject);
                        Object.Destroy(_player2SnakeUnit.gameObject);
                        Object.Destroy(_food.gameObject);
                    }

                    _battleGO = GamePlayApp.Instance.BattleGO;
                    _battleGO.SetActive(false);

                    _uIManager.HideAllWindows();
                    _uIManager.PopUpWindow<Window_Lobby>();
                }
            });
        }
    }
}