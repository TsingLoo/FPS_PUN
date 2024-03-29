using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRun : MonoBehaviour
{



    [SerializeField] Transform orientation;

    [Header("Detection")]
    [SerializeField] float wallDistance = 1f;
    [SerializeField] float minimumJumpHeight = 1.5f;

    [Header("Wall Running")]
    [SerializeField] private float wallRunGravity;
    [SerializeField] private float wallRunJumpForce;

    [Header("Camera")]
    [SerializeField] private Camera cam;
    [SerializeField] private float fov;
    [SerializeField] private float wallRunfov;
    [SerializeField] private float wallRunfovTime;
    [SerializeField] private float camTilt;
    [SerializeField] private float camTiltTime;



    public float tilt { get; private set; }

    bool wallLeft = false;
    bool wallRight = false;

    RaycastHit leftWallHit;
    RaycastHit rightWallHit;

    PhotonView PV;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
    }


    void CheckWall()
    {
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallDistance);
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallDistance);
    }

    // Start is called before the first frame update


    bool CanWallRun()
    {
  
        Gizmos.DrawRay(transform.position, Vector3.down);
        return !Physics.Raycast(transform.position, Vector3.down, minimumJumpHeight);
    }

    void Update()
    {
        if (!PV.IsMine)
        {
            return;
        }

        CheckWall();

        if (true)
        {
            if (wallLeft)
            {

                StartWallRun();
                Debug.Log("[WallRun][Player]Wall running on the left");
            }

            else if (wallRight)
            {
                StartWallRun();
                   Debug.Log("[WallRun][Player]Wall running on the right");
            }
            else
            {
                StopWallRun();
            }
        }
        else
        {
            StopWallRun();
        }

    }

    void StartWallRun()
    {
        //StopCoroutine(TimeScaleControl.instance.ActionE(0.5f));
        //StartCoroutine(TimeScaleControl.instance.ActionE(0.5f));
        rb.useGravity = false;

        rb.AddForce(Vector3.down * wallRunGravity, ForceMode.Force);

        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, wallRunfov, wallRunfovTime * Time.deltaTime);

        if (wallLeft)
            tilt = Mathf.Lerp(tilt, -camTilt, camTiltTime * Time.deltaTime);
        else if (wallRight)
            tilt = Mathf.Lerp(tilt, camTilt, camTiltTime * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (wallLeft)
            {
                Vector3 wallRunJumpDirection = transform.up + leftWallHit.normal;
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(wallRunJumpDirection * wallRunJumpForce * 100, ForceMode.Force);
            }
            else if (wallRight)
            {
                Vector3 wallRunJumpDirection = transform.up + rightWallHit.normal;
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(wallRunJumpDirection * wallRunJumpForce * 100, ForceMode.Force);

            }
        }

    }

    void StopWallRun()
    {

        rb.useGravity = true;
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fov, wallRunfovTime * Time.deltaTime);

        tilt = Mathf.Lerp(tilt, 0, camTiltTime * Time.deltaTime);

    }



}