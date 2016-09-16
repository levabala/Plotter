﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Drawing.Drawing2D;
using Microsoft.FSharp.Core;

namespace Ploter
{
    public partial class Form1 : Form
    {
        Camera cam;
        Matrix m;
        List<PointF> points = new List<PointF>();
        const float bufferPerc = 0.01f; //buffer in percentage       
        float max = 0f;
        float min = 0f;
        Timer wheelEndTimer = new Timer();
        string PLOTTER_TRANSFORM_REGKEY = "plotter_tranform";

        //properties
        bool smoothing = false;
        bool onebyone = true;

        public Form1()
        {
            cam = new Camera(new PointF(0f, min), new PointF(points.Count, max), points, 1);
            cam.onebyoneMode = onebyone;

            InitializeComponent();                        
            MouseWheel += Form1_MouseWheel;

            wheelEndTimer.Interval = 200; 
            wheelEndTimer.Tick += WheelEndTimer_Tick;

            //props            
            //smoothing = SmoothingCheckBox.Checked;
            //onebyone = OneByOneCheckBox.Checked;
            //SmoothingCheckBox.CheckedChanged += SmoothingCheckBox_CheckedChanged;
            //OneByOneCheckBox.CheckedChanged += OneByOneCheckBox_CheckedChanged;
        }

        private void OneByOneCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            //onebyone = OneByOneCheckBox.Checked;
            cam.detectPointsToDraw();
            GetToDrawPoints();
            Invalidate();
        }

        private void SmoothingCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            //smoothing = SmoothingCheckBox.Checked;
            GetToDrawPoints();
            Invalidate();
        }

        private void WheelEndTimer_Tick(object sender, EventArgs e)
        {
            wheelEndTimer.Stop();

            PointF newlp = DataPoint(new PointF(0f, 0f));
            PointF newrp = DataPoint(new PointF(ClientSize.Width, ClientSize.Height));

            cam.detectStep = points.Count / ClientSize.Width;
            cam.MoveTo(newlp, newrp);
            cam.detectPointsToDraw();

            GetToDrawPoints();

            Invalidate();        
        }

        private void Begin()
        {
            cam = new Camera(new PointF(0f, min), new PointF(points.Count, max), points, points.Count / ClientSize.Width);
            cam.onebyoneMode = onebyone;

            m = new Matrix();

            try
            {                
                string ms = (string)Microsoft.Win32.Registry.CurrentUser.GetValue(PLOTTER_TRANSFORM_REGKEY);
                string[] mv = ms.Split(' ');
                float[] mf = mv.Select(a => float.Parse(a)).ToArray();
                m = new Matrix(mf[0], mf[1], mf[2], mf[3], mf[4], mf[5]);
            }
            catch(Exception e)
            {
                MessageBox.Show("Error with martix loading", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                m = new Matrix();
                //invert axis Y
                m.Scale(1, -1);
                m.Translate(0, -ClientSize.Height);

                //resizing for camera size
                m.Scale(ClientSize.Width / cam.width, ClientSize.Height / cam.height);

                Form1_MouseWheel(this, new MouseEventArgs(MouseButtons.None, 0, ClientSize.Width / 2, ClientSize.Height / 2, -1));
            }            

            cam.drawW = DataPoint(new PointF(cam.rightP.X - cam.leftP.X, 0f)).X;
            cam.drawH = ClientSize.Height * m.Elements[4];
            cam.detectPointsToDraw();
        }        

        private void Form1_MouseWheel(object sender, MouseEventArgs e)
        {
            PointF pos = DataPoint(e.Location);
            bool inXscale = e.Location.Y < 50;
            bool inYscale = e.Location.X < 50;
            float z = e.Delta > 0 ? 1.1f : 1.0f / 1.1f;
            float kx = z;
            float ky = z;
            if (ModifierKeys.HasFlag(Keys.Control) || inXscale) ky = 1; //!(m.Elements[1] > -1e-5 && m.Elements[1] < -0.05) 
            if (ModifierKeys.HasFlag(Keys.Shift) || inYscale) kx = 1; //!(m.Elements[0] > 0.05 && m.Elements[0] < 1000)
            PointF po = DataPoint(e.Location);
            m.Translate(po.X, po.Y);
            m.Scale(kx, ky);
            m.Translate(-po.X, -po.Y);

            cam.drawW = DataPoint(cam.rightP).X - DataPoint(cam.leftP).X;
            cam.drawH = ClientSize.Height * m.Elements[4];

            GetToDrawPoints();
            Invalidate();

            wheelEndTimer.Stop();
            wheelEndTimer.Start();
        }

        public PointF DataPoint(PointF scr)
        {
            Matrix mr = m.Clone();
            mr.Invert();
            PointF[] po = new PointF[] { new PointF(scr.X, scr.Y) };
            mr.TransformPoints(po);
            return po[0];
        }
		public PointF ScreenPoint(PointF scr)
		{
			Matrix mr = m.Clone();
			//mr.Invert();
			PointF[] po = new PointF[] { new PointF(scr.X, scr.Y) };
			mr.TransformPoints(po);
			return po[0];
		}

        private void Form1_Load(object sender, EventArgs e)
        {
			string data_source = @"D:\work\test_6000_fast.raw";

			string[] args = Environment.GetCommandLineArgs();
			if (args.Length > 0 && System.IO.File.Exists(args[1])) data_source = args[1];

            Int64[] F5 = Parser.parseLM(data_source, 242);

            max = 0f;
            min = F5[1] - F5[0];
            float cr = 0f;
            for (int i = 1; i < F5.Length; i += 1)
            {
                //deltaF5[i / 2] = F5[i] - F5[i - 1];
                cr = (float)(60.0/((F5[i] - F5[i - 1])*1024*16e-9));
				float tt = (float)(F5[i] * 16e-9);
                //if (cr < 10000)                                 //for debug
                //{
                    //points.Add(new PointF((i - 1) , cr));
					points.Add(new PointF(i, cr));
                    if (cr > max) max = cr;
                    else if (cr < min) min = cr;
                //}
            }
            Begin();
            GetToDrawPoints();

			xGridPen.DashPattern = new float[] { 1, 5, 1 };
			yGridPen.DashPattern = new float[] { 1, 5, 1 };			

            Invalidate();
        }

        PointF[] toDrawArr;
        const float compCoeff = 1;
        private void GetToDrawPoints()
        {
            List<PointF> toDraw = new List<PointF>();
			Rectangle r = ClientRectangle;
            int step = (int)Math.Ceiling((double)(cam.toDraw.Count / r.Width * compCoeff));
            if (step == 0) step = 1;
            //Text = "toDraw.Count = "+cam.toDraw.Count.ToString()+ " step = " + step + " drawed = " + (cam.toDraw.Count / step).ToString();

            float currSum = 0;
            for (int i = 0; i < cam.toDraw.Count - step; i += step)
            {
                if (!smoothing)
                {
                    toDraw.Add(points[cam.toDraw[i]]);
                }
                else
                {
                    currSum = 0;
                    for (int a = i; a < i + step; a++)
                        currSum += points[cam.toDraw[a]].Y;
                    currSum /= step;
                    toDraw.Add(new PointF(points[cam.toDraw[i]].X + step / 2, currSum));
                }                
                //toDraw.Add(new PointF(points[cam.toDraw[i+1]].X, points[cam.toDraw[i]].Y));
            }
            toDrawArr = toDraw.ToArray();
        }

        Pen myPen = new Pen(Color.Green, 1);
		Font smallFont = new Font("Arial Narrow", 8);
		Pen xGridPen = new Pen(Brushes.Blue, 1);
		Pen yGridPen = new Pen(Brushes.Green, 1);
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            //Text = cam.ToString();            
            myPen.LineJoin = LineJoin.Bevel;
            Graphics g = e.Graphics;
            g.Clear(Color.White);      
            g.DrawString(
                m.Elements.Select(a => a.ToString() + "\n").Aggregate((a,b)=>a+b),
                this.Font, Brushes.Red, 50, 50
                );

			/*
            for (float w = ClientSize.Width / 10; w < ClientSize.Width; w += ClientSize.Width / 10)
                g.DrawString(Math.Round(DataPoint(new PointF(w, 0)).X, 2).ToString(), Font, Brushes.Black, new PointF(w, ClientSize.Height - 20));
            for (float h = ClientSize.Height / 10; h < ClientSize.Height; h += ClientSize.Height / 10)
                g.DrawString(Math.Round(DataPoint(new PointF(0,h)).Y, 2).ToString(), Font, Brushes.Black, new PointF(10, h));
			 * */

			PointF axes_step_pt = new PointF(0, 0);
			PointF axes_step_pt2 = new PointF(100, 50);
			PointF axes_step = DataPoint(axes_step_pt);
			PointF axes_step2 = DataPoint(axes_step_pt2);

			float dx = axes_step2.X - axes_step.X;
			float dy = axes_step2.Y - axes_step.Y;

			int power = 10;

			double lg_x = Math.Log(Math.Abs(dx), power);
			double lg_y = Math.Log(Math.Abs(dy), power);

			double pow_x = Math.Pow(power, Math.Floor(lg_x)+1);
			double pow_y = Math.Pow(power, Math.Floor(lg_y)+1);

			int step_x = (int)(pow_x/2);
			if (step_x < 1) step_x = 1;
			int step_y = (int)(pow_y/2);
			if (step_y < 1) step_y = 1;

			int x = 0;
			PointF xp = ScreenPoint(new PointF(x, 0));
			do
			{
				xp = ScreenPoint(new PointF(x, 0));
				//g.DrawLine(Pens.Blue, xp.X, xp.Y, xp.X, xp.Y + 20);
				g.DrawLine(Pens.Blue, xp.X, 0, xp.X, 10);
				g.DrawLine(xGridPen, xp.X, 10, xp.X, ClientRectangle.Height);
				g.DrawString(x.ToString(), smallFont, Brushes.Blue, xp.X, +10);
				x += step_x;
			} while (xp.X < ClientRectangle.Width);
			
			int y = 0;
			PointF yp = ScreenPoint(new PointF(0, y));
			do
			{
				yp = ScreenPoint(new PointF(0, y));
				//g.DrawLine(Pens.Red, yp.X, yp.Y, yp.X-20, yp.Y);
				g.DrawLine(Pens.Red, 0, yp.Y, 10, yp.Y);
				g.DrawLine(yGridPen, 10, yp.Y, ClientRectangle.Width, yp.Y);
				g.DrawString(y.ToString(), smallFont, Brushes.Red, 10, yp.Y);
				y += step_y;
			} while (yp.Y > 0);

            g.Transform = m;

            if (toDrawArr.Length == 0) return;
            //g.DrawLine(Pens.Black, -points.Count * 0.2f, 0, points.Count * 2, 0);
            //g.DrawLine(Pens.Black, 0, -max * 0.2f, 0, max);
                
            g.DrawLines(myPen, toDrawArr);                       
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            float dx = 0f;
            float dy = 0f;

            float deltaX = 10;
            float deltaY = 500;
            switch (e.KeyCode)
            {
				case Keys.Escape:
					Close();
					break;

                case Keys.Left:
                    dx = -deltaX;
                    break;
                    
                case Keys.Right:
                    dx = deltaX;
                    break;
                    
                case Keys.Up:
                    dy = deltaY;
                    break;
                    
                case Keys.Down:
                    dy = -deltaY;
                    break;
                    
                case Keys.R:
                    m = new Matrix();
                    //invert axis Y
                    m.Scale(1, -1);
                    m.Translate(0, -ClientSize.Height);

                    //resizing for camera size
                    m.Scale(ClientSize.Width / cam.width, ClientSize.Height / cam.height);

                    Form1_MouseWheel(this, new MouseEventArgs(MouseButtons.None, 0, ClientSize.Width / 2, ClientSize.Height / 2, -1));
                    

                    cam.drawW = DataPoint(new PointF(cam.rightP.X - cam.leftP.X, 0f)).X;
                    cam.drawH = ClientSize.Height * m.Elements[4];
                    cam.detectPointsToDraw();                        
                    break;                
            }
            cam.Move(dx, dy);
            m.Translate(-dx, -dy);
            cam.detectByOffset(dx);
            //cam.detectPointsToDraw();
            GetToDrawPoints();
            Invalidate();
        }

        PointF lastmousepos = new PointF(0f, 0f);        
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            Text = DataPoint(e.Location).ToString();//cam.offsetX.ToString() + " " + cam.offsetY.ToString();
            //Text = cam.width.ToString();
            if (e.Button == MouseButtons.Left)
            {
                float dx = DataPoint(lastmousepos).X - DataPoint(e.Location).X;
                float dy = DataPoint(lastmousepos).Y - DataPoint(e.Location).Y;

                //dx.ToString() + " " + dy.ToString();

                cam.Move(dx, dy);                
                m.Translate(-dx, -dy);
                Invalidate();
            }
            lastmousepos = e.Location;
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            /*m = new Matrix();

            //invert axis Y
            m.Scale(1, -1);
            m.Translate(0, -ClientSize.Height);

            //resizing for camera size
            m.Scale(ClientSize.Width / cam.width, ClientSize.Height / cam.height);

            m.Translate(-cam.offsetX, -cam.offsetY);*/
            GetToDrawPoints();
            Invalidate();
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                updateCameraPos();
        }

        private void updateCameraPos()
        {
            PointF newlp = DataPoint(new PointF(0f, 0f));
            PointF newrp = DataPoint(new PointF(ClientSize.Width, ClientSize.Height));

            cam.MoveTo(newlp, newrp);
            cam.detectPointsToDraw();
            GetToDrawPoints();
            Invalidate();
        }

        string matrixToString(Matrix m)
        {
            return
                m.Elements.Select(a => a.ToString()).Aggregate((a, b) => a + " " + b);
        }

        private void Form1_FormClosing_1(object sender, FormClosingEventArgs e)
        {
            Microsoft.Win32.Registry.CurrentUser.SetValue(PLOTTER_TRANSFORM_REGKEY, matrixToString(m));
        }
    }
}
