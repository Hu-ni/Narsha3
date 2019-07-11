using System.Collections;
using UnityEngine;

public class Buff : Skill
{
    public float duration;
    private float time;

    public bool isEffect = false;
    
    public IEnumerator Effect()
    {
        isEffect = true;
        time = duration;
        while (time >= 0)
        {
            time -= Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        ModelActive(false);
        isEffect = false;
    }
}
