using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Arcanoid
{
    public class Game : Control
    {
        static int _mapRow = 30; //Константы для нашего массива
        static int _mapCol = 30;

        protected int[,] _map;//игровое поле = массив

        protected Color _mapColor;//цвета игровых элементов 
        protected Color _ballColor;
        protected Color _platformColor;
        protected Color _blockColor;

        protected int _mapSizeW = 500;//ширина и высота карты
        protected int _mapSizeH = 601;

        protected int _mapSizeBlockW;//ширина и высота блоков
        protected int _mapSizeBlockH;

        protected int _platformX;//координаты платформы в массиве
        protected int _platformY;

        protected int _platformW;//размеры и координаты платформы 
        protected int _platformH;
        protected int _rxplatform;
        protected int _ryplatform;

        protected int _cfX;//отклоенение шара по оси Х, генерируется в методе IsCollide()
        protected int _cfY;//отклоенение шара по оси Y, генерируется в методе IsCollide()

        protected int _ballX; //размеры и координаты шара
        protected int _ballY;
        protected int _dirX;
        protected int _dirY;
        protected int _radiusBall;
        protected int _gmStatus = 0;

        protected event EventHandler _recordScore;
        protected event EventHandler _recordStatus;

        public System.Timers.Timer _timer; //таймер для обработки

        protected int score = 0;


        public event EventHandler RecordScore//событие 
        {
            add { _recordScore += value; }
            remove { _recordScore -= value; }
        }

        public event EventHandler RecordStatus//событие 
        {
            add { _recordStatus += value; }
            remove { _recordStatus -= value; }
        }

        public int Score//счет, свойство 
        {
            get { return score; }
            private set
            {
                if (score != value)
                {
                    score = value;
                    OnRecordScore();
                }
            }
        }
        public int GmStatus//статус игры, свойство 
        {
            get { return _gmStatus; }
            private set
            {
                if (_gmStatus != value)
                {
                    _gmStatus = value;
                    OnRecordStatus();
                }
            }
        }
        protected void OnRecordScore()//счет
        {
            _recordScore?.Invoke(this, new EventArgs());
        }

        protected void OnRecordStatus()//счет
        {
            _recordStatus?.Invoke(this, new EventArgs());
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)//адаптация, при изменени размера компонента
        {
            if (width < 501 || height < 601)
            {
                width = 501;
                height = 601;
            }

            _mapSizeBlockW = width / _mapCol;
            _mapSizeBlockH = height / _mapRow;

            width = _mapSizeBlockW * _mapCol;
            _radiusBall = _mapSizeBlockH / 3;

            _ryplatform = _mapSizeBlockH * _platformY;
            Invalidate();
            base.SetBoundsCore(x, y, width, height, specified);


        }

        public Game() : base()//конструктор 
        {
            _map = new int[_mapRow, _mapCol];
            MapColor = Color.Black;
            BallColor = Color.Red;
            BlockColor = Color.DarkGreen;
            PlatformColor = Color.Blue;
            _platformX = (_mapCol - 2) / 2;
            _platformY = _mapRow - 3;
            _mapSizeBlockW = _mapSizeW / _mapCol;
            _mapSizeBlockH = _mapSizeH / _mapRow;

            _ballY = ((_mapRow - 4) * _mapSizeBlockH);
            _ballX = (_mapCol / 2) * _mapSizeBlockW;
            _timer = new System.Timers.Timer(1);
            _timer.AutoReset = true;
            _timer.Elapsed += Update;
            _timer.Enabled = false;
            this.KeyDown += new KeyEventHandler(InputCheck);
            InitFunction();//метод для обработки массива
            _dirX = 1;
            _dirY = 1;
            _cfX = 1;
            _cfY = 1;
            Score = 0;
            GmStatus = 0;

        }

        public void StartGame()//начать игру
        {

            _platformX = (_mapCol - 2) / 2;
            _platformY = _mapRow - 3;
            _ballY = ((_mapRow - 4) * _mapSizeBlockH);
            _ballX = ((_mapCol + 1) / 2) * _mapSizeBlockW;
            InitFunction();
            _timer.Enabled = true;
            this.Focus();
            Score = 0;
            GmStatus = 1;
            Invalidate();
        }

        public void StopGame()//закончить игру
        {
            _timer.Enabled = false;
            GmStatus = 0;
        }
        protected void InitFunction()//метод для обработки массива, сначала обнуляет все элементы, потом генерирует блоки
        {
            Random r = new Random();
            for (int i = 0; i < _mapRow; i++)
            {
                for (int j = 0; j < _mapCol; j++)
                {
                    _map[i, j] = 0;
                }
            }
            for (int i = 0; i < _mapRow / 3; i++)
            {
                for (int j = 0; j < _mapCol; j++)
                {
                    _map[i, j] = r.Next(0, 2);

                }
            }
            _map[_platformY, _platformX] = 9;
            _map[_platformY, _platformX + 1] = 99;
            _map[_platformY, _platformX + 2] = 999;
        }
        protected int XToCol(int x, int dir)//преобразование координаты в пикселях по оси X
        {
            return (x + dir * _radiusBall) / _mapSizeBlockW;
        }

        protected int YToRow(int y, int dir)//преобразование координаты в пикселях по оси Y
        {
            return (y + dir * _radiusBall) / _mapSizeBlockH;
        }

        public void CheckGameStatus()//првоерка статуса игры, если блоки сбиты, то начинается новая игра
        {
            if (YToRow(_ballY, _dirY) + _dirY > _mapRow - 1)
            {
                _gmStatus = 0;
                StopGame();
            }

            for (int i = 0; i < _mapRow / 3; i++)
            {
                for (int j = 0; j < _mapCol; j++)
                {
                    if (_map[i, j] == 1)
                    {
                        _gmStatus++;
                        break;
                    }
                }
            }

            if (_gmStatus < 1)
            {
                StartGame(); //победа! все блоки сбиты, начинается следующий уровень
            }
        }

        private void Update(object Source, EventArgs e) // обновление, используется в таймере
        {

            CheckGameStatus();
            _ballX += (int)(_cfX * _dirX);// Менять коэф. движения шарика 
            _ballY += (int)(_cfY * _dirY);// Менять коэф. движения шарика 

            Thread thread = new Thread(() => //создание потока, для инкремента счета, без него все рушиться
            {
                Invoke((Action)(() =>
                                  {
                                      IsCollide();
                                  }));
            });
            thread.Start();
            Invalidate();
        }


        protected void IsCollide() // проверка коллизии, прибавление счета, удаление блока
        {
            Random r = new Random();
            if (_ballX + _radiusBall > Width - 2 || _ballX - _radiusBall < 2)//работает исправно, бывает не успевает менять траекторию, из-за большого количества проверок
            {                                                                  //из-за этого мяч может заедать в стене 
                _dirX *= -1;
            }

            if (_ballY < _radiusBall + 10)//работает исправно 
            {
                _dirY *= -1;
            }

            if (_map[YToRow(_ballY, _dirY), XToCol(_ballX, 0)] != 0)//работает исправно 
            {


                if (_map[YToRow(_ballY, _dirY), XToCol(_ballX, 0)] < 9)
                {
                    Score++;
                    _map[YToRow(_ballY, _dirY), XToCol(_ballX, 0)] = 0;
                }
                _dirY *= -1;
                if (_ballY > (_mapRow / 2) * _mapSizeBlockH)
                {
                    _cfY = r.Next(1, 4);
                    _cfX = r.Next(1, 4);
                }
            }

            if (_map[YToRow(_ballY, -1), XToCol(_ballX, _dirX)] != 0)//работает исправно 
            {
                if (_map[YToRow(_ballY, -1), XToCol(_ballX, _dirX)] < 9)
                {
                    Score++;
                    _map[YToRow(_ballY, -1), XToCol(_ballX, _dirX)] = 0;
                }
                _dirX *= -1;
            }
        }

        private void InputCheck(object Source, KeyEventArgs e) // движение платформы, проверка нажатия на клавишы A - Left, D - Right
        {
            _map[_platformY, _platformX] = 0;
            _map[_platformY, _platformX + 1] = 0;
            _map[_platformY, _platformX + 2] = 0;
            switch (e.KeyCode.ToString())
            {
                case "D":
                    if (_platformX + 1 < _mapCol - 2)
                        _platformX += 1;
                    break;

                case "A":
                    if (_platformX > 0)
                        _platformX -= 1;
                    break;
            }
            _map[_platformY, _platformX] = 9;
            _map[_platformY, _platformX + 1] = 99;
            _map[_platformY, _platformX + 2] = 999;
        }

        public Color MapColor // цвет фона
        {
            get
            {
                return _mapColor;
            }
            set
            {
                if (_mapColor != value)
                {
                    _mapColor = value;
                    Invalidate();
                }
            }
        }
        public Color BallColor //цвет шара
        {
            get
            {
                return _ballColor;
            }
            set
            {
                if (_ballColor != value)
                {
                    _ballColor = value;
                    Invalidate();
                }
            }
        }
        public Color PlatformColor //цвет платформы
        {
            get
            {
                return _platformColor;
            }
            set
            {
                if (_platformColor != value)
                {
                    _platformColor = value;
                    Invalidate();
                }
            }
        }

        public int MapRow //количество строк
        {
            get
            {
                if (_mapRow < 10)
                {
                    _mapRow = 10;
                }
                else if (_mapCol > _mapRow)
                {
                    _mapCol = _mapRow;
                }
                _mapSizeBlockW = Width / _mapCol;
                _mapSizeBlockH = Height / _mapRow;
                _platformX = (_mapCol - 2) / 2;
                _platformY = _mapRow - 3;
                _ryplatform = _mapSizeBlockH * _platformY;
                _ballY = ((_mapRow - 4) * _mapSizeBlockH);
                _ballX = ((_mapCol + 1) / 2) * _mapSizeBlockW;
                _radiusBall = _mapSizeBlockH / 3;
                InitFunction();
                return _mapRow;
            }
            set
            {
                if (_mapRow != value)
                {
                    _mapRow = value;
                    _mapSizeBlockW = Width / _mapCol;
                    _mapSizeBlockH = Height / _mapRow;
                    _platformX = (_mapCol - 2) / 2;
                    _platformY = _mapRow - 3;
                    _ryplatform = _mapSizeBlockH * _platformY;
                    _ballY = ((_mapRow - 4) * _mapSizeBlockH);
                    _ballX = ((_mapCol + 1) / 2) * _mapSizeBlockW;
                    _radiusBall = _mapSizeBlockH / 3;
                    InitFunction();
                    Invalidate();
                }
            }
        }
        public int MapCol //количество колонок
        {
            get
            {
                if (_mapCol < 5)
                {
                    _mapCol = 5;
                }
                else if (_mapCol > _mapRow)
                {
                    _mapCol = _mapRow;
                }
                _mapSizeBlockW = Width / _mapCol;
                _mapSizeBlockH = Height / _mapRow;
                _platformX = (_mapCol - 2) / 2;
                _platformY = _mapRow - 3;
                _ryplatform = _mapSizeBlockH * _platformY;
                _ballY = ((_mapRow - 4) * _mapSizeBlockH);
                _ballX = ((_mapCol + 1) / 2) * _mapSizeBlockW;
                _radiusBall = _mapSizeBlockH / 3;
                InitFunction();
                return _mapCol;
            }
            set
            {
                if (_mapCol != value)
                {
                    _mapCol = value;
                    _mapSizeBlockW = Width / _mapCol;
                    _mapSizeBlockH = Height / _mapRow;
                    _platformX = (_mapCol - 2) / 2;
                    _platformY = _mapRow - 3;
                    _ryplatform = _mapSizeBlockH * _platformY;
                    _ballY = ((_mapRow - 4) * _mapSizeBlockH);
                    _ballX = ((_mapCol + 1) / 2) * _mapSizeBlockW;
                    _radiusBall = _mapSizeBlockH / 3;
                    InitFunction();
                    Invalidate();
                }
            }
        }
        public Color BlockColor // цвет блоков 
        {
            get
            {
                return _blockColor;
            }
            set
            {
                if (_blockColor != value)
                {
                    _blockColor = value;
                    Invalidate();
                }
            }
        }


        protected override void OnPaint(PaintEventArgs e)//рисование 
        {
            Brush bMap = new SolidBrush(BackColor);
            Brush bBlock = new SolidBrush(BackColor);
            Brush bPlatform = new SolidBrush(BackColor);
            Brush bBall = new SolidBrush(BackColor);
            bMap = new SolidBrush(MapColor);
            bBlock = new SolidBrush(BlockColor);
            bPlatform = new SolidBrush(PlatformColor);
            bBall = new SolidBrush(BallColor);
            Pen p = new Pen(Color.Black, 1);

            e.Graphics.FillRectangle(bMap, 0, 0, Width, Height);
            e.Graphics.DrawRectangle(p, 0, 0, Width - 1, Height - 1);

            _platformH = _mapSizeBlockH / 2;
            _platformW = _mapSizeBlockW * 3;

            for (int i = 0; i < _mapRow; i++)//цикл по массиву
            {
                for (int j = 0; j < _mapCol; j++)
                {
                    if (_map[i, j] == 1)//отрисовка блоков
                    {

                        e.Graphics.DrawRectangle(p, j * _mapSizeBlockW, i * _mapSizeBlockH, _mapSizeBlockW, _mapSizeBlockH);
                        e.Graphics.FillRectangle(bBlock, j * _mapSizeBlockW, i * _mapSizeBlockH, _mapSizeBlockW, _mapSizeBlockH);
                        e.Graphics.DrawRectangle(p, j * _mapSizeBlockW - 1, i * _mapSizeBlockH - 1, _mapSizeBlockW, _mapSizeBlockH);

                    }

                    if (_map[i, j] == 9)//отрисовка платформы
                    {
                        _rxplatform = j * _mapSizeBlockW;
                        e.Graphics.DrawRectangle(p, _rxplatform - 1, _ryplatform - 1, _platformW, _platformH);
                        e.Graphics.FillRectangle(bPlatform, _rxplatform, _ryplatform, _platformW, _platformH);
                        e.Graphics.DrawRectangle(p, _rxplatform, _ryplatform, _platformW, _platformH);
                    }
                }
            }

            Rectangle _rectangleForBall = new Rectangle(_ballX - _radiusBall, _ballY - _radiusBall, 2 * _radiusBall, 2 * _radiusBall);
            e.Graphics.DrawEllipse(p, _rectangleForBall);
            e.Graphics.FillEllipse(bBall, _rectangleForBall);

            bBall.Dispose();
            bMap.Dispose();
            bBlock.Dispose();
            bPlatform.Dispose();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }




    }
}
