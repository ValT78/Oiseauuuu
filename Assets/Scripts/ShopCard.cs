using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopCard : MonoBehaviour
{
    [SerializeField] private TMP_Text text_title;
    [SerializeField] private TMP_Text text_wool;
    [SerializeField] private TMP_Text text_wood;
    [SerializeField] private TMP_Text text_compost;
    public RectTransform rectTransform;
    [SerializeField] private List<GameObject> buildingPrefabs; // Référence à l'objet BlockGenerator
    [HideInInspector] public GameObject blockGenerator; // Référence à l'objet BlockGenerator

    [Header("Animation Parameters")]
    [SerializeField] private float inOutAnimationDuration;
    Vector3 inOutStartPosition;
    [HideInInspector] public Vector3 inOutEndPosition;
    private bool isGoing = false;

    [Header("Selection Parameters")]
    [SerializeField] private float selectDuration;

    [Header("Choose Parameters")]
    [SerializeField] private float growDuration;
    [SerializeField] private float shrinkDuration;
    private bool isChosen = false;

    void Start()
    {
        // Instancier l'objet BlockGenerator et le placer au centre de la carte
        blockGenerator = Instantiate(GenerateRandomBlockType());
        blockGenerator.transform.localPosition = Vector3.zero;
        StartCoroutine(LateStart());
    }

    private IEnumerator LateStart()
    {
        yield return new WaitForEndOfFrame();
        BlockGenerator component = blockGenerator.GetComponent<BlockGenerator>();
        SetUpCard(((int)component.buildingType), component.woolCost, component.woodCost, component.compostCost);

    }

    void Update()
    {
        if (!isChosen)
        {
            // Mettre à jour la position de l'objet BlockGenerator pour qu'il reste centré sur la carte
            Vector3 screenPoint = rectTransform.TransformPoint(rectTransform.rect.center);
            screenPoint.z = 0; // Assurez-vous que l'objet reste sur le plan XY
            blockGenerator.transform.position = screenPoint;
        }
    }

    private GameObject GenerateRandomBlockType()
    {
        return buildingPrefabs[Random.Range(0, buildingPrefabs.Count)];
        
    }

    public void UpdateBlockSize(float size)
    {
        // Sauvegarde les positions locales des enfants
        Vector3[] childrenLocalPositions = new Vector3[blockGenerator.transform.childCount];
        for (int i = 0; i < blockGenerator.transform.childCount; i++)
        {
            childrenLocalPositions[i] = blockGenerator.transform.GetChild(i).localPosition;
        }

        // Modifie la scale de l'objet
        blockGenerator.transform.localScale = new Vector3(size, size, 1);

        // Réinitialise les positions locales des enfants
        for (int i = 0; i < blockGenerator.transform.childCount; i++)
        {
            blockGenerator.transform.GetChild(i).localPosition = childrenLocalPositions[i];
        }
    }

    public void SetUpCard(int buildingType , int woolCount, int woodCount, int compostCount)
    {
        text_wool.text = woolCount.ToString();
        text_wood.text = woodCount.ToString();
        text_compost.text = compostCount.ToString();
        text_title.text = ((BlockGenerator.BuildingType)buildingType).ToString();
    }

    public void SetAnimationParameters(Vector2 startPosition, Vector2 endPosition)
    {
        inOutStartPosition = startPosition;
        inOutEndPosition = endPosition;
    }

    public IEnumerator AnimateCardIn()
    {
        float elapsedTime = 0f;

        while (elapsedTime < inOutAnimationDuration)
        {
            if (isGoing || isChosen)
            {
                rectTransform.anchoredPosition = inOutEndPosition;
                yield break;
            }
            rectTransform.anchoredPosition = Vector3.Lerp(inOutStartPosition, inOutEndPosition, elapsedTime / inOutAnimationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rectTransform.anchoredPosition = inOutEndPosition;
    }

    public IEnumerator AnimateCardOut()
    {
        
        isGoing = true;
        float elapsedTime = 0f;

        while (elapsedTime < inOutAnimationDuration)
        {
            if (isGoing || isChosen)
            {
                Destroy(blockGenerator);
                Destroy(gameObject);
                yield break;
            }
            rectTransform.anchoredPosition = Vector3.Lerp(inOutEndPosition, inOutStartPosition, elapsedTime / inOutAnimationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Destroy(blockGenerator);
        Destroy(gameObject);

    }

    public IEnumerator AnimateSelectedCard(float startScale, float endScale, Vector2 startPosition, Vector2 endPosition)
    {
        
        float elapsedTime = 0f;

        while (elapsedTime < selectDuration)
        {
            if (isGoing || isChosen)
            {
                yield break;
            }
            float t = elapsedTime / selectDuration;
            rectTransform.localScale = Vector3.Lerp(new Vector3(startScale, startScale, 1), new Vector3(endScale, endScale, 1), t);
            UpdateBlockSize(rectTransform.localScale.x);
            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, endPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rectTransform.localScale = new Vector3(endScale, endScale, 1);
        rectTransform.anchoredPosition = endPosition;
    }

    public IEnumerator AnimateChoose()
    {
        
        isChosen = true;

        float elapsedTime = 0f;

        // Phase de croissance
        while (elapsedTime < growDuration)
        {
            if (isChosen || isGoing)
            {
                UpdateBlockSize(1);
                Destroy(gameObject);
                yield break;
            }
            float t = elapsedTime / growDuration;
            rectTransform.localScale = Vector3.Lerp(Vector3.one, new Vector3(1.5f, 1.5f, 1), t); // Ajustez 1.5f pour la taille de croissance souhaitée
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Réinitialiser le temps écoulé pour la phase de rétrécissement
        elapsedTime = 0f;

        // Phase de rétrécissement
        while (elapsedTime < shrinkDuration)
        {
            if (isChosen || isGoing)
            {
                UpdateBlockSize(1);
                Destroy(gameObject);
                yield break;
            }
            float t = elapsedTime / shrinkDuration;
            rectTransform.localScale = Vector3.Lerp(new Vector3(1.5f, 1.5f, 1), Vector3.zero, t);
            UpdateBlockSize(rectTransform.localScale.x);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        UpdateBlockSize(1);

        // Détruire le GameObject après l'animation
        Destroy(gameObject);
    }
}
