using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;


public class PlayerMovement : NetworkBehaviour
{
    private Rigidbody2D rb;
    private float moveH, moveV;
    [SerializeField] private float moveSpeed = 1.0f;
    private Camera mainCam;
    private PlayerAnimation playerAnimation;

    private GameObject nearbyBox;
    private GameObject pickedUpBox; 
    [SerializeField] private GameObject miniBoxPrefab; 
    private GameObject miniBoxInstance; 
    private Vector2 lastDirection;
    public Vector3 currentDirection = Vector3.right;

    private void Awake()
    {
        mainCam = Camera.main;
    }

    private void Start()
    {
        DeletePlayer();
        rb = GetComponent<Rigidbody2D>();
        playerAnimation = GetComponentInChildren<PlayerAnimation>();
    }

    private void Update()
    {
        if (!isLocalPlayer) return;
        if (nearbyBox != null && Input.GetKeyDown(KeyCode.E))
        {
            CmdPickUpBox(nearbyBox);
        }

        if (pickedUpBox != null && Input.GetKeyDown(KeyCode.E))
        {
            CmdDropDownBox();
        }
        Vector3 inputDirection = Vector3.zero;
        
        if (Input.GetKey(KeyCode.W))
            inputDirection += (Vector3.up);
        if (Input.GetKey(KeyCode.S))
            inputDirection += (Vector3.down );
        if (Input.GetKey(KeyCode.A))
            inputDirection += (Vector3.left );
        if (Input.GetKey(KeyCode.D))
            inputDirection += (Vector3.right );

        // Если есть направление, нормализуем его
        if (inputDirection != Vector3.zero)
        {
            currentDirection = inputDirection.normalized;
        }
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer) return;
        moveH = Input.GetAxis("Horizontal") * moveSpeed;
        moveV = Input.GetAxis("Vertical") * moveSpeed;
        rb.linearVelocity = new Vector2(moveH, moveV);
        Vector2 direction = new Vector2(moveH, moveV);
        if (direction != Vector2.zero)
        {
            lastDirection = direction.normalized;
        }
        playerAnimation.SetDirection(direction);

        CameraMovement();
    }

    private void CameraMovement()
    {
        float offsetX = +0.25f;
        mainCam.transform.position = new Vector3(transform.position.x + offsetX, transform.position.y, -10f);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("box"))
        {
            nearbyBox = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("box") && nearbyBox == other.gameObject)
        {
            nearbyBox = null; 
        }
    }

    [Command]
    private void CmdPickUpBox(GameObject box)
    {
        RpcShowMiniBox(); 
        RpcHideBox(box);
    }
    
    [Command]
    private void CmdDropDownBox()
    {
        RpcRemoveMiniBox();
        RpcRevilBox(pickedUpBox);
    }

    [ClientRpc]
    private void RpcHideBox(GameObject box)
    {
        pickedUpBox = box;
        box.SetActive(false); 
    }
    
    [ClientRpc]
    private void RpcRevilBox(GameObject box)
    {
        var boxLink = box.GetComponent<BoxLinkManager>();
        var oldPos = box.transform.position;
        Vector3 newPosition = transform.position + currentDirection * (0.3f) + new Vector3(0.2f, -0.35f, 0);;
        
        box.transform.position = newPosition;
        box.SetActive(true);
        
        if (boxLink != null && boxLink.IsPastBox)
        {
            GameObject futureBox = boxLink.FutureBox;
            if (futureBox != null)
            {
                Vector3 offset = newPosition - oldPos;
                var oldFuturePos = boxLink.FuturePos;
                futureBox.transform.position =  oldFuturePos + offset;
                boxLink.FuturePos = oldFuturePos + offset;
            }
        }

        pickedUpBox = null;
    }


    [ClientRpc]
    private void RpcShowMiniBox()
    {
        if (miniBoxPrefab != null && miniBoxInstance == null)
        {
            miniBoxInstance = Instantiate(miniBoxPrefab, transform.position + new Vector3(0.5f, 0.25f, 0), Quaternion.identity);
            miniBoxInstance.transform.SetParent(transform);
        }
    }

    [ClientRpc]
    private void RpcRemoveMiniBox()
    {
        if (miniBoxInstance != null)
        {
            Destroy(miniBoxInstance); 
            miniBoxInstance = null;
        }
    }
    [Command] // Этот метод вызывается на клиенте, но исполняется на сервере
    private void CmdDeletePlayer()
    {
        NetworkServer.Destroy(gameObject); // Уничтожаем объект игрока на сервере
    }
    private void DeletePlayer()
    {
        if (SceneManager.GetActiveScene().name != "Temple")
        {
            if (isServer)
            {
                RpcHidePlayer(); // Скрываем игрока на всех клиентах
                StartCoroutine(DestroyAfterDelay(0.5f)); // Удаляем объект с задержкой
            }
            else if (isLocalPlayer)
            {
                CmdRequestHideAndDestroy();
            }
        }
    }

    [Command]
    private void CmdRequestHideAndDestroy()
    {
        RpcHidePlayer(); // Скрываем игрока на всех клиентах
        StartCoroutine(DestroyAfterDelay(0.5f)); // Удаляем объект с задержкой
    }

    [ClientRpc]
    private void RpcHidePlayer()
    {
        gameObject.SetActive(false); // Мгновенно скрываем объект на всех клиентах
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (this.gameObject != null)
        {
            NetworkServer.Destroy(this.gameObject); // Удаляем объект с сервера (и синхронизируем удаление)
        }
    }
}