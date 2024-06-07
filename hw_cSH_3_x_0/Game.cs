using System.Net;
using System.Threading.Channels;

namespace hw_cSH_3_x_0;

public static class Game
{
    private static uint _step;
    private static bool _isWin;
    private static char[] _board;
    private static List<int> _availableСells;
    private static HashSet<Tuple<int, int, int>> _combiWin;
    private static char _pcSymbol;
    private static char _playerSymbol;

    
    private static void СolorDefinitionCell(int i, int dopNum=0)
    {
        if (_board[i * 3 + dopNum] == 'x')
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
        }
        else if (_board[i * 3 + dopNum] == 'o')
        {
            Console.ForegroundColor = ConsoleColor.Green;
        }
        Console.Write(_board[i * 3 + dopNum]);
        Console.ResetColor();
    }
    private static void DrawBoard()
    {
        if (_step > 0)
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
        }
        
        for (int i = 0; i < 3; i++)
        {
            Console.Write($"| ");
            СolorDefinitionCell(i);
            Console.Write($" | ");
            СolorDefinitionCell(i, 1);
            Console.Write($" | ");
            СolorDefinitionCell(i, 2);
            Console.WriteLine();
        }
    }
    
    private static int GetCell()
    {
        int numberCell;
        
        if (!int.TryParse(Console.ReadLine(), out numberCell))
        {
            throw new ArgumentException("Некорректный тип данных. Введите целое число!!!");
        } 
        
        if ((numberCell < 1 || numberCell > 9))
        {
            throw new ArgumentException("Число привышает допустимый диапазон!!!");
        }
        
        return numberCell;
    }
    
    private static void ChangeSymbol(ref char currentSymbol)
    {
        if (currentSymbol == 'x')
        {
            currentSymbol = 'o';
        }
        else
        {
            currentSymbol = 'x';
        }
    }

    private static void StepPC()
    {
        Random random = new Random();
        while(true)
        {
            
            try
            {
                // передаем рандомно один из доступных номеров, для заполнения ячейки компьютером 
                FillCell(_availableСells[random.Next(1, _availableСells.Count)], _pcSymbol);
                Console.WriteLine("Ожидаем ход компьютера...");
                Thread.Sleep(random.Next(500, 3000));
            }
            catch (ArgumentException e)
            {
                continue;
            } 
            break;
        }

    }

    private static bool CheckWin()
    {
        foreach (var combo in _combiWin)
        {
            if (_board[combo.Item1-1] == _board[combo.Item2-1] && _board[combo.Item2-1] == _board[combo.Item3-1])
            {
                return true;
            }
        }
        return false;
    }
    
    private static void FillCell(int numberCell, char symbol)
    {
        if ((_board[numberCell-1] == 'x' || _board[numberCell-1] == 'o'))
        {
            throw new ArgumentException($"Поле под номером {numberCell} занято!!!");
        }

        _step += 1;
        _board[numberCell - 1] = symbol;
        _availableСells.Remove(numberCell);
    }
    
    private static char GetUserSymbol()
    {
        string userString = Console.ReadLine();
        char symbol = userString[0];
        if (symbol != 'o' && symbol != 'x')
        {
            throw new ArgumentException("Выберите один из предложеных знаков!!!");
        }
        return symbol;
    }
    
    public static void StartGame()
    {
        _board = new[] { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        _availableСells = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        // комбинации для победы
        _combiWin = new HashSet<Tuple<int, int, int>>()
        {
            new Tuple<int, int, int>(1, 2, 3),
            new Tuple<int, int, int>(4, 5, 6),
            new Tuple<int, int, int>(7, 8, 9),
            
            new Tuple<int, int, int>(1, 4, 7),
            new Tuple<int, int, int>(2, 5, 8),
            new Tuple<int, int, int>(3, 6, 9),
            
            new Tuple<int, int, int>(1, 5, 6),
            new Tuple<int, int, int>(3, 5, 7),
        };
        // номер ячейки которую выбрал юзер
        int numberCell;
        
        // символы pc и юзера
        _pcSymbol = 'o';
        
        Console.WriteLine("Выберите свой знак 'x' или 'o'");
        while (true)
        {
            try
            {
                _playerSymbol = GetUserSymbol();
                Console.WriteLine();
                Console.WriteLine();
                break;
            }
            catch (ArgumentException e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
                Console.ResetColor();
            }
        }
        
        char currentSymbol = _playerSymbol;
        // определяем знак для pc
        if (_playerSymbol == 'o')
        {
            _pcSymbol = 'x';
        }

        
        
        while (_step != 9)
        {
            DrawBoard();
            if (CheckWin()) { break; }

            // проверка на количесво шагов, чтобы не менять знак до первого хода
            if (_step > 0)
            {
                ChangeSymbol(ref currentSymbol);
            }
            
            if (currentSymbol == _playerSymbol)
            {
                Console.Write($"Ваш ход, знак '{_playerSymbol}'.");
                
                while (true)
                {
                    Console.Write("Введите номер ячейки для заполнения: ");
                    try
                    {
                        numberCell = GetCell();
                        FillCell(numberCell, _playerSymbol);
                    }
                    catch (ArgumentException e)
                    {
                        Console.WriteLine();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(e.Message);
                        Console.ResetColor();
                        continue;
                    }
                    break;
                }
            }
            else
            {
                StepPC();
            }
        }
        DrawBoard();

        if (_step == 9)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Ничья!");
            Console.ResetColor();
        }
        else if (currentSymbol == _playerSymbol)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Вы выиграли!");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Вы проиграли!");
            Console.ResetColor();
        }
    }
}