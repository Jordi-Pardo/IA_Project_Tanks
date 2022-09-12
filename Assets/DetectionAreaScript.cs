using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionAreaScript : MonoBehaviour
{
    List<GameObject> collidersTanks = new List<GameObject>();
    [SerializeField] private Shoot shootSript;
    public bool isBoss = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isBoss)
        {
            if (other.gameObject.tag == "tank")
            {
                if (collidersTanks.Count == 0)
                {
                    shootSript.target = other.gameObject;
                }
                collidersTanks.Add(other.gameObject);

            }

        }
        else
        {
            if (other.gameObject.tag == "boss")
            {
                shootSript.target = other.gameObject;
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (isBoss)
        {
            collidersTanks.Remove(other.gameObject);
            if (other.gameObject.tag == "tank")
            {
                ChangeTarget();
            }
        }
        else
        {
            if (other.gameObject.tag == "boss")
            {
                shootSript.target = null;
            }
        }
    }

    public void ChangeTarget()
    {
        if(collidersTanks.Count > 0)
        {
            shootSript.target = collidersTanks[0].gameObject;
        }
        else
        {
            shootSript.target = null;
        }
    }
    public void RemoveTarget(GameObject gameObject)
    {
        collidersTanks.Remove(gameObject);
    }

}
