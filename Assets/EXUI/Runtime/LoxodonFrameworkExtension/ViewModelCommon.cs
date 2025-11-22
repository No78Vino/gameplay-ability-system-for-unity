using System;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.ViewModels;

public class ViewModelCommon : ViewModelBase
{
    public InteractionRequest<string> request;

    public ViewModelCommon()
    {
        request = new InteractionRequest<string>(this);
    }
    
    /// <summary>
    /// UI打开回调
    /// </summary>
    public virtual void OnShow()
    {
    }
    
    /// <summary>
    /// UI关闭回调
    /// </summary>
    public virtual void OnHide()
    {
    }
    
    /// <summary>
    /// 秒更新
    /// </summary>
    public virtual void Update_s()
    {
    }
    
    /// <summary>
    /// 帧更新
    /// </summary>
    public virtual void Update_f()
    {
    }

    public virtual void OnLoaded()
    {
    }

    public virtual void OnUnload()
    {
    }
}