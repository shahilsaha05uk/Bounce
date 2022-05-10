using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            StartCoroutine(Respawn());
        }
    }

    IEnumerator Respawn()
    {
        PlayerController.instance.gameObject.SetActive(false);
        SplatController.instance.MakeSplat();

        yield return new WaitForSeconds(1f);

        PlayerController.instance.transform.position = PlayerController.instance.startPoint;
        PlayerController.instance.gameObject.SetActive(true);
    }
}
