

using UnityEngine;
using System.Collections;
using Piacenti.Pooling;
public class MudLauncher : MonoBehaviour {
    [SerializeField]
    private MonoPool pool;

    private void Start() {
        StartCoroutine(StartLaunching());
    }
    private IEnumerator StartLaunching() {
        while (true)
        {
            GameObject projectile = pool.MyPool.GetInstance();
            projectile.transform.position = transform.position;
            projectile.GetComponent<Rigidbody>().velocity = Vector3.zero;
            projectile.SetActive(true);
            yield return new WaitForSeconds(1);

        }
    }
}
