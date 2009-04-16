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

/**** With lots of help from:
 * http://www.arstdesign.com/articles/popupkiller.html
 * http://www.codeguru.com/csharp/csharp/cs_graphics/sound/print.php/c10931
 * 
 * Does not seem to work with games! May be get async key will work along with a timer.s
 * 
 ****/

namespace VolumeHotkey {
    public partial class Form1 : Form {
        private Mixers mixers = new Mixers();
        private MixerLine line;
        private bool AllowClose = false;

        public Form1() {
            InitializeComponent();
            line = mixers.Playback.Lines.
               GetMixerFirstLineByComponentType(
               MIXERLINE_COMPONENTTYPE.DST_SPEAKERS);
            line.Channel = Channel.Uniform;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            //MessageBox.Show(e.CloseReason.ToString());
            if (e.CloseReason==CloseReason.UserClosing && !AllowClose) { e.Cancel = true; Hide();  }
        }
        
        public int GetVolume() {
            return line.Volume;
        }

        public void SetVolume(int volume) {
            if (volume > 0xffff) volume = 0xffff;
            else if (volume < 0) volume = 0;
            line.Volume = volume;
        }

        public bool IsMute() {
            return line.Mute;
        }

        public void SetMute(bool mute) {
            line.Mute = mute;
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
                if (WasActive(Keys.Up)) SetVolume(GetVolume() + 0xfff);
                if (WasActive(Keys.Down)) SetVolume(GetVolume() - 0xfff);
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
