using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HandOrganizer : MonoBehaviour
{
    public static HandOrganizer Instance { get; private set; }

    [Header("Hand Settings")]
    public Transform handCenterPoint; // Pivot point where the hand expands from
    public float cardSpacing = 1.2f;   // Horizontal space between cards
    public float selectionOffset = 0.5f; // How high a card pops up when selected
    public float fanAngle = 3f;        // Rotation fan effect for Balatro look
    public float curveHeight = 0.1f;    // Arch height for Balatro curved layout

    [Header("Selection Rules")]
    public int maxSelectableCards = 8;  // Balatro limit (e.g., 5 cards max for playing/discarding)

    [Header("Cards List")]
    public List<CardObj> cardsInHand = new List<CardObj>();
    private List<CardObj> selectedCards = new List<CardObj>();

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
    /// Adds a card to the hand and triggers layout reorganization.
    /// </summary>
    public void AddCardToHand(CardObj card)
    {
        if (!cardsInHand.Contains(card))
        {
            cardsInHand.Add(card);
            card.Pos = CardManager.position.hand;
            OrganizeHand();
        }
    }

    /// <summary>
    /// Removes a card from the hand and reorganizes remaining cards.
    /// </summary>
    public void RemoveCardFromHand(CardObj card)
    {
        if (cardsInHand.Contains(card))
        {
            cardsInHand.Remove(card);
            if (selectedCards.Contains(card))
            {
                selectedCards.Remove(card);
            }
            OrganizeHand();
        }
    }

    /// <summary>
    /// Handles the selection/deselection click logic.
    /// Call this when a card is clicked without dragging.
    /// </summary>
    public void ToggleSelectCard(CardObj card)
    {
        if (selectedCards.Contains(card))
        {
            // Second click on selected card -> Deselect
            selectedCards.Remove(card);
        }
        else
        {
            // First click on unselected card -> Select (if limit not reached)
            if (selectedCards.Count < maxSelectableCards)
            {
                selectedCards.Add(card);
            }
            else
            {
                Debug.Log("Selection limit reached!");
                return;
            }
        }

        // Refresh positions so selected card pops up/down
        OrganizeHand();
    }

    /// <summary>
    /// Recalculates Belong and BelongR for all cards in hand and triggers ResetPos().
    /// </summary>
    public void OrganizeHand()
    {
        int count = cardsInHand.Count;
        if (count == 0) return;

        Vector3 centerPos = handCenterPoint != null ? handCenterPoint.position : transform.position;

        for (int i = 0; i < count; i++)
        {
            CardObj card = cardsInHand[i];

            // 1. Calculate X offset relative to center
            float xOffset = (i - (count - 1) / 2.0f) * cardSpacing;

            // 2. Balatro Arch/Curve calculation (cards in middle are slightly higher)
            float normalizedIndex = (count > 1) ? (i - (count - 1) / 2.0f) : 0f;
            float yCurve = -Mathf.Pow(normalizedIndex, 2) * curveHeight;

            // 3. Selection vertical offset
            float ySelectOffset = selectedCards.Contains(card) ? selectionOffset : 0f;

            // Target Position
            Vector3 targetPosition = centerPos + new Vector3(xOffset, yCurve + ySelectOffset, -i * 0.01f);

            // 4. Balatro Fan Rotation
            float zRotation = -normalizedIndex * fanAngle;
            Vector3 targetRotation = new Vector3(0, 0, zRotation);

            // Set variables on CardObj
            card.Belong = targetPosition;
            card.BelongR = targetRotation;

            // Trigger smooth move via DOTween in CardObj
            if (card.canmove)
            {
                card.ResetPos();
            }
        }
    }

    /// <summary>
    /// Call this after dragging a card to re-insert it into the hand based on its released X-position.
    /// </summary>
    public void ReorderCardByPosition(CardObj draggedCard)
    {
        if (!cardsInHand.Contains(draggedCard)) return;

        // Remove temporarily to recalculate index
        cardsInHand.Remove(draggedCard);

        int newIndex = 0;
        for (int i = 0; i < cardsInHand.Count; i++)
        {
            if (draggedCard.transform.position.x > cardsInHand[i].transform.position.x)
            {
                newIndex = i + 1;
            }
        }

        // Insert at new index and refresh hand layout
        cardsInHand.Insert(newIndex, draggedCard);
        OrganizeHand();
    }

    /// <summary>
    /// UI Button Event: Arranges cards in hand by Rank.
    /// </summary>
    public void ArrangeByRank()
    {
        // Assuming card.Rank is comparable or castable to int/enum
        cardsInHand = cardsInHand.OrderBy(c => c.card.Rank).ToList();
        OrganizeHand();
    }

    /// <summary>
    /// UI Button Event: Arranges cards in hand by Suit (Optional bonus).
    /// </summary>
    public void ArrangeBySuit()
    {
        cardsInHand = cardsInHand.OrderBy(c => c.card.suits.FirstOrDefault()?.Name).ToList();
        OrganizeHand();
    }
}