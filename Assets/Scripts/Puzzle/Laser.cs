using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Laser : MonoBehaviour
{
    public LayerMask layerMask;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float laserRange = 10f;
    [SerializeField] private AbilityType currentAbility;

    public RaycastHit hit;
    private IAbilityTarget currentTarget;

    public void FixedUpdate()
    {
        currentAbility = AbilityType.Light;

        Ray ray = new Ray(shootPoint.position, shootPoint.forward);

        if (Physics.Raycast(ray, out hit, laserRange, layerMask))
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, shootPoint.position);
            lineRenderer.SetPosition(1, hit.point);

            if(hit.collider.TryGetComponent<IAbilityTarget>(out var target))
            {
                if(target != currentTarget)
                {
                    currentTarget?.AbilityEnd(currentAbility);
                    currentTarget = target;
                    currentTarget.AbilityStart(currentAbility);
                }
            }
            else { ClearTarget(); }
        }
        else
        {
            lineRenderer.SetPosition(0, shootPoint.position);
            lineRenderer.SetPosition(1, ray.origin + (ray.direction * laserRange));
            ClearTarget();
        }
    }
    void ClearTarget()
    {
        if (currentTarget != null)
        {
            currentTarget.AbilityEnd(currentAbility);
            currentTarget = null;
        }
    }
}
