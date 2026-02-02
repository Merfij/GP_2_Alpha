using System.Collections.Generic;
using UnityEngine;

public class Dissolve : MonoBehaviour
{
    [SerializeField] Material dissolve;
    List<Material> materials = new();
    bool dead = false;
    float count = 0;

    private void OnEnable()
    {
        GetComponent<EnemyBase>().onDeath += EnemyDissolve;
    }

    private void OnDisable()
    {
        GetComponent<EnemyBase>().onDeath -= EnemyDissolve;
    }

    void EnemyDissolve()
    {
        var objs = GetComponentsInChildren<Renderer>();
        foreach (var obj in objs)
        {
            obj.material = dissolve;
            materials.Add(obj.material);
        }
        dead = true;
    }
    private void Update()
    {
        
        if (dead)
        {
            count += (1 * Time.deltaTime) / 0.9f;
            foreach (var obj in materials)
            {
                obj.SetFloat("_Dissolve", count);
            }
        }
    }
}
