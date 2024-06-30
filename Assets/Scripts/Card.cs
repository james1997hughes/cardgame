using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class Card : MonoBehaviour
{

    /*
        This class is a little bloated
        But fixing it is a lot of work
        If it gets bad enough to hinder progress, I will fix
    */
    public virtual string CardName {get;set;}
    public virtual string CardDescription {get;set;}
    public virtual float HP {get;set;}
    public virtual float MonAtk {get;set;}
    public virtual float PlayerAtk {get;set;}
    public virtual float Def {get;set;}
    public virtual float Cost{get;set;}
    public virtual Dictionary<string, float> StatModifiers{get;set;}


    public bool inGame;
    public int cardNumber;
    public bool isMonster = false;
    public bool isSpell = false;
    public bool canBeTrap = false;
    public int PositionInHand;
    public Sprite subjectSprite;
    public GameObject portraitBackground;
    public Hand parentHand;

    public GameController controller;

    Vector3 InitialSpawn;
    Vector3 restPosition;
    
    public bool selected = false;
    public bool inHand = false;
    public bool inLane = false;
    public int laneNum = 0;

    public bool dragging = false;
    bool returning = false;
    bool mouseDown = false;
    Vector3 mousePosAtDown;

    GameObject UIGO;
    UI ui;


    // --Select Animation
    float speed = 15f;
    float lowery = -4.97f;
    float raisedy = -2.74f;

    float enemylowery = 6f;
    float enemyraisedy = 4f;

    Vector3 defaultScale = new Vector3(0.83f, 0.83f, 1f);
    Vector3 highlightScale = new Vector3(1.5f, 1.5f, 1f);
    IEnumerator raiseCoro;
    IEnumerator lowerCoro;
    // ^^Select Animation

    //Gameobject components
    public BoxCollider2D boxCollider;
    public SortingGroup sortingGroup;

    public void setProps(int handPos, Hand parent, GameController control, GameObject portraitBg, int numberCardsDrawn){
        PositionInHand = handPos;
        parentHand = parent;
        controller = control;
        portraitBackground = portraitBg;
        inHand = true;
        cardNumber = numberCardsDrawn;
        inGame = true;
    }
    public void setPropsInspect(GameObject portraitBg){
        portraitBackground = portraitBg;
        inHand = false;
        inGame = false;
    }

    void setMonster(bool i){
         isMonster = i;
    }
    void setSpell(bool i){
        isSpell = i;
    }
    void setTrap(bool i){
        canBeTrap = i;
    }
    void setPositionInHand(int i){
        PositionInHand = i;
    }

    void Start()
    {
        
        // Set Name
        GameObject textGO = transform.Find("Card_Front").Find("Canvas").Find("TextGO").gameObject;
        textGO.GetComponent<TextMeshProUGUI>().text = this.CardName;

        //Load Background
        Transform grid = transform.Find("Card_Front").Find("Card_PortraitBackground").Find("BackgroundGrid");
        GameObject background = Instantiate(portraitBackground, grid, false);
        background.transform.localPosition = new Vector3(0f,0f,0f);
        

        //Set portrait & shadow
        GameObject portrait = transform.Find("Card_Front").Find("Card_MonPortrait").gameObject;
        portrait.GetComponent<SpriteRenderer>().sprite = subjectSprite;
        GameObject shadow = portrait.transform.Find("Card_MonPortraitShadow").gameObject;
        shadow.GetComponent<SpriteRenderer>().sprite = subjectSprite;

        //Initialise stat mods
        StatModifiers = new Dictionary<string, float>
        {
            { "HP", 0 },
            { "MonAtk", 0 },
            { "PlayerAtk", 0 },
            { "Def", 0 }
        };

        //Stat Bars
        updateStatBars();


        //Mana cost
        Transform orbs = transform.Find("Card_Front").Find("Mana_Orbs");
        Transform[] transforms = orbs.GetComponentsInChildren<Transform>();
        //Transforms[0] is the Mana_Orbs parent / container gameobject transform
        //Orbs are at 1-5
        //Is there a better way to do this?
        
        if (Cost < 5){
            Destroy(transforms[5].gameObject);
        }if(Cost < 4){
            Destroy(transforms[4].gameObject);
        } if (Cost < 3){
            Destroy(transforms[3].gameObject);
        } if (Cost < 2){
            Destroy(transforms[2].gameObject);
        } if (Cost < 1){
            Destroy(transforms[1].gameObject);
        }

        if (inGame){
            SetupCardInPlay();
            dealCard();
        }
    }

    public void updateStatBars(){
        //this should also handle mods, maybe in a different color
        GameObject stats = transform.Find("Card_Front").Find("Card_Stats").gameObject;
        GameObject HPBar = stats.transform.Find("HPBar").Find("InnerBar").gameObject;
        GameObject DefBar = stats.transform.Find("DefBar").Find("InnerBar").gameObject;
        GameObject AtkBar = stats.transform.Find("AtkBar").Find("InnerBar").gameObject;
        GameObject PlayerAtkBar = stats.transform.Find("PlayerAtkBar").Find("InnerBar").gameObject;
        

        float maxScaleX = 4.6f;
        

        HPBar.transform.localScale = HPBar.transform.localScale + new Vector3((maxScaleX/12)*HP, 0, 0);
        HPBar.transform.localPosition = HPBar.transform.localPosition + new Vector3(HPBar.transform.localScale.x * 0.5f, 0f, 0f);

        DefBar.transform.localScale = DefBar.transform.localScale + new Vector3((maxScaleX/12)*Def, 0, 0);
        DefBar.transform.localPosition = DefBar.transform.localPosition + new Vector3(DefBar.transform.localScale.x * 0.5f, 0f, 0f);

        AtkBar.transform.localScale = AtkBar.transform.localScale + new Vector3((maxScaleX/12)*MonAtk, 0, 0);
        AtkBar.transform.localPosition = AtkBar.transform.localPosition + new Vector3(AtkBar.transform.localScale.x * 0.5f, 0f, 0f);

        PlayerAtkBar.transform.localScale = PlayerAtkBar.transform.localScale + new Vector3((maxScaleX/12)*PlayerAtk, 0, 0);
        PlayerAtkBar.transform.localPosition = PlayerAtkBar.transform.localPosition + new Vector3(PlayerAtkBar.transform.localScale.x * 0.5f, 0f, 0f);
    }

    void SetupCardInPlay(){
        UIGO = GameObject.Find("UI");
        ui = UIGO.GetComponent<UI>();
        
        boxCollider = gameObject.AddComponent<BoxCollider2D>();
        boxCollider.offset = new Vector2(0,-2.2f);
        boxCollider.size = new Vector2(2.25f, 8f);
        
        gameObject.transform.localScale = defaultScale;
        raiseCoro = RaiseCard();
        lowerCoro = LowerCard();
        if (parentHand.playerControlled){
            transform.Find("Card_Front").gameObject.SetActive(true);
        }else{
            transform.Find("Card_Front").gameObject.SetActive(false);
            transform.Find("Card_Back").gameObject.SetActive(true);
        }

        if(parentHand.playerControlled){
            InitialSpawn = new Vector3(8.5f, -2.67f, -0.01f); //Approximately on top of the deck
        }else{
            InitialSpawn = new Vector3(-8f,2.5f,-0.01f);
        }
        restPosition = getHandSpace();
    }

    public void fixText(){
        transform.Find("Card_Front").Find("Canvas").GetComponent<Canvas>().sortingLayerName = sortingGroup.sortingLayerName;
        transform.Find("Card_Front").Find("Canvas").GetComponent<Canvas>().sortingOrder = sortingGroup.sortingOrder +1;
    }
    public void SetOrderRecursively(Transform parentTransform, int newOrder)
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

    Vector3 getHandSpace(){
        if(parentHand.playerControlled){
            return new Vector3(-4.2f + (2f*PositionInHand),lowery, -1f + (0.1f*PositionInHand));
        } else{
            return new Vector3(3.04f - (2f*PositionInHand),enemylowery, -1f + (0.1f*PositionInHand));
        }
    }

    void dealCard(){
        gameObject.transform.position = InitialSpawn;
        inHand = true;
        StartCoroutine(returnCardAnim());
    }
    public void resetCard(){
        selected = false;
        dragging = false;
        returning = false;
        PositionInHand = parentHand.cardsInHand.IndexOf(this);
        restPosition = getHandSpace();
        StartCoroutine(returnCardAnim());
    }
    IEnumerator returnCardAnim(){
        gameObject.transform.localScale = defaultScale;
        returning = true;
        try{
            StopCoroutine(lowerCoro);
            StopCoroutine(raiseCoro);
        } catch{}
        while (Vector3.Distance(gameObject.transform.position, restPosition) > 0.005f){
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, restPosition, step);
            yield return null;
        }
        returning = false;
    }
    

    void Update()
    {
        if (mouseDown && parentHand.playerControlled){
            if (Vector3.Distance(Input.mousePosition, mousePosAtDown) > 50){
                sortingGroup.sortingLayerName = "cards_active";
                fixText();
                dragging = true;
                StopCoroutine(lowerCoro);
                StopCoroutine(raiseCoro);
                Vector3 screenPoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
                Vector3 dragPos = new Vector3(screenPoint.x, screenPoint.y, transform.position.z);
                gameObject.transform.position = dragPos;
            }
        }
        
        else if (dragging && !returning){
            //On release of dragged card:
                    sortingGroup.sortingLayerName = "cards_rest";
                    fixText();
                if (inHand){
                    Debug.Log("Checking lane bounds");
                    checkLaneBounds();
                } else if (inLane){
                    Debug.Log("Checking attack bounds");
                    checkAttackBounds();
                }
                StartCoroutine(returnCardAnim());
                dragging = false;
            }
        
    }

    void checkAttackBounds(){
        Vector3 releasePoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,Camera.main.nearClipPlane));
        var enemyLane1 = controller.getLane1Enemy();
        var enemyLane2 = controller.getLane2Enemy();

        if (controller.enemy.Health > 0){
            if(ui.enemyPortrait.GetComponent<Collider2D>().bounds.Contains(releasePoint) && controller.turnPhase == GameController.TurnPhase.ATTACK){ //Enemy Portrait
                controller.attackEnemy(PlayerAtk);
            }
        }

        if (enemyLane1 is not null){
            releasePoint.z = enemyLane1.transform.position.z; //this is stupid and shouldnt be needed... but it does work
            if(enemyLane1.boxCollider.bounds.Contains(releasePoint) && controller.turnPhase == GameController.TurnPhase.ATTACK){ //
                Debug.Log("Tried to attack "+enemyLane1.CardName);
                controller.attackCard(this, enemyLane1);
            }
        }
        if (enemyLane2 is not null){
            releasePoint.z = enemyLane2.transform.position.z;
            if(enemyLane2.boxCollider.bounds.Contains(releasePoint) && controller.turnPhase == GameController.TurnPhase.ATTACK){ //
                Debug.Log("Tried to attack "+enemyLane2.CardName);
                controller.attackCard(this, enemyLane2);
            }
        }


    }
    

    void checkLaneBounds(){
        Vector3 releasePoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,Camera.main.nearClipPlane));
        releasePoint.z = ui.monLane1.transform.position.z;
        if (parentHand.spellSlotCurrent - Cost >= 0 && controller.turnPhase == GameController.TurnPhase.SUMMON){ //probably seperate this out in future to parentHand.canCardBePlayed(card)
            if(ui.monLane1.GetComponent<Collider2D>().bounds.Contains(releasePoint) && !isSpell){
                parentHand.setLane1(this);
            }
            if(ui.monLane2.GetComponent<Collider2D>().bounds.Contains(releasePoint) && !isSpell){
                parentHand.setLane2(this);
            }
            if(ui.trapLane.GetComponent<Collider2D>().bounds.Contains(releasePoint) && !isMonster){
                parentHand.setTrapLane(this);
            }
        }
        if(ui.discardLane.GetComponent<Collider2D>().bounds.Contains(releasePoint)){
            parentHand.discard(this);
        }
    }

    public void playCard(int lane){
        //AI play
        //Lane: 1=1, 2=2, 3=trap, 4=discard
        inHand = false;
        transform.Find("Card_Front").gameObject.SetActive(true);
        transform.Find("Card_Back").gameObject.SetActive(false);
        parentHand.numberCardsPlayed += 1;
        sortingGroup.sortingLayerName = "cards_played";
        sortingGroup.sortingOrder = parentHand.numberCardsPlayed;
        fixText();
        switch(lane){
            case 1:
                inLane = true;
                laneNum = 1;
                restPosition = parentHand.playerControlled? ui.monLane1.transform.position : ui.enemyMonLane1.transform.position;
                parentHand.adjustSpellSlotCurrent(Cost);
                break;
            case 2:
                inLane = true;
                laneNum = 2;
                restPosition = parentHand.playerControlled? ui.monLane2.transform.position : ui.enemyMonLane2.transform.position;
                parentHand.adjustSpellSlotCurrent(Cost);
                break;
            case 3:
                inLane = true;
                laneNum = 3;
                restPosition = parentHand.playerControlled? ui.trapLane.transform.position : ui.enemyTrapLane.transform.position;
                parentHand.adjustSpellSlotCurrent(Cost);
                break;
            case 4:
                transform.Find("Card_Front").gameObject.SetActive(false);
                transform.Find("Card_Back").gameObject.SetActive(true);
                restPosition = parentHand.playerControlled? ui.discardLane.transform.position : ui.enemyDiscardLane.transform.position;
                break;
        }
        StartCoroutine(returnCardAnim());
    }

    public void KillCard(){
    
        if (inLane){
            switch (laneNum){
                case 0:
                    break;
                case 1:
                    parentHand.setLane1(null);
                    break;
                case 2:
                    parentHand.setLane2(null);
                    break;
                case 3:
                    parentHand.setTrapLane(null);
                    break;
            }
        }


        Destroy(gameObject);
    }

    public virtual void SelectEffect(){
        Debug.Log("No card specific select effect defined.");
    }
    public virtual void PreAttackEffect(){
        Debug.Log("No card specific pre attack defined.");
    }
    public virtual void PostAttackEffect(){
        Debug.Log("No card specific post attack effect defined.");
    }
    public virtual void SpellEffect(){
        Debug.Log("No card specific spell effect defined.");
    }

    public void resetStats(){
        StatModifiers["HP"] = 0;
        StatModifiers["MonAtk"] = 0;
        StatModifiers["PlayerAtk"] = 0;
        StatModifiers["Def"] = 0;
    }


    IEnumerator RaiseCard(){
        gameObject.transform.localScale = highlightScale;
        //gameObject.transform.position = gameObject.transform.position - new Vector3(0f,0f, 1f);
        sortingGroup.sortingLayerName = "cards_active";
        sortingGroup.sortingOrder = cardNumber + 100;
        fixText();
        while (gameObject.transform.position.y != raisedy){

            if (gameObject.transform.position.y > raisedy){gameObject.transform.position = new Vector3(gameObject.transform.position.x, raisedy, gameObject.transform.position.z);}
            if (gameObject.transform.position.y < raisedy){
                gameObject.transform.position = gameObject.transform.position + new Vector3(0, speed * Time.deltaTime, 0);
            }

            yield return null;
        }
    }

    IEnumerator LowerCard(){
        gameObject.transform.localScale = defaultScale;
       // gameObject.transform.position = gameObject.transform.position + new Vector3(0f,0f, 1f);
        sortingGroup.sortingLayerName = "cards_active";
        sortingGroup.sortingOrder = cardNumber;
        fixText();
        while (gameObject.transform.position.y != lowery){

            if (gameObject.transform.position.y < lowery){gameObject.transform.position = new Vector3(gameObject.transform.position.x, lowery, gameObject.transform.position.z);}
            if (gameObject.transform.position.y > lowery){
                gameObject.transform.position = gameObject.transform.position - new Vector3(0, speed * Time.deltaTime, 0);
            }

            yield return null;
        }
    }

    void OnMouseEnter() {
        //Coroutines are not the best way to do this
        //Probably have a Y position movement function running when y is not the hand default to a value N
        //on mouse enter, set N = -1.74f, on mouse exit set N=-3.74f
        //but who cares idk
        if (inGame){
            if (inHand && !selected && !dragging && !returning && parentHand.playerControlled){
                StopCoroutine(raiseCoro);
                StopCoroutine(lowerCoro);
                raiseCoro = RaiseCard();

                restPosition.y = raisedy;

                StartCoroutine(raiseCoro);
            }
        }
    }

    void OnMouseExit() {
        if (inGame){
            if (inHand && !selected && parentHand.playerControlled){
                if (!dragging && !returning){
                    StopCoroutine(raiseCoro);
                    StopCoroutine(lowerCoro);
                    lowerCoro = LowerCard();

                    StartCoroutine(lowerCoro);
                }
                restPosition.y = lowery;
            }
        }
        
    }

    void OnMouseDown(){
        mouseDown = true;
        mousePosAtDown = Input.mousePosition;

    }

    void OnMouseUp(){
        mouseDown = false;
        mousePosAtDown = new Vector3(0,0,5); //Reset

    }

    void OnMouseUpAsButton() {
        if (inHand && parentHand.playerControlled){
            if (selected && !dragging){
                selected = false;
                //Debug.Log("Selected: " + selected.ToString());
                return;
            }

            if (!selected && !dragging){
                selected = true;
                SelectEffect();
               // Debug.Log("Selected: " + selected.ToString());
                return;
            }
        }
    }
}
