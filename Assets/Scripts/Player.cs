using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.Debug;

namespace LevelMaze
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class Player : MonoBehaviour
    {
        int keys;

        internal static float speed;

        CharacterController controller;

        [SerializeField] float gravity = -9.81f;
        Vector3 velocity;

        bool groundCheck;
        [SerializeField] Transform groundObject;
        [SerializeField] LayerMask groundMask;

        [SerializeField] Animator cameraAnim;

        [SerializeField] AudioSource audioSource;
        [SerializeField] List<AudioClip> footSteps;

        internal static UnityAction onPlayerInZone;

        void Start()
        {
            controller = GetComponent<CharacterController>();
            KeyPanel.onPlayerTouchedPanel += onTouchPanel;

            WinController.onPlayerWin += OnPlayerWin;

            StartCoroutine(ChangeAudioClip());

            speed = 5f;
        }

        void UnitMove()
        {
            groundCheck = Physics.CheckSphere(groundObject.position, 0.3f, groundMask);
            if (groundCheck)
            {
                velocity.y = -2f;
            }
            //�������� �� �����
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            if (x != 0 || z != 0) 
            {
                cameraAnim.SetBool("isWalking", true);
                if (!audioSource.isPlaying) { audioSource.Play(); }
            }
            else 
            { 
                cameraAnim.SetBool("isWalking", false);
            }

            Vector3 move = transform.right * x + transform.forward * z;
            controller.Move(speed * move * Time.deltaTime);

            //����������
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }

        void Update()
        {
            UnitMove();
            if (!audioSource.isPlaying) { audioSource.Play(); }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Key")
            {
                keys++;
                Destroy(other.gameObject);
                Log("destroy");
            }
        }

        void onTouchPanel()
        {
            Log("touch");
            if (keys == 4)
            {
                KeyPanel.hasKeys = true;
            }
        }
        void OnPlayerWin()
        {
            speed = 0f;
        }

        IEnumerator ChangeAudioClip()
        {
            audioSource.clip = footSteps[Random.Range(0, footSteps.Count - 1)];
            Log("coroutine audio");
            yield return new WaitForSeconds(0.7f);
        }
    }
}