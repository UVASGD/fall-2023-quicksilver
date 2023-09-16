using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    ParticleSystem idlePS, winPS;

    // Start is called before the first frame update
    void Start()
    {
        getParticleSystems();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            idlePS.Pause();
            idlePS.Clear();
            winPS.Play();
            StartCoroutine(nextScene());
        }
    }

    private IEnumerator nextScene()
    {
        yield return new WaitForSecondsRealtime(1);
        SceneSwitcher.LoadNextSceneInBuildOrder();
    }

    private void getParticleSystems()
    {
        idlePS = GetComponentsInChildren<ParticleSystem>()[0];
        winPS = GetComponentsInChildren<ParticleSystem>()[1];
    }
}
