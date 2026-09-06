using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GO : MonoBehaviour
{
    public lC fad1;
    public lC fad2;
    public lC fad3;
    public lC fad4;
    public float bug;
    private void Awake()
    {
        bug = 0;
    }
    public void Update()
    {
        if(fad1.IsGrounded&&fad2.IsGrounded&&fad3.IsGrounded&&fad4.IsGrounded)
        {
            Debug.Log("F1");
         

        }
        else
        {
           bug++;
            if (bug > 1)
            {
                //»ç¸Á
            }
        }
    }
}
