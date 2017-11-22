using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Crosswords;

namespace CrosswordWPF
{
    public class UsedDrawingBlock : DrawingBlock
    {
        private Letter Letter { get; set; }
        protected override Color Fill => Colors.White;
        protected override Color Border => Colors.Black;
        private readonly TextBlock textBlock = new TextBlock();
        public string Text => textBlock.Text;

        public UsedDrawingBlock(Letter letter)
        {
            this.Letter = letter;
            textBlock.Text = letter.Character.ToString();
            textBlock.Foreground = new SolidColorBrush(Colors.Black);
            Square.Child = textBlock;
            textBlock.VerticalAlignment = VerticalAlignment.Center;
            textBlock.HorizontalAlignment = HorizontalAlignment.Center;
        }
    }
}