using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using Unity.VisualScripting;

public class Hand : MonoBehaviour
{
    public GameObject gameController;
    GameController game;
    public List<Card> cardsInHand;
    public List<String> cardsInDeck;
    List<Card> shuffledDeck;
    SpellSlots spellSlotContainer;

    public bool playerControlled;
    public int maxCardsInHand;

    public float Health;
    public float spellSlotMax;
    public float spellSlotCurrent;




    GameObject monPrefab;
    GameObject grassPrefab;
    Sprite spellCardSprite;
    Sprite reactiveSymbol;


    // Start is called before the first frame update
    void Start()
    {
        maxCardsInHand = 5;
        monPrefab = Resources.Load<GameObject>("Prefabs/MonsterCardPrefab"); //need to add spell
        spellCardSprite = Resources.Load<Sprite>("spell_card_blank");
        reactiveSymbol = Resources.Load<Sprite>("reactive_symbol");
        grassPrefab = Resources.Load<GameObject>("Prefabs/Grass"); //should be configurable per card
        game = gameController.GetComponent<GameController>();
        cardsInHand = new List<Card>();
        cardsInDeck = new List<string>();
        spellSlotContainer = GameObject.Find("SpellSlots").GetComponent<SpellSlots>();
        spellSlotContainer.setPercentage(100f);
        spellSlotContainer.setCurrentSlotText(spellSlotCurrent);
        spellSlotContainer.setMaxSlotText(spellSlotMax);
        loadDeck();
        cardsInDeck = shuffleDeck(cardsInDeck);
    }

    void loadDeck(){
        string[] lines = File.ReadAllLines(@"Assets\Decks\deck1.txt");

        foreach (string x in lines){
            try{
                cardsInDeck.Add(x);
            }catch{
                Debug.Log("Couldn't find card.");
            }
        }
        
    }
    public void drawToMax(){
        if (cardsInHand.Count < maxCardsInHand){
            drawCards(maxCardsInHand - cardsInHand.Count);
        }
    }
    
    public void drawCards(int i){
        if (cardsInDeck.Count > 0){
            for(int k = 0; k < i; k++){
                int lastIndex = cardsInDeck.Count - 1; // Index of the last item
                string poppedCard = cardsInDeck[lastIndex]; // Retrieve the last card
                cardsInDeck.RemoveAt(lastIndex); 
                GameObject cardGO = Instantiate(monPrefab, new Vector2(0,0), Quaternion.identity);
                var added = (Card)cardGO.AddComponent(Type.GetType(poppedCard));
                if (added.isSpell){
                    cardGO.transform.Find("Card_Front").Find("Card_Base").GetComponent<SpriteRenderer>().sprite = spellCardSprite;
                    cardGO.transform.Find("Card_Front").Find("Symbol").GetComponent<SpriteRenderer>().sprite = reactiveSymbol;
                    Destroy(cardGO.transform.Find("Card_Front").Find("Card_Stats").gameObject);
                }
                int index= cardsInHand.Count-1;
                added.setProps(index, this, game, grassPrefab);
                cardsInHand.Add(added);
            }
            resetCardsInHand();
        }
    }

    void resetCardsInHand(){
        Debug.Log("Resetting cards");
        foreach(Card card in cardsInHand){
            card.resetCard();
        }
    }

    public void adjustSpellSlotCurrent(float amount){
        spellSlotCurrent = spellSlotCurrent - amount;
        spellSlotContainer.setPercentage((spellSlotCurrent/spellSlotMax)*100);
        spellSlotContainer.setCurrentSlotText(spellSlotCurrent);
    }

    List<string> shuffleDeck(List<string> deck){
        return deck.OrderBy(_ => UnityEngine.Random.value).ToList();
    }
    public void setLane1(Card card){
        if(!cardsInHand.Remove(card)){
            Debug.Log("Card failed to remove!");
        };
        if(playerControlled){
            game.setLane1Player(card);
        }
        //drawCards(1);
    }
    public void setLane2(Card card){
        if(!cardsInHand.Remove(card)){
            Debug.Log("Card failed to remove!");
        };
        if(playerControlled){
            game.setLane2Player(card);
        } else {

        }
        //drawCards(1);
    }

    public void setTrapLane(Card card){
        if(!cardsInHand.Remove(card)){
            Debug.Log("Card failed to remove!");
        };
        if(playerControlled){
            game.setTrapLanePlayer(card);
        }else{}
        //drawCards(1);
    }

    public void discard(Card card){
        if(!cardsInHand.Remove(card)){
            Debug.Log("Card failed to remove!");
        };
        //drawCards(1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
