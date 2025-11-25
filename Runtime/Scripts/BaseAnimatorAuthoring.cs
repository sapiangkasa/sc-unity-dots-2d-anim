using UnityEngine;
using Unity.Entities;
using Unity.Rendering;
using Unity.Mathematics;
using Unity.Burst;

namespace SC.Ecs.Anim2d
{
    /********** Components **********/

    /*
    The sprite facing direction.
    1: Right
    -1: Left
    */
    [MaterialProperty("_FacingDirection")]
    public struct FacingDirection : IComponentData
    {
        public float Value;
    }

    // The index of the currently playing animation
    [MaterialProperty("_AnimationIndex")]
    public struct AnimationIndex : IComponentData
    {
        public float Value;
    }

    // The total animation playable animation count
    [MaterialProperty("_AnimationCount")]
    public struct AnimationCount : IComponentData
    {
        public float Value;
    }

    // The current frame index of the currently playing animation
    [MaterialProperty("_FrameIndex")]
    public struct FrameIndex : IComponentData
    {
        public float Value;
    }

    // The total frame counte of the currently playing animation
    [MaterialProperty("_FrameCount")]
    public struct FrameCount : IComponentData
    {
        public float Value;
    }

    // The maximum frame count of available in the sprite sheet 
    [MaterialProperty("_MaxFrameCount")]
    public struct MaxFrameCount : IComponentData
    {
        public float Value;
    }

    public struct PrevAnimationIndex : IComponentData
    {
        public int Value;
    }

    public struct CurrentAnimationData : IComponentData
    {
        public AnimationData AnimationData;
    }

    public struct OnAnimationEndedEventFlag : IComponentData, IEnableableComponent {}

    /********** Systems and Jobs **********/
    public partial struct AnimationFrameUpdateSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;

            var animationFrameUpdateJob = new AnimationFrameUpdateJob
            {
                DeltaTime = deltaTime,
                Fps = AnimationConstants.FRAMES_PER_SECOND
            };
            state.Dependency = animationFrameUpdateJob.ScheduleParallel(state.Dependency);
        }
    }

    [BurstCompile]
    [WithPresent(typeof(OnAnimationEndedEventFlag))]
    public partial struct AnimationFrameUpdateJob : IJobEntity
    {
        public float DeltaTime;
        public float Fps;

        private void Execute( Entity entity,
                            ref FrameIndex frameIndex,
                            ref FrameCount frameCount,
                            ref AnimationIndex animationIndex,
                            ref PrevAnimationIndex prevAnimationIndex,
                            EnabledRefRW<OnAnimationEndedEventFlag> onAnimEndedFlag,
                            in CurrentAnimationData currentAnimationData)
        {
            var animationData = currentAnimationData.AnimationData;

            // If animation changed, set new animation data and reset frame index
            if (prevAnimationIndex.Value != animationData.AnimationIndex)
            {
                // Reset frame index when animation changes
                frameIndex.Value = 0f;
                // Update to new animation data
                frameCount.Value = animationData.TotalFrames;
                animationIndex.Value = animationData.AnimationIndex;
                prevAnimationIndex.Value = animationData.AnimationIndex;
                onAnimEndedFlag.ValueRW = false;
                return;
            }

            // Update frame index based on delta time and fps
            var newIndex = frameIndex.Value + (DeltaTime * Fps);
            
            if (Utils.IsAnimationCompleted(animationData, (int)newIndex))
            {
                onAnimEndedFlag.ValueRW = true;
            }

            // Clamp or loop the frame index
            if (!animationData.IsLooping && newIndex >= animationData.TotalFrames)
            {
                newIndex = animationData.TotalFrames - 1;
            }
            else
            {
                newIndex = math.fmod(newIndex, frameCount.Value);
            }
            frameIndex.Value = newIndex;
        }
    }

    /********** Authoring **********/
    public abstract class BaseAnimatorAuthoring : MonoBehaviour
    {
        [Header("Animation Settings")]
        [SerializeField]
        protected int FacingDirection = 1;
        [SerializeField]
        protected int AnimationIndex;
        [SerializeField]
        protected int AnimationCount = 1;
        [SerializeField]
        protected int FrameIndex;
        [SerializeField]
        protected int FrameCount = 1;
        [SerializeField]
        protected int MaxFrameCount = 1;

        public abstract class AnimatorBaker<T> : Baker<T>  where T : BaseAnimatorAuthoring
        {
            public override void Bake(T authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new FacingDirection { Value = authoring.FacingDirection });
                AddComponent(entity, new AnimationIndex { Value = authoring.AnimationIndex });
                AddComponent(entity, new AnimationCount { Value = authoring.AnimationCount });
                AddComponent(entity, new FrameIndex { Value = authoring.FrameIndex });
                AddComponent(entity, new FrameCount { Value = authoring.FrameCount });
                AddComponent(entity, new MaxFrameCount { Value = authoring.MaxFrameCount });
                AddComponent(entity, new PrevAnimationIndex { Value = -1 });
                AddComponent(entity, new CurrentAnimationData
                {
                    AnimationData = new AnimationData
                    {
                        AnimationIndex = authoring.AnimationIndex,
                        TotalFrames = authoring.FrameCount,
                        IsLooping = true
                    }
                });

                AddComponent<OnAnimationEndedEventFlag>(entity);
                SetComponentEnabled<OnAnimationEndedEventFlag>(entity, false);
            }
        }
    }
}