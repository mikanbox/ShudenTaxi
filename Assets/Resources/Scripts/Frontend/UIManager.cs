using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class UIManager : SingletonMonoBehaviour<UIManager> {

    [SerializeField]private InputField SendingMessage;
    [SerializeField]private GameObject[] WindowObj;
    [SerializeField]private GameObject StatusBar;

    private static float interval = 60f;
    private UIState uiState = 0;

    void Update () {
        if (interval < 0) {
            if (uiState == UIState.Map)CreatingMap.Instance.GetMap();
            interval = 60;
        }
        interval -= Time.deltaTime;
    }

    protected override void Awake() {
        CheckInstance();
        StateManager.Instance.ObserveEveryValueChanged(x => x.uiState).Subscribe( _ => WindowChange());
        StateManager.Instance.ObserveEveryValueChanged(x => x.matchingState).Subscribe( _ => StatusBarManager());
    }

    public void StatusBarManager() {
        StatusBar.SetActive((StateManager.Instance.matchingState == MatchingState.Matching));
        if (StateManager.Instance.matchingState == MatchingState.Matched)CreatingMap.Instance.UpdateTaxi();
    }

    private void WindowChange() {
        WindowObj[(int)uiState].SetActive(false);
        WindowObj[(int)StateManager.Instance.uiState].SetActive(true);
        uiState = StateManager.Instance.uiState;
    }

}
