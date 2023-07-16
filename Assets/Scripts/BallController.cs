using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BallController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject ballEffectMesh;
    [SerializeField] private ParticleSystem boomParticle;
    [SerializeField] private AudioClip paddleClip;
    [SerializeField] private AudioSource source;
    [SerializeField] private Vector3 rotationDir;
    [SerializeField] private DangerBallPlayerController player;
    [SerializeField] private TrailRenderer[] normalRenderers;
    [SerializeField] private TrailRenderer boostRenderer;

    [Space(10), Header("Settings")]
    [SerializeField] private LayerMask bounceMask;
    [SerializeField] private ColorLimit from;
    [SerializeField] private ColorLimit to;

    private Vector3 velocity;
    private float aiForce = .5f;
    private Vector3 additionnalForce;
    private MeshRenderer meshRenderer;
    private Vector3 origin;
    private Material colorMat;
    private Light light;
    private Rigidbody rigidbody;
    private bool boost;

    [Serializable]
    public struct ColorLimit
    {
        public Transform pos;
        public Color color;
        public Gradient gradient;
    }

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
        colorMat = meshRenderer.materials[0];
        light = GetComponentInChildren<Light>();
        boostRenderer.enabled = false;
        origin = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        ChangeDirection(collision.gameObject.tag);
        source.PlayOneShot(paddleClip);
    }

    private void HandleColorChange()
    {
        float normalizedProgress = Mathf.InverseLerp(from.pos.transform.position.z, to.pos.transform.position.z, transform.position.z);
        Color col = Color.Lerp(from.color, to.color, normalizedProgress);
        colorMat.SetColor("_EmissionColor", col);
        light.color = col;

        foreach (var renderer in normalRenderers)
        {
            renderer.colorGradient = LerpGradient(from.gradient, to.gradient, normalizedProgress);
        }
    }

    Gradient LerpGradient(Gradient start, Gradient end, float t)
    {
        GradientColorKey[] startColorKeys = start.colorKeys;
        GradientColorKey[] endColorKeys = end.colorKeys;
        GradientAlphaKey[] startAlphaKeys = start.alphaKeys;
        GradientAlphaKey[] endAlphaKeys = end.alphaKeys;

        GradientColorKey[] lerpedColorKeys = new GradientColorKey[startColorKeys.Length];
        GradientAlphaKey[] lerpedAlphaKeys = new GradientAlphaKey[startAlphaKeys.Length];

        for (int i = 0; i < startColorKeys.Length; i++)
        {
            Color lerpedColor = Color.Lerp(startColorKeys[i].color, endColorKeys[i].color, t);
            lerpedColorKeys[i] = new GradientColorKey(lerpedColor, startColorKeys[i].time);
        }

        for (int i = 0; i < startAlphaKeys.Length; i++)
        {
            float lerpedAlpha = Mathf.Lerp(startAlphaKeys[i].alpha, endAlphaKeys[i].alpha, t);
            lerpedAlphaKeys[i] = new GradientAlphaKey(lerpedAlpha, startAlphaKeys[i].time);
        }

        Gradient lerpedGradient = new Gradient();
        lerpedGradient.SetKeys(lerpedColorKeys, lerpedAlphaKeys);

        return lerpedGradient;
    }

    private void ChangeDirection(string tag)
    {
        additionnalForce = Vector3.zero;
        ballEffectMesh.GetComponent<MeshRenderer>().enabled = false;

        switch (tag)
        {
            case "up":
                velocity.y = -Mathf.Abs(velocity.y);
                break;
            case "down":
                velocity.y = Mathf.Abs(velocity.y);
                break;
            case "right":
                velocity.x = -Mathf.Abs(velocity.x);
                break;
            case "left":
                velocity.x = Mathf.Abs(velocity.x);
                break;
            case "player":
                boost = false;
                boostRenderer.enabled = false;
                float last = velocity.z;
                velocity.z = Mathf.Clamp(Mathf.Abs(velocity.z) + player.GetHitForce(), .4f, 6f);
                if (player.GetHitForce() > 1.5f && velocity.z >= 5)
                {
                    velocity.z += 1.5f;
                    boost = true;
                    boostRenderer.enabled = true;
                }

                if (player.GetHitForce() < 0)
                {
                    velocity.x /= 2;
                    velocity.y /= 2;
                }

                velocity += Vector3.ClampMagnitude(player.GetMoveForce(), 3.5f) / 15f;
                rotationDir = player.GetMoveForce();

                if (rotationDir.magnitude > 3.5f && velocity.z >= 4f)
                {
                    additionnalForce = -rotationDir - rotationDir.normalized *
                        Mathf.InverseLerp(3.5f, 6f, rotationDir.magnitude);
                    ballEffectMesh.GetComponent<MeshRenderer>().enabled = true;
                }
                break;
            //TODO with more time allowed: Make AI able to use effects 
            case "ai":
                boost = false;
                boostRenderer.enabled = false;
                velocity.z = -Mathf.Clamp(Mathf.Abs(velocity.z) + aiForce, .4f, 6f);
                break;
        }

        source.pitch = 1 + velocity.z / 20f + additionnalForce.magnitude / 10f;
    }

    public void Launch(Vector3 _velocity)
    {
        velocity = _velocity;
    }

    public void Restart()
    {
        velocity = Vector3.zero;
        additionnalForce = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        boost = false;
        boostRenderer.enabled = false;
        HandleColorChange();
        boomParticle.Stop();
        meshRenderer.enabled = true;
        ballEffectMesh.GetComponent<MeshRenderer>().enabled = false;
        transform.position = origin;
    }

    public void Boom()
    {
        boomParticle.Play();
        meshRenderer.enabled = false;
    }

    private void Update()
    {
        HandleColorChange();
        
        velocity += additionnalForce / 2f * Time.deltaTime;
        rigidbody.velocity = velocity;

        ballEffectMesh.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(rotationDir.y, rotationDir.x));
    }
}
