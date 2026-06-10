using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public Camera playerCam;

    public float walkSpeed = 3f;
    public float runSpeed = 5f;
    public float jumpPower = 0f;
    public float gravity = 10f;

    public float lookSpeed = 2f;
    public float lookXLimit = 75f;
    public float cameraRotationSmooth = 5f;

    // ✅ Only wood sounds left
    public AudioClip[] woodFootstepSounds;
    public Transform footstepAudioPosition;
    public AudioSource audioSource;

    private AudioClip[] currentFootstepSounds;

    private bool isWalking = false;
    private bool isFootstepCoroutineRunning = false;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private float rotationY = 0;

    public int ZoomFOV = 35;
    private float initialFOV;
    public float cameraZoomSmooth = 5f;

    private bool isZoomed = false;
    private bool canMove = true;

    private CharacterController characterController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        if (playerCam == null)
        {
            Debug.LogError("Player Camera not assigned!");
            return;
        }

        if (audioSource == null)
        {
            Debug.LogError("AudioSource not assigned!");
        }

        initialFOV = playerCam.fieldOfView;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // ✅ Always wood now
        currentFootstepSounds = woodFootstepSounds;
    }

    void Update()
    {
        if (!canMove) return;

        // ===== Movement =====
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");

        Vector3 move = transform.forward * inputZ + transform.right * inputX;
        move = Vector3.ClampMagnitude(move, 1f);

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float speed = isRunning ? runSpeed : walkSpeed;

        float movementY = moveDirection.y;
        moveDirection = move * speed;
        moveDirection.y = movementY;

        // Jump
        if (characterController.isGrounded)
        {
            if (Input.GetButton("Jump"))
            {
                moveDirection.y = jumpPower;
            }
        }
        else
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        characterController.Move(moveDirection * Time.deltaTime);

        // ===== Camera =====
        rotationX -= Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

        rotationY += Input.GetAxis("Mouse X") * lookSpeed;

        Quaternion camRot = Quaternion.Euler(rotationX, 0, 0);
        Quaternion bodyRot = Quaternion.Euler(0, rotationY, 0);

        playerCam.transform.localRotation = Quaternion.Slerp(
            playerCam.transform.localRotation,
            camRot,
            Time.deltaTime * cameraRotationSmooth
        );

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            bodyRot,
            Time.deltaTime * cameraRotationSmooth
        );

        // ===== Zoom =====
        isZoomed = Input.GetButton("Fire2");

        float targetFOV = isZoomed ? ZoomFOV : initialFOV;
        playerCam.fieldOfView = Mathf.Lerp(
            playerCam.fieldOfView,
            targetFOV,
            Time.deltaTime * cameraZoomSmooth
        );

        // ===== Footsteps =====
        bool isMoving = move.magnitude > 0.1f && characterController.isGrounded;

        if (isMoving && !isWalking && !isFootstepCoroutineRunning)
        {
            isWalking = true;
            StartCoroutine(PlayFootstepSounds(0.5f / speed));
        }
        else if (!isMoving)
        {
            isWalking = false;
        }
    }

    IEnumerator PlayFootstepSounds(float delay)
    {
        isFootstepCoroutineRunning = true;

        while (isWalking)
        {
            if (woodFootstepSounds.Length > 0 && audioSource != null)
            {
                int i = Random.Range(0, woodFootstepSounds.Length);
                audioSource.transform.position = footstepAudioPosition.position;
                audioSource.clip = woodFootstepSounds[i];
                audioSource.Play();
            }

            yield return new WaitForSeconds(delay);
        }

        isFootstepCoroutineRunning = false;
    }
}