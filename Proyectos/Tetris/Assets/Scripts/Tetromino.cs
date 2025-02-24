using UnityEngine;
using UnityEngine.Tilemaps;

public enum Tetromino
{
    /*Aca manejaremos de acuerdo al documento en Wikipedia, el Tetronimo, aca los definiremos como parte de /
    /codigo y tendran o seguiran dicha forma de identificacion, para darles sentido en su forma fisica      /
    /despues en el juego                                                                                   */

    /*Segun el documento en Wikipedia, los tetronimos se pueden definir como 5, y se definen por su         /
    /similitud a una letra del alfabeto usual, con base a esto, definimos el codigo como un ENUM de estos   /
    /5 tetronimos.                                                                                         */

    I,         //Es una linea recta
    O,         //Es un cuadrado
    T,         //Tiene forma de T
    L,         //Representa una forma de L o bota
    J,         //Forma invertida de la L  
    Z,         //Forma de 2 piezas de domino horizontales, una chocando con la otra
    S          //Forma invertida de la Z
}

[System.Serializable] //Para hacer que éstos datos sean vistos desde el Editor
public struct TetrominoData
{
    /* Acá intentaremos hacer que la data sea vista desde el editor, además, seleccionaremos cual de los   /
    /  datos deseamos asociar con el dato, además de seleccionar qué tiles dibujar de acuerdo al Tetromino /
    /  seleccionado */

    public Tetromino tetromino;
    public Tile tile;
    public Vector2Int[] cells { get; private set; } //de ésta manera ya no salen en el editor de Unity    //
    public Vector2Int[,] wallKicks { get; private set; } //almacenamos los wallkicks a usar               //

    public void Initialize()
    {
        /*Acá tomamos acceso al diccionario Cells que formamos, y acá pasaremos el valor key o clave, en   /
        /éste caso el tetronimo de éste objeto, de ésta manera tenemos acceso fácil a las celdas a usar   */
        this.cells = Data.Cells[this.tetromino];
        this.wallKicks = Data.Wallkicks[this.tetromino];
    }
}
