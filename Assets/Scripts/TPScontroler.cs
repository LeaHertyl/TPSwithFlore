using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TPScontroler : MonoBehaviour
{
    [Header("Physics")]
    [SerializeField] private float gravity; //on declare une variable gravity de type float, serialisee pour y acceder dans l'inspector
    [SerializeField, Range(1, 10)] private float Jumpheight;
    [Header("Playermovement")]
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float Speed;
    [Header("Other")]
    [SerializeField] private Transform Feet;
    [Space]
    [SerializeField] private LayerMask raycastMask;

    private CharacterController controller; //on cree une variable controller de type CharacterController
    private Controls controls;

    private Camera mainCam;

    private Vector2 StickDirection;
    private bool IsJumping;

    private Vector3 direction3D;
    private Vector3 TotalMovement;


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
        mainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        TotalMovement = ApplyMove() + ApplyJump() + ApplyGravity();
        controller.Move(TotalMovement * Time.deltaTime); //on demande au controller de deplacer le Player en fonction de TotalMovement
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
        rotation *= Quaternion.Euler(0, mainCam.transform.rotation.eulerAngles.y, 0); //on ajoute a la rotation du joueur, la rotation en y de la camera
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
        //on dit que la rotation du joueur = Quaternion vu que c'est une rotation, RotateToward fait une animaition entre deux rotation de manière smooth, de la rotation dans laquelle on est à celle où on va * la vitesse de rotation
        //rotation de départ, rotation d'arrivée, vitesse de la rotation

        var MoveDirection = rotation * Vector3.forward; //il va se déplacer tout droit mais orienté selon la rotation
        return MoveDirection.normalized * Speed; //faudrait faire une variable sérialisée pour changer la vitesse du perso
    }

    private Vector3 ApplyGravity()
    {
        //var startraycastPos = transform.GetChild(0).position; //on recupere le transform.position du 1er enfant (0 = 1er enfant) de l'objet sur lequel est le script
        var startRaycastPos = Feet.position;
        var raycast = Physics.Raycast(startRaycastPos, Vector3.down, 0.1f, raycastMask); //point de depart, direction, taille (0.1f = 10cm), masque

        var DirectionToFall = Vector3.zero;

        if(raycast)
        {
            Debug.Log("touche");
            TotalMovement.y = 0;
        }
        else
        {
            Debug.Log("en l'air");
            DirectionToFall = new Vector3(0, TotalMovement.y + gravity * Time.deltaTime, 0); //on met time.deltatime ici en + de l'update parce qu'on veut par rapport au temps ecoule au carre (cette valeur doit etre mutliplie par elle meme pour appliquer la gravite)
        }

        return DirectionToFall;
    }

    private Vector3 ApplyJump()
    {
        if(!IsJumping || TotalMovement.y != 0) //on verifie si on est déjà en train de sauter ou si on est train de tomber
        {
            return Vector3.zero;
        }

        //vitesse = racine carre de (hauteur souhaitee x -2 x gravite)
        //la fonction Mathf.Sqrt() calcul pour nous la racine carree
        var heightSpeed = Mathf.Sqrt(Jumpheight * -2 * gravity);
        var JumpVector = new Vector3(0, heightSpeed, 0);
        return JumpVector;
    }

}