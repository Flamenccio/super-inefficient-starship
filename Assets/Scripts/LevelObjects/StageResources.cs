using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class StageResources : MonoBehaviour
{
    [SerializeField] private Stage mainScript; // the Stage script attached to this gameobject
    private static List<StageVariant> stageVariants = new List<StageVariant>();
    private void Start()
    {
        if (mainScript == null) throw new System.Exception("mainScript not assigned!");

        if (mainScript.InitialStage) // if this gameobject is the initial stage, load all resources; there must be exactly ONE initial stage!
        {
            stageVariants.AddRange(Resources.LoadAll<StageVariant>("StageVariants")); // load all stagevariants here
            mainScript.UpdateVariant(StageVariant.variants.Normal); // tell the stage to update
        }
    }
    /// <summary>
    /// Returns a StageVariant matching the variant given.
    /// </summary>
    public StageVariant GetStageVariant(StageVariant.variants variant)
    {
        foreach (StageVariant sv in stageVariants) 
        { 
            if (sv.Variant == variant) return sv;
        }
        return null;
    }
}
