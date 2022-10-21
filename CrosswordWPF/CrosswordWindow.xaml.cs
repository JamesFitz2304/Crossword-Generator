
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Annotations;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using CrosswordGenerator;

namespace CrosswordWPF
{
    /// <summary>
    /// Interaction logic for CrosswordWindow.xaml
    /// </summary>
    public partial class CrosswordWindow : Window
    {
        private DrawingBlock[,] drawingBlocks;
        private double SizeFactor = 1;
        private double BaseSize = 30;
        public bool CrosswordFailed = false;
        private Color backgroundColor = Colors.Black;
        private IList<Generation> _generations;
        public CrosswordWindow(IList<Generation> generations)
        {
            InitializeComponent();
            _generations = generations;
            var firstGen = generations.First();
            drawingBlocks = new DrawingBlock[firstGen.Blocks.GetLength(0), firstGen.Blocks.GetLength(1)];
            DrawCrossword(firstGen);
        }
        private void DrawCrossword(Generation generation)
        {
            CrosswordGrid.Children.RemoveRange(0, CrosswordGrid.Children.Count);
            Border background = new Border
            {
                Background = new SolidColorBrush(backgroundColor)
            };
            CrosswordGrid.Children.Add(background);
            Grid.SetRowSpan(background, drawingBlocks.GetLength(1));
            Grid.SetColumnSpan(background, drawingBlocks.GetLength(0));

            GenerateGridRowsAndColumns();
            for (int y = 0; y < generation.Blocks.GetLength(1); y++)
            {
                for (int x = 0; x < generation.Blocks.GetLength(0); x++)
                {
                    if (generation.Blocks[x, y] == null) continue;
                    DrawingBlock drawingBlock = new DrawingBlock(generation.Blocks[x, y].letter, x, y);
                    drawingBlocks[x, y] = drawingBlock;
                    CrosswordGrid.Children.Add(drawingBlock.Grid);
                }
            }
            CrosswordGrid.Width = double.NaN;
            CrosswordGrid.Height = double.NaN;
        }


        private void GenerateGridRowsAndColumns()
        {
            CrosswordGrid.RowDefinitions.Clear();
            CrosswordGrid.ColumnDefinitions.Clear();

            for (int i = 0; i < drawingBlocks.GetLength(0); i++)
            {
                ColumnDefinition column = new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) };
                CrosswordGrid.ColumnDefinitions.Add(column);
            }

            for (int i = 0; i < drawingBlocks.GetLength(1); i++)
            {
                RowDefinition row = new RowDefinition { Height = new GridLength(1, GridUnitType.Star) };
                CrosswordGrid.RowDefinitions.Add(row);
            }
        }

        private void ResizeBoard()
        {
            for (int y = 0; y < drawingBlocks.GetLength(1); y++)
            {
                for (int x = 0; x < drawingBlocks.GetLength(0); x++)
                {
                    DrawingBlock drawingBlock = drawingBlocks[x, y];
                    drawingBlock.BlockSize = BaseSize * SizeFactor;
                }
            }
            CrosswordGrid.Width = double.NaN;
            CrosswordGrid.Height = double.NaN;
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
            //else if (e.Key == Key.Enter)
            //{
            //    ShuffleAndRegnerate();

            //}
        }

        //private void ShuffleAndRegnerate()
        //{
        //    manager.Blocks = null;
        //    manager.ShuffleWords();
        //    GenerateCrossword();
        //    DrawCrossword();
        //}

    }
}
