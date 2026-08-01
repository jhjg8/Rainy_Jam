using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{
    public static Field Instance { get; private set; }

    [Header("Field Settings")]
    public Transform fieldCenterPoint;    // The center point of your field
    public float maxFieldLength = 10f;    // The maximum total width the cards are allowed to take up
    public float maxCardSpacing = 2f;     // The default space between cards before the field gets crowded
    public float popUpOffset = 2f;      // How high the card pops up when toggled

    [Header("Cards List")]
    public List<CardObj> cardsOnField = new List<CardObj>();
    private List<CardObj> poppedCards = new List<CardObj>(); // Tracks which cards are currently popped up

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Adds a card to the field and reorganizes the layout.
    /// </summary>
    public void AddCardToField(CardObj card)
    {
        if (!cardsOnField.Contains(card))
        {
            cardsOnField.Add(card);
            card.Pos = CardManager.position.field; // Update card's state to field
            OrganizeField();
            EventBus<CardMoveToField>.Raise(new CardMoveToField { Card = card.card });
        }
    }

    /// <summary>
    /// Removes a card from the field and reorganizes the layout.
    /// </summary>
    public void RemoveCardFromField(CardObj card)
    {
        if (cardsOnField.Contains(card))
        {
            cardsOnField.Remove(card);
            if (poppedCards.Contains(card))
            {
                poppedCards.Remove(card);
            }
            OrganizeField();
            EventBus<CardMoveOutOfField>.Raise(new CardMoveOutOfField { Card = card.card });
        }
    }

    /// <summary>
    /// Toggles the pop-up state of a specific card (moves it slightly up/forward).
    /// </summary>
    public void TogglePopCard(CardObj card)
    {
        if (!cardsOnField.Contains(card)) return;

        if (poppedCards.Contains(card))
        {
            poppedCards.Remove(card); // Un-pop
        }
        else
        {
            poppedCards.Add(card);    // Pop up
        }

        OrganizeField();
    }

    /// <summary>
    /// Recalculates Belong and BelongR for all cards on the field to fit within the max length.
    /// </summary>
    public void OrganizeField()
    {
        int count = cardsOnField.Count;
        if (count == 0) return;

        Vector3 centerPos = fieldCenterPoint != null ? fieldCenterPoint.position : transform.position;

        // 1. Calculate Spacing
        // Default to maxCardSpacing, but shrink it if the cards would exceed maxFieldLength
        float currentSpacing = maxCardSpacing;
        if (count > 1)
        {
            float requiredWidth = (count - 1) * maxCardSpacing;
            if (requiredWidth > maxFieldLength)
            {
                currentSpacing = maxFieldLength / (count - 1);
            }
        }

        // 2. Calculate starting X position so the entire line stays centered
        float startX = -(currentSpacing * (count - 1)) / 2f;

        for (int i = 0; i < count; i++)
        {
            CardObj card = cardsOnField[i];

            // 3. Apply Y offset if the card is "popped up"
            float yOffset = poppedCards.Contains(card) ? popUpOffset : 0f;

            // Target Position (Straight line, overlapping slightly on Z axis to prevent Z-fighting)
            Vector3 targetPosition = centerPos + new Vector3(startX + (i * currentSpacing), yOffset, i * 0.01f);

            // Target Rotation (Completely flat, no fan effect)
            Vector3 targetRotation = Vector3.zero;

            // Set variables on CardObj
            card.Belong = targetPosition;
            card.BelongR = targetRotation;

            // Trigger DOTween smooth move inside CardObj
            if (card.canmove)
            {
                card.ResetPos();
            }
        }
    }
}