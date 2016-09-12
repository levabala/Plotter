using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Ploter
{    
    class Camera
    {
        public PointF leftP, rightP;        
        public List<int> toDraw = new List<int>();
        public float width, height, offsetX, offsetY;
        List<PointF> points;
        private Object mylock = new Object();

        public int CountDelta = 0;

        const float buffer = 20f;

        public Camera(PointF lp, PointF rp, List<PointF> ps)
        {
            leftP = lp;
            rightP = rp;
            width = rightP.X - leftP.X;
            height = rightP.Y - leftP.Y;

            points = ps;
        }

        public void detectPointsToDraw()
        {
            int index = 0;
            toDraw.Clear();
            
            foreach (PointF p in points)
            {
                if (p.X >= leftP.X - buffer && p.X <= rightP.X + buffer) toDraw.Add(index);
                index++;
            }                
        }

        public void detectByOffset(float dx)
        {
            List<int> toRemove = new List<int>();
            if (dx > 0)
            {
                foreach (int i in toDraw)
                    if (points[i].X <= leftP.X + dx - buffer) toRemove.Add(i);
                    else break;

                for (int i = toDraw[toDraw.Count - 1]; i > 0; i--)
                    if (points[i].X >= rightP.X + dx + buffer) toDraw.Remove(i);
                    else break;

                foreach (int r in toRemove) toDraw.Remove(r);
            }
            else if (dx < 0)
            {
                for (int i = toDraw[0]; i > 0; i--)
                    if (points[i].X >= leftP.X + dx - buffer) toDraw.Insert(0,i);
                    else break;

                for (int i = toDraw[toDraw.Count-1]; i > 0; i--)
                    if (points[i].X >= rightP.X + dx + buffer) toDraw.Remove(i);
                    else break;                
            }
        }

        public void detectByOffset(float dr, float dl)
        {
            //Not works
            List<int> toRemove = new List<int>();
            int was = toDraw.Count;
            if (dr > 0)
                for (int i = toDraw[toDraw.Count - 1]; i < points.Count; i++)
                    if (points[i].X <= rightP.X + dr + buffer) toDraw.Add(i);
                    else break;
            else if (dr < 0)
            {
                int si = toDraw.Count;
                int ei = si;
                for (int i = toDraw.Count - 1; i > 0; i--)
                    if (points[toDraw[i]].X >= rightP.X + dr + buffer) ei = i;
                    else break;                
                toDraw.RemoveRange(ei, si-ei);
            }
            if (dl > 0)
                foreach (int i in toDraw)
                    if (points[i].X <= leftP.X + dl - buffer) toRemove.Add(i);
                    else break;
            else if (dl < 0)
                for (int i = toDraw[0]; i > 0; i--)
                    if (points[i].X >= leftP.X + dl - buffer) toDraw.Insert(0, i);
                    else break;

            CountDelta = toDraw.Count - was;
        }

        public void Move(float dx, float dy)
        {
            leftP.X += dx;
            rightP.X += dx;
            leftP.Y += dy;
            rightP.Y += dy;

            offsetX += dx;
            offsetY += dy;
        }

        public void Move(float dl, float dr, float dt, float db)
        {
            leftP.X += dl;
            rightP.X += dr;
            leftP.Y += db;
            rightP.Y += dt;

            offsetX += dl;
            offsetY += db;
        }

        public void MoveTo(PointF pl, PointF pr)
        {
            offsetX += pl.X - leftP.X;
            offsetY += pl.Y - rightP.Y;

            //it not works
            //detectByOffset(pl.X - leftP.X, pr.X - rightP.X);

            leftP = pl;
            rightP = pr;

            width = pr.X - pl.X;
            height = pr.Y - pl.Y;            
        }

        public void Scale(float sx, float sy)
        {
            float dx = width - width * sx;
            float dy = height - height * sy;

            leftP.X += dx;
            rightP.X -= dx;
            leftP.Y -= dy;
            rightP.Y += dy;

            offsetX -= dx;
            offsetY -= dy;

            width += dx;
            height += dy;
        }

        public override string ToString()
        {
            return leftP.X.ToString() + ":" + leftP.Y.ToString() + " " + rightP.X.ToString() + ":" + rightP.Y.ToString() + " drawed:" + toDraw.Count.ToString();
        }
    }
}
