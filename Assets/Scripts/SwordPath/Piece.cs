using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    [SerializeField]
    int ans;
    Transform towardT;
    // Start is called before the first frame update
    void Start()
    {
        //towardT = GameObject.Find("fish_Cube.006").GetComponent<Transform>();
        //gameObject.transform.LookAt(towardT);
        //gameObject.transform.Rotate(Vector3.right, 60, Space.Self);
        //gameObject.transform.Rotate(Vector3.down, 30 * ans, Space.Self);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Die()
    {
        // hit success then die;
        StartCoroutine(DieAni());
    }

    public void SetAns(int given) {
        ans = given;
    }

    IEnumerator DieAni() {
        int interationTime = 10;
        for (int j = 0; j < 4; j++) {
            for (int i = 0; i < interationTime; i++)
            {
                gameObject.transform.Rotate(Vector3.down, 2, Space.Self);
                yield return null;
            }
            for (int i = 0; i < interationTime; i++)
            {
                gameObject.transform.Rotate(Vector3.down, -2.2f, Space.Self);
                yield return null;
            }
        }
        Destroy(gameObject, 0.3f);
        yield return null;
    }
}
