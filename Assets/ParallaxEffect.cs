using UnityEngine;
using Cinemachine;

[ExecuteAlways]
[RequireComponent(typeof(CinemachineVirtualCamera))]
public class ParallaxEffect : CinemachineExtension
{
    [System.Serializable]
    public class ParallaxLayer
    {
        public Transform layerTransform;
        public float parallaxFactor;
    }

    public ParallaxLayer[] parallaxLayers;

    private Vector3 _lastCameraPosition;

    protected override void Awake()
    {
        base.Awake();
        _lastCameraPosition = VirtualCamera.transform.position;
    }

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage,
        ref CameraState state,
        float deltaTime)
    {
        if (stage != CinemachineCore.Stage.Body) return;

        Vector3 cameraDelta = state.FinalPosition - _lastCameraPosition;
        foreach (var layer in parallaxLayers)
        {
            if (layer.layerTransform != null)
            {
                layer.layerTransform.position += new Vector3(
                    cameraDelta.x * layer.parallaxFactor,
                    cameraDelta.y * layer.parallaxFactor,
                    0
                );
            }
        }
        _lastCameraPosition = state.FinalPosition;
    }
}
