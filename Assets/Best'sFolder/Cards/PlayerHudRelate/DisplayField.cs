using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayField : MonoBehaviour
{
    public static DisplayField Instance { get; private set; }

    [Header("Display Layout Settings")]
    public Transform displayCenterPoint;
    public float maxDisplayLength = 10f;
    public float maxCardSpacing = 2f;

    [Header("Default Timing Settings")]
    public float defaultInitialWait = 1f; // How long to wait before starting the loop
    public float defaultLoopSpeed = 0.3f; // Delay between processing each card

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
    /// Arranges a list of cards, waits a moment, processes them one by one, then fires an OnComplete action.
    /// </summary>
    public void DisplayAndProcess(List<CardObj> cardsToDisplay, Action<CardObj> onCardProcess, Action<List<CardObj>> onComplete)
    {
        if (cardsToDisplay == null || cardsToDisplay.Count == 0)
        {
            // If the list is empty, just trigger completion immediately
            onComplete?.Invoke(cardsToDisplay);
            return;
        }

        StartCoroutine(DisplayRoutine(cardsToDisplay, onCardProcess, onComplete));
    }

    private IEnumerator DisplayRoutine(List<CardObj> cards, Action<CardObj> onCardProcess, Action<List<CardObj>> onComplete)
    {
        int count = cards.Count;
        Vector3 centerPos = displayCenterPoint != null ? displayCenterPoint.position : transform.position;

        // 1. Calculate Spacing (Same layout math to keep them inside max length)
        float currentSpacing = maxCardSpacing;
        if (count > 1)
        {
            float requiredWidth = (count - 1) * maxCardSpacing;
            if (requiredWidth > maxDisplayLength)
            {
                currentSpacing = maxDisplayLength / (count - 1);
            }
        }

        float startX = -(currentSpacing * (count - 1)) / 2f;

        // 2. Assign positions and trigger DOTween move
        for (int i = 0; i < count; i++)
        {
            CardObj card = cards[i];

            Vector3 targetPosition = centerPos + new Vector3(startX + (i * currentSpacing), 0f, -i * 0.01f);

            card.Belong = targetPosition;
            card.BelongR = Vector3.zero;

            if (card.canmove)
            {
                card.ResetPos();
            }
        }

        // 3. Wait a short time for cards to reach their display positions
        yield return new WaitForSeconds(defaultInitialWait);

        // 4. Loop through every card, execute the method, and wait
        for (int i = 0; i < count; i++)
        {
            // Invoke the specific method for this card
            onCardProcess?.Invoke(cards[i]);

            // Wait for the assigned loop speed before going to the next card
            yield return new WaitForSeconds(defaultLoopSpeed);
        }

        // 5. Finished looping! Call the OnComplete function
        onComplete?.Invoke(cards);
    }
}