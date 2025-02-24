using Mono.Cecil;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; } 
    public TetrominoData[] tetrominoArray;
    public Vector3Int spawnPosition; //Ubiciaci�n inicial donde spawnean las piezas
    public Vector2Int boardSize = new Vector2Int(10, 20);

    public RectInt Bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(-this.boardSize.x / 2, -this.boardSize.y / 2);
            return new RectInt(position, this.boardSize);
        }
    }

    private void Awake()
    {
        //Llamado cuando se inicializa por primera vez el componente

        /* Antes que nada, asignaremos a nuestro tilemap el tilemap que se estar� usando   /
        /  en el Board, en nuestro editor tenemos un componente Tile, lo tomaremos        */
        this.tilemap = GetComponentInChildren<Tilemap>();
        this.activePiece = GetComponentInChildren<Piece>();

        //Primero iteraremos por nuestro array de piezas tetronimo y las inicializaremos
        for (int i = 0; i < this.tetrominoArray.Length; i++)
        {
            this.tetrominoArray[i].Initialize();
        }
    }

    private void Start()
    {
        /* Cuando nuestro juego originalmente inicie, ac� spawnearemos las piezas */
        SpawnPiece();
    }

    public void SpawnPiece()
    {
        /* Funci�n encargada de spawnear de forma aleatoria una pieza de la lista */
        int random_choise = Random.Range(0, this.tetrominoArray.Length);
        TetrominoData data = this.tetrominoArray[random_choise];

        this.activePiece.Initialize(this, spawnPosition, data);

        if (IsValidPosition(this.activePiece, this.spawnPosition))
        {
            Set(activePiece);
        }
        else
        {
            //Activamos Gameover
            GameOver();
        }
    }

    public void Set(Piece piece)
    {
        //Funci�n setter encargada de asignar ciertos datos al juego como tal
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            this.tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }
    public void Clear(Piece piece)
    {
        //Funci�n setter encargada de limpiar del Tilemap la posible ubicaci�n de la pieza
        //�til antes de modificar la posici�n de la pieza
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            this.tilemap.SetTile(tilePosition, null);
        }
    }
    public void ClearLines()
    {
        /* Ac� buscaremos aquellas l�neas que cumplen ser una fila completa, en dicho  /
        /  caso se liberar�n del sistema y generar�n puntos extra para el jugador,     /
        /  queremos iterar por filas, para �stas filas usaremos los l�mites que        /
        /  establecimos previamente, para empezar de abajo hacia arriba               */
        RectInt bounds = this.Bounds;
        int row = bounds.yMin; //Agarramos el valor m�nimo de los l�mites (en efecto aca abajo es menos y arriba es mas)

        while (row < bounds.yMax)
        {
            //Iteraremos desde abajo hasta arriba, y checaremos por una l�nea completa
            if (IsLineFull(row))
            {
                LineClear(row);
            }
            else
            {
                row++;
            }
        }

    }
    private bool IsLineFull(int row)
    {
        RectInt bounds = this.Bounds;

        //Ahora iteraremos por las columnas de la fila, para determinar que todas tengan tiles
        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0); //Usamos vector 3 ya que los tilemaps lo usan

            if (!this.tilemap.HasTile(position))
            {
                //Si la fila encuentra un punto donde no hay tile, entonces deber�a tirar un error
                return false;
            }
        }
        
        //Si no tir� un error, significa que toda la fila tiene un valor a limpiar
        return true;
    }

    public bool IsValidPosition(Piece piece, Vector3Int newPosition)
    {
        /* Ac� dado una pieza y una posici�n, nos encargaremos de verificar que /
        /  cada una de las celdas piezas sea v�lida */

        //Primero tomamos el l�mite o posibilidad de l�mite del tablero
        RectInt bounds = this.Bounds;

        //Ac� entonces iteraremos en el sistema
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + newPosition;

            //Verificamos si la pieza est� fuera de l�mites, usando el rect�ngulo, para checar que la contenga
            if (!bounds.Contains((Vector2Int)tilePosition))
            {
                return false;
            }

            //Verificamos si la pieza est� ubicada en la ubicaci�n de una pieza previa
            if (this.tilemap.HasTile(tilePosition))
            {
                return false;
            }
        }

        //Si no tir� ning�n error, entonces es una ubicaci�n v�lida
        return true;
    }

    private void LineClear(int row)
    {
        RectInt bounds = this.Bounds;
        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0); //Usamos vector 3 ya que los tilemaps lo usan
            //Ahora limpiaremos esa fila completa
            this.tilemap.SetTile(position, null);
        }

        while (row < bounds.yMax)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row + 1, 0); //Usamos vector 3 ya que los tilemaps lo usan
                TileBase above = this.tilemap.GetTile(position); 

                position = new Vector3Int(col, row, 0);
                this.tilemap.SetTile(position, above);
            }
            row++;
        }
    }

    
    private void GameOver()
    {
        //Limpiamos todo el gameboard, y tambi�n ac� incluir�amos datos como mostrar el Score y m�s
        this.tilemap.ClearAllTiles();
    }

}
