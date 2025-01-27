using UnityEngine;

public class PlayerBuildSystem : MonoBehaviour
{
    [SerializeField] BuildInputs GetBuildInputs;
    [SerializeField] Camera PlayerCamera;
    [SerializeField] GameObject TransparentCubePrefab;

    private GameObject currentCube;
    private bool isBuildModeActive = false;
    private float cubeSize = 1.0f; // Assuming the cube has a size of 1 unit

    private void Update()
    {
        if (GetBuildInputs.isBuilding)
        {
            if (!isBuildModeActive)
            {
                EnableBuildMode();
            }

            MoveCube();
        }
        else if (isBuildModeActive)
        {
            DisableBuildMode();
        }
    }

    private void EnableBuildMode()
    {
        isBuildModeActive = true;
        Vector3 spawnPosition = PlayerCamera.transform.position + PlayerCamera.transform.forward * 5;
        spawnPosition = SnapToGrid(spawnPosition); // Snap the spawn position to the grid
        currentCube = Instantiate(TransparentCubePrefab, spawnPosition, Quaternion.identity);
    }

    private void DisableBuildMode()
    {
        isBuildModeActive = false;
        if (currentCube != null)
        {
            Destroy(currentCube);
        }
    }

    private void MoveCube()
    {
        if (currentCube == null) return;

        Vector3 buildDirection = GetBuildInputs.GetBuildDirection();
        if (buildDirection != Vector3.zero)
        {
            // Move the cube in discrete steps (one grid unit at a time)
            Vector3 relativeDirection = PlayerCamera.transform.TransformDirection(buildDirection);
            relativeDirection.Normalize(); // Ensure consistent movement speed

            // Calculate the new position by adding the direction (scaled by cubeSize)
            Vector3 newPosition = currentCube.transform.position + relativeDirection * cubeSize;

            // Snap the new position to the grid
            newPosition = SnapToGrid(newPosition);

            // Update the cube's position
            currentCube.transform.position = newPosition;
        }
    }

    private Vector3 SnapToGrid(Vector3 position)
    {
        // Snap the position to the nearest grid point based on the cube size
        position.x = Mathf.Round(position.x / cubeSize) * cubeSize;
        position.y = Mathf.Round(position.y / cubeSize) * cubeSize; // Snap Y-axis
        position.z = Mathf.Round(position.z / cubeSize) * cubeSize;

        return position;
    }
}