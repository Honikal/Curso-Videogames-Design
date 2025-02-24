using UnityEngine;
using UnityEngine.Tilemaps;

public class Ghost : MonoBehaviour
{
    public Tile tile;           //Tenemos acceso al tilemap, para dibujar los tiles
    public Board board;         //Ocupamos referencia al tablero
    public Piece trackingPiece; //Ocupamos tener track de la pieza a la cual ocupamos dibujar como ghost

    public Tilemap tilemap { get; private set; }      //Obtenemos el Tilemap encargado de dibujar el fantasma
    public Vector3Int[] cells { get; private set; }   //Obtenemos las celdas que va a tener la pieza que planeamos dibujar

    public Vector3Int position { get; private set; }  //Obtenemos la posici�n a d�nde dibujar la pieza fantasma


    private void Awake()
    {
        //Seteamos variables iniciando el juego como tal
        this.tilemap = GetComponentInChildren<Tilemap>();
        this.cells = new Vector3Int[4];   //Celdas a dibujar
    }

    private void LateUpdate()
    {
        /* �sta funci�n principal es especial ya que se ejecuta siempre despu�s de todos los Update */

        //Primero limpiamos los cambios antiguos de la pieza
        Clear();

        //Luego copiaremos datos de la pieza inicial, como las celdas y la posici�n x
        Copy();

        //Luego hacemos la funci�n de Drop, de modo que dropeamos el clon fantasma a la ubicaci�n esperada
        Drop();

        //Finalmente hacemos la funci�n del Set, ambas Clear y Set son casi las mismas que el tablero
        Set();
    }

    private void Clear()
    {
        for (int i = 0; i < this.cells.Length; i++)
        {
            Vector3Int tilePosition = this.cells[i] + this.position;
            this.tilemap.SetTile(tilePosition, null);
        }
    }
    private void Copy()
    {
        //Copiamos las piezas de la pieza a la que copiamos
        for (int i = 0; i < this.cells.Length; i++)
        {
            this.cells[i] = this.trackingPiece.cells[i];
        }
    }
    private void Drop()
    {
        //Aplicamos una funci�n de drop como tal
        Vector3Int position = this.trackingPiece.position;

        int currentRow = position.y;
        int bottom = -this.board.boardSize.y / 2 - 1;

        this.board.Clear(this.trackingPiece);

        for (int row = currentRow; row >= bottom; row--)
        {
            position.y = row;

            if (this.board.IsValidPosition(this.trackingPiece, position))
            {
                this.position = position;
            } else
            {
                //Si ya lleg� a un punto donde no es v�lido, entonces no es necesario actualizar la ubicaci�n de la pieza, ya habr�a llegado a una posici�n m�xima
                break;
            }
        }

        this.board.Set(this.trackingPiece);
    }
    private void Set()
    {
        for (int i = 0; i < this.cells.Length; i++)
        {
            Vector3Int tilePosition = this.cells[i] + this.position;
            this.tilemap.SetTile(tilePosition, this.tile);
        }
    }
}
