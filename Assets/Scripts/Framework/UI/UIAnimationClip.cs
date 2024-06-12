using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Framework.UI
{
    public abstract class UIAnimation
    {
        public float StartTime;
        public float EndTime;

        public Ease           Ease;
        public AnimationCurve AnimationCurve;
    }

    [Serializable]
    public class PositionAnimation : UIAnimation
    {
        public Vector2 StartPosition;
        public Vector2 EndPosition;
    }

    [Serializable]
    public class RotationAnimation : UIAnimation
    {
        public float StartAngle;
        public float EndAngle;
    }

    [Serializable]
    public class ScaleAnimation : UIAnimation
    {
        public Vector2 StartScale;
        public Vector2 EndScale;
    }

    [Serializable]
    public class AlphaAnimation : UIAnimation
    {
        public float StartAlpha;
        public float EndAlpha;
    }
    
    [CreateAssetMenu(fileName = "UIAnimationClip", menuName = "UI框架/UIAnimationClip")]
    public class UIAnimationClip : ScriptableObject
    {
        public List<PositionAnimation> PositionAnimations;
        public List<RotationAnimation> RotationAnimations;
        public List<ScaleAnimation>    ScaleAnimations;
        public List<AlphaAnimation>    AlphaAnimations;
    }
}