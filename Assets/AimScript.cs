﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AimScript : MonoBehaviour {
  public Transform sword;
  public Transform aimIndicator;
  public Sprite[] swordSprites;

  public float radius = 4;
  public float angleRange = 120;
  public float deadZone = 0.3f;
  public float slashDuration = .5f;

  public float swingAngleStart = -75;

  SpriteRenderer swordSpriter;
  TrailRenderer swordTrail;

  float aimAngle;
  float blockAngleModifier = 0.6f;

  public float ellipseRadiusX = 3;
  public float ellipseRadiusY = 1;
  public float ellipseOffsetY = -2;

  float slashTimeRemaining = 0;

  void Start() {
    swordSpriter = sword.GetComponent<SpriteRenderer>();
    swordTrail = sword.GetComponentInChildren<TrailRenderer>();
  }

  void Update() {
    Gamepad gamepad = Gamepad.current;
    if (gamepad == null) {
      return;
    }

    if (slashTimeRemaining <= 0) {
      Vector2 leftAim = gamepad.leftStick.ReadValue();
      Vector2 rightAim = gamepad.rightStick.ReadValue();
      if (leftAim.magnitude >= deadZone) {
        SetAimAngle(leftAim);
      }
      else if (rightAim.magnitude >= deadZone) {
        SetAimAngle(rightAim);
      }

      if (gamepad.leftShoulder.isPressed || gamepad.rightShoulder.isPressed) {
        float blockAngle = aimAngle * blockAngleModifier + Mathf.Sign(aimAngle) * angleRange * (1 - blockAngleModifier);
        PositionSword(0, blockAngle);
        sword.position -= new Vector3(sword.position.x, 0, 0);
        sword.localScale *= new Vector2(-1, 1);
        sword.localRotation = Quaternion.Euler(0, 0, -sword.localRotation.eulerAngles.z);
      }
      else if (gamepad.leftTrigger.wasPressedThisFrame || gamepad.rightTrigger.wasPressedThisFrame) {
        slashTimeRemaining = slashDuration;
        swordTrail.emitting = true;
      }
    }

    if (slashTimeRemaining > 0) {
      float lerpValue = (slashDuration - slashTimeRemaining) / slashDuration;
      PositionSword(lerpValue);

      if (lerpValue > .9f) {
        swordTrail.emitting = false;
      }

      slashTimeRemaining -= Time.deltaTime;
    }
  }

  void SetAimAngle(Vector2 inputAim) {
    aimAngle = Vector2.SignedAngle(Vector2.up, inputAim);
    float minAngle = (180 - angleRange) / 2;
    float maxAngle = 180 - minAngle;
    aimAngle = Mathf.Sign(aimAngle) * Mathf.Clamp(Mathf.Abs(aimAngle), minAngle, maxAngle);
    aimIndicator.position = Quaternion.AngleAxis(aimAngle, Vector3.forward) * Vector2.up * radius;
    PositionSword(0);
  }

  void PositionSword(float lerpValue) {
    PositionSword(lerpValue, aimAngle);
  }

  void PositionSword(float lerpValue, float angle) {
    if (angle < 0) {
      lerpValue = 1 - lerpValue;
    }

    float swingAngle = Util.EaseInOutCubic(-swingAngleStart, swingAngleStart, lerpValue);
    float swingAngleRad = (swingAngle + 90) * Mathf.Deg2Rad;

    Vector2 ellipseCenter = new Vector2(0, ellipseOffsetY);
    Vector2 handlePoint = ellipseCenter + new Vector2(ellipseRadiusX * Mathf.Cos(swingAngleRad), ellipseRadiusY * Mathf.Sin(swingAngleRad));
    float swordHeight = -handlePoint.y;
    float swordWidth = swordHeight * Mathf.Tan(-swingAngle * Mathf.Deg2Rad);
    Vector2 swordPoint = handlePoint + new Vector2(swordWidth, swordHeight);

    Quaternion rotation = Quaternion.AngleAxis(angle - 90 * Mathf.Sign(aimAngle), Vector3.forward);
    handlePoint = rotation * handlePoint;
    swordPoint = rotation * swordPoint;
    // Debug.DrawLine(handlePoint, swordPoint);

    // slashIndicator.position = Vector2.Lerp(aimIndicator.position, -aimIndicator.position, lerpValue);
    sword.position = swordPoint;
    sword.localRotation = rotation;

    swordSpriter.sprite = swordSprites[Mathf.Min((int)(lerpValue * swordSprites.Length), swordSprites.Length - 1)];
    float spriteHeight = swordSpriter.sprite.rect.height / swordSpriter.sprite.pixelsPerUnit;
    sword.localScale = Vector2.one * swordHeight / spriteHeight;
  }
}
