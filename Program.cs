using System.Drawing;
using System.Windows.Forms;

Connect3State state = new Connect3State();
Piece piece = Piece.White;

int frame = 0;
Bitmap bmp = null;
Graphics g = null;
bool isDown = false;
bool inDown = false;
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
            int x = desloc + size * (i + 1);
            RectangleF rect = new RectangleF(x, size * (j + 1), 3 * size / 4, 3 * size / 4);
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

    for (byte i = 0; i < 4; i++)
    {
        int x = desloc + size * (i + 1);
        if (cursor.X < x || cursor.X > x + size)
            continue;
        
        var j = 3 - state.GetLevel(i);
        if (j > 3)
            return;
        
        RectangleF rect = new RectangleF(x, size * (j + 1), 3 * size / 4, 3 * size / 4);
        switch (piece)
        {
            case Piece.Black:
                var brush = new SolidBrush(Color.FromArgb(128, 255, 0, 0));
                g.FillEllipse(brush, rect);
                break;
            
            case Piece.White:
                var brush2 = new SolidBrush(Color.FromArgb(128, 0, 0, 255));
                g.FillEllipse(brush2, rect);
                break;

        }

        if (inDown && !isDown)
        {
            state = new Connect3State(state, i, piece);
            switch (state.CheckWin())
            {
                case Piece.White:
                    g.Clear(Color.Blue);
                    tm.Stop();
                    break;
                
                case Piece.Black:
                    g.Clear(Color.Red);
                    tm.Stop();
                    break;
            }
            piece = piece == Piece.White ? Piece.Black : Piece.White;
            inDown = false;
            return;
        }

        if (!inDown && isDown)
            inDown = true;

    }
}
