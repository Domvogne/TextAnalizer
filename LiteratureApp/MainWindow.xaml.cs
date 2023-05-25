using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace LiteratureApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public readonly string OriginalText;
        public List<Replacing> replacings;
        public MainWindow()
        {
            InitializeComponent();
            var conya = File.ReadAllText("conya.txt");
            OriginalText = conya;
            TextTB.Text = OriginalText;
            replacings = new List<Replacing>();
            ReplaceLB.ItemsSource = replacings;
        }

        private void OnAddReplace(object sender, RoutedEventArgs e)
        {
            replacings.Add(new Replacing());
            UpdateList();
        }

        private void OnRemoveReplace(object sender, RoutedEventArgs e)
        {
            if (ReplaceLB.SelectedItem == null)
                return;
            replacings.Remove((Replacing)ReplaceLB.SelectedItem);
            UpdateList();
        }

        private void OnUpdate(object sender, RoutedEventArgs e)
        {
            var sorted = replacings.OrderBy(i => i.From.Length).Reverse().ToList();
            var workText = OriginalText;

            for (int i = 0; i < sorted.Count; i++)
            {
                var repl = sorted[i];
                if (string.IsNullOrEmpty(repl.From))
                    continue;
                workText = workText.Replace(repl.From, ('\u0003').ToString());
                repl.N = workText.Count(i => i == '\u0003');
                repl.OnPropertyChanged("N");
                workText = workText.Replace(('\u0003').ToString(), repl.To);
            }
            TextTB.Text = workText;
        }
        void UpdateList()
        {
            ReplaceLB.ItemsSource = null;
            ReplaceLB.ItemsSource = replacings;
        }

        private void OnLetterAnalize(object sender, RoutedEventArgs e)
        {
            var stats = GetStatistics();
            new LettersWindow(stats).Show();
        }

        public List<LetterStatictics> GetStatistics()
        {
            List<LetterStatictics> ls = new List<LetterStatictics>();
            var workText = TextTB.Text;
            var total = workText.Length;
            while (workText.Length > 0)
            {
                var next = workText[0];
                var stat = new LetterStatictics(next) { Total = total };
                var startLen = workText.Length;
                workText = workText.Replace(next.ToString(), "");
                stat.Count = startLen - workText.Length;
                ls.Add(stat);
            }
            ls = ls.OrderBy(x => x.Count).Reverse().ToList();
            for (int i = 0; i < ls.Count; i++)
            {
                ls[i].Raiting = i + 1;
            }

            return ls;
        }
    }

    public class Replacing : INotifyPropertyChanged
    {
        public string From { get; set; }
        public string To { get; set; }
        public int N { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
    public class LetterStatictics
    {
        public char Letter { get; set; }
        public int Raiting { get; set; }
        public int Total { get; set; }
        public int Count { get; set; }
        public LetterStatictics(char letter)
        {
            Letter = letter;
        }
    }
}
