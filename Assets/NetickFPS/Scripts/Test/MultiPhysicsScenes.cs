using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MultiPhysicsScenes : MonoBehaviour
{

    [Header("Object Position Controls")]
    public Vector3 ObjectPosition = new Vector3(0, 0, 1);

    [Header("Raycast Origin Controls")]
    public Vector3 RayOriginPosition = new Vector3(0, 0, 0);
    public Vector3 RayDirection = Vector3.forward;
    public float MaxDistance = 10;

    [Header("Name")]
    public string PhysicsSceneName = "AdditionalPhysicsScene";


    [Header("Debugging")]
    public PhysicsScene _physicsScene;
    public Scene _scene;
    public bool Hitting;

    public GameObject ClonedObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Creating scenes");

        _scene = SceneManager.CreateScene(PhysicsSceneName);

        _physicsScene = _scene.GetPhysicsScene();

        AddObjectToScene("Object ONE", ObjectPosition, Quaternion.identity, Vector3.one);
    }

    private void AddObjectToScene(string name, Vector3 position, Quaternion rotation, Vector3 localScale)
    {
        ClonedObject = new GameObject(name + " Cloned");
        ClonedObject.transform.position = position;
        ClonedObject.transform.rotation = rotation;
        ClonedObject.transform.localScale = localScale;
        ClonedObject.AddComponent<BoxCollider>();

        SceneManager.MoveGameObjectToScene(ClonedObject, _scene);
    }

    private void Update()
    {
        Hitting = _physicsScene.Raycast(RayOriginPosition, RayDirection, out RaycastHit hitInfo, MaxDistance);

        
    }
}
