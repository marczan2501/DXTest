using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace DXTest
{
    public partial class Form1 : Form
    {
        private Device device;
        private Mesh mesh;
        private Texture texture;
        public List<Vector3> normalsToFace;
        public List<Vector3> vertexes3D;
        float angle = 0;

        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load_1(object sender, EventArgs e)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            InitializeDirectX();
            InitializeScene();
        }
        private void InitializeDirectX()
        {
            PresentParameters PP = new PresentParameters();
            PP.Windowed = true;
            PP.SwapEffect = SwapEffect.Discard;

            PP.AutoDepthStencilFormat = DepthFormat.D24S8;
            PP.EnableAutoDepthStencil = true;

            PP.DeviceWindowHandle = pictureBox1.Handle;
            device = new Device(0, DeviceType.Hardware, pictureBox1.Handle, CreateFlags.HardwareVertexProcessing, PP);
        }
        private void InitializeScene()
        {
            texture = TextureLoader.FromFile(device, "earth.jpg");
            mesh = Mesh.Sphere(device, 0.5f, 30, 30);
            mesh = SetCoordinatesTexture(mesh);
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
            Vector3 rot = new Vector3(0, 1, 0);

            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);

            float zoom = trackBar1.Value;
            float aspect = (float)pictureBox1.Width / pictureBox1.Height;
            
            device.Transform.Projection = Matrix.PerspectiveFovLH(45f, aspect, 1.0f, 100f);
            device.Transform.World = Matrix.RotationAxis(new Vector3(1, 0, 0), 30);
            device.Transform.World *= Matrix.RotationAxis(rot, angle);
            device.Transform.World *= Matrix.Translation(0f, 0f, zoom/2);
            SetLight();

            device.RenderState.Wrap0 = WrapCoordinates.Zero;
            device.RenderState.Lighting = false;
            device.RenderState.CullMode = Cull.None;
            device.RenderState.FillMode = FillMode.Solid;
            
            
            device.BeginScene();
            device.SetTexture(0, texture);
            mesh.DrawSubset(0);
            device.EndScene();
            device.Present();
        }
        private static Mesh SetCoordinatesTexture(Mesh mesh)
        {
            Vector3 vertexRay, meshCenter;

            mesh = mesh.Clone(mesh.Options.Value, CustomVertex.PositionNormalTextured.Format, 
                mesh.Device);
            mesh.ComputeNormals();

            Geometry.ComputeBoundingSphere((GraphicsStream)mesh.VertexBuffer.Lock(0, 0, LockFlags.None), 
                mesh.NumberVertices, mesh.VertexFormat, out meshCenter);
            mesh.VertexBuffer.Unlock();

            using (VertexBuffer vb = mesh.VertexBuffer)
            {
                CustomVertex.PositionNormalTextured[] verts =
                    (CustomVertex.PositionNormalTextured[])vb.Lock(0,
                    typeof(CustomVertex.PositionNormalTextured), LockFlags.None, mesh.NumberVertices);
                
                for (int i = 0; i < verts.Length; i++)
                {
                    vertexRay = Vector3.Normalize(verts[i].Position - meshCenter);
                    double phi = Math.Acos((double)vertexRay.Z);
                    verts[i].Tv = (float)(phi / Math.PI);
                    double psi = (double)vertexRay.Y / Math.Sin(phi);
                    float u = (float)(Math.Acos(psi) / (2.0 * Math.PI));
                    verts[i].Tu = (vertexRay.X > 0f) ? u : 1 - u;
                }
                vb.Unlock();
            }
            return mesh;
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
            Invalidate();
        }
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Render();
        }
    }
}
