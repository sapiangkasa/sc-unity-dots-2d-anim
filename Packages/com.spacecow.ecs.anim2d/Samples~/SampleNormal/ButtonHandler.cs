using UnityEngine;
using Unity.Entities;

public class ButtonHandler : MonoBehaviour
{
    [SerializeField] private ECharacterAnimation _characterAnimation;

    public void OnButtonPressed()
    {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var singletonQuery = entityManager.CreateEntityQuery(typeof(InputHandlerTag));

        // Try to get the singleton entity
        if (singletonQuery.TryGetSingletonEntity<InputHandlerTag>(out Entity singletonEntity))
        {
            entityManager.SetComponentData(singletonEntity, new CurrentAnimationType { Value = (float)_characterAnimation });
        }
    }
}
