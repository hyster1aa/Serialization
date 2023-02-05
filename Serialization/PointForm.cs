using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using PointLib;
using Newtonsoft.Json;


namespace Serialization
{

    public partial class PointForm : Form
    {

        public PointForm()
        {
            InitializeComponent();
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
        private Point[] points = null;
        private void btnCreate_Click(object sender, EventArgs e)
        {
            points = new Point[5];

            var rnd = new Random();

            for (int i = 0; i < points.Length; i++)
                points[i] = rnd.Next(3) % 2 == 0 ? new Point() : new Point3D();

            listBox1.DataSource = points;
        }

        private void btnSort_Click(object sender, EventArgs e)
        {
            if (points == null)
                return;

            Array.Sort(points);

            listBox1.DataSource = null;
            listBox1.DataSource = points;

        }

        private void btnSerialize_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "SOAP|*.soap|XML|*.xml|JSON|*.json|Binary|*.bin";
            if (saveFileDialog1.ShowDialog() != DialogResult.OK)
                return;
            using (var fs = new FileStream(saveFileDialog1.FileName, FileMode.Create, FileAccess.Write))
            {
                switch (Path.GetExtension(saveFileDialog1.FileName))
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
                        var xf = new XmlSerializer(typeof(Point[]),new[] {typeof(Point3D)});
                        xf.Serialize(fs, points);
                        break;
                    case ".json":
                        var jf = Newtonsoft.Json.JsonConvert.SerializeObject(points);
                        var temp = Encoding.Default.GetBytes(jf);
                        fs.Write(temp,0,temp.Count());
                        break;
                }
            }
        }

        private void btnDeserialize_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;
            using (var fs = new FileStream(openFileDialog1.FileName, FileMode.Open, FileAccess.Read))
            {
                switch(Path.GetExtension(openFileDialog1.FileName))
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
                        var xf = new XmlSerializer(typeof(Point[]), new[] {typeof(Point3D) });
                        points = (Point[])xf.Deserialize(fs);
                        break;
                    case ".json":
                        var jf = new JsonSerializer();
                        using (var r = new StreamReader(fs))
                            points = (Point[])jf.Deserialize(r, typeof(Point[]));
                        break;

                }
            }
            listBox1.DataSource = null;
            listBox1.DataSource = points;

        }
    }
    

}
