using LiveSplit.Model;
using LiveSplit.TimeFormatters;
using LiveSplit.UI.Components;
using LiveSplit.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml;
using System.Windows.Forms;
using System.Diagnostics;

namespace LiveSplit.FEAR
{
    class FEARComponent : LogicComponent
    {
        public override string ComponentName
        {
            get { return "FEAR"; }
        }

        public FEARSettings Settings { get; set; }

        public bool Disposed { get; private set; }
        public bool IsLayoutComponent { get; private set; }

        private TimerModel _timer;
        private GameMemory _gameMemory;
        private LiveSplitState _state;

        public FEARComponent(LiveSplitState state, bool isLayoutComponent)
        {
            _state = state;
            this.IsLayoutComponent = isLayoutComponent;

            this.Settings = new FEARSettings();

           _timer = new TimerModel { CurrentState = state };

            _gameMemory = new GameMemory(this.Settings);
            _gameMemory.OnFirstLevelLoading += gameMemory_OnFirstLevelLoading;
            _gameMemory.OnPlayerGainedControl += gameMemory_OnPlayerGainedControl;
            _gameMemory.OnLoadStarted += gameMemory_OnLoadStarted;
            _gameMemory.OnLoadFinished += gameMemory_OnLoadFinished;
            _gameMemory.OnSplitCompleted += gameMemory_OnSplitCompleted;
            state.OnStart += State_OnStart;
            _gameMemory.StartMonitoring();
        }

        public override void Dispose()
        {
            this.Disposed = true;

            _state.OnStart -= State_OnStart;

            if (_gameMemory != null)
            {
                _gameMemory.Stop();
            }

        }

        void State_OnStart(object sender, EventArgs e)
        {
            _gameMemory.resetSplitStates();
        }

        void gameMemory_OnFirstLevelLoading(object sender, EventArgs e)
        {
            if (this.Settings.AutoReset)
            {
                _timer.Reset();
            }
        }

        void gameMemory_OnPlayerGainedControl(object sender, EventArgs e)
        {
            if (this.Settings.AutoStart)
            {
                _timer.Start();
            }
        }

        void gameMemory_OnLoadStarted(object sender, EventArgs e)
        {
            _state.IsGameTimePaused = true;
        }

        void gameMemory_OnLoadFinished(object sender, EventArgs e)
        {
            _state.IsGameTimePaused = false;
        }

        void gameMemory_OnSplitCompleted(object sender, GameMemory.SplitArea split, uint frame)
        {
            Debug.WriteLineIf(split != GameMemory.SplitArea.None, String.Format("[NoLoads] Trying to split {0}, State: {1} - {2}", split, _gameMemory.splitStates[(int)split], frame));
            if (_state.CurrentPhase == TimerPhase.Running && !_gameMemory.splitStates[(int)split] &&
                ((split == GameMemory.SplitArea.C01Inception && this.Settings.sC01Inception) ||
                (split == GameMemory.SplitArea.C02Initiation && this.Settings.sC02Initiation) ||
                (split == GameMemory.SplitArea.C03EscalationP1 && this.Settings.sC03EscalationP1) ||
                (split == GameMemory.SplitArea.C03EscalationP2 && this.Settings.sC03EscalationP2) ||
                (split == GameMemory.SplitArea.C03EscalationP3 && this.Settings.sC03EscalationP3) ||
                (split == GameMemory.SplitArea.C03EscalationP4 && this.Settings.sC03EscalationP4) ||
                (split == GameMemory.SplitArea.C04InfiltrationP1 && this.Settings.sC04InfiltrationP1) ||
                (split == GameMemory.SplitArea.C04InfiltrationP2 && this.Settings.sC04InfiltrationP2) ||
                (split == GameMemory.SplitArea.C05ExtractionP1 && this.Settings.sC05ExtractionP1) ||
                (split == GameMemory.SplitArea.C05ExtractionP2 && this.Settings.sC05ExtractionP2) ||
                (split == GameMemory.SplitArea.C06InterceptionP1 && this.Settings.sC06InterceptionP1) ||
                (split == GameMemory.SplitArea.C06InterceptionP2 && this.Settings.sC06InterceptionP2) ||
                (split == GameMemory.SplitArea.C06InterceptionP3 && this.Settings.sC06InterceptionP3) ||
                (split == GameMemory.SplitArea.C06InterceptionP4 && this.Settings.sC07RedirectionP1) ||
                (split == GameMemory.SplitArea.C07Redirection && this.Settings.sC07RedirectionP2) ||
                (split == GameMemory.SplitArea.C08DesolationP1 && this.Settings.sC08DesolationP1) ||
                (split == GameMemory.SplitArea.C08DesolationP2 && this.Settings.sC08DesolationP2) ||
                (split == GameMemory.SplitArea.C09IncursionP1 && this.Settings.sC09IncursionP1) ||
                (split == GameMemory.SplitArea.C09IncursionP2 && this.Settings.sC09IncursionP2) ||
                (split == GameMemory.SplitArea.C10Revelation && this.Settings.sC10Revelation) ||
                (split == GameMemory.SplitArea.C11Retaliation && this.Settings.sC11Retaliation)))
            {
                Trace.WriteLine(String.Format("[NoLoads] {0} Split - {1}", split, frame));
                _timer.Split();
                _gameMemory.splitStates[(int)split] = true;
            }
        }

        public override XmlNode GetSettings(XmlDocument document)
        {
            return this.Settings.GetSettings(document);
        }

        public override Control GetSettingsControl(LayoutMode mode)
        {
            return this.Settings;
        }

        public override void SetSettings(XmlNode settings)
        {
            this.Settings.SetSettings(settings);
        }

        public override void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode) { }
        //public override void RenameComparison(string oldName, string newName) { }
    }
}
