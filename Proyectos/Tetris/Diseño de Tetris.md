Acá documentaremos el aprendizaje de la tarea o proyecto de calentamiento, asignada para el **3 de marzo del 2025**, acá deberemos de crear un videojuego en formato WebGL y publicarlo en nuestra página propia de Itch.IO.

Primero he de decir, decidí escoger Tetris ya que por cuenta propia junto a varios compañeros de la Universidad estamos haciendo el reto [20 Games Challenge](https://20_games_challenge.gitlab.io/challenge/), y por nivel de dificultad consideré que es el que está más a mi nivel (ahora mismo voy por nivel 2, ya hice el Breakout y me falta el otro). Al mismo tiempo, escogí Godot 4.3 ya que tenía más experiencia que Unity, sin embargo, acá sin embargo si estoy obligado a aprender las artes del Unity.

Seré sincero, si empecé el proyecto guiándome de un tutorial del cual lo hace paso a paso, pero más que todo lo hice debido a que no entiendo datos como el cómo funciona la interfaz del programa, así como el cómo funciona el manejo de sprites y assets del sistema. Acá cito el vídeo que utilicé para guiarme inicialmente: [How to make Tetris in Unity](https://www.youtube.com/watch?v=ODLzYI4d-J8).

Acá aprendí que a diferencia de Godot 4, si es obligatorio usar Cámaras, a utilizar en los sprites datos cómo el modificar *pixels per unit, wrap mode, mesh type*, o también el modificar espacio que abarca una cámara o el background color que usa ésta.

El juego Tetris originalmente usa un espacio entre **10x20**, 10 de ancho y 20 de alto, podemos usar un objeto grid, y podemos usar un objeto para el borde, y un grid dónde dibujaremos los tilesets. Acá tenemos que tener en cuenta el atributo de los objetos *Order in Layer*, que es como el *Z index* u orden de dibujo.  **Acá elegí como el grid negro o base como 0, el borde como 3, ya que debe dibujarse encima de los demás, y el tilemap del grid como 2, el 1 será utilizado o es opcional, para un tilemap donde se dibujaría un tile que es como el fantasma de dónde se verían la ubicación del bloque a colocar**.

Luego vamos al tilemap tablero, veremos una opción llamada **Open Tile Palette**, acá le damos click, creamos una nueva paleta de Tilesets y le ponemos nombre, luego de ésto pasamos los sprites que usamos y daremos uso. 

Luego, es importante entender el cómo funciona Tetris, para esto usaremos el estudio conocido como los [tetrónimos](https://es.wikipedia.org/wiki/Tetromin%C3%B3), ésta geometría la cual está compuesta de 4 cuadrados iguales, conectados entre sí **ortogonalmente (de lado)**, acá tendremos que formar el código que permitirá definirlas, usando tanto el tilemap para los distintos colores a usar, como el código para definir las colisiones.

Luego usaremos la definición de tetrónimos para hacer el script, haremos de hecho 3 scripts, usando el lenguaje que usa Unity, conocido como `C#`, acá definiremos el Script de `Tetronimo`, `Board` o tablero y `Data`, en el de Tetrónimo, usaremos el documento de Wikipedia, lo leeremos primero para entender el contexto, observamos que los Tetrónimo son demostrados por 5 letras, (o 7 considerando que al voltear 2 en su eje forman otras letras), y con base a ésto, definimos el código de ese Script.

```C#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Tetromino
{
    I,         //Es una linea recta
    O,         //Es un cuadrado
    T,         //Tiene forma de T
    L,         //Representa una forma de L o bota
    J,         //Forma invertida de la L  
    Z,         //Forma de 2 piezas de domino horizontales, una chocando con la otra
    S          //Forma invertida de la Z
}
```

Además, acá también en éste script tendremos acceso a los Tiles y al Enum, creando un struct que tenga acceso a ambos.

```C#
[System.Serializable] //Para hacer que éstos datos sean vistos desde el Editor
public struct TetrominoData
{
    public Tetromino tetromino;
    public Tile tile;
	public Vector2Int[] cells;
}
```

Mientras, en el script del tablero tendremos acceso a un array de estos structs como tal. Una vez así, observamos que así es, el código si llega a afectar el cómo se observa en el editor y el que tiene acceso a ciertos datos, casi como que de forma automática hace lo que en Godot se vería con el @export var.

```C#
using UnityEngine;

public class Board : MonoBehaviour
{
    public TetrominoData[] tetrominoArray;
}
```
En el editor, especificamos que usamos 7 diferentes tipos de piezas en el nuevo objeto que aparece en el script de board. Por cada una de éstas especificamos a que Enum o tetronimo pertenecen, y que tiles pintan como tal. Para seleccionar que tiles corresponden vamos a nuestra carpeta de tile y seleccionamos de la lista que usamos (nos podemos guiar del juego original). 

Luego trabaremos en el script `data` en el cual lo que haremos es indicar o cómo manejar el cómo se dibujarán los tetrónimos. Formar éste data es bastante tedia, no hay más que decir eso, lo importante sin embargo, es el trabajar con el código para inicializar los datos, nos tocar modificar la clase de tetrónimo, donde crearemos una función para inicializar las celdas a pintar o utilizar, usando los datos de la clase estática `data`. 

```C#
[System.Serializable] //Para hacer que éstos datos sean vistos desde el Editor
public struct TetrominoData
{
    public Tetromino tetromino;
    public Tile tile;
    public Vector2Int[] cells { get; private set; } //de ésta manera ya no salen en el editor de Unity //

    public void Initialize()
    {
        /*Acá tomamos acceso al diccionario Cells que formamos, y acá pasaremos el valor key o clave, en éste caso el tetronimo de éste objeto, de ésta manera tenemos acceso fácil a las celdas a usar */
        this.cells = Data.Cells[this.tetromino];
    }
}
```

Luego, podemos empezar a probar la lógica del juego, para esto codificaremos en el Script de Board, que se verá un poco así:

```C#
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    public TetrominoData[] tetrominoArray;

    private void Awake()
    {
        this.tilemap = GetComponentInChildren<Tilemap>();

        for (int i = 0; i < this.tetrominoArray.Length; i++)
        {
            this.tetrominoArray[i].Initialize();
        }
    }
    private void Start()
    {
        SpawnPiece();
    }
    public void SpawnPiece()
    {
        int random_choise = Random.Range(0, this.tetrominoArray.Length);
        TetrominoData data = this.tetrominoArray[random_choise];
    }

	//Función setter encargada de asignar ciertos datos al juego como tal
    public void Set() {} 
}
```

Lo bueno sin embargo, de Tetris, es que en cuanto a Inputs, solo manejamos el Input de una sola pieza, así que en realidad podemos crear un código para la pieza a usar en general, mientras que el código que tenemos de Tetronimo, es más como el generador del cómo debe de lucir ésta, en nuestro Script `Piece` nos encargaremos de hacer cosas como el movimiento, y el Input.

Éste Script debe de contener datos, tales como su ubicación de spawn, datos que maneja la pieza siendo Tetrónimo, y una referencia al tablero para luego ejercer funciones (tales como para la puntuación, y así)... por eso primero empezamos con una función para inicializar datos.

```C#
using UnityEngine;

public class Piece : MonoBehaviour
{
    public Board board { get; private set; }
    public TetrominoData data { get; private set; }
    public Vector3Int[] cells { get; private set; } //Tilemaps usan ésta clase de vectores

    public Vector3Int position { get; private set; } //Tilemaps usan ésta clase de vectores

    public void Initialize(Board board, Vector3Int position, TetrominoData data)
    {
        //Asignamos los datos o los inicializamos
        this.board = board;
        this.position = position;
        this.data = data;

        if (this.cells == null)
        {
            this.cells = new Vector3Int[data.cells.Length];
        }
        for (int i = 0; i < data.cells.Length; i++)
        {
            this.cells[i] = (Vector3Int)data.cells[i];
        }
    }
}
```

Y luego en el Tablero o Gameboard de nuevo:

```C#
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; } 
    public TetrominoData[] tetrominoArray;
    public Vector3Int spawnPosition; //Ubiciación inicial donde spawnean las piezas

    private void Awake()
    {
        this.tilemap = GetComponentInChildren<Tilemap>();
        this.activePiece = GetComponentInChildren<Piece>();

        for (int i = 0; i < this.tetrominoArray.Length; i++)
        {
            this.tetrominoArray[i].Initialize();
        }
    }
    private void Start()
    {
        SpawnPiece();
    }
    public void SpawnPiece()
    {
        int random_choise = Random.Range(0, this.tetrominoArray.Length);
        TetrominoData data = this.tetrominoArray[random_choise];

        this.activePiece.Initialize(this, spawnPosition, data);
        Set(activePiece);
    }
    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            this.tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }
}
```

Luego probamos, observamos que la variable `spawnPosition` asignada en nuestro sistema centra a la pieza, según visto normalmente usualmente spawnean más de un lado (izquierdo o derecho) que en el centro, así que podemos modificar los valores, por ahora, los valores los dejé de la siguiente manera: *(x = 1, y = 8, z = 0)*. La aparición usualmente, dado que la mitad es tipo en posición 10, luego, las piezas toman mínimo un espacio de 2, entonces el valor más recomendado sería (10 - 2), en éste caso 8. 

Volvemos al código de la pieza, dónde controlamos lo básico, movimiento, Input y demás. Acá manejaremos el movimiento, ocuparemos volver también al Board para crear una función **Clear** pero es igual a la función set, para limpiar el Tilemap al movernos. Luego empezaremos rotación, y para eso podemos usar varios materiales para guiarnos, entre ellos el [SRS](https://harddrop.com/wiki/SRS), el cual es conocido como **Super Rotation System**, éste sistema en sí funciona como una matriz de 4 direcciones, la cual la vamos a utilizar de ésta manera para la rotación. También lo manejamos dentro de nuestra pieza.

Como previamente se mencionó, en data lo que usamos es lo conocido como un [Rotation Matrix](https://en.wikipedia.org/wiki/Rotation_matrix)para realizar las rotaciones, éste es usado matemática y algebraicamente para usar la rotación Euclidiana. Éste lo usaremos para definir las nuevas posiciones de los tiles a tomar en consideración. Una vez implementado, observaremos el caso de un bug, al rotar algunas piezas parecen salir del límite, para eso usamos la otra parte tediosa del código que son los WallKicks, también, tenemos 2, ya que tenemos el caso de I y O que son distintos, pero en resumen, en algunos casos al rotar se ocupa empujar del lado de la pared para que no se salgan de los límites. De nuevo, se usó la mayor parte de la página del [Super Rotation System](https://harddrop.com/wiki/SRS) para la realización del WallKick también. 

Como el WallKick es diferente con I, debemos de modificar el Tetrómino de modo que tome en cuenta ésto:

```C#
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Tetromino
{
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
    public Tetromino tetromino;
    public Tile tile;
    public Vector2Int[] cells { get; private set; } 
    public Vector2Int[,] wallKicks { get; private set; } //almacenamos los wallkicks a usar               //

    public void Initialize()
    {
        this.cells = Data.Cells[this.tetromino];
        this.wallKicks = Data.Wallkicks[this.tetromino]; //Agregado nuevo
    }
}
```

Luego del cambio, podemos volver a modificar el código de la Pieza para efectuar los WallKicks.

```C#
using UnityEngine;

public class Piece : MonoBehaviour
{
    public Board board { get; private set; }
    public TetrominoData data { get; private set; }
    public Vector3Int[] cells { get; private set; } //Tilemaps usan ésta clase de vectores
    public Vector3Int position { get; private set; } 
    public int rotationIndex { get; private set; }   //Manejaremos la rotación mediante una matriz SRS

    public void Initialize(Board board, Vector3Int position, TetrominoData data)
    {
        //Asignamos los datos o los inicializamos
        this.board = board;
        this.position = position;
        this.data = data;
        this.rotationIndex = 0;

        if (this.cells == null)
        {
            this.cells = new Vector3Int[data.cells.Length];
        }
        for (int i = 0; i < data.cells.Length; i++)
        {
            this.cells[i] = (Vector3Int)data.cells[i];
        }
    }
    private void Update()
    {
        //Primero que todo, limpiamos el tablero y la última ubicación que usó
        this.board.Clear(this);

        //Manejamos acá los Inputs para rotar las piezas
        if (Input.GetKeyDown(KeyCode.Keypad1)){
            //Rotamos izquierda
            Rotate(-1);
        } else if (Input.GetKeyDown(KeyCode.Keypad3)){
            //Rotamos a la derecha
            Rotate(1);
        }

        //Aplicamos los inputs para checar el movimiento
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Move(Vector2Int.left);   //Nos movemos a la izquierda
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Move(Vector2Int.right);  //Nos movemos a la derecha
        }

        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            Move(Vector2Int.down);   //Nos movemos abajo de forma suave
        }

        if (Input.GetKeyDown(KeyCode.Space)){ 
            HardDrop();
        }

        //Reseteamos en el tablero con nuestra función set
        this.board.Set(this);
    }


    private bool Move(Vector2Int translation)
    {
        Vector3Int newPosition = this.position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        //Comunicamos con el board para checar si la posición es válida
        bool valid = this.board.IsValidPosition(this, newPosition);
        if (valid)
        {
            this.position = newPosition;
        }
        return valid;
    }

    private void Rotate(int direction)
    {
        int originalRotation = this.rotationIndex;
        this.rotationIndex = Wrap(this.rotationIndex + direction, 0, 4);

        ApplyRotationMatrix(direction);

        if (!TestWallKick(this.rotationIndex, originalRotation))
        {
            //Aplicamos la rotación al lado inverso
            this.rotationIndex = originalRotation;
            ApplyRotationMatrix(-direction);
        }
    }
    
    private void ApplyRotationMatrix(int direction)
    {
        /* Ésta función es ejecutada en caso que la rotación sea posible, ya sea              usando wallkicks o sin usar como tal, acá nos encargamos de ejecutar la            rotación */

        //Aplicamos la matriz de rotación como tal
        for (int i = 0; i < this.cells.Length; i++)
        {
            /* Dado que el sistema SRS usa una rotación distinta para los I y O,                  rotando además con un flotante (división entre 2 de un punto), nos                 vemos obligados a usar un Vector3 normal */
            Vector3 cell = this.cells[i];
            int x, y; //Nuevas coordenadas al rotar

            switch (this.data.tetromino)
            {
                case Tetromino.I:
                case Tetromino.O:
                    //La rotación difiera en la letra I y la O
                    //Primero modificamos el offset de ambos por la mitad de una unidad
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    //Redondeamos al techo del valor 
                    x = Mathf.CeilToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;
                default:
                    //El método default, la rotación se aplica de la misma manera para el resto de letras
                    x = Mathf.RoundToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;
            }

            this.cells[i] = new Vector3Int(x, y, 0);
        }
    }

    private bool TestWallKick(int rotationIndex, int rotationDirection)
    {
        //Lo primero que debemos de saber es a cual WallKick es al que vamos a aplicar, ocupamos el index
        int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);
        
        //Luego iteraremos pero ésta vez sobre la dimensión 2, que son la lista de tests (son 5)
        for (int i = 0; i < this.data.wallKicks.GetLength(1); i++)
        {
            //Iteramos por cada uno de los tests, si todos fallan entonces se ocupa redireccionar la rotación
            Vector2Int translation = this.data.wallKicks[wallKickIndex, i];
            if (Move(translation))
            {
                //Si se puede mover entonces, si existe posibilidad de wall kick
                return true;
            }
        }
        //Todos los sets fallaron como tal, la rotación falló en algo y por lo tanto no debería aplicarse
        return false;
    }
    private int GetWallKickIndex(int rotationIndex, int rotationDirection)
    {
        //Obtiene el index del wallKick al que se debe checar
        //Al observar que se sigue un patrón, los valores impares son para ir reverso, los valores 
        //pares usualmente son para ir adelante
        int wallKickIndex = rotationIndex * 2;
        if (rotationDirection < 0)
        {
            wallKickIndex--;
        }
        //Aplicamos el Wrap también
        return Wrap(wallKickIndex, 0, this.data.wallKicks.GetLength(0));
    }

    private int Wrap(int input, int min, int max)
    {
        if (input < min)
        {
            return max - (min - input) % (max - min);
        } else
        {
            return min + (input - min) % (max - min);
        }
    }

    private void HardDrop()
    {
        while (Move(Vector2Int.down)) {
            continue;
        }
    }
}
```

Luego de haber checado éste código, avanzaremos con el siguiente paso, hacer que las piezas caigan a su tiempo, y hacer que una vez caigan al suelo no ocupen ejecutarse más, a esto lo conoceremos como **Steps** y **Locks**. Acá básicamente lo que haremos es un timer para cada vez que la pieza vaya cayendo una cantidad de tiempo sin la necesidad de ejecución del jugador, para agregar la dificultad. Por otro lado, el *Lock time* nos permitirá básicamente definir cuando una pieza definitivamente tocó el piso y no hizo movimiento (en caso que el jugador quiera moverla un poco más a los lados), antes de quedarse completamente quieta y spawnear otra pieza.

Luego de haber aplicado los timers, nos toca el último paso, hacer que el tablero libere o limpie las piezas que cumplan el caso de Tetris deseado (una fila completa). Después de eso haremos el fantasma como tal. También hacemos código para el GameOver, de modo que al verificar, si una pieza spawnea en un lugar que no es verídico (no hay espacio ya que los campos están completos), tiramos aviso de GameOver.

Hacemos una función de Ghost, de modo que una pieza tiene un fantasma que tiene en el suelo, de modo que se ve bien en el suelo, y aparece dónde se pondría en el suelo. Para esto creamos un nuevo Script llamado Ghost, éste por supuesto, debe tener acceso a la pieza que planea interpretar como copia, al tilemap o las celdas a considerar donde dibujarlos, y a una posición. El código es simple, limpiar del sistema la pieza copiada (por si cambia la pieza actual), copiar los datos de la celda de la pieza a copiar, e imitar el hecho que estamos en el suelo de la pieza, y hacer la función de set.

Claro, eso habría sido siguiendo el tutorial, ahora siendo el día 24/02/2025, intentaré reiniciar el proyecto de nuevo pero por cuenta propia. Para hacer que tenga UI, además del UI hacer que tenga algo para demostrar el cómo se vería la siguiente pieza como tal.
