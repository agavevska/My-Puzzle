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

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Easy&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Medium&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Hard

![image](https://github.com/agavevska/My-Puzzle/assets/138719425/6cfb03c0-4537-4953-aee2-1b9ea596e07f)

`3. Опис на решение на проблемот`

`3.1 Генерирање сложувалка`

Функцијата `GeneratePuzzle()` има за цел да генерира и прикаже парчиња од сложувалката со соодветни контроли. `puzzlePieces = new PictureBox[puzzleSize]` парчињата сложувалка ги чуваме во нов `PictureBox` каде всушност тие претставуваат слика која подоцна се дели на парчиња. Создаваме нова инстанца од класата `Bitmap` `bm` која ја користи изворната слика `originalImage`. 

`Bitmap pieceImage = bm.Clone(new Rectangle(col * pieceWidth, row * pieceHeight, pieceWidth, pieceHeight), bm.PixelFormat)` - оваа линија код креира нов `Bitmap` објект `pieceImage` користејќи методот `Clone()` на постоечкиот `Bitmap објект` `bm` (кој претставува изворна слика). 

На крај креираме нов `PictureBox` `pb` за прикажување на сликата на парчето сложувалка. 

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











