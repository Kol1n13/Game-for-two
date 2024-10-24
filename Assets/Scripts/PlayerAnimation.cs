using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerAnimation : NetworkBehaviour
{
    private Animator anim;

    public string[] staticDirections = { "Static N", "Static NW", "Static W", "Static SW", "Static S", "Static SE", "Static E", "Static NE" };
    public string[] runDirections = { "Run N", "Run NW", "Run W", "Run SW", "Run S", "Run SE", "Run E", "Run NE" };

    int lastDirection;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    // Этот метод вызывается для установки направления анимации
    public void SetDirection(Vector2 _direction)
    {
        if (!isOwned) return;  // Используем проверку на владение объектом

        string[] directionArray = null;

        if (_direction.magnitude < 0.01f) // Персонаж стоит на месте
        {
            directionArray = staticDirections;
        }
        else
        {
            directionArray = runDirections;
            lastDirection = DirectionToIndex(_direction); // Получение индекса направления
        }

        // Локально играем анимацию
        anim.Play(directionArray[lastDirection]);

        // Отправляем команду на синхронизацию анимации на сервере
        CmdSyncAnimation(lastDirection, _direction.magnitude < 0.01f);
    }

    // Конвертирует Vector2 направление в индекс, который соответствует сегменту окружности (в градусах)
    private int DirectionToIndex(Vector2 _direction)
    {
        Vector2 norDir = _direction.normalized; // Нормализация вектора
        float step = 360 / 8; // Угол на сегмент окружности (45 градусов)
        float offset = step / 2; // Добавляем небольшой оффсет, чтобы получить корректные индексы

        float angle = Vector2.SignedAngle(Vector2.up, norDir); // Угол между вектором вверх и направлением персонажа
        angle += offset;

        if (angle < 0) 
        {
            angle += 360; // Избегаем отрицательных значений углов
        }

        float stepCount = angle / step;
        return Mathf.FloorToInt(stepCount); // Округляем до целого значения для индекса анимации
    }

    // Команда для синхронизации анимации на сервере
    [Command]
    private void CmdSyncAnimation(int direction, bool isStatic)
    {
        if (!isServer) return;  // Добавляем проверку, чтобы сервер не отправлял команду сам себе
        RpcPlayAnimation(direction, isStatic);
    }

    // RPC для проигрывания анимации на всех клиентах
    [ClientRpc]
    private void RpcPlayAnimation(int direction, bool isStatic)
    {
        if (isLocalPlayer) return; // Не проигрываем анимацию для локального игрока снова

        string[] directionArray = isStatic ? staticDirections : runDirections;
        anim.Play(directionArray[direction]); // Проигрываем нужную анимацию на других клиентах
    }
}
