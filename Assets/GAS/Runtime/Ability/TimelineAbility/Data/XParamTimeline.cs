using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace GAS.Runtime
{
    public class XParamTimeline : XParam
    {
        [ShowInInspector]
        public int ID { get; private set; }
        
        [ShowInInspector]
        public string Name { get; private set; }
        
        [ShowInInspector]
        public int LifeTime { get; private set; }
        
        [ShowInInspector]
        public bool ManualEndAbility { get; private set; }
        
        [ShowInInspector]
        public List<Track> Tracks = new List<Track>();
        
        public XParamTimeline()
        {
            Name = string.Empty;
            LifeTime = 1;
            ManualEndAbility = false;
        }
        
        public XParamTimeline(int id,string name,int lifeTime,bool manualEndAbility,List<Track> tracks)
        {
            SetID(id);
            SetName(name);
            SetLifeTime(lifeTime);
            SetManualEndAbility(manualEndAbility);
            SetTracks(tracks);
        }

        public void SetID(int value) => ID = value;
        
        public void SetName(string value)
        {
            Name = value;
        }

        public void SetLifeTime(int value)
        {
            LifeTime = value;
        }
        
        public void SetManualEndAbility(bool value)
        {
            ManualEndAbility = value;
        }
        
        public void SetTracks(List<Track> value)
        {
            Tracks = value;
        }

#if UNITY_EDITOR
        public void DecodeExcelData(List<object> paramData)
        {
            // if (paramData == null || paramData.Count == 0)
            // {
            //     Value = string.Empty;
            //     return;
            // }
            //
            // var strData = paramData[0] as string;
            // if (string.IsNullOrEmpty(strData))
            // {
            //     Value = string.Empty;
            //     return;
            // }
            //
            // Value = strData;
        }

        public List<object> EncodeExcelData()
        {
            return new List<object>();
        }
#endif
    }
}