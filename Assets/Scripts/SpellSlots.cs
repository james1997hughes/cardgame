using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpellSlots : MonoBehaviour
{

    Transform barTransform;
    TextMeshProUGUI currentSlotsText;
    TextMeshProUGUI maxSlotsText;
    float maxFill = 10000f;
    void Start()
    {
        barTransform = transform.Find("SpellSlotBar").transform;
        currentSlotsText = transform.Find("Canvas").Find("CurrentSlotsText").gameObject.GetComponent<TextMeshProUGUI>();
        maxSlotsText = transform.Find("Canvas").Find("MaxSlotsText").gameObject.GetComponent<TextMeshProUGUI>();
    }

    public void setPercentage(float percent){
        float scaleVal = (percent/100)*maxFill;
        barTransform.localScale = new Vector3(barTransform.localScale.x, scaleVal, barTransform.localScale.z);
    }

    public void setCurrentSlotText(float current){
        currentSlotsText.text = current.ToString();
    }
    public void setMaxSlotText(float max){
        maxSlotsText.text = max.ToString();
    }
}
