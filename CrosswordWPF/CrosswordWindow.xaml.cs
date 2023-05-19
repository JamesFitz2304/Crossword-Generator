
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Annotations;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using CrosswordGenerator.GenerationManager;
using Color = System.Windows.Media.Color;
using Point = System.Drawing.Point;

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
        private Color backgroundColor = Colors.DarkSlateGray;
        private IList<Generation> _generations;
        public CrosswordWindow(Generation generation)
        {
            InitializeComponent();
            drawingBlocks = new DrawingBlock[generation.YSize, generation.XSize];
            DrawCrossword(generation);

        }
        private void DrawCrossword(Generation generation)
        {
            CrosswordGrid.Children.RemoveRange(0, CrosswordGrid.Children.Count);
            Border background = new Border
            {
                Background = new SolidColorBrush(backgroundColor)
            };
            CrosswordGrid.Children.Add(background);
            Grid.SetRowSpan(background, drawingBlocks.GetLength(0));
            Grid.SetColumnSpan(background, drawingBlocks.GetLength(1));

            GenerateGridRowsAndColumns();
            for (int y = 1; y <= generation.YSize; y++)
            {
                for (int x = 1; x <= generation.XSize; x++)
                {
                    var point = new Point(x, y);
                    if (!generation.Blocks.TryGetValue(point, out var block)) continue;
                    DrawingBlock drawingBlock = new DrawingBlock(block, x-1, y-1);
                    drawingBlocks[y-1, x-1] = drawingBlock;
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

            for (int i = 0; i < drawingBlocks.GetLength(1); i++)
            {
                ColumnDefinition column = new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) };
                CrosswordGrid.ColumnDefinitions.Add(column);
            }

            for (int i = 0; i < drawingBlocks.GetLength(0); i++)
            {
                RowDefinition row = new RowDefinition { Height = new GridLength(1, GridUnitType.Star) };
                CrosswordGrid.RowDefinitions.Add(row);
            }
        }

        private void ResizeBoard()
        {
            for (int y = 0; y < drawingBlocks.GetLength(0); y++)
            {
                for (int x = 0; x < drawingBlocks.GetLength(1); x++)
                {
                    DrawingBlock drawingBlock = drawingBlocks[y, x];
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
