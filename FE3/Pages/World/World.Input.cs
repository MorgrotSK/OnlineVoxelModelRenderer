using System.Numerics;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;

namespace FE3.Pages;

public partial class World
{
    // Player / camera
    private Vector3 _playerPos = new(0, 0, 0);
    private float _eyeHeight = 10f;

    private float _heading = 0f;
    private float _attitude = 0f;

    // Tick / movement
    private int _tickMs = 20;
    private float _moveSpeed = 24f;
    private readonly float _fastMult = 4.0f;
    private readonly float _slowMult = 0.45f;

    // Input
    private HashSet<string> _keysDown = new(StringComparer.OrdinalIgnoreCase);

    // Pointer lock + deltas (from JS)
    private bool _pointerLocked;
    private float _pendingMouseDx;
    private float _pendingMouseDy;

    // Smoothing / feel
    private const float _mouseSensitivity = 0.10f;
    private float _smoothedDx;
    private float _smoothedDy;
    private const float _mouseSmoothing = 0.45f;

    void ApplyMouseLook(float dt)
    {
        if (_camera == null) return;
        if (!_pointerLocked) return;

        var dx = _pendingMouseDx; _pendingMouseDx = 0;
        var dy = _pendingMouseDy; _pendingMouseDy = 0;

        var t = 1f - MathF.Pow(1f - _mouseSmoothing, 60f * dt);
        _smoothedDx = Lerp(_smoothedDx, dx, t);
        _smoothedDy = Lerp(_smoothedDy, dy, t);

        _heading = NormalizeAngle(_heading + _smoothedDx * _mouseSensitivity);
        _attitude = Math.Clamp(_attitude - _smoothedDy * _mouseSensitivity, -89f, 89f);

        _camera.Heading = _heading;
        _camera.Attitude = _attitude;
    }

    void ApplyKeyboardMove(float dt)
    {
        if (_camera == null) return;

        float forward = 0;
        float strafe = 0;
        float vertical = 0;

        if (_keysDown.Contains("w")) forward += 1;
        if (_keysDown.Contains("s")) forward -= 1;
        if (_keysDown.Contains("a")) strafe -= 1;
        if (_keysDown.Contains("d")) strafe += 1;

        if (_keysDown.Contains("space")) vertical += 1;
        if (_keysDown.Contains("shift")) vertical -= 1;

        var intent = new Vector3(strafe, vertical, forward);
        if (intent.LengthSquared() > 0)
        {
            intent = Vector3.Normalize(intent);

            var speed = _moveSpeed;
            if (_keysDown.Contains("control")) speed *= _fastMult;
            if (_keysDown.Contains("alt")) speed *= _slowMult;

            MoveFly(
                strafeAmount: intent.X * speed * dt,
                upAmount:     intent.Y * speed * dt,
                forwardAmount:intent.Z * speed * dt
            );
        }

        _camera.CameraPosition = new Vector3(_playerPos.X, _playerPos.Y + _eyeHeight, _playerPos.Z);
    }

    void MoveFly(float strafeAmount, float upAmount, float forwardAmount)
    {
        var up = Vector3.UnitY;

        float yaw = _heading * (MathF.PI / 180f);
        float pitch = _attitude * (MathF.PI / 180f);

        var forward = new Vector3(
            MathF.Sin(yaw) * MathF.Cos(pitch),
            MathF.Sin(pitch),
            -MathF.Cos(yaw) * MathF.Cos(pitch));

        forward = Vector3.Normalize(forward);

        var right = new Vector3(MathF.Cos(yaw), 0, MathF.Sin(yaw));

        _playerPos += right * strafeAmount;
        _playerPos += forward * forwardAmount;
        _playerPos += up * upAmount;
    }

    void OnKeyDown(KeyboardEventArgs e)
    {
        var k = NormalizeKey(e.Key);
        if (!string.IsNullOrWhiteSpace(k))
            _keysDown.Add(k);
    }

    void OnKeyUp(KeyboardEventArgs e)
    {
        var k = NormalizeKey(e.Key);
        if (!string.IsNullOrWhiteSpace(k))
            _keysDown.Remove(k);
    }

    static string NormalizeKey(string? key)
    {
        var k = (key ?? "").ToLowerInvariant();

        return k switch
        {
            " " => "space",
            "spacebar" => "space",
            "escape" => "escape",
            "left" => "arrowleft",
            "right" => "arrowright",
            "up" => "arrowup",
            "down" => "arrowdown",
            _ => k
        };
    }

    [JSInvokable]
    public void OnPointerLockChanged(bool locked)
    {
        _pointerLocked = locked;

        _smoothedDx = 0;
        _smoothedDy = 0;

        if (locked)
            _ = _inputHost.FocusAsync();
    }

    [JSInvokable]
    public void OnMouseDelta(float dx, float dy)
    {
        _pendingMouseDx += dx;
        _pendingMouseDy += dy;
    }

    [JSInvokable]
    public void OnWheel(float deltaY)
    {
        var step = 1.0f;
        _moveSpeed = Math.Clamp(_moveSpeed + (deltaY > 0 ? -step : step), 2f, 60f);
    }

    static float NormalizeAngle(float a)
    {
        a %= 360f;
        if (a < 0f) a += 360f;
        return a;
    }

    static float Lerp(float a, float b, float t) => a + (b - a) * Math.Clamp(t, 0f, 1f);
}
