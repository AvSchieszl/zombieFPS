using UnityEngine;

[CreateAssetMenu(fileName = "Trail Config", menuName = "Guns/ Gun Trail Config", order = 4)]

public class TrailConfigurationScriptableObject : ScriptableObject
{
    [SerializeField] private Material material;
    [SerializeField] private AnimationCurve widthCurve;
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private float minVertexDistance = 0.1f;
    [SerializeField] private Gradient color;

    [SerializeField] private float missDistance = 100f;
    [SerializeField] private float simulationSpeed = 100f;
}