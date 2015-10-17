﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Management;
using System.Drawing.Imaging;
using System.Collections;
using Microsoft.DirectX.DirectInput;

namespace QuadcopterAvoidanceSimulation
{
    public partial class Form1 : Form
    {
        ObstacleLayout walls;
        Timer time;
        Quad mainQuad;
        private Joystick joystick;
        private bool[] joystickButtons;
        private Thread joystickUpdateThread;
        

        public Form1()
        {
            InitializeComponent();

            joystick = new Joystick(this.Handle);
            connectToJoystick(joystick);
            joystickUpdateThread = new Thread(new ThreadStart(joystickUpdate));
            joystickUpdateThread.Start();

            time = new Timer(DateTime.Now.Ticks);
            walls = new ObstacleLayout();   
            mainQuad = new Quad(100, 100,time);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void mainViewPort_Paint(object sender, PaintEventArgs e)
        {
            Pen blackPen = new Pen(Color.Black, 1);
            SolidBrush blackBrush = new SolidBrush(Color.Black);
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            for (int i = 0; i < walls.Obstacles.Count; i++)
            {
                Obstacle o = (Obstacle)walls.Obstacles[i];
            }
            int x = Convert.ToInt32(mainQuad.xPosition);
            int y = Convert.ToInt32(mainViewPort.Height - mainQuad.yPosition);
            int x2 = Convert.ToInt32(x - 20 * mainQuad.quadHeadingVector.X);
            int y2 = Convert.ToInt32(y - 20 * mainQuad.quadHeadingVector.Y);
           
            g.FillEllipse(blackBrush,x,y,4,4);
            g.DrawLine(blackPen,x+2,y+2,x2,y2);
            g.Dispose();
        }

        private void viewPortUpdate_Tick(object sender, EventArgs e)
        {
            joystickUpdate();
            mainQuad.updateState();
            mainViewPort.Invalidate();
        }

        private void enableTimer()
        {
            if (this.InvokeRequired)
            {
                BeginInvoke(new ThreadStart(delegate()
                {
                    //joystickTimer.Enabled = true;
                }));
            }
            //else
                //joystickTimer.Enabled = true;
        }

        private void connectToJoystick(Joystick joystick)
        {
            while (true)
            {
                string sticks = joystick.FindJoysticks();
                if (sticks != null)
                {
                    if (joystick.AcquireJoystick(sticks))
                    {
                        enableTimer();
                        break;
                    }
                }
            }
        }

        private void joystickUpdate()
        {
            try
            {
                joystick.UpdateStatus();
                joystickButtons = joystick.buttons;

                mainQuad.roll = Equations.toRad(joystick.Xaxis / 2.0);
                mainQuad.pitch = Equations.toRad(-1*joystick.Yaxis / 2.0);
                mainQuad.yawRate = Equations.toRad(-1*joystick.Zaxis);
               //Console.WriteLine(Equations.toDeg(mainQuad.yaw));
                if(joystickButtons[0] == true)
                    mainQuad.resetQuad();
                for (int i = 0; i < joystickButtons.Length; i++)
                {
                    if (joystickButtons[i] == true)
                        Console.Write("Button " + i + " Pressed\n");
                }
            }
            catch
            {
                //joystickTimer.Enabled = false;
                connectToJoystick(joystick);
            }
        }

    }

    
}
