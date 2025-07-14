using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.AI;

public enum
Faction : int
{
    Allies,
    Enemy,
    Wild,
    None
}

public enum
PieceState : int
{
    Idling,
    Moving,
    Attacking,
    Total
}

//NOTE: eventually we want a sort of database to load and then have a separate
//character creation system and this only takes existing characters
public struct
PieceRequest
{
    public int ProfileID;
    public string Name;
    public Faction Alignment;
    public int Health;
    public int MovementSpeed;
    //NOTE: Add any necessary requested information here
}

public class
GamePiece : MonoBehaviour
{
    public string Name;
    public Faction Alignment;
    public int MaxHealth;
    public int Health;
    public int MovementSpeed;
    public bool Dead;
    public PieceState State;
    public Transform MyTransform;
    private const float PieceSpeed = 0.5f;
    private MeshRenderer Renderer;
    private NavMeshAgent Agent;
    private CapsuleCollider Collider;
    //TODO: Need to be able to revive an entity via some kind of entity manager
    //screen, maybe we should also be able to see their corpse or something on
    //the screen and just default them to some kind of death position/animation

    private void
    Awake()
    {
        Agent = this.gameObject.AddComponent<NavMeshAgent>();
        Collider = this.gameObject.GetComponent<CapsuleCollider>();
    }

    private void
    Start()
    {
        Dead = false;
        MyTransform = this.transform;
        Renderer = transform.GetChild(0).GetComponent<MeshRenderer>();
    }

    public void
    Update()
    {
        switch( State )
        {
            case PieceState.Idling:
                break;
            case PieceState.Moving:
                if( (Agent.destination - MyTransform.position).magnitude < 0.1f )
                {
                    State = PieceState.Idling;
                }
                break;
            case PieceState.Attacking:
                break;
        }

    }

    public void
    MoveTo( Vector3 Position )
    {
        if( (Position - MyTransform.position).magnitude <= MovementSpeed )
        {
            Agent.destination = Position;
        }
    }

    public void
    InitializePieceValues( PieceRequest PR )
    {
        Name = PR.Name;
        Alignment = PR.Alignment;
        MaxHealth = PR.Health;
        Health = MaxHealth;
        MovementSpeed = PR.MovementSpeed;
    }

    public void
    SetPieceValues( PieceRequest PR )
    {
        Name = PR.Name;
        Alignment = PR.Alignment;
        MaxHealth = PR.Health;
        MovementSpeed = PR.MovementSpeed;
        if( Health > MaxHealth )
        {
            Health = MaxHealth;
        }
    }

    public void
    OnObjectHover()
    {
        this.gameObject.layer = LayerMask.NameToLayer("Active");
        foreach( Transform child in MyTransform )
        {
            child.gameObject.layer = LayerMask.NameToLayer("Active");
        }
    }

    public void
    OnObjectTarget()
    {
        this.gameObject.layer = LayerMask.NameToLayer("Enemy");
        foreach( Transform child in MyTransform )
        {
            child.gameObject.layer = LayerMask.NameToLayer("Enemy");
        }
    }

    public void
    OnObjectExit()
    {
        switch( Alignment )
        {
            case Faction.Allies:
                gameObject.layer = LayerMask.NameToLayer("Friendly");
                foreach( Transform child in MyTransform )
                {
                    child.gameObject.layer = LayerMask.NameToLayer("Friendly");
                }
                break;
            case Faction.Enemy:
                gameObject.layer = LayerMask.NameToLayer("Enemy");
                foreach( Transform child in MyTransform )
                {
                    child.gameObject.layer = LayerMask.NameToLayer("Enemy");
                }
                break;
        }
    }

    public void
    OnObjectSelect()
    {
        this.gameObject.layer = LayerMask.NameToLayer("Active");
        foreach( Transform child in MyTransform )
        {
            child.gameObject.layer = LayerMask.NameToLayer("Active");
        }
    }

    public PieceRequest
    EditRequest()
    {
        PieceRequest Result = new PieceRequest();
        Result.Name = Name;
        Result.Alignment = Alignment;
        Result.Health = Health;
        Result.MovementSpeed = MovementSpeed;
        if( Health <= 0 )
        {
            Dead = true;
            Renderer.gameObject.SetActive( false );
        }
        return Result;
    }

    public void
    RequestDamage( ActionRequest Request )
    {
        Debug.Log( $"{Request.SourcePiece.Name} attacks {Request.DestinationPiece.Name} for {Request.Health:N} damage" );
        Health -= Request.Health;
        if( Health <= 0 )
        {
            Dead = true;
            //TODO: Deactivate Collider
            Renderer.gameObject.SetActive( false );
        }
    }

    public void
    RequestHealing( ActionRequest Request )
    {
        Debug.Log( $"{Request.SourcePiece.Name} heals {Request.DestinationPiece.Name} for {Request.Health:N} health" );
        Health +=Request.Health;
        if( Health > MaxHealth )
        {
            Health = MaxHealth;
        }
    }

    public override string
    ToString()
    {
        return $"Name: {Name}\nHealth: {Health:N}\nMovementSpeed: {MovementSpeed:N}";
    }
}

public class
InitiativePiece
{
    public int Initiative;
    public GamePiece Piece;
}

public struct
GamePieceList
{
    public int CurrentPiece;
    public int Size;
    public int MaxSize;
    private InitiativePiece[] List;

    public
    GamePieceList( int NumElements )
    {

        CurrentPiece = 0;
        Size = 0;
        MaxSize = NumElements;
        List = new InitiativePiece[ MaxSize ];
    }

    public void
    Sort()
    {
        //NOTE: Bubble Sort
        for( int i = 0; i < Size - 1; ++i )
        {
            bool Swapped = false;
            for( int j = 0; j < Size - i - 1; ++j )
            {
                if( List[j].Initiative < List[j+1].Initiative )
                {
                    InitiativePiece temp = List[j];
                    List[j] = List[j+1];
                    List[j+1] = temp;
                    Swapped = true;
                }
            }
            if( !Swapped ) break;
        }
    }

    public void
    Sort( Faction FirstPieces )
    {
        Sort();
        if( FirstPieces == Faction.None )
        {
            return;
        }


        //NOTE: Double Bubble Sort
        for( int i = 0; i < Size - 1; ++i )
        {
            bool Swapped = false;
            for( int j = 0; j < Size - i - 1; ++j )
            {
                if( List[j].Piece.Alignment != FirstPieces && List[j+1].Piece.Alignment == FirstPieces )
                {
                        InitiativePiece temp = List[j];
                        List[j] = List[j+1];
                        List[j+1] = temp;
                        Swapped = true;
                }
            }
            if( !Swapped ) break;
        }
    }

    public void
    Insert( InitiativePiece Piece )
    {
        List[Size] = Piece;
        Assert.IsTrue( (Size + 1) <= List.Length );
        Size++;
    }

    public void
    Remove( int Position )
    { //TODO: Implement Removal
    }

    public GamePiece
    GetCurrent()
    {
        Assert.IsTrue( Size != 0 );
        GamePiece gp = List[ CurrentPiece ].Piece;
        CurrentPiece = (CurrentPiece + 1) % Size;
        return gp;
    }

    public void
    PrintList()
    {
        for( int i = 0; i < Size; ++i )
        {
            InitiativePiece current = List[i];
            Debug.Log("Initiative: " + current.Initiative );
            Debug.Log("Name: " + current.Piece.Name );
        }
    }

    public void
    PrintFront()
    {
        Debug.Log("Initiative: " + List[0].Initiative );
        Debug.Log( List[0].Piece.ToString() );
    }

    public void
    PrintEnd()
    {
        Debug.Log("Initiative: " + List[Size - 1].Initiative );
        Debug.Log( List[Size - 1].Piece.ToString() );
    }

    public void
    Expand( int newSize )
    {
        MaxSize = newSize;
        InitiativePiece[] temp = List;
        List = new InitiativePiece[ MaxSize ];
        for( int i = 0; i < temp.Length; ++i )
        {
            List[i] = temp[i];
        }
    }
}
