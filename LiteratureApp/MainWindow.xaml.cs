using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

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
                workText = workText.Replace(repl.From.ToLower(), ('\u0003').ToString());
                workText = workText.Replace(repl.From, ('\u0003').ToString());
                repl.N = workText.Count(i => i == '\u0003');
                repl.OnPropertyChanged("N");
                workText = workText.Replace(('\u0003').ToString(), repl.To);
            }
            var remSum = sorted.Sum(i => i.N);
            sorted.ForEach(i  => i.UpdatePersent(remSum));
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
            var workText = OriginalText.Replace(" ", "");
            workText = workText.Replace('\n', ' ');
            while (workText.Length > 0)
            {
                var next = workText[0];
                var stat = new LetterStatictics(next);
                var startLen = workText.Length;
                if (stat.HasUL)
                {
                    workText = workText.Replace(next.ToString().ToLower(), "");
                    stat.Lower = startLen - workText.Length;
                    workText = workText.Replace(next.ToString().ToUpper(), "");
                    stat.Count = startLen - workText.Length;
                    stat.Upper = stat.Count - stat.Lower;
                }
                else
                {
                    workText = workText.Replace(next.ToString(), "");
                    stat.Lower = startLen - workText.Length;
                }

                stat.Count = startLen - workText.Length;
                ls.Add(stat);
            }
            ls = ls.OrderBy(x => x.Count).Reverse().ToList();
            var max = ls.MaxBy(x => x.Count).Count;
            for (int i = 0; i < ls.Count; i++)
            {
                ls[i].Raiting = i + 1;
                ls[i].Total = max;
            }
            
            return ls;
        }
    }

    public class Replacing : INotifyPropertyChanged
    {
        public string From { get; set; }
        public string To { get; set; }
        public int N { get; set; }
        public double Persentage { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        public void UpdatePersent(int total)
        {
            Persentage = Math.Round((double)N / (double)total * 109);
            OnPropertyChanged("Persentage");

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
        public int Upper { get; set; }
        public int Lower { get; set; }
        public bool HasUL => char.IsLetter(Letter);
        public string Parts => HasUL ? $"({Letter.ToString().ToLower()} {Lower}/{Letter.ToString().ToUpper()} {Upper})" : string.Empty;
    }
}
