using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace KeyboardHandwritingAuthentication
{
    public partial class LoginForm : Form
    {
        SqlConnection sqlcon = new SqlConnection(@"Data Source=Admin-PC;Initial Catalog=KeyboardDB;Integrated Security=True");

        System.Diagnostics.Stopwatch time = new System.Diagnostics.Stopwatch();
        System.Diagnostics.Stopwatch dynamicsOfClicks = new System.Diagnostics.Stopwatch();

        string result = string.Empty;
        int counterError = 0;
        double dynamicsOfClicksSpeed = 0;
        double enterSpeed = 0;

        string correctText = "Этот текст";

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
            double minEnterSpeed = 0;
            double maxEnterSpeed = 0;
            double minDynamicsOfClicksSpeed = 0;
            double maxDynamicsOfClicksSpeed = 0;
            int maxCounterError = 0;

            if (txtLogin.Text.Trim() != string.Empty)
            {
                if (txtEnter.Text.Trim() != string.Empty)
                {
                    //=====================================[Вивід даних з БД]============================================
                    sqlcon.Open();
                    string query = "Select * from Data Where Login = '" + txtLogin.Text.Trim() + "'";
                    SqlDataAdapter sda = new SqlDataAdapter(query, sqlcon);
                    DataTable dtbl = new DataTable();
                    sda.Fill(dtbl);
                    if (dtbl.Rows.Count == 1)
                    {
                        SqlCommand cmd = sqlcon.CreateCommand();
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "Select MinEnterSpeed From Data Where Login = '" + txtLogin.Text.Trim() + "'";
                        minEnterSpeed = Convert.ToDouble(cmd.ExecuteScalar());
                        SqlCommand cmd1 = sqlcon.CreateCommand();
                        cmd1.CommandType = CommandType.Text;
                        cmd1.CommandText = "Select MaxEnterSpeed From Data Where Login = '" + txtLogin.Text.Trim() + "'";
                        maxEnterSpeed = Convert.ToDouble(cmd1.ExecuteScalar());

                        SqlCommand cmd2 = sqlcon.CreateCommand();
                        cmd2.CommandType = CommandType.Text;
                        cmd2.CommandText = "Select MinClicksSpeed From Data Where Login = '" + txtLogin.Text.Trim() + "'";
                        minDynamicsOfClicksSpeed = Convert.ToDouble(cmd2.ExecuteScalar());
                        SqlCommand cmd3 = sqlcon.CreateCommand();
                        cmd3.CommandType = CommandType.Text;
                        cmd3.CommandText = "Select MaxClicksSpeed From Data Where Login = '" + txtLogin.Text.Trim() + "'";
                        maxDynamicsOfClicksSpeed = Convert.ToDouble(cmd3.ExecuteScalar());

                        SqlCommand cmd4 = sqlcon.CreateCommand();
                        cmd4.CommandType = CommandType.Text;
                        cmd4.CommandText = "Select counterError From Data Where Login = '" + txtLogin.Text.Trim() + "'";
                        maxCounterError = Convert.ToInt32(cmd4.ExecuteScalar());

                        //MessageBox.Show(minEnterSpeed + "\r\n" + maxEnterSpeed + "\r\n" + minDynamicsOfClicksSpeed + "\r\n" + maxDynamicsOfClicksSpeed + "\r\n" + maxCounterError);
                    }
                    else
                    {
                        MessageBox.Show("Даного Логіна немає в Базі Даних!!!");
                    }
                    sqlcon.Close();
                    //=====================================[Порівняння даних з програми і бд]=================================
                    int flag = 3;
                    //MessageBox.Show(enterSpeed.ToString() + "\r\n" + dynamicsOfClicksSpeed + "\r\n" + counterError);
                    if (enterSpeed < minEnterSpeed || enterSpeed > maxEnterSpeed)
                    {
                        flag--;
                        if (dynamicsOfClicksSpeed < minDynamicsOfClicksSpeed || dynamicsOfClicksSpeed > maxDynamicsOfClicksSpeed)
                        {
                            flag--;
                            if (counterError > maxCounterError)
                            {
                                flag--;
                            }
                        }
                    }
                    if(flag < 2)
                    {
                         MessageBox.Show("Ви не " + txtLogin.Text.Trim() + "!!!");
                    }
                    else
                    {
                        MessageBox.Show("Доброго дня, " + txtLogin.Text.Trim());
                    }
                    //========================================================================================================
                }
                else
                {
                    MessageBox.Show("Введіть фразу для Автентифікації!!!");
                }
            }
            else
            {
                MessageBox.Show("Введіть Логін!!!");
            }
        }

        private void LoginForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData != Keys.F1 || e.KeyData == Keys.Enter)
            {
                dynamicsOfClicks.Start();
            }
            if (e.KeyData == Keys.F1)
            {
                time.Start();
                dynamicsOfClicks.Reset();
            }
            if (e.KeyData == Keys.Enter)
            {
                if (time.ElapsedMilliseconds != 0)
                {
                    time.Stop();
                    result += txtEnter.Text;
                    enterSpeed = Convert.ToDouble((result.Length * 1.0) / time.ElapsedMilliseconds);
                    dynamicsOfClicksSpeed = Convert.ToDouble(dynamicsOfClicks.ElapsedMilliseconds / (result.Length * 1.0));
                    while (result.Length < correctText.Length)
                    {
                        result += " ";
                    }
                    for (int i = 0; i < correctText.Length; i++)
                    {
                        if (result[i] != correctText[i])
                        {
                            counterError++;
                        }
                    }
                    result = string.Empty;
                    time.Reset();
                    dynamicsOfClicks.Reset();
                }
                else
                {
                    dynamicsOfClicks.Reset();
                    MessageBox.Show("Ви не запустили таймер (Для запуска таймера натисніть F1)!!!");
                }
            }
        }

        private void LoginForm_KeyUp(object sender, KeyEventArgs e)
        {
            dynamicsOfClicks.Stop();
        }
    }
}
