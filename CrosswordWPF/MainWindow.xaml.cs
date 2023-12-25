using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CrosswordGenerator.GenerationManager;
using CrosswordGenerator.Mapper;
using CrosswordGenerator.Models;

namespace CrosswordWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly List<WordInput> _wordInputs;
        private readonly IGenerationManager _manager;
        private readonly IPuzzleMapper _mapper;

        public MainWindow(IGenerationManager manager, IPuzzleMapper mapper)
        {
            _manager = manager;
            _mapper = mapper;
            InitializeComponent();
            _wordInputs = new List<WordInput>
            {
                new WordInput(boxWord1, boxClue1),
                new WordInput(boxWord2, boxClue2)
            };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var words = new List<WordCluePair>();
            var x = 1;
            foreach (var input in _wordInputs)
            {
                if(input.WordBox.Text.Trim().Length > 0) 
                    words.Add(new WordCluePair(input.WordBox.Text, id: x));
                x++;
            }

            if (!words.Any())
            {
                words = new List<WordCluePair>()
                {
                        new WordCluePair("Dog", id: 1, clue: "Goes woof"),
                        new WordCluePair("Cat",id:2, clue: "Goes meow"),
                        new WordCluePair("Chicken", id:3, clue : "Goes cluck"),
                        new WordCluePair("Cow", id:4, clue : "Goes moo"),
                        new WordCluePair("Monkey", id:5, clue : "Goes ooh ooh ahh ahh"),
                        new WordCluePair("Salmon", id:5, clue : "Goes swimming"),
                        new WordCluePair("Goat", id:6, clue : "Goes bahh with horns"),
                        new WordCluePair("Worm", id:7, clue : "Goes wriggle"),
                        new WordCluePair("Wasp", id:8, clue : "Goes string"),
                        new WordCluePair("Bee", id:9, clue : "Goes for honey"),
                        new WordCluePair("Ostrich", id:10, clue : "Goes not flying"),
                        new WordCluePair("Parrot", id:11, clue : "Goes whatever you say"),
                        new WordCluePair("Frog", id:12, clue : "Goes ribbit"),
                        new WordCluePair("Skunk", id:13, clue : "Goes stink"),
                        new WordCluePair("Tiger", id:14, clue : "Goes growl in India"),
                        new WordCluePair("Rabbit", id:15, clue : "Goes hopping"),
                        new WordCluePair("Bat", id:16, clue : "Goes screech in the dark"),
                        new WordCluePair("Antelope", id:17, clue : "Goes running with antlers"),
                        new WordCluePair("Tortoise", id:18, clue : "Goes slow")
                    };
            }

            IList<Generation> generations;

            try
            {
                generations = _manager.GenerateCrosswords(words, timeout: int.MaxValue).ToList();
            }
            catch (FormatException)
            {
                MessageBox.Show("One or more words contained invalid characters");
                return;
            }

            var generationsSorted = generations
                .OrderBy(g => g.BlocksSize)
                .ThenByDescending(g => g.SizeRatio);

            var generation = generationsSorted.First();
            

            if (generations.Any())
            {
                var puzzle = _mapper.Map(generation, words);
                var crossword = new CrosswordWindow(puzzle);
                crossword.Show();
                //var message = generation.PlacedWords.Aggregate("Word Starts\n", (current, placedWord) => current + (placedWord.Word + ' ' + placedWord.Start + "\n"));

                //message += "\nUnplaced Words\n";

                //message = generation.UnplacedWords.Aggregate(message, (current, unplaced) => current + (unplaced + "\n"));

                //MessageBox.Show(message);
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
