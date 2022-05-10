using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Button_Script : MonoBehaviour
{
    public ObjectType type;
    public string obj_name;
    public string obj_cost;

    public event Action<SC_Objects> onItemBought;

    [SerializeField] private Button m_Button;
    [SerializeField] private TextMeshProUGUI m_NameText;
    [SerializeField] private TextMeshProUGUI m_CostText;
    [SerializeField] private Image m_itemImage;
    [SerializeField] private GameObject item;

    private SC_Objects sc_Objects;
    
    public void Init(SC_Objects scObjects)
    {
        sc_Objects = scObjects;

        m_NameText.text = sc_Objects.objText;
        m_CostText.text = "£"+ sc_Objects.objCost;
        m_itemImage.sprite = scObjects.image;
        item = sc_Objects.obj;

    }

    public void OnBuy()
    {
        onItemBought?.Invoke(sc_Objects);
        
    }

}
