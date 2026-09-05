namespace EXProceduralMachine
{
    public enum SecondOrderDynamicValueType
    {
        Position,
        Rotation,
        Scale,
        Custom,
        /// <summary>四元数旋转（对数空间二阶系统，避免万向锁；小幅多轴旋转场景）</summary>
        QuaternionRotation
    }
}