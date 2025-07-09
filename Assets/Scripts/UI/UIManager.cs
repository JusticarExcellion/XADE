using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;

public class
SurpriseDecision : ScriptableObject //NOTE: This Sucks
{
    //TODO: Consider changing this to an enum
    public bool EnemyFirst;
    public bool PlayerFirst;
    public bool WildFirst;
    public bool NoneFirst;

    public bool Decided;
}

public class
InitiativeDecision : ScriptableObject
{
    public InitiativePiece InitiativePiece;
    public bool Decided;
}

public class
ProfileDecision : ScriptableObject
{
    public Vector3 SpawnPoint;
    public PieceRequest Piece;
    public bool Decided;
}

public class
EditDecision : ScriptableObject
{
    public PieceRequest Request;
    public GamePiece Piece;
    public bool Decided;
}

public struct
PieceLabel
{
    public NameLabel Script;
    public GamePiece Piece;
    public GameObject Label;
}

public class
ActionRequest //TODO: Consider changing this so we can incorporate different things like healing, stat boosts, Damage, etc
{
    public GamePiece SourcePiece;
    public GamePiece DestinationPiece;
    public int Health;
    public bool Decided;
}

public class UIManager : MonoBehaviour
{
    public static UIManager Manager;
    private const string SurpriseDocPath           = "UI/Surprise_Interface";
    private const string CharacterProfilePath      = "UI/Profile";
    private const string InitiativeDocPath         = "UI/Initiative";
    private const string BannerDocPath             = "UI/Banner";
    private const string PieceLabelPath            = "UI/PieceLabel";
    private const string DistanceCheckPath         = "UI/DistanceLabel";
    private const string EditProfilePath           = "UI/EditCharacter";
    private const string ModeLabelPath             = "UI/ModeLabel";
    private const string DamageRequestPath         = "UI/DamageRequest";
    private const string HealRequestDocumentPath   = "UI/HealRequest";
    private const string MusicBrowserDocumentPath  = "UI/MusicBrowser";
    private const string RadialPrefabPath          = "UI/Radial";
    private const string GamePieceIconPath         = "Sprites/PieceIcon";
    private const string ObstacleIconPath          = "Sprites/ObstaclesIcon";
    private const string AttackIconPath            = "Sprites/AttackIcon";
    private const string HealingIconPath           = "Sprites/HealingIcon";
    private const string PlayButtonIconPath        = "Sprites/PlayIcon";
    private const string PauseButtonIconPath       = "Sprites/PauseIcon";

    private VisualTreeAsset SurpriseDocument;
    private VisualTreeAsset CharacterProfileDocument;
    private VisualTreeAsset InitiativeDocument;
    private VisualTreeAsset EditCharacterDocument;
    private VisualTreeAsset DamageRequestDocument;
    private VisualTreeAsset HealRequestDocument;
    private VisualTreeAsset MusicBrowserDocument;

    private GameObject BannerPrefab;
    private GameObject PieceLabelPrefab;
    private GameObject DistanceCheckPrefab;
    private GameObject LabelModePrefab;
    private GameObject RadialPrefab;

    private Sprite GamePieceIcon;
    private Sprite ObstacleIcon;
    private Sprite AttackIcon;
    private Sprite HealingIcon;
    private Sprite PlayIcon;
    private Sprite PauseIcon;


    [Header("UI Roots")]
    [SerializeField]
    private UIDocument SourceAsset;
    [SerializeField]
    private GameObject Canvas;

    [Header("Music Browser")]
    [SerializeField]
    private VisualTreeAsset MusicListItem;

    private RectTransform CanvasTransform;
    private PieceLabel[] PieceLabels;
    private int NumberOfLabels;
    private GameObject DistanceCheck;
    private Label_Distance DistanceLabel;
    private ModeLabel ModeLabel;
    private GameObject RadialWheel;


    private void
    Awake()
    {
        if( UIManager.Manager != null && UIManager.Manager != this )
        {
            Destroy( this );
            return;
        }
        UIManager.Manager = this;
        DontDestroyOnLoad( this );
        if( !Initialize() ) Debug.LogError("ERROR: UI INITIALIZATION FAILED");
    }

    private void
    Update()
    {
        UpdateLabels();
    }

    private bool
    Initialize()
    {
        bool Valid = true;
        RadialWheel = null;
        SurpriseDocument = Resources.Load<VisualTreeAsset>( SurpriseDocPath );
        if( !SurpriseDocument )
        {
            Debug.LogError("ERROR: SURPRISE DOCUMENT NOT FOUND!!!");
            Valid = false;
        }

        BannerPrefab = Resources.Load<GameObject>( BannerDocPath );
        if( !BannerPrefab )
        {
            Debug.LogError("ERROR: BANNER PREFAB NOT FOUND!!!");
            Valid = false;
        }

        PieceLabelPrefab = Resources.Load<GameObject>( PieceLabelPath );
        if( !PieceLabelPrefab )
        {
            Debug.LogError("ERROR: PIECE LABEL PREFAB NOT FOUND!!!");
            Valid = false;
        }

        CharacterProfileDocument = Resources.Load<VisualTreeAsset>( CharacterProfilePath );
        if( !CharacterProfileDocument )
        {
            Debug.LogError("ERROR: CHARACTER PROFILE DOCUMENT NOT FOUND!!!");
            Valid = false;
        }

        InitiativeDocument = Resources.Load<VisualTreeAsset>( InitiativeDocPath );
        if( !InitiativeDocument )
        {
            Debug.LogError("ERROR: INITIATIVE DOCUMENT NOT FONUD!!!");
            Valid = false;
        }

        CanvasTransform = Canvas.GetComponent<RectTransform>();
        if( !CanvasTransform )
        {
            Debug.LogError("ERROR: CANVAS TRANSFORM NOT FOUND!!!");
            Valid = false;
        }

        DistanceCheckPrefab = Resources.Load<GameObject>( DistanceCheckPath );
        if( !DistanceCheckPrefab )
        {
            Debug.LogError("ERROR: FAILED TO LOAD DISTANCE LABEL PREFAB!!!");
            Valid = false;
        }

        DistanceCheck = Instantiate( DistanceCheckPrefab, CanvasTransform );
        if( !DistanceCheck )
        {
            Debug.LogError("ERROR: FAILED TO CREATE DISTANCE CHECK!!!");
            Valid = false;
        }
        DistanceCheck.SetActive( false );
        DistanceLabel = DistanceCheck.GetComponent<Label_Distance>();
        if( !DistanceLabel )
        {
            Debug.LogError("ERROR: FAILED TO FIND DISTANCE CHECK LABEL!!!");
            Valid = false;
        }

        EditCharacterDocument = Resources.Load<VisualTreeAsset>( EditProfilePath );
        if( !EditCharacterDocument )
        {
            Debug.LogError("ERROR: EDIT DOCUMENT NOT FONUD!!!");
            Valid = false;
        }

        LabelModePrefab  = Resources.Load<GameObject>( ModeLabelPath );
        if( !LabelModePrefab )
        {
            Debug.LogError("ERROR: MODE LABEL PREFFAB NOT FOUND!!!");
            Valid = false;
        }

        GameObject go = Instantiate( LabelModePrefab, CanvasTransform );
        ModeLabel = go.GetComponent<ModeLabel>();
        if( ModeLabel == null )
        {
            Debug.LogError("ERROR: MODE LABEL OBJECT NOT FOUND");
            Valid = false;
        }
        HideEditSign();

        DamageRequestDocument = Resources.Load<VisualTreeAsset>( DamageRequestPath );
        if( !DamageRequestDocument )
        {
            Debug.LogError("ERROR: DAMAGE REQUEST DOCUMENT NOT FOUND!!!");
            Valid = false;
        }

        HealRequestDocument = Resources.Load<VisualTreeAsset>( HealRequestDocumentPath );
        if( !HealRequestDocument )
        {
            Debug.LogError("ERROR: HEAL REQUEST DOCUMENT NOT FOUND!!!");
            Valid = false;
        }

        RadialPrefab = Resources.Load<GameObject>( RadialPrefabPath );
        if( !RadialPrefab )
        {
            Debug.LogError("ERROR: RADIAL PREFAB NOT FOUND!!!");
            Valid = false;
        }

        MusicBrowserDocument = Resources.Load<VisualTreeAsset>( MusicBrowserDocumentPath );
        if( !MusicBrowserDocument )
        {
            Debug.LogError("ERROR: MUSIC BROWSER NOT FOUND!!!");
            Valid = false;
        }

        PlayIcon  = Resources.Load<Sprite>( PlayButtonIconPath  );
        if( !PlayIcon )
        {
            Debug.LogError("ERROR: PLAY ICON NOT FOUND!!!");
            Valid = false;
        }

        PauseIcon = Resources.Load<Sprite>( PauseButtonIconPath );
        if( !PauseIcon )
        {
            Debug.LogError("ERROR: PAUSE ICON NOT FOUND!!!");
            Valid = false;
        }

        PieceLabels = new PieceLabel[64];
        NumberOfLabels = 0;

        return Valid;
    }

    public void
    DisplaySurprise( SurpriseDecision Decision )
    {
        SourceAsset.visualTreeAsset = SurpriseDocument;
        Button EnemyButton = UQueryExtensions.Q<Button>( SourceAsset.rootVisualElement,"EnemyButton");
        Button PlayerButton = UQueryExtensions.Q<Button>( SourceAsset.rootVisualElement,"PlayerButton");
        Button WildButton = UQueryExtensions.Q<Button>( SourceAsset.rootVisualElement,"WildButton");
        Button NoneButton = UQueryExtensions.Q<Button>( SourceAsset.rootVisualElement,"NoneButton");
        if( EnemyButton == null )
        {
            Debug.LogError("ERROR: NO ENEMY BUTTON FOUND!!!");
            return;
        }
        if( PlayerButton == null )
        {
            Debug.LogError("ERROR: NO PLAYER BUTTON FOUND!!!");
            return;
        }
        if( WildButton == null )
        {
            Debug.LogError("ERROR: NO WILD BUTTON FOUND!!!");
            return;
        }
        if( NoneButton == null )
        {
            Debug.LogError("ERROR: NO NONE BUTTON FOUND!!!");
            return;
        }

        //Lambda Functions because this shit sucks
        EnemyButton.clicked += () => { Decision.EnemyFirst = true; Decision.Decided = true; };
        PlayerButton.clicked += () => { Decision.PlayerFirst = true; Decision.Decided = true; };
        WildButton.clicked += () => { Decision.WildFirst = true; Decision.Decided = true; };
        NoneButton.clicked += () => { Decision.NoneFirst = true; Decision.Decided = true; };
    }

    public void
    DisplayBanner( string Text )
    {
        GameObject go = Instantiate( BannerPrefab, CanvasTransform );
        Banner BannerClass = go.GetComponent<Banner>();
        StartCoroutine( BannerClass.DisplayBannerText( Text ) );
    }

    public void
    DisplayBanner( string Text, Color BannerColor )
    {
        GameObject go = Instantiate( BannerPrefab, CanvasTransform );
        Banner BannerClass = go.GetComponent<Banner>();
        BannerClass.SetBannerColor( BannerColor );
        StartCoroutine( BannerClass.DisplayBannerText( Text ) );
    }

    public void
    DisplayCharacterProfile( PlacementMemory PlacementMemory )
    {
        //TODO: Add profile browser functionality, load all profiles,
        //display them, etc.
        SourceAsset.visualTreeAsset = CharacterProfileDocument;
        Button Submit = UQueryExtensions.Q<Button>( SourceAsset.rootVisualElement,"Submit");
        if( Submit == null )
        {
            Debug.LogError("ERROR: NO SUBMIT BUTTON FOUND!!!");
            return;
        }

        Button GenerateProfile = UQueryExtensions.Q<Button>( SourceAsset.rootVisualElement,"GenerateProfile");
        if( GenerateProfile == null )
        {
            Debug.LogError("ERROR: NO GENERATE PROFILE BUTTON FOUND!!!");
            return;
        }

        RadioButtonGroup RBG = UQueryExtensions.Q<RadioButtonGroup>( SourceAsset.rootVisualElement,"Faction");
        if( RBG == null )
        {
            Debug.LogError("ERROR: NO FACTION RADIO BUTTON GROUP FOUND!!!");
            return;
        }
        RBG.RegisterCallback<ChangeEvent<int>>( ( evt ) =>
            {
                    switch( evt.newValue )
                    {
                        case 0:
                            PlacementMemory.Decision.Piece.Alignment = Faction.Allies;
                        break;
                        case 1:
                            PlacementMemory.Decision.Piece.Alignment = Faction.Enemy;
                        break;
                        case 2:
                            PlacementMemory.Decision.Piece.Alignment = Faction.Allies;
                        break;
                        case 3:
                            PlacementMemory.Decision.Piece.Alignment = Faction.Wild;
                        break;
                        default:
                            Debug.LogError("ERROR: NEW FACTION VALUE IS INVALID!!!");
                            PlacementMemory.Decision.Piece.Alignment = Faction.None;
                        break;
                    }
            }
        );
        TextField NameField = UQueryExtensions.Q<TextField>( SourceAsset.rootVisualElement,"Name");
        if( NameField == null  )
        {
            Debug.LogError("ERROR: NO NAME FIELD FOUND!!!");
            return;
        }

        NameField.RegisterCallback<ChangeEvent<string>>( ( evt ) =>
            {
                PlacementMemory.Decision.Piece.Name = evt.newValue;
            }
        );
        IntegerField HealthField = UQueryExtensions.Q<IntegerField>( SourceAsset.rootVisualElement,"Health");
        if( HealthField == null  )
        {
            Debug.LogError("ERROR: NO HEALTH FIELD FOUND!!!");
            return;
        }
        HealthField.RegisterCallback<ChangeEvent<int>>( ( evt ) =>
            {
                PlacementMemory.Decision.Piece.Health = evt.newValue;
            }
        );

        IntegerField MovementField = UQueryExtensions.Q<IntegerField>( SourceAsset.rootVisualElement,"Movement");
        if( MovementField == null  )
        {
            Debug.LogError("ERROR: NO MOVEMENT FIELD FOUND!!!");
            return;
        }
        MovementField.RegisterCallback<ChangeEvent<int>>( ( evt ) => 
            {
                PlacementMemory.Decision.Piece.MovementSpeed = evt.newValue;
            }
        );


        Submit.clicked += () => { PlacementMemory.Decision.Decided = true; };
        GenerateProfile.clicked += () => { DataManager.Instance.SaveProfile( in PlacementMemory.Decision.Piece );  };

        List<PieceRequest> InMemoryProfiles  = DataManager.Instance.LoadAllProfiles();
        //TODO: Set the Data Source for the Profile Browser
    }

    public void
    DisplayInitiative( InitiativeDecision Decision )
    {
        SourceAsset.visualTreeAsset = InitiativeDocument;
        Label CharacterLabel = UQueryExtensions.Q<Label>( SourceAsset.rootVisualElement,"Character" );
        Button AcceptButton = UQueryExtensions.Q<Button>( SourceAsset.rootVisualElement,"AcceptButton" );
        IntegerField Field = UQueryExtensions.Q<IntegerField>( SourceAsset.rootVisualElement,"IntegerField");

        if( CharacterLabel == null )
        {
            Debug.LogError("ERROR: NO CHARACTER LABEL FOUND!!!");
            return;
        }

        if( AcceptButton == null )
        {
            Debug.LogError("ERROR: NO ACCEPT BUTTON FOUND!!!");
            return;
        }

        if( Field == null )
        {
            Debug.LogError("ERROR: NO INTEGER FIELD FOUND!!!");
            return;
        }

        CharacterLabel.text = Decision.InitiativePiece.Piece.Name + "'s Initiative";
        Field.RegisterCallback<ChangeEvent<int>>( ( evt ) =>
        {
                Decision.InitiativePiece.Initiative = evt.newValue;
            }
        );


        AcceptButton.clicked += () => {  Decision.Decided = true;};
    }

    public void
    ClearUI()
    {
        SourceAsset.visualTreeAsset = null;
    }

    public void
    CreatePieceLabel( GamePiece GP )
    {
        GameObject go = Instantiate( PieceLabelPrefab, CanvasTransform );
        NameLabel nl = go.GetComponent<NameLabel>();
        PieceLabels[NumberOfLabels].Label = go;
        PieceLabels[NumberOfLabels].Piece = GP;
        PieceLabels[NumberOfLabels].Script = nl;
        Color pieceAlignment = Color.white;
        switch( GP.Alignment )
        {
            case Faction.Allies:
                pieceAlignment = Color.blue;
                break;
            case Faction.Enemy:
                pieceAlignment = Color.red;
                break;
            case Faction.Wild:
                pieceAlignment = Color.yellow;
                break;
        }
        pieceAlignment.a = .6f;
        nl.SetBackground( pieceAlignment );
        nl.SetText( GP.Name );
        NumberOfLabels++;
    }

    private void
    UpdateLabels()
    {
        GameObject Label;
        GameObject Piece;
        for( int i = 0; i < NumberOfLabels; i++ )
        {
            Label = PieceLabels[i].Label;
            Piece = PieceLabels[i].Piece.gameObject;

            RectTransform LabelTransform = Label.GetComponent<RectTransform>();
            if(LabelTransform == null)
            {
                Debug.LogError("ERROR: NO LABEL TRANSFORM FOUND");
                Debug.Break();
            }

            Vector3 PiecePosition = Piece.transform.position;
            PiecePosition.y += 0.5f;
            Vector3 ViewportPoint = Camera.main.WorldToViewportPoint( PiecePosition );
            Vector3 CanvasPoint = new Vector3(
                    ( ( ViewportPoint.x * CanvasTransform.sizeDelta.x ) - (CanvasTransform.sizeDelta.x * 0.5f ) ),
                    ( ( ViewportPoint.y * CanvasTransform.sizeDelta.y ) - (CanvasTransform.sizeDelta.y * 0.5f ) ),
                    0
                    );

            LabelTransform.anchoredPosition = CanvasPoint;
        }
    }

    public void
    DisplayLineDistance( Vector3 LineMidpoint, float Distance )
    {
        if( !DistanceCheck.activeSelf ) DistanceCheck.SetActive( true );
        string LabelText = $"Distance: {Distance:F2}m";

        RectTransform LabelTransform = DistanceCheck.GetComponent<RectTransform>();
        if(LabelTransform == null)
        {
            Debug.LogError("ERROR: NO LABEL TRANSFORM FOUND");
            Debug.Break();
        }

        Vector3 ViewportPoint = Camera.main.WorldToViewportPoint( LineMidpoint );
        Vector3 CanvasPoint = new Vector3(
                ( ( ViewportPoint.x * CanvasTransform.sizeDelta.x ) - (CanvasTransform.sizeDelta.x * 0.5f ) ),
                ( ( ViewportPoint.y * CanvasTransform.sizeDelta.y ) - (CanvasTransform.sizeDelta.y * 0.5f ) ),
                0
                );

        LabelTransform.anchoredPosition = CanvasPoint;
        DistanceLabel.SetLabelText( LabelText );
    }

    public void
    DisableLineDistance()
    {
        DistanceCheck.SetActive( false );
    }

    public void
    DisplayEditCharacterScreen( EditDecision Decision )
    {
        SourceAsset.visualTreeAsset = EditCharacterDocument;

        TextField NameField = UQueryExtensions.Q<TextField>( SourceAsset.rootVisualElement,"Name");
        if( NameField == null )
        {
            Debug.LogError("ERROR: NO NAME FIELD FOUND!!!");
            return;
        }

        NameField.value = Decision.Piece.Name;
        NameField.RegisterCallback<ChangeEvent<string>>( ( evt ) =>
            {
                Decision.Request.Name = evt.newValue;
            }
        );

        IntegerField HealthField = UQueryExtensions.Q<IntegerField>( SourceAsset.rootVisualElement,"Health");
        if( HealthField == null  )
        {
            Debug.LogError("ERROR: NO HEALTH FIELD FOUND!!!");
            return;
        }

        HealthField.value = Decision.Piece.Health;
        HealthField.RegisterCallback<ChangeEvent<int>>( ( evt ) =>
            {
                Decision.Request.Health = evt.newValue;
            }
        );

        IntegerField MovementField = UQueryExtensions.Q<IntegerField>( SourceAsset.rootVisualElement,"Movement");
        if( MovementField == null  )
        {
            Debug.LogError("ERROR: NO MOVEMENT FIELD FOUND!!!");
            return;
        }

        MovementField.value = Decision.Piece.MovementSpeed;
        MovementField.RegisterCallback<ChangeEvent<int>>( ( evt ) =>
            {
                Decision.Request.MovementSpeed = evt.newValue;
            }
        );

        Button Submit = UQueryExtensions.Q<Button>( SourceAsset.rootVisualElement,"Submit");
        if( Submit == null )
        {
            Debug.LogError("ERROR: NO SUBMIT BUTTON FOUND!!!");
            return;
        }

        Submit.clicked += () =>
        {
            Decision.Decided = true;
            ClearUI();
            Decision.Piece.SetPieceValues( Decision.Request );
            for( int i = 0; i < NumberOfLabels; i++ )
            { //Reolading all name labels
                GamePiece Piece = PieceLabels[i].Piece;
                PieceLabels[i].Script.SetText( Piece.Name );
            }
            Decision.Piece.SetPieceValues( Decision.Request );
        };

    }

    public void
    DisplayEditSign()
    {
        ModeLabel.gameObject.SetActive( true );
        ModeLabel.Label.text = "Edit";
    }

    public void
    HideEditSign()
    {
        ModeLabel.gameObject.SetActive( false );
    }

    public void
    DisplayDamageRequest( ActionRequest Decision )
    {
        SourceAsset.visualTreeAsset = DamageRequestDocument;

        IntegerField DamageField = UQueryExtensions.Q<IntegerField>( SourceAsset.rootVisualElement,"Damage");
        if( DamageField == null  )
        {
            Debug.LogError("ERROR: NO DAMAGE FIELD FOUND!!!");
            return;
        }
        DamageField.value = 0;
        DamageField.RegisterCallback<ChangeEvent<int>>( ( evt ) =>
            {
                Decision.Health = evt.newValue;
            }
        );


        Button Submit = UQueryExtensions.Q<Button>( SourceAsset.rootVisualElement,"Submit");
        if( Submit == null )
        {
            Debug.LogError("ERROR: NO SUBMIT BUTTON FOUND!!!");
            return;
        }

        Submit.clicked += () =>
        {
            Decision.DestinationPiece.RequestDamage( Decision );
            ClearUI();
            ReloadNameLabels();
        };

    }

    public void
    DisplayHealRequest( ActionRequest Decision )
    {
        SourceAsset.visualTreeAsset = HealRequestDocument;

        IntegerField DamageField = UQueryExtensions.Q<IntegerField>( SourceAsset.rootVisualElement,"Healing");
        if( DamageField == null  )
        {
            Debug.LogError("ERROR: NO HEALING FIELD FOUND!!!");
            return;
        }
        DamageField.value = 0;
        DamageField.RegisterCallback<ChangeEvent<int>>( ( evt ) =>
            {
                Decision.Health = evt.newValue;
            }
        );


        Button Submit = UQueryExtensions.Q<Button>( SourceAsset.rootVisualElement,"Submit");
        if( Submit == null )
        {
            Debug.LogError("ERROR: NO SUBMIT BUTTON FOUND!!!");
            return;
        }

        Submit.clicked += () =>
        {
            Decision.DestinationPiece.RequestHealing( Decision );
            ClearUI();
            ReloadNameLabels();
        };

    }


    private void
    ReloadNameLabels()
    {
        for(int i = 0; i < NumberOfLabels; ++i)
        {
            if(PieceLabels[i].Piece.Dead )
            {
                PieceLabels[i].Script.gameObject.SetActive( false );
            }
            else
            {
                PieceLabels[i].Script.gameObject.SetActive( true );
            }
        }
    }

    public void
    CreateCreationRadial( PlacementMemory Memory )
    {
        if( RadialWheel != null )
        {
            Destroy( RadialWheel );
            RadialWheel = null;
        }

        RadialWheel= Instantiate( RadialPrefab, CanvasTransform );
        if( !RadialWheel )
        {
            Debug.LogError("ERROR: FAILED TO CREATE CREATION RADIAL!!!");
        }
        RadialMenu Menu = RadialWheel.GetComponent<RadialMenu>();

        if( GamePieceIcon == null )
        {
            GamePieceIcon = Resources.Load<Sprite>( GamePieceIconPath );
        }

        if(ObstacleIcon == null)
        {
            ObstacleIcon = Resources.Load<Sprite>( ObstacleIconPath );
        }

        Menu.SetRadialButtonIcon( 0, GamePieceIcon );
        Menu.SetRadialButtonIcon( 1, ObstacleIcon  );
        Menu.SetRadialButtonName( 0, "Create New Piece" );
        Menu.SetRadialButtonName( 1, "Create New Obstacle" );

        Menu.CanvasTransform = CanvasTransform;
        Menu.PivotPoint = Memory.Decision.SpawnPoint;

        Menu.Buttons[0].Button.onClick.AddListener( delegate {
                Debug.Log("Create Game Piece");
                GameManager.Instance.AdvancePlacementState();
                RadialWheel = null;
                Menu.ClearRadial();
                });

        Menu.Buttons[1].Button.onClick.AddListener( delegate {
                Debug.Log("Create Obstacle");

                GameManager.Instance.AdvancePlacementState();
                RadialWheel = null;
                Menu.ClearRadial();
                });
    }

    public void
    CreateActionRadial( ActionRequest Decision )
    {
        if( RadialWheel != null )
        {
            Destroy( RadialWheel );
            RadialWheel = null;
        }

        RadialWheel= Instantiate( RadialPrefab, CanvasTransform );
        if( !RadialWheel )
        {
            Debug.LogError("ERROR: FAILED TO CREATE CREATION RADIAL!!!");
        }
        RadialMenu Menu = RadialWheel.GetComponent<RadialMenu>();

        if( AttackIcon == null )
        {
            AttackIcon = Resources.Load<Sprite>( AttackIconPath );
        }

        if( HealingIcon == null)
        {
            HealingIcon = Resources.Load<Sprite>( HealingIconPath );
        }

        Menu.SetRadialButtonIcon( 0, AttackIcon    );
        Menu.SetRadialButtonIcon( 1, HealingIcon   );
        Menu.SetRadialButtonName( 0, "Attack Game Piece" );
        Menu.SetRadialButtonName( 1, "Heal GamePiece" );

        Menu.CanvasTransform = CanvasTransform;
        Menu.PivotPoint = Decision.DestinationPiece.transform.position;

        Menu.Buttons[0].Button.onClick.AddListener( delegate {
                Debug.Log("Attack Piece");
                DisplayDamageRequest( Decision );
                RadialWheel = null;
                Menu.ClearRadial();
                });

        Menu.Buttons[1].Button.onClick.AddListener( delegate {
                Debug.Log("Heal Piece");
                DisplayHealRequest( Decision );
                RadialWheel = null;
                Menu.ClearRadial();
                });
    }

    public void
    ShowMusicBrowser()
    {
        SourceAsset.visualTreeAsset = MusicBrowserDocument;

        Background PlayButtonIcon = new Background();
        PlayButtonIcon.sprite = PlayIcon;

        /****************Grabbing Lists and Tabs****************/
        ListView MusicList = UQueryExtensions.Q<ListView>( SourceAsset.rootVisualElement,"MusicList");
        ListView AmbienceList = UQueryExtensions.Q<ListView>( SourceAsset.rootVisualElement,"AmbienceList");
        TabView BrowserTabView = UQueryExtensions.Q<TabView>( SourceAsset.rootVisualElement,"AudioTabs");

        /****************Setting up lists and tabs****************/
        List<AudioFileDetails> First5_Music = AudioManager.Instance.MusicFiles.GrabSet( 0 );

        Func<VisualElement> MB_MakeItem = () => MusicListItem.Instantiate();
        Action<VisualElement, int> MB_MusicBindItem = ( e, i ) => {
            e.dataSource = First5_Music[i];
            Debug.Log($"Data Sourced: {AudioManager.Instance.MusicFiles.Files[i].Name}");
        };

        MusicList.selectedIndicesChanged += ( newIndex ) =>
        {
            Debug.Log($"New Item Selected: {AudioManager.Instance.MusicFiles.Files[MusicList.selectedIndex].Name}");
        };

        MusicList.makeItem = MB_MakeItem;
        MusicList.bindItem = MB_MusicBindItem;
        MusicList.itemsSource = First5_Music;
        MusicList.ClearSelection();

        List<AudioFileDetails> First5_Ambience = AudioManager.Instance.AmbienceFiles.GrabSet( 0 );

        Action<VisualElement, int> MB_AmbienceBindItem = ( e, i ) => {
            e.dataSource = First5_Ambience[i];
            Debug.Log($"Data Sourced: {AudioManager.Instance.AmbienceFiles.Files[i].Name}");
        };

        AmbienceList.selectedIndicesChanged += ( newIndex ) =>
        {
            Debug.Log($"New Item Selected: {AudioManager.Instance.AmbienceFiles.Files[AmbienceList.selectedIndex].Name}");
        };

        AmbienceList.makeItem = MB_MakeItem;
        AmbienceList.bindItem = MB_AmbienceBindItem;
        AmbienceList.itemsSource = First5_Ambience;
        AmbienceList.ClearSelection();


        /****************Getting and Setting up buttons****************/
        Button NextButton = UQueryExtensions.Q<Button>( SourceAsset.rootVisualElement,"Next");
        Button PrevButton = UQueryExtensions.Q<Button>( SourceAsset.rootVisualElement,"Previous");
        Button PlayButton = UQueryExtensions.Q<Button>( SourceAsset.rootVisualElement,"Play");
        Button PlayPrevButton = UQueryExtensions.Q<Button>( SourceAsset.rootVisualElement,"PlayPrev");
        Button PlayNextButton = UQueryExtensions.Q<Button>( SourceAsset.rootVisualElement,"PlayNext");
        Button ExitButton = UQueryExtensions.Q<Button>( SourceAsset.rootVisualElement,"Exit");

        if( NextButton == null )
        {
            Debug.LogError("ERROR: NEXT BUTTON NOT FOUND ON MUSIC BROWSER!!!");
        }

        if( PrevButton == null )
        {
            Debug.LogError("ERROR: PREVIOUS BUTTON NOT FOUND ON MUSIC BROWSER!!!");
        }

        if( PlayButton == null )
        {
            Debug.LogError("ERROR: PREVIOUS BUTTON NOT FOUND ON MUSIC BROWSER!!!");
        }

        if( PlayPrevButton == null )
        {
            Debug.LogError("ERROR: PLAY PREVIOUS BUTTON NOT FOUND ON MUSIC BROWSER!!!");
        }

        if( PlayNextButton == null )
        {
            Debug.LogError("ERROR: PLAY NEXT BUTTON NOT FOUND ON MUSIC BROWSER!!!");
        }

        if( ExitButton == null )
        {
            Debug.LogError("ERROR: EXIT BUTTON NOT FOUND ON MUSIC BROWSER!!!");
        }

        PlayButton.iconImage = PlayButtonIcon;

        NextButton.clicked += () =>
        {
            if( BrowserTabView.selectedTabIndex != 1 )
            {
                First5_Music = AudioManager.Instance.MusicFiles.GrabNextSet();
                MusicList.RefreshItems();
            }
            else
            {
                First5_Ambience = AudioManager.Instance.AmbienceFiles.GrabNextSet();
                AmbienceList.RefreshItems();
            }
        };

        PrevButton.clicked += () =>
        {
            if( BrowserTabView.selectedTabIndex != 1 )
            {
                First5_Music = AudioManager.Instance.MusicFiles.GrabPrevSet();
                MusicList.RefreshItems();
            }
            else
            {
                First5_Ambience = AudioManager.Instance.AmbienceFiles.GrabPrevSet();
                AmbienceList.RefreshItems();
            }
        };

        BrowserTabView.activeTabChanged += ( prevTab, NewTab ) => {
            if( BrowserTabView.selectedTabIndex != 1 )
            {
                if( !AudioManager.Instance.IsMusicPlaying() )
                {
                    PlayButtonIcon.sprite = PlayIcon;
                    PlayButton.iconImage = PlayButtonIcon;
                }
                else
                {
                    PlayButtonIcon.sprite = PauseIcon;
                    PlayButton.iconImage = PlayButtonIcon;
                }
            }
            else
            {
                if( !AudioManager.Instance.IsAmbiencePlaying() )
                {
                    PlayButtonIcon.sprite = PlayIcon;
                    PlayButton.iconImage = PlayButtonIcon;
                }
                else
                {
                    PlayButtonIcon.sprite = PauseIcon;
                    PlayButton.iconImage = PlayButtonIcon;
                }
            }
        };

        PlayButton.clicked += () =>
        {
            if( BrowserTabView.selectedTabIndex != 1 )
            {
                AudioManager.Instance.SetMusicTrack( AudioManager.Instance.MusicFiles.LoadClip( true, MusicList.selectedIndex ) );
                AudioManager.Instance.SetMusicPlay();
            }
            else
            {
                AudioManager.Instance.SetAmbienceTrack( AudioManager.Instance.AmbienceFiles.LoadClip( false, AmbienceList.selectedIndex ) );
                AudioManager.Instance.SetAmbiencePlay();
            }

            if( BrowserTabView.selectedTabIndex != 1 )
            {
                if( !AudioManager.Instance.IsMusicPlaying() )
                {
                    PlayButtonIcon.sprite = PlayIcon;
                    PlayButton.iconImage = PlayButtonIcon;
                }
                else
                {
                    PlayButtonIcon.sprite = PauseIcon;
                    PlayButton.iconImage = PlayButtonIcon;
                }
            }
            else
            {
                if( !AudioManager.Instance.IsAmbiencePlaying() )
                {
                    PlayButtonIcon.sprite = PlayIcon;
                    PlayButton.iconImage = PlayButtonIcon;
                }
                else
                {
                    PlayButtonIcon.sprite = PauseIcon;
                    PlayButton.iconImage = PlayButtonIcon;
                }
            }

        };

        ExitButton.clicked += () =>
        {
            ClearUI();
        };

    }

    public bool
    TextModeActive()
    {
        return SourceAsset.visualTreeAsset != null;
    }

    public void
    CreateNewProfile()
    {
    }

    public void
    LoadAllProfiles()
    {
    }
}
