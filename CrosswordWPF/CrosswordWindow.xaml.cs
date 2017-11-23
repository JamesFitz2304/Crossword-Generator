
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
        private CrosswordGenerator generator;
        private DrawingBlock[,] drawingBlocks;
        private double SizeFactor = 1;
        private double BaseSize = 30;
        public bool CrosswordFailed = false;
        public CrosswordWindow(CrosswordGenerator generator)
        {
            InitializeComponent();
            this.generator = generator;
            if (!GenerateCrossword())
            {
                CrosswordFailed = true;
                return;
            }
            DrawCrossword();
        }

        private bool GenerateCrossword()
        {

            if (!generator.Generate())
            {
                MessageBox.Show("A crossword could not be generated from the given words.");
                this.Close();
                return false;
            }
            drawingBlocks = new DrawingBlock[generator.blocks.GetLength(0), generator.blocks.GetLength(1)];
            return true;
        }

        private void DrawCrossword()
        {
            CrosswordCanvas.Children.RemoveRange(0, CrosswordCanvas.Children.Count);
            CrosswordCanvas.Width = BaseSize * generator.blocks.GetLength(0);
            CrosswordCanvas.Height = BaseSize * generator.blocks.GetLength(1);
            for (int y = 0; y < generator.blocks.GetLength(1); y++)
            {
                for (int x = 0; x < generator.blocks.GetLength(0); x++)
                {
                    DrawingBlock drawingBlock = generator.blocks[x, y] == null ? new BlankDrawingBlock() : (DrawingBlock) new UsedDrawingBlock(generator.blocks[x, y].letter);
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
            CrosswordCanvas.Width = BaseSize * SizeFactor * generator.blocks.GetLength(0);
            CrosswordCanvas.Height = BaseSize * SizeFactor * generator.blocks.GetLength(1);

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
            else if (e.Key == Key.Enter)
            {
                ShuffleAndRegnerate();
            }
        }

        private void ShuffleAndRegnerate()
        {
            generator.blocks = null;
            generator.ShuffleWords();
            GenerateCrossword();
            DrawCrossword();
        }
    }
}
