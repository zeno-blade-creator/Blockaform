using UnityEngine;
using Unity.Cinemachine;
using System.IO;
using System.Collections.Generic;

// Data structure to store original platform information
[System.Serializable]
public class PlatformData
{
    public Vector3 position;
    public string name;
    
    public PlatformData(Vector3 pos, string platformName)
    {
        position = pos;
        name = platformName;
    }
}

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
    
    [Tooltip("Spacing between each platform in Unity units, should equal the platform size for no overlap")]
    public float spacingBetweenPlatforms = 2f;

    [Tooltip("Size of the platform in Unity units, default is 2")]
    public float platformSize = 2f;
    
    [Tooltip("Maximum height variation in meters (each platform will be randomized within this range)")]
    public float heightVariation = 5f;

    public List<GameObject> platformList = new List<GameObject>();
    
    // Store original platform positions to restore on restart
    private List<PlatformData> originalPlatformData = new List<PlatformData>();
    private Vector3 originalHighestPlatformPosition;
    private Vector3 originalLowestPlatformPosition;
    
    
    // This method creates the entire level
    public void GenerateLevel()
    {
        ClearObjects();
        platformList.Clear();
        originalPlatformData.Clear();

        // Check if we have the required prefabs assigned
        if (platformPrefab == null)
        {
            Debug.LogError("Platform Prefab is not assigned in LevelDesigner!");
            return;
        }
        
        Vector3 highestPlatformPosition = Vector3.zero;
        Vector3 lowestPlatformPosition = Vector3.zero;
        
        // Generate platforms in a grid
        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                // Calculate world position for this grid cell
                // We center the grid around (0, 0, 0)
                float worldX = (x * spacingBetweenPlatforms) - ((gridSize - 1) * spacingBetweenPlatforms / 2f);
                float worldZ = (z * spacingBetweenPlatforms) - ((gridSize - 1) * spacingBetweenPlatforms / 2f);
                
                // Randomize the height within the specified range
                float randomHeight =  Random.Range(-heightVariation / 2f, heightVariation / 2f);
                
                // Create the position vector
                Vector3 platformPosition = new Vector3(worldX, randomHeight, worldZ);

                if (randomHeight > highestPlatformPosition.y)
                {
                    highestPlatformPosition = platformPosition;
                }
                if (randomHeight < lowestPlatformPosition.y)
                {
                    lowestPlatformPosition = platformPosition;
                }
                
                // Store original platform data for restoration
                string platformName = $"Platform_{x}_{z}";
                originalPlatformData.Add(new PlatformData(platformPosition, platformName));
                
                // Instantiate (create) the platform at this position
                GameObject platform = Instantiate(platformPrefab, platformPosition, Quaternion.identity);
                platform.transform.localScale = new Vector3(platformSize, 0.5f, platformSize);
                
                // Make the platform a child of this LevelDesigner object (keeps hierarchy clean)
                platform.transform.SetParent(transform);
                
                // Name it so we can see it in the hierarchy
                platform.name = platformName;
                
                AddPlatformToList(platform);
            }
        }

        // Store original highest/lowest positions for player and goal placement
        originalHighestPlatformPosition = highestPlatformPosition;
        originalLowestPlatformPosition = lowestPlatformPosition;

        // Spawn player on the lowest platform
        if (lowestPlatformPosition != Vector3.zero && playerPrefab != null)
        {
            GameObject player = SpawnPlayer(lowestPlatformPosition, lowestPlatformPosition.y);
            AssignPlayerToCamera(player);
        }

        // Spawn goal on the highest platform
        if (highestPlatformPosition != Vector3.zero && goalPrefab != null)
        {
            SpawnGoal(highestPlatformPosition, highestPlatformPosition.y);
        }
    }

    void AddPlatformToList(GameObject platform)
    {
        platformList.Add(platform);
    }

    public void RestartLevel()
    {
        ClearObjects();
        
        // If we have stored original platform data, restore from it
        // Otherwise, generate a new level (first time)
        if (originalPlatformData.Count > 0)
        {
            RestorePlatformsFromData();
        }
        else
        {
            GenerateLevel();
        }
    }
    
    // Restore platforms to their original positions
    void RestorePlatformsFromData()
    {
        platformList.Clear();
        
        // Recreate all platforms at their original positions
        foreach (PlatformData data in originalPlatformData)
        {
            GameObject platform = Instantiate(platformPrefab, data.position, Quaternion.identity);
            platform.transform.localScale = new Vector3(platformSize, 0.5f, platformSize);
            platform.transform.SetParent(transform);
            platform.name = data.name;
            AddPlatformToList(platform);
        }
        
        // Respawn player on the original lowest platform
        if (originalLowestPlatformPosition != Vector3.zero && playerPrefab != null)
        {
            GameObject player = SpawnPlayer(originalLowestPlatformPosition, originalLowestPlatformPosition.y);
            AssignPlayerToCamera(player);
        }
        
        // Respawn goal on the original highest platform
        if (originalHighestPlatformPosition != Vector3.zero && goalPrefab != null)
        {
            SpawnGoal(originalHighestPlatformPosition, originalHighestPlatformPosition.y);
        }
    }

    void ClearObjects()
    {
        // Destroy all platforms in the list (check for null in case some were already destroyed)
        foreach (GameObject platform in platformList)
        {
            if (platform != null)
            {
                Destroy(platform);
            }
        }

        GameObject oldPlayer = GameObject.Find("Player");
        if (oldPlayer != null)
        {
            Destroy(oldPlayer);
        }

        GameObject oldGoal = GameObject.Find("Goal");
        if (oldGoal != null)
        {
            Destroy(oldGoal);
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
            followCam = GameObject.FindFirstObjectByType<CinemachineCamera>();
            
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

