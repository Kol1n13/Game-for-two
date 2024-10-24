using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerMovement : NetworkBehaviour
{
    private Rigidbody2D rb;
    private float moveH, moveV;
    [SerializeField] private float moveSpeed = 1.0f;
    private Camera mainCam;
    private PlayerAnimation playerAnimation;

    private void Awake()
    {
        mainCam = Camera.main;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerAnimation = GetComponentInChildren<PlayerAnimation>(); // Получаем ссылку на компонент анимации
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer) return;

        // Получаем ввод для перемещения
        moveH = Input.GetAxis("Horizontal") * moveSpeed;
        moveV = Input.GetAxis("Vertical") * moveSpeed;

        // Используем Rigidbody2D для перемещения
        rb.linearVelocity = new Vector2(moveH, moveV);

        Vector2 direction = new Vector2(moveH, moveV);

        // Анимация игрока
        playerAnimation.SetDirection(direction); // Вызываем анимацию через сохранённую ссылку

        // Перемещение камеры
        CameraMovement();
    }

    private void CameraMovement()
    {
        float offsetX = +0.25f;
        mainCam.transform.position = new Vector3(transform.position.x + offsetX, transform.position.y, -10f);
    }
}