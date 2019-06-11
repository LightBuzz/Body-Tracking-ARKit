using UnityEngine;

public static class LineRendererExtensions
{
    /// <summary>
    /// Sets the positions from the transforms to the line renderer. Variable positionCount must be already set.
    /// </summary>
    /// <param name="lineRenderer">Line renderer.</param>
    /// <param name="transforms">Transforms.</param>
    public static void SetPositions(this LineRenderer lineRenderer, Transform[] transforms)
    {
        Vector3[] positions = new Vector3[transforms.Length];

        for (int i = 0; i < transforms.Length; i++)
            positions[i] = transforms[i].position;

        lineRenderer.SetPositions(positions);
    }
}
