﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CrosswordGenerator.GenerationManager;
using CrosswordGenerator.Generator;
using CrosswordGenerator.Generator.Models;
using CrosswordGenerator.Models;

namespace CrosswordWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        List<WordInput> WordInputs = new List<WordInput>();
        private readonly IGenerationManager _manager;

        public MainWindow(IGenerationManager manager)
        {
            _manager = manager;
            InitializeComponent();
            WordInputs.Add(new WordInput(boxWord1, boxClue1));
            WordInputs.Add(new WordInput(boxWord2, boxClue2));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var words = new List<WordCluePair>();
            var x = 1;
            foreach (var input in WordInputs)
            {
                if(input.WordBox.Text.Trim().Length > 0) 
                    words.Add(new WordCluePair(input.WordBox.Text, x));
                x++;
            }

            if (!words.Any())
            {
                words = new List<WordCluePair>()
                {
                        new WordCluePair("Dog",1),
                        new WordCluePair("Cat",2),
                        new WordCluePair("Chicken", 3),
                        new WordCluePair("Cow", 4),
                        new WordCluePair("Monkey", 5),
                        new WordCluePair("Salmon", 5),
                        new WordCluePair("Goat", 6),
                        new WordCluePair("Worm", 7),
                        new WordCluePair("Wasp", 8),
                        new WordCluePair("Bee", 9),
                        new WordCluePair("Ostrich", 10),
                        new WordCluePair("Parrot", 11),
                        new WordCluePair("Frog", 12),
                        new WordCluePair("Skunk", 13),
                        new WordCluePair("Tiger", 14),
                        new WordCluePair("Rabbit", 15),
                        new WordCluePair("Bat", 16),
                        new WordCluePair("Antelope", 17),
                        new WordCluePair("Tortoise", 18)
                    };
            }

            IList<Generation> generations;

            try
            {
                generations = _manager.GenerateCrosswords(words, timeout: 6000, cullIdenticals: true).ToList();
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

            CrosswordWindow crossword = new CrosswordWindow(generation);

            if (generations.Any())
            {
                crossword.Show();
                //var message = generation.PlacedWords.Aggregate("Word Starts\n", (current, placedWord) => current + (placedWord.Word + ' ' + placedWord.FirstLetterCoordinates + "\n"));

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
