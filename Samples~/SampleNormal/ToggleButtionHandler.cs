using UnityEngine;
using UnityEngine.UI;
using Unity.Entities;

public class ToggleButtionHandler : MonoBehaviour
{
    [SerializeField]
    private Toggle _toggle;

    public void Start()
    {
        // Initialize the toggle state if needed
        _toggle.isOn = false;
    }

    public void OnFlipButtonPressed(bool isOn)
    {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var singletonQuery = entityManager.CreateEntityQuery(typeof(InputHandlerTag));

        // Try to get the singleton entity
        if (singletonQuery.TryGetSingletonEntity<InputHandlerTag>(out Entity singletonEntity))
        {
            entityManager.SetComponentData(singletonEntity, new CurrentFlipState { Value = isOn });
        }
    }
}
