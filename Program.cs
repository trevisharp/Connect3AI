using System.Drawing;
using System.Windows.Forms;

Connect3State state = new Connect3State();
state = new Connect3State(state, 0, Piece.White);

state = new Connect3State(state, 1, Piece.Black);
state = new Connect3State(state, 0, Piece.Black);

state = new Connect3State(state, 0, Piece.White);
state = new Connect3State(state, 1, Piece.White);
state = new Connect3State(state, 2, Piece.White);

state = new Connect3State(state, 0, Piece.Black);
state = new Connect3State(state, 1, Piece.Black);
state = new Connect3State(state, 2, Piece.Black);
state = new Connect3State(state, 3, Piece.Black);

int frame = 0;
Bitmap bmp = null;
Graphics g = null;
bool isDown = false;
PointF cursor = PointF.Empty;

ApplicationConfiguration.Initialize();

var form = new Form();
form.WindowState = FormWindowState.Maximized;
form.FormBorderStyle = FormBorderStyle.None;

PictureBox pb = new PictureBox();
pb.Dock = DockStyle.Fill;
form.Controls.Add(pb);

Timer tm = new Timer();
tm.Interval = 20;

pb.MouseDown += (o, e) =>
{
    isDown = true;
};

pb.MouseUp += (o, e) =>
{
    isDown = false;
};

pb.MouseMove += (o, e) =>
{
    cursor = e.Location;
};

form.Load += delegate
{
    bmp = new Bitmap(pb.Width, pb.Height);
    pb.Image = bmp;

    g = Graphics.FromImage(bmp);
    g.Clear(Color.White);
    pb.Refresh();

    tm.Start();
};

form.KeyDown += (o, e) =>
{
    switch (e.KeyCode)
    {
        case Keys.Escape:
            Application.Exit();
            break;
    }
};

tm.Tick += delegate
{
    g.Clear(Color.White);

    frame++;
    draw();

    pb.Refresh();
};

Application.Run(form);

void draw()
{
    int size = pb.Height / 6;
    int desloc = (pb.Width - pb.Height) / 2;
    for (int j = 0; j < 4; j++)
    {
        for (int i = 0; i < 4; i++)
        {
            RectangleF rect = new RectangleF(desloc + size * (i + 1), size * (j + 1), 3 * size / 4, 3 * size / 4);
            var piece = state[i, 3 - j];
            switch (piece)
            {
                case Piece.Black:
                    g.FillEllipse(Brushes.Red, rect);
                    break;
                
                case Piece.White:
                    g.FillEllipse(Brushes.Blue, rect);
                    break;

            }
            g.DrawEllipse(Pens.Black, rect);
        }
    }
}