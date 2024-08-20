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
    private string defaultText = "Press 1, 2, 3 or 4 num keys, to select a building, then press the same keys to start placing the building. move the camera with the arrows keys";

    private string houseExplanationTitle = "House";
    [TextArea(3, 10)]
    private string houseExplanationText = "Houses are used to increase the population of your city. The more houses you have, the more people will live in your city.";

    private string farmExplanationTitle = "Farm";
    [TextArea(3, 10)]
    private string farmExplanationText = "Farms are used to produce food. The more farms you have, the more food you will have to feed the population.";

    private string composterExplanationTitle = "Composter";
    [TextArea(3, 10)]
    private string composterExplanationText = "Composters are used to produce compost. The more composters you have, the more compost you will produce.\nRessources are produced after each blocks placed and depend on feeded population";

    private string woodFactoryExplanationTitle = "Wood Factory";
    [TextArea(3, 10)]
    private string woodFactoryExplanationText = "Wood Factories are used to produce wood. The more wood factories you have, the more wood you will produce.\nRessources are produced after each blocks placed and depend on feeded population";

    private string woolFactoryExplanationTitle = "Wool Factory";
    [TextArea(3, 10)]
    private string woolFactoryExplanationText = "Wool Factories are used to produce wool. The more wool factories you have, the more wool you will produce.\nRessources are produced after each blocks placed and depend on feeded population";

    private string glueBlockExplanationTitle = "Glue block";
    [TextArea(3, 10)]
    private string glueBlockExplanationText = "Glue blocks is just used for placement, it does not produce anything. It glue to surface and is used to build on top of it";

    private string simpleBlockExplanationTitle = "Simple block";
    [TextArea(3, 10)]
    private string simpleBlockExplanationText = "Simple blocks is just used for placement, it does not produce anything but is free. It is used to build on top of it";

    private string rotatingExplanationTitle = "Rotating";
    [TextArea(3, 10)]
    private string rotatingExplanationText = "Rotate the building by pressing Q or E (phsyical keys, qwerty reference). \nPress SPACE for the block to start falling. \nMove the camera with the arrows keys";

    private string movingExplanationTitle = "Moving";
    [TextArea(3, 10)]
    private string movingExplanationText = "Move the building by pressing A, S or D (phsyical keys, qwerty reference). \nMove the camera with the arrows keys";



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
