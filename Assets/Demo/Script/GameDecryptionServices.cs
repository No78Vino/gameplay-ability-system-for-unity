using System;
using System.IO;
using UnityEngine;
using YooAsset;

namespace Demo.Script
{
    public class GameDecryptionServices : IDecryptionServices
    {
        public ulong LoadFromFileOffset(DecryptFileInfo fileInfo)
        {
            return 32;
        }
    
        public byte[] LoadFromMemory(DecryptFileInfo fileInfo)
        {
            // 如果没有内存加密方式，可以返回空
            throw new NotImplementedException();
        }

        public Stream LoadFromStream(DecryptFileInfo fileInfo)
        {
            // 如果没有流加密方式，可以返回空
            throw new NotImplementedException();
        }
    
        public uint GetManagedReadBufferSize()
        {
            return 1024;
        }

        public AssetBundle LoadAssetBundle(DecryptFileInfo fileInfo, out Stream managedStream)
        {
            throw new NotImplementedException();
        }

        public AssetBundleCreateRequest LoadAssetBundleAsync(DecryptFileInfo fileInfo, out Stream managedStream)
        {
            throw new NotImplementedException();
        }
    }
}