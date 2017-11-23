using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace CrosswordWPF
{
    internal class WordInput
    {
        public TextBox WordBox;
        public TextBox ClueBox;

        public WordInput(List<WordInput> words, Grid MainGrid)
        {
            WordBox = new TextBox();
            ClueBox = new TextBox();
            SetupBox(WordBox, "boxWord", 0, 120, new Thickness(0, 0, 2, 0), 1, HorizontalAlignment.Right, words, MainGrid);
            SetupBox(ClueBox, "boxClue", 1, 300, new Thickness(2, 0, 0, 0), 2, HorizontalAlignment.Left, words, MainGrid);
        }

        public WordInput(TextBox wordBox, TextBox clueBox)
        {
            WordBox = wordBox;
            ClueBox = clueBox;
        }

        private void SetupBox(TextBox textbox, string prefix, int column, int width, Thickness margin, int tabIndex, HorizontalAlignment horizontal, List<WordInput> words, Grid mainGrid)
        {
            textbox.Name = prefix + (words.Count + 1);
            mainGrid.Children.Add(textbox);
            Grid.SetRow(textbox, mainGrid.RowDefinitions.Count - 2);
            Grid.SetColumn(textbox, column);
            textbox.Height = 23;
            textbox.Width = width;
            textbox.VerticalAlignment = VerticalAlignment.Center;
            textbox.HorizontalAlignment = horizontal;
            textbox.Margin = margin;
            textbox.TabIndex = words.Count * 2 + tabIndex;
        }

    }
}