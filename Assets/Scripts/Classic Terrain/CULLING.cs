using UnityEngine;
using System.Collections;

public class DistanceCulling : MonoBehaviour
{
    public float cullDistance = 80f;

    Transform cam;
    Renderer[] renderers;

    void Start()
    {
        cam = Camera.main.transform;
        renderers = GetComponentsInChildren<Renderer>();
        StartCoroutine(CheckVisibility());
    }

    IEnumerator CheckVisibility()
    {
        WaitForSeconds wait = new WaitForSeconds(0.25f);

        while (true)
        {
            float d = Vector3.Distance(transform.position, cam.position);
            bool visible = d < cullDistance;

            foreach (var r in renderers)
                r.enabled = visible;

            yield return wait;
        }
    }
}
