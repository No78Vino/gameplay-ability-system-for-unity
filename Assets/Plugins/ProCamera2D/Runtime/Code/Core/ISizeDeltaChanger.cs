namespace Com.LuisPedroFonseca.ProCamera2D
{
    public interface ISizeDeltaChanger
    {
        float AdjustSize(float deltaTime, float originalDelta);

        int SDCOrder { get; set;}
    }
}