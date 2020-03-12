using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HangMan
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TcpClient client;
        public StreamReader STR;
        public StreamWriter STW;
        public string recieve;
        Player playerHost = new Player();
        GuestPlayer playerGuest = new GuestPlayer();
        List<String> hiddenWord = new List<string>();
        List<String> guessedWord = new List<string>();
        List<String> incorrectChars = new List<string>();
        string guessedChar;
        BackgroundWorker worker1 = new BackgroundWorker();
        BackgroundWorker worker2 = new BackgroundWorker();
        public MainWindow()
        {
            worker1.DoWork += worker1_DoWork;
            worker2.DoWork += worker2_DoWork;
            InitializeComponent();
            playerGuest.SetTries(8);
        }

        private void worker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (client.Connected)
            {
                try
                {
                    recieve = STR.ReadLine();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        private void worker2_DoWork(object sender, DoWorkEventArgs e)
        {
            if (client.Connected)
            {
            }
            else
            {
                MessageBox.Show("Sending failed");
            }
            worker2.CancelAsync();

        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (playerGuest.GetTries() > 0)
                {
                    guessedChar = GuessWordXAML.Text.ToUpper();
                    if (guessedChar.All(char.IsDigit))
                    {
                        return;
                    }
                    else
                    {
                        bool correctWord = false;
                        int i = 0;
                        foreach (string ch in hiddenWord)
                        {
                            if (ch == guessedChar)
                            {
                                guessedWord[i] = guessedChar.ToUpper();
                                correctWord = true;
                            }
                            i++;
                        }
                        if (correctWord == false)
                        {
                            playerGuest.SetTries(playerGuest.GetTries() - 1);
                            TriesLeft.Text = "Tries Left: " + playerGuest.GetTries().ToString();
                            incorrectChars.Add(guessedChar);
                            tBlIncorrectChars.Text = string.Join("", incorrectChars);

                        }
                        HiddenWordXAML.Text = HiddenWordXAML.Text = string.Join("", guessedWord);
                    }
                }

            }
            worker2.RunWorkerAsync();
        }

        private void GuessWordXAML_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                hiddenWord.Clear();

                foreach (char ch in EnterWordXAML.Text)
                {
                    hiddenWord.Add(ch.ToString().ToUpper());
                    guessedWord.Add("_");
                }
                HiddenWordXAML.Text = string.Join("", guessedWord);
                EnterWordXAML.IsEnabled = false;
            }
            worker2.RunWorkerAsync();
        }
        private void RestartGame(object sender, RoutedEventArgs e)
        {
            hiddenWord.Clear();
            guessedWord.Clear();
            incorrectChars.Clear();
            playerGuest.SetTries(8);
            GuessWordXAML.Text = "";
            HiddenWordXAML.Text = "";
            EnterWordXAML.Text = "";
            TriesLeft.Text = "Tries Left: 8";
            tBlIncorrectChars.Text = "";
            tblGameStatus.Text = "";
            EnterWordXAML.IsEnabled = true;
            worker2.RunWorkerAsync();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TcpListener listener = new TcpListener(IPAddress.Any, int.Parse(tbPortHost.Text));
                listener.Start();
                tblStatus.Text = "Server Started!";
                client = listener.AcceptTcpClient(); //Accept a pending connection request 
                STR = new StreamReader(client.GetStream());
                STW = new StreamWriter(client.GetStream());
                STW.AutoFlush = true;
                worker1.RunWorkerAsync();
                worker2.WorkerSupportsCancellation = true;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                client = new TcpClient();

                //Represents a network endpoint as an IP address and a port number.
                IPEndPoint ipEnd = new IPEndPoint(IPAddress.Parse(tbIPUser.Text), int.Parse(tbPortUser.Text));

                //Connects the client to a remote TCP host using the specified host name and port number.
                client.Connect(ipEnd);
                if (client.Connected)
                {
                    STR = new StreamReader(client.GetStream());
                    STW = new StreamWriter(client.GetStream());
                    STW.AutoFlush = true;
                    worker1.RunWorkerAsync();
                    worker2.WorkerSupportsCancellation = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
    }
}
