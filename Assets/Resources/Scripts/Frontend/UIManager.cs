using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class UIManager : SingletonMonoBehaviour<UIManager> {

    [SerializeField]private InputField SendingMessage;
    [SerializeField]private GameObject[] WindowObj;
    [SerializeField]private GameObject StatusBar;

    private UIState uiState = 0;

    protected override void Awake()
    {
        CheckInstance();
        StateManager.Instance.ObserveEveryValueChanged(x => x.uiState).Subscribe( _ => WindowChange());
        StateManager.Instance.ObserveEveryValueChanged(x => x.matchingState).Subscribe( _ => StatusBarManager());
    }

    // public void Window_SettingOption() {
    //     WindowChange(0, 4);
    //     CreatingMap.Instance.GetMap();
    // }

    // public void Window_SendingMess() {
    //     WindowChange(0, 1);
    // }

    // public void SeindingMessage() {
    //     //string sendtxt = SendingMessage.text;
    //     WindowChange(1, 2);
    //     CreatingMap.Instance.GetMap();
    //     StatusBarManager(true);
    // }

    // public void Window_ReachTaxi() {
    //     WindowChange(2, 3);
    // }

    // public void Window_ReturnTopfromReachTaxi() {
    //     WindowChange(3, 0);
    // }

    // public void Window_ReturnTopfromOption(bool Result) {
    //     WindowChange(4, 0);
    //     if (Result==true){
    //         //StateManager.Instance.obj_lat=0;
    //         //StateManager.Instance.obj_lng=0;
    //     }
    // }

    public void StatusBarManager() {
        StatusBar.SetActive((StateManager.Instance.matchingState == MatchingState.matching));
    }

    private void WindowChange() {
        WindowObj[(int)uiState].SetActive(false);
        WindowObj[(int)StateManager.Instance.uiState].SetActive(true);
        uiState = StateManager.Instance.uiState;
    }
}
