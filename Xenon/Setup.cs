using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xenon.Crypt;

namespace Xenon
{
    public partial class Setup : Form
    {
        public Setup()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {   
            
            if (passwordInput.Text == verifypassInput.Text)
            {
                if (tosVerify.Checked == true)
                {
                    {
                        var setts = Properties.Settings.Default;
                        Aes enc = new Aes();
                        var Key = passwordInput.Text;
                        var Key2 = usernameInput.Text;
                        var uname = enc.Encrypt(usernameInput.Text, Key2);
                        var pword = enc.Encrypt(passwordInput.Text, Key);
                        setts.passSet = "on";
                        setts.encUser = uname;
                        setts.encPass = pword;
                        setts.Save();
                        Form1 form = new Form1();
                        
                        form.Show();
                        this.Hide();
                    }
                }
                else
                {
                    errorProvider1.SetError(tosVerify, "This must be checked to continue.");
                }
                
            }
            else
            {
                errorProvider1.SetError(verifypassInput, "The value entered does not match");
            }
            
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }
    }
}
