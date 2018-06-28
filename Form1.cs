using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace TicTacToe
{

    public partial class Form1 : Form
    {
        bool turn = true;
        private int counter = 0;
        private int winCounterX = 0;
        private int winCounterO = 0;
        Socket skara;
        EndPoint epLocal, epRemote;
        private string xo = "O";
        private bool connected = false;
        private bool multi =true;

        public Form1()
        {
            InitializeComponent();
            skara = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            skara.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);


            textBox1.Text = getLocalIP();
            textBox3.Text = getLocalIP();
            
        }

        private string getLocalIP()
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip.ToString();
            }

            return "192.168.1.254";
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button10_Click(object sender, EventArgs e)
        {
            try
            {
                System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
                byte[] msg = new byte[1500];
                msg = enc.GetBytes("m" + textMessage.Text);

                skara.Send(msg);
                listMessage.Items.Add("Ty:" + textMessage.Text);
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.ToString());
            }
        }

        private void MessageCallBack(IAsyncResult aResult)
        {
            try
            {
                int size = skara.EndReceiveFrom(aResult, ref epRemote);
                if (size > 0)
                {
                    byte[] receivedData = new byte[1464];

                    receivedData = (byte[])aResult.AsyncState;
                    ASCIIEncoding eEncoding = new ASCIIEncoding();

                    String receivedMessage = eEncoding.GetString(receivedData);
                    char msgid = receivedMessage[0];
                    receivedMessage = receivedMessage.Substring(1, size - 1);
                    switch (msgid)
                    {
                        case 'm':
                            listMessage.Invoke(new Action(delegate ()
                            {
                                listMessage.Items.Add("Przeciwnik: " + receivedMessage);
                            }));
                            break;
                        case 'r':
                            enableButtons();
                            handleTurn(receivedMessage);
                            break;
                        case 's':
                            xo = receivedMessage;
                            connected = true;
                            break;
                    }



                }
                byte[] buffer = new byte[1500];
                skara.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.ToString());
            }
        }

        private void showScoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("X: " + winCounterX + " O:" + winCounterO, "Score");
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void singleplayerToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }



        private void handleTurn(String btnName)
        {
            Button btn;

            foreach (Control c in Controls)
                if (c.GetType() == typeof(Button))
                {

                    if (c.Name.Equals(btnName))
                    {
                        // MessageBox.Show("here");
                        btn = (Button)c;
                        btn.Invoke(new Action(delegate ()
                        {
                            btn.BackColor = Color.Yellow;
                            btn.Text = xo == "O" ? "X" : "O";
                            btn.Enabled = false;
                        }));
                        counter++;
                        turn = true;
                    }
                }
            winChecker();
        }

        private void buttonClick(object sender, EventArgs e)
        {
            Button butt = (Button)sender;
            if (!multi)
            {
                if (turn)
                {
                    butt.Text = "O";
                    butt.BackColor = Color.Green;
                }
                else if (!turn)
                {
                    butt.Text = "X";
                    butt.BackColor = Color.Blue;
                }

            }
            else
            {
                butt.Text = xo;
                butt.BackColor = Color.Blue;
                ASCIIEncoding eEncoding = new ASCIIEncoding();
                byte[] msg = new byte[15];
                msg = eEncoding.GetBytes("r" + butt.Name);
                //MessageBox.Show(msg.ToString());
                skara.Send(msg);
                disableButtons();
            }
            turn = !turn;
            disableButtons();
            butt.Enabled = false;
            counter++;
            winChecker();

        }

        private void Your_message_here_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void winChecker()
        {
            bool ifWin = false;
            String win = "The winner is";
            if ((button1.Text == button2.Text) && (button2.Text == button3.Text) && (!(button1.Text == "")))
            {
                ifWin = true;
                button1.BackColor = button2.BackColor = button3.BackColor = Color.Red;
            }
            if ((button1.Text == button4.Text) && (button4.Text == button7.Text) && (!(button1.Text == "")))
            {
                ifWin = true;
                button1.BackColor = button4.BackColor = button7.BackColor = Color.Red;
            }
            if ((button1.Text == button5.Text) && (button5.Text == button9.Text) && (!(button1.Text == "")))
            {
                ifWin = true;
                button1.BackColor = button5.BackColor = button9.BackColor = Color.Red;
            }
            if ((button2.Text == button5.Text) && (button5.Text == button8.Text) && (!(button2.Text == "")))
            {
                ifWin = true;
                button2.BackColor = button5.BackColor = button8.BackColor = Color.Red;
            }
            if ((button3.Text == button5.Text) && (button5.Text == button7.Text) && (!(button3.Text == "")))
            {
                ifWin = true;
                button3.BackColor = button5.BackColor = button7.BackColor = Color.Red;
            }
            if ((button3.Text == button6.Text) && (button6.Text == button9.Text) && (!(button3.Text == "")))
            {
                ifWin = true;
                button3.BackColor = button6.BackColor = button9.BackColor = Color.Red;
            }
            if ((button4.Text == button5.Text) && (button5.Text == button6.Text) && (!(button4.Text == "")))
            {
                ifWin = true;
                button4.BackColor = button5.BackColor = button6.BackColor = Color.Red;
            }
            if ((button7.Text == button8.Text) && (button8.Text == button9.Text) && (!(button7.Text == "")))
            {
                ifWin = true;
                button7.BackColor = button8.BackColor = button9.BackColor = Color.Red;
            }
            if (ifWin)
            {
                if (turn)
                {
                    MessageBox.Show(win + " X", "X, u nutz");
                    winCounterX++;
                    disableButtons();

                }
                else
                {
                    MessageBox.Show(win + " O", "O, u nutz");
                    disableButtons();
                    winCounterO++;

                }
            }
            else if (counter == 9)
                MessageBox.Show("There is no winner... DRAW", "Next time");


            textBox4.Invoke(new Action(delegate ()
            {
                textBox4.Text = winCounterX.ToString();

            }));

            textBox5.Invoke(new Action(delegate ()
            {
                textBox5.Text = winCounterO.ToString();

            }));


        }

        private void disableButtons()
        {
            foreach (Control c in Controls)
            {
                if (c.GetType() == typeof(Button))
                {
                    Button b = (Button)c;

                    b.Invoke(new Action(delegate ()
                    {
                        b.Enabled = false;
                    }));

                }
            }
        }

        private void enableButtons()
        {

            foreach (Control c in Controls)
            {
                if (c.GetType() == typeof(Button))
                {
                    Button b = (Button)c;

                    b.Invoke(new Action(delegate ()
                    {
                        if (b.Text == "")
                            b.Enabled = true;
                    }));

                }
            }
        }

        private void singleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            turn = true;
            counter = 0;
            foreach (Control c in Controls)
            {
                if (c.GetType() == typeof(Button))
                {
                    Button b = (Button)c;
                    b.Invoke(new Action(delegate ()
                    {
                        b.Enabled = true;
                        b.Text = String.Empty;
                        b.BackColor = Color.FromArgb(224, 224, 224);
                    }));
                }
            }

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void button11_Click(object sender, EventArgs e)
        {
            try
            {
                epLocal = new IPEndPoint(IPAddress.Parse(textBox1.Text), Convert.ToInt32(textBox6.Text));
                skara.Bind(epLocal);
                epRemote = new IPEndPoint(IPAddress.Parse(textBox3.Text), Convert.ToInt32(textBox7.Text));
                skara.Connect(epRemote);
                byte[] buffer = new byte[1500];
                skara.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);
                if (!connected)
                {
                    buffer[0] = (byte)'s';
                    buffer[1] = (byte)'x';
                    turn = false;
                }
                skara.Send(buffer);
                button11.Text = "Połączono";
                button11.Enabled = false;
                button10.Enabled = true;
                textMessage.Focus();
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.ToString());
            }
        }

        private void multiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            multi = true;
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }
    }
}