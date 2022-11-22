using UnityEngine;

public class Fader : MonoBehaviour
{
    
    public Animator animator;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)){
            FadeToLevel(1);
        }
    }

    void Switch() {
        FadeToLevel(1);
    }

    public void FadeToLevel(int levelIndex) {
        animator.SetTrigger("FadeOut");
    }
}
