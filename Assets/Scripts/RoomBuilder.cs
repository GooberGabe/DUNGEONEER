using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBuilder : MonoBehaviour
{
    public Sprite icon;
    public string roomName;
    private void Start()
    {
        BuildRoom();
    }

    void BuildRoom()
    {
        MapGrid grid = GameManager.instance.GetGrid();

        foreach (Module child in GetComponentsInChildren<Module>())
        {
            GameObject tileObject = grid.GetTile(new Vector3(child.transform.position.x, 0, child.transform.position.z));

            if (tileObject != null)
            {
                tileObject.GetComponent<GridTile>().AddRoomModule(child.gameObject);
                child.transform.localPosition = Vector3.zero;
            }
        }

        GameManager.instance.RequestReload();

        Destroy(gameObject);
    }
}
