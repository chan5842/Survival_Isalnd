using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LayserBeam : MonoBehaviour
{
    Transform tr;
    LineRenderer line;
    RaycastHit hit;

    void Start()
    {
        tr = this.transform;
        line = GetComponent<LineRenderer>();

        line.useWorldSpace = false;
        line.enabled = false;
    }

    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        Ray ray = new Ray(tr.position + (Vector3.up * 0.02f), tr.forward);
        Debug.DrawRay(ray.origin, ray.direction * 20f, Color.green);

        if(Input.GetButtonDown("Fire1"))
        {
            line.SetPosition(0, tr.InverseTransformPoint(ray.origin));

            if(Physics.Raycast(ray, out hit, 100f))
            {
                line.SetPosition(1, tr.InverseTransformPoint(hit.point));
            }
            else
            {
                line.SetPosition(1, tr.InverseTransformPoint(ray.GetPoint(100f)));
            }
            StartCoroutine(ShowLayerBeam());
        }
    }

    IEnumerator ShowLayerBeam()
    {
        line.enabled = true;
        yield return new WaitForSeconds(Random.Range(0.01f, 0.2f));
        line.enabled = false;
    }
}
