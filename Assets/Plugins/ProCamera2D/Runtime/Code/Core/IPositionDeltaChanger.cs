using UnityEngine;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    public interface IPositionDeltaChanger
    {
        Vector3 AdjustDelta(float deltaTime, Vector3 originalDelta);

        int PDCOrder { get; set;}
    }
}