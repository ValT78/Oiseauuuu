using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BonusIndicator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private RectTransform rectTransformText;
    [SerializeField] private RectTransform rectTransformIcon;
    [SerializeField] private float speed;
    [SerializeField] private float timeToLive;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, timeToLive);
    }

    // Update is called once per frame
    void Update()
    {
        //rectTransformIcon.position += Vector3.up * speed * Time.deltaTime;
        rectTransformText.position += Vector3.up * speed * Time.deltaTime;
    }

    public void Initialize (int bonus, Vector3 housePosition)
    {
        Debug.Log("Bonus: " + bonus);
        if (bonus != 0)
        {
            text.text = "+" + bonus.ToString();
        }
        else
        {
            text.text = "Not enough feeded citizens";
        }
        rectTransformText.position = Camera.main.WorldToScreenPoint(housePosition) + new Vector3(30, 0, 0);
        //rectTransformIcon.position = Camera.main.WorldToScreenPoint(housePosition) + new Vector3(-30, 0, 0);

    }
}
