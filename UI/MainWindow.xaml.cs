using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private LowLevelKeyboardListener _listener;
        private bool _isStartButtonClicked = false;
        private string _translateToLang = "tr";
        private string _translateFromLang = "en";



        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CheckNetwokrConnectionAndWriteToLabel();
            AppStartConfigurations();
            StartToListenKeyPress();

        }

        /// <summary>
        /// Create a listiner and start to listen global keypress event.
        /// </summary>
        private void StartToListenKeyPress()
        {
            _listener = new LowLevelKeyboardListener();
            _listener.OnKeyPressed += _listener_OnKeyPressed;
            _listener.HookKeyboard();
        }
        /// <summary>
        /// Before app started , Configure settings about MainWindows.
        /// </summary>
        private void AppStartConfigurations()
        {
            this.ResizeMode = ResizeMode.NoResize;

            richTextBox1.Document.Blocks.Clear();
            aboutLabel.Content = "Kopyala tuşuna bastıktan sonra 'H' ye basarak çeviri yapabilirsiniz.";
            this.Title = "Translater";
            radioButtonIngToTurkish.IsChecked = true;
            StartButton.Content = "Stop";
        }
        /// <summary>
        /// Check network connection to google and write status to a label in main window.
        /// </summary>
        private void CheckNetwokrConnectionAndWriteToLabel()
        {
            if (isConnectionOK())
            {
                connLabel.Content = "Bağlantı var";
                connLabel.Foreground = new SolidColorBrush(Color.FromRgb(0, 255, 0));

            }
            else
            {
                connLabel.Content = "Bağlantı hatası";
                connLabel.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            }
        }
       

        private void _listener_OnKeyPressed(object sender, KeyPressedArgs e)
        {
         
            if (e.KeyPressed == Key.H)
            {

                richTextBox1.Document.Blocks.Clear();
                richTextBox1.Document.Blocks.Add(new Paragraph(new Run(Translate(Clipboard.GetText(),_translateToLang,_translateFromLang))));

            }

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _listener.UnHookKeyboard();//Stop listening to global key event
        }

        private void StartButton_Click_1(object sender, RoutedEventArgs e)
        {
            if (_isStartButtonClicked is false)
            {
                _isStartButtonClicked = true;
                StartButton.Content = "Start";
                _listener.UnHookKeyboard();
            }
            else
            {
                _isStartButtonClicked = false;
                StartButton.Content = "Stop";
                _listener.HookKeyboard();
            }
        }
        /// <summary>
        /// Check network connection via www.google.com
        /// </summary>
        /// <returns>Bool</returns>
        private bool isConnectionOK()
        {
            string adress = "https://www.google.com/";

            try
            {
                WebRequest request = WebRequest.Create(adress);
                WebResponse response = request.GetResponse();
            }
            catch (Exception)
            {

                return false;
            }
            return true;
        }

        private void radioButtonTurkishToIng_Checked(object sender, RoutedEventArgs e)
        {

            _translateFromLang = "tr";
            _translateToLang = "en";
        }

        private void radioButtonIngToTurkish_Checked(object sender, RoutedEventArgs e)
        {
            _translateFromLang = "en";
            _translateToLang = "tr";
        }


        /// <summary>
        /// Translate given word 
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public String Translate(String word,string toLang,string fromLang)
        {
            var toLanguage = toLang; //_translateToLang; //"tr";//English
            var fromLanguage = fromLang; //_translateFromLang; //"en";//Deutsch

            var url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={fromLanguage}&tl={toLanguage}&dt=t&q={HttpUtility.UrlEncode(word)}";
            var webClient = new WebClient
            {
                Encoding = System.Text.Encoding.UTF8
            };
            var result = webClient.DownloadString(url);
            try
            {
                result = result.Substring(4, result.IndexOf("\"", 4, StringComparison.Ordinal) - 4);
                return result;
            }
            catch
            {
                return "Error";
            }
        }


    }
}
