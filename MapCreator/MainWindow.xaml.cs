using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace MapCreator
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private Image draggedImage;
        private Image RotatedImage;
        private Point mousePosition;


        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var files = System.IO.Directory.GetFiles("BaseImages");

            foreach (var file in files)
            {
                var uri = new Uri(System.IO.Path.GetFullPath(file));
                var bitmap = new BitmapImage(uri);
                Image img = new Image();
                img.Source = bitmap;
                img.RenderTransformOrigin = new Point(0.5, 0.5);
                if (file == @"BaseImages\120mm.png")
                    Panel.SetZIndex(img, 0);
                else
                    Panel.SetZIndex(img, 1);
                Canvas.SetLeft(img, 0);
                Canvas.SetTop(img, 0);
                MainCanvas.Children.Add(img);
            }
        }

        private void MainCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var image = e.Source as Image;

            if (image != null && MainCanvas.CaptureMouse())
            {
                mousePosition = e.GetPosition(MainCanvas);
                draggedImage = image;
                Panel.SetZIndex(draggedImage, 2); // in case of multiple images
            }
        }
        
        private void MainCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (draggedImage != null)
            {
                MainCanvas.ReleaseMouseCapture();
                Panel.SetZIndex(draggedImage, 1);
                draggedImage = null;
            }
        }

        private void MainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (draggedImage != null)
            {
                var position = e.GetPosition(MainCanvas);
                var offset = position - mousePosition;
                mousePosition = position;
                Canvas.SetLeft(draggedImage, Canvas.GetLeft(draggedImage) + offset.X);
                Canvas.SetTop(draggedImage, Canvas.GetTop(draggedImage) + offset.Y);
            }
            else if (RotatedImage != null)
            {
                var position = e.GetPosition(MainCanvas);
                var angle = AngleBetween(new Vector(1, 0), new Vector(position.X - mousePosition.X, position.Y - mousePosition.Y));

                RotateTransform rotateTransform = new RotateTransform(angle);
                RotatedImage.RenderTransform = rotateTransform;

                //var position = e.GetPosition(MainCanvas);
                //var offset = position - mousePosition;
                //mousePosition = position;
                //Canvas.SetLeft(draggedImage, Canvas.GetLeft(draggedImage) + offset.X);
                //Canvas.SetTop(draggedImage, Canvas.GetTop(draggedImage) + offset.Y);
            }
        }

        public static double AngleBetween(Vector vector1, Vector vector2)
        {
            double sin = vector1.X * vector2.Y - vector2.X * vector1.Y;
            double cos = vector1.X * vector2.X + vector1.Y * vector2.Y;

            return Math.Atan2(sin, cos) * (180 / Math.PI);
        }

        private void MainCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var image = e.Source as Image;
            if (image == null)
                if (draggedImage != null)
                    image = draggedImage;

            if (MainCanvas.CaptureMouse())
            {
                mousePosition.X = Canvas.GetLeft(image);
                mousePosition.Y = Canvas.GetTop(image);
                RotatedImage = image;
                Panel.SetZIndex(RotatedImage, 2); // in case of multiple images
            }
        }

        private void MainCanvas_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (RotatedImage != null)
            {
                MainCanvas.ReleaseMouseCapture();
                Panel.SetZIndex(RotatedImage, 1);
                RotatedImage = null;
            }
        }
        
    }
}
