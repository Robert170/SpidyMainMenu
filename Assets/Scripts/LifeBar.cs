using UnityEngine;
using UnityEngine.UI;
public class LifeBar : MonoBehaviour
{
  public Image thumbnailImage;                //Sets the image used to change when Spidy dies
  public Image runImage;                      //Sets the image used to define current stamina Spidy has
  public Image levelImage;                    //Sets the image used increase experience from Spidy until he increase one level
  public Image lifeImage;
  public Image comboLevel;                    //Image that sets the combo increase before Spidy can change into Berserker mode
  public Text level;                          //Text used to flag what level Spidy is currently
  public Text levelInner;
  public Text money;                          //Text used to define the amount of money Spidy has
  public Text moneyInner;
  public Text moneyShadow;
  public Image fullComboLevel;
  private Color enableAlpha;
  private Color disableAlpha;
  public GameObject effect;

  //-------------------------------------------------------------------------------------------------
  //Sets initial conditions for Spidy Experience and current level
  public void Start()
  {
    lifeImage.fillAmount = StaticVar.currentLife / StaticVar.maxLife;
    levelImage.fillAmount = StaticVar.levelExperience / StaticVar.maxExperienceLevel;
    level.text = "LEVEL " + StaticVar.characterLevel;
    levelInner.text = "LEVEL " + StaticVar.characterLevel;
    enableAlpha = new Color(1, 1, 1, 1); 
    disableAlpha = new Color(1, 1, 1, 0);
    fullComboLevel.color = disableAlpha;
    comboLevel.fillAmount = 0;
  }
  //Sets initial conditions for Spidy Experience and current level
  //-------------------------------------------------------------------------------------------------

  //-------------------------------------------------------------------------------------------------
  //Used to update amount of money Spidy has as he moves through the game
  public void Update()
  {
    money.text = StaticVar.money.ToString();
    moneyInner.text = StaticVar.money.ToString();
    moneyShadow.text = StaticVar.money.ToString();
  }
  //Used to update amount of money Spidy has as he moves through the game
  //-------------------------------------------------------------------------------------------------

  //-------------------------------------------------------------------------------------------------
  //Method used to reduce bright of the thumbnail, signaling Spidy has died
  public void SetThumbnail()
  {
    Color color = thumbnailImage.color;
    color.a = 0.5f;
    thumbnailImage.color = color;
  }
  //Method used to reduce bright of the thumbnail, signaling Spidy has died
  //-------------------------------------------------------------------------------------------------

  //-------------------------------------------------------------------------------------------------
  //Enables one cricket if allowed for Spidy to have an extra life
  public void EnableLifeBar(bool enabled)
  {
    foreach (Transform tr in transform)
    {
      tr.gameObject.SetActive(enabled);
    }
  }
  //Enables one cricket if allowed for Spidy to have an extra life
  //-------------------------------------------------------------------------------------------------

  //-------------------------------------------------------------------------------------------------
  //Routine used to define how crickets will appear in the game
  public void SetCurrentLife(float currentLife)
  {
        lifeImage.fillAmount = currentLife;
  }
  //Routine used to define how crickets will appear in the game
  //-------------------------------------------------------------------------------------------------

  //-------------------------------------------------------------------------------------------------
  //Sets the stamina of Spidy
  public void SetStamina(float staminaFactor)
  {
    runImage.fillAmount = staminaFactor;
  }
  //Sets the stamina of Spidy
  //-------------------------------------------------------------------------------------------------

  //-------------------------------------------------------------------------------------------------
  //Sets the level experience Spidy has before jumping into the next level
  public void SetLevel(float levelExperience)
  {
    levelImage.fillAmount = levelExperience;
  }
  //Sets the level experience Spidy has before jumping into the next level
  //-------------------------------------------------------------------------------------------------

  //-------------------------------------------------------------------------------------------------
  //Sets the current combo value before Spidy can change into Berserker mode
  public void SetCombo(float leveler)
  {
    comboLevel.fillAmount = leveler;
    fullComboLevel.fillAmount = leveler;
  }
  //Sets the current combo value before Spidy can change into Berserker mode
  //-------------------------------------------------------------------------------------------------

  //-------------------------------------------------------------------------------------------------
  //Sets the current level Spidy has
  public void SetLevelText(int valueLevel)
  {
    level.text = "LEVEL " + valueLevel;
    levelInner.text = "LEVEL " + valueLevel;
  }
    //Sets the current level Spidy has
    //-------------------------------------------------------------------------------------------------

    public void ActivateFullPowerImage()
  {
    fullComboLevel.color = enableAlpha;
    comboLevel.color = disableAlpha;
        effect.SetActive(true);
  }

  public void DeactivateFullPowerImage()
  {
    fullComboLevel.color = disableAlpha;
    comboLevel.color = enableAlpha;
    effect.SetActive(false);
  }
}
