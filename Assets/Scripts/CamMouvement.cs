using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class CamMouvement : MonoBehaviour
{
    const int speed = 10;
    private Vector2 movement;
    [SerializeField] private Transform transform_bg4;
    [SerializeField] private Transform transform_bg3;
    [SerializeField] private Transform transform_bg2;
    [SerializeField] private Transform transform_bg1;
    [SerializeField] private Transform transform_bgm1;

    // Variables pour le screen shake
    private float shakeDuration = 0f;
    private float shakeAmount = 0.7f;
    private float decreaseFactor = 1.0f;
    private Vector3 originalPos;
    private Vector3 shakeOffset;

    // Variables pour le zoom
    private bool isZooming = false;
    private float originalSize;
    private Vector3 originalZoomPos;

    void Start()
    {
        originalPos = transform.localPosition;
        originalSize = Camera.main.orthographicSize;
    }

    void Update()
    {
        Vector3 toMove = speed * Time.deltaTime * new Vector3(movement.x, movement.y, 0);
        transform.Translate(toMove);

        // clamp to border
        if (transform.position.y < 0 || transform.position.y > 68) transform.Translate(-1 * toMove);
        if (transform.position.x < -25 || transform.position.x > 25)
        {
            transform.Translate(-1 * toMove);
        }
        else
        {
            // Background x parallax
            transform_bg4.Translate(new Vector3(toMove.x * 1.1f, 0, 0));
            transform_bg3.Translate(new Vector3(toMove.x * 1.075f, 0, 0));
            transform_bg2.Translate(new Vector3(toMove.x * 1.05f, 0, 0));
            transform_bg1.Translate(new Vector3(toMove.x * 1.025f, 0, 0));
            transform_bgm1.Translate(new Vector3(toMove.x, 0, 0));
        }

        // Screen shake logic
        if (shakeDuration > 0)
        {
            shakeOffset = Random.insideUnitSphere * shakeAmount;
            transform.localPosition = originalPos + shakeOffset;
            shakeDuration -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            shakeDuration = 0f;
            shakeOffset = Vector3.zero;
        }

        // Appliquez le mouvement de la caméra après le tremblement
        originalPos = transform.localPosition - shakeOffset;
    }

    public void Move(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();
    }

    // Méthode pour démarrer le tremblement
    public void StartShake(float duration, float intensity)
    {
        shakeDuration = duration;
        shakeAmount = intensity;
        originalPos = transform.localPosition;
    }

    // Méthode pour démarrer le zoom
    public void StartZoom(float zoomValue, Vector3 zoomPoint, float moveDuration, float holdDuration, bool comeBack)
    {
        if (!isZooming)
        {
            StartCoroutine(ZoomCoroutine(zoomValue, zoomPoint, moveDuration, holdDuration, comeBack));
        }
    }

    private IEnumerator ZoomCoroutine(float zoomValue, Vector3 zoomPoint, float moveDuration, float holdDuration, bool comeBack)
    {
        isZooming = true;
        float elapsedTime = 0f;
        originalZoomPos = new Vector3(transform.position.x, transform.position.y, -10);
        zoomPoint = new Vector3(zoomPoint.x, zoomPoint.y, -10);
        Vector3 targetZoomPos = zoomPoint;

        while (elapsedTime < moveDuration)
        {
            Camera.main.orthographicSize = Mathf.Lerp(originalSize, zoomValue, elapsedTime / moveDuration);
            transform.position = Vector3.Lerp(originalZoomPos, targetZoomPos, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Camera.main.orthographicSize = zoomValue;
        transform.position = targetZoomPos;

        yield return new WaitForSeconds(holdDuration);

        elapsedTime = 0f;
        while (elapsedTime < moveDuration)
        {
            Camera.main.orthographicSize = Mathf.Lerp(zoomValue, originalSize, elapsedTime / moveDuration);
            if(comeBack) transform.position = Vector3.Lerp(targetZoomPos, originalZoomPos, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Camera.main.orthographicSize = originalSize;
        if (comeBack) transform.position = originalZoomPos;
        isZooming = false;
    }
}
