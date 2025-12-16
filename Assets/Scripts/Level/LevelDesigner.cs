using UnityEngine;
using Unity.Cinemachine;
using System.IO;

public class LevelDesigner : MonoBehaviour
{
    [Header("Setup")]
    [Tooltip("The platform prefab to instantiate for each grid cell")]
    public GameObject platformPrefab;
    
    [Tooltip("The player prefab to spawn at the start")]
    public GameObject playerPrefab;

    [Tooltip("The virtual camera to follow the player (assign in Inspector or leave null to find automatically)")]
    public CinemachineCamera followCam;
    
    [Tooltip("The goal object prefab to spawn at the end")]
    public GameObject goalPrefab;
    
    [Header("Grid Settings")]
    [Tooltip("Size of the grid (5x5 means 5 platforms in each direction)")]
    public int gridSize = 5;
    
    [Tooltip("Size of each platform in Unity units")]
    public float platformSize = 3f;
    
    [Tooltip("Maximum height variation in meters (each platform will be randomized within this range)")]
    public float heightVariation = 5f;
    
    [Tooltip("Base height for all platforms (Y position)")]
    public float baseHeight = 0f;
    
    // This method is called automatically when the game starts
    void Start()
    {
        GenerateLevel();
    }
    
    // This method creates the entire level
    void GenerateLevel()
    {
        // Check if we have the required prefabs assigned
        if (platformPrefab == null)
        {
            Debug.LogError("Platform Prefab is not assigned in LevelDesigner!");
            return;
        }
        
        // Generate platforms in a grid
        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                // Calculate world position for this grid cell
                // We center the grid around (0, 0, 0)
                float worldX = (x * platformSize) - ((gridSize - 1) * platformSize / 2f);
                float worldZ = (z * platformSize) - ((gridSize - 1) * platformSize / 2f);
                
                // Randomize the height within the specified range
                float randomHeight = baseHeight + Random.Range(-heightVariation / 2f, heightVariation / 2f);
                
                // Create the position vector
                Vector3 platformPosition = new Vector3(worldX, randomHeight, worldZ);
                
                // Instantiate (create) the platform at this position
                GameObject platform = Instantiate(platformPrefab, platformPosition, Quaternion.identity);
                
                // Make the platform a child of this LevelDesigner object (keeps hierarchy clean)
                platform.transform.SetParent(transform);
                
                // Name it so we can see it in the hierarchy
                platform.name = $"Platform_{x}_{z}";
                
                // Spawn player on bottom-left platform (grid position [0, 0])
                if (x == 0 && z == 0 && playerPrefab != null)
                {
                    GameObject player = SpawnPlayer(platformPosition, randomHeight);
                    AssignPlayerToCamera(player);
                }
                
                // Spawn goal on top-right platform (grid position [gridSize-1, gridSize-1])
                if (x == gridSize - 1 && z == gridSize - 1 && goalPrefab != null)
                {
                    SpawnGoal(platformPosition, randomHeight);
                }
            }
        }
    }
    
    // Spawn the player on top of the starting platform
    GameObject SpawnPlayer(Vector3 platformPosition, float platformHeight)
    {
        // Calculate position on top of the platform
        // Platform's top surface is at: platformHeight + (platform height / 2)
        float platformHalfHeight = platformPrefab.transform.localScale.y / 2f;
        float playerHalfHeight = playerPrefab.transform.localScale.y / 2f;
        float extraOffset = 0.5f;
        float playerY = platformHeight + platformHalfHeight + playerHalfHeight + extraOffset;
        Vector3 playerPosition = new Vector3(platformPosition.x, playerY, platformPosition.z);
        
        // Instantiate the player
        GameObject player = Instantiate(playerPrefab, playerPosition, Quaternion.identity);
        player.name = "Player";
        
        // #region agent log
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) {
            Debug.Log($"Player instantiated with CharacterController: height={cc.height}, center={cc.center}, skinWidth={cc.skinWidth}");
        }
        return player;
    }
    
    // Assign the player to the Cinemachine virtual camera's Follow and LookAt targets
    void AssignPlayerToCamera(GameObject player)
    {
        // If virtualCamera is not assigned in Inspector, try to find one in the scene
        if (followCam == null)
        {
            followCam = FindObjectOfType<CinemachineCamera>();
            
            if (followCam == null)
            {
                Debug.LogWarning("CinemachineVirtualCamera not found! Please assign it in the Inspector or add one to the scene.");
                return;
            }
        }
        
        // Assign the player's transform to Follow and LookAt
        if (player != null && followCam != null)
        {
            followCam.Follow = player.transform;
            followCam.LookAt = player.transform;
            Debug.Log($"Player assigned to Cinemachine camera: Follow={player.transform}, LookAt={player.transform}");
        }
        else
        {
            Debug.LogError("Cannot assign player to camera: player or virtualCamera is null!");
        }
    }
    
    // Spawn the goal on top of the end platform
    void SpawnGoal(Vector3 platformPosition, float platformHeight)
    {
        // Calculate position on top of the platform
        float goalY = platformHeight + platformPrefab.transform.localScale.y / 2f; // adds half of the platform's height
        Vector3 goalPosition = new Vector3(platformPosition.x, goalY, platformPosition.z);
        
        // Instantiate the goal
        GameObject goal = Instantiate(goalPrefab, goalPosition, Quaternion.identity);
        goal.name = "Goal";
    }
}

