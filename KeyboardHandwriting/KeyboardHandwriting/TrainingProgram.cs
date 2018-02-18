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

namespace KeyboardHandwriting
{
    public partial class TrainingProgram : Form
    {
        System.Diagnostics.Stopwatch time = new System.Diagnostics.Stopwatch();
        System.Diagnostics.Stopwatch dynamicsOfClicks = new System.Diagnostics.Stopwatch();
        
        int count = 0;
        
        string result = string.Empty;
        int[] counterError = new int[10];
        double[] dynamicsOfClicksSpeed = new double[10];
        double[] enterSpeed = new double[10];
        
        string correctText = "Этот текст";

        SqlConnection sqlcon = new SqlConnection(@"Data Source=Admin-PC;Initial Catalog=KeyboardDB;Integrated Security=True");

        public TrainingProgram()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void TrainingProgram_KeyDown(object sender, KeyEventArgs e)
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
                if (count < 10)
                {
                    if (time.ElapsedMilliseconds != 0)
                    {
                        time.Stop();
                        result += txtEnter.Text;
                        if (result.Length != 0)
                        {
                            enterSpeed[count] = Convert.ToDouble((result.Length * 1.0) / time.ElapsedMilliseconds);
                            dynamicsOfClicksSpeed[count] = Convert.ToDouble(dynamicsOfClicks.ElapsedMilliseconds / (result.Length * 1.0));
                            while (result.Length < correctText.Length)
                            {
                                result += " ";
                            }
                            for (int i = 0; i < correctText.Length; i++)
                            {
                                if (result[i] != correctText[i])
                                {
                                    counterError[count]++;
                                }
                            }
                            count++;
                            result = string.Empty;
                            txtEnter.Clear();
                            time.Reset();
                            dynamicsOfClicks.Reset();
                        }
                        else
                        {
                            time.Reset();
                            dynamicsOfClicks.Reset();
                            MessageBox.Show("Ви не ввели текст!!!");
                        }
                    }
                    else
                    {
                        dynamicsOfClicks.Reset();
                        MessageBox.Show("Ви не запустили таймер (Для запуска таймера натисніть F1)!!!");
                    }
                }
                else
                {
                    time.Reset();
                    double minEnterSpeed = enterSpeed[0];
                    double maxEnterSpeed = enterSpeed[0];
                    double minDynamicsOfClicksSpeed = dynamicsOfClicksSpeed[0];
                    double maxDynamicsOfClicksSpeed = dynamicsOfClicksSpeed[0];
                    double maxCounterError = counterError[0];
                    for (int i = 0; i < 2; i++)
                    {
                        if (minEnterSpeed >= enterSpeed[i])
                        {
                            minEnterSpeed = Convert.ToDouble(enterSpeed[i]);
                        }
                    }
                    for (int i = 0; i < 2; i++)
                    {
                        if (maxEnterSpeed <= enterSpeed[i])
                        {
                            maxEnterSpeed = Convert.ToDouble(enterSpeed[i]);
                        }
                    }
                    for (int i = 0; i < 2; i++)
                    {
                        if (minDynamicsOfClicksSpeed >= dynamicsOfClicksSpeed[i])
                        {
                            minDynamicsOfClicksSpeed = Convert.ToDouble(dynamicsOfClicksSpeed[i]);
                        }
                    }
                    for (int i = 0; i < 2; i++)
                    {
                        if (maxDynamicsOfClicksSpeed <= dynamicsOfClicksSpeed[i])
                        {
                            maxDynamicsOfClicksSpeed = Convert.ToDouble(dynamicsOfClicksSpeed[i]);
                        }
                    }
                    for (int i = 0; i < 2; i++)
                    {
                        if (maxCounterError <= counterError[i])
                        {
                            maxCounterError = Convert.ToDouble(counterError[i]);
                        }
                    }
                    sqlcon.Open();
                    string query = "Select * from Data Where Login = '" + Login.UserName + "'";
                    SqlDataAdapter sda = new SqlDataAdapter(query, sqlcon);
                    DataTable dtbl = new DataTable();
                    sda.Fill(dtbl);
                    if (dtbl.Rows.Count == 0)
                    {
                        SqlCommand cmd = sqlcon.CreateCommand();
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "INSERT INTO Data(Login, MinEnterSpeed, MaxEnterSpeed, MinClicksSpeed, MaxClicksSpeed, counterError) VALUES('" + Login.UserName + "','" + minEnterSpeed + "','" + maxEnterSpeed + "','" + minDynamicsOfClicksSpeed + "','" + maxDynamicsOfClicksSpeed + "','" + maxCounterError + "')";
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        SqlCommand cmd1 = sqlcon.CreateCommand();
                        cmd1.CommandType = CommandType.Text;
                        cmd1.CommandText = "UPDATE Data SET MinEnterSpeed='" + minEnterSpeed + "', MaxEnterSpeed='" + maxEnterSpeed + "', MinClicksSpeed= '" + minDynamicsOfClicksSpeed + "', MaxClicksSpeed= '" + maxDynamicsOfClicksSpeed + "', counterError='" + maxCounterError + "' WHERE Login='" + Login.UserName + "'";
                        cmd1.ExecuteNonQuery();
                    }
                    sqlcon.Close();
                    //MessageBox.Show(minEnterSpeed + "\r\n" + maxEnterSpeed + "\r\n" + maxDynamicsOfClicksSpeed + "\r\n" + minDynamicsOfClicksSpeed + "\r\n" + maxCounterError);
                    MessageBox.Show("Ваші дані внесено в Базу Даних. До побачення.");
                    Application.Exit();
                }
            }

        }

        private void TrainingProgram_KeyUp(object sender, KeyEventArgs e)
        {
            dynamicsOfClicks.Stop();
        }
    }
}
