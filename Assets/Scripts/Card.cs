using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class Card : MonoBehaviour
{
    public virtual string CardName { get; set; }
    public virtual string CardDescription { get; set; }
    public virtual float HP { get; set; }
    public virtual float MonAtk { get; set; }
    public virtual float PlayerAtk { get; set; }
    public virtual float Def { get; set; }
    public virtual float Cost { get; set; }
    public virtual Dictionary<string, float> StatModifiers { get; set; }

    public AudioSource audioSource;
    public AudioClip OnPlayAudio;
    public AudioClip OnSelectAudio;
    public AudioClip OnDeathAudio;
    public AudioClip OnDiscardAudio;

    public bool inGame;
    public int cardNumber;
    public bool isMonster = false;
    public bool isSpell = false;
    public bool canBeTrap = false;
    public bool isReactive = false;
    public int PositionInHand;
    public Sprite subjectSprite;
    public GameObject portraitBackground;
    public Hand parentHand;

    public GameController controller;

    Vector3 InitialSpawn;
    public Vector3 restPosition;

    public bool selected = false;
    public bool inHand = false;
    public bool inLane = false;

    public Lane lane;

    public bool dragging = false;
    bool returning = false;
    bool mouseDown = false;
    Vector3 mousePosAtDown;

    GameObject UIGO;
    UI ui;


    // --Select Animation
    float speed = 15f;
    float lowery = -4.97f;
    float raisedy = -2.1f;

    float enemylowery = 6f;
    float enemyraisedy = 4f;

    public Vector3 defaultScale = new Vector3(0.83f, 0.83f, 1f);

    public Vector3 viewScale = new Vector3(2f, 2f, 0f);
    // ^^Select Animation

    //Gameobject components
    public BoxCollider2D boxCollider;
    public SortingGroup sortingGroup;

    public void setProps(int handPos, Hand parent, GameController control, GameObject portraitBg, int numberCardsDrawn)
    {
        PositionInHand = handPos;
        parentHand = parent;
        controller = control;
        portraitBackground = portraitBg;
        inHand = true;
        cardNumber = numberCardsDrawn;
        inGame = true;
    }
    public void setPropsInspect(GameObject portraitBg)
    {
        portraitBackground = portraitBg;
        inHand = false;
        inGame = false;
    }


    void Start()
    {


        // BEGIN VISUAL SETUP
        // Set Name
        GameObject textGO = transform.Find("Card_Front").Find("Canvas").Find("TextGO").gameObject;
        textGO.GetComponent<TextMeshProUGUI>().text = this.CardName;

        //Load Background
        Transform grid = transform.Find("Card_Front").Find("Card_PortraitBackground").Find("BackgroundGrid");
        GameObject background = Instantiate(portraitBackground, grid, false);
        background.transform.localPosition = new Vector3(0f, 0f, 0f);


        //Set portrait & shadow
        GameObject portrait = transform.Find("Card_Front").Find("Card_MonPortrait").gameObject;
        portrait.GetComponent<SpriteRenderer>().sprite = subjectSprite;
        GameObject shadow = portrait.transform.Find("Card_MonPortraitShadow").gameObject;
        shadow.GetComponent<SpriteRenderer>().sprite = subjectSprite;

        // Set Ability text
        if (CardDescription == null || isSpell)
        {
            Destroy(transform.Find("Card_Front").Find("AbilityBox").gameObject);
        }
        else
        {
            GameObject AbilityTextGO = transform.Find("Card_Front").Find("Canvas").Find("AbilityTextGO").gameObject;
            AbilityTextGO.GetComponent<TextMeshProUGUI>().text = this.CardDescription;
        }

        //Set Spell text
        if (isSpell)
        {
            GameObject SpellTextGO = transform.Find("Card_Front").Find("Canvas").Find("SpellTextGO").gameObject;
            SpellTextGO.GetComponent<TextMeshProUGUI>().text = this.CardDescription;
        }
        else
        {
            Destroy(transform.Find("Card_Front").Find("Canvas").Find("SpellTextGO").gameObject);
        }

        //Stat Bars
        updateStatBars();

        //Mana cost
        Transform orbs = transform.Find("Card_Front").Find("Mana_Orbs");
        Transform[] transforms = orbs.GetComponentsInChildren<Transform>();

        if (Cost < 5)
        {
            Destroy(transforms[5].gameObject);
        }
        if (Cost < 4)
        {
            Destroy(transforms[4].gameObject);
        }
        if (Cost < 3)
        {
            Destroy(transforms[3].gameObject);
        }
        if (Cost < 2)
        {
            Destroy(transforms[2].gameObject);
        }
        if (Cost < 1)
        {
            Destroy(transforms[1].gameObject);
        }
        //END VISUAL SETUP

        //Audio setup
        audioSource = gameObject.AddComponent<AudioSource>();
        //Define default sound effects
        OnPlayAudio = Resources.Load<AudioClip>("Sound/Placement");
        OnSelectAudio = Resources.Load<AudioClip>("Sound/Sliceup");
        OnDeathAudio = null; //not sure if this is not just the same as discard, will have to playtest options
        OnDiscardAudio = Resources.Load<AudioClip>("Sound/Discard");

        //Initialise stat mods
        StatModifiers = new Dictionary<string, float>
        {
            { "HP", 0 },
            { "MonAtk", 0 },
            { "PlayerAtk", 0 },
            { "Def", 0 }
        };

        if (inGame)
        {
            SetupCardInPlay();
            dealCard();
        }
    }
    public void resetStats()
    {
        StatModifiers["HP"] = 0;
        StatModifiers["MonAtk"] = 0;
        StatModifiers["PlayerAtk"] = 0;
        StatModifiers["Def"] = 0;
    }

    // Interfaces / default behaviour
    public virtual void SelectEffect()
    {
        //Play audio - either default or specific
        audioSource.clip = OnSelectAudio;
        audioSource.Play();

        //Do other default select behaviour

    }
    public virtual void PlayEffect()
    {
        //Play audio - either default or specific
        audioSource.clip = OnPlayAudio;
        audioSource.Play();

        //Do other default play behaviour

    }
    public virtual void PreAttackEffect()
    {

    }
    public virtual void PostAttackEffect()
    {

    }
    public virtual void SpellEffect()
    {

    }
    public virtual void DiscardEffect()
    {
        //Play audio - either default or specific
        audioSource.clip = OnDiscardAudio;
        audioSource.Play();

        //Do other default discard behaviour

    }

    // Graphic & animation logic
    void SetupCardInPlay()
    {
        UIGO = GameObject.Find("UI");
        ui = UIGO.GetComponent<UI>();// TODO refactor to controller.ui

        boxCollider = gameObject.AddComponent<BoxCollider2D>();
        if (!parentHand.playerControlled)
        {
            boxCollider.offset = new Vector2(0, 0); //Bound collider to the card if enemy card - clipping issues
            boxCollider.size = new Vector2(2.25f, 3f);
        }
        else
        {
            boxCollider.offset = new Vector2(0, -2.2f); //Bound collider to below the card - selecting card moves card above hitbox
            boxCollider.size = new Vector2(2.25f, 8f);
        }

        gameObject.transform.localScale = defaultScale;
        if (parentHand.playerControlled)
        {
            transform.Find("Card_Front").gameObject.SetActive(true);
        }
        else
        {
            transform.Find("Card_Front").gameObject.SetActive(false);
            transform.Find("Card_Back").gameObject.SetActive(true);
        }

        if (parentHand.playerControlled)
        {
            InitialSpawn = new Vector3(8.5f, -2.67f, -0.01f); //Approximately on top of the deck
        }
        else
        {
            InitialSpawn = new Vector3(-8f, 2.5f, -0.01f);
        }
        restPosition = getHandSpace();
    }
    public void updateStatBars()
    {
        //Currently broken, but transforms stat bars on card to represent internal values


        //this should also handle mods, maybe in a different color
        GameObject stats = transform.Find("Card_Front").Find("Card_Stats").gameObject;
        GameObject HPBar = stats.transform.Find("HPBar").Find("InnerBar").gameObject;
        GameObject DefBar = stats.transform.Find("DefBar").Find("InnerBar").gameObject;
        GameObject AtkBar = stats.transform.Find("AtkBar").Find("InnerBar").gameObject;
        GameObject PlayerAtkBar = stats.transform.Find("PlayerAtkBar").Find("InnerBar").gameObject;


        float maxScaleX = 4.6f;


        HPBar.transform.localScale = HPBar.transform.localScale + new Vector3((maxScaleX / 12) * HP, 0, 0);
        HPBar.transform.localPosition = HPBar.transform.localPosition + new Vector3(HPBar.transform.localScale.x * 0.5f, 0f, 0f);

        DefBar.transform.localScale = DefBar.transform.localScale + new Vector3((maxScaleX / 12) * Def, 0, 0);
        DefBar.transform.localPosition = DefBar.transform.localPosition + new Vector3(DefBar.transform.localScale.x * 0.5f, 0f, 0f);

        AtkBar.transform.localScale = AtkBar.transform.localScale + new Vector3((maxScaleX / 12) * MonAtk, 0, 0);
        AtkBar.transform.localPosition = AtkBar.transform.localPosition + new Vector3(AtkBar.transform.localScale.x * 0.5f, 0f, 0f);

        PlayerAtkBar.transform.localScale = PlayerAtkBar.transform.localScale + new Vector3((maxScaleX / 12) * PlayerAtk, 0, 0);
        PlayerAtkBar.transform.localPosition = PlayerAtkBar.transform.localPosition + new Vector3(PlayerAtkBar.transform.localScale.x * 0.5f, 0f, 0f);

        // Stat Values
        if (isSpell)
        {
            Destroy(transform.Find("Card_Front").Find("Canvas").Find("HPGO").gameObject);
            Destroy(transform.Find("Card_Front").Find("Canvas").Find("MONATKGO").gameObject);
            Destroy(transform.Find("Card_Front").Find("Canvas").Find("DEFGO").gameObject);
            Destroy(transform.Find("Card_Front").Find("Canvas").Find("ATKGO").gameObject);
        }
        else
        {
            GameObject HPGO = transform.Find("Card_Front").Find("Canvas").Find("HPGO").gameObject;
            HPGO.GetComponent<TextMeshProUGUI>().text = HP.ToString();

            GameObject MONATKGO = transform.Find("Card_Front").Find("Canvas").Find("MONATKGO").gameObject;
            MONATKGO.GetComponent<TextMeshProUGUI>().text = MonAtk.ToString();

            GameObject DEFGO = transform.Find("Card_Front").Find("Canvas").Find("DEFGO").gameObject;
            DEFGO.GetComponent<TextMeshProUGUI>().text = Def.ToString();

            GameObject ATKGO = transform.Find("Card_Front").Find("Canvas").Find("ATKGO").gameObject;
            ATKGO.GetComponent<TextMeshProUGUI>().text = PlayerAtk.ToString();
        }

    }
    public void fixText()
    {
        transform.Find("Card_Front").Find("Canvas").GetComponent<Canvas>().sortingLayerName = sortingGroup.sortingLayerName;
        transform.Find("Card_Front").Find("Canvas").GetComponent<Canvas>().sortingOrder = sortingGroup.sortingOrder + 1;
    }
    void SetOrderRecursively(Transform parentTransform, int newOrder)
    {
        foreach (Transform child in parentTransform)
        {
            // Check if the child has a SpriteRenderer component
            SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = newOrder;
            }

            // Recursively call this function for each child
            SetOrderRecursively(child, newOrder);
        }
    }
    Vector3 getHandSpace()
    {
        if (parentHand.playerControlled)
        {
            return new Vector3(-4.2f + (2f * PositionInHand), lowery, -1f + (0.1f * PositionInHand));
        }
        else
        {
            return new Vector3(3.04f - (2f * PositionInHand), enemylowery, -1f + (0.1f * PositionInHand));
        }
    }
    void dealCard()
    {
        gameObject.transform.position = InitialSpawn;
        inHand = true;
        StartCoroutine(returnCardAnim());
    }
    public void resetCard()
    {
        selected = false;
        dragging = false;
        returning = false;
        PositionInHand = parentHand.cardsInHand.IndexOf(this);
        restPosition = getHandSpace();
        StartCoroutine(returnCardAnim());
    }
    public IEnumerator returnCardAnim()
    {
        //gameObject.transform.localScale = defaultScale;
        returning = true;
        while (Vector3.Distance(gameObject.transform.position, restPosition) > 0.005f)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, restPosition, step);
            yield return null;
        }
        returning = false;
    }
    void Update()
    {
        if (mouseDown && parentHand.playerControlled)
        {
            if (Vector3.Distance(Input.mousePosition, mousePosAtDown) > 10)
            {
                sortingGroup.sortingLayerName = "cards_active";
                fixText();
                dragging = true;
                Vector3 screenPoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
                Vector3 dragPos = new Vector3(screenPoint.x, screenPoint.y, transform.position.z);
                gameObject.transform.position = dragPos;
            }
        }

        else if (dragging && !returning)
        {
            //On release of dragged card:
            sortingGroup.sortingLayerName = "cards_rest";
            fixText();
            if (inHand)
            {
                Debug.Log("Checking lane bounds");
                checkLaneBounds();
            }
            else if (inLane)
            {
                Debug.Log("Checking attack bounds");
                checkAttackBounds();
            }
            StartCoroutine(returnCardAnim());
            dragging = false;
        }

    }
    void checkAttackBounds()
    {
        Vector3 releasePoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
        var enemyLane1 = controller.getLane1Enemy();
        var enemyLane2 = controller.getLane2Enemy();

        if (controller.enemy.Health > 0)
        {
            if (ui.enemyPortrait.GetComponent<Collider2D>().bounds.Contains(releasePoint) && controller.turnPhase == GameController.TurnPhase.ATTACK)
            { //Enemy Portrait
                controller.attackGamePlayer(controller.enemy, PlayerAtk);
            }
        }

        if (enemyLane1 is not null)
        {
            releasePoint.z = enemyLane1.transform.position.z; //this is stupid and shouldnt be needed... but it does work
            if (enemyLane1.boxCollider.bounds.Contains(releasePoint) && controller.turnPhase == GameController.TurnPhase.ATTACK)
            { //
                Debug.Log("Tried to attack " + enemyLane1.CardName);
                controller.attackCard(this, enemyLane1);
            }
        }
        if (enemyLane2 is not null)
        {
            releasePoint.z = enemyLane2.transform.position.z;
            if (enemyLane2.boxCollider.bounds.Contains(releasePoint) && controller.turnPhase == GameController.TurnPhase.ATTACK)
            { //
                Debug.Log("Tried to attack " + enemyLane2.CardName);
                controller.attackCard(this, enemyLane2);
            }
        }


    }
    void checkLaneBounds()
    {
        Vector3 releasePoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
        releasePoint.z = ui.monLane1GO.transform.position.z;

        if (ui.discardLaneGO.GetComponent<Collider2D>().bounds.Contains(releasePoint))
        {
            controller.tryDiscard(this, parentHand);
        }
        else
        {
            //Playables (Monsters & traps)
            if (isMonster && controller.turnPhase == GameController.TurnPhase.SUMMON)
            {
                if (ui.monLane1GO.GetComponent<Collider2D>().bounds.Contains(releasePoint))
                {
                    StartCoroutine(controller.executeMove(new Move(MoveType.SUMMON, this, Lanes.MONSTER_LANE_1, controller.player)));
                }
                if (ui.monLane2GO.GetComponent<Collider2D>().bounds.Contains(releasePoint))
                {
                    StartCoroutine(controller.executeMove(new Move(MoveType.SUMMON, this, Lanes.MONSTER_LANE_2, controller.player)));
                }
            }
            if (canBeTrap && controller.turnPhase == GameController.TurnPhase.SUMMON)
            {
                if (ui.trapLaneGO.GetComponent<Collider2D>().bounds.Contains(releasePoint))
                {
                    StartCoroutine(controller.executeMove(new Move(MoveType.SUMMON, this, Lanes.TRAP_LANE, controller.player)));
                }
                //Spells
                //...
            }
        }

    }
    void OnMouseEnter()
    {
        if (inGame)
        {
            if (inHand && !selected && !dragging && !returning && parentHand.playerControlled)
            {
                restPosition.y = raisedy;
                gameObject.transform.localScale = viewScale;
                StartCoroutine(returnCardAnim());
            }
        }
    }
    void OnMouseExit()
    {
        if (inGame)
        {
            if (inHand && !selected && parentHand.playerControlled)
            {
                if (!dragging && !returning)
                {
                    restPosition.y = lowery;
                    gameObject.transform.localScale = defaultScale;
                    StartCoroutine(returnCardAnim());
                }

            }
        }

    }
    void OnMouseDown()
    {
        mouseDown = true;
        mousePosAtDown = Input.mousePosition;

    }
    void OnMouseUp()
    {
        mouseDown = false;
        mousePosAtDown = new Vector3(0, 0, 5); //Reset

    }
    void OnMouseUpAsButton()
    {
        if (inHand && parentHand.playerControlled)
        {
            if (!selected && !dragging) // Select a card
            {
                selected = true;
                SelectEffect();
                return;
            }
            if (selected && !dragging) // Deselect a card
            {
                selected = false;
                return;
            }
        }
    }
}