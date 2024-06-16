using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChemicalFire : Fire
{
    protected override void Start()
    {
        fireType = FireType.Chemical;
        //hp = 100f; // Set the default HP here
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        // Additional logic specific to OrganicFire
    }
}
