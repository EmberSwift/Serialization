﻿using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Windows.Forms;
using System.Xml.Serialization;
using ClassLibrary1;
using Newtonsoft.Json;

namespace Сериализация2
{
   
    public partial class PointForm : Form
    {
        private Point[] points = null;
        public PointForm()
        {
            InitializeComponent();
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            points = new Point[5];

            var rnd = new Random();

            for (int i = 0; i < points.Length; i++)
                points[i] = rnd.Next(3) % 2 == 0 ? new Point() : new Point3D();

            listBox.DataSource = points;
        }

        private void btnSort_Click(object sender, EventArgs e)
        {
            if (points == null)
                return;
            Array.Sort(points);
            listBox.DataSource = null;
            listBox.DataSource = points;
        }

        private void btnSerialize_Click(object sender, EventArgs e)
        {
            var dlg = new SaveFileDialog();
            dlg.Filter = "SOAP|*.soap|XML|*.xml|JSON|*.json|Binary|*.bin";
            if (dlg.ShowDialog() != DialogResult.OK)
                return;
            using (var fs = new FileStream(dlg.FileName, FileMode.Create, FileAccess.Write))
            {
                switch (Path.GetExtension(dlg.FileName))
                {
                    case ".bin":
                        var bf = new BinaryFormatter();
                        bf.Serialize(fs, points);
                        break;
                    case ".soap":
                        var sf = new SoapFormatter();
                        sf.Serialize(fs, points);
                        break;
                    case ".xml":
                        var xf = new XmlSerializer(typeof(Point[]), new[]
                        {typeof(Point3D)});
                        xf.Serialize(fs, points);
                        break;
                    case ".json":
                        JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
                        string Serialized = JsonConvert.SerializeObject(points, settings);
                        using (var j = new StreamWriter(fs))
                            j.Write(Serialized);
                        break;
                }
            }
        }

        private void btnDeserialize_Click(object sender, EventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.Filter = "SOAP|*.soap|XML|*.xml|JSON|*.json|Binary|*.bin";
            if (dlg.ShowDialog() != DialogResult.OK)
                return;
            using (var fs =
            new FileStream(dlg.FileName, FileMode.Open, FileAccess.Read))
            {
                switch (Path.GetExtension(dlg.FileName))
                {
                    case ".bin":
                        var bf = new BinaryFormatter();
                        points = (Point[])bf.Deserialize(fs);
                        break;
                    case ".soap":
                        var sf = new SoapFormatter();
                        points = (Point[])sf.Deserialize(fs);
                        break;
                    case ".xml":
                        var xf = new XmlSerializer(typeof(Point[]), new[]
                        {typeof(Point3D)});
                        points = (Point[])xf.Deserialize(fs);
                        break;
                    case ".json":
                        using (var r = new StreamReader(fs))
                        {
                            string json = r.ReadToEnd();
                            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
                            points = JsonConvert.DeserializeObject<Point[]>(json, settings);
                        }
                        break;
                }
            }
            listBox.DataSource = null;
            listBox.DataSource = points;
        }
    }
    [Serializable]
    public class Point3D : Point
    {
        public int Z { get; set; }
        public Point3D() : base()
        {
            Z = rnd.Next(10);
        }
        public Point3D(int x, int y, int z) : base(x, y)
        {
            Z = z;
        }
        public override double Metric()
        {
            return Math.Sqrt(X * X + Y * Y + Z * Z);
        }
        public override string ToString()
        {
            return string.Format($"({X} , {Y}, {Z})");
        }
    }
}
