using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class Generator : MonoBehaviour
{
    [SerializeField] private GameObject[] sand;
    //private List<Collider> colliders;
    //private Collider[] hitColliders = new Collider[1000];
    [SerializeField] private LayerMask layer;
    private List<GameObject> terrainObjects = new List<GameObject>();

    void Start()
    {
        
        
    }
    public void SandTerrain()
    {
        
        DestroyTerrain();
        Collider[] hitColliders = new Collider[10000];

        for (int i = 0; i < sand.Length; i++)
        {
            //List<Collider> colliders = new List<Collider>();
            
            var position = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
            
            int collisions = Physics.OverlapSphereNonAlloc(position, sand[i].GetComponentInChildren<MeshRenderer>().bounds.size.z, hitColliders, layer);
            Debug.Log($"Collisions: {collisions}");
            if (collisions >=1 ){

                do
                {
                    position = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
                    Array.Clear(hitColliders, 0, hitColliders.Length);
                    collisions = Physics.OverlapSphereNonAlloc(position, sand[i].GetComponentInChildren<MeshRenderer>().bounds.size.z , hitColliders, layer);
                } while (collisions >= 1);
                
                

            }
            GameObject terrain = Instantiate(sand[i], position, Quaternion.identity);
            terrainObjects.Add(terrain);
            //



        }



    }


    private void DestroyTerrain()
    {
        foreach (var terrain in terrainObjects)

            Destroy(terrain);
            terrainObjects.Clear();
    }


    

}
