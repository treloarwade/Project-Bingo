using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class GoInside : MonoBehaviour
{
    public GameObject door;
    public Vector2 location;
    public Vector2 cameraLocation;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!GoInAndOut.Instance.transitioning)
        {
            if (collision.CompareTag("Player") &&
    collision.TryGetComponent(out NetworkBehaviour nb) &&
    nb.IsLocalPlayer)
            {
                StartCoroutine(GoingInside(collision.transform));
            }
        }
    }
    IEnumerator GoingInside(Transform player)
    {
        door.SetActive(true);
        GoInAndOut.Instance.transitioning = true;
        float duration = 0.2f;
        float elapsed = 0f;

        Vector3 originalposition = player.position;
        Vector3 enterposition = door.transform.position;
        enterposition.y -= 0.3f;
        // Move trainer toward the target
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            player.position = Vector3.Lerp(originalposition, enterposition, t);


            elapsed += Time.deltaTime;
            yield return null;
        }
        elapsed = 0f;
        while (elapsed < 1f)
        {
            float t = elapsed / duration;
            player.position = Vector3.Lerp(enterposition, door.transform.position, t);


            elapsed += Time.deltaTime;
            yield return null;
        }
        player.position = location;
        CameraFollow.Instance.smoothTime = 0f;
        CameraFollow.Instance.battleActive = true;
        CameraFollow.Instance.battlePosition = cameraLocation;
        GoInAndOut.Instance.transitioning = false;
        door.SetActive(false);
        yield return null;
    }
}
