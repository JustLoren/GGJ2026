using System.Collections.Generic;
using UnityEngine;

public class OneOf : MonoBehaviour
{
    public static HashSet<string> KnownInstances = new();

    void Awake()
    {
        if (KnownInstances.Contains(this.name))
            Destroy(this.gameObject);

        KnownInstances.Add(this.name);
        DontDestroyOnLoad(this.gameObject);
    }
}
