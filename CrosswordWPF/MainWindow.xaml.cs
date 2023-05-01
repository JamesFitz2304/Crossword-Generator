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
        private readonly List<WordInput> _wordInputs = new List<WordInput>();

        public MainWindow()
        {
            InitializeComponent();
            _wordInputs.Add(new WordInput(boxWord1, boxClue1));
            _wordInputs.Add(new WordInput(boxWord2, boxClue2));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var words = new List<string>();

            foreach (var input in _wordInputs)
            {
                if(input.WordBox.Text.Trim().Length > 0) 
                    words.Add(input.WordBox.Text.Trim());
            }

            if (!words.Any())
            {
                words = new List<string>()
                {

                    "Dog",
                    "Cat",
                    "Chicken",
                    "Cow",
                    "Monkey",
                    "Salmon",
                    "Goat",
                    "Worm",
                    "Wasp",
                    "Bee",
                    "Ostrich",
                    "Parrot",
                    "Frog",
                    "Skunk",
                    "Tiger",
                    "Rabbit",
                    "Bat",
                    "Antelope",
                    "Tortoise"
                };
            }

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
            _wordInputs.Add(new WordInput(_wordInputs, MainGrid));

            if (_wordInputs.Count > 2)
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
            MainGrid.Children.Remove(_wordInputs[_wordInputs.Count - 1].WordBox);
            MainGrid.Children.Remove(_wordInputs[_wordInputs.Count - 1].ClueBox);
            _wordInputs.RemoveAt(_wordInputs.Count - 1);
            RemoveGridRow();
            if (_wordInputs.Count < 3)
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
            if (e.Key == Key.Tab && _wordInputs[_wordInputs.Count - 1].ClueBox.IsFocused)
            {
                AddNewWordLine();
            }
        }
    }
}
