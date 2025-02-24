using UnityEngine;

public class Piece : MonoBehaviour
{
    public Board board { get; private set; }
    public TetrominoData data { get; private set; }
    public Vector3Int[] cells { get; private set; } //Tilemaps usan ésta clase de vectores
    public Vector3Int position { get; private set; } //Tilemaps usan ésta clase de vectores
    public int rotationIndex { get; private set; }   //Manejaremos la rotación mediante una matriz SRS


    //Manejamos cosas como manejo de tiempo y delays
    public float stepDelay = 1f;    //Manejamos un tiempo de 1 segundo
    public float lockDelay = 0.5f;  //Manejamos un tiempo de LockDelay con 0.5 segundos

    //Temporizadores 
    private float stepTime;
    private float lockTime;

    public void Initialize(Board board, Vector3Int position, TetrominoData data)
    {
        //Asignamos los datos o los inicializamos
        this.board = board;
        this.position = position;
        this.data = data;
        this.rotationIndex = 0;

        this.stepTime = Time.time + this.stepDelay; // 
        this.lockTime = 0f;                         // Éste tiempo inicia como 0, es el tiempo de duración para determinar el movimiento

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
        /* Ésta función es llamada automáticamente por cada frame que pasa o que es ejecutado, /
        /  en éste caso lo usaremos para el Input                                             */

        //Primero que todo, limpiamos el tablero y la última ubicación que usó
        this.board.Clear(this);

        //Incrementamos el tiempo del locktime
        this.lockTime += Time.deltaTime;

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
            //Nos movemos a la izquierda
            Move(Vector2Int.left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            //Nos movemos a la derecha
            Move(Vector2Int.right);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            //Nos movemos abajo de forma suave
            Move(Vector2Int.down);
        }

        //Modificar de modo que es si se mantiene presionado que se sigue moviendo hacia abajo
        if (Input.GetKeyDown(KeyCode.Space)){ 
            HardDrop();
        }

        //Acá checamos el SetTime
        if (Time.time >= this.stepTime)
        {
            //Si se aplica el timer como tal, entonces llamamos ésto, de modo que apliquemos el movimiento
            Step();
        }

        //Reseteamos en el tablero con nuestra función set
        this.board.Set(this);
    }


    private bool Move(Vector2Int translation)
    {
        /* Acá manejaremos el movimiento de la pieza usando un vector, para indicar qué tanto  /
        /  nos querremos mover hacia un lado, sin embargo, también querremos validar cosas     /
        /  como la posibilidad de chocar con otra pieza o con una pared                       */
        Vector3Int newPosition = this.position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        //Comunicamos con el board para checar si la posición es válida
        bool valid = this.board.IsValidPosition(this, newPosition);
        if (valid)
        {
            //Podemos modificar la posición de la pieza una vez validado
            this.position = newPosition;
            this.lockTime = 0f; //Se reinicia por cada movimiento hecho
        }

        return valid;
    }

    private void Rotate(int direction)
    {
        /* Modificamos la rotación... sin embargo, tenemos que recordar, es de 0 a 3,     /
        /  entonces tenemos que aplicar también un módulo, para que no se pase de éstos   /
        /  valores                                                                       */

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
        /* Ésta función es ejecutada en caso que la rotación sea posible, ya sea usando   /
        /  wallkicks o sin usar como tal, acá nos encargamos de ejecutar la rotación     */

        //Aplicamos la matriz de rotación como tal
        for (int i = 0; i < this.cells.Length; i++)
        {
            /* Dado que el sistema SRS usa una rotación distinta para los I y O, rotando  /
            /  además con un flotante (división entre 2 de un punto), nos vemos obligados /
            /  a usar un Vector3 normal                                                  */
            Vector3 cell = this.cells[i];
            int x, y; //Nuevas coordenadas al rotar

            /* Dado que la rotación difiere en I O del resto, entonces lo mejor sería     /
            /* usar un switch para determinar qué letra rota                             */
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

        Lock();
    }

    private void Step()
    {
        /* Ésta función es ejecutada cada vez que el timer del StepTime se cumpla, /
        /  lo que sucede es simple, cada vez que ésta función se ejecute, la pieza /
        /  actual se mueve hacia abajo, y entonces se reinicia el timer del step  */
        this.stepTime = Time.time + this.stepDelay;
        Move(Vector2Int.down);

        //Checamos por el caso de un lock, para ésto el timer
        if (this.lockTime >= this.lockDelay)
        {
            Lock();
        }
    }
    private void Lock()
    {
        /* La pieza como tal ha llegado a su punto de bloqueo, ya no se va a mover /
        /  de ésta posición, lo que significa que ésta pieza se quedará quieta,    /
        /  y que nos toca spawnear una nueva pieza                                */
        this.board.Set(this);
        this.board.ClearLines();
        this.board.SpawnPiece();
    }
}
