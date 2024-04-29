using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Card : MonoBehaviour
{

    /*
        This class is a little bloated
        I chose to do animation & "can this be played in this lane" logic here
        It is also the parent class of all individual cards.
        It perhaps should be somewhere else, but I don't think it's that bad.
        When a card is played to a Lane, it triggers a Gamecontroller method and stores lanes there
        This is good. In Gamecontroller, main game logic - actions in each phase - is done
        Main concern for this class is that I wrote it as "Player-owned card" rather than generic "Card"
        So some checks will need to be implemented around animations for enemy owned cards
    */
    public virtual string CardName {get;set;}
    public virtual string CardDescription {get;set;}
    public virtual string PlayEffectDescription {get;set;}
    public virtual float HP {get;set;}
    public virtual float MonAtk {get;set;}
    public virtual float PlayerAtk {get;set;}
    public virtual float Def {get;set;}
    public virtual float Cost{get;set;}


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

    public bool dragging = false;
    bool returning = false;
    bool mouseDown = false;
    Vector3 mousePosAtDown;

    GameObject UIGO;
    UI ui;


    // --Select Animation
    float speed = 15f;
    float lowery = -5.74f;
    float raisedy = -2.74f;

    float enemylowery = 6f;
    float enemyraisedy = 4f;

    Vector3 defaultScale = new Vector3(0.73f, 0.73f, 0.73f);
    Vector3 highlightScale = new Vector3(1f, 1f, 1f);
    IEnumerator raiseCoro;
    IEnumerator lowerCoro;
    // ^^Select Animation

    //Gameobject components
    BoxCollider2D boxCollider;

    public void setProps(int handPos, Hand parent, GameController control, GameObject portraitBg){
        PositionInHand = handPos;
        parentHand = parent;
        controller = control;
        portraitBackground = portraitBg;
        inHand = true;
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
        UIGO = GameObject.Find("UI");
        ui = UIGO.GetComponent<UI>();
        gameObject.transform.localScale = defaultScale;
        raiseCoro = RaiseCard();
        lowerCoro = LowerCard();

        if(parentHand.playerControlled){
            InitialSpawn = new Vector3(8.5f, -2.67f, -0.01f); //Approximately on top of the deck
        }else{
            InitialSpawn = new Vector3(-8f,2.5f,-0.01f);
        }
        restPosition = getHandSpace();
        
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

        //Stat Bars
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




        boxCollider = gameObject.AddComponent<BoxCollider2D>();
        boxCollider.offset = new Vector2(0,-2.2f);
        boxCollider.size = new Vector2(2.25f, 8f);

        if (parentHand.playerControlled){
            transform.Find("Card_Front").gameObject.SetActive(true);
        }else{
            transform.Find("Card_Front").gameObject.SetActive(false);
            transform.Find("Card_Back").gameObject.SetActive(true);
        }

        dealCard();
    }

    Vector3 getHandSpace(){
        if(parentHand.playerControlled){
            return new Vector3(-5.8f + (2f*PositionInHand),lowery, -1f + (0.1f*PositionInHand));
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
        releasePoint.z = ui.enemyPortrait.transform.position.z;
        if(ui.enemyPortrait.GetComponent<Collider2D>().bounds.Contains(releasePoint) && controller.turnPhase == GameController.TurnPhase.ATTACK){
            controller.attackEnemy(PlayerAtk);
        }
    }
    

    void checkLaneBounds(){
        Vector3 releasePoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,Camera.main.nearClipPlane));
        releasePoint.z = ui.monLane1.transform.position.z;
        if (parentHand.spellSlotCurrent - Cost >= 0 && controller.turnPhase == GameController.TurnPhase.SUMMON){ //probably seperate this out in future to parentHand.canCardBePlayed(card)
            if(ui.monLane1.GetComponent<Collider2D>().bounds.Contains(releasePoint) && !isSpell){
                inHand = false;
                inLane = true;
                parentHand.setLane1(this);
                parentHand.adjustSpellSlotCurrent(Cost);
                restPosition = ui.monLane1.transform.position;
            }
            if(ui.monLane2.GetComponent<Collider2D>().bounds.Contains(releasePoint) && !isSpell){
                inHand = false;
                inLane = true;
                parentHand.setLane2(this);
                parentHand.adjustSpellSlotCurrent(Cost);
                restPosition = ui.monLane2.transform.position;
            }
            if(ui.trapLane.GetComponent<Collider2D>().bounds.Contains(releasePoint) && !isMonster){
                inHand = false;
                inLane = true;
                parentHand.setTrapLane(this);
                parentHand.adjustSpellSlotCurrent(Cost);
                restPosition = ui.trapLane.transform.position;
            }
        }
        if(ui.discardLane.GetComponent<Collider2D>().bounds.Contains(releasePoint)){
            inHand = false;
            parentHand.discard(this);
            restPosition = ui.discardLane.transform.position;
        }
    }

    public void playCard(int lane){
        //AI play
        //Lane: 1=1, 2=2, 3=trap, 4=discard
        inHand = false;
        transform.Find("Card_Front").gameObject.SetActive(true);
        transform.Find("Card_Back").gameObject.SetActive(false);
        
        switch(lane){
            case 1:
                inLane = true;
                parentHand.setLane1(this);
                restPosition = parentHand.playerControlled? ui.monLane1.transform.position : ui.enemyMonLane1.transform.position;
                parentHand.adjustSpellSlotCurrent(Cost);
                break;
            case 2:
                inLane = true;
                parentHand.setLane2(this);
                restPosition = parentHand.playerControlled? ui.monLane2.transform.position : ui.enemyMonLane2.transform.position;
                parentHand.adjustSpellSlotCurrent(Cost);
                break;
            case 3:
                inLane = true;
                parentHand.setTrapLane(this);
                restPosition = parentHand.playerControlled? ui.trapLane.transform.position : ui.enemyTrapLane.transform.position;
                parentHand.adjustSpellSlotCurrent(Cost);
                break;
            case 4:
                parentHand.discard(this);
                restPosition = parentHand.playerControlled? ui.discardLane.transform.position : ui.enemyDiscardLane.transform.position;
                break;
        }

    }

    public virtual void SelectEffect(){
        Debug.Log("No card specific select effect defined.");
    }
    public virtual void PlayEffect(){
        Debug.Log("No card specific play effect defined.");
    }
    public virtual void SpellEffect(){
        Debug.Log("No card specific spell effect defined.");
    }


    IEnumerator RaiseCard(){
        gameObject.transform.localScale = highlightScale;
        gameObject.transform.position = gameObject.transform.position - new Vector3(0f,0f, 1f);
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
        gameObject.transform.position = gameObject.transform.position + new Vector3(0f,0f, 1f);
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
        if (inHand && !selected && !dragging && !returning && parentHand.playerControlled){
            StopCoroutine(raiseCoro);
            StopCoroutine(lowerCoro);
            raiseCoro = RaiseCard();

            restPosition.y = raisedy;

            StartCoroutine(raiseCoro);
        }
    }

    void OnMouseExit() {
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
