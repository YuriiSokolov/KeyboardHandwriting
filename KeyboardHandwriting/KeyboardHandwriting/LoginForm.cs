using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeyboardHandwriting
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnEnter_Click(object sender, EventArgs e)
        {
            if (txtLogin.Text != string.Empty)
            {
                Login.UserName = txtLogin.Text.Trim();
                TrainingProgram objTrainingProgram = new TrainingProgram();
                objTrainingProgram.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Введіть свій Логін");
            }
        }
    }
}
