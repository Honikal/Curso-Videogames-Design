using System.Collections.Generic;
using UnityEngine;

public static class Data
{
    /*Conseguimos el valor flotante de cos y sin, será utilizado para la animación del como se cambia la rotación
    de los objetos*/
    public static readonly float cos = Mathf.Cos(Mathf.PI / 2f);
    public static readonly float sin = Mathf.Sin(Mathf.PI / 2f);
    public static readonly float[] RotationMatrix = new float[] { cos, sin, -sin, cos };

    //Ahora manejamos un diccionario dónde demostramos el cómo se dibuja cada uno de los Tetróminos
    public static readonly Dictionary<Tetromino, Vector2Int[]> Cells = new Dictionary<Tetromino, Vector2Int[]>()
    {
        { Tetromino.I, new Vector2Int[] {new Vector2Int(-1, 1), new Vector2Int(0, 1),  new Vector2Int(1,1),  new Vector2Int(2, 1) } },
        { Tetromino.J, new Vector2Int[] {new Vector2Int(-1, 1), new Vector2Int(-1, 0), new Vector2Int(0,0),  new Vector2Int(1, 0) } },
        { Tetromino.L, new Vector2Int[] {new Vector2Int(1, 1),  new Vector2Int(-1, 0), new Vector2Int(0,0),  new Vector2Int(1, 0) } },
        { Tetromino.O, new Vector2Int[] {new Vector2Int(0, 1),  new Vector2Int(1, 1),  new Vector2Int(0,0),  new Vector2Int(1, 0) } },
        { Tetromino.S, new Vector2Int[] {new Vector2Int(0, 1),  new Vector2Int(1, 1),  new Vector2Int(-1,0), new Vector2Int(0, 0) } },
        { Tetromino.T, new Vector2Int[] {new Vector2Int(0, 1),  new Vector2Int(-1, 0), new Vector2Int(0,0),  new Vector2Int(1, 0) } },
        { Tetromino.Z, new Vector2Int[] {new Vector2Int(-1, 1), new Vector2Int(0, 1),  new Vector2Int(0,0),  new Vector2Int(1, 0) } }
    };

    //Manejamos el cómo el Tetronimo I se comporta al chocar con la pared
    private static readonly Vector2Int[,] WallkicksI = new Vector2Int[,]
    {
        { new Vector2Int(0, 0), new Vector2Int(-2, 0), new Vector2Int( 1, 0), new Vector2Int(-2, -1), new Vector2Int( 1,  2) },
        { new Vector2Int(0, 0), new Vector2Int( 2, 0), new Vector2Int(-1, 0), new Vector2Int( 2,  1), new Vector2Int(-1, -2) },
        { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int( 2, 0), new Vector2Int(-1,  2), new Vector2Int( 2, -1) },
        { new Vector2Int(0, 0), new Vector2Int( 1, 0), new Vector2Int(-2, 0), new Vector2Int( 1, -2), new Vector2Int(-2,  1) },
        { new Vector2Int(0, 0), new Vector2Int( 2, 0), new Vector2Int(-1, 0), new Vector2Int( 2,  1), new Vector2Int(-1, -2) },
        { new Vector2Int(0, 0), new Vector2Int(-2, 0), new Vector2Int( 1, 0), new Vector2Int(-2, -1), new Vector2Int( 1,  2) },
        { new Vector2Int(0, 0), new Vector2Int( 1, 0), new Vector2Int(-2, 0), new Vector2Int( 1, -2), new Vector2Int(-2,  1) },
        { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int( 2, 0), new Vector2Int(-1,  2), new Vector2Int( 2, -1) }
    };

    //Manejamos el cómo el Tetronimo J se comporta al chocar con la pared
    private static readonly Vector2Int[,] WallkicksJLostZ = new Vector2Int[,]
    {
        { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(-1, 1), new Vector2Int(0,-2), new Vector2Int(-1, -2) },
        { new Vector2Int(0, 0), new Vector2Int( 1, 0), new Vector2Int( 1,-1), new Vector2Int(0, 2), new Vector2Int( 1,  2) },
        { new Vector2Int(0, 0), new Vector2Int( 1, 0), new Vector2Int( 1,-1), new Vector2Int(0, 2), new Vector2Int( 1,  2) },
        { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(-1, 1), new Vector2Int(0,-2), new Vector2Int(-1, -2) },
        { new Vector2Int(0, 0), new Vector2Int( 1, 0), new Vector2Int( 1, 1), new Vector2Int(0,-2), new Vector2Int( 1, -2) },
        { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(-1,-1), new Vector2Int(0, 2), new Vector2Int(-1,  2) },
        { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(-1,-1), new Vector2Int(0, 2), new Vector2Int(-1,  2) },
        { new Vector2Int(0, 0), new Vector2Int( 1, 0), new Vector2Int( 1, 1), new Vector2Int(0,-2), new Vector2Int( 1, -2) }
    };

    //Acá lo que haremos básicamente es hacer un diccionario y colocaremos dónde los bloques deban de tener como un choque o rebote
    public static readonly Dictionary<Tetromino, Vector2Int[,]> Wallkicks = new Dictionary<Tetromino, Vector2Int[,]>()
    {
        {Tetromino.I, WallkicksI },
        {Tetromino.J, WallkicksJLostZ },
        {Tetromino.L, WallkicksJLostZ },
        {Tetromino.O, WallkicksJLostZ },
        {Tetromino.T, WallkicksJLostZ },
        {Tetromino.S, WallkicksJLostZ },
        {Tetromino.Z, WallkicksJLostZ }
    };

}
