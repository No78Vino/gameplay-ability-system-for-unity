using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GAS.Runtime;
using OfficeOpenXml;

namespace GAS.Editor
{
    public static class GasAbilityTimelineXlsxReadWrite
    {
        private static List<XParamTimeline> _timelineAbilities;

        public static void LoadTimelineAbilities()
        {
            var setting = GASSettingAsset.LoadOrCreate();
            // 假设GASSettingAsset已添加PathOfExcelTimelineAbility属性
            var filePath = setting.PathOfExcelTimelineAbility;

            _timelineAbilities = new List<XParamTimeline>();

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets[1]; // 使用第一个工作表
                var row = 6; // 数据从第6行开始
                var safeCnt = 99999;
                XParamTimeline currentAbility = null;
                Track currentTrack = null;

                while (safeCnt > 0 && row <= worksheet.Dimension.End.Row)
                {
                    safeCnt--;

                    // 读取当前行的关键单元格
                    var idCell = worksheet.Cells[row, 2].Value;
                    var nameCell = worksheet.Cells[row, 3].Value;
                    var lifeTimeCell = worksheet.Cells[row, 4].Value;
                    var manualEndCell = worksheet.Cells[row, 5].Value;
                    var trackNameCell = worksheet.Cells[row, 6].Value;
                    var startTimeCell = worksheet.Cells[row, 7].Value;
                    var endTimeCell = worksheet.Cells[row, 8].Value;
                    var taskNameCell = worksheet.Cells[row, 9].Value;
                    var taskTypeCell = worksheet.Cells[row, 10].Value;

                    // 如果所有关键单元格都为空，结束解析
                    if (idCell == null && nameCell == null && lifeTimeCell == null &&
                        manualEndCell == null && trackNameCell == null && taskTypeCell == null)
                        break;

                    // 处理新技能
                    if (idCell != null)
                    {
                        // 保存之前的技能
                        if (currentAbility != null)
                        {
                            if (currentTrack != null) currentAbility.Tracks.Add(currentTrack);

                            _timelineAbilities.Add(currentAbility);
                        }

                        // 创建新技能
                        currentAbility = new XParamTimeline();
                        currentAbility.SetID(int.Parse(idCell.ToString()));
                        currentAbility.SetName(nameCell?.ToString() ?? "");
                        currentAbility.SetLifeTime(lifeTimeCell != null ? int.Parse(lifeTimeCell.ToString()) : 0);
                        currentAbility.SetManualEndAbility(
                            manualEndCell != null && bool.Parse(manualEndCell.ToString()));
                        currentTrack = null;
                    }

                    // 处理新轨道
                    if (trackNameCell != null)
                    {
                        if (currentAbility != null && currentTrack != null)
                            currentAbility.Tracks.Add(currentTrack);

                        currentTrack = new Track
                        {
                            Name = trackNameCell.ToString()
                        };
                    }

                    // 处理任务
                    if (taskTypeCell != null)
                    {
                        if (currentTrack == null)
                        {
                            // 如果轨道名为空，创建默认轨道（根据数据逻辑，通常不会发生）
                            currentTrack = new Track { Name = "Default" };
                            if (currentAbility != null) currentAbility.Tracks.Add(currentTrack);
                        }

                        var task = new TaskClipData
                        {
                            TaskType = taskTypeCell.ToString(),
                            StartTime = startTimeCell != null ? int.Parse(startTimeCell.ToString()) : 0,
                            EndTime = endTimeCell != null ? int.Parse(endTimeCell.ToString()) : 0,
                            Name = taskNameCell?.ToString() ?? ""
                        };

                        // 读取参数（假设最多10个参数，从第10列到第19列）
                        var parameters = new List<object>();
                        for (var col = 11; col <= 20; col++)
                        {
                            var paramCell = worksheet.Cells[row, col].Value;
                            if (paramCell != null)
                                parameters.Add(paramCell);
                        }

                        var param = EditorAbilityHelper.CreateAbilityTaskParameter(taskTypeCell.ToString());
                        param.DecodeExcelData(parameters);
                        task.SetParameter(param);
                        currentTrack.TaskClips.Add(task);
                    }

                    row++;
                }

                // 添加最后一个技能
                if (currentAbility != null)
                {
                    if (currentTrack != null) currentAbility.Tracks.Add(currentTrack);
                    _timelineAbilities.Add(currentAbility);
                }
            }
        }

        public static void Write()
        {
            // 内存中还没加载或没有任何技能，不需要写
            if (_timelineAbilities == null || _timelineAbilities.Count == 0)
                return;

            var setting = GASSettingAsset.LoadOrCreate();
            var filePath = setting.PathOfExcelTimelineAbility;

            // 确保目录存在
            var dir = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var isNewFile = !File.Exists(filePath);
            var fileInfo = new FileInfo(filePath);

            using (var package = isNewFile ? new ExcelPackage() : new ExcelPackage(fileInfo))
            {
                if (isNewFile) return;

                // 复用原有第一个 sheet（里面头几行已有 luban 的定义）
                var worksheet = package.Workbook.Worksheets[1];

                // 清除旧数据：从第 6 行开始都是数据区
                const int dataStartRow = 6;
                if (worksheet.Dimension != null && worksheet.Dimension.End.Row >= dataStartRow)
                {
                    var delCount = worksheet.Dimension.End.Row - dataStartRow + 1;
                    if (delCount > 0)
                        worksheet.DeleteRow(dataStartRow, delCount);
                }

                var row = dataStartRow;

                // 按 XParamTimeline -> Track -> TaskClipData 的层级写回
                foreach (var ability in _timelineAbilities)
                {
                    // 标记：当前 ability 的第一条任务所在行，需要写 ID/Name/LifeTime/ManualEndAbility
                    var firstTrackInAbility = true;

                    // 如果某个技能没有任何 Track/Task，也可以选择至少写一行，只写基础信息；
                    // 下面代码选择：只有存在 Task 时才写行；若你需要“空技能”也写一行，可额外处理。
                    if (ability.Tracks == null || ability.Tracks.Count == 0)
                        continue;

                    foreach (var track in ability.Tracks)
                    {
                        var firstTaskInTrack = true;

                        if (track.TaskClips == null || track.TaskClips.Count == 0)
                            continue;

                        foreach (var task in track.TaskClips)
                        {
                            // ===== 写 Ability 基本信息（只在该技能第一条任务那一行写一次） =====
                            if (firstTrackInAbility && firstTaskInTrack)
                            {
                                worksheet.Cells[row, 2].Value = ability.ID;
                                worksheet.Cells[row, 3].Value = ability.Name;
                                worksheet.Cells[row, 4].Value = ability.LifeTime;
                                worksheet.Cells[row, 5].Value = ability.ManualEndAbility;
                            }

                            // ===== 写 Track 名（只在该轨道第一条任务那一行写一次） =====
                            if (firstTaskInTrack) worksheet.Cells[row, 6].Value = track.Name;

                            // ===== 写 TaskClipData 基本信息，每条任务一行 =====
                            worksheet.Cells[row, 7].Value = task.StartTime;
                            worksheet.Cells[row, 8].Value = task.EndTime;
                            worksheet.Cells[row, 9].Value = task.Name;
                            worksheet.Cells[row, 10].Value = task.TaskType;

                            // ===== 写参数（列 11~20），调用 XParam.EncodeExcelData() =====
                            if (task.Parameter != null)
                            {
                                var paramList = task.Parameter.EncodeExcelData(); // List<object>
                                if (paramList != null)
                                {
                                    // 最多占用 10 列（11~20）
                                    var maxParamCols = 10;
                                    var count = Math.Min(maxParamCols, paramList.Count);
                                    for (var i = 0; i < count; i++) worksheet.Cells[row, 11 + i].Value = paramList[i];
                                }
                            }

                            // 移动到下一行，后续任务继续写
                            row++;
                            firstTaskInTrack = false;
                        }

                        // 当前 ability 已经至少写过一行了，后续 Track 都不再写 ID/Name/LifeTime/ManualEndAbility
                        firstTrackInAbility = false;
                    }
                }

                // 保存 Excel 文件
                package.Save();
            }
        }

        public static List<XParamTimeline> GetTimelineAbilities(bool forceReload = false)
        {
            if (_timelineAbilities == null || forceReload)
                LoadTimelineAbilities();
            return _timelineAbilities;
        }

        public static List<string> GetTimelineAbilityIDList()
        {
            var abilities = GetTimelineAbilities();

            return abilities.Select(ability => ability.ID.ToString()).ToList();
        }

        public static XParamTimeline GetTimelineAbility(string id)
        {
            var abilities = GetTimelineAbilities();

            return abilities.FirstOrDefault(ability => ability.ID.ToString() == id);
        }
    }
}