using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System;
using System.IO;

namespace PuzzleGame
{
    public partial class Form1 : Form
    {
        private int puzzleSize;
        private Image originalImage;
        private PictureBox[] puzzlePieces;
        private Point[] originalLocations;
        private int piecesPerRow;
        private int pieceWidth;
        private int pieceHeight;
        private const int snapThreshold = 15;
        private DateTime startTime;
        private Timer timer;
        private int remainingHints = 3;

        public Form1()
        {
            InitializeComponent();
            InitializeGame();
            DoubleBuffered = true;
        }

        private void InitializeGame()
        {
            rbEasy.CheckedChanged += DifficultyChanged;
            rbMedium.CheckedChanged += DifficultyChanged;
            rbHard.CheckedChanged += DifficultyChanged;
            btnUpload.Click += BtnUpload_Click;
            pnlPuzzleBoard.Paint += PnlPuzzleBoard_Paint;
            btnRestart.Click += BtnRestart_Click;

            timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += Timer_Tick;

            lblTimer = new Label();
            lblTimer.AutoSize = true;
            lblTimer.Font = new Font("Calibri", 10);
            lblTimer.Location = new Point(615, 150);

            lblTimer.Text = "Time: 00:00";
            this.Controls.Add(lblTimer);

            btnHint.Click += BtnHint_Click;
            btnStart.Click += BtnStart_Click;

            rbEasy.Checked = true;
            DifficultyChanged(null, EventArgs.Empty);
            LoadDefaultImage();
        }

        private void LoadDefaultImage()
        {
            string defaultImagePath = Path.Combine(Application.StartupPath, "Resources", "Dog.jpg");
            if (File.Exists(defaultImagePath))
            {
                originalImage = Image.FromFile(defaultImagePath);
                pbUploadedImage.Image = originalImage;
                GeneratePuzzle();
                btnStart.Enabled = true;
            }
            else
            {
                MessageBox.Show("Default image not found.");
            }
        }

        private void DifficultyChanged(object sender, EventArgs e)
        {
            if (rbEasy.Checked) puzzleSize = 9;
            if (rbMedium.Checked) puzzleSize = 25;
            if (rbHard.Checked) puzzleSize = 49;

            piecesPerRow = (int)Math.Sqrt(puzzleSize);
            if (originalImage != null)
            {
                GeneratePuzzle();
            }
        }

        private void BtnUpload_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    originalImage = Image.FromFile(ofd.FileName);
                    pbUploadedImage.Image = originalImage;
                    GeneratePuzzle();
                    btnStart.Enabled = true;
                }
            }
        }

        private void BtnRestart_Click(object sender, EventArgs e)
        {
            timer.Stop();

            pnlPuzzleBoard.Controls.Clear();
            pnlPieceHolder.Controls.Clear();

            GeneratePuzzle();

            remainingHints = 3;
            btnHint.Text = $"Hint: {remainingHints}";

            btnStart.Enabled = true;
            lblTimer.Text = "Time: 00:00";
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            startTime = DateTime.Now;
            timer.Start();
            btnStart.Enabled = false;
        }

        private void GeneratePuzzle()
        {
            pnlPuzzleBoard.Controls.Clear();
            pnlPieceHolder.Controls.Clear();
            piecesPerRow = (int)Math.Sqrt(puzzleSize);
            pieceWidth = pnlPuzzleBoard.Width / piecesPerRow;
            pieceHeight = pnlPuzzleBoard.Height / piecesPerRow;
            puzzlePieces = new PictureBox[puzzleSize];
            originalLocations = new Point[puzzleSize];

            Bitmap bm = new Bitmap(originalImage, pnlPuzzleBoard.Size);
            for (int i = 0; i < puzzleSize; i++)
            {
                int row = i / piecesPerRow;
                int col = i % piecesPerRow;

                Bitmap pieceImage = bm.Clone(new Rectangle(col * pieceWidth, row * pieceHeight, pieceWidth, pieceHeight), bm.PixelFormat);
                PictureBox pb = new PictureBox
                {
                    Width = pieceWidth,
                    Height = pieceHeight,
                    Image = pieceImage,
                    BorderStyle = BorderStyle.FixedSingle,
                    Location = new Point(col * pieceWidth, row * pieceHeight),
                    Tag = i
                };

                pb.MouseDown += PuzzlePiece_MouseDown;
                pb.MouseMove += PuzzlePiece_MouseMove;
                pb.MouseUp += PuzzlePiece_MouseUp;

                puzzlePieces[i] = pb;
                originalLocations[i] = pb.Location;
                pnlPieceHolder.Controls.Add(pb);
            }

            ShufflePuzzle();
            pnlPuzzleBoard.Invalidate();
        }

        private void ShufflePuzzle()
        {
            Random rand = new Random();
            foreach (var pb in puzzlePieces)
            {
                int newX = rand.Next(pnlPieceHolder.ClientRectangle.Left, pnlPieceHolder.ClientRectangle.Right - pb.Width);
                int newY = rand.Next(pnlPieceHolder.ClientRectangle.Top, pnlPieceHolder.ClientRectangle.Bottom - pb.Height);
                pb.Location = new Point(newX, newY);
            }
        }

        private PictureBox selectedPiece = null;
        private Point previousLocation;

        private void PuzzlePiece_MouseDown(object sender, MouseEventArgs e)
        {
            selectedPiece = sender as PictureBox;
            previousLocation = e.Location;

            selectedPiece.Parent = this;
            selectedPiece.BringToFront();
        }

        private void PuzzlePiece_MouseMove(object sender, MouseEventArgs e)
        {
            if (selectedPiece != null)
            {
                int newX = selectedPiece.Left + (e.X - previousLocation.X);
                int newY = selectedPiece.Top + (e.Y - previousLocation.Y);

                newX = Math.Max(0, Math.Min(newX, this.ClientSize.Width - selectedPiece.Width));
                newY = Math.Max(0, Math.Min(newY, this.ClientSize.Height - selectedPiece.Height));

                selectedPiece.Left = newX;
                selectedPiece.Top = newY;
            }
        }

        private void PuzzlePiece_MouseUp(object sender, MouseEventArgs e)
        {
            if (selectedPiece != null)
            {
                int index = (int)selectedPiece.Tag;
                Point correctLocation = originalLocations[index];

                int snapX = pnlPuzzleBoard.Left + (correctLocation.X / pieceWidth) * pieceWidth;
                int snapY = pnlPuzzleBoard.Top + (correctLocation.Y / pieceHeight) * pieceHeight;

                bool isInPuzzleBoard = selectedPiece.Left >= pnlPuzzleBoard.Left &&
                                       selectedPiece.Right <= pnlPuzzleBoard.Right &&
                                       selectedPiece.Top >= pnlPuzzleBoard.Top &&
                                       selectedPiece.Bottom <= pnlPuzzleBoard.Bottom;

                if (isInPuzzleBoard && Math.Abs(selectedPiece.Left - snapX) < snapThreshold && Math.Abs(selectedPiece.Top - snapY) < snapThreshold)
                {
                    selectedPiece.Left = snapX - pnlPuzzleBoard.Left;
                    selectedPiece.Top = snapY - pnlPuzzleBoard.Top;
                    selectedPiece.Parent = pnlPuzzleBoard;

                    selectedPiece.Location = new Point(snapX - pnlPuzzleBoard.Left, snapY - pnlPuzzleBoard.Top);

                    selectedPiece.MouseDown -= PuzzlePiece_MouseDown;
                    selectedPiece.MouseMove -= PuzzlePiece_MouseMove;
                    selectedPiece.MouseUp -= PuzzlePiece_MouseUp;

                    if (IsPuzzleSolved())
                    {
                        timer.Stop();

                        ShowCompletionMessage(DateTime.Now - startTime);
                    }
                }
                else
                {
                    selectedPiece.Parent = this;
                }

                selectedPiece = null;
            }
        }

        private bool IsPuzzleSolved()
        {
            foreach (var pb in puzzlePieces)
            {
                int index = (int)pb.Tag;
                Point correctLocation = originalLocations[index];
                int snapX = pnlPuzzleBoard.Left + (correctLocation.X / pieceWidth) * pieceWidth;
                int snapY = pnlPuzzleBoard.Top + (correctLocation.Y / pieceHeight) * pieceHeight;

                if (pb.Left != snapX - pnlPuzzleBoard.Left || pb.Top != snapY - pnlPuzzleBoard.Top)
                {
                    return false;
                }
            }

            return true;
        }

        private void PnlPuzzleBoard_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen gridPen = new Pen(Color.Black, 1);
            Pen borderPen = new Pen(Color.Black, 1);

            for (int i = 1; i < piecesPerRow; i++)
            {
                g.DrawLine(gridPen, new Point(i * pieceWidth, 0), new Point(i * pieceWidth, pnlPuzzleBoard.Height));
                g.DrawLine(gridPen, new Point(0, i * pieceHeight), new Point(pnlPuzzleBoard.Width, i * pieceHeight));
            }

            g.DrawRectangle(borderPen, 0, 0, pnlPuzzleBoard.Width - 1, pnlPuzzleBoard.Height - 1);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            TimeSpan elapsedTime = DateTime.Now - startTime;
            string formattedTime = $"{elapsedTime:mm\\:ss}";
            lblTimer.Text = $"Time: {formattedTime}";

            if (IsPuzzleSolved())
            {
                timer.Stop();

                ShowCompletionMessage(elapsedTime);
            }
        }

        private void ShowCompletionMessage(TimeSpan elapsedTime)
        {
            int minutes = (int)elapsedTime.TotalMinutes;
            int seconds = elapsedTime.Seconds;

            string message;
            if (minutes > 0)
                message = $"Congratulations! You solved the puzzle in {minutes} minutes and {seconds} seconds.\nPlay Again?";
            else
                message = $"Congratulations! You solved the puzzle in {seconds} seconds.\nPlay Again?";



            var result = MessageBox.Show(message, "Puzzle Solved", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                BtnRestart_Click(null, EventArgs.Empty);
            }
            else if (result == DialogResult.No)
            {
                this.Close();
            }
        }


        private void BtnHint_Click(object sender, EventArgs e)
        {
            if (remainingHints > 0)
            {
                var incorrectPieces = puzzlePieces.Where(pb =>
                {
                    int index = (int)pb.Tag;
                    Point correctLocation = originalLocations[index];
                    return pb.Parent != pnlPuzzleBoard || pb.Left != correctLocation.X || pb.Top != correctLocation.Y;
                }).ToList();

                if (incorrectPieces.Count > 0)
                {
                    Random rand = new Random();
                    PictureBox pb = incorrectPieces[rand.Next(incorrectPieces.Count)];

                    int index = (int)pb.Tag;
                    Point correctLocation = originalLocations[index];

                    pb.Parent = pnlPuzzleBoard;
                    pb.Location = new Point(correctLocation.X, correctLocation.Y);

                    pb.MouseDown -= PuzzlePiece_MouseDown;
                    pb.MouseMove -= PuzzlePiece_MouseMove;
                    pb.MouseUp -= PuzzlePiece_MouseUp;

                    remainingHints--;

                    btnHint.Text = $"Hint: {remainingHints}";

                    if (IsPuzzleSolved())
                    {
                        timer.Stop();
                        ShowCompletionMessage(DateTime.Now - startTime);
                    }
                }
                else
                {
                    MessageBox.Show("All pieces are already in their correct positions.");
                }
            }
            else
            {
                MessageBox.Show("No more hints available.");
            }
        }
    }
}
