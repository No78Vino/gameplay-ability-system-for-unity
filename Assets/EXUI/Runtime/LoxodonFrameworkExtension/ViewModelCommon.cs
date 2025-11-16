using System;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.ViewModels;

public class ViewModelCommon : ViewModelBase
{
    public InteractionRequest<string> transitionRequest = new InteractionRequest<string>();
    public InteractionRequest<string> commonRequest = new InteractionRequest<string>();

    /// <summary>
    /// UI打开回调
    /// </summary>
    public virtual void OnOpen()
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

    public virtual void OnLoaded(){ }
    public virtual void OnUnload(){}
}