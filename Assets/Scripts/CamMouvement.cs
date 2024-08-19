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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 toMove = new Vector3(movement.x, movement.y, 0) * speed * Time.deltaTime;
        transform.Translate(toMove);
        // clamp to border
        if (transform.position.y < 0 || transform.position.y > 68) transform.Translate(-1 * toMove);

        // Background x paralax
        transform_bg4.Translate(new Vector3(toMove.x * 1.1f, 0, 0));
        transform_bg3.Translate(new Vector3(toMove.x * 1.075f, 0, 0));
        transform_bg2.Translate(new Vector3(toMove.x * 1.05f, 0, 0));
        transform_bg1.Translate(new Vector3(toMove.x * 1.025f, 0, 0));
        transform_bgm1.Translate(new Vector3(toMove.x, 0, 0));


    }

    public void Move(InputAction.CallbackContext context)
    {
         movement = context.ReadValue<Vector2>();    
        
    }
}
