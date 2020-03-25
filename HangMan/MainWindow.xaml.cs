using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Input;

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
        private string doFunction;
        private string functionKey;
        Player playerHost = new Player();
        GuestPlayer playerGuest = new GuestPlayer();
        List<String> hiddenWord = new List<string>();
        List<String> guessedWord = new List<string>();
        List<String> incorrectChars = new List<string>();
        private string guessedChar;
        BackgroundWorker worker1 = new BackgroundWorker();
        BackgroundWorker worker2 = new BackgroundWorker();
        public MainWindow()
        {
            worker1.DoWork += worker1_DoWork;
            worker2.DoWork += worker2_DoWork;
            InitializeComponent();
            playerGuest.SetTries(8);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void worker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (client.Connected)
            {
                try
                {
                    switch (doFunction)
                    {
                        case "KeyDown":
                            GuessWordKey(functionKey);
                            break;
                        case "EnterKey":
                            EnterWord(functionKey);
                            break;
                        case "restart":
                            RestartGame();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void worker2_DoWork(object sender, DoWorkEventArgs e)
        {
            if (client.Connected)
            {
            }
            else
            {
                MessageBox.Show("Sending failed");
            }
            MessageBox.Show("Sending failed");
            worker2.CancelAsync();

        }
        private void StartButton_Click(object sender, RoutedEventArgs e)
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
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
        /// <summary>
        /// Click Event for TextBox, calls for function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GuessWord_KeyDown(object sender, KeyEventArgs e)
        {
            GuessWordKey(e.Key.ToString());
            //worker1.RunWorkerAsync();
        }
        /// <summary>
        /// Click Event for TextBox, calls for function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnterWord_KeyDown(object sender, KeyEventArgs e)
        {
            EnterWord(e.Key.ToString());
            //worker2.RunWorkerAsync();
        }
        /// <summary>
        /// Checks game state to see if "Guess Word" input is correct and checks game state if the game is over
        /// </summary>
        /// <param name="key"></param>
        private void GuessWordKey(string key)

        {
            if (key.CompareTo(Key.Return.ToString()) == 0) //If key pressed down is "Enter"
            {
                guessedChar = tBGuessWord.Text.ToUpper(); //Convert to upperCase
                if (guessedChar.All(char.IsDigit)) //if input is a digit, return.
                {
                    return;
                }
                else
                {
                    bool correctWord = false;
                    int j = 0;
                    foreach (string ch in hiddenWord) //compare input to every character in hiddenWord list
                    {
                        if (ch == guessedChar) //if char matches input
                        {
                            guessedWord[j] = guessedChar.ToUpper();
                            correctWord = true;
                        }
                        j++;
                    }
                    if (correctWord == false) //if input matched no chars, decrease tries by 1 and update gui.
                    {
                        playerGuest.SetTries(playerGuest.GetTries() - 1);
                        TriesLeft.Text = "Tries Left: " + playerGuest.GetTries().ToString();
                        incorrectChars.Add(guessedChar);
                        tBlIncorrectChars.Text = string.Join("", incorrectChars);

                    }
                    tBHiddenWord.Text = string.Join("", guessedWord); //update gui to match guessedWord list
                }
                correctWord = false; //
                if (string.Join("", hiddenWord).CompareTo(string.Join("", guessedWord)) == 0)
                {
                    correctWord = true;
                }

                if (correctWord == true) //Add participated game to both players and win to guest
                {
                    playerGuest.AddParticipatedGame();
                    playerGuest.AddWin();
                    playerHost.AddParticipatedGame();
                    doFunction = "restart"; //Send function "restart" to other client
                    RestartGame();
                    tblStatus.Text = "Guest Won Game!";
                }
                if (playerGuest.GetTries() == 0) //If player runs out of tries
                {
                    //Add participated game to both players and win to host
                    playerGuest.AddParticipatedGame(); //Add played game into guest player's stats
                    playerHost.AddWin(); //Add win into guest player's stats
                    playerHost.AddParticipatedGame();
                    doFunction = "restart"; //Send function "restart" to other client
                    RestartGame();
                    tblStatus.Text = "Host Won Game!";
                }
            }
            doFunction = "KeyDown";
            functionKey = key;
        }
        /// <summary>
        /// Adds word into 
        /// </summary>
        /// <param name="key"></param>
        public void EnterWord(string key)
        {
            if (key.CompareTo(Key.Return.ToString()) == 0) //if input is Enter key
            {
                foreach (char ch in tBEnterWord.Text)
                {
                    hiddenWord.Add(ch.ToString().ToUpper()); //Add every char of string into list
                    guessedWord.Add("_"); //Add "_" for every char in string
                }
                tBEnterWord.Text = "";
                tBHiddenWord.Text = string.Join("", guessedWord);
                tBEnterWord.IsEnabled = false; //Turn off
                doFunction = "EnterKey"; //Send "EnterKey" to sync with client
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void RestartGame()
        {
            hiddenWord.Clear();
            guessedWord.Clear();
            incorrectChars.Clear();
            playerGuest.SetTries(8);
            tBGuessWord.Text = "";
            tBHiddenWord.Text = "";
            tBEnterWord.Text = "";
            TriesLeft.Text = "Tries Left:  " + playerGuest.GetTries().ToString();
            tBlIncorrectChars.Text = "";
            tblGameStatus.Text = "";
            tBEnterWord.IsEnabled = true;
            //worker2.RunWorkerAsync();
        }
    }
}

