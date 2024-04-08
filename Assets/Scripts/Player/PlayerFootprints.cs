using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFootprints : MonoBehaviour
{
    [SerializeField] private GameObject footprintPrefab;
    private const int FOOTPRINT_AMOUNT = 7;
    //private GameObject[] footprints = new GameObject[FOOTPRINT_AMOUNT];
    private const float STEP_DISTANCE = 5.0f; // the distance needed to travel to leave one footprint

    private Footprint currentFootprint = null;
    private Footprint previousFootprint = null;

    private void Start()
    {

        for (int i = 0; i < FOOTPRINT_AMOUNT; i++)
        {
            currentFootprint = Instantiate(footprintPrefab, transform.position, Quaternion.identity).GetComponent<Footprint>();
            currentFootprint.SetPrev(previousFootprint);
            if (previousFootprint != null) previousFootprint.SetNext(currentFootprint);

            previousFootprint = currentFootprint;
        }

        currentFootprint = BottomFootprint();

        /*
        // spawn footprints at the player's location and store them into the array
        for (int i = 0; i < FOOTPRINT_AMOUNT; i++)
        {
            GameObject fp = Instantiate(footprint, transform.position, Quaternion.identity);
            footprints[i] = fp;
        }
        */
    }
    private void Update()
    {

        if (Vector2.Distance(transform.position, previousFootprint.transform.position) >= STEP_DISTANCE)
        {
            Footprint bottomFootprint = currentFootprint.NextFootprint;

            currentFootprint.NextFootprint.SetPrev(null);
            currentFootprint.SetNext(null);

            previousFootprint.SetNext(currentFootprint);
            currentFootprint.SetPrev(previousFootprint);

            currentFootprint.Place(transform.position);

            previousFootprint = currentFootprint;
            currentFootprint = bottomFootprint;
        }

        /*
        if (Vector2.Distance(footprints[0].transform.position, transform.position) >= STEP_DISTANCE)
        {
            // take the last footprint, place it at the player's current position, and then move it to the top of the array while pushing everything else down 1

            currentFP = footprints[footprints.Length - 1];
            footprints[footprints.Length - 1] = null;
            currentFP.GetComponent<Footprint>().Place(transform.position);

            for (int i = footprints.Length - 1; i >= 0; i--)
            {
                GameObject temp = footprints[i];
                if (temp == null) continue;
                footprints[i + 1] = temp;
            }

            footprints[0] = currentFP;
        }
        */
    }
    /// <summary>
    /// returns the footprint whose previous footprint is null
    /// </summary>
    private Footprint BottomFootprint()
    {
        Footprint fp = previousFootprint;
        
        while (fp.PrevFootprint != null)
        {
            fp = fp.PrevFootprint;
        }
        return fp;
    }
}
