using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CrosswordGenerator.Models.Puzzle;

namespace CrosswordWPF
{
    public class DrawingBlock
    {
        private readonly PuzzleBlock _puzzleBlock;
        private double _blockSize = 30;

        public Grid Grid;

        public DrawingBlock(PuzzleBlock puzzleBlock, int x, int y)
        {
            _puzzleBlock = puzzleBlock;
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

            var textBox = new TextBox
            {
                Foreground = new SolidColorBrush(Colors.Black),
                MaxLength = 1,
                BorderBrush = null,
                Background = null,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Height = _blockSize,
                Width = _blockSize
            };

            canvas.Children.Add(textBox);

            if (puzzleBlock.WordStart != null)
            {
                var wordNumText = new TextBlock
                {
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    FontSize = 8,
                    Text = puzzleBlock.WordStart.Order.ToString()
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