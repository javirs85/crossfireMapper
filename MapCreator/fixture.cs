using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace MapCreator
{
    public enum FixtureType {notSet, wood, field, orchard, house, bunker, fence, crest, rough, river, hill};

    public class Fixture
    {
        public FixtureType type { get; set; }
        public string Name { get; set; }
        public Point Position { get; set; }
        public bool IsTable = false;
        public double Rotation { get; set; }
        private BitmapImage bitmap;
        [XmlIgnore]
        public Image img;


        public void LoadImage(string path)
        {
            var uri = new Uri(System.IO.Path.GetFullPath(path));
            bitmap = new BitmapImage(uri);
            img = new Image
            {
                Source = bitmap,
                RenderTransformOrigin = new Point(0.5, 0.5)
            };
        }

        public static Fixture FromPNG(string file)
        {
            Fixture fix = new Fixture();
            var uri = new Uri(System.IO.Path.GetFullPath(file));
            fix.bitmap = new BitmapImage(uri);
            fix.img = new Image();
            var name = System.IO.Path.GetFileNameWithoutExtension(file);
            fix.Name = name;
            fix.img.Source = fix.bitmap;
            fix.img.RenderTransformOrigin = new Point(0.5, 0.5);

            return fix;
        }

        public void Rotate(double numberOfDegrees)
        {
            Rotation += numberOfDegrees;
            RotateTransform rotateTransform = new RotateTransform(Rotation);
            img.RenderTransform = rotateTransform;
        }
    }

    public class FixtureCollection
    {
        public List<Fixture> Fixtures = new List<Fixture>();
        [XmlIgnore]
        public BitmapImage compressedBMimage;

        public void Add(Fixture fix)
        {
            Fixtures.Add(fix);
        }

        public Fixture FindByName(string name)
        {
            return Fixtures.Find(x => x.Name == name);
        }

        public Fixture FindByImage(Image imag)
        {
            return Fixtures.Find(x => x.img == imag);
        }

        public void SaveToFile(string path = "fixtures.xml")
        {
            var writer = new XmlSerializer(typeof(FixtureCollection));
            var file = System.IO.File.Create(path);
            writer.Serialize(file, this);
            file.Close();
        }

        public static FixtureCollection FromFile(string path = "fixtures.xml")
        {
            try
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(FixtureCollection));
                var file = System.IO.File.OpenRead(path);
                return (FixtureCollection)deserializer.Deserialize(file);
            }catch(Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
        }
    }   
}
