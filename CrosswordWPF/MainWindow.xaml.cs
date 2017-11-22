using System.Collections.Generic;
using System.Windows;
using Crosswords;

namespace CrosswordWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            CrosswordGenerator generator = new CrosswordGenerator(new List<Word>
            {
                new Word("Dog"),
                new Word("Cat"),
                new Word("Chicken"),
                new Word("Cow"),
                new Word("Monkey"),
                new Word("Salmon"),
                new Word("Goat"),
                new Word("Worm"),
                new Word("Wasp"),
                new Word("Bee"),
                new Word("Ostrich"),
                new Word("Parrot"),
                new Word("Frog"),
                new Word("Skunk"),
                new Word("Tiger"),
                new Word("Rabbit"),
                new Word("BAT")
            });

            generator.Generate();

            CrosswordWindow crossword = new CrosswordWindow(generator.blocks);
            crossword.Show();
        }
    }
}
