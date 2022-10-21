using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CrosswordGenerator;

namespace CrosswordWPF
{
    public class DrawingBlock
    {
        private double blockSize = 30;
        private Border foreground;
        private Border border;
        public Grid Grid;

        private Letter Letter { get; set; }

        private readonly TextBlock textBlock;

        public string Text => textBlock.Text;

        public DrawingBlock(Letter letter, int x, int y)
        {
            Grid = new Grid();
            Grid.SetRow(Grid, y);
            Grid.SetColumn(Grid, x);
            Grid.Width = Grid.Height = blockSize;
            border = new Border
            {
                Background = new SolidColorBrush(Colors.Black),
                Margin = new Thickness(-1, -1, 0, 0)
            };
            Grid.Children.Add(border);
            foreground = new Border
            {
                Background = new SolidColorBrush(Colors.White),
                Margin = new Thickness(0, 0, 1, 1)
            };
            Grid.Children.Add(foreground);
            this.Letter = letter;
            textBlock = new TextBlock
            {
                Text = letter.Character.ToString(),
                Foreground = new SolidColorBrush(Colors.Black)
            };
            foreground.Child = textBlock;
            textBlock.VerticalAlignment = VerticalAlignment.Center;
            textBlock.HorizontalAlignment = HorizontalAlignment.Center;
        }


        public double BlockSize
        {
            get => blockSize;
            set
            {
                blockSize = value;
                Grid.Height = Grid.Width = blockSize;
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

            border.Margin = new Thickness(BL, BU, 0, 0);
            foreground.Margin = new Thickness(FL, FU, 1, 1);
        }


    }
}