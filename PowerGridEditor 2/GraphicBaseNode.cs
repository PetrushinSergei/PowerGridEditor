using System.Drawing;

namespace PowerGridEditor
{
    public class GraphicBaseNode
    {
        public BaseNode Data { get; set; }
        public Point Location { get; set; }
        public static readonly Size NodeSize = new Size(60, 60);
        public bool IsSelected { get; set; }

        public GraphicBaseNode(BaseNode data, Point location)
        {
            Data = data;
            Location = location;
            IsSelected = false;
        }

        public void Draw(Graphics g)
        {
            // Базисный узел рисуем желтым цветом
            Brush fillBrush = IsSelected ? Brushes.LightGreen : Brushes.Gold;
            g.FillEllipse(fillBrush, new Rectangle(Location, NodeSize));
            g.DrawEllipse(Pens.Black, new Rectangle(Location, NodeSize));

            using (Font font = new Font("Arial", 10, FontStyle.Bold))
            using (StringFormat format = new StringFormat())
            {
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;

                using (Brush textBrush = new SolidBrush(Color.Black))
                {
                    g.DrawString(
                        Data.Number.ToString(),
                        font,
                        textBrush,
                        new Rectangle(Location, NodeSize),
                        format
                    );
                }
            }
        }

        public bool Contains(Point point)
        {
            return new Rectangle(Location, NodeSize).Contains(point);
        }
    }
}