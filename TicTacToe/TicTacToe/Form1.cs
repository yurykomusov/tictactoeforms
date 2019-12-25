using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TicTacToe
{
    public partial class Form1 : Form
    {
        TicTacToe _game;
        byte _currentPlayer;

        public Form1()
        {
            InitializeComponent();

            var size = 10;
            _currentPlayer = 1; // x starts first
            _game = new TicTacToe(size);
            
            CreateBoard(size);
        }

        public void CreateBoard(int size)
        {
            var offset = 10;
            var buttonSize = 100;

            for (var i = 0; i < size; i++)
            {
                var positionLeft = offset;

                for (var j = 0; j < size; j++)
                {
                    var positionTop = offset;

                    var button = new Button();
                    button.Height = buttonSize;
                    button.Width = buttonSize;
                    button.Left = i * (buttonSize + offset) + offset;
                    button.Top = j * (buttonSize + offset) + offset;
                    button.Tag = new int[2] { i, j };

                    button.Click += Button_Click;

                    this.Controls.Add(button);
                }
            }
        }

        public void ResetBoard()
        {
            foreach (var button in this.Controls)
            {
                if (button is Button)
                {
                    ((Button)button).Text = "";
                }
            }
            _game.Reset();
        }

        private void Button_Click(object sender, EventArgs e)
        {
            var button = ((Button)sender);
            var x = ((int[])button.Tag)[0];
            var y = ((int[])button.Tag)[1];

            Console.WriteLine("clicked!");

            if (_game.NextTurn(x, y))
            {
                switch (_currentPlayer)
                {
                    case 1: 
                        button.Text = "X";
                        break;
                    case 0:
                        button.Text = "O";
                        break;
                }
                _currentPlayer = _game.CurrentPlayer;
            }

            byte? winner = null;

            if (_game.IsGameOver(out winner))
            {
                if (winner == null)
                {
                    MessageBox.Show("Draw!");
                }
                else
                {
                    MessageBox.Show($"Winner is {(winner == 1 ? 'X' : 'O')}", "Game over", MessageBoxButtons.RetryCancel);
                }
                ResetBoard();
            }
        }
    }

    public class TicTacToe
    {
        private byte?[,] _state;
        private int _size;
        private byte _currentPlayer; // 0 - o , X - 1

        public TicTacToe(int size)
        {
            _size = size;
            _state = new byte?[size, size];
            _currentPlayer = 1;
        }

        public byte CurrentPlayer { get { return _currentPlayer; } }

        public bool NextTurn(int x, int y)
        {
            if (_state[x, y].HasValue)
                return false; // forbidden move

            _state[x, y] = _currentPlayer;

            _currentPlayer = (byte)Math.Abs(_currentPlayer - 1);

            return true;
        }

        public bool IsGameOver(out byte? winner) // winner: 0 - o wins, 1 - zero null - draws
        {
            return CheckVertical(out winner) ||
                CheckHorizontal(out winner) ||
                CheckDiagonals(out winner);
        }

        private bool CheckHorizontal(out byte? winner)
        {
            byte? candidate = null;
            int rowSize = 0;
            winner = null;

            for (var i = 0; i < _size; i++)
            {
                rowSize = 0;
                for (int j = 0; j < _size; j++)
                {
                    if (!_state[i, j].HasValue)
                    {
                        rowSize = 0;
                        candidate = null;
                    }
                    else if (candidate == _state[i, j])
                    {
                        rowSize++;

                        if (rowSize == 3)
                        {
                            winner = candidate;
                            return true;
                        }
                    }
                    else
                    {
                        candidate = _state[i, j];
                        rowSize = 1;
                    }
                }
            }
            return false;
        }

        private bool CheckVertical(out byte? winner)
        {
            byte? candidate = null;
            int rowSize = 0;
            winner = null;

            for (var i = 0; i < _size; i++)
            {
                rowSize = 0;

                for (int j = 0; j < _size; j++)
                {
                    if (_state[j, i].HasValue)
                    {
                        if (candidate == _state[j, i])
                        {
                            rowSize++;

                            if (rowSize == 3)
                            {
                                winner = candidate;
                                return true;
                            }
                        }
                        else
                        {
                            candidate = _state[j, i];
                            rowSize = 1;
                        }

                    }
                    else
                    {
                        rowSize = 0;
                        candidate = null;
                    }
                }
            }

            return false;
        }

        private bool CheckDiagonals(out byte? winner)
        {
            for (var i = 0; i < _size; i++)
            {
                var result = 
                    CheckSingleDiagonal(0, i, out winner) || 
                    CheckSingleDiagonal(i, 0, out winner) || 
                    CheckSingleDiagonal2(0, i, out winner) ||
                    CheckSingleDiagonal2(i, _size, out winner);

                if (result == true) 
                    return result;
            }
            winner = null;
            return false;
        }

        private bool CheckSingleDiagonal(int startLeft, int startTop, out byte? winner)
        {
            var x = startLeft;
            var y = startTop;
            byte? candidate = winner = null;
            int rowSize = 0;

            while (x < _size && y < _size)
            {
                if (!_state[x, y].HasValue)
                {
                    candidate = null;
                    rowSize = 0;
                }
                else if (_state[x, y] == candidate)
                {
                    rowSize++;
                    if (rowSize == 3)
                    {
                        winner = candidate;
                        return true;
                    }
                }
                else
                {
                    candidate = _state[x, y];
                    rowSize = 1;
                }
                x++;
                y++;
            }
            return false;
        }

        private bool CheckSingleDiagonal2(int startLeft, int startTop, out byte? winner)
        {
            var x = startLeft;
            var y = startTop;
            byte? candidate = winner = null;
            int rowSize = 0;

            while (x >= 0 && y < _size)
            {
                if (!_state[x, y].HasValue)
                {
                    candidate = null;
                    rowSize = 0;
                }
                else if (_state[x, y] == candidate)
                {
                    rowSize++;
                    if (rowSize == 3)
                    {
                        winner = candidate;
                        return true;
                    }
                }
                else
                {
                    candidate = _state[x, y];
                    rowSize = 1;
                }
                x--;
                y++;
            }
            return false;
        }

        public void Reset()
        {
            _state = new byte?[_size, _size];
            _currentPlayer = 1;
        }
    }
}
