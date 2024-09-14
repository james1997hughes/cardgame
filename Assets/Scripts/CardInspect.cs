using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.VisualScripting;

public class CardInspect : MonoBehaviour
{

    GameObject monPrefab;
    GameObject spellPrefab;
    GameObject grassPrefab;
    Sprite spellCardSprite;
    Sprite reactiveSymbol;
    // Start is called before the first frame update
    void Start()
    {
        monPrefab = Resources.Load<GameObject>("Prefabs/CardPrefab"); //need to add spell
        spellPrefab = Resources.Load<GameObject>("Prefabs/SpellCardPrefab");
        spellCardSprite = Resources.Load<Sprite>("spell_card_blank");
        reactiveSymbol = Resources.Load<Sprite>("reactive_symbol");
        grassPrefab = Resources.Load<GameObject>("Prefabs/Grass");
        loadCards();

    }

    // Update is called once per frame
    void Update()
    {

    }

    void loadCards()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();

        // Get all types in the assembly
        Type[] types = assembly.GetTypes();

        // Filter to get only classes within the specified namespace
        var classes = types.Where(t => t.IsClass && t.Namespace == "Cards");

        // Output the class names
        float col = 1f;
        foreach (var cls in classes)
        {
            setupCard(cls, col);
            col++;
        }
        /* for (int i = 0; i < 6; i++){ //Fill out the list for test purposes
             foreach (var cls in classes)
             {
                 setupCard(cls, col);
                 col++;
             }
         }8*/
        var rt = transform.GetComponent<RectTransform>();
        Vector2 sizeDelta = rt.sizeDelta;
        sizeDelta.y = 200 + (float)Math.Ceiling(col / 6f) * 210;
        rt.sizeDelta = sizeDelta;
    }
    void setupCard(Type cls, float col)
    {
        Type currentCardType = Type.GetType(cls.FullName);
        GameObject cardGO = Instantiate(monPrefab, new Vector2(0, 0), Quaternion.identity);
        cardGO.transform.parent = transform;
        cardGO.transform.localScale = new Vector3(60f, 60f, 0);
        var added = (Card)cardGO.AddComponent(currentCardType);
        if (added.isSpell)
        {
            cardGO.transform.Find("Card_Front").Find("Card_Base").GetComponent<SpriteRenderer>().sprite = spellCardSprite;
            cardGO.transform.Find("Card_Front").Find("Symbol").GetComponent<SpriteRenderer>().sprite = reactiveSymbol;
            Destroy(cardGO.transform.Find("Card_Front").Find("Card_Stats").gameObject);
        }
        added.setPropsInspect(grassPrefab);
        added.sortingGroup = added.gameObject.GetComponent<SortingGroup>();
        added.sortingGroup.sortingLayerName = "cards_rest";
        added.fixText();
        cardGO.transform.localPosition = new Vector2(150 + (150 * (col % 6)), -1 * (float)Math.Ceiling(col / 6f) * 210); //this is a bit fucked but works for now

    }

    void displayCards()
    {

    }
}
