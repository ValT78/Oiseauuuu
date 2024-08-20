using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using UnityEngine;
using static BlockGenerator;
using System.ComponentModel;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    [SerializeField] private GameObject shopCardPrefab;
    [SerializeField] private RectTransform parentTransform;
    private List<ShopCard> shopCards = new();
    [HideInInspector] public bool isBuyable = false;

    [Header("Card Space Parameters")]
    [SerializeField] private float cardSpacing;
    [SerializeField] private int numberOfCards; // Nombre de cartes � afficher
    private float cardHeight;
    private float cardScale; // Calculer l'�chelle des cartes (la moiti� de l'�cran)

    [Header("Selected Card Parameters")]
    [SerializeField] private float selectedCardScale; // �chelle de la carte s�lectionn�e
    private Vector2 centerPosition; // Position centrale de l'�cran
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
        if(!isBuyable)
        {
            shopCards.Clear();
            cardScale = ((1080 - cardSpacing) / numberOfCards - cardSpacing) / cardHeight;

            float startY = 540 - cardSpacing - cardHeight * cardScale / 2; // Position de d�part en haut de l'�cran

            for (int i = 0; i < numberOfCards; i++)
            {
                GameObject newCard = Instantiate(shopCardPrefab, parentTransform);
                if (i == numberOfCards - 1)
                {
                    newCard.GetComponent<ShopCard>().Initialize(true);
                }
                else
                {
                    newCard.GetComponent<ShopCard>().Initialize(false);
                }
                RectTransform cardRectTransform = newCard.GetComponent<RectTransform>();

                // Ajustez l'�chelle de la carte
                cardRectTransform.localScale = new Vector3(cardScale, cardScale, 1);



                float posY = startY - i * (cardHeight * cardScale + cardSpacing);
                cardRectTransform.anchoredPosition = new Vector2(960 + cardHeight * cardScale, posY); // Position de d�part hors �cran
                ShopCard shopCard = newCard.GetComponent<ShopCard>();
                shopCard.SetAnimationParameters(new Vector3(960 + cardHeight * cardScale, posY, 0), new Vector3(960 - cardHeight * cardScale / 2, posY, 0));
                shopCard.ID = i + 1;
                shopCards.Add(shopCard);
                StartCoroutine(shopCard.AnimateCardIn());
            }
            isBuyable = true;
        }
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
                HelperCanvasScript.Instance.RotatingExplanation();
            }
        }
        else {
            if (selectedCard != null)
            {
                // R�initialiser la carte pr�c�demment s�lectionn�e avec animation
                StartCoroutine(selectedCard.AnimateSelectedCard(selectedCardScale, cardScale, centerPosition, selectedCard.inOutEndPosition));
            }

            // Mettre en �vidence la nouvelle carte s�lectionn�e avec animation
            selectedCard = card;
            StartCoroutine(selectedCard.AnimateSelectedCard(cardScale, selectedCardScale, selectedCard.inOutEndPosition, centerPosition));
            switch (card.blockGenerator.GetComponent<BlockGenerator>().buildingType) 
            {
                case BuildingType.GlueBlock:
                    HelperCanvasScript.Instance.GlueBlockExplanation();
                    break;
                case BuildingType.WoodFactory:
                    HelperCanvasScript.Instance.WoodFactoryExplanation();
                    break;
                case BuildingType.WoolFactory:
                    HelperCanvasScript.Instance.WoolFactoryExplanation();
                    break;
                case BuildingType.Composter:
                    HelperCanvasScript.Instance.ComposterExplanation();
                    break;
                case BuildingType.CropFields:
                    HelperCanvasScript.Instance.FarmExplanation();
                    break;
                case BuildingType.House:
                    HelperCanvasScript.Instance.HouseExplanation();
                    break;
                case BuildingType.SimpleBlock:
                    HelperCanvasScript.Instance.SimpleBlockExplanation();
                    break;
            }

        }

        
    }

    private void choseCard(int cardIndex)
    {
        if (!isBuyable) return;
        AudioManager.Instance.PlaySelection();
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
