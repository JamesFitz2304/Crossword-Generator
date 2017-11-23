using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Crosswords;

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

            CrosswordGenerator generator;

            try
            {
              generator = new CrosswordGenerator(words);
            }
            catch (FormatException)
            {
                MessageBox.Show("One or more words contained invalid characters");
                return;
            }

            if (!generator.Generate())
            {
                MessageBox.Show("Could not generate a crossword with the entered words.");
                return;
            }
            
            CrosswordWindow crossword = new CrosswordWindow(generator.blocks);
            crossword.Show();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddNewWordLine();
            if (WordInputs.Count > 2)
            {
                btnRemove.Visibility = Visibility.Visible;
            }
        }

        private void AddNewWordLine()
        {
            AddNewGridRow();
            WordInputs.Add(new WordInput(WordInputs, MainGrid));
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
            MainGrid.Children.Remove(WordInputs[WordInputs.Count-1].WordBox);
            MainGrid.Children.Remove(WordInputs[WordInputs.Count - 1].ClueBox);
            WordInputs.RemoveAt(WordInputs.Count-1);
            RemoveGridRow();
            Grid.SetRow(btnRemove, MainGrid.RowDefinitions.Count - 2);
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
        }
    }
}
