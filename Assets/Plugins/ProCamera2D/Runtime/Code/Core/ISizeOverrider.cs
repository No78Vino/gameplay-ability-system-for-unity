namespace Com.LuisPedroFonseca.ProCamera2D
{
    public interface ISizeOverrider
    {
        float OverrideSize(float deltaTime, float originalSize);

        int SOOrder { get; set;}
    }
}