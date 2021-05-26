using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnCollision : MonoBehaviour
{
    [SerializeField] private AudioClip destroySound;
    private AudioSource audioSource;
    private bool isDestroyClipPlaying = false;


    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    private void Update()
    {
        if (isDestroyClipPlaying && !audioSource.isPlaying)
            Destroy(this.gameObject);

    }
    private void OnCollisionEnter(Collision collision)
    {
        audioSource.Play();
        isDestroyClipPlaying = true;
    }
}
