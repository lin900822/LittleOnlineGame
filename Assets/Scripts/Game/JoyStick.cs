using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
    public class JoyStick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        public float Angle { get; private set; }
        
        public RectTransform joystickBackground; // Joystick 背景
        public RectTransform joystickKnob; // Joystick 控制點
        public float maxRadius = 100f; // 搖桿最大移動範圍

        private Vector2 joystickCenter;
        private Vector2 rawDragPosition; // 記錄玩家拖曳的實際位置

        void Start()
        {
            // **記錄 Joystick 背景的中心點**
            joystickCenter = joystickBackground.anchoredPosition;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 touchPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickBackground, eventData.position, eventData.pressEventCamera, out touchPosition);

            // **計算拖曳的方向與距離**
            Vector2 offset = touchPosition - joystickCenter;
            float distance = offset.magnitude;

            // **限制在最大半徑內**
            if (distance > maxRadius)
            {
                offset = offset.normalized * maxRadius;
            }

            // **更新 Knob 位置**
            joystickKnob.anchoredPosition = joystickCenter + offset;

            // **記錄原始拖曳的位置**
            rawDragPosition = offset;

            // **計算角度**
            Angle = Mathf.Atan2(rawDragPosition.y, rawDragPosition.x) * Mathf.Rad2Deg;
            if (Angle < 0) Angle += 360;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            // **恢復 Knob 到中心點**
            joystickKnob.anchoredPosition = joystickCenter;
            rawDragPosition = Vector2.zero;
        }

        public Vector2 GetInputVector()
        {
            return rawDragPosition.normalized; // 使用歸一化向量來表示方向
        }
    }
}