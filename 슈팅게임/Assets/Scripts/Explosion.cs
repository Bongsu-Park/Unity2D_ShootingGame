using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {

	Animator anim;

	void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void OnEnable()
    {
        Invoke("Disable", 2f);
    }

    void Disable()
    {
        gameObject.SetActive(false);
    }

    public void StartExplosion(string target)
    {
        anim.SetTrigger("OnExplosion");

        //비활성화 되는 대상의 크기에 따라 스케일 변화 주도록 작성
        switch(target)
        {
            case "P":
                transform.localScale = Vector3.one * 1f;
                break;
            case "S":
                transform.localScale = Vector3.one * 0.7f;
                break;
            case "M":
                transform.localScale = Vector3.one * 1f;
                break;
            case "L":
                transform.localScale = Vector3.one * 2f;
                break;
            case "B":
                transform.localScale = Vector3.one * 3f;
                break;
        }
    }
}
