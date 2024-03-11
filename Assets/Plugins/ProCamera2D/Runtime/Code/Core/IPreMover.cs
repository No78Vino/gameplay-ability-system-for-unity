using UnityEngine;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    public interface IPreMover
    {
        void PreMove(float deltaTime);

        int PrMOrder { get; set; }
    }
}