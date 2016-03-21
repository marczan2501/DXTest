using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace DXTest
{
    public partial class Form1 : Form
    {
        private Mesh mesh;
        private Device device;
        private CustomVertex.PositionColored[] coloredsq;
        float angle = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            InitializeDirectX();
        }

        private void InitializeDirectX()
        {
            PresentParameters PP = new PresentParameters();
            PP.Windowed = true;
            PP.SwapEffect = SwapEffect.Discard;

            PP.DeviceWindowHandle = pictureBox1.Handle;

            device = new Device(0, DeviceType.Hardware, pictureBox1.Handle, CreateFlags.HardwareVertexProcessing, PP);
            InitializeScene();
        }

        private void InitializeScene()
        {
            mesh = Mesh.Teapot(device);
        }
        private void Render()
        {
            float zoom = trackBar1.Value;

            Vector3 rotate = new Vector3(0, 0, 0);

            device.Clear(ClearFlags.Target, Color.Black, 1.0f, 0);

            if (radioButton1.Checked)
                device.Transform.Projection = Matrix.PerspectiveFovLH(45.0f, 1.0f, 0.1f, 100f);

            if (radioButton2.Checked)
                device.Transform.Projection = Matrix.OrthoOffCenterLH(-1.0f, 1.0f, -1.0f, 1.0f, 1.0f, 100.0f);

            if (checkBox1.Checked)
                rotate.X = 1f;
            else
                rotate.X = 0f;

            if (checkBox2.Checked)
                rotate.Y = 1f;
            else
                rotate.Y = 0f;

            if (checkBox3.Checked)
                rotate.Z = 1f;
            else
                rotate.Z = 0f;
                       
            device.Transform.World = Matrix.RotationAxis(rotate, angle);
            device.Transform.World *= Matrix.Translation(0f, 0f, zoom);

            device.RenderState.Lighting = false;
            device.RenderState.FillMode = FillMode.WireFrame;
            device.RenderState.CullMode = Cull.None;

            device.BeginScene();
            mesh.DrawSubset(0);
            
            device.EndScene();
            device.Present();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Render();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            device.Dispose();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            
            angle += 0.1f;
            Render();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Render();
        }
    }
}
