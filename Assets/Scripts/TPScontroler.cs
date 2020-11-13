using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TPScontroler : MonoBehaviour
{
    [SerializeField] private float gravity; //on declare une variable gravity de type float, serialisee pour y acceder dans l'inspector

    private CharacterController controller; //on cree une variable controller de type CharacterController
    private Controls controls;

    private Vector2 StickDirection;
    private bool IsJumping;

    private Vector3 direction3D;


    private void OnEnable()
    {
        controls = new Controls();
        controls.Enable();

        controls.Player.Move.performed += OnMovePerformed;
        controls.Player.Move.canceled += OnMoveCanceled;

        controls.Player.Jump.performed += OnJumpPerformed;
        controls.Player.Jump.canceled += OnJumpCanceled;
    }

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>(); //on vient recuperer le composant CharacterController du Player
    }

    // Update is called once per frame
    void Update()
    {
        var TotalMovement = ApplyMove() + ApplyJump() + ApplyGravity();
        controller.Move(TotalMovement * Time.deltaTime); //on demande au controller de deplacer le Player en fonction de DirectionToFall
    }


    private void OnMovePerformed(InputAction.CallbackContext obj)
    {
        StickDirection = obj.ReadValue<Vector2>();
        direction3D = new Vector3(StickDirection.x, 0, StickDirection.y); //mettre cette ligne dans cette fonction et non dans l'Update pour une question d'optimisation
    }

    private void OnMoveCanceled(InputAction.CallbackContext obj)
    {
        StickDirection = Vector2.zero;
        direction3D = Vector3.zero;
    }

    private void OnJumpPerformed(InputAction.CallbackContext obj)
    {
        //IsJumping = obj.ReadValueAsButton();
        IsJumping = true;
    }

    private void OnJumpCanceled(InputAction.CallbackContext obj)
    {
        IsJumping = false;
    }

    private Vector3 ApplyMove()
    {
        if(direction3D == Vector3.zero)
        {
            return Vector3.zero;
        }

        var rotation = Quaternion.LookRotation(direction3D);
        rotation *= Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0); //on ajoute a la rotation du joueur, la rotation en y de la camera
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 100 * Time.deltaTime);
        //on dit que la rotation du joueur = Quaternion vu que c'est une rotation, RotateToward fait une animaition entre deux rotation de manière smooth, de la rotation dans laquelle on est à celle où on va * la vitesse de rotation
        //rotation de départ, rotation d'arrivée, vitesse de la rotation

        var MoveDirection = rotation * Vector3.forward; //il va se déplacer tout droit mais orienté selon la rotation
        return MoveDirection.normalized * 25; //faudrait faire une variable sérialisée pour changer la vitesse du perso
    }

    private Vector3 ApplyGravity()
    {
        var DirectionToFall = new Vector3(0, gravity, 0);
        return DirectionToFall;
    }

    private Vector3 ApplyJump()
    {
        return Vector3.zero;
    }

}