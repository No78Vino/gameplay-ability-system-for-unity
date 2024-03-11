using UnityEngine;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    /// <summary>
    /// Basic blit class used by the ProCamera2DTransitionFX extension
    /// </summary>

    [ExecuteInEditMode]
    public class BasicBlit : MonoBehaviour
    {
        public Material CurrentMaterial;

        void OnRenderImage(RenderTexture src, RenderTexture dst)
        {
            if (CurrentMaterial != null)
                Graphics.Blit(src, dst, CurrentMaterial);
        }
    }
}