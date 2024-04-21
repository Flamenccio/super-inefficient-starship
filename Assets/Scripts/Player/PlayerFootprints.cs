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
