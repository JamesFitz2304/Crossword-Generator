using System.Windows.Media;

namespace CrosswordWPF
{
    public class BlankDrawingBlock : DrawingBlock
    {
        protected override Color Fill => Colors.Black;
        protected override Color Border => Colors.Black;
    }
}