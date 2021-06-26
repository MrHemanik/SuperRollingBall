using ManageObjectScripts;
using UnityEngine;

namespace ObjectScripts.Buttons
{
    public class ButtonMoveOnActivation : MonoBehaviour
    {
        public GameObject[] buttons;
        public int puzzleID; //Startet bei Puzzle 1


        public void TestIfPuzzleIsSolved()
        {
            //Falls ein Knopf nicht gedrückt ist, wird activate auf false gesetzt
            bool activate = true;
            foreach (var button in buttons)
            {
                if (!button.GetComponent<ButtonActivateScript>().IsPressed()) activate = false;
            }

            if (activate) PuzzleSolved();
        }


        private void PuzzleSolved()
        {
            Debug.Log("Puzzle " + puzzleID + " gelöst");
            gameObject.transform.GetChild(0).GetComponent<Animator>()
                .SetInteger(Animator.StringToHash("PuzzleSolved"), puzzleID);
            GameManager.TriggerEvent("PuzzleSolved", puzzleID.ToString());
        }
    }
}