using System.Collections;
using UnityEngine;

public class SegmentGenerator : MonoBehaviour
{
    public GameObject[] segment;

    [SerializeField] int zPos = 50;
    [SerializeField] bool creatingSegment = false;
    [SerializeField] int segmenNum;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Update()
    {
        if (creatingSegment == false)
        {
            creatingSegment = true;
            StartCoroutine(SegmentGen());
        }
    }

    IEnumerator SegmentGen()
    {
        segmenNum = Random.Range(0, 3);
        Instantiate(segment[segmenNum], new Vector3(0, 0, zPos), Quaternion.identity);
        zPos += 50;
        yield return new WaitForSeconds(3);
        creatingSegment = false;
    }
}
