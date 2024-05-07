using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]              //The RequireComponent forces the GameObject to have an attached camera for the script to work
public class CameraBounds : MonoBehaviour
{
    public float minVisibleY;                   //Height minimum size position of the background layer
    public float maxVisibleY;                   //Height maximum size position of the background layer
    public float cameraHalfWidth;               //Represents the half width of the camera size
    public double cameraHalfHeight;             //Represents the half height of the camera size

    public Vector3 trans;                       //Stores the position of the Game Manager
    public float offsetx = 0;                   //Variable to slowly move camera over X to Spidy after a battle event
    public float offsety = 0;                   //Variable to slowly move camera when Spidy jumps in a second floor
    public Camera activeCamera;                 //Stores the reference of the scene´s Main Camera
    public Transform cameraRoot;                //The transform that will move through game to follow Spidy
    public Transform leftBounds;                //Wall left collider when a battle is ocurring
    public Transform rightBounds;               //Wall right collider when a battle is ocurring

    public bool waitInterpolation;
    public float cameraYPosition;

    void Start()
    {
        activeCamera = Camera.main;             //Attaches main camera to activeCamera variable

    }

    //-------------------------------------------------------------------------------------------------
    //After a battle event is complete, camera can follow over X to character again
    //and these instructions accomodate the camera slowly over X
    //-------------------------------------------------------------------------------------------------
    //Calculates the difference between the X setpoint and the current camera X position
    public void CalculateOffsetX(float actorPositionx)
    {
        offsetx = cameraRoot.position.x - actorPositionx;
        SetXPosition(actorPositionx, 0, 10000);
        StartCoroutine(EaseOffsetX());
    }
    //Calculates the difference between the X setpoint and the current camera X position
    //-------------------------------------------------------------------------------------------------

    //-------------------------------------------------------------------------------------------------
    //Slowly moves camera over X to reach X setpoint
    private IEnumerator EaseOffsetX()
    {
        while (offsetx != 0)
        {
            offsetx = Mathf.Lerp(offsetx, 0, 0.05f);
            if (Mathf.Abs(offsetx) < 0.05f)
            {
                offsetx = 0;
            }
            yield return new WaitForFixedUpdate();
        }
    }
    //Slowly moves camera over X to reach X set point
    //-------------------------------------------------------------------------------------------------
    //After a battle event is complete, camera can follow over X to character again
    //and these instructions accommodate the camera slowly over X
    //-------------------------------------------------------------------------------------------------

    //-------------------------------------------------------------------------------------------------
    //Once character is in a zone that needs to change of floor, then character changes floor
    //and these instructions accommodate the camera slowly over X
    //-------------------------------------------------------------------------------------------------
    //Calculates the difference between the X set point and the current camera X position
    public void CalculateOffsetY(float actorPositionY)
    {
        offsety = cameraYPosition - actorPositionY;
        cameraYPosition = actorPositionY;
        SetYPosition(actorPositionY);
        StartCoroutine(EaseOffsetY());
    }
    //Calculates the difference between the X set point and the current camera X position
    //-------------------------------------------------------------------------------------------------

    //-------------------------------------------------------------------------------------------------
    //Slowly moves camera over X to reach X set point
    private IEnumerator EaseOffsetY()
    {
        while (offsety != 0)
        {
            offsety = Mathf.Lerp(offsety, 0, 0.05f);
            if (Mathf.Abs(offsety) < 0.05f)
            {
                offsety = 0;
            }
            yield return new WaitForFixedUpdate();
        }
    }
    //Slowly moves camera over X to reach X setpoint
    //-------------------------------------------------------------------------------------------------
    //Once character is in a zone that needs to change of floor, then character changes floor
    //and these instructions accommodate the camera slowly over Y
    //-------------------------------------------------------------------------------------------------

    //-------------------------------------------------------------------------------------------------
    //Moves the game manager over X position based on the X reference without surpassing the min and max X values
    public void SetXPosition(float x, float minValueX, float maxValueX)
    {
        minValueX += cameraHalfWidth;
        maxValueX -= cameraHalfWidth;
        trans = cameraRoot.position;
        trans.x = Mathf.Clamp(x + offsetx, minValueX, maxValueX);
        cameraRoot.position = trans;
    }
    //Moves the game manager over X position based on the X reference without surpassing the min and max X values
    //-------------------------------------------------------------------------------------------------

    //-------------------------------------------------------------------------------------------------
    //Moves the game manager over Y position based on the Y reference without surpassing the min and max Y values
    public void SetYPosition(float y)
    {
        trans = cameraRoot.position;
        trans.y = cameraYPosition;
        trans.y = Mathf.Clamp(y + offsety, 0, 900);
        cameraRoot.position = trans;
    }
    //Moves the game manager over Y position based on the Y reference without surpassing the min and max Y values
    //-------------------------------------------------------------------------------------------------

    //-------------------------------------------------------------------------------------------------
    //Moves the game manager over Z position based on the Z reference without surpassing the min and max Z values
    public void SetZPosition(float z, float minZValue, float maxZValue)
    {
        trans = cameraRoot.position;
        trans.z = Mathf.Clamp(z, minZValue, maxZValue);
        cameraRoot.position = trans;
    }
    //Moves the game manager over Z position based on the Z reference without surpassing the min and max Z values
    //-------------------------------------------------------------------------------------------------

    //-------------------------------------------------------------------------------------------------
    //Calculates size of the camera in X and Y based on its area position
    public void AllignBoundaries()
    {
        cameraHalfWidth = Mathf.Abs(activeCamera.ScreenToWorldPoint(new Vector3(0, 0, 0)).x -
              activeCamera.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x) * 0.5f;
        cameraHalfHeight = System.Math.Round(Mathf.Sqrt(Mathf.Pow(Mathf.Abs(Camera.main.transform.position.y) -
            Mathf.Abs(activeCamera.ScreenToWorldPoint(new Vector3(0, 0, 0)).y), 2) +
            Mathf.Pow(Mathf.Abs(Camera.main.transform.position.z) -
            Mathf.Abs(activeCamera.ScreenToWorldPoint(new Vector3(0, 0, 0)).z), 2)), 1);
        //-------------------------------------------------------------------------------------------------
        //Accomodates Bounds according to size screen
        Vector3 position;
        position = leftBounds.transform.localPosition;
        position.x = transform.localPosition.x - cameraHalfWidth;
        leftBounds.transform.localPosition = position;
        position = rightBounds.transform.localPosition;
        position.x = transform.localPosition.x + cameraHalfWidth;
        rightBounds.transform.localPosition = position;
        //Accomodates Bounds according to size screen
        //-------------------------------------------------------------------------------------------------
    }
    //Calculates the center of the camera in X and Y based on its area position
    //-------------------------------------------------------------------------------------------------
}
