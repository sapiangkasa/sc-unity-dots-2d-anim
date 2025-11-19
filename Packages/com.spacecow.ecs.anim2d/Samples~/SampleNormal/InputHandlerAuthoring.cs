using UnityEngine;
using Unity.Entities;

using SC.Ecs.Anim2d;
using Unity.Burst;

public struct InputHandlerTag : IComponentData { }

public struct CurrentAnimationType : IComponentData
{
    public float Value;
}

public struct CurrentFlipState : IComponentData
{
    public bool Value;
}

public class InputHandlerAuthoring : MonoBehaviour
{

    public class InputHandlerBaker : Baker<InputHandlerAuthoring>
    {
        public override void Bake(InputHandlerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent<InputHandlerTag>(entity);
            AddComponent(entity, new CurrentAnimationType { Value = (float)ECharacterAnimation.Idle });
            AddComponent(entity, new CurrentFlipState { Value = false });
        }
    }
}

public partial struct InputHandlerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<CurrentAnimationType>();
        state.RequireForUpdate<CurrentFlipState>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var currentAnimation = SystemAPI.GetSingleton<CurrentAnimationType>();
        var currentFlipState = SystemAPI.GetSingleton<CurrentFlipState>();

        foreach (var (animationData, facingDirection) in
                 SystemAPI.Query<RefRW<CurrentAnimationData>, RefRW<FacingDirection>>())
        {
            switch ((ECharacterAnimation)(int)currentAnimation.Value)
            {
                case ECharacterAnimation.Idle:
                    animationData.ValueRW.AnimationData.AnimationIndex = (int)currentAnimation.Value;
                    animationData.ValueRW.AnimationData.TotalFrames = 11;
                    animationData.ValueRW.AnimationData.IsLooping = true;
                    break;
                case ECharacterAnimation.Run:
                    animationData.ValueRW.AnimationData.AnimationIndex = (int)currentAnimation.Value;
                    animationData.ValueRW.AnimationData.TotalFrames = 12;
                    animationData.ValueRW.AnimationData.IsLooping = true;
                    break;
                case ECharacterAnimation.Damaged:
                    animationData.ValueRW.AnimationData.AnimationIndex = (int)currentAnimation.Value;
                    animationData.ValueRW.AnimationData.TotalFrames = 7;
                    animationData.ValueRW.AnimationData.IsLooping = false;
                    break;
            }
            facingDirection.ValueRW.Value = currentFlipState.Value ? -1f : 1f;
        }

        
    }
}

[UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
public partial struct HandleAnimationEventSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<CurrentAnimationType>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // In this sample all animation authoring components are controlled by the same UI input
        bool shouldReturnToIdle = false;
        foreach (var onAnimEndFlag in SystemAPI.Query<EnabledRefRW<OnAnimationEndedEventFlag>>())
        {
            // Reset animation flag
            onAnimEndFlag.ValueRW = false;
            shouldReturnToIdle = true;
        }

        if (shouldReturnToIdle)
        {
            var currentAnimation = SystemAPI.GetSingleton<CurrentAnimationType>();
            currentAnimation.Value = (float)ECharacterAnimation.Idle;
            SystemAPI.SetSingleton(currentAnimation);
        }
    }
}