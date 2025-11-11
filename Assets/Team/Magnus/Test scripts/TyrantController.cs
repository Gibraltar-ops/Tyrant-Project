using System.Collections.Generic;
using UnityEngine;

public class TyrantController : MonoBehaviour
{
    [Header("Scene references")]
    [SerializeField] private CapsuleCollider2D absorbArea;
    [SerializeField] private CircleCollider2D catchTrigger;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Transform mouthPoint;

    [Header("Absorption settings")]
    [SerializeField] private float liftSpeed = 6f;
    [SerializeField] private double biomassPerHuman = 5;

    private void Reset()
    {
        absorbArea = GetComponentInChildren<CapsuleCollider2D>();
        catchTrigger = GetComponent<CircleCollider2D>();
        if (gameManager == null) gameManager = FindFirstObjectByType<GameManager>();
        if (mouthPoint == null) mouthPoint = transform;
    }

    private void Awake()
    {
        if (catchTrigger != null) catchTrigger.isTrigger = true;
        if (absorbArea != null) absorbArea.isTrigger = true;

    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (absorbArea != null && absorbArea.OverlapPoint(worldPoint))
            {
                TryStartAbsorbingOneHuman();
            }
        }
    }

    private void TryStartAbsorbingOneHuman()
    {
        var results = new Collider2D[32];
        var filter = ContactFilter2D.noFilter;
        int count = Physics2D.OverlapCollider(absorbArea, filter, results);

        HumanMovement chosen = null;
        float bestDist = float.PositiveInfinity;

        for (int i = 0; i < count; i++)
        {
            var col = results[i];
            if (col == null) continue;
            if (!col.CompareTag("Human")) continue;
            if (!col.TryGetComponent(out HumanMovement hm)) continue;
            if (hm.IsBeingAbsorbed) continue;

            float d = ((Vector2)(col.transform.position - transform.position)).sqrMagnitude;
            if (d < bestDist)
            {
                bestDist = d;
                chosen = hm;
            }
        }

        if (chosen != null)
        {
            chosen.StartAbsorption(mouthPoint, liftSpeed, biomassPerHuman);
        }
    }


}