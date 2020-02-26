using System;
using System.Collections.Generic;
using System.Linq;
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
        List<String> hiddenWord = new List<string>();
        List<String> guessedWord = new List<string>();
        string guessedChar;
        string hiddenChar;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                guessedChar = GuessWordXAML.Text;
                if(guessedChar.All(char.IsDigit))
                {
                    return;
                }
                else
                {
                    int i = 0;
                    foreach(string ch in hiddenWord)
                    {
                        if(ch == guessedChar)
                        {
                            guessedWord[i] = guessedChar;
                        }

                        i++;
                    }
                }

            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            hiddenWord.Clear();
            foreach (char ch in EnterWordXAML.Text)
            {
                hiddenWord.Add(ch.ToString());
            }
        }
    }
}
