using UnityEngine;

namespace ObjectScripts
{
    public class ButtonMoveOnActivation : MonoBehaviour
    {
        public GameObject[] buttons;
        private static readonly int Start = Animator.StringToHash("Start");


        public void TestIfPuzzleIsSolved()
        {
            //Falls ein Knopf nicht gedrückt ist, wird activate auf false gesetzt
            bool activate = true;
            foreach (var button in buttons)
            {
                if (!button.GetComponent<ButtonActivateScript>().IsPressed()) activate = false;
            }
            if(activate) PuzzleSolved();
        }

                   
        
        

        private void PuzzleSolved()
        {
            gameObject.transform.GetChild(0).GetComponent<Animator>().SetTrigger(Start);
            GameManager.TriggerEvent("Puzzle1Solved");
        }
    }
}
