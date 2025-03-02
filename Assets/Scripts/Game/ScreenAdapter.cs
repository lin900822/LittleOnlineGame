using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ScreenAdapter : MonoBehaviour
    {
        [SerializeField] List<Camera> cameras = new List<Camera>();

        [SerializeField] float standardRatio = 16f / 9f;

        float screenWidth;
        float screenHeight;

        float viewWidth;
        float viewHeight;

        void Start()
        {
            screenWidth = Screen.width;
            screenHeight = Screen.height;

            float screenRation = screenWidth / screenHeight;

            Rect newRect = new Rect();

            if (screenRation < standardRatio)
            {
                viewWidth = screenWidth;
                viewHeight = viewWidth / standardRatio;

                newRect.width = 1f;
                newRect.height = viewHeight / screenHeight;

                newRect.y = ((screenHeight / 2f) - (viewHeight / 2f)) / screenHeight;
            }
            else
            {
                viewHeight = screenHeight;
                viewWidth = viewHeight * standardRatio;

                newRect.width = viewWidth / screenWidth;
                newRect.height = 1f;

                newRect.x = ((screenWidth / 2f) - (viewWidth / 2f)) / screenWidth;
            }

            for (int i = 0; i < cameras.Count; i++)
                cameras[i].rect = newRect;
        }
    }
}