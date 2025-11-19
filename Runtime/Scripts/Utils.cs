using Unity.Entities;

namespace SC.Ecs.Anim2d
{
    public static class AnimationConstants
    {
        public const float FRAMES_PER_SECOND = 12f;
    }

    public class Utils
    {
        public static bool IsAnimationCompleted(in AnimationData animationData, int newFrameIndex)
        {
            if (animationData.IsLooping)
            {
                return false;
            }
            return newFrameIndex >= animationData.TotalFrames;
        }
    }
}
