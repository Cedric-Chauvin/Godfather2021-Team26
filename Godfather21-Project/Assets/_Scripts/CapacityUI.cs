using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CapacityUI : MonoBehaviour
{
    private static CapacityUI ui = null;
    public static CapacityUI UI
    {
        get => ui;
    }

    [SerializeField]
    private Animator BlueOrder;
    [SerializeField]
    private Animator RedOrder;
    [SerializeField]
    private Animator BlueRally;
    [SerializeField]
    private Animator RedRally;

    private void Awake()
    {
        if (!ui)
            ui = this;
        else
            Destroy(this);
    }

    public void OrderUsable(int id,bool usable)
    {
        if(id == 0)
            BlueOrder.SetBool("Usable", usable);
        else
            RedOrder.SetBool("Usable", usable);
    }

    public void CrownToggle(int id, bool haveCrown) { 
        if(id ==0)
        {
            BlueOrder.SetBool("Crown", haveCrown);
            BlueRally.SetBool("Crown", haveCrown);
        }
        else
        {
            RedOrder.SetBool("Crown", haveCrown);
            RedRally.SetBool("Crown", haveCrown);
        }
    }

    public void RallyUsable(int id,bool usable)
    {
        if (id == 0)
            BlueRally.SetBool("Usable", usable);
        else
            RedRally.SetBool("Usable", usable);
    }
}
