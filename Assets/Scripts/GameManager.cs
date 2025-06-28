using UnityEngine;
using UnityEngine.Assertions;

public enum
CombatOrder : int
{
    Surprise,
    Setup,
    Initiative,
    StartFight,
    PlayerTurn,
    NPCTurn,
    NextRound
}

public enum
PlacementOrder : int
{
    Position,
    Profile,
    Creation
}

public enum
GamePieceTurnOrder : int
{
    MoveMode,
    AttackMode,
    None
}

public enum
EditorMode : int
{
    Play,
    Edit,
    Measure
}

public struct
PlacementMemory
{
    public ProfileDecision Decision;
    public PlacementOrder  State;
    public bool            StateTransition;
}

public struct
InitiativeMemory
{
    public InitiativeDecision Decision;
    public int CurrentGamePiece;
}

public class
GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private const string GamePiecePrefabPath   = "Prefabs/Game_Piece";
    private const string WildPiecePrefabPath   = "Prefabs/WildPiece";
    private const string EnemyPiecePrefabPath  = "Prefabs/EnemyPiece";
    private const string PlayerPiecePrefabPath = "Prefabs/PlayerPiece";

    private GameObject GamePiecePrefab;
    private GameObject WildPiecePrefab;
    private GameObject EnemyPiecePrefab;
    private GameObject PlayerPiecePrefab;

    [Header("Game Details")]
    [SerializeField]
    private LayerMask Terrain_LM;
    [SerializeField]
    private LayerMask GamePiece_LM;

    private GamePiece[] GamePieces;
    private GamePieceList InitiativeList;
    private Faction SurpriseFaction;
    private CombatOrder CurrentState;
    private GamePieceTurnOrder CurrentTurnState;

    private SurpriseDecision Decision;
    private InitiativeMemory Initiative;
    private PlacementMemory Placement;
    private EditDecision EditDecision;

    private int  CurrentTurn;
    private int  NumberOfPieces;
    private int  StartEditPieces;
    private bool StateSwitch;
    private bool Hovering;
    private GamePiece CurrentPiece;
    private GamePiece SelectedObject;
    private GamePiece HoverObject;
    private GamePiece TargetObject;
    private Vector3 MousePointOnFloor;
    private Vector3 MeasurePivotPoint;
    public  EditorMode EditorMode;

    private void
    Awake()
    {
        if( GameManager.Instance != null && GameManager.Instance != this )
        {
            Destroy( this );
            return;
        }
        GameManager.Instance = this;
        DontDestroyOnLoad( this );
        Initialize();
    }

    private bool
    Initialize()
    {
        bool Valid = true;
        GamePiecePrefab   = Resources.Load<GameObject>( GamePiecePrefabPath );
        if( GamePiecePrefab == null )
        {
            Debug.LogError("ERROR: FAILED TO LOAD PATH AT " + GamePiecePrefabPath );
            Valid = false;
        }

        WildPiecePrefab   = Resources.Load<GameObject>( WildPiecePrefabPath );
        if( WildPiecePrefab == null )
        {
            Debug.LogError("ERROR: FAILED TO LOAD PATH AT " + WildPiecePrefabPath );
            Valid = false;
        }

        EnemyPiecePrefab  = Resources.Load<GameObject>( EnemyPiecePrefabPath );
        if( EnemyPiecePrefab == null )
        {
            Debug.LogError("ERROR: FAILED TO LOAD PATH AT " + EnemyPiecePrefabPath );
            Valid = false;
        }

        PlayerPiecePrefab = Resources.Load<GameObject>( PlayerPiecePrefabPath );
        if( PlayerPiecePrefab == null )
        {
            Debug.LogError("ERROR: FAILED TO LOAD PATH AT " + PlayerPiecePrefabPath );
            Valid = false;
        }

        CurrentState = 0;
        NumberOfPieces = 0;
        CurrentTurn = 1;
        StateSwitch = true;
        Hovering = false;
        SelectedObject = null;
        HoverObject = null;
        EditDecision = null;
        MeasurePivotPoint = Vector3.zero;
        CurrentTurnState = GamePieceTurnOrder.None;
        EditorMode = EditorMode.Play;

        Placement  = new PlacementMemory();
        Placement.Decision = null;

        Initiative = new InitiativeMemory();
        Initiative.Decision = null;

        GamePieces = new GamePiece[64];

        return Valid;
    }

    private void
    Start()
    {
    }

    private void
    Update()
    {
        if( Input.GetKeyDown( KeyCode.LeftControl ) )
        {
            if( EditorMode != EditorMode.Edit )
            {
                UIManager.Manager.DisplayEditSign();
                DisableMeasureMode();
                StartEditPieces = NumberOfPieces;
                EditorMode = EditorMode.Edit;
                Debug.Log("Enable Edit Mode");
            }
            else
            {
                StateSwitch = true; // Retriggers the current state of the object
                EditorMode = EditorMode.Play;
                UIManager.Manager.HideEditSign();
                Debug.Log("Disable Edit Mode");
            }

        }

        if( Input.GetKeyDown( KeyCode.Backspace) && !UIManager.Manager.TextModeActive() )
        {
            UIManager.Manager.ShowMusicBrowser();
        }

        if( Input.GetKeyDown( KeyCode.M ) && !UIManager.Manager.TextModeActive() )
        {
            EditorMode = EditorMode.Measure;
            StateSwitch = true;
        }

        switch( EditorMode )
        {
            case EditorMode.Play:
                FightRun();
                break;
            case EditorMode.Edit:
                if( Input.GetKeyDown( KeyCode.C ) && SelectedObject != null )
                { //TODO: We Need to test and fix the many bugs that are inherent in going to edit mode while the UI is active
                    UIManager.Manager.ClearUI();

                    Debug.Log("Editing Character: \n" + SelectedObject.ToString() );
                    EditDecision = null;
                    EditDecision = ScriptableObject.CreateInstance<EditDecision>();
                    EditDecision.Request = SelectedObject.EditRequest();
                    EditDecision.Piece = SelectedObject;
                    EditDecision.Decided = false;
                    UIManager.Manager.DisplayEditCharacterScreen( EditDecision );
                }
                PlacementMode();
                if( StartEditPieces != NumberOfPieces )
                {
                    if( GetInitiative() )
                    {
                        StartEditPieces = NumberOfPieces;
                    }
                }
                break;
            case EditorMode.Measure:
                if( StateSwitch )
                {
                    Debug.Log("Measure Mode");
                    DrawHelper.Instance.SetLineColor( Color.green );
                    StateSwitch = false;
                }
                MeasureMode();
                break;

        }
    }

    private void
    FixedUpdate()
    {
        Vector3 ScreenCoords = Input.mousePosition;
        Ray WorldRay =  Camera.main.ScreenPointToRay( ScreenCoords );
        RaycastHit hit;
        if( Physics.Raycast( WorldRay.origin, WorldRay.direction, out hit, 100f,  GamePiece_LM ) )
        {
            if( !Hovering )
            {
                HoverObject = hit.transform.GetComponent<GamePiece>();
                HoverObject.OnObjectHover();
            }

            if( CurrentPiece != null && CurrentTurnState == GamePieceTurnOrder.MoveMode )
            {
                TargetObject = HoverObject;
                if( TargetObject != null && TargetObject != CurrentPiece )
                {
                    TargetObject.OnObjectTarget();
                    DrawHelper.Instance.SetLineColor( Color.red );
                }
            }

            if( Input.GetMouseButton(0) )
            {
                Debug.Log("Selected Object");
                if( SelectedObject != null ) SelectedObject.OnObjectExit();
                if( CurrentPiece != null ) CurrentPiece.OnObjectSelect();
                SelectedObject = hit.transform.GetComponent<GamePiece>();
                if( SelectedObject != null ) SelectedObject.OnObjectSelect();
            }

            Hovering = true;
        }
        else
        {
            if( Hovering && HoverObject != null )
            {
                HoverObject.OnObjectExit();
                if( CurrentPiece != null )   CurrentPiece.OnObjectSelect();//NOTE: In case that the Current Object was the hover object
                if( SelectedObject != null ) SelectedObject.OnObjectSelect();
            }

            if( Input.GetMouseButton(0) )
            {
                Debug.Log("Object Deselected");
                if( SelectedObject != null ) SelectedObject.OnObjectExit();
                SelectedObject = null;
                if( CurrentPiece != null ) CurrentPiece.OnObjectSelect();
            }

            TargetObject = null;
            HoverObject = null;
            Hovering = false;
            if( DrawHelper.Instance.DrawingLine() )
            {
                DrawHelper.Instance.SetLineColor( Color.blue );
            }
        }

        if( Physics.Raycast( WorldRay.origin, WorldRay.direction, out hit, 100f,  Terrain_LM ) )
        {
            MousePointOnFloor = hit.point;
        }

    }

    private bool
    PlacementMode()
    {
        if( Placement.Decision == null )
        {
            Placement.Decision = ScriptableObject.CreateInstance<ProfileDecision>();
            Placement.Decision.Piece = new PieceRequest();
        }

        switch( Placement.State )
        {
            case PlacementOrder.Position:
                if( Input.GetMouseButtonDown(1) )
                {
                    Vector3 ScreenCoords = Input.mousePosition;
                    Ray WorldRay =  Camera.main.ScreenPointToRay( ScreenCoords );
                    RaycastHit hit;
                    if( Physics.Raycast( WorldRay.origin, WorldRay.direction, out hit, 100f,  Terrain_LM ) )
                    {
                        Debug.Log("Contact Point: " + hit.point );
                        Placement.Decision.SpawnPoint = hit.point;
                        UIManager.Manager.CreateCreationRadial( Placement );
                    }
                }
                break;
            case PlacementOrder.Profile:
                if( Placement.StateTransition )
                {
                    UIManager.Manager.DisplayCharacterProfile( Placement );
                    Placement.StateTransition = false;
                }

                if( Placement.Decision.Decided )
                {
                    UIManager.Manager.ClearUI();
                    Placement.State++;
                    Placement.StateTransition = true;
                }
                break;
            case PlacementOrder.Creation:
                CreateNewGamePiece( Placement );
                Placement.State = 0;
                Placement.Decision = null;
                Placement.StateTransition = false;
                Debug.Log("New Piece Created!!!");
                break;
        }

        if( Input.GetKeyDown( KeyCode.Escape ) )
        {
            Debug.Log("Cancelled Current Placement");
            UIManager.Manager.ClearUI();
            Placement.State = 0;
            Placement.Decision = null;
            Placement.StateTransition = false;
        }

        if( Input.GetKeyDown( KeyCode.Return ) && Placement.State == PlacementOrder.Position )
        {
            Debug.Log("Setup Finished");
            UIManager.Manager.ClearUI();
            Placement.State = 0;
            Placement.Decision = null;
            Placement.StateTransition = false;
            return true;
        }

        return false;
    }

    private bool
    GetInitiative()
    {
        if( Initiative.Decision == null )
        {
            Initiative.Decision = ScriptableObject.CreateInstance<InitiativeDecision>();
            Initiative.Decision.InitiativePiece = new InitiativePiece();
            Initiative.Decision.InitiativePiece.Piece = GamePieces[ Initiative.CurrentGamePiece ];
            UIManager.Manager.DisplayInitiative( Initiative.Decision );
        }

        if( Initiative.Decision.Decided )
        {
            if( InitiativeList.MaxSize < NumberOfPieces )
            {
                InitiativeList.Expand( NumberOfPieces );
            }
            InitiativeList.Insert( Initiative.Decision.InitiativePiece );

            Debug.Log("Piece: " + Initiative.Decision.InitiativePiece.Piece.Name + "; Initiative: " + Initiative.Decision.InitiativePiece.Initiative );
            UIManager.Manager.ClearUI();

            Initiative.Decision = null;
            Initiative.CurrentGamePiece++;
            if( Initiative.CurrentGamePiece == NumberOfPieces )
            {
                InitiativeList.Sort( SurpriseFaction );
                return true;
            }
        }
        return false;
    }

    private void
    CreateNewGamePiece( PlacementMemory PlacementMemory )
   {
        GameObject go = null;

        switch( PlacementMemory.Decision.Piece.Alignment )
        {
            case Faction.Allies:
                go = Instantiate( PlayerPiecePrefab, PlacementMemory.Decision.SpawnPoint, Quaternion.identity );
                break;
            case Faction.Enemy:
                go = Instantiate( EnemyPiecePrefab, PlacementMemory.Decision.SpawnPoint, Quaternion.identity );
                break;
            case Faction.Wild:
                go = Instantiate( WildPiecePrefab, PlacementMemory.Decision.SpawnPoint, Quaternion.identity );
                break;
            default:
                Debug.Log("No Faction");
                break;
        }
        if( go == null ) Debug.LogError("ERROR: FAILED TO CREATE NEW GAME PIECE");
        GamePieces[NumberOfPieces] = go.AddComponent<GamePiece>();
        Assert.IsTrue( GamePieces[NumberOfPieces] != null );
        GamePieces[NumberOfPieces].InitializePieceValues( PlacementMemory.Decision.Piece );

        UIManager.Manager.CreatePieceLabel( GamePieces[NumberOfPieces] );
        NumberOfPieces++;
    }

    private void
    ChooseTurn()
    {
        if( CurrentPiece != null ) CurrentPiece.OnObjectExit();
        CurrentPiece = InitiativeList.GetCurrent();
        while( CurrentPiece.Dead )
        {
            CurrentPiece = InitiativeList.GetCurrent();
        }
        CurrentPiece.OnObjectSelect();

        Color BannerColor = Color.grey;
        switch( CurrentPiece.Alignment )
        {
            case Faction.Allies:
                CurrentState = CombatOrder.PlayerTurn;
                BannerColor = Color.blue;
                break;
            case Faction.Enemy:
                CurrentState = CombatOrder.NPCTurn;
                BannerColor = Color.red;
                break;
            case Faction.Wild:
                CurrentState = CombatOrder.NPCTurn;
                BannerColor = Color.yellow;
                break;
        }
        BannerColor.a = .3f;

        string BannerTitle = "Turn " + CurrentTurn + ": " +  CurrentPiece.Name;
        UIManager.Manager.DisplayBanner( BannerTitle, BannerColor );
    }

    private void
    GamePieceTurn()
    {
        if( Input.GetKeyUp( KeyCode.Space ) )
        {
            if( CurrentTurnState != GamePieceTurnOrder.MoveMode )
            {
                CurrentTurnState = GamePieceTurnOrder.MoveMode;
            }
            else
            {
                CurrentTurnState = GamePieceTurnOrder.None;
                DrawHelper.Instance.DisableLine();
            }
        }

        if( CurrentTurnState == GamePieceTurnOrder.MoveMode )
        {
            DrawHelper.Instance.DrawLineStartEnd( CurrentPiece.transform.position, MousePointOnFloor, CurrentPiece.MovementSpeed );
            if( Input.GetMouseButtonDown( 0 ) )
            {
                if( TargetObject != null )
                {
                    ActionRequest Request = new ActionRequest();
                    Request.SourcePiece     = CurrentPiece;
                    Request.DestinationPiece = TargetObject;
                    UIManager.Manager.CreateActionRadial( Request );
                }
                else
                {
                    CurrentPiece.MoveTo( MousePointOnFloor );
                }

                CurrentTurnState = GamePieceTurnOrder.None;
                DrawHelper.Instance.DisableLine();
            }

        }

    }

    private void
    FightRun()
    {
        switch( CurrentState )
        {
            case CombatOrder.Surprise:
                if(StateSwitch)
                {
                    Debug.Log("Order: Surprise");
                    StateSwitch = false;
                    Decision = ScriptableObject.CreateInstance<SurpriseDecision>();
                    UIManager.Manager.DisplaySurprise( Decision );
                }

                if( Decision.Decided )
                {

                    UIManager.Manager.ClearUI();
                    if( Decision.PlayerFirst )
                    {
                        UIManager.Manager.DisplayBanner( "Players Surprise the Enemies!" );
                        SurpriseFaction = Faction.Allies;
                    }

                    if( Decision.EnemyFirst )
                    {
                        UIManager.Manager.DisplayBanner( "Enemies Surprise the Players!" );
                        SurpriseFaction = Faction.Enemy;
                    }

                    if( Decision.WildFirst )
                    {
                        UIManager.Manager.DisplayBanner( "The Wild Surprise the Players and Enemies!" );
                        SurpriseFaction = Faction.Wild;
                    }

                    if( Decision.NoneFirst)
                    {
                        SurpriseFaction = Faction.None;
                    }

                    StateSwitch = true;
                    CurrentState++;
                }
                break;
            case CombatOrder.Setup:
                if(StateSwitch)
                {
                    Debug.Log("Order: Setup");
                    StateSwitch = false;
                }
                if( PlacementMode() )
                {
                    CurrentState++;
                    StateSwitch = true;
                }
                break;
            case CombatOrder.Initiative:
                if(StateSwitch)
                {
                    InitiativeList = new GamePieceList( NumberOfPieces );
                    Debug.Log("Order: Set Initiative");
                    StateSwitch = false;
                }
                if( GetInitiative() )
                {
                    CurrentState++;
                    StateSwitch = true;
                }
                break;

            case CombatOrder.StartFight:
                if(StateSwitch)
                {
                    Debug.Log("Order: Start Fight");
                    StateSwitch = false;
                    ChooseTurn();
                }

                break;

            case CombatOrder.PlayerTurn:
                if(StateSwitch)
                {
                    Debug.Log("Order: Player Turn");
                    StateSwitch = false;
                }

                GamePieceTurn();
                if( Input.GetKeyDown( KeyCode.Return ) )
                {
                    UIManager.Manager.ClearUI();
                    StateSwitch = true;
                    if( InitiativeList.CurrentPiece == 0)
                    {
                        CurrentState = CombatOrder.NextRound;
                    }
                    else
                    {
                        ChooseTurn();
                    }
                }
                break;

            case CombatOrder.NPCTurn:
                if(StateSwitch)
                {
                    Debug.Log("Order: NPC Turn");
                    StateSwitch = false;
                }

                GamePieceTurn();
                if( Input.GetKeyDown( KeyCode.Return ) )
                {
                    UIManager.Manager.ClearUI();
                    StateSwitch = true;
                    if( InitiativeList.CurrentPiece == 0)
                    {
                        CurrentState = CombatOrder.NextRound;
                    }
                    else
                    {
                        ChooseTurn();
                    }
                }
                break;

            case CombatOrder.NextRound:
                if(StateSwitch)
                {
                    Debug.Log("Order: Next Round");
                    StateSwitch = false;
                    InitiativeList.Sort();
                    CurrentTurn++;
                    ChooseTurn();
                    SelectedObject = null;
                }
                break;
        }
    }

    private void
    MeasureMode()
    {
        if( Input.GetKeyDown( KeyCode.Escape ) )
        {
            if( MeasurePivotPoint != Vector3.zero )
            {
                MeasurePivotPoint = Vector3.zero;
            }
            else
            {
                DisableMeasureMode();
                EditorMode = EditorMode.Play;
                StateSwitch = true;
            }
        }

        if( Input.GetMouseButtonDown( 0 ) )
        {
            MeasurePivotPoint = MousePointOnFloor;
        }

        if( MeasurePivotPoint != Vector3.zero )
        {
            DrawHelper.Instance.Measure( MeasurePivotPoint, MousePointOnFloor );
        }
        else
        {
            DrawHelper.Instance.DisableLine();
        }
    }

    private void
    DisableMeasureMode()
    {
        MeasurePivotPoint = Vector3.zero;
        DrawHelper.Instance.SetLineColor( Color.blue );
        DrawHelper.Instance.DisableLine();
    }

    public void
    AdvancePlacementState()
    {
        Placement.State++;
        Placement.StateTransition = true;
    }
}

