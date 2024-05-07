using System.Collections;
using UnityEngine;

public class ActorFlicker : MonoBehaviour
{
    public SpriteRenderer[] baseSprites;
    private float waitingTime = 0.1f;
    private float jumpSpace = 10;
    private float realJump;
    private Color color;
    // Start is called before the first frame update
    void Start()
    {
        realJump = waitingTime / jumpSpace;
        StartCoroutine(Flickers(baseSprites));

    }

    private IEnumerator Flickers(SpriteRenderer[] Sprites)
    {
        int i = 0;
        while (i <= jumpSpace)
        {
            foreach (var sprite in Sprites)
            {
                color = sprite.color;
                color.a = i / jumpSpace;
                sprite.color = color;

            }
            i ++;
            yield return new WaitForSecondsRealtime(realJump);

        }

    }

}
