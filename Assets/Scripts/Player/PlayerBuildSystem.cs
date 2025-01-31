using UnityEngine;

public class PlayerBuildSystem : MonoBehaviour
{
    [SerializeField] BuildInputs GetBuildInputs;
    [SerializeField] Camera PlayerCamera;
    [SerializeField] GameObject TransparentCubePrefab;
    [SerializeField] GameObject SolidCubePrefab;

    private GameObject currentCube;
    private bool isBuildModeActive = false;
    private float cubeSize = 0.5f;

    private void Update()
    {
        if (GetBuildInputs.isBuilding)
        {
            if (!isBuildModeActive)
            {
                EnableBuildMode();
            }

            Vector3 buildDirection = GetBuildInputs.GetBuildDirection();
            if (buildDirection != Vector3.zero)
            {
                MoveCube(buildDirection);
            }
            else if (GetBuildInputs.IsLeftMouseClickPressed())
            {
                MoveCubeWithMouse();
            }

            if (GetBuildInputs.IsPlaceBlockPressed())
            {
                PlaceBlock();
            }
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
        spawnPosition = SnapToGrid(spawnPosition);
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

    private void MoveCube(Vector3 buildDirection)
    {
        if (currentCube == null) return;

        if (buildDirection.x != 0 || buildDirection.z != 0)
        {
            Vector3 cameraForward = PlayerCamera.transform.forward;
            Vector3 cameraRight = PlayerCamera.transform.right;
            cameraForward.y = 0f;
            cameraRight.y = 0f;
            cameraForward.Normalize();
            cameraRight.Normalize();

            Vector3 relativeDirection = (cameraForward * buildDirection.z) + (cameraRight * buildDirection.x);
            relativeDirection.Normalize();

            Vector3 newPosition = currentCube.transform.position + relativeDirection * cubeSize;
            newPosition = SnapToGrid(newPosition);
            currentCube.transform.position = newPosition;
        }

        if (buildDirection.y != 0)
        {
            Vector3 newPosition = currentCube.transform.position + Vector3.up * buildDirection.y * cubeSize;
            newPosition = SnapToGrid(newPosition);
            currentCube.transform.position = newPosition;
        }
    }

    private void MoveCubeWithMouse()
    {
        if (currentCube == null) return;

        Ray ray = PlayerCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 offset = hit.normal * (cubeSize);

            Vector3 newPosition = hit.point + offset;

            newPosition = SnapToGrid(newPosition);

            currentCube.transform.position = newPosition;
        }
    }

    private void PlaceBlock()
    {
        if (currentCube != null)
        {
            Instantiate(SolidCubePrefab, currentCube.transform.position, Quaternion.identity);
        }
    }

    private Vector3 SnapToGrid(Vector3 position)
    {
        position.x = Mathf.Round(position.x / cubeSize) * cubeSize;
        position.y = Mathf.Round(position.y / cubeSize) * cubeSize;
        position.z = Mathf.Round(position.z / cubeSize) * cubeSize;

        return position;
    }
}