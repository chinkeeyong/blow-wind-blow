using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class UIanim : MonoBehaviour
{
    public Animator animator;


    SerialPort sp = new SerialPort("COM3", 9600);

    // Start is called before the first frame update
    void Start()
    {
        sp.Open();
        sp.ReadTimeout = 1;
    }

    // Update is called once per frame
    void Update()
    {

        if (sp.IsOpen)
        {
            try
            {
                MoveObject(sp.ReadByte());
                print(sp.ReadByte());
            }

            catch (System.Exception)
            {

            }
        }
    }

    void MoveObject(int Direction)
    {
        if (Direction == 2)
        {
            animator.SetBool("IsMoving", true);
        }
        

        if (Direction == 1)
        {
            
        }
    }
}
