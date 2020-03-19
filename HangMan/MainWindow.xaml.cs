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
        private bool correctWord;
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
                MessageBox.Show("Hello");
            }
            else
            {
                MessageBox.Show("Sending failed");
            }
            worker2.CancelAsync();

        }

        private void TextBoxKey(string key)
        {
            if (key.CompareTo(Key.Return.ToString()) == 0)
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
                        int j = 0;
                        foreach (string ch in hiddenWord)
                        {
                            if (ch == guessedChar)
                            {
                                guessedWord[j] = guessedChar.ToUpper();
                                correctWord = true;
                            }
                            j++;
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
                    if (playerGuest.GetTries() == 0)
                    {
                        playerGuest.AddParticipatedGame();
                        playerHost.AddWin();
                        playerHost.AddParticipatedGame();
                        RestartGame();
                    }
                    correctWord = true;
                    int i = 0;
                    while (correctWord == true)
                    {
                        if (hiddenWord[i] == guessedWord[i])
                        {
                            correctWord = true;
                        }
                        else
                        {
                            correctWord = false;
                        }
                    }
                    if (correctWord == true)
                    {
                        playerGuest.AddParticipatedGame();
                        playerGuest.AddWin();
                        playerHost.AddParticipatedGame();
                    }
                    else
                    {
                        playerHost.AddParticipatedGame();
                        playerHost.AddWin();
                        playerGuest.AddParticipatedGame();
                    }
                }

            }
            worker2.RunWorkerAsync();
        }
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            ////TextBoxKey(e.Key.ToString());
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
                        int j = 0;
                        foreach (string ch in hiddenWord)
                        {
                            if (ch == guessedChar)
                            {
                                guessedWord[j] = guessedChar.ToUpper();
                                correctWord = true;
                            }
                            j++;
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
                    if (playerGuest.GetTries() == 0)
                    {
                        playerGuest.AddParticipatedGame();
                        playerHost.AddWin();
                        playerHost.AddParticipatedGame();
                        RestartGame();
                    }
                    correctWord = false;
                    int i = 0;
                    if(string.Join("", hiddenWord).CompareTo(string.Join("", guessedWord)) == 0) {
                        correctWord = true;
                    }
                    if (correctWord == true)
                    {
                        playerGuest.AddParticipatedGame();
                        playerGuest.AddWin();
                        playerHost.AddParticipatedGame();
                    }
                    else
                    {
                        playerHost.AddParticipatedGame();
                        playerHost.AddWin();
                        playerGuest.AddParticipatedGame();
                    }
                }
                worker2.RunWorkerAsync();
            }

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
                worker2.RunWorkerAsync();
                worker2.WorkerSupportsCancellation = true;
            }
        }
        private void RestartGame()
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

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            RestartGame();
        }
    }
}
