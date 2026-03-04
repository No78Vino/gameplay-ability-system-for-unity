namespace EXUI
{
    public enum UILayer  
    {  
        Background = 0,   // 背景层（场景 UI）  
        Normal     = 100, // 普通窗口  
        Modal      = 200, // 模态弹窗  
        Mask       = 300, // 遮罩层（MaskWindow）  
        Top        = 400, // 顶层（Toast、Loading）  
    }
}