using UnityEngine;
using UnityEngine.InputSystem;

public class RabbitLauncher : MonoBehaviour
{
    [SerializeField] private Transform slingAnchor;
    [SerializeField] private GameObject[] rabbitPrefabs;
    [SerializeField] private float maxDragDistance = 2.5f;
    [SerializeField] private float launchPower = 8f;
    [SerializeField] private float respawnDelay = 2.5f;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LineRenderer trajectoryLine;
    [SerializeField] private int trajectoryPointCount = 30;
    [SerializeField] private float trajectoryTimeStep = 0.08f;

    private int rabbitIndex;
    private GameObject currentRabbit;
    private Rigidbody currentRabbitRb;
    private bool dragging;
    private bool rabbitReady;

    private void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        GameManager.Instance?.SetTotalRabbits(rabbitPrefabs.Length);
        SpawnNextRabbit();
    }

    private void Update()
    {
        if (!rabbitReady || currentRabbit == null)
            return;

        var mouse = Mouse.current;
        if (mouse == null)
            return;

        if (mouse.leftButton.wasPressedThisFrame)
        {
            dragging = true;
        }
        else if (mouse.leftButton.wasReleasedThisFrame && dragging)
        {
            dragging = false;
            LaunchRabbit();
        }

        if (dragging)
        {
            Vector3 offset = ClampOffset(GetMouseWorldPoint() - slingAnchor.position);
            currentRabbit.transform.position = slingAnchor.position + offset;
            UpdateTrajectoryPreview(offset);
        }
        else
        {
            HideTrajectoryPreview();
        }
    }

    private void UpdateTrajectoryPreview(Vector3 offset)
    {
        if (trajectoryLine == null)
            return;

        Vector3 startPos = slingAnchor.position + offset;
        Vector3 velocity = (slingAnchor.position - startPos) * launchPower;
        Vector3 gravity = Physics.gravity;

        trajectoryLine.enabled = true;
        trajectoryLine.positionCount = trajectoryPointCount;

        int pointsUsed = trajectoryPointCount;
        for (int i = 0; i < trajectoryPointCount; i++)
        {
            float t = i * trajectoryTimeStep;
            Vector3 point = startPos + velocity * t + 0.5f * gravity * t * t;
            trajectoryLine.SetPosition(i, point);

            if (point.y <= 0f && i > 0)
            {
                pointsUsed = i + 1;
                break;
            }
        }

        trajectoryLine.positionCount = pointsUsed;
    }

    private void HideTrajectoryPreview()
    {
        if (trajectoryLine != null)
            trajectoryLine.enabled = false;
    }

    private Vector3 ClampOffset(Vector3 offset)
    {
        // Keep the rabbit on the sling's plane and only allow pulling back
        // (away from the target), so every shot flies toward the structure.
        offset.z = 0f;
        if (offset.x > 0f)
            offset.x = 0f;

        if (offset.magnitude > maxDragDistance)
            offset = offset.normalized * maxDragDistance;

        return offset;
    }

    private Vector3 GetMouseWorldPoint()
    {
        // A plane facing the camera at the sling's depth, so dragging the
        // mouse moves the rabbit in the same X/Y plane as the slingshot.
        Plane plane = new Plane(Vector3.forward, slingAnchor.position);
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (plane.Raycast(ray, out float distance))
            return ray.GetPoint(distance);
        return slingAnchor.position;
    }

    private void LaunchRabbit()
    {
        Vector3 dragVector = slingAnchor.position - currentRabbit.transform.position;

        currentRabbitRb.isKinematic = false;
        currentRabbitRb.linearVelocity = dragVector * launchPower;

        HideTrajectoryPreview();

        rabbitReady = false;
        currentRabbit = null;
        currentRabbitRb = null;

        Invoke(nameof(SpawnNextRabbit), respawnDelay);
    }

    private void SpawnNextRabbit()
    {
        GameManager.Instance?.UpdateRabbitsRemaining(Mathf.Max(0, rabbitPrefabs.Length - rabbitIndex));

        if (rabbitIndex >= rabbitPrefabs.Length)
            return;

        currentRabbit = Instantiate(rabbitPrefabs[rabbitIndex], slingAnchor.position, Quaternion.identity);
        currentRabbitRb = currentRabbit.GetComponent<Rigidbody>();
        currentRabbitRb.isKinematic = true;
        rabbitIndex++;
        rabbitReady = true;
    }

    public void LaunchRabbitForTest(Vector3 dragOffset)
    {
        if (!rabbitReady || currentRabbit == null)
            return;

        currentRabbit.transform.position = slingAnchor.position + ClampOffset(dragOffset);
        LaunchRabbit();
    }

    public void PreviewTrajectoryForTest(Vector3 dragOffset)
    {
        if (!rabbitReady || currentRabbit == null)
            return;

        dragging = true;
        Vector3 offset = ClampOffset(dragOffset);
        currentRabbit.transform.position = slingAnchor.position + offset;
        UpdateTrajectoryPreview(offset);
    }
}
