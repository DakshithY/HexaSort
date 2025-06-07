using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerups : MonoBehaviour
{
    private void Update()
    {
        if (PowerOn == true) 
        {
            Destroystack();
        }
    }
    public void Activatepower() 
    {
     PowerOn = true;
    }
    public bool PowerOn = false;

    public void Destroystack()
    {
        if (!PowerOn) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 500f))
        {
            Hexstack hexstack = hit.collider.GetComponentInParent<Hexstack>();

            if (hexstack != null)
            {
                hexstack.ClearStack();
                PowerOn = false;
            }
        }
    }

}
