using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiveSplit.FEAR
{
    class GameMemory
    {
        bool isVersion1_0 = false;

        public enum SplitArea : int
        {
            None,
            C01Inception,
            C02Initiation,
            C03EscalationP1,
            C03EscalationP2,
            C03EscalationP3,
            C03EscalationP4,
            C04InfiltrationP1,
            C04InfiltrationP2,
            C05ExtractionP1,
            C05ExtractionP2,
            C06InterceptionP1,
            C06InterceptionP2,
            C06InterceptionP3,
            C06InterceptionP4,
            C07Redirection,
            C08DesolationP1,
            C08DesolationP2,
            C09IncursionP1,
            C09IncursionP2,
            C10Revelation,
            C11Retaliation,
        }

        //public event EventHandler OnFirstLevelLoading;
        public event EventHandler OnPlayerGainedControl;
        public event EventHandler OnLoadStarted;
        public event EventHandler OnLoadFinished;
        public delegate void SplitCompletedEventHandler(object sender, SplitArea type, uint frame);
        public event SplitCompletedEventHandler OnSplitCompleted;

        private Task _thread;
        private CancellationTokenSource _cancelSource;
        private SynchronizationContext _uiThread;
        private List<int> _ignorePIDs;
        private FEARSettings _settings;

        private DeepPointer _isLoadingPtr;
        private DeepPointer _levelNamePtr;

        private static class LevelName
        {
            public const string C01Inception = "Worlds\\Release\\Intro.World00p";
            public const string C02Initiation = "Worlds\\Release\\Docks.World00p";
            public const string C03EscalationP1 = "Worlds\\Release\\WTF_Entry.World00p";
            public const string C03EscalationP2 = "Worlds\\Release\\WTF_Ambush.World00p";
            public const string C03EscalationP3 = "Worlds\\Release\\Moody.World00p";
            public const string C03EscalationP4 = "Worlds\\Release\\WTF_Exfil.World00p";
            public const string C04InfiltrationP1 = "Worlds\\Release\\ATC_Roof.World00p";
            public const string C04InfiltrationP2 = "Worlds\\Release\\Admin.World00p";
            public const string C05ExtractionP1 = "Worlds\\Release\\Bishop_Rescue.World00p";
            public const string C05ExtractionP2 = "Worlds\\Release\\Bishop_Evac.World00p";
            public const string C06InterceptionP1 = "Worlds\\Release\\Mapes_Elevator.World00p";
            public const string C06InterceptionP2 = "Worlds\\Release\\Badge.World00p";
            public const string C06InterceptionP3 = "Worlds\\Release\\Hives.World00p";
            public const string C07RedirectionP1 = "Worlds\\Release\\Alice.World00p";
            public const string C07RedirectionP2 = "Worlds\\Release\\Getting_Out.World00p";
            public const string C08DesolationP1 = "Worlds\\Release\\Wades.World00p";
            public const string C08DesolationP2 = "Worlds\\Release\\Factory.World00p";
            public const string C09IncursionP1 = "Worlds\\Release\\Facility_Upper.World00p";
            public const string C09IncursionP2 = "Worlds\\Release\\Facility_Bypass.World00p";
            public const string C10Revelation = "Worlds\\Release\\Vault.World00p";
            public const string C11Retaliation = "Worlds\\Release\\Alma.World00p";
            public const string Epilogue = "Worlds\\Release\\Aftermath.World00p";
        }

        private enum Cutscenes
        {

        }

        private enum ExpectedDllSizes
        {
            FEARv1_0 = 10059776,
            FEARGOG = 1691648,
            FEARSteam = 29642752,
        }

        public bool[] splitStates { get; set; }

        public void resetSplitStates()
        {
            for (int i = 0; i <= (int)SplitArea.C11Retaliation; i++)
            {
                splitStates[i] = false;
            }

        }

        public GameMemory(FEARSettings componentSettings)
        {
            _settings = componentSettings;
            splitStates = new bool[(int)SplitArea.C11Retaliation + 1];

            resetSplitStates();

            _ignorePIDs = new List<int>();
        }

        public void StartMonitoring()
        {
            if (_thread != null && _thread.Status == TaskStatus.Running)
            {
                throw new InvalidOperationException();
            }
            if (!(SynchronizationContext.Current is WindowsFormsSynchronizationContext))
            {
                throw new InvalidOperationException("SynchronizationContext.Current is not a UI thread.");
            }

            _uiThread = SynchronizationContext.Current;
            _cancelSource = new CancellationTokenSource();
            _thread = Task.Factory.StartNew(MemoryReadThread);
        }

        public void Stop()
        {
            if (_cancelSource == null || _thread == null || _thread.Status != TaskStatus.Running)
            {
                return;
            }

            _cancelSource.Cancel();
            _thread.Wait();
        }

        void MemoryReadThread()
        {
            Debug.WriteLine("[NoLoads] MemoryReadThread");

            while (!_cancelSource.IsCancellationRequested)
            {
                try
                {
                    Debug.WriteLine("[NoLoads] Waiting for fear.exe...");

                    Process game;
                    while ((game = GetGameProcess()) == null)
                    {
                        Thread.Sleep(250);
                        if (_cancelSource.IsCancellationRequested)
                        {
                            return;
                        }
                    }

                    Debug.WriteLine("[NoLoads] Got games process!");

                    uint frameCounter = 0;

                    bool prevIsLoading = false;
                    string prevStreamGroupId = String.Empty;
                    string streamGroupId = String.Empty;

                    bool loadingStarted = false;

                    while (!game.HasExited)
                    {
                        bool isLoading;
                        if (!isVersion1_0)
                            _isLoadingPtr.Deref(game, out isLoading);
                        else
                            isLoading = Convert.ToBoolean(TrainerRead.ReadByte("FEAR", 0x1018CB3C));    //Nice mess, eh?
                        

                        string streamGroupIdCheck = String.Empty;
                        _levelNamePtr.Deref(game, out streamGroupIdCheck, 55);
                        if(streamGroupIdCheck != String.Empty)
                        {
                            streamGroupId = streamGroupIdCheck;                             //Since during loading it changes to null
                        }

                        if (streamGroupId != prevStreamGroupId)
                        {
                            if (prevStreamGroupId == LevelName.C01Inception && streamGroupId == LevelName.C02Initiation)
                            {
                                Split(SplitArea.C01Inception, frameCounter);
                            }
                            else if (prevStreamGroupId == LevelName.C02Initiation && streamGroupId == LevelName.C03EscalationP1)
                            {
                                Split(SplitArea.C02Initiation, frameCounter);
                            }
                            else if (prevStreamGroupId == LevelName.C03EscalationP1 && streamGroupId == LevelName.C03EscalationP2)
                            {
                                Split(SplitArea.C03EscalationP1, frameCounter);
                            }
                            else if (prevStreamGroupId == LevelName.C03EscalationP2 && streamGroupId == LevelName.C03EscalationP3)
                            {
                                Split(SplitArea.C03EscalationP2, frameCounter);
                            }
                            else if (prevStreamGroupId == LevelName.C03EscalationP3 && streamGroupId == LevelName.C03EscalationP4)
                            {
                                Split(SplitArea.C03EscalationP3, frameCounter);
                            }
                            else if (prevStreamGroupId == LevelName.C03EscalationP4 && streamGroupId == LevelName.C04InfiltrationP1)
                            {
                                Split(SplitArea.C03EscalationP4, frameCounter);
                            }
                            else if (prevStreamGroupId == LevelName.C04InfiltrationP1 && streamGroupId == LevelName.C04InfiltrationP2)
                            {
                                Split(SplitArea.C04InfiltrationP1, frameCounter);
                            }
                            else if (prevStreamGroupId == LevelName.C04InfiltrationP2 && streamGroupId == LevelName.C05ExtractionP1)
                            {
                                Split(SplitArea.C04InfiltrationP2, frameCounter);
                            }
                            else if (prevStreamGroupId == LevelName.C05ExtractionP1 && streamGroupId == LevelName.C05ExtractionP2)
                            {
                                Split(SplitArea.C05ExtractionP1, frameCounter);
                            }
                            else if (prevStreamGroupId == LevelName.C05ExtractionP2 && streamGroupId == LevelName.C06InterceptionP1)
                            {
                                Split(SplitArea.C05ExtractionP2, frameCounter);
                            }
                            else if (prevStreamGroupId == LevelName.C06InterceptionP1 && streamGroupId == LevelName.C06InterceptionP2)
                            {
                                Split(SplitArea.C06InterceptionP1, frameCounter);
                            }
                            else if (prevStreamGroupId == LevelName.C06InterceptionP2 && streamGroupId == LevelName.C06InterceptionP3)
                            {
                                Split(SplitArea.C06InterceptionP2, frameCounter);
                            }
                            else if (prevStreamGroupId == LevelName.C06InterceptionP3 && streamGroupId == LevelName.C07RedirectionP1)
                            {
                                Split(SplitArea.C06InterceptionP3, frameCounter);
                            }
                            else if (prevStreamGroupId == LevelName.C07RedirectionP1 && streamGroupId == LevelName.C07RedirectionP2)
                            {
                                Split(SplitArea.C06InterceptionP4, frameCounter);
                            }
                            else if (prevStreamGroupId == LevelName.C07RedirectionP2 && streamGroupId == LevelName.C08DesolationP1)
                            {
                                Split(SplitArea.C07Redirection, frameCounter);
                            }
                            else if (prevStreamGroupId == LevelName.C08DesolationP1 && streamGroupId == LevelName.C08DesolationP2)
                            {
                                Split(SplitArea.C08DesolationP1, frameCounter);
                            }
                            else if (prevStreamGroupId == LevelName.C08DesolationP2 && streamGroupId == LevelName.C09IncursionP1)
                            {
                                Split(SplitArea.C08DesolationP2, frameCounter);
                            }
                            else if (prevStreamGroupId == LevelName.C09IncursionP1 && streamGroupId == LevelName.C09IncursionP2)
                            {
                                Split(SplitArea.C09IncursionP1, frameCounter);
                            }
                            else if (prevStreamGroupId == LevelName.C09IncursionP2 && streamGroupId == LevelName.C10Revelation)
                            {
                                Split(SplitArea.C09IncursionP2, frameCounter);
                            }
                            else if (prevStreamGroupId == LevelName.C10Revelation && streamGroupId == LevelName.C11Retaliation)
                            {
                                Split(SplitArea.C10Revelation, frameCounter);
                            }
                            else if (prevStreamGroupId == LevelName.C11Retaliation && streamGroupId == LevelName.Epilogue)
                            {
                                Split(SplitArea.C11Retaliation, frameCounter);
                            }

                        }

                        if (isLoading != prevIsLoading)
                        {
                            if (isLoading)
                            {
                                Debug.WriteLine(String.Format("[NoLoads] Load Start - {0}", frameCounter));

                                loadingStarted = true;

                                // pause game timer
                                _uiThread.Post(d =>
                                {
                                    if (this.OnLoadStarted != null)
                                    {
                                        this.OnLoadStarted(this, EventArgs.Empty);
                                    }
                                }, null);

                                if (streamGroupId == LevelName.C01Inception)
                                {
                                    // reset game timer
                                    /*_uiThread.Post(d =>
                                    {
                                        if (this.OnFirstLevelLoading != null)
                                        {
                                            this.OnFirstLevelLoading(this, EventArgs.Empty);
                                        }
                                    }, null);*/
                                }
                            }
                            else
                            {
                                Debug.WriteLine(String.Format("[NoLoads] Load End - {0}", frameCounter));
                                if (loadingStarted)
                                {
                                    loadingStarted = false;

                                    // unpause game timer
                                    _uiThread.Post(d =>
                                    {
                                        if (this.OnLoadFinished != null)
                                        {
                                            this.OnLoadFinished(this, EventArgs.Empty);
                                        }
                                    }, null);

                                    if (streamGroupId == LevelName.C01Inception)
                                    {
                                        // start game timer
                                        _uiThread.Post(d =>
                                        {
                                            if (this.OnPlayerGainedControl != null)
                                            {
                                                this.OnPlayerGainedControl(this, EventArgs.Empty);
                                            }
                                        }, null);
                                    }
                                }
                            }
                        }
                        prevIsLoading = isLoading;

                        Debug.WriteLineIf(streamGroupId != prevStreamGroupId, String.Format("[NoLoads] streamGroupId changed from {0} to {1} - {2}", prevStreamGroupId, streamGroupId, frameCounter));
                        prevStreamGroupId = streamGroupId;
                        
                        frameCounter++;

                        Thread.Sleep(15);

                        if (_cancelSource.IsCancellationRequested)
                        {
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                    Thread.Sleep(1000);
                }
            }
        }

        private void Split(SplitArea split, uint frame)
        {
            Debug.WriteLine(String.Format("[NoLoads] split {0} - {1}", split, frame));
            _uiThread.Post(d =>
            {
                if (this.OnSplitCompleted != null)
                {
                    this.OnSplitCompleted(this, split, frame);
                }
            }, null);
        }

        Process GetGameProcess()
        {
            Process game = Process.GetProcesses().FirstOrDefault(p => p.ProcessName.ToLower() == "fear" && !p.HasExited && !_ignorePIDs.Contains(p.Id));
            if (game == null)
            {
                return null;
            }

            if(game.MainModule.ModuleMemorySize == (int)ExpectedDllSizes.FEARSteam)
            {
                _isLoadingPtr = new DeepPointer(0x00176FCC, 0x10, 0xE0, 0x8, 0x728); // == 1 if a loadscreen is happening
                _levelNamePtr = new DeepPointer(0x16C036);
                isVersion1_0 = false;
            }
            else if (game.MainModule.ModuleMemorySize == (int)ExpectedDllSizes.FEARGOG)
            {
                _isLoadingPtr = new DeepPointer(0x00176FCC, 0x10, 0xE0, 0x8, 0x728); // == 1 if a loadscreen is happening
                _levelNamePtr = new DeepPointer(0x16C036);
                isVersion1_0 = false;
            }
            else if(game.MainModule.ModuleMemorySize == (int)ExpectedDllSizes.FEARv1_0)
            {
                _levelNamePtr = new DeepPointer(0x163FCE);
                isVersion1_0 = true;
            }
            else
            {
                _ignorePIDs.Add(game.Id);
                _uiThread.Send(d => MessageBox.Show("Unexpected game version. FEAR v1.0 or 1.08 (Steam) is required.", "LiveSplit.FEAR",
                    MessageBoxButtons.OK, MessageBoxIcon.Error), null);
                return null;
            }

            return game;
        }
    }
}
