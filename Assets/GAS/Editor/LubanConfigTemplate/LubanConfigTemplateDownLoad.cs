using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;  ///  核心：原生Zip支持

namespace GAS.Editor
{
    public class LubanConfigTemplateDownLoad : EditorWindow
    {
        // 配置参数
        private const string GithubOwner = "No78Vino";
        private const string GithubRepo = "No78Vino.github.io";
        private const string RemoteZipPath = "ProjectConfigTable.zip"; // 仓库里zip的路径
        private string _localTargetDirectory = "EX_GAS_Config"; // 解压目标路径
        private string _personalAccessToken = ""; // 私有仓库必填

        private bool isProcessing;

        private void OnGUI()
        {
            GUILayout.Label("GitHub云端信息:\n" +
                            $"用户名:{GithubOwner}\n" +
                            $"仓库名:{GithubRepo}\n" +
                            $"云端配置文件路径（压缩包）:{RemoteZipPath}", EditorStyles.wordWrappedLabel);

            GUILayout.Space(5);
            GUILayout.Label("本地的配置表工程路径，请不要放在在Assets目录下！\n" +
                            "配置表工程路径是不算项目数据的，只有导出的json文件才算。" , EditorStyles.helpBox);
            _localTargetDirectory = EditorGUILayout.TextField("本地部署路径", _localTargetDirectory);

            GUILayout.Space(5);
            GUILayout.Label("github每小时的API匿名访问次数是60，而使用token是每小时5000次。\n" +
                            "不过，为了减少使用token的麻烦之处，我将模板配置文件夹整个打包成了一个压缩包放在云端，" +
                            "所以每次下载只会消耗1次API访问。\n" +
                            "如果不清楚github的token如何申请，可以访问：https://github.com/settings/tokens", EditorStyles.helpBox);
            _personalAccessToken = EditorGUILayout.PasswordField("[选填]认证Token", _personalAccessToken);

            GUILayout.Space(15);

            GUI.enabled = !isProcessing;
            if (GUILayout.Button("下载并解压 (一键部署)", GUILayout.Height(40)))
                InstallFromZip();

            GUI.enabled = true;
        }

        [MenuItem("EXTool/EX-GAS/导入模板Luban配置目录")]
        public static void ShowWindow()
        {
            GetWindow<LubanConfigTemplateDownLoad>("导入模板Luban配置目录");
        }

        private async void InstallFromZip()
        {
            isProcessing = true;
            // 临时文件路径放在系统缓存区，避免干扰Assets目录
            var tempZipPath = GetTempZipPath();

            try
            {
                // 1. 获取 GitHub 文件的真实下载地址
                var downloadUrl = await GetGitHubDownloadUrl();

                // 2. 下载 Zip 文件
                await DownloadFileAsync(downloadUrl, tempZipPath);

                // 3. 准备解压目录 (清空旧数据)
                var extractPath = Path.Combine(Directory.GetCurrentDirectory(), _localTargetDirectory);
                PrepareDirectory(extractPath);

                // 4. 解压
                ExtractZip(tempZipPath, extractPath);

                // 5. 刷新 Unity 资源数据库
                AssetDatabase.Refresh();

                EditorUtility.DisplayDialog("部署成功", $"资源已成功解压至:\n{_localTargetDirectory}", "OK");
            }
            catch (Exception e)
            {
                Debug.LogError($"[ZipInstaller] Error: {e}");
                EditorUtility.DisplayDialog("部署失败", $"流程出错: {e.Message}", "关闭");
            }
            finally
            {
                // 6. 清理临时 Zip 文件 (无论成功失败都要做)
                if (File.Exists(tempZipPath)) File.Delete(tempZipPath);
                EditorUtility.ClearProgressBar();
                isProcessing = false;
            }
        }

        // --- 核心步骤实现 ---

        /// <summary>
        ///     Step 1: 通过 API 获取文件的 download_url (兼容私有仓库)
        /// </summary>
        private async Task<string> GetGitHubDownloadUrl()
        {
            var encodedPath = Uri.EscapeUriString(RemoteZipPath);
            var apiUrl = $"https://api.github.com/repos/{GithubOwner}/{GithubRepo}/contents/{encodedPath}";

            using (var request = UnityWebRequest.Get(apiUrl))
            {
                if (!string.IsNullOrEmpty(_personalAccessToken))
                    request.SetRequestHeader("Authorization", $"token {_personalAccessToken}");
                request.SetRequestHeader("User-Agent", "Unity-Plugin");

                var operation = request.SendWebRequest();
                while (!operation.isDone) await Task.Yield();

                if (request.result != UnityWebRequest.Result.Success)
                    throw new Exception($"【网络错误】无法获取文件信息: {request.error}");

                // 简单解析 JSON 获取 download_url
                // 这种轻量级解析比引入 JsonUtility Wrapper 更快
                var json = request.downloadHandler.text;
                var keyword = "\"download_url\":\"";
                var start = json.IndexOf(keyword) + keyword.Length;
                var end = json.IndexOf("\"", start);

                if (start < keyword.Length || end == -1)
                    throw new Exception("解析 GitHub API 响应失败，未找到 download_url");

                // GitHub 返回的 URL 是转义过的 Unicode，通常可以直接用
                return json.Substring(start, end - start);
            }
        }

        /// <summary>
        ///     Step 2: 下载大文件 (使用 DownloadHandlerFile 节省内存)
        /// </summary>
        private async Task DownloadFileAsync(string url, string savePath)
        {
            using (var request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET))
            {
                // 重要：使用 DownloadHandlerFile 可以直接流式写入硬盘，防止大 Zip 撑爆内存
                request.downloadHandler = new DownloadHandlerFile(savePath);

                if (!string.IsNullOrEmpty(_personalAccessToken) && url.Contains("api.github.com"))
                    // 如果 download_url 指向 raw.githubusercontent 通常不需要 token，
                    // 但如果是 API 代理链接则需要。通常带上比较保险。
                    request.SetRequestHeader("Authorization", $"token {_personalAccessToken}");

                var operation = request.SendWebRequest();

                while (!operation.isDone)
                {
                    EditorUtility.DisplayProgressBar("下载资源包",
                        $"下载中... {request.downloadProgress * 100:F0}%",
                        request.downloadProgress);
                    await Task.Yield();
                }

                if (request.result != UnityWebRequest.Result.Success)
                    throw new Exception($"下载失败: {request.error}");
            }
        }

        /// <summary>
        ///     Step 3: 清空并重建目标目录
        /// </summary>
        private void PrepareDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                var confirm = EditorUtility.DisplayDialog("覆盖警告",
                    $"目标目录 '{_localTargetDirectory}' 已存在，即将清空并覆盖。\n确认操作？", "覆盖", "取消");
                if (!confirm) throw new Exception("用户取消操作");

                Directory.Delete(path, true);
            }

            Directory.CreateDirectory(path);
        }

        /// <summary>
        ///     Step 4: 解压 Zip (System.IO.Compression)
        /// </summary>
        private void ExtractZip(string zipPath, string extractPath)
        {
            EditorUtility.DisplayProgressBar("解压中", "正在解压资源...", 1f);

            // 必须引用 System.IO.Compression.FileSystem 程序集
            // 如果这里报错，请在 VS 里引用 System.IO.Compression.FileSystem
            // 或者在 Unity Player Settings -> Api Compatibility Level 设为 .NET 4.x

            try
            {
                ZipFile.ExtractToDirectory(zipPath, extractPath);
            }
            catch (Exception ex)
            {
                throw new Exception($"解压失败 (可能是Zip损坏): {ex.Message}");
            }
        }

        // 获取一个安全的临时路径
        private string GetTempZipPath()
        {
            return Path.Combine(Application.temporaryCachePath, "RepoDownloadCache.zip");
        }
    }
}