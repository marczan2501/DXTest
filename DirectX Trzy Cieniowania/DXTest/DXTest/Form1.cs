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
        private Material material;
        private CustomVertex.PositionNormalColored[] flatObject;

        float angle = 0;
        public List<Vector3> normalsToFace;
        public List<Vector3> vertexes3D;

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

            PP.AutoDepthStencilFormat = DepthFormat.D24S8;
            PP.EnableAutoDepthStencil = true;

            PP.DeviceWindowHandle = pictureBox1.Handle;
            device = new Device(0, DeviceType.Hardware, pictureBox1.Handle, CreateFlags.HardwareVertexProcessing, PP);
            InitializeScene();
        }
        private void InitializeScene()
        {
            mesh = Mesh.Teapot(device);

            computeNormalsToFace(mesh);
            prepareFlatObject(mesh);

            material = new Material();
            material.Diffuse = Color.Yellow;
        }
        public void computeNormalsToFace(Mesh mesh)
        {
            float[] vertices = (float[])mesh.LockVertexBuffer(typeof(float), LockFlags.ReadOnly, 6 * mesh.NumberVertices);
            short[] faces = (short[])mesh.LockIndexBuffer(typeof(short), LockFlags.ReadOnly, 3 * mesh.NumberFaces);

            normalsToFace = new List<Vector3>();

            for (int i = 0; i < mesh.NumberFaces * 3; i += 3)
            {
                float x1 = vertices[faces[i] * 6 + 0];
                float y1 = vertices[faces[i] * 6 + 1];
                float z1 = vertices[faces[i] * 6 + 2];

                float x2 = vertices[faces[i + 1] * 6 + 0];
                float y2 = vertices[faces[i + 1] * 6 + 1];
                float z2 = vertices[faces[i + 1] * 6 + 2];

                float x3 = vertices[faces[i + 2] * 6 + 0];
                float y3 = vertices[faces[i + 2] * 6 + 1];
                float z3 = vertices[faces[i + 2] * 6 + 2];

                Vector3 v1 = new Vector3(x1, y1, z1);
                Vector3 v2 = new Vector3(x2, y2, z2);
                Vector3 v3 = new Vector3(x3, y3, z3);

                v1.Subtract(v3);
                v2.Subtract(v3);

                normalsToFace.Add(Vector3.Cross(v1, v2));
            }
            mesh.UnlockIndexBuffer();
            mesh.UnlockVertexBuffer();
        }
        private void prepareFlatObject(Mesh mesh)
        {
            float[] verticles = (float[])mesh.LockVertexBuffer(typeof(float), LockFlags.ReadOnly, 6 * mesh.NumberVertices);
            short[] faces = (short[])mesh.LockIndexBuffer(typeof(short), LockFlags.ReadOnly, 3 * mesh.NumberFaces);

            flatObject = new CustomVertex.PositionNormalColored[mesh.NumberFaces * 3];

            for (int i = 0; i < faces.Length; i++)
            {
                flatObject[i].X = verticles[faces[i] * 6 + 0];
                flatObject[i].Y = verticles[faces[i] * 6 + 1];
                flatObject[i].Z = verticles[faces[i] * 6 + 2];
                flatObject[i].Normal = normalsToFace[i / 3];
                flatObject[i].Color = Color.Yellow.ToArgb();
            }

            mesh.UnlockIndexBuffer();
            mesh.UnlockVertexBuffer();
        }
        private void Render()
        {
            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);

            float zoom = trackBar1.Value;
            float aspect = (float)pictureBox1.Width / pictureBox1.Height;

            device.Transform.Projection = Matrix.PerspectiveFovLH(45f, aspect, 0.1f, 1000f);
            device.Transform.World = Matrix.RotationAxis(new Vector3(0, 1, 0), angle);
            device.Transform.World *= Matrix.Translation(0f, 0f, zoom/2);

            device.Lights[0].Type = LightType.Directional;
            device.Lights[0].Direction = new Vector3(0.0f, 0.0f, 1.0f);
            device.Lights[0].Enabled = true;
            device.RenderState.CullMode = Cull.None;

            device.RenderState.NormalizeNormals = true;

            device.BeginScene();
            device.Material = material;

            if (radioButton1.Checked)
            {
                device.RenderState.ShadeMode = ShadeMode.Flat;
                mesh.DrawSubset(0);
            }

            if (radioButton2.Checked)
            {
                device.VertexFormat = CustomVertex.PositionNormalColored.Format;
                device.DrawUserPrimitives(PrimitiveType.TriangleList, flatObject.Length / 3, flatObject);
            }

            if (radioButton3.Checked)
            {
                device.RenderState.ShadeMode = ShadeMode.Gouraud;
                mesh.DrawSubset(0);
            }
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
            Invalidate();
        }
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Render();
        }
    }
}
