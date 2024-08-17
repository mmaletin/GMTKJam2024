using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BankLoader : MonoBehaviour
{
    public List<string> Banks = new List<string>();

    public StudioEventEmitter amb;

    public StudioEventEmitter aaaahh;

    public GameObject loading;

    public Image logo;
    public Image logoText;
    public GameObject introPanel;

    public float fadeGameLogoTime;
    private bool startLogo = false;
    private bool stopLogo = false;

    public float fadeDelayStartLogo;
    public float fadeDelayStopLogo;
    public float titleScreenDelay;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadGameAsync());
    }

    // Update is called once per frame
    void Update()
    {
        //if (startLogo)
        //{
        //    fadeGameLogoTime += Time.deltaTime * 0.5f;
        //    logo.color = new Color(logo.color.r, logo.color.g, logo.color.b, fadeGameLogoTime);
        //    logoText.color = new Color(logoText.color.r, logoText.color.g, logoText.color.b, fadeGameLogoTime);
        //}

        //if (stopLogo)
        //{
        //    fadeGameLogoTime -= Time.deltaTime * 3f;
        //    logo.color = new Color(logo.color.r, logo.color.g, logo.color.b, fadeGameLogoTime);
        //    logoText.color = new Color(logoText.color.r, logoText.color.g, logoText.color.b, fadeGameLogoTime);
        //}
    }

    IEnumerator LoadGameAsync()
    {
        // Iterate all the Studio Banks and start them loading in the background
        // including the audio sample data
        foreach (var bank in Banks)
        {
            RuntimeManager.LoadBank(bank, true);
        }

        // Keep yielding the co-routine until all the bank loading is done
        // (for platforms with asynchronous bank loading)
        while (RuntimeManager.HaveAllBanksLoaded)
        {
            yield return null;
        }

        // Keep yielding the co-routine until all the sample data loading is done
        while (RuntimeManager.AnySampleDataLoading())
        {
            yield return null;
        }

        // Allow the scene to be activated. This means that any OnActivated() or Start()
        // methods will be guaranteed that all FMOD Studio loading will be completed and
        // there will be no delay in starting events

        //amb.Play();

        //loading.SetActive(false);
        //StartCoroutine(StartDelayLogos());
        //StartCoroutine(StopDelayLogos());
        //StartCoroutine(StartTitleScreeen());
    }

    //private IEnumerator StartDelayLogos()
    //{
    //    var wait = new WaitForSeconds(fadeDelayStartLogo);

    //    yield return wait;

    //    startLogo = true;
    //    aaaahh.Play();
    //}

    //private IEnumerator StopDelayLogos()
    //{
    //    var wait = new WaitForSeconds(fadeDelayStopLogo);

    //    yield return wait;

    //    startLogo = false;
    //    stopLogo = true;
    //}

    //private IEnumerator StartTitleScreeen()
    //{
    //    var wait = new WaitForSeconds(titleScreenDelay);

    //    yield return wait;

    //    introPanel.SetActive(false);
    //    amb.Play();
    //}
}
