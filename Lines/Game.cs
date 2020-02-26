using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lines
{
    class Game
    {
        int max = 9;
        int[,] map;
        int MaxColors = 6;
        ShowItem Show;
        Status status;
        Ball[] ball =new Ball[3];
        static Random rand = new Random();
        enum Status
        {
            init,
            wait,
            ballMark,
            pathShow,
            ballMove,
            nextBalls,
            lineStrip,
            stop
        }
        
        public Game(int max, ShowItem Show)
        {
            this.max = max;
            this.Show = Show;
            map = new int[max, max];
            FMap = new int[max, max];
            status = Status.init;
            Path = new Ball[81];
            Strip = new Ball[99];
        }
        public void InitMap()
        {
            Ball none;
            none.color = 0;
            for (int x = 0; x < max; x++)
                for (int y = 0; y < max; y++)
                {
                    map[x, y] = 0;
                    none.x = x;
                    none.y = y;
                    Show(none, Item.none);
                }
                    
            
        }
        Ball marketBall;
        Ball destinationBall;
        int marketJump;
        public void ClickBox(int x,int y)
        {
            if (status == Status.wait||status==Status.ballMark)
            {
                if (map[x, y] > 0)
                {
                    if (status == Status.ballMark)
                    {
                        Show(marketBall, Item.ball);
                    }
                    marketBall.x = x;
                    marketBall.y = y;
                    marketBall.color = map[x, y];
                    status = Status.ballMark;
                    marketJump = 0;
                   return;
                }
            }
            if(status==Status.ballMark)
                if (map[x, y] <= 0)
                {
                    destinationBall.x = x;
                    destinationBall.y = y;
                    destinationBall.color = marketBall.color;
                    if (FindPath())
                        status = Status.pathShow;
                    //status = Status.ballMove;
                    return;
                }
            if (status == Status.stop)
                status = Status.init;
        }//
        public void Step()
        {
            switch (status)
            {
                case Status.init:
                    InitMap();
                    SelectNextBalls();
                    ShowNextBalls();
                    SelectNextBalls();
                    status = Status.wait;
                    break;
                case Status.wait:
                    break;
                case Status.ballMark:
                    JumpBall();
                    break;
                case Status.pathShow:
                    PathShow();
                    break;
                case Status.ballMove:
                    MoveBall();
                    break;
                case Status.lineStrip:
                    StripLines();
                    break;
                case Status.nextBalls:
                    ShowNextBalls();
                    SelectNextBalls();
                    break;
                case Status.stop:
                    break;
            }
        }

        
        private void SelectNextBalls()
        {
            ball[0] = SelectNextBall();
            ball[1] = SelectNextBall();
            ball[2] = SelectNextBall();

          
        }
        private Ball SelectNextBall()
        {
            return SelectNextBall(rand.Next(1,MaxColors+1));
        }
        private Ball SelectNextBall(int color)
        {
            int loop = 100;
            Ball next;
            next.color = color;
            do
            {
                next.x = rand.Next(0, max);
                next.y = rand.Next(0, max);
                if (--loop < 0)
                {
                    next.x = -1;
                    return next;
                }
            }
            while (map[next.x, next.y]!= 0);
            map[next.x, next.y] = -1;
            Show(next, Item.next);
            return next;
        }
        private void ShowNextBalls()
        {
            ShowNextBall(ball[0]);
            ShowNextBall(ball[1]);
            ShowNextBall(ball[2]);
            if (FindStripLines())
                status = Status.lineStrip;
            else
                if (IsMapFull())
                    status = Status.stop;
                else
                    status = Status.wait;
            


        }//
        private void HintNextBalls()
        {
            HintNextBall(ball[0]);
            HintNextBall(ball[1]);
            HintNextBall(ball[2]);
        }
        private void HintNextBall(Ball next)
        {
            if (next.x < 0) return;
            Show(next, Item.next);
        }

        private void ShowNextBall(Ball next)
        {
            if (next.x < 0) return;
            if (map[next.x, next.y] > 0)
            {
                next = SelectNextBall(next.color);
                if (next.x < 0) return;
            }
            map[next.x, next.y] = next.color;
            Show(next, Item.ball);

        }//
        int[,] FMap;
        Ball[] Path;
        int Paths;
        private bool FindPath()
        {
            if (!(map[marketBall.x, marketBall.y] > 0 &&
                map[destinationBall.x, destinationBall.y] <= 0))
                return false;
            for (int x = 0; x < max; x++)
                for (int y = 0; y < max; y++)
                    FMap[x, y] = 0;
                
            bool Added;
            bool Found=false;
            FMap[marketBall.x, marketBall.y] = 1;
            int nr = 1;
            do
            {
                Added = false;
                for (int x = 0; x < max; x++)
                    for (int y = 0; y < max; y++)
                        if (FMap[x, y] == nr)
                        {
                            MarkPath(x + 1, y, nr + 1);
                            MarkPath(x - 1, y, nr + 1);
                            MarkPath(x , y + 1, nr + 1);
                            MarkPath(x , y - 1, nr + 1);
                            Added = true;
                        }
                     
               if(FMap[destinationBall.x, destinationBall.y] > 0)
                {
                    Found = true;
                    break;
                }
                nr++;
            } 
            while (Added);
            if (!Found)
                return false;
            int PX = destinationBall.x;
            int PY = destinationBall.y;

            Paths = nr;
            while (nr >= 0)
            {
                Path[nr].x = PX;
                Path[nr].y = PY;
               
                if (IsPath(PX + 1, PY, nr)) PX++; 
                else
                if (IsPath(PX - 1, PY, nr)) PX--; 
                else
                if (IsPath(PX, PY + 1, nr)) PY++; 
                else
                if (IsPath(PX, PY - 1, nr)) PY--;
                nr--;
            }
            PathStep = 0;
            return true;
        }

        private void MarkPath(int x, int y, int k)
        {
            if (x < 0 || x >= max) return;
            if (y < 0 || y >= max) return;
            if (map[x, y] > 0) return;
            if (FMap[x, y] > 0) return;
            FMap[x, y] = k;
        }//
        private bool IsPath(int x, int y, int k)
        {
            if (x < 0 || x >= max) return false;
            if (y < 0 || y >= max) return false;
            return (FMap[x, y] == k);
        }//
        int PathStep;
        private void PathShow()
        {
            if (PathStep == 0)
            {
                for (int nr = 1; nr <= Paths; nr++)
                    Show(Path[nr], Item.path);
                PathStep++;
                return;               
            }
            Ball MovingBall;

            MovingBall = Path[PathStep - 1];
            Show(MovingBall, Item.none);

            MovingBall = Path[PathStep];
            MovingBall.color = marketBall.color;
            Show(MovingBall, Item.ball);

            PathStep++;
            
            if (PathStep > Paths)
            {
                HintNextBalls();
                status = Status.ballMove;
            }
                
        }//

       

        private void MoveBall()
        {
            if (status != Status.ballMove)
                return;
            if(map[marketBall.x, marketBall.y]>0 && 
                map[destinationBall.x, destinationBall.y]<=0)
            {
                map[marketBall.x, marketBall.y] = 0;
                map[destinationBall.x, destinationBall.y] = marketBall.color;
                Show(marketBall, Item.none);
                Show(destinationBall, Item.ball);
                status = Status.nextBalls;
           
                if (FindStripLines())
                    status = Status.lineStrip;
                else
                    status = Status.nextBalls;
            }

        }//

        private void JumpBall()
        {
            if (status != Status.ballMark)
                return;
            if(marketJump==0)
                Show(marketBall, Item.jump);
            else
                Show(marketBall, Item.ball);
            marketJump = 1 - marketJump;
        }//

        Ball[] Strip;
        int Strips;
        int StripStep;
        private bool FindStripLines()
        {
            Strips = 0;
            for (int x = 0; x < max; x++)
                for (int y = 0; y < max; y++)
                {
                    CheckLine(x, y, 1, 0);
                    CheckLine(x, y, 1, 1);
                    CheckLine(x, y, 0, 1);
                    CheckLine(x, y, -1, 1);
                }
            if (Strips == 0)
                return false;
            StripStep = 4;
            return true;

        }

        private void CheckLine(int x, int y, int sx, int sy)
        {
            int p = 4;
            if (x < 0 || x >= max) return;
            if (y < 0 ||y >= max) return;
            if (x+p*sx < 0 || x+p*sx >= max) return;
            if (y + p * sy < 0 || y + p * sy >= max) return;
            int Color = map[x, y];
            if (Color <= 0) return;
            for (int k = 0; k <= p; k++)
                if (map[x + k * sx, y + k * sy] != Color)
                    return;
            for (int k = 0; k <=p; k++)
            {
                Strip[Strips].x = x + k * sx;
                Strip[Strips].y = y + k * sy;
                Strip[Strips].color = Color;
                Strips++;
            }
          
        }
        private void StripLines()
        {
            if (StripStep <= 0)
            {
                for (int j = 0; j < Strips; j++)
                    map[Strip[j].x, Strip[j].y] = 0;
                HintNextBalls();
                status = Status.wait;
                return;
            }
            StripStep--;
            for (int i = 0; i < Strips; i++)
            {
                switch (StripStep)
                {
                    case 3: Show(Strip[i], Item.jump);break;
                    case 2: Show(Strip[i], Item.ball); break;
                    case 1: Show(Strip[i], Item.next); break;
                    case 0: Show(Strip[i], Item.none); break;
                }
            }
        }

        private bool IsMapFull()
        {
            for (int x = 0; x < max; x++)
                for (int y = 0; y < max; y++)
                    if (map[x, y] <= 0)
                        return false;
            return true;
               
        }
    }
}
