using System.Collections.Generic;

namespace GAS.Runtime
{
    public class XParamALTimelineID:XParam
    {
        public int ID;
        private XParamTimeline _timelineParam;
        
        public void SetID(int value)
        {
            ID = value;
        }
        
        /// <summary>
        /// 这个函数是用于缓存的，需要在XLuban的获取参数阶段调用，要传入XParamTimeline所需的参数【都来自luban的配置参数】
        /// </summary>
        /// <param name="timelineParam"></param>
        public void CacheTimelineParam(XParamTimeline timelineParam)
        {
            _timelineParam = timelineParam;
        }
        
        public XParamALTimelineID(int id)
        {
            ID = id;
        }
        
        public XParamALTimelineID()
        {
            ID = 0;
        }
        
        public XParamTimeline CreateTimelineParam()
        {
            return _timelineParam;
        }
        
#if UNITY_EDITOR
        public void DecodeExcelData(List<object> paramData)
        {
            if (paramData == null || paramData.Count == 0)
            {
                ID = 0;
                return;
            }

            var strData = paramData[0] as string;
            if (string.IsNullOrEmpty(strData))
            {
                ID = 0;
                return;
            }

            if (!int.TryParse(strData, out ID))
            {
                ID = 0;
            }
        }

        public List<object> EncodeExcelData()
        {
            var result = new List<object> { ID };
            return result;
        }
#endif
    }
}