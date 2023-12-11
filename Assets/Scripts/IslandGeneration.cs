using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandGeneration : MonoBehaviour
{
    [SerializeField] private GameObject[] sand;
    [SerializeField] private GameObject[] grass;
    [SerializeField] private GameObject[] forestTrees;
    [SerializeField] private GameObject[] tropicalTrees;
    [SerializeField] private GameObject[] rocks;
    [SerializeField] private GameObject[] sandObjects;
    [SerializeField] private GameObject[] forestObjects;
    

    private List<GameObject> terrainObjects = new List<GameObject>();
    [SerializeField] private LayerMask grassLayer;
    [SerializeField] private LayerMask sandLayer;
    [SerializeField] private LayerMask treeLayer;
    [SerializeField] private LayerMask greenLayer;
    [SerializeField] private LayerMask waterLayer;
    [SerializeField] private LayerMask rockLayer;

    [SerializeField] private GameObject clouds;

    private void Start()
    {
       clouds.GetComponent<GameObject>();
    }
    public void GenerateIsland()
    {
        Vector3 initialCloudsPosition = clouds.transform.position;

        // Move the clouds to a new position (22.21f in this example) before generating the island
        StartCoroutine(MoveCloudsSmoothly(new Vector3(clouds.transform.position.x, 18.21f, clouds.transform.position.z), 1f, () =>
        {
            // The callback is executed when the first movement is complete
            DestroyTerrain();
            Invoke("GenerateSand", 0.1f);
            Invoke("GenerateGrass", 0.1f);
            Invoke("GenerateTrees", 0.2f);
            Invoke("GenerateJungleTrees", 0.2f);
            Invoke("GenerateForestObjects", 0.4f);
            Invoke("GenerateSandObjects", 0.3f);
            Invoke("GenerateRocks", 0.3f);

            // Add a delay after generating the island (for example, 2 seconds)
            StartCoroutine(DelayedAction(0.5f, () =>
            {
                // After the delay, move the clouds back to their initial position
                StartCoroutine(MoveCloudsSmoothly(initialCloudsPosition, 0.5f, () =>
                {
                    // The callback is executed when the second movement is complete
                    // No debug log in this version
                }));
            }));
        }));
    }
    public void GenerateSand()
    {
        
        
        var position = new Vector3(0,0,0);
        GameObject terrain = Instantiate(sand[Random.Range(0, sand.Length)], position, Quaternion.Euler(0, Random.Range(0, 360), 0));
        terrain.transform.localScale = new Vector3(Random.Range(2, 3), 1, Random.Range(2, 3));
        terrainObjects.Add(terrain);

    }

    public void GenerateGrass()
    {
        ShuffleArray(grass);

        for (int i = 0; i < Random.Range(2,grass.Length +1); i++)
        {
            
            int maxAttempts = 1000;
            int attempt = 0;
            var position = new Vector3(Random.Range(-15,15), 0, Random.Range(-15, 15));

            Collider[] hitColliders = Physics.OverlapBox(position, grass[i].GetComponentInChildren<MeshRenderer>().bounds.size* 0.6f, Quaternion.identity, grassLayer);
            Debug.Log(hitColliders.Length);

            if((hitColliders.Length > 0))
            {

                do
                {
                    attempt++;
                    position = new Vector3(Random.Range(-15, 15), 0, Random.Range(-15, 15));
                    hitColliders = Physics.OverlapBox(position, grass[i].GetComponentInChildren<MeshRenderer>().bounds.size * 0.6f, Quaternion.identity, grassLayer);
                
                    if(attempt >= maxAttempts)
                    {

                        Debug.Log("Max attempts reached");
                        break;
                    }

                }while ((hitColliders.Length > 0));


            }

            RaycastHit hitInfo;
            if (Physics.Raycast(position, Vector3.down, out hitInfo, Mathf.Infinity, waterLayer))
            {
                position.y = hitInfo.point.y; // Set the y-coordinate to the hit point on the terrain
                

            }
            GameObject terrain = Instantiate(grass[i], position, Quaternion.Euler(0, Random.Range(0, 360), 0));
            terrainObjects.Add(terrain);
            //hitColliders = null;
        }
        

        
    }

    public void GenerateTrees()
    {

        ShuffleArray(forestTrees);

        for (int i = 0; i <  forestTrees.Length; i++)
        {

            int maxAttempts = 1000;
            int attempt = 0;
            var position = new Vector3(Random.Range(-20, 20), 4, Random.Range(-20, 20));
            RaycastHit hitInfo;
            Collider[] hitColliders = Physics.OverlapBox(position, forestTrees[i].GetComponent<Collider>().bounds.size * 0.3f, Quaternion.identity, treeLayer);
            Debug.Log(hitColliders.Length);

            do
            {
                attempt++;
                position = new Vector3(Random.Range(-20, 20), 4, Random.Range(-20, 20));
                hitColliders = Physics.OverlapBox(position, forestTrees[i].GetComponent<Collider>().bounds.size * 0.3f, Quaternion.identity, treeLayer);

                if (attempt >= maxAttempts)
                {

                    Debug.Log("Max attempts reached");
                    break;
                }

            } while ((hitColliders.Length > 0) || (!Physics.Raycast(position, Vector3.down, out hitInfo, Mathf.Infinity, grassLayer)) || !(hitInfo.collider is MeshCollider));


            if(Physics.Raycast(position, Vector3.down, Mathf.Infinity, grassLayer)){

                Debug.Log("Hit!");


            }


            if (Physics.Raycast(position, Vector3.down, out hitInfo, Mathf.Infinity, grassLayer))
            {
                // Set the y-coordinate to the hit point on the terrain
                position.y = hitInfo.point.y;

                // Attempt to find a new adjusted position up to 10 times
                for (int j = 0; j < 70; j++)
                {
                    // Adjust position towards the center of the grass object in x and z coordinates
                    Vector3 grassCenter = hitInfo.collider.bounds.center;
                    Vector3 directionToCenter = grassCenter - position;

                    // Move the position slightly towards the center, maintaining the y-coordinate
                    float adjustmentDistance = Random.Range(0.5f, 0.8f);
                    Vector3 adjustedPosition = position + directionToCenter.normalized * adjustmentDistance;

                    // Check for collisions at the adjusted position
                    Collider[] collidersAtAdjustedPosition = Physics.OverlapBox(adjustedPosition, forestTrees[i].GetComponent<Collider>().bounds.size * 0.3f, Quaternion.identity, treeLayer);

                    // If there are no collisions at the adjusted position, update the x and z components of the position and break out of the loop
                    if (collidersAtAdjustedPosition.Length == 0)
                    {
                        position.x = adjustedPosition.x;
                        position.z = adjustedPosition.z;
                        break; // Exit the loop if a valid position is found
                    }
                }

                // Ensure the y-coordinate is updated after adjusting x and z
                position.y = hitInfo.point.y;
            }


                GameObject terrain = Instantiate(forestTrees[i], position, Quaternion.Euler(0, Random.Range(0, 360), 0));
                terrain.transform.localScale = terrain.transform.localScale*Random.Range(1f, 1.3f);
                terrainObjects.Add(terrain);

            
            
            
        }


    }

    public void GenerateJungleTrees()
    {

        ShuffleArray(tropicalTrees);

        for (int i = 0; i < tropicalTrees.Length; i++)
        {

            int maxAttempts = 1000;
            int attempt = 0;
            var position = new Vector3(Random.Range(-15, 15), 1, Random.Range(-15, 15));
            RaycastHit hitInfo;
            Collider[] hitColliders = Physics.OverlapBox(position, tropicalTrees[i].GetComponent<Collider>().bounds.size * 0.5f, Quaternion.identity, greenLayer);
            Debug.Log(hitColliders.Length);

            do
            {
                attempt++;
                position = new Vector3(Random.Range(-15, 15), 1, Random.Range(-15, 15));
                hitColliders = Physics.OverlapBox(position, tropicalTrees[i].GetComponent<Collider>().bounds.size * 0.5f, Quaternion.identity, greenLayer);

                if (attempt >= maxAttempts)
                {

                    Debug.Log("Max attempts reached");
                    break;
                }

            } while ((hitColliders.Length > 0) || (!Physics.Raycast(position, Vector3.down, out hitInfo, Mathf.Infinity, sandLayer)) || !(hitInfo.collider is MeshCollider));


            if (Physics.Raycast(position, Vector3.down, Mathf.Infinity, sandLayer))
            {

                Debug.Log("Hit!");


            }


            if (Physics.Raycast(position, Vector3.down, out hitInfo, Mathf.Infinity, sandLayer))
            {
                // Set the y-coordinate to the hit point on the terrain
                position.y = hitInfo.point.y;

                // Attempt to find a new adjusted position up to 10 times
                for (int j = 0; j < 100; j++)
                {
                    // Adjust position towards the center of the grass object in x and z coordinates
                    Vector3 grassCenter = hitInfo.collider.bounds.center;
                    Vector3 directionToCenter = grassCenter - position;

                    // Move the position slightly towards the center, maintaining the y-coordinate
                    float adjustmentDistance = Random.Range(1f, 6.0f);
                    Vector3 adjustedPosition = position + directionToCenter.normalized * adjustmentDistance;

                    // Check for collisions at the adjusted position
                    Collider[] collidersAtAdjustedPosition = Physics.OverlapBox(adjustedPosition, tropicalTrees[i].GetComponent<Collider>().bounds.size * 0.2f, Quaternion.identity, greenLayer);
                    
                    // If there are no collisions at the adjusted position, update the x and z components of the position and break out of the loop
                    if (collidersAtAdjustedPosition.Length == 0 && Physics.Raycast( new Vector3(adjustedPosition.x, 2, adjustedPosition.z), Vector3.down, Mathf.Infinity, sandLayer))
                    {
                        position.x = adjustedPosition.x;
                        position.z = adjustedPosition.z;
                        break; // Exit the loop if a valid position is found
                    }
                }

                // Ensure the y-coordinate is updated after adjusting x and z
                position.y = hitInfo.point.y;
            }


            GameObject terrain = Instantiate(tropicalTrees[i], position, Quaternion.Euler(0, Random.Range(0, 360), 0));
            terrain.transform.localScale = terrain.transform.localScale * Random.Range(0.8f, 1f);
            terrainObjects.Add(terrain);




        }


    }

    public void GenerateRocks()
    {


        ShuffleArray(rocks);

        for (int i = 0; i < rocks.Length; i++)
        {

            int maxAttempts = 1000;
            int attempt = 0;
            var position = new Vector3(Random.Range(-18, 18), 2, Random.Range(-18, 18));
            RaycastHit hitInfo;
            Collider[] hitColliders = Physics.OverlapBox(position, rocks[i].GetComponent<Collider>().bounds.size * 0.2f, Quaternion.identity, rockLayer);
            Debug.Log(hitColliders.Length);

            do
            {
                attempt++;
                position = new Vector3(Random.Range(-18, 18), 2, Random.Range(-18, 18));
                hitColliders = Physics.OverlapBox(position, rocks[i].GetComponent<Collider>().bounds.size * 0.2f, Quaternion.identity, rockLayer);

                if (attempt >= maxAttempts)
                {

                    Debug.Log("Max attempts reached");
                    break;
                }

            } while ((hitColliders.Length > 0) || (!Physics.BoxCast(position, rocks[i].GetComponent<Collider>().bounds.size * 0.5f, Vector3.down, out hitInfo, Quaternion.identity, Mathf.Infinity, waterLayer)) || (Physics.Raycast(position, Vector3.down, out hitInfo, Mathf.Infinity, sandLayer)));


            


            if (Physics.Raycast(position, Vector3.down, out hitInfo, Mathf.Infinity, waterLayer))
            {
                // Set the y-coordinate to the hit point on the terrain
                position.y = hitInfo.point.y - 0.2f;

                
            }


            GameObject terrain = Instantiate(rocks[i], position, Quaternion.Euler(0, Random.Range(0, 360), 0));
            terrain.transform.localScale = terrain.transform.localScale * Random.Range(0.8f, 1f);
            terrainObjects.Add(terrain);




        }


        }
    
    public void GenerateSandObjects()
    {
        ShuffleArray(sandObjects);

        for (int i = 0; i < sandObjects.Length; i++)
        {

            int maxAttempts = 1000;
            int attempt = 0;
            var position = new Vector3(Random.Range(-15, 15), 1, Random.Range(-15, 15));
            RaycastHit hitInfo;
            Collider[] hitColliders = Physics.OverlapBox(position, sandObjects[i].GetComponent<Collider>().bounds.size * 0.5f, Quaternion.identity, greenLayer);
            Debug.Log(hitColliders.Length);

            do
            {
                attempt++;
                position = new Vector3(Random.Range(-15, 15), 1, Random.Range(-15, 15));
                hitColliders = Physics.OverlapBox(position, sandObjects[i].GetComponent<Collider>().bounds.size * 0.5f, Quaternion.identity, greenLayer);

                if (attempt >= maxAttempts)
                {

                    Debug.Log("Max attempts reached");
                    break;
                }

            } while ((hitColliders.Length > 0) || (!Physics.Raycast(position, Vector3.down, out hitInfo, Mathf.Infinity, sandLayer)) || !(hitInfo.collider is MeshCollider));


            if (Physics.Raycast(position, Vector3.down, Mathf.Infinity, sandLayer))
            {

                Debug.Log("Hit!");


            }


            if (Physics.Raycast(position, Vector3.down, out hitInfo, Mathf.Infinity, sandLayer))
            {
                // Set the y-coordinate to the hit point on the terrain
                position.y = hitInfo.point.y;

                // Attempt to find a new adjusted position up to 10 times
                for (int j = 0; j < 70; j++)
                {
                    // Adjust position towards the center of the grass object in x and z coordinates
                    Vector3 grassCenter = hitInfo.collider.bounds.center;
                    Vector3 directionToCenter = grassCenter - position;

                    // Move the position slightly towards the center, maintaining the y-coordinate
                    float adjustmentDistance = Random.Range(0.5f, 1f);
                    Vector3 adjustedPosition = position + directionToCenter.normalized * adjustmentDistance;

                    // Check for collisions at the adjusted position
                    Collider[] collidersAtAdjustedPosition = Physics.OverlapBox(adjustedPosition, sandObjects[i].GetComponent<Collider>().bounds.size * 0.2f, Quaternion.identity, greenLayer);

                    // If there are no collisions at the adjusted position, update the x and z components of the position and break out of the loop
                    if (collidersAtAdjustedPosition.Length == 0)
                    {
                        position.x = adjustedPosition.x;
                        position.z = adjustedPosition.z;
                        break; // Exit the loop if a valid position is found
                    }
                }

                // Ensure the y-coordinate is updated after adjusting x and z
                position.y = hitInfo.point.y;
            }


            GameObject terrain = Instantiate(sandObjects[i], position, Quaternion.Euler(0, Random.Range(0, 360), 0));
            terrain.transform.localScale = terrain.transform.localScale * Random.Range(0.8f, 1f);
            terrainObjects.Add(terrain);




        }




    }

    public void GenerateForestObjects()
    {
        ShuffleArray(forestObjects);

        for (int i = 0; i < forestObjects.Length; i++)
        {

            int maxAttempts = 1000;
            int attempt = 0;
            var position = new Vector3(Random.Range(-20, 20), 4, Random.Range(-20, 20));
            RaycastHit hitInfo;
            RaycastHit hitTree;
            //Collider[] hitColliders = Physics.OverlapBox(position, forestObjects[i].GetComponent<Collider>().bounds.size * 0.3f, Quaternion.identity, treeLayer);
            Physics.BoxCast(position, forestObjects[i].GetComponent<Collider>().bounds.size * 0.7f, Vector3.down, out hitTree, Quaternion.identity, Mathf.Infinity, treeLayer);
            //Debug.Log(hitColliders.Length);

            do
            {
                attempt++;
                position = new Vector3(Random.Range(-20, 20), 4, Random.Range(-20, 20));
                //hitColliders = Physics.OverlapBox(position, forestObjects[i].GetComponent<Collider>().bounds.size * 0.3f, Quaternion.identity, treeLayer);

                if (attempt >= maxAttempts)
                {

                    Debug.Log("Max attempts reached");
                    break;
                }

            } while ((hitTree.collider != null) || (!Physics.BoxCast(position, forestObjects[i].GetComponent<Collider>().bounds.size * 0.7f, Vector3.down, out hitInfo, Quaternion.identity, Mathf.Infinity, grassLayer)) || !(hitInfo.collider is MeshCollider));


            if (Physics.Raycast(position, Vector3.down, Mathf.Infinity, grassLayer))
            {

                Debug.Log("Hit!");


            }
            

            if (Physics.Raycast(position, Vector3.down, out hitInfo, Mathf.Infinity, grassLayer))
            {
                // Set the y-coordinate to the hit point on the terrain
                position.y = hitInfo.point.y;

                // Attempt to find a new adjusted position up to 10 times
                for (int j = 0; j < 70; j++)
                {
                    // Adjust position towards the center of the grass object in x and z coordinates
                    Vector3 grassCenter = hitInfo.collider.bounds.center;
                    Vector3 directionToCenter = grassCenter - position;

                    // Move the position slightly towards the center, maintaining the y-coordinate
                    float adjustmentDistance = Random.Range(0.5f, 2f);
                    Vector3 adjustedPosition = position + directionToCenter.normalized * adjustmentDistance;

                    // Check for collisions at the adjusted position
                    Collider[] collidersAtAdjustedPosition = Physics.OverlapBox(adjustedPosition, forestObjects[i].GetComponent<Collider>().bounds.size * 0.3f, Quaternion.identity, treeLayer);

                    // If there are no collisions at the adjusted position, update the x and z components of the position and break out of the loop
                    if (collidersAtAdjustedPosition.Length == 0)
                    {
                        position.x = adjustedPosition.x;
                        position.z = adjustedPosition.z;
                        break; // Exit the loop if a valid position is found
                    }
                }

                // Ensure the y-coordinate is updated after adjusting x and z
                position.y = hitInfo.point.y;
            }

            if(attempt >= maxAttempts)
            {

            }
            else
            {
                GameObject terrain = Instantiate(forestObjects[i], position, Quaternion.identity);
                
                terrainObjects.Add(terrain);

            }
            




        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    






    private IEnumerator MoveCloudsSmoothly(Vector3 targetPosition, float duration, System.Action onComplete = null)
    {
        float elapsedTime = 0f;
        Vector3 initialPosition = clouds.transform.position;

        while (elapsedTime < duration)
        {
            clouds.transform.position = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Ensure reaching the exact target position
        clouds.transform.position = targetPosition;

        onComplete?.Invoke();

    }

    private IEnumerator DelayedAction(float delay, System.Action action)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }

    private void DestroyTerrain()
    {

        
        foreach (var terrain in terrainObjects)

            Destroy(terrain);
        terrainObjects.Clear();

    }

    
    



    void ShuffleArray<T>(T[] array)
    {
        System.Random random = new System.Random();

        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = random.Next(0, i + 1);

            // Swap array[i] and array[j]
            T temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
    }



}
