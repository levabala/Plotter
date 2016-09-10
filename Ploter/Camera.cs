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
