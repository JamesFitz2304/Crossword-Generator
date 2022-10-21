using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CrosswordGenerator;

namespace CrosswordWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        List<WordInput> WordInputs = new List<WordInput>();

        public MainWindow()
        {
            InitializeComponent();
            WordInputs.Add(new WordInput(boxWord1, boxClue1));
            WordInputs.Add(new WordInput(boxWord2, boxClue2));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            List<Word> words = new List<Word>();

            foreach (WordInput input in WordInputs)
            {
                words.Add(new Word(input.WordBox.Text));
            }

            //words = new List<Word>()
            //{
            //        new Word("Dog"),
            //        new Word("Cat"),
            //        new Word("Chicken"),
            //        new Word("Cow"),
            //        new Word("Monkey"),
            //        new Word("Salmon"),
            //        new Word("Goat"),
            //        new Word("Worm"),
            //        new Word("Wasp"),
            //        new Word("Bee"),
            //        new Word("Ostrich"),
            //        new Word("Parrot"),
            //        new Word("Frog"),
            //        new Word("Skunk"),
            //        new Word("Tiger"),
            //        new Word("Rabbit"),
            //        new Word("Bat"),
            //        new Word("Antelope"),
            //        new Word("Tortoise")
            //    };

            GenerationManager manager = new GenerationManager(new Generator());

            IList<Generation> generations;

            try
            {
                generations = manager.GenerateCrosswords(words, timeout: Int32.MaxValue).ToList();
            }
            catch (FormatException)
            {
                MessageBox.Show("One or more words contained invalid characters");
                return;
            }

            CrosswordWindow crossword = new CrosswordWindow(generations);

            if (generations.Any())
            {
                crossword.Show();
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddNewWordLine();
        }

        private void AddNewWordLine()
        {
            AddNewGridRow();
            WordInputs.Add(new WordInput(WordInputs, MainGrid));

            if (WordInputs.Count > 2)
            {
                btnRemove.Visibility = Visibility.Visible;
            }
        }

        private void AddNewGridRow()
        {
            RowDefinition newRow = new RowDefinition
            {
                Height = new GridLength(30)
            };
            MainGrid.RowDefinitions.Add(newRow);
            Grid.SetRow(btnAdd, MainGrid.RowDefinitions.Count - 1);
            Grid.SetRow(btnGo, MainGrid.RowDefinitions.Count - 1);
            Grid.SetRow(btnRemove, MainGrid.RowDefinitions.Count - 2);
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            RemoveWordLine();  
        }

        private void RemoveWordLine()
        {
            MainGrid.Children.Remove(WordInputs[WordInputs.Count - 1].WordBox);
            MainGrid.Children.Remove(WordInputs[WordInputs.Count - 1].ClueBox);
            WordInputs.RemoveAt(WordInputs.Count - 1);
            RemoveGridRow();
            if (WordInputs.Count < 3)
            {
                btnRemove.Visibility = Visibility.Hidden;
            }

        }

        private void RemoveGridRow()
        {
            MainGrid.RowDefinitions.RemoveAt(MainGrid.RowDefinitions.Count-1);
            Grid.SetRow(btnAdd, MainGrid.RowDefinitions.Count - 1);
            Grid.SetRow(btnGo, MainGrid.RowDefinitions.Count - 1);
            Grid.SetRow(btnRemove, MainGrid.RowDefinitions.Count - 2);
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Tab && WordInputs[WordInputs.Count - 1].ClueBox.IsFocused)
            {
                AddNewWordLine();
            }
        }
    }
}
