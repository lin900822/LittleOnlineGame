using System;
using UnityEngine;

namespace Game.MonoBehaviours
{
    public class SnakeUnit : MonoBehaviour
    {
        private Vector2 _lastPos;
        private Vector2 _targetPos;

        private float _startTime; 
        private const float LERP_DURATION = 1f / 20f; // 每幀 0.05 秒

        public void UpdatePos(int x, int y)
        {
            _lastPos = _targetPos;
            _targetPos = new Vector2(x / 1000f, y / 1000f);
        
            _startTime = Time.time; // 記錄開始時間
        }

        private void Update()
        {
            float elapsedTime = Time.time - _startTime;
            float lerpRatio = Mathf.Clamp01(elapsedTime / LERP_DURATION);

            transform.position = Vector2.Lerp(_lastPos, _targetPos, lerpRatio);

            // 確保補間完成時精確到目標點
            if (lerpRatio >= 1)
            {
                transform.position = _targetPos;
            }
        }
    }
}