using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPScontroler : MonoBehaviour
{
    [SerializeField] private float gravity; //on declare une variable gravity de type float, serialisee pour y acceder dans l'inspector

    private CharacterController controller; //on cree une variable controller de type CharacterController

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>(); //on vient recuperer le composant CharacterController du Player
    }

    // Update is called once per frame
    void Update()
    {
        //Appliquer une gravite au Player
        var DirectionToFall = new Vector3(0, gravity, 0); //on applique la valeur de gravity uniquement sur l'axe y
        controller.Move(DirectionToFall * Time.deltaTime); //on demande au controller de deplacer le Player en fonction de DirectionToFall
    }
}
