﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TitleScript : MonoBehaviour {
  public GameObject[] toActivate;
  public GameObject[] toDeactivate;
  public float cooldown = .5f;
  float showContinueTime;

  void Start() {
    Time.timeScale = 0;
    showContinueTime = Time.unscaledTime + cooldown;
  }

  void Update() {
    if (Time.unscaledTime >= showContinueTime) {
      Gamepad gamepad = Gamepad.current;
      if (gamepad != null && (
        gamepad.buttonNorth.wasPressedThisFrame ||
        gamepad.buttonSouth.wasPressedThisFrame ||
        gamepad.buttonEast.wasPressedThisFrame ||
        gamepad.buttonWest.wasPressedThisFrame ||
        gamepad.leftTrigger.wasPressedThisFrame ||
        gamepad.rightTrigger.wasPressedThisFrame ||
        gamepad.leftShoulder.wasPressedThisFrame ||
        gamepad.rightShoulder.wasPressedThisFrame ||
        gamepad.selectButton.wasPressedThisFrame ||
        gamepad.startButton.wasPressedThisFrame
      )) {
        foreach (GameObject obj in toActivate) {
          obj.SetActive(true);
        }
        foreach (GameObject obj in toDeactivate) {
          obj.SetActive(false);
        }

        Time.timeScale = 1;
      }
    }
  }
}
