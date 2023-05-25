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
using System.Windows.Shapes;

namespace LiteratureApp
{
    /// <summary>
    /// Логика взаимодействия для LettersWindow.xaml
    /// </summary>
    public partial class LettersWindow : Window
    {
        public LettersWindow(List<LetterStatictics> stats)
        {
            InitializeComponent();
            StatsLB.ItemsSource = stats;
        }
    }
}
