using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

public class TrailerManager : MonoBehaviour
{
    public GameObject vcam;
    private CinemachineVirtualCamera cinemachineVirtualCamera;

    [Header("Zoom")]
    public float endZoom;
    public float smooth;

    [Header("Rooms")]
    public int initialNumberRooms;
    public int incrementNumberRooms;


    // Start is called before the first frame update
    void Start()
    {
        cinemachineVirtualCamera = vcam.GetComponent<CinemachineVirtualCamera>();
        InvokeRepeating("ChangeDungeon", 2.0f, 2.0f);

        GameReferences.gameManager.initialNumberRooms = initialNumberRooms;
        GameReferences.gameManager.incrementNumberRooms = incrementNumberRooms;
        GameReferences.gameManager.numDungeons = 60;
    }

    // Update is called once per frame
    void Update()
    {
        cinemachineVirtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(cinemachineVirtualCamera.m_Lens.OrthographicSize, endZoom, Time.deltaTime * smooth);
    }

    void ChangeDungeon()
    {
        GameReferences.gameManager.ExitReached();
    }



}
