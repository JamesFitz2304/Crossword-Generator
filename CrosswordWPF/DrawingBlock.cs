using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CrosswordWPF
{
    public abstract class DrawingBlock
    {
        private double RectSize = 30;
        public Border Square;

        public double LocationX
        {
            set
            {
                Thickness margin = Square.Margin;
                margin.Left = value;
                Square.Margin = margin;
            }
            get => Square.Margin.Left;
        }

        public double LocationY
        {
            set
            {
                Thickness margin = Square.Margin;
                margin.Top = value;
                Square.Margin = margin;
            }
            get => Square.Margin.Top;
        }

        public double SquareSize
        {
            get => RectSize;
            set
            {
                RectSize = value;
                Square.Height = Square.Width = RectSize;
            }
        }

        protected abstract Color Fill { get; }
        protected abstract Color Border { get; }

        protected DrawingBlock()
        {
            SetRectangle();
        }

        private void SetRectangle()
        {
            Square = new Border();
            Square.Height = Square.Width = RectSize;
            Square.BorderBrush = new SolidColorBrush(Border);
            Square.Background = new SolidColorBrush(Fill);
            Square.BorderThickness = new Thickness(1);
        }
    }
}