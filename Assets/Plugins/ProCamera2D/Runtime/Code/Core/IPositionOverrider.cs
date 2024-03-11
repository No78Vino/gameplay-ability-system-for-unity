using UnityEngine;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    public interface IPositionOverrider
    {
        Vector3 OverridePosition(float deltaTime, Vector3 originalPosition);

        int POOrder { get; set; }
    }
}