using UnityEngine;

public class ParticleLoopManager : MonoBehaviour
{
    [Header("Particles")]
    public ParticleSystem particle1;

    public ParticleSystem particle2;

    public ParticleSystem particle3;

    public ParticleSystem particle4;

    void Start()
    {
        SetupParticle(particle1);
        SetupParticle(particle2);
        SetupParticle(particle3);
        SetupParticle(particle4);
    }

    void Update()
    {
        CheckParticle(particle1);
        CheckParticle(particle2);
        CheckParticle(particle3);
        CheckParticle(particle4);
    }

    // SETUP PARTICLE
    void SetupParticle(ParticleSystem ps)
    {
        if (ps != null)
        {
            var main = ps.main;

            main.loop = true;

            ps.Play();
        }
    }

    // CHECK PARTICLE
    void CheckParticle(ParticleSystem ps)
    {
        if (ps != null && !ps.isPlaying)
        {
            ps.Play();
        }
    }
}