using UnityEngine;

public class SceneManager : MonoBehaviour
{

    public CameraMovementManagerComponent CameraMovementManagerComponent;

    private CameraMovementManager CameraMovementManager;
    private StepManager StepManager;
    void Start()
    {
        Transform cameraPivotPoint = GameObject.FindGameObjectWithTag("CameraPivotPoint").transform;
        CameraMovementManager = new CameraMovementManager(cameraPivotPoint, CameraMovementManagerComponent);
        StepManager = new StepManager(this);
    }

    // Update is called once per frame
    void Update()
    {
        var deltaTime = Time.deltaTime;
        CameraMovementManager.Tick(deltaTime);
        StepManager.Tick(deltaTime);
    }

    #region Internal Events
    public void OnStepChange(int stepNb)
    {

        var stepTectObjects = GameObject.Find("Step" + (stepNb - 1) + "Text");
        var textparent = stepTectObjects.transform.parent;
        Destroy(stepTectObjects);
        var stepTextInstanciated = Instantiate(Resources.Load("Step" + stepNb + "Text"), textparent);
        stepTextInstanciated.name = "Step" + stepNb + "Text";

        var stepObject = GameObject.Find("Step" + stepNb);
        CameraMovementManager.OnStepChange(stepObject.transform);
    }
    #endregion
}


#region Camera
class CameraMovementManager
{
    private Transform cameraPivotPoint;
    private CameraMovementManagerComponent CameraMovementManagerComponent;

    public CameraMovementManager(Transform cameraPivotPoint, CameraMovementManagerComponent cameraMovementManagerComponent)
    {
        this.cameraPivotPoint = cameraPivotPoint;
        CameraMovementManagerComponent = cameraMovementManagerComponent;
    }

    private bool isTransitioning;
    private Transform newCameraPivotPointPosion;

    public void OnStepChange(Transform newCameraPivotPointPosion)
    {
        isTransitioning = true;
        this.newCameraPivotPointPosion = newCameraPivotPointPosion;
        cameraPivotPoint.localEulerAngles = Vector3.zero;
    }

    public void Tick(float d)
    {
        //rotation
        cameraPivotPoint.localEulerAngles += new Vector3(CameraMovementManagerComponent.CameraRotationSpeed * Input.GetAxis("Vertical"), CameraMovementManagerComponent.CameraRotationSpeed * Input.GetAxis("Horizontal"), 0);

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            cameraPivotPoint.localEulerAngles = Vector3.zero;

        }

        if (isTransitioning)
        {
            cameraPivotPoint.position = Vector3.Lerp(cameraPivotPoint.position, newCameraPivotPointPosion.position, d);
            if (Vector3.Distance(cameraPivotPoint.position, newCameraPivotPointPosion.position) <= 0.01)
            {
                cameraPivotPoint.position = newCameraPivotPointPosion.position;
                isTransitioning = false;
            }
        }
    }
}

[System.Serializable]
public class CameraMovementManagerComponent
{
    public float CameraSpeed;
    public float CameraRotationSpeed;
}
#endregion

#region Step Management
class StepManager
{
    private SceneManager SceneManager;

    public StepManager(SceneManager sceneManager)
    {
        SceneManager = sceneManager;
    }

    private int currentStep = 1;

    public void Tick(float d)
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnStepChanged();
        }
    }

    private void OnStepChanged()
    {
        currentStep += 1;
        SceneManager.OnStepChange(currentStep);
    }

}
#endregion