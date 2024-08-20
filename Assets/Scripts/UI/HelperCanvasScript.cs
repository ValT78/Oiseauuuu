using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HelperCanvasScript : MonoBehaviour
{
    public static HelperCanvasScript Instance { get; private set; }
    public TextMeshProUGUI helperTitle;
    public TextMeshProUGUI helperText;

    // Change string in inspector (it will be overwritten if you write here)

    public string defaultTitle = "Welcome to the game!";
    [TextArea(3, 10)]
    public string defaultText = "Press 1, 2 or 3 num keys, to select a building, then press the same keys to start placing the building";

    public string houseExplanationTitle = "House";
    [TextArea(3, 10)]
    public string houseExplanationText = "Houses are used to increase the population of your city. The more houses you have, the more people will live in your city.";

    public string farmExplanationTitle = "Farm";
    [TextArea(3, 10)]
    public string farmExplanationText = "Farms are used to produce food. The more farms you have, the more food you will produce. Ressources are produced after each blocks placed";

    public string composterExplanationTitle = "Composter";
    [TextArea(3, 10)]
    public string composterExplanationText = "Composters are used to produce compost. The more composters you have, the more compost you will produce. Ressources are produced after each blocks placed";

    public string woodFactoryExplanationTitle = "Wood Factory";
    [TextArea(3, 10)]
    public string woodFactoryExplanationText = "Wood Factories are used to produce wood. The more wood factories you have, the more wood you will produce. Ressources are produced after each blocks placed";

    public string woolFactoryExplanationTitle = "Wool Factory";
    [TextArea(3, 10)]
    public string woolFactoryExplanationText = "Wool Factories are used to produce wool. The more wool factories you have, the more wool you will produce. Ressources are produced after each blocks placed";

    public string glueBlockExplanationTitle = "Glue block";
    [TextArea(3, 10)]
    public string glueBlockExplanationText = "Glue blocks is just used for placement, it does not produce anything. It glue to surface and is used to build on top of it";

    public string simpleBlockExplanationTitle = "Simple block";
    [TextArea(3, 10)]
    public string simpleBlockExplanationText = "Simple blocks is just used for placement, it does not produce anything and it's free. It is used to build on top of it";

    public string rotatingExplanationTitle = "Rotating";
    [TextArea(3, 10)]
    public string rotatingExplanationText = "You can rotate the building by pressing Q or E (phsyical keys, qwerty reference) press space for the block to start falling";

    public string movingExplanationTitle = "Moving";
    [TextArea(3, 10)]
    public string movingExplanationText = "You can move the building by pressing A, S or D (phsyical keys, qwerty reference)";



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }
    void Start()
    {
        DefaultHelper();
    }

    public void DefaultHelper()
    {
        helperTitle.text = defaultTitle;
        helperText.text = defaultText;
    }

    public void HouseExplanation()
    {
        helperTitle.text = houseExplanationTitle;
        helperText.text = houseExplanationText;
    }

    public void FarmExplanation()
    {
        helperTitle.text = farmExplanationTitle;
        helperText.text = farmExplanationText;
    }

    public void ComposterExplanation()
    {
        helperTitle.text = composterExplanationTitle;
        helperText.text = composterExplanationText;
    }

    public void WoodFactoryExplanation()
    {
        helperTitle.text = woodFactoryExplanationTitle;
        helperText.text = woodFactoryExplanationText;
    }

    public void WoolFactoryExplanation()
    {
        helperTitle.text = woolFactoryExplanationTitle;
        helperText.text = woolFactoryExplanationText;
    }

    public void GlueBlockExplanation()
    {
        helperTitle.text = glueBlockExplanationTitle;
        helperText.text = glueBlockExplanationText;
    }

    public void SimpleBlockExplanation()
    {
        helperTitle.text = simpleBlockExplanationTitle;
        helperText.text = simpleBlockExplanationText;
    }

    public void RotatingExplanation()
    {
        helperTitle.text = rotatingExplanationTitle;
        helperText.text = rotatingExplanationText;
    }

    public void MovingExplanation()
    {
        helperTitle.text = movingExplanationTitle;
        helperText.text = movingExplanationText;
    }




}
