using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GamepadRumble
{
    private bool _gamepadConnected;
    private Coroutine _createRumble = null;
    private bool _isInitialized = false;
    private CharacterView _player;
    private Gamepad _gamepad;

    public void Initialize(CharacterView player)
    {
        InputSystem.onDeviceChange += OnDeviceChange;
        _isInitialized = true;
        _player = player;
        CheckControllerConnectedInitial();
    }

    private void CheckControllerConnectedInitial()
    {
        if(_gamepad!=null)
        {
            return;
        }
        if(Gamepad.all.Count>0)
        {
            _gamepad = Gamepad.current;
            _gamepadConnected = true;
        }
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        try
        {
            if (device is Gamepad && !(change == InputDeviceChange.Disconnected || change == InputDeviceChange.Disabled))
            {
                _gamepad = device as Gamepad;
                _gamepadConnected = true;
            }
            else
            {
                _gamepad = null;
                _gamepadConnected = false;
            }
        }
        catch (Exception exception)
        {
            Debug.LogError(exception);
            _gamepadConnected = false;
        }
    }

    public void Rumble(float lowFrequency, float highFrequency, float duration)
    {
        CheckControllerConnectedInitial();
        if (!_isInitialized || !_gamepadConnected)
        {
            return;
        }
        if (_createRumble != null)
        {
            _player.StopCoroutine(_createRumble);
            _createRumble = null;
            _gamepad?.SetMotorSpeeds(0, 0);
        }
        _createRumble = _player.StartCoroutine(RumbleActivate(lowFrequency, highFrequency, duration));
    }

    public void DecreasingRumble(float lowFrequency, float highFrequency, float duration)
    {
        //TODO
    }

    public IEnumerator RumbleActivate(float lowFrequency, float highFrequency, float duration)
    {
        _gamepad?.SetMotorSpeeds(lowFrequency,highFrequency);
        yield return Awaitable.WaitForSecondsAsync(duration);
        _gamepad?.SetMotorSpeeds(0,0);
        _createRumble = null;
    }
}
