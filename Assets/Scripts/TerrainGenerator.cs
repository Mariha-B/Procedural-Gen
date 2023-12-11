using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] private GameObject[] sand;
    private List<GameObject> terrainObjects = new List<GameObject>();
    
    [SerializeField] private LayerMask layer;

    private void Start()
    {
        
    }

    public void Generate()
    {   
        DestroyTerrain();

        for (int i = 0; i<sand.Length; i++)
        {


            int maxAttempts = 200;
            int attempt = 0;

            var position = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
            

            Collider[] hitColliders = Physics.OverlapSphere(position, sand[i].GetComponentInChildren<MeshRenderer>().bounds.size.x*0.6f, layer);
            if (hitColliders.Length > 0 && attempt < maxAttempts)
            {
                do
                {
                    attempt++;
                    position = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
                    hitColliders = Physics.OverlapSphere(position, sand[i].GetComponentInChildren<MeshRenderer>().bounds.size.x * 0.6f, layer);
                    
                    
                    

                } while (hitColliders.Length > 0 && attempt < maxAttempts);

                if(attempt == maxAttempts)
                {

                    Debug.Log("Max attempts reached");
                }
                
            }


            GameObject terrain = Instantiate(sand[i], position, Quaternion.Euler(0,Random.Range(0,360),0));
            terrainObjects.Add(terrain);

        }


    }

    private void DestroyTerrain()
    {
        foreach (var terrain in terrainObjects)

            Destroy(terrain);
        terrainObjects.Clear();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        foreach (var terrain in terrainObjects)
        {
            Gizmos.DrawWireSphere(terrain.transform.position, terrain.GetComponentInChildren<MeshRenderer>().bounds.size.x * 0.6f);
        }
    }
}
