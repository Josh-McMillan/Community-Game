using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnviromentalInteractions : MonoBehaviour
{
    public void OnInteracted()
    {
        Destroy(gameObject);
        // Add the item to the player's inventory
        print("Enviromental Interactions: Automatic Item Pickup: \n " + gameObject.name + " has been picked up and added to the player's inventory.");
        print("Failed to add object: " + gameObject.name + " to the player's inventory, error: \n No inventory has been asigned in 'EnviromentalInteractions.cs'!");
    }
}