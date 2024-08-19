using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
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
    private Vector2 centerPosition; // Position centrale de l'écran
    private ShopCard selectedCard = null;

    /*
    private readonly Dictionary<KeyCode, int> keyToCardIndex = new()
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
    */

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
        cardHeight = shopCardPrefab.GetComponent<RectTransform>().rect.height;
        centerPosition = new(960-cardHeight * 1.5f, 0);
        InitializeShop(); // Exemple d'initialisation avec 5 cartes
    }

    void Update()
    {
        /*
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
        */
    }

    public void InitializeShop()
    {
        shopCards.Clear();
        cardScale = ((1080 - cardSpacing) / numberOfCards - cardSpacing) / cardHeight;

        float startY = 540 - cardSpacing - cardHeight*cardScale / 2; // Position de départ en haut de l'écran

        for (int i = 0; i < numberOfCards; i++)
        {
            GameObject newCard = Instantiate(shopCardPrefab, parentTransform);
            if(i == numberOfCards-1)
            {
                newCard.GetComponent<ShopCard>().Initialize(true);
            }
            else
            {
                newCard.GetComponent<ShopCard>().Initialize(false);
            }
            RectTransform cardRectTransform = newCard.GetComponent<RectTransform>();
            
            // Ajustez l'échelle de la carte
            cardRectTransform.localScale = new Vector3(cardScale, cardScale, 1);

            float posY = startY - i * (cardHeight * cardScale + cardSpacing);
            cardRectTransform.anchoredPosition = new Vector2(Screen.width + cardHeight * cardScale, posY); // Position de départ hors écran

            ShopCard shopCard = newCard.GetComponent<ShopCard>();
            shopCard.SetAnimationParameters(new Vector3(Screen.width + cardHeight * cardScale, posY, 0), new Vector3(Screen.width - cardHeight * cardScale/2, posY, 0));

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
                card.blockGenerator.GetComponent<BlockGenerator>().canBeDestroyed = true;
                isBuyable = false;
                StartCoroutine(card.AnimateChoose(centerPosition));
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

    private void choseCard(int cardIndex)
    {
        if (!isBuyable) return;
        if (cardIndex >= shopCards.Count) return;
        SelectCard(shopCards[cardIndex]);
    }

    public void Chose1(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started) return;
        choseCard(0);
    }
    public void Chose2(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started) return;
        choseCard(1);
    }
    public void Chose3(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started) return;
        choseCard(2);
    }
    public void Chose4(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started) return;
        choseCard(3);
    }
    public void Chose5(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started) return;
        choseCard(4);
    }


}
