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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector3(movement.x * speed * Time.deltaTime, movement.y * speed * Time.deltaTime, 0));
        // clamp y at least to 0
        if (transform.position.y < 0) transform.Translate(new Vector3(0, -movement.y * speed * Time.deltaTime, 0)); ;

        // Background x paralax
        transform_bg4.Translate(new Vector3(movement.x * speed * Time.deltaTime * 1.1f, 0, 0));
        transform_bg3.Translate(new Vector3(movement.x * speed * Time.deltaTime * 1.075f, 0, 0));
        transform_bg2.Translate(new Vector3(movement.x * speed * Time.deltaTime * 1.05f, 0, 0));
        transform_bg1.Translate(new Vector3(movement.x * speed * Time.deltaTime * 1.025f, 0, 0));

        if (transform.position.y < 0) transform.Translate(new Vector3(0, - movement.y * speed * Time.deltaTime, 0)); ;
    }

    public void Move(InputAction.CallbackContext context)
    {
         movement = context.ReadValue<Vector2>();    
        
    }
}
