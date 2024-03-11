using UnityEngine;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    public interface IPostMover
    {
        void PostMove(float deltaTime);

        int PMOrder { get; set; }
    }
}