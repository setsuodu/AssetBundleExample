using UnityEngine;

public class DoubleClickShow : MonoBehaviour
{
    Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void OnGUI()
    {
        Event Mouse = Event.current;
        
        if (Mouse.isMouse && Mouse.type == EventType.MouseDown && Mouse.clickCount == 2)
        {
            animator.SetBool("toFire", true);
        }

        AnimatorStateInfo stateinfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateinfo.IsName("Base Layer.tripodIdle"))
        {

        }
        else if (stateinfo.IsName("Base Layer.tripodfire"))
        {
            animator.SetBool("toFire", false);
        }
    }
}
