using UnityEngine;

public class Piece : MonoBehaviour
{
    public Board board { get; private set; }
    public TetrominoData data { get; private set; }
    public Vector3Int[] cells { get; private set; } //Tilemaps usan �sta clase de vectores
    public Vector3Int position { get; private set; } //Tilemaps usan �sta clase de vectores
    public int rotationIndex { get; private set; }   //Manejaremos la rotaci�n mediante una matriz SRS


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
        this.lockTime = 0f;                         // �ste tiempo inicia como 0, es el tiempo de duraci�n para determinar el movimiento

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
        /* �sta funci�n es llamada autom�ticamente por cada frame que pasa o que es ejecutado, /
        /  en �ste caso lo usaremos para el Input                                             */

        //Primero que todo, limpiamos el tablero y la �ltima ubicaci�n que us�
        this.board.Clear(this);

        //Incrementamos el tiempo del locktime
        this.lockTime += Time.deltaTime;

        //Manejamos ac� los Inputs para rotar las piezas
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

        //Ac� checamos el SetTime
        if (Time.time >= this.stepTime)
        {
            //Si se aplica el timer como tal, entonces llamamos �sto, de modo que apliquemos el movimiento
            Step();
        }

        //Reseteamos en el tablero con nuestra funci�n set
        this.board.Set(this);
    }


    private bool Move(Vector2Int translation)
    {
        /* Ac� manejaremos el movimiento de la pieza usando un vector, para indicar qu� tanto  /
        /  nos querremos mover hacia un lado, sin embargo, tambi�n querremos validar cosas     /
        /  como la posibilidad de chocar con otra pieza o con una pared                       */
        Vector3Int newPosition = this.position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        //Comunicamos con el board para checar si la posici�n es v�lida
        bool valid = this.board.IsValidPosition(this, newPosition);
        if (valid)
        {
            //Podemos modificar la posici�n de la pieza una vez validado
            this.position = newPosition;
            this.lockTime = 0f; //Se reinicia por cada movimiento hecho
        }

        return valid;
    }

    private void Rotate(int direction)
    {
        /* Modificamos la rotaci�n... sin embargo, tenemos que recordar, es de 0 a 3,     /
        /  entonces tenemos que aplicar tambi�n un m�dulo, para que no se pase de �stos   /
        /  valores                                                                       */

        int originalRotation = this.rotationIndex;
        this.rotationIndex = Wrap(this.rotationIndex + direction, 0, 4);

        ApplyRotationMatrix(direction);

        if (!TestWallKick(this.rotationIndex, originalRotation))
        {
            //Aplicamos la rotaci�n al lado inverso
            this.rotationIndex = originalRotation;
            ApplyRotationMatrix(-direction);
        }
    }
    
    private void ApplyRotationMatrix(int direction)
    {
        /* �sta funci�n es ejecutada en caso que la rotaci�n sea posible, ya sea usando   /
        /  wallkicks o sin usar como tal, ac� nos encargamos de ejecutar la rotaci�n     */

        //Aplicamos la matriz de rotaci�n como tal
        for (int i = 0; i < this.cells.Length; i++)
        {
            /* Dado que el sistema SRS usa una rotaci�n distinta para los I y O, rotando  /
            /  adem�s con un flotante (divisi�n entre 2 de un punto), nos vemos obligados /
            /  a usar un Vector3 normal                                                  */
            Vector3 cell = this.cells[i];
            int x, y; //Nuevas coordenadas al rotar

            /* Dado que la rotaci�n difiere en I O del resto, entonces lo mejor ser�a     /
            /* usar un switch para determinar qu� letra rota                             */
            switch (this.data.tetromino)
            {
                case Tetromino.I:
                case Tetromino.O:
                    //La rotaci�n difiera en la letra I y la O
                    //Primero modificamos el offset de ambos por la mitad de una unidad
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    //Redondeamos al techo del valor 
                    x = Mathf.CeilToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;
                default:
                    //El m�todo default, la rotaci�n se aplica de la misma manera para el resto de letras
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
        
        //Luego iteraremos pero �sta vez sobre la dimensi�n 2, que son la lista de tests (son 5)
        for (int i = 0; i < this.data.wallKicks.GetLength(1); i++)
        {
            //Iteramos por cada uno de los tests, si todos fallan entonces se ocupa redireccionar la rotaci�n
            Vector2Int translation = this.data.wallKicks[wallKickIndex, i];
            if (Move(translation))
            {
                //Si se puede mover entonces, si existe posibilidad de wall kick
                return true;
            }
        }
        //Todos los sets fallaron como tal, la rotaci�n fall� en algo y por lo tanto no deber�a aplicarse
        return false;
    }
    private int GetWallKickIndex(int rotationIndex, int rotationDirection)
    {
        //Obtiene el index del wallKick al que se debe checar
        //Al observar que se sigue un patr�n, los valores impares son para ir reverso, los valores 
        //pares usualmente son para ir adelante
        int wallKickIndex = rotationIndex * 2;
        if (rotationDirection < 0)
        {
            wallKickIndex--;
        }

        //Aplicamos el Wrap tambi�n
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
        /* �sta funci�n es ejecutada cada vez que el timer del StepTime se cumpla, /
        /  lo que sucede es simple, cada vez que �sta funci�n se ejecute, la pieza /
        /  actual se mueve hacia abajo, y entonces se reinicia el timer del step  */
        this.stepTime = Time.time + this.stepDelay;
        Move(Vector2Int.down);

        //Checamos por el caso de un lock, para �sto el timer
        if (this.lockTime >= this.lockDelay)
        {
            Lock();
        }
    }
    private void Lock()
    {
        /* La pieza como tal ha llegado a su punto de bloqueo, ya no se va a mover /
        /  de �sta posici�n, lo que significa que �sta pieza se quedar� quieta,    /
        /  y que nos toca spawnear una nueva pieza                                */
        this.board.Set(this);
        this.board.ClearLines();
        this.board.SpawnPiece();
    }
}
