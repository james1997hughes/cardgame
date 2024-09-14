using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using UnityEngine.Rendering;
using Unity.VisualScripting;
using System.Data.Common;

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

    public int numberCardsDrawn;
    public int numberCardsPlayed;

    public float Health;
    public float spellSlotMax;
    public float spellSlotCurrent;
    GameObject cardPrefab;
    GameObject grassPrefab;
    GameObject stonePrefab;
    Sprite spellCardSprite;
    Sprite reactiveSymbol;


    // Start is called before the first frame update
    void Start()
    {
        numberCardsDrawn = 0;
        numberCardsPlayed = 0;
        maxCardsInHand = 5;
        cardPrefab = Resources.Load<GameObject>("Prefabs/CardPrefab"); //need to add spell
        spellCardSprite = Resources.Load<Sprite>("spell_card_blank");
        reactiveSymbol = Resources.Load<Sprite>("reactive_symbol");
        grassPrefab = Resources.Load<GameObject>("Prefabs/Grass"); //should be configurable per card
        stonePrefab = Resources.Load<GameObject>("Prefabs/Stone"); //should be configurable per card
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

    void loadDeck()
    {
        string[] lines = File.ReadAllLines(@"Assets\Decks\deck2.txt");

        foreach (string x in lines)
        {
            try
            {
                cardsInDeck.Add(x);
            }
            catch
            {
                Debug.Log("Couldn't find card.");
            }
        }

    }
    public void drawToMax()
    {
        if (cardsInHand.Count < maxCardsInHand)
        {
            drawCards(maxCardsInHand - cardsInHand.Count);
        }
    }

    public void drawCards(int i)
    {
        if (cardsInDeck.Count > 0)
        {
            for (int k = 0; k < i; k++)
            {
                int lastIndex = cardsInDeck.Count - 1; // Index of the last item
                string poppedCard = cardsInDeck[lastIndex]; // Retrieve the last card
                cardsInDeck.RemoveAt(lastIndex);
                numberCardsDrawn += 1;
                GameObject cardGO = Instantiate(cardPrefab, new Vector2(0, 0), Quaternion.identity);
                var added = (Card)cardGO.AddComponent(Type.GetType("Cards." + poppedCard));
                Debug.Log(poppedCard);
                int index = cardsInHand.Count - 1;
                if (added.isSpell)
                {
                    if (added.isReactive)
                    {
                    cardGO.transform.Find("Card_Front").Find("Symbol").GetComponent<SpriteRenderer>().sprite = reactiveSymbol;
                    }
                cardGO.transform.Find("Card_Front").Find("Card_Base").GetComponent<SpriteRenderer>().sprite = spellCardSprite;
                Destroy(cardGO.transform.Find("Card_Front").Find("Card_Stats").gameObject);
                added.setProps(index, this, game, stonePrefab, numberCardsDrawn);
                }
                
                added.setProps(index, this, game, grassPrefab, numberCardsDrawn);
                added.sortingGroup = added.gameObject.GetComponent<SortingGroup>();
                Debug.Log(added.sortingGroup.sortingLayerName);
                added.sortingGroup.sortingLayerName = "cards_rest";
                added.sortingGroup.sortingOrder = numberCardsDrawn;
                added.fixText();
                cardsInHand.Add(added);
            }
            resetCardsInHand();
        }
    }

    void resetCardsInHand()
    {
        Debug.Log("Resetting cards");
        foreach (Card card in cardsInHand)
        {
            card.resetCard();
        }
    }

    public void adjustSpellSlotCurrent(float amount, bool max = false)
    {
        spellSlotCurrent = spellSlotCurrent - amount;
        if (max) { spellSlotCurrent = spellSlotMax; }
        if (playerControlled)
        {
            spellSlotContainer.setPercentage((spellSlotCurrent / spellSlotMax) * 100);
            spellSlotContainer.setCurrentSlotText(spellSlotCurrent);
        }
    }

    List<string> shuffleDeck(List<string> deck)
    {
        return deck.OrderBy(_ => UnityEngine.Random.value).ToList();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
