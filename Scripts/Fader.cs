using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Fade_Complete_Event : UnityEvent { }

public class Fader : MonoBehaviour
{
    static Fader Instance = null;

    public Text m_Desc;

    public Fade_Complete_Event m_FCE = new Fade_Complete_Event();

    public static Fader Start_Fade(string text)
    {
        if(Instance == null)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefab/Fader");
            GameObject obj = Instantiate(prefab);
            Instance = obj.GetComponent<Fader>();
            Instance.m_Desc.text = text;
            DontDestroyOnLoad(obj);
        }
        return Instance;
    }

    IEnumerator Fade_Complete()
    {
        yield return new WaitForSecondsRealtime(1f);

        m_FCE.Invoke();

        yield return null;
    }

    public static void End_Fade()
    {
        if(Instance != null)
        {
            Instance.GetComponent<Animation>().Play("Fader_Out");
            Destroy(Instance.gameObject, 1.5f);
        }
    }

    private void Start()
    {
        StartCoroutine(Fade_Complete());
    }
}
