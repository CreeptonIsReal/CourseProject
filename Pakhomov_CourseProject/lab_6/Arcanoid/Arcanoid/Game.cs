using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Arcanoid
{
  public class Game : Control
  {
    const int _mapRow = 30; //Константы для нашего массива
    const int _mapCol = 11;

    //protected int x1, y1, x2, y2; //координаты для игровых элементов

    protected int[,] _map = new int[_mapRow, _mapCol];//игровое поле = массив

    protected Color _mapColor;//цвета игровых элементов 
    protected Color _ballColor;
    protected Color _platformColor;
    protected Color _blockColor;

    protected int _mapSizeW = 401;//ширина и высота карты
    protected int _mapSizeH = 601;

    protected int _mapSizeBlockW;//ширина и высота блоков
    protected int _mapSizeBlockH;

    protected int _platformX;//координаты платформы в массиве
    protected int _platformY;

    protected int _platformW;//размеры и координаты платформы 
    protected int _platformH;
    protected int _rxplatform;
    protected int _ryplatform;

    protected float _cfX;
    protected float _cfY;

    protected int _ballX;
    protected int _ballY;
    protected int _dirX;
    protected int _dirY;
    protected int _radiusBall;

    protected event EventHandler _recordScore;

    protected System.Timers.Timer _timer; //таймер для обработки

    protected int score = 1;


    public event EventHandler RecordScore
    {
      add { _recordScore += value; }
      remove { _recordScore -= value; }
    }

    public int Score
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
    protected void OnRecordScore()
    {
      _recordScore?.Invoke(this, new EventArgs());
    }

    protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
    {
      if (width < 401 || height < 601)
      {
        width = 401;
        height = 601;
      }

      _mapSizeBlockW = width / _mapCol;
      _mapSizeBlockH = height / _mapRow;

      //_radiusBall = _mapSizeBlockH; //

      //_rxplatform = (int)_mapSizeBlockW * (_mapCol / 2) - (int)_mapSizeBlockW;
      _ryplatform = _mapSizeBlockH * _platformY;
      Invalidate();
      base.SetBoundsCore(x, y, width, height, specified);
    }
    public Game() : base()
    {
      MapColor = Color.Black;
      BallColor = Color.Red;
      BlockColor = Color.DarkGreen;
      PlatformColor = Color.Blue;
      _platformX = (_mapCol - 2) / 2;
      _platformY = _mapRow - 3;
      _mapSizeBlockW = _mapSizeW / _mapCol;
      _mapSizeBlockH = _mapSizeH / _mapRow;

      _radiusBall = _mapSizeBlockH / 2;

      _ballY = (_mapRow / 2 * _mapSizeBlockH);
      _ballX = (_mapSizeBlockW * 3) * 3;

      _timer = new System.Timers.Timer(1);
      _timer.AutoReset = true;
      _timer.Elapsed += update;
      _timer.Enabled = false;
      this.KeyDown += new KeyEventHandler(inputCheck);
      InitFunction();//метод для обработки массива
      _dirX = 1;
      _dirY = 1;
      _cfX = 1f;
      _cfY = 1f;
    }

    public void StartGame()
    {
      InitFunction();
      _timer.Enabled = true;
      this.Focus();
      Invalidate();
    }

    public void StopGame()
    {
      _ballY = (_mapRow / 2 * _mapSizeBlockH);
      _ballX = ((_mapCol+1)/2) * _mapSizeBlockW;
      _timer.Enabled = false;
      Invalidate();
    }
    protected void InitFunction()//метод для обработки массива
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
    protected int XToCol(int x, int dir)
    {
      return (x + dir * _radiusBall) / _mapSizeBlockW;
    }

    protected int YToRow(int y, int dir)
    {
      return (y + dir * _radiusBall) / _mapSizeBlockH;
    }

    protected void CheckGameStatus()
    {
      if (YToRow(_ballY, _dirY) + _dirY > _mapRow - 1)
      {
        _timer.Stop();//Гейм овер(проигрыш)доработать
      }

      int _gmStatus = 0;

      for (int i = 0; i < _mapRow / 3; i++)
      {
        for (int j = 0; j < _mapCol; j++)
        {
          if (_map[i, j] == 1)
          {
            _gmStatus++;
          }
        }
      }

      if (_gmStatus < 1)
      {
        _timer.Stop(); //победа! все блоки сбиты
      }

    }

    private void update(object Source, EventArgs e)
    {

      CheckGameStatus();
      IsCollide();
      _ballX += (int)(_cfX * _dirX);// Менять коэф. движения шарика x = k/y + b _cfX
      _ballY += (int)(_cfY * _dirY);// Менять коэф. движения шарика y = kx + b _cfY

      _map[_platformY, _platformX] = 9;
      _map[_platformY, _platformX + 1] = 99;
      _map[_platformY, _platformX + 2] = 999;

      Invalidate();
    }

    protected void IsCollide()
    {
      Random r = new Random();
      //bool IsColliding = false;
      if (_ballX + _radiusBall > Width - 2 || _ballX - _radiusBall < 0)
      {
        _dirX *= -1;
        //IsColliding = true;
      }

      if (_ballY < _radiusBall)
      {
        _dirY *= -1;
      }

      if (_map[YToRow(_ballY, _dirY), XToCol(_ballX, 0)] != 0)//+
      {


        if (_map[YToRow(_ballY, _dirY), XToCol(_ballX, 0)] < 9)
        {
          _map[YToRow(_ballY, _dirY), XToCol(_ballX, 0)] = 0;
        }

        _dirY *= -1;
        if(_ballY > (_mapRow/2)*_mapSizeBlockH)
        {
          _cfY = r.Next(1, 6);
          _cfX = r.Next(1, 6);
        }
        Score++;
        //IsColliding = true;
      }

      if (_map[YToRow(_ballY, -1), XToCol(_ballX, _dirX)] != 0)
      {


        if (_map[YToRow(_ballY, -1), XToCol(_ballX, _dirX)] < 9)
        {
          _map[YToRow(_ballY, -1), XToCol(_ballX, _dirX)] = 0;
        }
        _dirX *= -1;
        Score++;
        //IsColliding = true;
      }
      Invalidate();

      //return IsColliding;
    }

    private void inputCheck(object Source, KeyEventArgs e)
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
        case "F2":
          _timer.Stop();
          break;
        case "F1":
          _timer.Start();
          break;
      }
      _map[_platformY, _platformX] = 9;
      _map[_platformY, _platformX + 1] = 99;
      _map[_platformY, _platformX + 2] = 999;
      //Invalidate();
    }

    public Color MapColor
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
    public Color BallColor
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
    public Color PlatformColor
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
    public Color BlockColor
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


    protected override void OnPaint(PaintEventArgs e)
    {
      Brush bMap = new SolidBrush(BackColor);
      Brush bBlock = new SolidBrush(BackColor);
      Brush bPlatform = new SolidBrush(BackColor);
      Brush bBall = new SolidBrush(BackColor);
      e.Graphics.FillRectangle(bMap, ClientRectangle);
      bMap = new SolidBrush(MapColor);
      bBlock = new SolidBrush(BlockColor);
      bPlatform = new SolidBrush(PlatformColor);
      bBall = new SolidBrush(BallColor);
      Pen p = new Pen(Color.Black, 1);

      e.Graphics.FillRectangle(bMap, 0, 0, Width, Height);
      e.Graphics.DrawRectangle(p, 0, 0, Width - 1, Height - 1);

      _platformH = _mapSizeBlockH;
      _platformW = _mapSizeBlockW * 3;

      for (int i = 0; i < _mapRow; i++)
      {
        for (int j = 0; j < _mapCol; j++)
        {
          if (_map[i, j] == 1)//отрисовка блоков
          {
            e.Graphics.FillRectangle(bBlock, j * _mapSizeBlockW, i * _mapSizeBlockH, _mapSizeBlockW, _mapSizeBlockH);
            e.Graphics.DrawRectangle(p, j * _mapSizeBlockW, i * _mapSizeBlockH, _mapSizeBlockW, _mapSizeBlockH);
          }

          if (_map[i, j] == 9)//отрисовка платформы
          {
            _rxplatform = j * _mapSizeBlockW;
            e.Graphics.FillRectangle(bPlatform, _rxplatform, _ryplatform, _platformW, _platformH);
            e.Graphics.DrawRectangle(p, _rxplatform, _ryplatform, _platformW, _platformH);
          }
        }
      }

      Rectangle _rectangleForBall = new Rectangle(_ballX - _radiusBall, _ballY - _radiusBall, 2 * _radiusBall, 2 * _radiusBall);
      e.Graphics.DrawEllipse(p, _rectangleForBall);
      e.Graphics.FillEllipse(bBall, _rectangleForBall);

      String s = XToCol(_ballX, _dirX).ToString();
      Font font = new Font("Times New Roman", 12);
      e.Graphics.DrawString(s, font, bBall, 50, 350);

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
