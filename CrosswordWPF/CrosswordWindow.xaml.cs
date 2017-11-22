
using System;
using System.Windows;
using System.Windows.Annotations;
using System.Windows.Input;
using System.Windows.Shapes;
using Crosswords;

namespace CrosswordWPF
{
    /// <summary>
    /// Interaction logic for CrosswordWindow.xaml
    /// </summary>
    public partial class CrosswordWindow : Window
    {
        private Block[,] blocks;
        private DrawingBlock[,] drawingBlocks;
        private double SizeFactor = 1;
        private double BaseSize = 30;
        public CrosswordWindow(Block[,] blocks)
        {
            this.blocks = blocks;
            drawingBlocks = new DrawingBlock[blocks.GetLength(0), blocks.GetLength(1)];
            InitializeComponent();
            DrawCrossword();
            CrosswordCanvas.Width = BaseSize * blocks.GetLength(0);
            CrosswordCanvas.Height = BaseSize * blocks.GetLength(1);
        }

        private void DrawCrossword()
        {
            for (int y = 0; y < blocks.GetLength(1); y++)
            {
                for (int x = 0; x < blocks.GetLength(0); x++)
                {
                    DrawingBlock drawingBlock = blocks[x, y] == null ? new BlankDrawingBlock() : (DrawingBlock) new UsedDrawingBlock(blocks[x, y].letter);
                    drawingBlocks[x, y] = drawingBlock;
                    CrosswordCanvas.Children.Add(drawingBlock.Square);
                    drawingBlock.LocationX = BaseSize * x;
                    drawingBlock.LocationY = BaseSize * y;
                }
            }
        }



        private void ResizeBoard()
        {
            for (int y = 0; y < drawingBlocks.GetLength(1); y++)
            {
                for (int x = 0; x < drawingBlocks.GetLength(0); x++)
                {
                    DrawingBlock drawingBlock = drawingBlocks[x, y];
                    drawingBlock.SquareSize = BaseSize * SizeFactor;
                    drawingBlock.LocationX = drawingBlock.SquareSize * x;
                    drawingBlock.LocationY = drawingBlock.SquareSize * y;
                }
            }
            CrosswordCanvas.Width = BaseSize * SizeFactor * blocks.GetLength(0);
            CrosswordCanvas.Height = BaseSize * SizeFactor * blocks.GetLength(1);

        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.OemMinus || e.Key == Key.Subtract)
            {
                SizeFactor = Math.Round(SizeFactor - 0.1, 1);
                ResizeBoard();
            }
            else if (e.Key == Key.OemPlus || e.Key == Key.Add)
            {
                SizeFactor = Math.Round(SizeFactor + 0.1, 1);
                ResizeBoard();
            }
        }
    }
}
