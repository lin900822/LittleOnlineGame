using System;
using UnityEngine;

namespace Game.MonoBehaviours
{
    public class SnakeUnit : MonoBehaviour
    {
        private Vector2 _target;
        
        public void UpdatePos(int x, int y)
        {
            _target = new Vector2(x / 1000f, y / 1000f);
        }

        private void Update()
        {
            transform.position = Vector2.Lerp(transform.position, _target, Time.deltaTime * 5f);
        }
    }
}