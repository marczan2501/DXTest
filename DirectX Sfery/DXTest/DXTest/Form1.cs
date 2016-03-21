using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace DXTest
{
    public partial class Form1 : Form
    {
        private Mesh mesh1;
        private Mesh mesh2, mesh3;
        private Device device;
        private Material material;
        float angle1 = 0;
        float angle2 = 0;
        float angle3 = 0;

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

            PP.AutoDepthStencilFormat = DepthFormat.D24X8;
            PP.EnableAutoDepthStencil = true;

            PP.Windowed = true;
            PP.SwapEffect = SwapEffect.Discard;

            PP.DeviceWindowHandle = pictureBox1.Handle;

            device = new Device(0, DeviceType.Hardware, pictureBox1.Handle, CreateFlags.HardwareVertexProcessing, PP);
            InitializeScene();
        }

        private void InitializeScene()
        {

            mesh1 = Mesh.Sphere(device, 10, 20, 20);
            mesh1.ComputeNormals();
            mesh2 = Mesh.Sphere(device, 10, 20, 20);
            mesh2.ComputeNormals();
            mesh3 = Mesh.Sphere(device, 10, 20, 20);
            mesh3.ComputeNormals();

            material = new Material();
            material.Diffuse = Color.Yellow;
        }

        private void SetRenderState()
        {
            device.RenderState.NormalizeNormals = true;
            device.RenderState.CullMode = Cull.None;
            device.RenderState.ShadeMode = ShadeMode.Gouraud;
            device.RenderState.Lighting = true;
        }

        private void SetLight()
        {
            device.Lights[0].Type = LightType.Directional;
            device.Lights[0].Position = new Vector3(0.0f, 0.0f, 40.0f);
            device.Lights[0].Direction = new Vector3(0.0f, 0.0f, 1.0f);
            device.Lights[0].Enabled = true;

        }
        private void Render()
        {
            SetRenderState();
            SetLight();
            float zoom = trackBar1.Value;

            label1.Text = zoom.ToString();

            Vector3 rotate = new Vector3(0, 0, 0);

            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);

            device.Transform.Projection = Matrix.PerspectiveFovLH(45.0f, 1.0f, 0.1f, 1000f);
            
            device.Material = material;
            device.BeginScene();

            device.Transform.World = Matrix.RotationAxis(new Vector3(1,1,1), angle1);
            device.Transform.World *= Matrix.Translation(0f, 0f, zoom);
            mesh1.DrawSubset(0);

            device.Transform.World = Matrix.Translation(0f, 0f, zoom);
            device.Transform.World *= Matrix.Translation(0f, 0f, -zoom-100f);
            device.Transform.World *= Matrix.RotationAxis(new Vector3(0, 1, 0), angle2);
            device.Transform.World *= Matrix.Translation(0f, 0f, zoom+100f);
            mesh2.DrawSubset(0);

            device.Transform.World = Matrix.Translation(0f, 0f, zoom);
            device.Transform.World *= Matrix.Translation(0f, 0f, -zoom-100);
            device.Transform.World *= Matrix.RotationAxis(new Vector3(1, 0, 0), angle3);
            device.Transform.World *= Matrix.Translation(0f, 0f, zoom+100f);
            mesh3.DrawSubset(0);

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
            
            angle1 += 0.01f;
            angle2 += 0.02f;
            angle3 += 0.03f;
            Render();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Render();
        }
    }
}
