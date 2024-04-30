using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using URPGlitch.Runtime.AnalogGlitch;

public class ChangePostProcess : MonoBehaviour
{
    [SerializeField] private Volume volume;
    public ChromaticAberration chromaticAberration;
    public Vignette vignette;
    public Bloom bloom;
    public AnalogGlitchVolume glich;

    [SerializeField] public Player player;
    
    public void Start()
    {
        if (volume.profile.TryGet(out ChromaticAberration ca))
        {
            chromaticAberration = ca;
        }

        if (volume.profile.TryGet(out Vignette vig))
        {
            vignette = vig;
        }

        if (volume.profile.TryGet(out Bloom blm))
        {
            bloom = blm;
        }

        if (volume.profile.TryGet(out AnalogGlitchVolume ag))
        {
            glich = ag;
        }

        player = GetComponent<Player>();
    }   
    
    public void ChromaticScreen()
    {
        StartCoroutine(ChromaticAberrationEffect());
    }

    private IEnumerator ChromaticAberrationEffect()
    {
        chromaticAberration.intensity.value = 1f;

        yield return new WaitForSecondsRealtime(1f);
        
        float elapsedTime = 0f;
        float duration = 1f;

        while (elapsedTime < duration)
        {
            chromaticAberration.intensity.Override(Mathf.Lerp(1f, 0f, elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        chromaticAberration.intensity.value = 0f;
    }

    public void VignetteScreen()
    {
        StartCoroutine(VignetteEffect());
    }

    private IEnumerator VignetteEffect()
    {
        vignette.active = true;
        vignette.intensity.value = 0.4f;

        yield return new WaitForSecondsRealtime(1f);
        
        float elapsedTime = 0f;
        float duration = 1f;

        while (elapsedTime < duration)
        {
            vignette.intensity.Override(Mathf.Lerp(0.4f, 0f, elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        vignette.active = false;
    }

    public void RageMode()
    {
        if (!player.isRage)
        {
            StartCoroutine(RageEffect());
        }
    }

    private IEnumerator RageEffect()
    {
        player.anim.SetFloat("attackSpeed", 1.2f);
        player.moveSpeed = player.rageSpeed;
        player.ghost.makeGhost = true;
        player.eyeLight.emitting = true;
        if (!player.ghost.makeGhost)
        {
            player.ghost.makeGhost = true;
        }
        player.isRage = true;
        Color rageCol =new Color(208, 121, 25);
        Color originCol = new Color(255, 255, 255);
        
        bloom.dirtIntensity.value = 20f;
        bloom.tint.value = rageCol;
        bloom.intensity.value = 6f;
        chromaticAberration.intensity.value = 0.4f;
        
        yield return new WaitForSecondsRealtime(0.1f);

        float bloomTimer = 0f;
        float bloomDur = 0.1f;
        
        while (bloomTimer < bloomDur)
        {
            bloom.intensity.Override(Mathf.Lerp(6f, 2f, bloomTimer / bloomDur));
            bloomTimer += Time.deltaTime;
            yield return null;
        }
        bloom.intensity.value = 2f;
        
        yield return new WaitForSecondsRealtime(4f);
        
        float elapsedTime = 0f;
        float duration = 1f;

        while (elapsedTime < duration)
        {
            bloom.dirtIntensity.Override(Mathf.Lerp(20f, 0f, elapsedTime / duration));
            bloom.tint.Override(Color.Lerp(rageCol, originCol, elapsedTime / duration));
            chromaticAberration.intensity.Override(Mathf.Lerp(0.4f, 0f, elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        player.anim.SetFloat("attackSpeed", 1f);
        player.moveSpeed = 6f;
        player.ghost.makeGhost = false;
        player.eyeLight.emitting = false;
        bloom.dirtIntensity.value = 0f;
        bloom.intensity.value = 1f;
        bloom.tint.value = originCol;
        chromaticAberration.intensity.value = 0f;
        player.isRage = false;
    }
    
    public void GlitchScreen()
    {
        StartCoroutine(GlithEffect());
    }

    private IEnumerator GlithEffect()
    {
        glich.active = true;

        yield return new WaitForSecondsRealtime(0.3f);

        glich.active = false;
    }
}
