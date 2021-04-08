using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public bool isDrawingLine;
    public Line currentLine;
    // Start is called before the first frame update
    void Start()
    {
        isDrawingLine = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            /*RaycastHit2D hitLeft;

            hitLeft = Physics2D.Raycast(Camera., leftRayDirection, MaxRayLength, layerMask);

            if (hitLeft.collider != null)
            {
                newDirection = hitLeft.collider.transform.position - transform.position;
            }*/

            
            Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            position.z = 0;
            ObjectPooler.Instance.SpawnFromPool("rect", position, Quaternion.identity);
            

            /*Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            position.z = 0;
            currentLine = ObjectPooler.Instance.SpawnFromPool("line", position, Quaternion.identity).GetComponent<Line>();
            currentLine.UpdateLinePointPosition(0, position);
            isDrawingLine = true;*/


        }
        /*if (isDrawingLine)
        {
            Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            position.z = 0;
            currentLine.UpdateLinePointPosition(1, position);
        }*/
        
    }
}
