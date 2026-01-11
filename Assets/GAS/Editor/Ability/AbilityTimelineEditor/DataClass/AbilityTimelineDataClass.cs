using System.Collections.Generic;

namespace GAS.Editor
{
    public class EdtTimelineAbility
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int LifeTime { get; set; }
        public bool ManualEndAbility { get; set; }
        public List<EdtTrack> Tracks { get; set; } = new List<EdtTrack>();
    }

    public class EdtTrack
    {
        public string Name { get; set; }
        public List<EdtAbilityTask> AbilityTasks { get; set; } = new List<EdtAbilityTask>();
    }

    public class EdtAbilityTask
    {
        public string TaskType { get; set; }
        public List<string> Parameters { get; set; } = new List<string>();
    }
}