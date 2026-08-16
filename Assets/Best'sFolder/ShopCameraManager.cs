using DG.Tweening;
using EasyTransition;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class ShopCameraManager : MonoBehaviour
{
    public static ShopCameraManager Instance;

    [Header("Camera Control")]
    public Camera mainCamera;

    [Header("Camera Positions")]
    public Transform gameCameraPos;
    public Transform shopCameraPos;

    [Header("UI Panels")]
    public GameObject shopUIPanel;
    public GameObject fightUIPanel; // NEW: Reference to your normal game/combat UI

    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    private Transform targetPos;
    public bool isInShop = false;

    public TransitionSettings transitionSettings;
    TransitionManager manager;

    private void Awake()
    {
        Instance = this;
        if (mainCamera == null) mainCamera = GetComponent<Camera>();
        if (mainCamera == null) mainCamera = Camera.main;
    }

    private void Start()
    {
        if (gameCameraPos != null)
        {
            targetPos = gameCameraPos;
            transform.position = gameCameraPos.position;
            transform.rotation = gameCameraPos.rotation;
        }

        // Setup starting UI states (Start in Fight phase)
        if (shopUIPanel != null) shopUIPanel.SetActive(false);
        if (fightUIPanel != null) fightUIPanel.SetActive(true); // Ensure fight UI is on

        if (mainCamera != null)
            mainCamera.orthographic = true;
    }

    private void Update()
    {
        if (targetPos == null) return;

        transform.position = Vector3.Lerp(transform.position, targetPos.position, Time.deltaTime * moveSpeed);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetPos.rotation, Time.deltaTime * moveSpeed);
    }

    public void ToggleShop()
    {
        if (isInShop)
        {
            manager = TransitionManager.Instance();

            manager.onTransitionCutPointReached += () =>
            {
                GoToGame();
            };

            manager.Transition(transitionSettings, 3f);
            SoundManager.Instance.ChangeBgm(1);
        }
        else
        {
            manager = TransitionManager.Instance();

            manager.onTransitionCutPointReached += () =>
            {
                GoToShop();
            };

            manager.Transition(transitionSettings, 3f);
            SoundManager.Instance.ChangeBgm(0);
        }
    }

    public void GoToShop()
    {
        if (shopCameraPos == null) return;

        targetPos = shopCameraPos;
        isInShop = true;

        if (mainCamera != null)
            mainCamera.orthographic = false;

        // Toggle UI panels
        if (shopUIPanel != null) shopUIPanel.SetActive(true);
        if (fightUIPanel != null) fightUIPanel.SetActive(false); // Hide fight UI

        if (ShopFocusManager.Instance != null)
            ShopFocusManager.Instance.Unfocus();

        if (ShopManager.Instance != null)
        {
            ShopManager.Instance.GenerateShopItems();
        }
    }

    public void GoToGame()
    {
        if (gameCameraPos == null) return;

        targetPos = gameCameraPos;
        isInShop = false;

        if (mainCamera != null)
            mainCamera.orthographic = true;

        // Toggle UI panels
        if (shopUIPanel != null) shopUIPanel.SetActive(false);
        if (fightUIPanel != null) fightUIPanel.SetActive(true); // Show fight UI

        if (ShopManager.Instance != null)
        {
            ShopManager.Instance.ClearShop();
        }
    }

    public bool IsInShop() => isInShop;
}