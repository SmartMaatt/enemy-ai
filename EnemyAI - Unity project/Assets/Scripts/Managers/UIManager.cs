﻿using UnityEngine;
using TMPro;
using System;

public class UIManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status { get; private set; }

    private NonePanelControll nonePanel;

    [Header("UI")]
    [SerializeField] private GameObject UIObject;

    [Header("Left panel")]
    [SerializeField] private GameObject leftPanelOwner;
    [SerializeField] private RealPanelControll leftPanel;

    [Header("Right panel")]
    [SerializeField] private GameObject rightPanelOwner;
    [SerializeField] private RealPanelControll rightPanel;

    [Header("Element selector")]
    [SerializeField] private GameObject elementBarOwner;
    [SerializeField] private ElementBar elementBar;

    [Header("Elements sprites")]
    [SerializeField] private Sprite fire;
    [SerializeField] private Sprite water;
    [SerializeField] private Sprite snow;

    [Header("PopUp display panel")]
    [SerializeField] private PopUpMessager popUpMessager;

    [Header("Episode time")]
    [SerializeField] private ValueBar episodeTimeBar;

    [Header("Labels")]
    [SerializeField] private TMP_Text generationLabel;
    [SerializeField] private TMP_Text FPSLabel;
    [SerializeField] private float fpsCounterTime = 0f;
    [SerializeField] private float fpsIntervalTime = 0.5f;

    [Header("LockScrean")]
    [SerializeField] private LockScrean lockScreen;

    [Header("Escape menu")]
    [SerializeField] private GameObject escapeMenuScreen;
    [SerializeField] private GameObject giveUpButton;

    [Header("Loading screan")]
    [SerializeField] private GameObject loadingScreen;

    private bool escapeMenu;
    private bool lockState;
    private bool visibleGui;
    private bool fpsCounter;

    public void Startup()
    {
        Debug.Log("Starting UI manager");

        nonePanel = new NonePanelControll();
        leftPanel.gameObject.SetActive(false);
        rightPanel.gameObject.SetActive(false);

        elementBar.SetupBarSprites(fire, water, snow);
        elementBar.gameObject.SetActive(false);

        ActivateGiveUpButton(false);
        ActivateFPSCounter(false);
        CloseEscapeMenu();
        visibleGui = true;

        status = ManagerStatus.Started;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ActivateEscapeMenu();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            ToggleLockState();
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            ToggleVisibleGui();
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            ToggleFPSCounter();
        }
        CalculateCurrentFPSCount();
    }

    public void LockApp(string reason)
    {
        SetLockScreanActive(true);
        SetLockScreanReason(reason);

        CloseEscapeMenu();
        LockCursor(false);

        enabled = false;
    }

    public PanelControll SetupUIPanelController(GameObject owner, PanelType type)
    {
        if (type == PanelType.LEFT)
        {
            if (leftPanelOwner == null)
            {
                SetLeftPanelOwner(owner);
                return leftPanel;
            }

            if (leftPanelOwner == owner)
            {
                return leftPanel;
            }
        }

        if (type == PanelType.RIGHT)
        {
            if (rightPanelOwner == null)
            {
                SetRightPanelOwner(owner);
                return rightPanel;
            }

            if (rightPanelOwner == owner)
            {
                return rightPanel;
            }
        }

        return nonePanel;
    }

    public ElementBar SetupElementBar(GameObject owner)
    {
        if (elementBarOwner == null)
        {
            SetElementBarOwner(owner);
            return elementBar;
        }

        if (elementBarOwner == owner)
        {
            return elementBar;
        }

        return null;
    }

    private void SetLeftPanelOwner(GameObject owner)
    {
        leftPanelOwner = owner;
        leftPanel.gameObject.SetActive(true);
    }

    private void SetRightPanelOwner(GameObject owner)
    {
        rightPanelOwner = owner;
        rightPanel.gameObject.SetActive(true);
    }

    private void SetElementBarOwner(GameObject owner)
    {
        elementBarOwner = owner;
        elementBar.gameObject.SetActive(true);
    }

    public void RemovePanelOwner(GameObject owner, PanelType panelType)
    {
        if (panelType == PanelType.LEFT && leftPanelOwner == owner)
        {
            leftPanelOwner = null;
            leftPanel.gameObject.SetActive(false);
        }
        else if (panelType == PanelType.RIGHT && rightPanelOwner == owner)
        {
            rightPanelOwner = null;
            rightPanel.gameObject.SetActive(false);
        }
    }

    public void RemoveElementBarOwner(GameObject owner)
    {
        if (elementBarOwner == owner)
        {
            elementBar.gameObject.SetActive(false);
            elementBarOwner = null;
        }
    }

    public void DisplayPopUpMessage(string msg)
    {
        popUpMessager.DisplayMessage(msg);
    }

    public void DisplayPopUpMessageWithTime(string msg, float time)
    {
        popUpMessager.SetTimeOfDisplay(time);
        popUpMessager.DisplayMessage(msg);
    }

    public void SetupGenerationLabel(int generationValue, int iterationValue)
    {
        generationLabel.text = "Generacja: " + generationValue + " | Iteracja: " + iterationValue;
    }

    public void SetupEpisodeTimeBar(float maxValue, float value)
    {
        episodeTimeBar.SetupBar(maxValue, value);
    }

    public void ChangeEpisodeTimeBarValue(float newValue)
    {
        episodeTimeBar.ChangeValue(newValue);
    }

    public void SetLockScreanReason(string reason)
    {
        lockScreen.SetReason(reason);
    }

    public void SetLockScreanActive(bool active)
    {
        lockScreen.gameObject.SetActive(active);
    }

    public void ActivateEscapeMenu()
    {
        Managers.App.PauseGame();
        Managers.Level.LockPlayerLook(true);
        Managers.Level.LockSpectatorLook(true);
        LockCursor(false);
        escapeMenuScreen.SetActive(true);
        escapeMenu = true;
    }

    public void CloseEscapeMenu()
    {
        Managers.App.ResumeGame();
        Managers.Level.LockPlayerLook(false);
        Managers.Level.LockSpectatorLook(false);
        LockCursor(true);
        escapeMenuScreen.SetActive(false);
        escapeMenu = false;
    }

    public void ActivateGiveUpButton(bool value)
    {
        giveUpButton.SetActive(value);
    }

    public void ActivateLockState(bool value)
    {
        Managers.Level.LockPlayerLook(value);
        Managers.Level.LockSpectatorLook(value);
        LockCursor(!value);
        lockState = value;
    }

    public void ToggleLockState()
    {
        ActivateLockState(!lockState);
    }

    public void LockCursor(bool locked)
    {
        Cursor.visible = !locked;
        if (locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void ToggleCursor()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            LockCursor(false);
        }
        else if (Cursor.lockState == CursorLockMode.None)
        {
            LockCursor(true);
        }
    }
    public void ToggleVisibleGui()
    {
        visibleGui = !visibleGui;
        ActivateVisibleGui(visibleGui);
    }

    public void ActivateVisibleGui(bool activate)
    {
        leftPanel.gameObject.SetActive(activate);
        rightPanel.gameObject.SetActive(activate);
        episodeTimeBar.gameObject.SetActive(activate);
        generationLabel.gameObject.SetActive(activate);

        popUpMessager.ResetPopUp();
        popUpMessager.gameObject.SetActive(activate);
        if (Managers.App.IsPlayerInGame())
        {
            elementBar.gameObject.SetActive(activate);
        }
    }

    private void CalculateCurrentFPSCount()
    {
        fpsCounterTime += Time.deltaTime;
        if (fpsCounterTime >= fpsIntervalTime)
        {
            FPSLabel.text = "FPS " + (int)(1f / Time.unscaledDeltaTime);
            fpsCounterTime -= fpsIntervalTime;
        }
    }

    public void ToggleFPSCounter()
    {
        fpsCounter = !fpsCounter;
        ActivateFPSCounter(fpsCounter);
    }

    public void ActivateFPSCounter(bool activate)
    {
        FPSLabel.gameObject.SetActive(activate);
    }

    public void ActivateLoadingScrean(bool value)
    {
        loadingScreen.SetActive(value);
    }
}