using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShootScript : MonoBehaviour
{
    public float damage, range, rate, coolTime;
    public float damage2, range2, rate2, coolTime2;
    public float spread;
    public int shotsFired;
    public bool isPiercer;
    public Camera cam;
    public Transform prefab;
    public LayerMask PlayerLayerMask;
    // Start is called before the first frame update
    public virtual void Shoot()
    {
        for (int i = 0; i < shotsFired; i++)
        {
            Vector3 angle = Quaternion.Euler(Random.Range(-spread, spread), Random.Range(-spread, spread), Random.Range(-spread, spread)) * cam.transform.forward;
            //Debug.Log(Mathf.Sin(spread));
            if (Physics.Raycast(cam.transform.position, angle, out RaycastHit hit, range, PlayerLayerMask))
            {
                //Debug.Log(hit.transform.name);
                Instantiate(prefab, hit.point, transform.rotation);
            }
        }
    }
    public virtual void Shoot2()
    {
        for (int i = 0; i < shotsFired; i++)
        {
            Vector3 angle = Quaternion.Euler(Random.Range(-spread, spread), Random.Range(-spread, spread), Random.Range(-spread, spread)) * cam.transform.forward;
            //Debug.Log(angle);
            RaycastHit[] hits;
            hits = Physics.RaycastAll(cam.transform.position, angle, range, PlayerLayerMask);
            System.Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));
            if (isPiercer)
            {
                for (int o = 0; o < hits.Length; o++)
                {
                    RaycastHit hit = hits[o];
                    Debug.Log(hit.transform.name);
                    Instantiate(prefab, hit.point, transform.rotation);
                }
            }
            else
            {
                if (0 < hits.Length)
                {
                    RaycastHit hit = hits[0];
                    Debug.Log(hit.transform.name);
                    Instantiate(prefab, hit.point, transform.rotation);
                }
            }
        }
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (Input.GetButton("Fire1") && Time.time >= coolTime && Time.time >= coolTime2)
        {
            coolTime = Time.time + rate / 1000;
            Shoot();
        }
        if (Input.GetButton("Fire2") && Time.time >= coolTime && Time.time >= coolTime2)
        {
            coolTime2 = Time.time + rate2 / 1000;
            Shoot2();
        }
    }
    /*IEnumerator ExampleCoroutine()
    {
        cut = false;
        RaycastHit hit;
        currMag--;
        for (int i = 0; i < shotsFired; i++) 
        {
            Vector3 angle = cam.transform.forward + new Vector3(Random.Range(-spread, spread), Random.Range(-spread, spread), Random.Range(-spread, spread));
            if (Physics.Raycast(cam.transform.position, angle, out hit, range))
            {
                Debug.Log(hit.transform.name);
                Instantiate(prefab, hit.point, Quaternion.identity);
            }
        }
        animator.SetBool("isWalking", true);
        yield return new WaitForSeconds(0.10f); 
        animator.SetBool("isWalking", false);
        Debug.Log(animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"));
        if (currMag <= 0)
        {
            for (int i = 1; i < 7; i++)
            {
                main.sprite = spr[i];
                size.rect.Set(spr[i].rect.x * 4, spr[i].rect.y * 4, spr[i].rect.width * 4, spr[i].rect.height * 4);
                yield return new WaitForSeconds(0.33f);
            }
            currMag = mag;
            main.sprite = spr[0];
            yield return new WaitForSeconds(0.33f);
        }
        
        //spread += 0.01f;
        cut = true;
    }*/
}
