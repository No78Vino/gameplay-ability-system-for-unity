using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;

namespace XYooAsset
{
    public class XYooAssetManager
    {
        public static XYooAssetManager Instance { get; } = new XYooAssetManager();

        private XYooAssetHost _host;
        
        private ResourcePackage _defaultPackage;

        public ResourcePackage Package => _defaultPackage;

        public void Initialize(string defaultPackageName)
        {
            _host = new GameObject("XYooAssetHost").AddComponent<XYooAssetHost>();
            _host.StartCoroutine(InitPackage(defaultPackageName));
        }
        
        public IEnumerator InitPackage(string defaultPackageName)
        {
            YooAssets.Initialize();
            
            var package = YooAssets.CreatePackage(defaultPackageName);
            _defaultPackage = package;
            YooAssets.SetDefaultPackage(package);
            
            var fileSystemParams = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
    
            var createParameters = new OfflinePlayModeParameters
            {
                BuildinFileSystemParameters = fileSystemParams
            };

            // 1. 初始化资源包
            var initOperation = package.InitializeAsync(createParameters);
            yield return initOperation;

            if (initOperation.Status != EOperationStatus.Succeed)
            {
                Debug.LogError($"资源包初始化失败：{initOperation.Error}");
                yield break;
            }
            
            // 2. 请求资源清单的版本信息
            var requestPackageOperation = package.RequestPackageVersionAsync();
            yield return requestPackageOperation;
            if (requestPackageOperation.Status != EOperationStatus.Succeed)
            {
                Debug.LogError($"请求资源清单版本信息失败：{requestPackageOperation.Error}");
                yield break;
            }
    
            // 3. 传入的版本信息更新资源清单
            var updatePackageManifestOperation = package.UpdatePackageManifestAsync(requestPackageOperation.PackageVersion);
            yield return updatePackageManifestOperation;
            if (updatePackageManifestOperation.Status != EOperationStatus.Succeed)
            {
                Debug.LogError($"更新资源清单失败：{updatePackageManifestOperation.Error}");
                yield break;
            }
            
            Debug.Log("资源包初始化完成");
        }
        
        public ResourcePackage GetPackage(string packageName)
        {
            return YooAssets.GetPackage(packageName);
        }

        public void LoadSceneAsync(string path, System.Action<SceneHandle> completed = null)
        {
            var sceneMode = LoadSceneMode.Single;
            var physicsMode = LocalPhysicsMode.None;
            SceneHandle handle = YooAssets.LoadSceneAsync(path, sceneMode, physicsMode);
            handle.Completed += completed;
        }

        public TObject LoadAssetSync<TObject>(string assetPath) where TObject : Object
        {
            var handle =  Package.LoadAssetSync<TObject>(assetPath);
            return handle.AssetObject as TObject;
        }
        
        public void LoadAssetAsync<TObject>(string assetPath, System.Action<TObject> completed) where TObject : Object
        {
            var handle = Package.LoadAssetAsync<TObject>(assetPath);
            handle.Completed += operation =>
            {
                completed?.Invoke(operation.AssetObject as TObject);
            };
        }
    }



    /// <summary>
    /// 便捷XYooAsset访问类
    /// </summary>
    public static class XYoo
    {
        public static void Initialize(string defaultPackageName)
        {
            XYooAssetManager.Instance.Initialize(defaultPackageName);
        }
        
        public static TObject LoadAssetSync<TObject>(string assetPath) where TObject : Object
        {
            return XYooAssetManager.Instance.LoadAssetSync<TObject>(assetPath);
        }
        
        public static void LoadAssetAsync<TObject>(string assetPath, System.Action<TObject> completed) where TObject : Object
        {
            XYooAssetManager.Instance.LoadAssetAsync(assetPath, completed);
        }
        
        public static void LoadSceneAsync(string path, System.Action<SceneHandle> completed = null)
        {
            XYooAssetManager.Instance.LoadSceneAsync(path, completed);
        }
    }
}