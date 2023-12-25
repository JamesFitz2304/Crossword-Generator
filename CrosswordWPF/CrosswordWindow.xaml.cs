
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CrosswordGenerator.Models.Puzzle;

namespace CrosswordWPF
{
    /// <summary>
    /// Interaction logic for CrosswordWindow.xaml
    /// </summary>
    public partial class CrosswordWindow : Window
    {
        private DrawingBlock[,] _drawingBlocks;
        private double _sizeFactor = 1;
        private const double BaseSize = 30;
        private readonly Color _backgroundColor = Colors.DarkSlateGray;

        private readonly IList<Puzzle> _puzzles;
        private int _puzzleIndex = 0;

        public CrosswordWindow(IList<Puzzle> puzzles)
        {
            InitializeComponent();
            _puzzles = puzzles;
            DrawCrossword();
            DrawClues();
        }
        private void DrawCrossword()
        {
            var puzzle = _puzzles[_puzzleIndex];
            var puzzleBlocks = puzzle.Blocks.ToList();
            var blocksWidth = puzzleBlocks.Max(b => b.Coordinate.X);
            var blocksHeight = puzzleBlocks.Max(b => b.Coordinate.Y);
            _drawingBlocks = new DrawingBlock[blocksHeight, blocksWidth];

            CrosswordGrid.Children.Clear();

            var background = new Border
            {
                Background = new SolidColorBrush(_backgroundColor)
            };

            CrosswordGrid.Children.Add(background);
            Grid.SetRowSpan(background, _drawingBlocks.GetLength(0));
            Grid.SetColumnSpan(background, _drawingBlocks.GetLength(1));

            GenerateGridRowsAndColumns();

            foreach (var puzzleBlock in puzzleBlocks)
            {
                var x = puzzleBlock.Coordinate.X - 1;
                var y = puzzleBlock.Coordinate.Y - 1;
                var drawingBlock = new DrawingBlock(puzzleBlock, x, y);
                _drawingBlocks[y, x] = drawingBlock;
                CrosswordGrid.Children.Add(drawingBlock.Grid);
            }

        }

        private void DrawClues()
        {
            
            var wordClues = _puzzles[_puzzleIndex].Words.ToList();
            var x = 0;
            var acrossClues = wordClues.Where(w => w.Across).ToList();

            SetRows(AcrossGrid, acrossClues.Count);

            foreach (var across in acrossClues)
            {
                var textBlock = new TextBlock
                {
                    Text = $"{across.Order}: {across.Clue}"
                };
                textBlock.SetValue(Grid.RowProperty, x);
                AcrossGrid.Children.Add(textBlock);
                x++;
            }


            var downClues = wordClues.Where(w => !w.Across).ToList();
            SetRows(DownGrid, downClues.Count);

            x = 0;
            foreach (var down in downClues)
            {
                var textBlock = new TextBlock
                {
                    Text = $"{down.Order}: {down.Clue}"
                };
                textBlock.SetValue(Grid.RowProperty, x);
                DownGrid.Children.Add(textBlock);
                x++;
            }

            void SetRows(Grid grid, int rows)
            {
                grid.RowDefinitions.Clear();
                grid.Children.Clear();
                for (var i = 0; i < rows; i++)
                {
                    grid.RowDefinitions.Add(new RowDefinition
                    {
                        Height = GridLength.Auto
                    });
                }
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
                    if(drawingBlock != null)
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
            else if (e.Key == Key.Enter)
            {
                GetNextCrossword();

            }
        }

        private void GetNextCrossword()
        {
            _puzzleIndex++;
            if (_puzzleIndex >= _puzzles.Count)
            {
                _puzzleIndex = 0;
            }
            DrawCrossword();
            DrawClues();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (var block in _drawingBlocks)
            {
                if (block != null)
                {
                    block.TextBox.IsReadOnly = true;
                    block.TextBox.Text = block.PuzzleBlock.Character.ToString();
                }
            }
        }
    }
}
