using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace GAS.Runtime
{
    public class XParamCueList: XParam
    {
        private static List<ValueDropdownItem> CueIDs = new List<ValueDropdownItem>();
        
        [ShowInInspector]
        [ValueDropdown(nameof(CueIDs))]
        public int[] Cues { get; private set; }

        public void SetCues(int[] value)
        {
            Cues = value;
        }
        
        public XParamCueList(int[] value)
        {
            Cues = value;
        }
        
        public XParamCueList()
        {
            Cues = Array.Empty<int>();
        }
#if UNITY_EDITOR
        public void DecodeExcelData(List<object> paramData)
        {
            if (paramData == null || paramData.Count == 0)
            {
                Cues = Array.Empty<int>();
                return;
            }

            
            var strData = paramData[0] as string;
            if (string.IsNullOrEmpty(strData))
            {
                Cues = Array.Empty<int>();
                return;
            }

            var ids = strData.Split(';');
            Cues = new int[ids.Length];
            for (var i = 0; i < ids.Length; i++)
            {
                var strID = ids[i];
                if (!int.TryParse(strID, out var id))
                    Cues[i] = id;
            }

            if (CueIDs.Count == 0)
            {
                XLuban.Init();
                foreach (var cue in XLuban.Tables.TbgameplayCue.DataList)
                    CueIDs.Add(new ValueDropdownItem($"{cue.Id}[{cue.Name}]",cue.Id));
            }
        }

        public List<object> EncodeExcelData()
        {
            var strIDs = "";
            for (var i = 0; i < Cues.Length; i++)
            {
                strIDs += Cues[i].ToString();
                if (i < Cues.Length - 1) strIDs += ";";
            }
            var result = new List<object> { strIDs };
            return result;
        }
#endif
    }
}