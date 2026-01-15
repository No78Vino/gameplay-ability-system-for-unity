using System.Collections.Generic;
using System.IO;
using System.Linq;
using OfficeOpenXml;

namespace GAS.Editor
{
    public static class GasAbilityTimelineXlsxReadWrite
    {
        private static List<EdtTimelineAbility> _timelineAbilities;

        public static void LoadTimelineAbilities()
        {
            var setting = GASSettingAsset.LoadOrCreate();
            // 假设GASSettingAsset已添加PathOfExcelTimelineAbility属性
            var filePath = setting.PathOfExcelTimelineAbility;

            _timelineAbilities = new List<EdtTimelineAbility>();

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets[1]; // 使用第一个工作表
                var row = 6; // 数据从第6行开始
                var safeCnt = 99999;
                EdtTimelineAbility currentAbility = null;
                EdtTrack currentEdtTrack = null;

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
                    var taskTypeCell = worksheet.Cells[row, 9].Value;

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
                            if (currentEdtTrack != null)
                            {
                                currentAbility.Tracks.Add(currentEdtTrack);
                            }

                            _timelineAbilities.Add(currentAbility);
                        }

                        // 创建新技能
                        currentAbility = new EdtTimelineAbility
                        {
                            Id = int.Parse(idCell.ToString()),
                            Name = nameCell?.ToString() ?? "",
                            LifeTime = lifeTimeCell != null ? int.Parse(lifeTimeCell.ToString()) : 0,
                            ManualEndAbility = manualEndCell != null && bool.Parse(manualEndCell.ToString())
                        };
                        currentEdtTrack = null;
                    }

                    // 处理新轨道
                    if (trackNameCell != null)
                    {
                        if (currentAbility != null && currentEdtTrack != null)
                            currentAbility.Tracks.Add(currentEdtTrack);

                        currentEdtTrack = new EdtTrack
                        {
                            Name = trackNameCell.ToString()
                        };
                    }

                    // 处理任务
                    if (taskTypeCell != null)
                    {
                        if (currentEdtTrack == null)
                        {
                            // 如果轨道名为空，创建默认轨道（根据数据逻辑，通常不会发生）
                            currentEdtTrack = new EdtTrack { Name = "Default" };
                            if (currentAbility != null) currentAbility.Tracks.Add(currentEdtTrack);
                        }

                        var task = new EdtAbilityTask
                        {
                            TaskType = taskTypeCell.ToString(),
                            StartTime = startTimeCell != null ? int.Parse(startTimeCell.ToString()) : 0,
                            EndTime = endTimeCell != null ? int.Parse(endTimeCell.ToString()) : 0,
                        };

                        // 读取参数（假设最多10个参数，从第10列到第19列）
                        for (var col = 10; col <= 19; col++)
                        {
                            var paramCell = worksheet.Cells[row, col].Value;
                            if (paramCell != null) task.Parameters.Add(paramCell.ToString());
                        }

                        currentEdtTrack.AbilityTasks.Add(task);
                    }

                    row++;
                }

                // 添加最后一个技能
                if (currentAbility != null)
                {
                    if (currentEdtTrack != null) currentAbility.Tracks.Add(currentEdtTrack);
                    _timelineAbilities.Add(currentAbility);
                }
            }
        }

        public static List<EdtTimelineAbility> GetTimelineAbilities(bool forceReload = false)
        {
            if (_timelineAbilities == null||forceReload)
                LoadTimelineAbilities();
            return _timelineAbilities;
        }

        public static List<string> GetTimelineAbilityIDList()
        {
            var abilities = GetTimelineAbilities();

            return abilities.Select(ability => ability.Id.ToString()).ToList();
        }
        
        public static EdtTimelineAbility GetTimelineAbility(string id)
        {
            var abilities = GetTimelineAbilities();

            return abilities.FirstOrDefault(ability => ability.Id.ToString() == id);
        }
    }
}