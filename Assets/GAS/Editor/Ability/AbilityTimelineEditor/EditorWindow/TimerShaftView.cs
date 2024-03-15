
#if UNITY_EDITOR
namespace GAS.Editor
{
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;
    public class TimerShaftView
    {
        private readonly VisualElement _root;

        private int _dottedLineFrameIndex = -1;

        private Rect _dragItemPreviewRect;

        private bool _showDragItemPreview;
        private VisualElement contentViewPort;
        private IMGUIContainer DottedLine;
        private IMGUIContainer DragItemPreview;
        private IMGUIContainer FinishLine;
        private IMGUIContainer SelectLine;

        private bool timerShaftMouseIn;

        public TimerShaftView(VisualElement root)
        {
            _root = root;
            InitTimerShaft();
        }

        private static AbilityTimelineEditorWindow EditorInst => AbilityTimelineEditorWindow.Instance;
        private static AbilityTimelineEditorConfig Config => AbilityTimelineEditorWindow.Instance.Config;

        public IMGUIContainer TimerShaft { get; private set; }
        public VisualElement TimeLineContainer { get; private set; }
        public ScrollView MainContent { get; private set; }

        public int DottedLineFrameIndex
        {
            get => _dottedLineFrameIndex;
            set
            {
                if (_dottedLineFrameIndex == value) return;
                _dottedLineFrameIndex = value;
                var showDottedLine = _dottedLineFrameIndex * Config.FrameUnitWidth >= EditorInst.CurrentFramePos &&
                                     _dottedLineFrameIndex * Config.FrameUnitWidth <
                                     EditorInst.CurrentFramePos + TimerShaft.contentRect.width;
                DottedLine.style.display = showDottedLine ? DisplayStyle.Flex : DisplayStyle.None;
                DottedLine.MarkDirtyRepaint();
            }
        }

        public bool ShowDragItemPreview
        {
            get => _showDragItemPreview;
            set
            {
                if (_showDragItemPreview == value) return;
                _showDragItemPreview = value;
                DragItemPreview.MarkDirtyRepaint();
            }
        }

        public Rect DragItemPreviewRect
        {
            get => _dragItemPreviewRect;
            set
            {
                _dragItemPreviewRect = value;
                DragItemPreview.MarkDirtyRepaint();
            }
        }

        private void InitTimerShaft()
        {
            var mainContainer = _root.Q<ScrollView>("MainContent");
            MainContent = mainContainer;
            TimeLineContainer = mainContainer.Q<VisualElement>("unity-content-container");
            contentViewPort = mainContainer.Q<VisualElement>("unity-content-viewport");

            TimerShaft = _root.Q<IMGUIContainer>(nameof(TimerShaft));
            TimerShaft.onGUIHandler = OnTimerShaftGUI;
            TimerShaft.RegisterCallback<WheelEvent>(OnWheelEvent);
            TimerShaft.RegisterCallback<MouseDownEvent>(OnTimerShaftMouseDown);
            TimerShaft.RegisterCallback<MouseMoveEvent>(OnTimerShaftMouseMove);
            TimerShaft.RegisterCallback<MouseUpEvent>(OnTimerShaftMouseUp);
            TimerShaft.RegisterCallback<MouseOutEvent>(OnTimerShaftMouseOut);

            SelectLine = _root.Q<IMGUIContainer>(nameof(SelectLine));
            SelectLine.onGUIHandler = OnSelectLineGUI;

            FinishLine = _root.Q<IMGUIContainer>(nameof(FinishLine));
            FinishLine.onGUIHandler = OnFinishLineGUI;

            DottedLine = _root.Q<IMGUIContainer>(nameof(DottedLine));
            DottedLine.onGUIHandler = OnDottedLineGUI;

            DragItemPreview = _root.Q<IMGUIContainer>(nameof(DragItemPreview));
            DragItemPreview.onGUIHandler = OnDragItemPreviewGUI;
        }

        public void RefreshTimerDraw()
        {
            TimerShaft.MarkDirtyRepaint();
            SelectLine.MarkDirtyRepaint();
            FinishLine.MarkDirtyRepaint();
        }

        private void OnTimerShaftMouseDown(MouseDownEvent evt)
        {
            timerShaftMouseIn = true;
            EditorInst.CurrentSelectFrameIndex = GetFrameIndexByMouse(evt.localMousePosition.x);
        }

        private void OnTimerShaftMouseUp(MouseUpEvent evt)
        {
            timerShaftMouseIn = false;
        }

        private void OnTimerShaftMouseMove(MouseMoveEvent evt)
        {
            if (timerShaftMouseIn) EditorInst.CurrentSelectFrameIndex = GetFrameIndexByMouse(evt.localMousePosition.x);
        }

        private void OnTimerShaftMouseOut(MouseOutEvent evt)
        {
            timerShaftMouseIn = false;
        }

        public int GetFrameIndexByMouse(float x)
        {
            return GetFrameIndexByPosition(x + EditorInst.CurrentFramePos);
        }

        public int GetFrameIndexByPosition(float x)
        {
            return Mathf.RoundToInt(x) / Config.FrameUnitWidth;
        }

        private void OnSelectLineGUI()
        {
            if (EditorInst.CurrentSelectFramePos >= EditorInst.CurrentFramePos &&
                EditorInst.CurrentSelectFramePos < EditorInst.CurrentFramePos + TimerShaft.contentRect.width)
            {
                Handles.BeginGUI();
                Handles.color = Color.green;
                var length = contentViewPort.contentRect.height + TimerShaft.contentRect.height;
                var x = EditorInst.CurrentSelectFramePos - EditorInst.CurrentFramePos;
                x = Mathf.Max(x, 1);
                Handles.DrawLine(new Vector3(x, 0), new Vector3(x, length));
                Handles.EndGUI();
            }
        }

        private void OnFinishLineGUI()
        {
            if (EditorInst.CurrentEndFramePos >= EditorInst.CurrentFramePos &&
                EditorInst.CurrentEndFramePos < EditorInst.CurrentFramePos + TimerShaft.contentRect.width)
            {
                Handles.BeginGUI();
                Handles.color = Color.red;
                var length = contentViewPort.contentRect.height + TimerShaft.contentRect.height;
                var x = EditorInst.CurrentEndFramePos - EditorInst.CurrentFramePos;
                x = Mathf.Max(x, 1);
                Handles.DrawLine(new Vector3(x, 0), new Vector3(x, length));
                Handles.EndGUI();
            }
        }

        private void OnDottedLineGUI()
        {
            var dottedLinePos = DottedLineFrameIndex * Config.FrameUnitWidth;
            if (dottedLinePos >= EditorInst.CurrentFramePos &&
                dottedLinePos < EditorInst.CurrentFramePos + TimerShaft.contentRect.width)
            {
                Handles.BeginGUI();
                Handles.color = new Color(1, 0.5f, 0, 1);
                var length = contentViewPort.contentRect.height + TimerShaft.contentRect.height;
                var x = dottedLinePos - EditorInst.CurrentFramePos;
                x = Mathf.Max(x, 1);

                var lineUnitSize = 10f;
                var lineUnitsCount = 0;
                for (float i = 0; i < length; i += lineUnitSize)
                    if (lineUnitsCount++ % 2 == 0)
                        Handles.DrawLine(new Vector3(x, i), new Vector3(x, i + lineUnitSize));

                Handles.EndGUI();
            }
        }

        private void OnDragItemPreviewGUI()
        {
            if (ShowDragItemPreview)
            {
                Handles.BeginGUI();
                Handles.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                Handles.DrawSolidRectangleWithOutline(DragItemPreviewRect, new Color(0.9f, 0.5f, 0, 0.9f), Color.white);
                Handles.EndGUI();
            }
        }

        private void OnWheelEvent(WheelEvent evt)
        {
            var deltaY = (int)evt.delta.y;
            Config.FrameUnitWidth =
                Mathf.Clamp(Config.FrameUnitWidth - deltaY,
                    AbilityTimelineEditorConfig.StandardFrameUnitWidth,
                    Mathf.RoundToInt(AbilityTimelineEditorConfig.MaxFrameUnitLevel *
                                     AbilityTimelineEditorConfig.StandardFrameUnitWidth));

            // 以鼠标为缩放中心
            // var mousePos = evt.localMousePosition.x;
            // var mouseFrame = GetFrameIndexByMouse(mousePos);
            // var mouseFramePos = mouseFrame * Config.FrameUnitWidth;
            // var deltaFrame = mouseFramePos - EditorInst.CurrentFramePos;
            // var contentWidth = contentViewPort.contentRect.width;
            // var scrollViewWidth = MainContent.worldBound.width;
            // var scrollOffsetDelta = (EditorInst.CurrentFramePos + deltaFrame * Config.FrameUnitWidth) / contentWidth *
            //                         scrollViewWidth;
            // MainContent.scrollOffset = new Vector2(MainContent.scrollOffset.x - scrollOffsetDelta, 0);
            
            RefreshTimerDraw();
            EditorInst.TrackView.UpdateContentSize();
        }

        private void OnTimerShaftGUI()
        {
            Handles.BeginGUI();
            Handles.color = Color.white;

            var rect = TimerShaft.contentRect;
            var tickStep = AbilityTimelineEditorConfig.MaxFrameUnitLevel + 1 -
                           Config.FrameUnitWidth / AbilityTimelineEditorConfig.StandardFrameUnitWidth;
            tickStep /= 2;
            tickStep = Mathf.Max(tickStep, 2);

            var index = Mathf.CeilToInt(EditorInst.CurrentFramePos / Config.FrameUnitWidth);
            var startFrameOffset =
                index > 0 ? Config.FrameUnitWidth - EditorInst.CurrentFramePos % Config.FrameUnitWidth : 0;

            var minDrawStep = AbilityTimelineEditorConfig.MinTimerShaftFrameDrawStep;
            var tooSmall = Config.FrameUnitWidth < minDrawStep;
            var drawStepFrame = tooSmall ? Mathf.CeilToInt(minDrawStep / Config.FrameUnitWidth) : 1;
            tickStep *= drawStepFrame;

            for (var i = startFrameOffset; i <= rect.width; i += Config.FrameUnitWidth)
            {
                var isDraw = !tooSmall || index % drawStepFrame == 0;
                if (isDraw)
                {
                    var isTick = index % tickStep == 0;
                    var x = i;
                    var startY = isTick ? rect.height * 0.5f : rect.height * 0.85f;
                    var endY = rect.height;
                    Handles.DrawLine(new Vector3(x, startY), new Vector3(x, endY));

                    if (isTick)
                    {
                        var frameStr = index.ToString();
                        Handles.Label(new Vector3(x, rect.height * 0.3f), frameStr);
                    }
                }

                index++;
            }

            Handles.EndGUI();
        }
    }
}
#endif