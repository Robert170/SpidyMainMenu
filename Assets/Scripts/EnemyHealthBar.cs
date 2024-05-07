using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Image barImage;
    public Canvas barCanvas;

    public void setLife(float currentLife, float maxLife)
    {
        barImage.fillAmount = currentLife / maxLife;

    }

    public void setPosition(Vector3 enemyPosition, float yPosition, float zPosition)
    {
        enemyPosition.z += zPosition;
        enemyPosition.y += yPosition;
        barCanvas.transform.localPosition = enemyPosition;

    }

    public void OnDestroy()
    {
        Destroy(this.gameObject);

    }

}
