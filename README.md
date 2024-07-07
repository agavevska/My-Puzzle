# MyPuzzle

My Puzzle е апликација од забавен карактер развиена во C# со користење на Windows Forms. Апликацијата овозможува корисникот да прикачи слика, која потоа се дели на парчиња. Корисникот може да избере тежина на играта преку радио копчиња, што влијае на бројот и големината на деловите.

`1. Опис на апликацијата`

My Puzzle е апликација која има 3 нивоа:
* Easy (3x3)
* Medium (5x5)
* Hard (7x7)

Со еден клик, корисникот може лесно да одбере на која тежина сака да ја игра играта.

Функционалности на играта:
* `Start` копче - при самото вклучување на играта, корисникот треба да го кликне ова копче за тајмерот на играта да почне да одбројува.
* `Upload` копче - играта има опција за корисникот да може да прикачи своја фотографија. На самиот почеток корисникот може да одлучи дали сака да продолжи со играта со веќе поставената фотографија или со онаа што ја прикачил.
* `Hint` копче - со клик на ова копче корисникот добива помош при коплетирање на сложувалката. Едно парче сложувалка автоматски оди на точното место на таблата. Оваа помош корисникот може да ја искористи максимум 3 пати.
* `Restart` копче - во случај корисникот да се предомислил во врска со тежината на сложувалката, или пак сликата, може да го кликне ова копче и да започне повторно со играта.

![image](https://github.com/agavevska/My-Puzzle/assets/138719425/12ecf722-bdf2-423e-b088-c7a5e63df2d8)

`2. Упатство за користење`

- При стартување на апликацијата, веднаш под логото ќе може да изберете на која тежина сакате да ја играте играта.
- Прикачeте своја фотографија или пак продолжете со веќе поставената.
- Кликате на копчето `Start` со што тајмерот започнува да одбројува.
- Ги местите парчињата сложувалка да одговараат на избраната фотографија, со едноставно повлекување кон таблата. Ако парчето е поставено на соодветната локација, нема да може повторно да се помести, со што корисникот ќе знае дека се наоѓа на точно место.
- По успешно завршување на играта, може да одберете дали сакате да играте повторно или не.

![image](https://github.com/agavevska/My-Puzzle/assets/138719425/ed9a9e91-b743-4287-8e5b-837bc240f1da)

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Easy&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Medium&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Hard

![image](https://github.com/agavevska/My-Puzzle/assets/138719425/6cfb03c0-4537-4953-aee2-1b9ea596e07f)

`3. Опис на структурата`

```
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
    }
}
```


* `originalImage` - променлива за чување на оригиналната слика која се користи за креирање на парчиња за сложувалката.
* `puzzlePieces` - низа од `PictureBox` контроли кои ги претставуваат парчињата од сложувалката на формата.
* `originalLocations` - низа од `Point` структури кои ги чуваат оригиналните локации (координати) на парчињата.
* `snapThreshold` - константа која дефинира колку блиску треба да биде некое парче за да се закачи на своето место.


`4. Опис на решението`

`4.1 Генерирање сложувалка`

Функцијата `GeneratePuzzle()` има за цел да генерира и прикаже парчиња од сложувалката со соодветни контроли. `puzzlePieces = new PictureBox[puzzleSize]` парчињата сложувалка ги чуваме во нов `PictureBox` каде всушност тие претставуваат слика која подоцна се дели на парчиња. Создаваме нова инстанца од класата `Bitmap` `bm` која ја користи изворната слика `originalImage`. 

`Bitmap pieceImage = bm.Clone(new Rectangle(col * pieceWidth, row * pieceHeight, pieceWidth, pieceHeight), bm.PixelFormat)` 

Oваа линија код креира нов `Bitmap` објект `pieceImage` користејќи методот `Clone()` на постоечкиот `Bitmap објект` `bm` (кој претставува изворна слика). На крај креираме нов `PictureBox` `pb` за прикажување на сликата на парчето сложувалка. 

```
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
```


`4.2 Користење помош`

Функцијата `BtnHint_Click()` има за цел да му помогне на корисникот за комплетирање на сложувалката. Се наоѓаат сите парчиња `PictureBox` кои не се на нивните точни позиции. Ова се прави со споредба на нивната локација, со точната локација од `originalsLocations` за соодветниот индекс.

Ако постојат неправилно поставени парчиња, по случаен избор се избира едно и се поставува на неговата точна локација на `pnlPuzzleBoard`. Со ова се намалува бројот за користење на помош.

```
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
```


`4.3 Прикачување своја фотографија`

Методот `BtnUpload_Click()` е поврзан со кликнување на копчето `btnUpload` за прикачување на фотографија. Се отвора нов прозорец за избор на датотека `OpenFileDialog()`. `Filter` својството ограничува кои видови на датотеки може да се отворат.

Сликата од датотеката која е избрана се чита во променливата `originalImage` со помош на методот `Image.FromFile.` Потоа, сликата се поставува како слика на `PictureBox`, контролата `pbUploadedImage` на формата, што овозможува корисникот да ја види избраната слика на екранот.

```
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
```










