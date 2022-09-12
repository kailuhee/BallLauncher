using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class BallHandler : MonoBehaviour
{
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Rigidbody2D pivot;
    [SerializeField] private float detachBallDelay;
    [SerializeField] private float respawnDelay;

    private Rigidbody2D currentBallRigidBody;
    private SpringJoint2D currentBallSpringJoint;
    private Camera mainCamera;
    private bool isDragging;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        SpawnNewBall();
    }

    void OnEnable() {
        EnhancedTouchSupport.Enable();
    }

    void OnDisable() {
        EnhancedTouchSupport.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentBallRigidBody == null) {
            return;
        }

        if (Touch.activeTouches.Count == 0) 
        {
            if (isDragging) {
                LaunchBall();
            }
            isDragging = false;
            return;
        }

        Vector2 touchPosition = new Vector2();

        foreach (Touch touch in Touch.activeTouches) {
            touchPosition += touch.screenPosition;
        }

        touchPosition /= Touch.activeTouches.Count;

        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(touchPosition);

        currentBallRigidBody.position = worldPosition;

        isDragging = true;
        currentBallRigidBody.isKinematic = true;
    }

    private void SpawnNewBall() {
        GameObject ballInstance = Instantiate(ballPrefab,pivot.position, Quaternion.identity);

        currentBallRigidBody = ballInstance.GetComponent<Rigidbody2D>();
        currentBallSpringJoint = ballInstance.GetComponent<SpringJoint2D>();

        currentBallSpringJoint.connectedBody = pivot;
    }

    private void LaunchBall() {
        currentBallRigidBody.isKinematic = false;
        currentBallRigidBody = null;

        Invoke(nameof(DetachBall), detachBallDelay);
    }

    private void DetachBall() {
        currentBallSpringJoint.enabled = false;
        currentBallSpringJoint = null;

        Invoke(nameof(SpawnNewBall), respawnDelay);
    }
}
