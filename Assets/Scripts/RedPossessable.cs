using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedPossessable : MonoBehaviour
{
    public bool isHighlighted = false;
    public bool isPossessed = false;

    public bool translateX = false;
    public bool translateY = false;
    public bool translateZ = false;

    public bool rotateX = false;
    public bool rotateY = false;
    public bool rotateZ = false;

    public bool resizeX = false;
    public bool resizeY = false;
    public bool resizeZ = false;

    public float ballSpeed;
    public float translationSpeed;
    public float rotationSpeed;
    public float resizeSpeed;

    public float minX;
    public float maxX;
    public float minY;
    public float maxY;
    public float minZ;
    public float maxZ;

    public Vector3 initialPos;
    public Vector3 initialScale;
    public Vector3 initialRot;

    public enum ManipulationType { Ball, Translatable, Rotatable, Resizable };
    public enum Shape { Sphere, Cube };
    public enum Hand { None, Left, Right };
    public ManipulationType manipulationType;
    public Shape shape;
    public Hand hand;
    public Material redMat;

    private void Start()
    {
        // get the initial position, scale, and rotation of the object
        initialPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        initialScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
        initialRot = new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

        switch (shape)
        {
            // if the object is a Sphere
            case Shape.Sphere:
                // add a Sphere to the Scene
                this.gameObject.AddComponent<MeshFilter>();
                GameObject goTempSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                GetComponent<MeshFilter>().mesh = goTempSphere.GetComponent<MeshFilter>().mesh;
                Destroy(goTempSphere);
                this.gameObject.AddComponent<MeshRenderer>();
                GetComponent<MeshRenderer>().material = redMat;
                this.gameObject.GetComponent<SphereCollider>().enabled = true;
                break;

            // if the object is a Cube
            case Shape.Cube:
                // add a Cube to the Scene
                this.gameObject.AddComponent<MeshFilter>();
                GameObject goTempCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                GetComponent<MeshFilter>().mesh = goTempCube.GetComponent<MeshFilter>().mesh;
                Destroy(goTempCube);
                this.gameObject.AddComponent<MeshRenderer>();
                GetComponent<MeshRenderer>().material = redMat;
                this.gameObject.GetComponent<BoxCollider>().enabled = true;
                break;

            default:
                break;
        }

        switch (manipulationType)
        {
            // if the object is a Ball
            case ManipulationType.Ball:
                // add a Rigidbody
                this.gameObject.AddComponent<Rigidbody>();
                break;

            case ManipulationType.Translatable:
                break;

            case ManipulationType.Rotatable:
                break;

            case ManipulationType.Resizable:
                break;
        }
    }

    /// <summary>
    /// Called when the object is possessed
    /// </summary>
    public void Possess()
    {
        isPossessed = true;

        switch (manipulationType)
        {
            case ManipulationType.Ball:
                Debug.Log("Ball Object Possessed");
                break;

            case ManipulationType.Translatable:
                Debug.Log("Translatable Object Possessed");
                break;

            case ManipulationType.Rotatable:
                Debug.Log("Rotatable Object Possessed");
                break;

            case ManipulationType.Resizable:
                Debug.Log("Resizable Object Possessed");
                break;

            default:
                Debug.Log("Red Object Possessed");
                break;
        }
    }

    /// <summary>
    /// Called when the object is unpossessed
    /// </summary>
    public void Unpossess()
    {
        isPossessed = false;
        hand = Hand.None;
    }

    private void Update()
    {
        // if the object is not possessed
        if (!isPossessed)
        {
            // if the object is being highlighted
            if (isHighlighted)
            {
                // make the object glow
                GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
            }

            // if the object is not being highlighted
            else
            {
                // make the object stop glowing
                GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");
            }
        }

        // if the object is possessed
        else
        {
            // make the object glow
            GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
        }
    }
}
