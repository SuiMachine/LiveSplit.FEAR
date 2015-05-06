using System;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.FEAR
{
    public partial class FEARSettings : UserControl
    {
        public bool AutoReset { get; set; }
        public bool AutoStart { get; set; }
        public bool sC01Inception { get; set; }
        public bool sC02Initiation { get; set; }
        public bool sC03EscalationP1 { get; set; }
        public bool sC03EscalationP2 { get; set; }
        public bool sC03EscalationP3 { get; set; }
        public bool sC03EscalationP4 { get; set; }
        public bool sC04InfiltrationP1 { get; set; }
        public bool sC04InfiltrationP2 { get; set; }
        public bool sC05ExtractionP1 { get; set; }
        public bool sC05ExtractionP2 { get; set; }
        public bool sC06InterceptionP1 { get; set; }
        public bool sC06InterceptionP2 { get; set; }
        public bool sC06InterceptionP3 { get; set; }
        public bool sC07RedirectionP1 { get; set; }
        public bool sC07RedirectionP2 { get; set; }
        public bool sC08DesolationP1 { get; set; }
        public bool sC08DesolationP2 { get; set; }
        public bool sC09IncursionP1 { get; set; }
        public bool sC09IncursionP2 { get; set; }
        public bool sC10Revelation { get; set; }
        public bool sC11Retaliation { get; set; }

        private const bool DEFAULT_AUTORESET = false;
        private const bool DEFAULT_AUTOSTART = true;
        private const bool DEFAULT_C01INCEPTION = true;
        private const bool DEFAULT_C02INITIATION = true;
        private const bool DEFAULT_C03ESCALATIONP1 = true;
        private const bool DEFAULT_C03ESCALATIONP2 = true;
        private const bool DEFAULT_C03ESCALATIONP3 = true;
        private const bool DEFAULT_C03ESCALATIONP4 = true;
        private const bool DEFAULT_C04INFILTRATIONP1 = true;
        private const bool DEFAULT_C04INFILTRATIONP2 = true;
        private const bool DEFAULT_C05EXTRACTIONP1 = true;
        private const bool DEFAULT_C05EXTRACTIONP2 = true;
        private const bool DEFAULT_C06INTERCEPTIONP1 = true;
        private const bool DEFAULT_C06INTERCEPTIONP2 = true;
        private const bool DEFAULT_C06INTERCEPTIONP3 = true;
        private const bool DEFAULT_C07REDIRECTIONP1 = true;
        private const bool DEFAULT_C07REDIRECTIONP2 = true;
        private const bool DEFAULT_C08DESOLATIONP1 = true;
        private const bool DEFAULT_C08DESOLATIONP2 = true;
        private const bool DEFAULT_C09INCURSIONP1 = true;
        private const bool DEFAULT_C09INCURSIONP2 = true;
        private const bool DEFAULT_C10REVELATION = true;
        private const bool DEFAULT_C11RETALIATION = true;

        public FEARSettings()
        {
            InitializeComponent();

            this.chkAutoReset.DataBindings.Add("Checked", this, "AutoReset", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chkAutoStart.DataBindings.Add("Checked", this, "AutoStart", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chk01.DataBindings.Add("Checked", this, "sC01Inception", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chk02.DataBindings.Add("Checked", this, "sC02Initiation", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chk03P1.DataBindings.Add("Checked", this, "sC03EscalationP1", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chk03P2.DataBindings.Add("Checked", this, "sC03EscalationP2", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chk03P3.DataBindings.Add("Checked", this, "sC03EscalationP3", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chk03P4.DataBindings.Add("Checked", this, "sC03EscalationP4", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chk04P1.DataBindings.Add("Checked", this, "sC04InfiltrationP1", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chk04P2.DataBindings.Add("Checked", this, "sC04InfiltrationP2", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chk05P1.DataBindings.Add("Checked", this, "sC05ExtractionP1", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chk05P2.DataBindings.Add("Checked", this, "sC05ExtractionP2", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chk06P1.DataBindings.Add("Checked", this, "sC06InterceptionP1", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chk06P2.DataBindings.Add("Checked", this, "sC06InterceptionP1", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chk06P3.DataBindings.Add("Checked", this, "sC06InterceptionP1", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chk07P1.DataBindings.Add("Checked", this, "sC07RedirectionP1", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chk07P2.DataBindings.Add("Checked", this, "sC07RedirectionP2", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chk08P1.DataBindings.Add("Checked", this, "sC08DesolationP1", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chk08P2.DataBindings.Add("Checked", this, "sC08DesolationP2", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chk09P1.DataBindings.Add("Checked", this, "sC09IncursionP1", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chk09P2.DataBindings.Add("Checked", this, "sC09IncursionP2", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chk10.DataBindings.Add("Checked", this, "sC10Revelation", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chk11.DataBindings.Add("Checked", this, "sC11Retaliation", false, DataSourceUpdateMode.OnPropertyChanged);

            // defaults
            this.AutoReset = DEFAULT_AUTORESET;
            this.AutoStart = DEFAULT_AUTOSTART;
            this.sC01Inception = DEFAULT_C01INCEPTION;
            this.sC02Initiation = DEFAULT_C02INITIATION;
            this.sC03EscalationP1 = DEFAULT_C03ESCALATIONP1;
            this.sC03EscalationP2 = DEFAULT_C03ESCALATIONP2;
            this.sC03EscalationP3 = DEFAULT_C03ESCALATIONP3;
            this.sC03EscalationP4 = DEFAULT_C03ESCALATIONP4;
            this.sC04InfiltrationP1 = DEFAULT_C04INFILTRATIONP1;
            this.sC04InfiltrationP2 = DEFAULT_C04INFILTRATIONP2;
            this.sC05ExtractionP1 = DEFAULT_C05EXTRACTIONP1;
            this.sC05ExtractionP2 = DEFAULT_C05EXTRACTIONP2;
            this.sC06InterceptionP1 = DEFAULT_C06INTERCEPTIONP1;
            this.sC06InterceptionP2 = DEFAULT_C06INTERCEPTIONP2;
            this.sC06InterceptionP3 = DEFAULT_C06INTERCEPTIONP3;
            this.sC07RedirectionP1 = DEFAULT_C07REDIRECTIONP1;
            this.sC07RedirectionP2 = DEFAULT_C07REDIRECTIONP2;
            this.sC08DesolationP1 = DEFAULT_C08DESOLATIONP1;
            this.sC08DesolationP2 = DEFAULT_C08DESOLATIONP2;
            this.sC09IncursionP1 = DEFAULT_C09INCURSIONP1;
            this.sC09IncursionP2 = DEFAULT_C09INCURSIONP2;
            this.sC10Revelation = DEFAULT_C10REVELATION;
            this.sC11Retaliation = DEFAULT_C11RETALIATION;

        }

        public XmlNode GetSettings(XmlDocument doc)
        {
            XmlElement settingsNode = doc.CreateElement("Settings");

            settingsNode.AppendChild(ToElement(doc, "Version", Assembly.GetExecutingAssembly().GetName().Version.ToString(3)));

            settingsNode.AppendChild(ToElement(doc, "AutoReset", this.AutoReset));
            settingsNode.AppendChild(ToElement(doc, "AutoStart", this.AutoStart));
            settingsNode.AppendChild(ToElement(doc, "C1", this.sC01Inception));
            settingsNode.AppendChild(ToElement(doc, "C2", this.sC02Initiation));
            settingsNode.AppendChild(ToElement(doc, "C3P1", this.sC03EscalationP1));
            settingsNode.AppendChild(ToElement(doc, "C3P2", this.sC03EscalationP2));
            settingsNode.AppendChild(ToElement(doc, "C3P3", this.sC03EscalationP3));
            settingsNode.AppendChild(ToElement(doc, "C3P4", this.sC03EscalationP4));
            settingsNode.AppendChild(ToElement(doc, "C4P1", this.sC04InfiltrationP1));
            settingsNode.AppendChild(ToElement(doc, "C4P2", this.sC04InfiltrationP2));
            settingsNode.AppendChild(ToElement(doc, "C5P1", this.sC05ExtractionP1));
            settingsNode.AppendChild(ToElement(doc, "C5P2", this.sC05ExtractionP2));
            settingsNode.AppendChild(ToElement(doc, "C6P1", this.sC06InterceptionP1));
            settingsNode.AppendChild(ToElement(doc, "C6P2", this.sC06InterceptionP2));
            settingsNode.AppendChild(ToElement(doc, "C6P3", this.sC06InterceptionP3));
            settingsNode.AppendChild(ToElement(doc, "C7P1", this.sC07RedirectionP1));
            settingsNode.AppendChild(ToElement(doc, "C7P2", this.sC07RedirectionP2));
            settingsNode.AppendChild(ToElement(doc, "C8P1", this.sC08DesolationP1));
            settingsNode.AppendChild(ToElement(doc, "C8P2", this.sC08DesolationP2));
            settingsNode.AppendChild(ToElement(doc, "C9P1", this.sC09IncursionP1));
            settingsNode.AppendChild(ToElement(doc, "C9P2", this.sC09IncursionP2));
            settingsNode.AppendChild(ToElement(doc, "C10", this.sC10Revelation));
            settingsNode.AppendChild(ToElement(doc, "C11", this.sC11Retaliation));

            return settingsNode;
        }

        public void SetSettings(XmlNode settings)
        {
            this.AutoReset = ParseBool(settings, "AutoReset", DEFAULT_AUTORESET);
            this.AutoStart = ParseBool(settings, "AutoStart", DEFAULT_AUTOSTART);
            this.sC01Inception = ParseBool(settings, "C1", DEFAULT_C01INCEPTION);
            this.sC02Initiation = ParseBool(settings, "C2", DEFAULT_C02INITIATION);
            this.sC03EscalationP1 = ParseBool(settings, "C3P1", DEFAULT_C03ESCALATIONP1);
            this.sC03EscalationP2 = ParseBool(settings, "C3P2", DEFAULT_C03ESCALATIONP2);
            this.sC03EscalationP3 = ParseBool(settings, "C3P3", DEFAULT_C03ESCALATIONP3);
            this.sC03EscalationP4 = ParseBool(settings, "C3P4", DEFAULT_C03ESCALATIONP4);
            this.sC04InfiltrationP1 = ParseBool(settings, "C4P1", DEFAULT_C04INFILTRATIONP1);
            this.sC04InfiltrationP2 = ParseBool(settings, "C4P2", DEFAULT_C04INFILTRATIONP2);
            this.sC05ExtractionP1 = ParseBool(settings, "C5P1", DEFAULT_C05EXTRACTIONP1);
            this.sC05ExtractionP2 = ParseBool(settings, "C5P2", DEFAULT_C05EXTRACTIONP2);
            this.sC06InterceptionP1 = ParseBool(settings, "C6P1", DEFAULT_C06INTERCEPTIONP1);
            this.sC06InterceptionP2 = ParseBool(settings, "C6P2", DEFAULT_C06INTERCEPTIONP2);
            this.sC06InterceptionP3 = ParseBool(settings, "C6P3", DEFAULT_C06INTERCEPTIONP3);
            this.sC07RedirectionP1 = ParseBool(settings, "C7P1", DEFAULT_C07REDIRECTIONP1);
            this.sC07RedirectionP2 = ParseBool(settings, "C7P2", DEFAULT_C07REDIRECTIONP2);
            this.sC08DesolationP1 = ParseBool(settings, "C8P1", DEFAULT_C08DESOLATIONP1);
            this.sC08DesolationP2 = ParseBool(settings, "C8P2", DEFAULT_C08DESOLATIONP2);
            this.sC09IncursionP1 = ParseBool(settings, "C9P1", DEFAULT_C09INCURSIONP1);
            this.sC09IncursionP2 = ParseBool(settings, "C9P2", DEFAULT_C09INCURSIONP2);
            this.sC10Revelation = ParseBool(settings, "C10", DEFAULT_C10REVELATION);
            this.sC11Retaliation = ParseBool(settings, "C11", DEFAULT_C11RETALIATION);
        }

        static bool ParseBool(XmlNode settings, string setting, bool default_ = false)
        {
            bool val;
            return settings[setting] != null ?
                (Boolean.TryParse(settings[setting].InnerText, out val) ? val : default_)
                : default_;
        }

        static XmlElement ToElement<T>(XmlDocument document, string name, T value)
        {
            XmlElement str = document.CreateElement(name);
            str.InnerText = value.ToString();
            return str;
        }
    }
}
