using Unity.Collections;

namespace TestUnit_ForGASECS
{
    public class StructForShow
    {
        public static string[] FixedStringToStringArray(NativeArray<FixedString32Bytes> array)
        {
            string[] strings = new string[array.Length];
            for (int i = 0; i < array.Length; ++i)
                strings[i] = array[i].ToString();
            return strings;
        }
    }

    
    public struct ModifierForShow
    {
        
    }
}