
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Annotations;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using CrosswordGenerator.GenerationManager;
using CrosswordGenerator.Models.Puzzle;

namespace CrosswordWPF
{
    /// <summary>
    /// Interaction logic for CrosswordWindow.xaml
    /// </summary>
    public partial class CrosswordWindow : Window
    {
        private readonly DrawingBlock[,] _drawingBlocks;
        private double _sizeFactor = 1;
        private const double BaseSize = 30;
        public bool CrosswordFailed = false;
        private readonly Color _backgroundColor = Colors.DarkSlateGray;
        public CrosswordWindow(Puzzle puzzle)
        {
            InitializeComponent();
            var blocksWidth = puzzle.Blocks.Max(b => b.Coordinate.X);
            var blocksHeight = puzzle.Blocks.Max(b => b.Coordinate.Y);
            _drawingBlocks = new DrawingBlock[blocksHeight, blocksWidth];
            DrawCrossword(puzzle);

        }
        private void DrawCrossword(Puzzle puzzle)
        {
            CrosswordGrid.Children.RemoveRange(0, CrosswordGrid.Children.Count);
            var background = new Border
            {
                Background = new SolidColorBrush(_backgroundColor)
            };

            CrosswordGrid.Children.Add(background);
            Grid.SetRowSpan(background, _drawingBlocks.GetLength(0));
            Grid.SetColumnSpan(background, _drawingBlocks.GetLength(1));

            GenerateGridRowsAndColumns();

            foreach (var puzzleBlock in puzzle.Blocks)
            {
                var x = puzzleBlock.Coordinate.X - 1;
                var y = puzzleBlock.Coordinate.Y - 1;
                var drawingBlock = new DrawingBlock(puzzleBlock, x, y);
                _drawingBlocks[y, x] = drawingBlock;
                CrosswordGrid.Children.Add(drawingBlock.Grid);
            }
        }


        private void GenerateGridRowsAndColumns()
        {
            CrosswordGrid.RowDefinitions.Clear();
            CrosswordGrid.ColumnDefinitions.Clear();

            for (var i = 0; i < _drawingBlocks.GetLength(1); i++)
            {
                var column = new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) };
                CrosswordGrid.ColumnDefinitions.Add(column);
            }

            for (var i = 0; i < _drawingBlocks.GetLength(0); i++)
            {
                var row = new RowDefinition { Height = new GridLength(1, GridUnitType.Star) };
                CrosswordGrid.RowDefinitions.Add(row);
            }
        }

        private void ResizeBoard()
        {
            for (var y = 0; y < _drawingBlocks.GetLength(0); y++)
            {
                for (var x = 0; x < _drawingBlocks.GetLength(1); x++)
                {
                    var drawingBlock = _drawingBlocks[y, x];
                    drawingBlock.BlockSize = BaseSize * _sizeFactor;
                }
            }
            CrosswordGrid.Width = double.NaN;
            CrosswordGrid.Height = double.NaN;
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.OemMinus || e.Key == Key.Subtract)
            {
                _sizeFactor = Math.Round(_sizeFactor - 0.1, 1);
                ResizeBoard();
            }
            else if (e.Key == Key.OemPlus || e.Key == Key.Add)
            {
                _sizeFactor = Math.Round(_sizeFactor + 0.1, 1);
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
