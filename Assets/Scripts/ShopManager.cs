using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    [SerializeField] private GameObject shopCardPrefab;
    [SerializeField] private RectTransform parentTransform;
    private List<ShopCard> shopCards = new List<ShopCard>();
    private bool isBuyable = false;

    [Header("Card Space Parameters")]
    [SerializeField] private float cardSpacing;
    [SerializeField] private int numberOfCards; // Nombre de cartes à afficher
    private float cardHeight;
    private float cardScale; // Calculer l'échelle des cartes (la moitié de l'écran)

    [Header("Selected Card Parameters")]
    [SerializeField] private float selectedCardScale; // Échelle de la carte sélectionnée
    [SerializeField] private Vector2 centerPosition; // Position centrale de l'écran
    private ShopCard selectedCard = null;


    private Dictionary<KeyCode, int> keyToCardIndex = new Dictionary<KeyCode, int>
    {
        { KeyCode.Q, 0 },
        { KeyCode.S, 1 },
        { KeyCode.D, 2 },
        { KeyCode.F, 3 },
        { KeyCode.G, 4 },
        { KeyCode.H, 5 },
        { KeyCode.J, 6 },
        { KeyCode.K, 7 },
        { KeyCode.L, 8 },
        { KeyCode.M, 9 }
    };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        cardHeight = shopCardPrefab.GetComponent<RectTransform>().rect.height;
        cardScale = ((Screen.width - cardSpacing) / numberOfCards - cardSpacing) / cardHeight;
        InitializeShop(); // Exemple d'initialisation avec 5 cartes
    }

    void Update()
    {
        if (isBuyable)
        {
            foreach (var key in keyToCardIndex.Keys)
            {
                if (Input.GetKeyDown(key))
                {
                    int cardIndex = keyToCardIndex[key];
                    if (cardIndex < shopCards.Count)
                    {
                        SelectCard(shopCards[cardIndex]);

                    }
                }
            }
        }
    }

    public void InitializeShop()
    {
        shopCards.Clear();
        float startY = 540 - cardSpacing - cardHeight / 2; // Position de départ en haut de l'écran

        for (int i = 0; i < numberOfCards; i++)
        {
            GameObject newCard = Instantiate(shopCardPrefab, parentTransform);
            RectTransform cardRectTransform = newCard.GetComponent<RectTransform>();
            

            // Ajustez l'échelle de la carte
            cardRectTransform.localScale = new Vector3(cardScale, cardScale, 1);

            float posY = startY - i * (cardHeight * cardScale + cardSpacing);
            cardRectTransform.anchoredPosition = new Vector2(Screen.width, posY); // Position de départ hors écran

            ShopCard shopCard = newCard.GetComponent<ShopCard>();
            shopCard.SetUpCard("Card " + (i + 1), i, i + 1, i + 2);
            shopCard.SetAnimationParameters(new Vector3(Screen.width, posY, 0), new Vector3(480, posY, 0));

            shopCards.Add(shopCard);
            StartCoroutine(shopCard.AnimateCardIn());
        }
        isBuyable = true;
    }

    private void SelectCard(ShopCard card)
    {
        if(card == selectedCard)
        {
            if(card.blockGenerator.GetComponent<BlockMouseFollower>().TryBuyCard())
            {
                isBuyable = false;
                StartCoroutine(card.AnimateChoose());
                foreach (ShopCard shopCard in shopCards)
                {
                    if (shopCard != card)
                    {
                        StartCoroutine(shopCard.AnimateCardOut());
                    }
                }   
            }
        }
        else {
            if (selectedCard != null)
            {
                // Réinitialiser la carte précédemment sélectionnée avec animation
                StartCoroutine(selectedCard.AnimateSelectedCard(selectedCardScale, cardScale, centerPosition, selectedCard.inOutEndPosition));
            }

            // Mettre en évidence la nouvelle carte sélectionnée avec animation
            selectedCard = card;
            StartCoroutine(selectedCard.AnimateSelectedCard(cardScale, selectedCardScale, selectedCard.inOutEndPosition, centerPosition));
        }

        
    }

    
}
