using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BonusIndicator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
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
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    public void Initialize (int bonus)
    {
        text.text = "+"+bonus.ToString();
    }
}
