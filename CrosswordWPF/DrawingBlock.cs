using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CrosswordGenerator.Generator.Models;
using CrosswordGenerator.Models.Puzzle;

namespace CrosswordWPF
{
    public class DrawingBlock
    {
        private double _blockSize = 30;
        private readonly Border _foreground;
        private readonly Border _border;
        private readonly TextBlock _textBlock;

        public Grid Grid;

        public string Text => _textBlock.Text;

        public DrawingBlock(PuzzleBlock puzzleBlock, int x, int y)
        {
            Grid = new Grid();
            Grid.SetRow(Grid, y);
            Grid.SetColumn(Grid, x);
            Grid.Width = Grid.Height = _blockSize;
            _border = new Border
            {
                Background = new SolidColorBrush(Colors.Black),
                Margin = new Thickness(-1, -1, 0, 0)
            };
            Grid.Children.Add(_border);
            _foreground = new Border
            {
                Background = new SolidColorBrush(Colors.White),
                Margin = new Thickness(0, 0, 1, 1)
            };
            Grid.Children.Add(_foreground);
            _textBlock = new TextBlock
            {
                Text = puzzleBlock.Character.ToString(),
                Foreground = new SolidColorBrush(Colors.Black)
            };
            _foreground.Child = _textBlock;
            _textBlock.VerticalAlignment = VerticalAlignment.Center;
            _textBlock.HorizontalAlignment = HorizontalAlignment.Center;
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

        public void SetBorder(bool leftEmpty, bool topEmpty)
        {
            double BL = -1, BU = -1, FL = 0, FU = 0;

            if (leftEmpty)
            {
                BL = 0;
                FL = 1;
            }

            if (topEmpty)
            {
                BU = 0;
                FU = 1;
            }

            _border.Margin = new Thickness(BL, BU, 0, 0);
            _foreground.Margin = new Thickness(FL, FU, 1, 1);
        }


    }
}