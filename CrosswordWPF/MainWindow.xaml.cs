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

        List<TextBox> wordBoxes = new List<TextBox>();
        List<TextBox> clueBoxes = new List<TextBox>();

        public MainWindow()
        {
            InitializeComponent();
            wordBoxes.Add(boxWord1);
            wordBoxes.Add(boxWord2);
            clueBoxes.Add(boxClue1);
            clueBoxes.Add(boxClue2);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            List<Word> words = new List<Word>();

            foreach (TextBox box in wordBoxes)
            {
                words.Add(new Word(box.Text));
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
        }

        private void AddNewWordLine()
        {
            AddNewGridRow();
            TextBox newWord = new TextBox();
            TextBox newClue = new TextBox();
            newWord.Name = "boxWord" + wordBoxes.Count + 1;
            newClue.Name = "boxClue" + wordBoxes.Count + 1;
            MainGrid.Children.Add(newWord);
            MainGrid.Children.Add(newClue);
            Grid.SetRow(newWord, MainGrid.RowDefinitions.Count-2);
            Grid.SetRow(newClue, MainGrid.RowDefinitions.Count - 2);
            Grid.SetColumn(newWord, 0);
            Grid.SetColumn(newClue, 1);
            newWord.Height = newClue.Height = 23;
            newWord.Width = 120;
            newClue.Width = 300;
            newWord.VerticalAlignment = newClue.VerticalAlignment = VerticalAlignment.Center;
            newWord.HorizontalAlignment = HorizontalAlignment.Right;
            newClue.HorizontalAlignment = HorizontalAlignment.Left;
            newWord.Margin = new Thickness(0, 0, 2, 0);
            newClue.Margin = new Thickness(2, 0, 0, 0);

            wordBoxes.Add(newWord);
            clueBoxes.Add(newClue);
        }

        private void AddNewGridRow()
        {
            RowDefinition newRow = new RowDefinition();
            newRow.Height = new GridLength(30);
            MainGrid.RowDefinitions.Add(newRow);
            Grid.SetRow(btnAdd, MainGrid.RowDefinitions.Count-1);
            Grid.SetRow(btnGo, MainGrid.RowDefinitions.Count - 1);
        }
    }
}
