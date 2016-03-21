using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace WindowsFormsApplication3
{
    public partial class Form1 : Form
    {
        private Device device;
        private Texture texture;
        private CustomVertex.TransformedTextured[] vertexes;
        public Form1()
        {

            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
            InitializeDirectX();
        }
        private void InitializeDirectX()
        {
            PresentParameters PP = new PresentParameters();
            PP.Windowed = true;
            PP.SwapEffect = SwapEffect.Discard;
            if (!PP.Windowed)
            {
                PP.FullScreenRefreshRateInHz = 60;
                PP.BackBufferWidth = 800;
                PP.BackBufferHeight = 600;
                PP.BackBufferCount = 1;
                PP.BackBufferFormat = Format.X8R8G8B8;
                PP.PresentFlag = PresentFlag.None;
            }
            device = new Device(0, DeviceType.Hardware, this, CreateFlags.HardwareVertexProcessing, PP);
            InitializeScene();
        }
        private void InitializeScene()
        {
            vertexes = new CustomVertex.TransformedTextured[6];
      
            vertexes[0].Position = new Vector4(100, 100, 0, 1);
            vertexes[0].Tu = 0;
            vertexes[0].Tv = 0;
            vertexes[1].Position = new Vector4(400, 100, 0, 1);
            vertexes[1].Tu = 1;
            vertexes[1].Tv = 0;

            vertexes[2].Position = new Vector4(100, 400, 0, 1);
            vertexes[2].Tu = 0;
            vertexes[2].Tv = 1;

            vertexes[3].Position = new Vector4(400, 400, 0, 1);
            vertexes[3].Tu = 1;
            vertexes[3].Tv = 1;

            vertexes[4].Position = new Vector4(100, 400, 0, 1);
            vertexes[4].Tu = 0;
            vertexes[4].Tv = 1;

            vertexes[5].Position = new Vector4(400, 100, 0, 1);
            vertexes[5].Tu = 1;
            vertexes[5].Tv = 0;

            texture = TextureLoader.FromFile(device, "bricks.bmp");

        }
        public void Render()
        {

            
            device.Clear(ClearFlags.Target, Color.Blue, 1.0f, 0);
            device.BeginScene();
            device.SetTexture(0, texture);
            device.VertexFormat = CustomVertex.TransformedTextured.Format;
            device.DrawUserPrimitives(PrimitiveType.TriangleStrip, 2, vertexes);

            device.EndScene();
            device.Present();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Render();
        }
    }
}
