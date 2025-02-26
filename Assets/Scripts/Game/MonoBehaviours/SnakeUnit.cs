using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.MonoBehaviours
{
    public class SnakeUnit : MonoBehaviour
    {
        private Vector2 _lastPos;
        private Vector2 _targetPos;

        private float _startTime;
        private const float LERP_DURATION = 1f / 20f; // 每幀 0.05 秒

        private Transform _snakeUnitBodyPrefab;
        private List<Transform> _snakeUnitBodies;

        private List<Vector2> _lastPoses;
        
        private List<Vector2> Body { get; set; }

        private void Awake()
        {
            _snakeUnitBodyPrefab = Resources.Load<Transform>("Prefabs/SnakeUnit_Body");
        }

        public void UpdateStatus(int x, int y, int length)
        {
            _lastPos = _targetPos;
            _targetPos = new Vector2(x / 1000f, y / 1000f);

            _startTime = Time.time; // 記錄開始時間
            
            if (Body == null)
            {
                Body = new List<Vector2>();
                _snakeUnitBodies = new List<Transform>();
                _lastPoses = new List<Vector2>();

                for (int i = 0; i < length; i++)
                {
                    Body.Add(_targetPos);
                    _lastPoses.Add(_targetPos);
                    var obj = Instantiate(_snakeUnitBodyPrefab,_targetPos, Quaternion.identity, transform);
                    _snakeUnitBodies.Add(obj);
                }
            }
            else
            {
                for (int i = Body.Count; i < length; i++)
                {
                    Body.Add(Body[^1]);
                    _lastPoses.Add(Body[^1]);
                    var obj = Instantiate(_snakeUnitBodyPrefab,Body[^1], Quaternion.identity, transform);
                    _snakeUnitBodies.Add(obj);
                }
            }
            
            for (var i = 0; i < _snakeUnitBodies.Count; i++)
            {
                _lastPoses[i] = _snakeUnitBodies[i].position;
            }
            
            for (int i = Body.Count - 1; i > 0; i--)
            {
                Body[i] = Body[i - 1];
            }
            
            Body[0] = _targetPos;
        }

        private void Update()
        {
            float elapsedTime = Time.time - _startTime;
            float lerpRatio = Mathf.Clamp01(elapsedTime / LERP_DURATION);

            transform.position = Vector2.Lerp(_lastPos, _targetPos, lerpRatio);
            
            for (var i = 0; i < _snakeUnitBodies.Count; i++)
            {
                _snakeUnitBodies[i].position = Vector2.Lerp(_lastPoses[i], Body[i], lerpRatio);
            }

            // 確保補間完成時精確到目標點
            if (lerpRatio >= 1)
            {
                transform.position = _targetPos;
            }
        }
    }
}