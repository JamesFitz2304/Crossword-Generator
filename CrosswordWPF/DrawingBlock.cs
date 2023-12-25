using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CrosswordGenerator.Models.Puzzle;

namespace CrosswordWPF
{
    public class DrawingBlock
    {
        public readonly PuzzleBlock PuzzleBlock;
        public TextBox TextBox;
        private double _blockSize = 30;

        public Grid Grid;

        public DrawingBlock(PuzzleBlock puzzleBlock, int x, int y)
        {
            PuzzleBlock = puzzleBlock;
            Grid = new Grid();
            Grid.SetRow(Grid, y);
            Grid.SetColumn(Grid, x);
            Grid.Width = Grid.Height = _blockSize;

            var border = new Border
            {
                Background = new SolidColorBrush(Colors.Black),
                Margin = new Thickness(-1, -1, 0, 0),
                Height = _blockSize,
                Width = _blockSize
            };

            Grid.Children.Add(border);

            var background = new Border
            {
                Background = new SolidColorBrush(Colors.White),
                Margin = new Thickness(0, 0, 1, 1)
            };

            Grid.Children.Add(background);

            var canvas = new Canvas();

            TextBox = new TextBox
            {
                Foreground = new SolidColorBrush(Colors.Black),
                MaxLength = 1,
                BorderBrush = null,
                BorderThickness = new Thickness(0),
                Background = null,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Height = _blockSize,
                Width = _blockSize,
                IsTabStop = false,
                AcceptsTab = false
            };

            canvas.Children.Add(TextBox);

            if (puzzleBlock.WordStarts.Any())
            {
                var wordNumText = new TextBlock
                {
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    FontSize = 8,
                    Text = puzzleBlock.WordStarts.First().Order.ToString()
                };
                canvas.Children.Add(wordNumText);
            }

            background.Child = canvas;
        }

        public double BlockSize
        {
            get => _blockSize;
            set
            {
                _blockSize = value;
                Grid.Height = Grid.Width = _blockSize;
            }
        }

    }
}