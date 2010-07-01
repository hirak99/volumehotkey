using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using WaveLib.AudioMixer;
using CoreAudioApi;

/**** With lots of help from:
 * http://www.arstdesign.com/articles/popupkiller.html
 * http://www.codeguru.com/csharp/csharp/cs_graphics/sound/print.php/c10931
 * 
 * Does work with games!
 * 
 ****/

namespace VolumeHotkey {
    public partial class Form1 : Form {
        bool useMixers = true;  // Otherwise use Windows 7 based mechanism - CoreAudioApi

        private Mixers mixers;
        private MixerLine line;
        private bool AllowClose = false;

        MMDevice defaultDevice;

        public Form1() {
            InitializeComponent();
            try {
                // Windows XP
                mixers = new Mixers();
                line = mixers.Playback.Lines.
                   GetMixerFirstLineByComponentType(
                   MIXERLINE_COMPONENTTYPE.DST_SPEAKERS);
                line.Channel = Channel.Uniform;
                useMixers = true;
            } catch (Exception) {
                // Windows 7
                useMixers = false;
            }
            if (useMixers == false) {
                // Try to load for Windows 7
                try {
                    MMDeviceEnumerator devEnum = new MMDeviceEnumerator();
                    defaultDevice = devEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
                } catch (Exception) {
                    // Could not load for Windows 7 either
                    toolStripExit.PerformClick();
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            //MessageBox.Show(e.CloseReason.ToString());
            if (e.CloseReason==CloseReason.UserClosing && !AllowClose) { e.Cancel = true; Hide();  }
        }
        
        public float GetVolume() {
            return (useMixers ? (float)line.Volume/0xffff : defaultDevice.AudioEndpointVolume.Channels[0].VolumeLevelScalar);
        }

        public void SetVolume(float volume) {
            if (volume > 1) volume = 1;
            else if (volume < 0) volume = 0;
            if (useMixers) line.Volume = (int)(volume * 0xffff + 0.5);
            else defaultDevice.AudioEndpointVolume.Channels[0].VolumeLevelScalar = volume;
        }

        public bool IsMute() {
            return (useMixers ? line.Mute : defaultDevice.AudioEndpointVolume.Mute);
        }

        public void SetMute(bool mute) {
            if (useMixers) line.Mute = mute;
            else defaultDevice.AudioEndpointVolume.Mute = mute;
        }

        [DllImport("User32.dll")]
        private static extern short GetAsyncKeyState(System.Windows.Forms.Keys vKey);

        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(int vKey);

        private bool WasActive(Keys key) {
            return (GetAsyncKeyState(key) & (-32767 | 1)) != 0;
        }

        private void timer1_Tick(object sender, EventArgs e) {
            /*
            foreach (System.Int32 i in Enum.GetValues(typeof(Keys))) {
                int x = GetAsyncKeyState(i);
                //if ((x == 1) || (x == -32767)) {
                //if (x!=0 && x!=1 && x!=-32767) {
                if ((x & (-32767 | 1)) !=0 ) {
                    textBox1.Text += Enum.GetName(typeof(Keys), i) + " ";//this is WinAPI listener of the keys
                }
            }*/
            if (WasActive(Keys.ControlKey) && (WasActive(Keys.LMenu) || WasActive(Keys.RMenu))) {
                timer1.Interval = 100;
                if (WasActive(Keys.D0)) SetMute(!IsMute());
                if (WasActive(Keys.Up)) SetVolume((float)(GetVolume() + 1.0f/16));
                if (WasActive(Keys.Down)) SetVolume((float)(GetVolume() - 1.0f/16));
            }
            else timer1.Interval = 1000;
        }

        private void toolStripExit_Click(object sender, EventArgs e) {
            AllowClose = true;
            Close();
            Application.Exit();
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                Show();
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e) {
            try {
                System.Diagnostics.Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=4428258");
            }
            catch (Exception) {
                MessageBox.Show("Error opening browser. Please send donation through Paypal, to hirak99@gmail.com.", "Donate", MessageBoxButtons.OK);
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            Hide();
        }

        private void Form1_Shown(object sender, EventArgs e) {
            ActiveControl = button1;
        }

        private void Form1_Load(object sender, EventArgs e) {
            // Basic protection against email harvesting
            textBox1.Text += ("hirak99" + "@" + "gmail" + "." + "com");
        }
    }
}
